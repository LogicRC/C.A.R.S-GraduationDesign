//================================================================================================================================
//
//  Copyright (c) 2015-2023 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
//  EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
//  and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
//
//================================================================================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace easyar
{
    /// <summary>
    /// <para xml:lang="en"><see cref="MonoBehaviour"/> which controls <see cref="InputFramePlayer"/> in the scene, providing a few extensions in the Unity environment. There is no need to use <see cref="InputFramePlayer"/> directly.</para>
    /// <para xml:lang="zh">在场景中控制<see cref="InputFramePlayer"/>的<see cref="MonoBehaviour"/>，在Unity环境下提供功能扩展。不需要直接使用<see cref="InputFramePlayer"/>。</para>
    /// </summary>
    public class FramePlayer : FrameSource
    {
        /// <summary>
        /// <para xml:lang="en">File path type. Set before OnEnable or <see cref="ARSession.Start"/>.</para>
        /// <para xml:lang="zh">路径类型。可以在OnEnable或<see cref="ARSession.Start"/>之前设置。</para>
        /// </summary>
        public WritablePathType FilePathType;

        /// <summary>
        /// <para xml:lang="en">File path. Set before OnEnable or <see cref="ARSession.Start"/>.</para>
        /// <para xml:lang="zh">文件路径。可以在OnEnable或<see cref="ARSession.Start"/>之前设置。</para>
        /// </summary>
        public string FilePath = string.Empty;

        internal FrameMetaSource MetaSource;

        private static IReadOnlyList<ARSession.ARCenterMode> availableCenterMode = new List<ARSession.ARCenterMode> { ARSession.ARCenterMode.FirstTarget, ARSession.ARCenterMode.Camera, ARSession.ARCenterMode.SpecificTarget };
        /// <senseapi/>
        private InputFramePlayer player;
        private bool isStarted;
        private bool isPaused;
        private DisplayEmulator display;
        private bool assembled;
        private bool disableAutoPlay;
        [SerializeField, HideInInspector]
        private WorldRootController worldRoot;
        private WorldRootController worldRootCache;
        private GameObject worldRootObject;
        private bool hasSpatialInfo;
        private bool isMetaSourceRequired;
        private Optional<InputFrameSourceType> type = Optional<InputFrameSourceType>.Empty;

        internal event Action<InputFrameSourceType> TypeChange;

        public override Optional<InputFrameSourceType> Type { get => type; }

        public override Optional<bool> IsAvailable { get => true; }

        public override IReadOnlyList<ARSession.ARCenterMode> AvailableCenterMode { get => availableCenterMode; }

        /// <summary>
        /// <para xml:lang="en"> Whether the playback is completed.</para>
        /// <para xml:lang="zh"> 是否已完成播放。</para>
        /// </summary>
        public bool IsCompleted
        {
            get
            {
                if (isStarted)
                {
                    return player.isCompleted();
                }
                return false;
            }
        }

        /// <summary>
        /// <para xml:lang="en"> Total expected playback time. The unit is second.</para>
        /// <para xml:lang="zh"> 预期的总播放时间。单位为秒。</para>
        /// </summary>
        public float Length
        {
            get
            {
                if (isStarted)
                {
                    return (float)player.totalTime();
                }
                return 0;
            }
        }

        /// <summary>
        /// <para xml:lang="en"> Current time played.</para>
        /// <para xml:lang="zh"> 已经播放的时间。</para>
        /// </summary>
        public float Time
        {
            get
            {
                if (isStarted)
                {
                    return (float)player.currentTime();
                }
                return 0;
            }
        }

        /// <summary>
        /// <para xml:lang="en">The object Camera move against, will be automatically get from the scene or generate if not set.</para>
        /// <para xml:lang="zh">相机运动的相对物体，如果没设置，将会自动从场景中获取或生成。</para>
        /// </summary>
        public WorldRootController WorldRoot
        {
            get => worldRoot;
            set
            {
                if (assembled) { return; }
                worldRoot = value;
            }
        }

        public override GameObject Origin { get => hasSpatialInfo && worldRoot ? worldRoot.gameObject : null; }

        internal IDisplay Display
        {
            get { return display; }
        }

        protected override void Awake()
        {
            base.Awake();
            if (!EasyARController.Initialized)
            {
                return;
            }
            if (!worldRoot) { worldRootCache = FindObjectOfType<WorldRootController>(); }
            MetaSource = new FrameMetaSource();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (player != null && isStarted && !isPaused)
            {
                if (isMetaSourceRequired) { MetaSource.Resume(); }
                player.resume();
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (player != null && isStarted && !isPaused)
            {
                if (isMetaSourceRequired) { MetaSource.Pause(); }
                player.pause();
            }
        }

        private void OnApplicationPause(bool pause)
        {
            if (isMetaSourceRequired) { MetaSource.OnApplicationPause(pause); }
        }

        protected virtual void OnDestroy()
        {
            MetaSource?.Dispose();
            if (player != null)
            {
                player.Dispose();
            }
            if (worldRootObject) Destroy(worldRootObject);
        }

        public override void OnAssemble(ARSession session)
        {
            base.OnAssemble(session);
            isMetaSourceRequired = session.Assembly.FrameFilters.Where(f => f is MegaTrackerFrameFilter).Any();
            StartCoroutine(AutoPlay());
            assembled = true;
        }

        /// <summary>
        /// <para xml:lang="en">Start eif file playback.</para>
        /// <para xml:lang="zh">播放eif文件。</para>
        /// </summary>
        public bool Play()
        {
            disableAutoPlay = true;
            isPaused = false;

            if (!isStarted)
            {
                var path = FilePath;
                if (FilePathType == WritablePathType.PersistentDataPath)
                {
                    path = Application.persistentDataPath + "/" + path;
                }
                if (player == null)
                {
                    player = InputFramePlayer.create();
                    if (sink != null)
                    {
                        player.output().connect(sink);
                    }
                }
                if (isMetaSourceRequired)
                {
                    var metaPath = path + ".json";
                    MetaSource.Start(metaPath, () => { return Time; });
                    type = MetaSource.Type;
                    TypeChange?.Invoke(type.Value);
                }
                isStarted = player.start(path);
                if (isStarted)
                {
                    display = new DisplayEmulator();
                    display.EmulateRotation(player.initalScreenRotation());
                }
                else
                {
                    GUIPopup.EnqueueMessage(typeof(FramePlayer) + " fail to start with file: " + path, 5);
                }
            }
            if (enabled)
            {
                OnEnable();
            }
            return isStarted;
        }

        /// <summary>
        /// <para xml:lang="en">Stop eif file playback.</para>
        /// <para xml:lang="zh">停止播放eif文件。</para>
        /// </summary>
        public void Stop()
        {
            disableAutoPlay = true;
            isStarted = false;
            isPaused = false;
            display = null;
            OnDisable();
            if (isMetaSourceRequired) { MetaSource.Stop(); }
            if (player != null)
            {
                player.stop();
            }
        }

        /// <summary>
        /// <para xml:lang="en">Pause eif file playback.</para>
        /// <para xml:lang="zh">暂停播放eif文件。</para>
        /// </summary>
        public void Pause()
        {
            if (isStarted)
            {
                isPaused = true;
                if (isMetaSourceRequired) { MetaSource.Pause(); }
                player.pause();
            }
        }

        public override void Connect(InputFrameSink val)
        {
            base.Connect(val);
            if (player != null)
            {
                player.output().connect(val);
            }
        }

        /// <summary>
        /// <para xml:lang="en" access="internal">WARNING: Designed for internal tools only. Do not use this interface in your application. Accessibility Level may change in future.</para>
        /// <para xml:lang="zh" access="internal">警告：仅用于内部工具。不要在应用开发中使用这个接口。可访问级别可能会在未来产生变化。</para>
        /// </summary>
        public IEnumerator PreLoadFrameMeta(Action callback)
        {
            if (!isMetaSourceRequired)
            {
                callback?.Invoke();
                yield break;
            }

            var path = FilePath;
            if (FilePathType == WritablePathType.PersistentDataPath)
            {
                path = Application.persistentDataPath + "/" + path;
            }
            var metaPath = path + ".json";
            if (!File.Exists(metaPath))
            {
                throw new FileNotFoundException(metaPath);
            }
            Debug.Log($"loading {metaPath}...");

            bool taskFinished = false;
            EasyARController.Instance.Worker.Run(() =>
            {
                MetaSource.Load(metaPath);
                taskFinished = true;
            });

            while (!taskFinished)
            {
                yield return 0;
            }
            Debug.Log($"load completed");
            callback?.Invoke();
        }

        internal void RequireSpatial()
        {
            SetupOriginUsingWorldRoot();
        }

        private IEnumerator AutoPlay()
        {
            while (!enabled)
            {
                if (disableAutoPlay) { yield break; }
                yield return null;
            }
            if (disableAutoPlay) { yield break; }
            Play();
        }

        private void SetupOriginUsingWorldRoot()
        {
            hasSpatialInfo = true;
            availableCenterMode = allCenterMode;
            if (worldRoot) { return; }
            worldRoot = worldRootCache;
            if (worldRoot) { return; }
            worldRoot = FindObjectOfType<WorldRootController>();
            if (worldRoot) { return; }
            Debug.Log($"WorldRoot not found, create from {typeof(ARCoreFrameSource)}");
            worldRootObject = new GameObject("WorldRoot");
            worldRoot = worldRootObject.AddComponent<WorldRootController>();
        }

        internal class FrameMetaSource : IDisposable
        {
            private FrameMetaSlice slice;
            private string file;
            private bool isManualPaused;
            private bool isSystemPaused;
            private InputFrameSourceType type = InputFrameSourceType.General;
            private Thread thread;
            private bool finished;
            private Func<float> time;
            private int index;
            private double startTimestamp;

            public event Action<AccelerometerResult> AccelerometerUpdate;
            public event Action<LocationResult> LocationUpdate;

            public InputFrameSourceType Type { get => type; }

            ~FrameMetaSource()
            {
                Finish();
            }

            public void Dispose()
            {
                Finish();
                GC.SuppressFinalize(this);

            }
            public void Load(string path)
            {
                if (!File.Exists(path))
                {
                    throw new FileNotFoundException(path);
                }
                if (file == path) { return; }
                var data = File.ReadAllText(path);
                slice = JsonUtility.FromJson<FrameMetaSlice>(data);
                if (slice.device == null || slice.application == null || string.IsNullOrEmpty(slice.device.vioDevice) || string.IsNullOrEmpty(slice.application.platform)) { throw new InvalidDataException("Error decode frame source type from: " + path); }
                if (slice.device.vioDevice == "MotionTracker")
                {
                    type = InputFrameSourceType.MotionTracker;
                }
                else if (slice.device.vioDevice == "ARCore")
                {
                    type = InputFrameSourceType.ARCore;
                }
                else if (slice.device.vioDevice == "ARKit")
                {
                    type = InputFrameSourceType.ARKit;
                }
                else if (slice.device.vioDevice == "HuaweiAREngine")
                {
                    type = InputFrameSourceType.AREngine;
                }
                else if (slice.device.vioDevice == "AREngine")
                {
                    type = InputFrameSourceType.AREngine;
                }
                else if (slice.device.vioDevice == "CameraDevice" || slice.device.vioDevice == "Nreal")
                {
                    type = InputFrameSourceType.General;
                }
                else if (slice.device.vioDevice == "ARFoundation")
                {
                    if (slice.application.platform == "Android")
                    {
                        type = InputFrameSourceType.ARCore;
                    }
                    else if (slice.application.platform == "IPhonePlayer")
                    {
                        type = InputFrameSourceType.ARKit;
                    }
                    else
                    {
                        type = InputFrameSourceType.General;
                    }
                }
                else
                {
                    throw new InvalidDataException($"Error decode VIO device type '{slice.device.vioDevice}' from: {path}");
                }
                file = path;
            }

            public void Start(string path, Func<float> timeFunc)
            {
                Load(path);

                if (slice == null || slice.deviceInput == null || slice.deviceInput.Count <= 0)
                {
                    throw new InvalidOperationException("no data");
                }
                time = timeFunc;

                Finish();
                index = 0;
                startTimestamp = slice.deviceInput[0].frameTimestamp;
                isManualPaused = false;
                isSystemPaused = false;
                finished = false;
                thread = new Thread(() =>
                {
                    while (!finished)
                    {
                        float t = 0;
                        Monitor.Enter(time);
                        try
                        {
                            while (!finished && (isManualPaused || isSystemPaused))
                            {
                                Monitor.Wait(time);
                            }
                            t = time();
                        }
                        finally
                        {
                            Monitor.Exit(time);
                        }

                        try
                        {
                            if (!finished)
                            {
                                for (var i = index; i < slice.deviceInput.Count; ++i)
                                {
                                    if (slice.deviceInput[i].frameTimestamp - startTimestamp <= t)
                                    {
                                        if (slice.deviceInput[i].accelerometer != null)
                                        {
                                            var data = slice.deviceInput[i].accelerometer;
                                            AccelerometerUpdate?.Invoke(new AccelerometerResult(data.x, data.y, data.z, data.timestamp));
                                        }
                                        if (slice.deviceInput[i].location != null)
                                        {
                                            var data = slice.deviceInput[i].location;
                                            LocationUpdate?.Invoke(new LocationResult(data.latitude, data.longitude, data.altitude, 0, 0, true, false, false));
                                        }
                                        index++;
                                    }
                                    else
                                    {
                                        Thread.Sleep((int)((slice.deviceInput[i].frameTimestamp - startTimestamp - t) * 1000));
                                        break;
                                    }
                                }

                                if (index >= slice.deviceInput.Count) { break; }
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError(ex.ToString());
                        }
                    }
                });
                thread.Start();
            }

            public void Stop()
            {
                Finish();
            }

            public void Pause()
            {
                Monitor.Enter(time);
                try
                {
                    isManualPaused = true;
                }
                finally
                {
                    Monitor.Exit(time);
                }
            }

            public void Resume()
            {
                Monitor.Enter(time);
                try
                {
                    if (!isManualPaused) { return; }
                    isManualPaused = false;
                    if (isSystemPaused) { return; }
                    Monitor.PulseAll(time);
                }
                finally
                {
                    Monitor.Exit(time);
                }
            }

            public void OnApplicationPause(bool pause)
            {
                if (pause)
                {
                    Monitor.Enter(time);
                    try
                    {
                        isSystemPaused = true;
                    }
                    finally
                    {
                        Monitor.Exit(time);
                    }
                }
                else
                {
                    Monitor.Enter(time);
                    try
                    {
                        if (!isSystemPaused) { return; }
                        isSystemPaused = false;
                        if (isManualPaused) { return; }
                        Monitor.PulseAll(time);
                    }
                    finally
                    {
                        Monitor.Exit(time);
                    }
                }
            }

            private void Finish()
            {
                if (thread == null || !thread.IsAlive)
                {
                    return;
                }

                Monitor.Enter(time);
                try
                {
                    finished = true;
                    isSystemPaused = false;
                    isManualPaused = false;
                    Monitor.PulseAll(time);
                }
                finally
                {
                    Monitor.Exit(time);
                }
                thread.Join();
            }

            [Serializable]
            private class FrameMetaSlice
            {
                public Application application = default;
                public Device device = default;
                public List<DeviceInput> deviceInput = default;

                [Serializable]
                public class Application
                {
                    public string platform = default;
                }

                [Serializable]
                public class Device
                {
                    public string vioDevice = default;
                }

                [Serializable]
                public class DeviceInput
                {
                    public double frameTimestamp = default;
                    public AccelerometerResult accelerometer = default;
                    public LocationResult location = default;

                    [Serializable]
                    public class AccelerometerResult
                    {
                        public float x = default;
                        public float y = default;
                        public float z = default;
                        public double timestamp = default;
                    }

                    [Serializable]
                    public class LocationResult
                    {
                        public double latitude = default;
                        public double longitude = default;
                        public double altitude = default;
                    }
                }
            }
        }
    }
}
