//================================================================================================================================
//
//  Copyright (c) 2015-2023 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
//  EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
//  and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
//
//================================================================================================================================

#if EASYAR_ENABLE_MEGA
using EasyAR.Mega.Scene;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif

namespace easyar
{
    /// <summary>
    /// <para xml:lang="en"><see cref="MonoBehaviour"/> which controls <see cref="MegaTracker"/> in the scene, providing a few extensions in the Unity environment. Use <see cref="Tracker"/> directly when necessary.</para>
    /// <para xml:lang="zh">在场景中控制<see cref="MegaTracker"/>的<see cref="MonoBehaviour"/>，在Unity环境下提供功能扩展。如有需要可以直接使用<see cref="Tracker"/>。</para>
    /// </summary>
#if EASYAR_ENABLE_MEGA
    [RequireComponent(typeof(BlockHolder))]
#endif
    public class MegaTrackerFrameFilter : FrameFilter, FrameFilter.IInputFrameSink, FrameFilter.IOutputFrameSource
    {
        /// <summary>
        /// <para xml:lang="en">EasyAR Sense API. Accessible when <see cref="ARSession.State"/> &gt; <see cref="ARSession.SessionState.Ready"/> if available.</para>
        /// <para xml:lang="zh">EasyAR Sense API，如果功能可以使用，可以在<see cref="ARSession.State"/> &gt; <see cref="ARSession.SessionState.Ready"/>时访问。</para>
        /// </summary>
        /// <senseapi/>
        public MegaTracker Tracker { get; private set; }

        /// <summary>
        /// <para xml:lang="en" access="internal">WARNING: Designed for internal tools only. Do not use this interface in your application. Accessibility Level may change in future.</para>
        /// <para xml:lang="zh" access="internal">警告：仅用于内部工具。不要在应用开发中使用这个接口。可访问级别可能会在未来产生变化。</para>
        /// </summary>
        /// <senseapi/>
        public Accelerometer Accelerometer { get; private set; }

        /// <summary>
        /// <para xml:lang="en">Use global service config or not. The global service config can be changed on the inspector after click Unity menu EasyAR -> Sense -> Configuration.</para>
        /// <para xml:lang="zh">是否使用全局服务器配置。全局配置可以点击Unity菜单EasyAR -> Sense -> Configuration后在属性面板里面进行填写。</para>
        /// </summary>
        public bool UseGlobalServiceConfig = true;

        /// <summary>
        /// <para xml:lang="en">Service config when <see cref="UseGlobalServiceConfig"/> == false, only valid for this object.</para>
        /// <para xml:lang="zh"><see cref="UseGlobalServiceConfig"/> == false时使用的服务器配置，只对该物体有效。</para>
        /// </summary>
        [HideInInspector, SerializeField]
        public MegaLocalizationServiceConfig ServiceConfig = new MegaLocalizationServiceConfig();
        /// <summary>
        /// <para xml:lang="en">Fallback options mainly used for develop and debug.</para>
        /// <para xml:lang="zh">退化选项，主要用于开发和调试。</para>
        /// </summary>
        [HideInInspector, SerializeField]
        public FallbackOptions Fallbacks = new FallbackOptions();

        [HideInInspector, SerializeField]
        private TimeParameters requestTimeParameters = new TimeParameters();
        private ResultPoseTypeParameters resultPoseType = new ResultPoseTypeParameters();
        private ARSession arSession;
        private bool isStarted;
        private string requestMessage;
        /// <senseapi/>
        private LocationResultSink locationResultSink;
        /// <senseapi/>
        private AccelerometerResultSink accelerometerResultSink;
        /// <senseapi/>
        private CloudLocalizer cloudLocalizer;
        /// <senseapi/>
        private ProximityLocationResultSink proximityLocationResultSink;
        private Optional<ProximityLocationResult> proximityLocation;

#if EASYAR_ENABLE_MEGA
        private Optional<Pose> cameraToVIOOrigin;
        private Request request = new Request();

        /// <summary>
        /// <para xml:lang="en">CloudLocalization service callback event. This event is usually used for debug, objects transform and status in the scene does not change with event data in the same time the event is triggered.</para>
        /// <para xml:lang="zh">服务定位返回事件。该事件通常用作debug，事件发生时场景中物体的位置和状态与事件中的数据无对应关系。</para>
        /// </summary>
        public event Action<LocalizationResponse> LocalizationUpdate;

        /// <summary>
        /// <para xml:lang="en" access="internal">WARNING: Designed for internal tools only. Do not use this interface in your application. Accessibility Level may change in future.</para>
        /// <para xml:lang="zh" access="internal">警告：仅用于内部工具。不要在应用开发中使用这个接口。可访问级别可能会在未来产生变化。</para>
        /// </summary>
        public event Action<MegaTrackerLocalizationResponse> LocalizationUpdateRaw;

        /// <summary>
        /// <para xml:lang="en">The block holder which holds and manages blocks in the scene.</para>
        /// <para xml:lang="zh">持有Block的组件，在场景中持有并管理Block。</para>
        /// </summary>
        public BlockHolder MapHolder { get; private set; }
        /// <summary>
        /// <para xml:lang="en" access="internal">WARNING: Designed for internal tools only. Do not use this interface in your application. Accessibility Level may change in future.</para>
        /// <para xml:lang="zh" access="internal">警告：仅用于内部工具。不要在应用开发中使用这个接口。可访问级别可能会在未来产生变化。</para>
        /// </summary>
        public LocationManager LocationManager { get; private set; }
#endif

        public override int BufferRequirement
        {
            get { return Tracker.bufferRequirement(); }
        }

        /// <summary>
        /// <para xml:lang="en">Request time parameters.</para>
        /// <para xml:lang="zh">请求时间参数。</para>
        /// </summary>
        public TimeParameters RequestTimeParameters
        {
            get => requestTimeParameters;
            set
            {
                requestTimeParameters = value;
                Tracker?.setRequestTimeParameters(value.Timeout, value.RequestInterval);
            }
        }

        /// <summary>
        /// <para xml:lang="en">Result pose parameters.</para>
        /// <para xml:lang="zh">结果姿态类型参数。</para>
        /// </summary>
        public ResultPoseTypeParameters ResultPoseType
        {
            get => resultPoseType;
            set
            {
                resultPoseType = value;
                Tracker?.setResultPoseType(value.EnableLocalization, value.EnableStabilization);
            }
        }

        /// <summary>
        /// <para xml:lang="en">Proximity location result.</para>
        /// <para xml:lang="zh">邻近位置结果。</para>
        /// </summary>
        public Optional<ProximityLocationResult> ProximityLocation
        {
            private get => proximityLocation;
            set
            {
                proximityLocation = value;
                if (proximityLocation.OnSome)
                {
                    HandleProximityLocation(proximityLocation.Value);
                }
            }
        }

        /// <summary>
        /// <para xml:lang="en" access="internal">WARNING: Designed for internal tools only. Do not use this interface in your application. Accessibility Level may change in future.</para>
        /// <para xml:lang="zh" access="internal">警告：仅用于内部工具。不要在应用开发中使用这个接口。可访问级别可能会在未来产生变化。</para>
        /// </summary>
        public string RequestMessage
        {
            get => requestMessage;
            set
            {
                requestMessage = value;
                Tracker?.setRequestMessage(value ?? string.Empty);
            }
        }

        protected virtual void Awake()
        {
#if EASYAR_ENABLE_MEGA
            MapHolder = gameObject.GetComponent<BlockHolder>();
            if (!MapHolder)
            {
                MapHolder = gameObject.AddComponent<BlockHolder>();
            }
#else
            Debug.LogWarning($"Package com.easyar.mega is required to use {nameof(MegaTrackerFrameFilter)}");
#endif
        }

#if EASYAR_ENABLE_MEGA
        protected virtual void OnEnable()
        {
            if (isStarted)
            {
                if (LocationManager)
                {
                    LocationManager.enabled = true;
                }
                Accelerometer?.openWithSamplingPeriod(100);
                Tracker?.start();
            }
        }

        protected virtual void OnDisable()
        {
            if (LocationManager)
            {
                LocationManager.enabled = false;
            }
            Tracker?.stop();
            Accelerometer?.close();
        }

        protected virtual void OnDestroy()
        {
            accelerometerResultSink?.Dispose();
            locationResultSink?.Dispose();
            proximityLocationResultSink?.Dispose();
            Tracker?.close();
            Tracker?.Dispose();
            Accelerometer?.Dispose();
            cloudLocalizer?.Dispose();
        }
#endif

        public InputFrameSink InputFrameSink()
        {
            return Tracker?.inputFrameSink();
        }

        public OutputFrameSource OutputFrameSource()
        {
            return Tracker?.outputFrameSource();
        }

        public override void OnAssemble(ARSession session)
        {
            base.OnAssemble(session);
            arSession = session;
#if EASYAR_ENABLE_MEGA

            if (!MegaTracker.isAvailable()) { throw new UIPopupException(typeof(MegaTracker) + " not available"); }

            var config = new MegaLocalizationServiceConfig();
            if (UseGlobalServiceConfig)
            {
                if (EasyARSettings.Instance)
                {
                    config = EasyARSettings.Instance.GlobalMegaLocalizationServiceConfig;
                }
            }
            else
            {
                config = ServiceConfig;
            }
            NotifyEmptyConfig(config);
            try
            {
                Tracker = MegaTracker.create(config.ServerAddress.Trim(), config.APIKey.Trim(), config.APISecret.Trim(), config.AppID.Trim());
            }
            catch (ArgumentNullException)
            {
                throw new UIPopupException($"Fail to create {nameof(MegaTracker)}, check logs for detials.");
            }

            locationResultSink = Tracker.locationResultSink();
            accelerometerResultSink = Tracker.accelerometerResultSink();
            if (ProximityLocation.OnSome)
            {
                HandleProximityLocation(ProximityLocation.Value);
            }
            Tracker.setRequestTimeParameters(RequestTimeParameters.Timeout, RequestTimeParameters.RequestInterval);
            Tracker.setResultPoseType(ResultPoseType.EnableLocalization, ResultPoseType.EnableStabilization);
            Tracker.setRequestMessage(RequestMessage ?? string.Empty);
            if (arSession.Assembly.FrameSource.Type.OnSome)
            {
                Tracker.setInputFrameSourceType(arSession.Assembly.FrameSource.Type.Value);
            }
            else
            {
                StartCoroutine(GetFrameSourceType());
            }
            Tracker.setLocalizationCallback(EasyARController.Scheduler, (Action<MegaTrackerLocalizationResponse>)((response) =>
            {
                var blocks = new List<BlockController>();
                foreach (var instance in response.instances())
                {
                    using (instance)
                    {
                        blocks.Add(MapHolder.OnLocalize(new BlockController.BlockInfo
                        {
                            ID = instance.blockId(),
                            Name = instance.name(),
                        }));
                    }
                }
                var timestamp = 0.0;
                using (var iFrame = response.inputFrame())
                {
                    timestamp = iFrame.timestamp();
                }
                LocalizationUpdate?.Invoke(new LocalizationResponse
                {
                    Timestamp = timestamp,
                    Status = response.status(),
                    Blocks = blocks,
                    ServerCalculationDuration = response.serverCalculationDuration(),
                    ServerResponseDuration = response.serverResponseDuration(),
                    ErrorMessage = response.errorMessage(),
                });
                LocalizationUpdateRaw?.Invoke(response);
            }));
            if (arSession.Assembly.FrameSource is FramePlayer player)
            {
                player.TypeChange += (type) =>
                {
                    Tracker?.setInputFrameSourceType(type);
                };
                player.MetaSource.AccelerometerUpdate += (data) =>
                {
                    accelerometerResultSink?.handle(data);
                    request.AccelerometerResult = data;
                };
                player.MetaSource.LocationUpdate += (data) =>
                {
                    locationResultSink?.handle(data);
                    request.LocationResult = data;
                };
            }
            else
            {
                if (!arSession.Assembly.FrameSource.IsHMD)
                {
                    Accelerometer = new Accelerometer();
                    if (!Accelerometer.isAvailable())
                    {
                        Accelerometer.Dispose();
                        Accelerometer = null;
                    }
                    Accelerometer?.output().connect(accelerometerResultSink);
                }
                if (Fallbacks.AllowNonEifRemote)
                {
                    GUIPopup.EnqueuePersistentWarning("Mega is running defectively for non-eif remote test." + Environment.NewLine +
                        "  For real Mega experience, please" + Environment.NewLine +
                        "    1) Turn off fallback option `AllowNonEifRemote` when you are on spot, or" + Environment.NewLine +
                        $"    2) Use {nameof(FramePlayer)} to play EIF data for remote test." + Environment.NewLine +
                        "  EasyAR official apps (developer tools) may display this on purpose.");
                }
                else
                {
                    if (Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer)
                    {
                        throw new UIPopupException($"Fallback option `AllowNonEifRemote` is requrired under {Application.platform} if not using {nameof(FramePlayer)} to play EIF data.");
                    }
                    LocationManager = gameObject.AddComponent<LocationManager>();
                    RequestLocationPermission();
                    StartCoroutine(HandleLocation());
                }
            }
            if (Fallbacks.AllowNoTracking)
            {
                arSession.FrameUpdate += CheckFrame;
            }
            else
            {
                if (!(arSession.Assembly.FrameSource is FramePlayer) && Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer)
                {
                    throw new UIPopupException($"Fallback option `AllowNoTracking` is requrired under {Application.platform} if not using {nameof(FramePlayer)} to play EIF data.");
                }
            }

            isStarted = true;
            if (enabled)
            {
                OnEnable();
            }
#endif
        }

        public void OnResult(Optional<FrameFilterResult> frameFilterResult)
        {
#if EASYAR_ENABLE_MEGA
            if (cloudLocalizer != null) { return; }
            if (!frameFilterResult.OnSome)
            {
                MapHolder.OnTrack(null);
                return;
            }

            var blocks = new List<Tuple<BlockController.BlockInfo, BlockHolder.PoseSet>>();
            var result = frameFilterResult.Value as MegaTrackerResult;
            foreach (var instance in result.instances())
            {
                using (instance)
                {
                    blocks.Add(Tuple.Create(new BlockController.BlockInfo
                    {
                        ID = instance.blockId(),
                        Name = instance.name(),
                    }, new BlockHolder.PoseSet
                    {
                        BlockToCamera = instance.pose().ToUnityPose(),
                        CameraToVIOOrigin = cameraToVIOOrigin.OnSome ? cameraToVIOOrigin.Value : default(Pose?)
                    }));
                }
            }
            MapHolder.OnTrack(blocks);
#endif
        }

        /// <summary>
        /// <para xml:lang="en">Reset tracker.</para>
        /// <para xml:lang="zh">重置tracker。</para>
        /// </summary>
        public void ResetTracker()
        {
            Tracker?.reset();
        }

#if EASYAR_ENABLE_MEGA

        public override void UpdateMotion(MotionTrackingStatus _, Pose cameraToVIOOrigin) => this.cameraToVIOOrigin = cameraToVIOOrigin;

        public override Optional<Tuple<GameObject, Pose>> TryGetCenter(GameObject center) => MapHolder.TryGetCenter(center);

        public override void UpdateTransform(GameObject center, Pose centerPose) => MapHolder.UpdateTransform(center, centerPose);

        private void CheckFrame(OutputFrame oFrame)
        {
            arSession.FrameUpdate -= CheckFrame;
            using (var iFrame = oFrame.inputFrame())
            {
                if (iFrame.hasSpatialInformation())
                {
                    return;
                }
            }

            if (Fallbacks.WarnAllowNoTracking)
            {
                GUIPopup.EnqueuePersistentWarning("Mega is running defectively without device motion tracking ability." + Environment.NewLine +
                    "  If you are doing this on purpose, you can turn off this message by changing option `WarnAllowNoTracking`." + Environment.NewLine);
            }

            if (!CloudLocalizer.isAvailable()) { throw new UIPopupException(typeof(CloudLocalizer) + " not available"); }

            var config = new MegaLocalizationServiceConfig();
            if (UseGlobalServiceConfig)
            {
                if (EasyARSettings.Instance)
                {
                    config = EasyARSettings.Instance.GlobalMegaLocalizationServiceConfig;
                }
            }
            else
            {
                config = ServiceConfig;
            }
            NotifyEmptyConfig(config);
            try
            {
                cloudLocalizer = CloudLocalizer.create(config.ServerAddress.Trim(), config.APIKey.Trim(), config.APISecret.Trim(), config.AppID.Trim());
            }
            catch (ArgumentNullException)
            {
                throw new UIPopupException($"Fail to create {nameof(CloudLocalizer)}, check logs for detials.");
            }

            arSession.FrameUpdate += (outputFrame) =>
            {
                Resolve(outputFrame);
            };
        }

        private void Resolve(OutputFrame outputFrame)
        {
            if (cloudLocalizer == null) { return; }
            if (!enabled || !resultPoseType.EnableLocalization) { return; }

            if (request.IsInResolve) { return; }
            if ((Time.time - request.StartTime) * 1000 < RequestTimeParameters.RequestInterval) { return; }

            request.IsInResolve = true;
            request.StartTime = Time.time;

            using (var iFrame = outputFrame.inputFrame())
            {
                var deviceAuxiliaryInfo = DeviceAuxiliaryInfo.create();
                if (request.AccelerometerResult.OnSome)
                {
                    deviceAuxiliaryInfo.setAcceleration(request.AccelerometerResult.Value);
                }
                else if (Accelerometer != null)
                {
                    var accResult = Accelerometer.getCurrentResult();
                    if (accResult.OnSome)
                    {
                        deviceAuxiliaryInfo.setAcceleration(accResult.Value);
                    }
                }
                if (request.LocationResult.OnSome)
                {
                    deviceAuxiliaryInfo.setGPSLocation(request.LocationResult.Value);
                }
                else if (LocationManager)
                {
                    var loc = LocationManager.CurrentResult;
                    if (loc.HasValue)
                    {
                        deviceAuxiliaryInfo.setGPSLocation(new LocationResult(loc.Value.latitude, loc.Value.longitude, loc.Value.altitude, loc.Value.horizontalAccuracy, loc.Value.verticalAccuracy, true, true, true));
                    }
                }
                if (proximityLocation.OnSome)
                {
                    deviceAuxiliaryInfo.setProximityLocation(proximityLocation.Value);
                }

                var timestamp = iFrame.timestamp();
                cloudLocalizer.resolve(iFrame, RequestMessage ?? string.Empty, deviceAuxiliaryInfo, requestTimeParameters.Timeout, EasyARController.Scheduler, (response) =>
                {
                    request.IsInResolve = false;
                    var blocks = new List<BlockController>();
                    var blocksInfo = new List<Tuple<BlockController.BlockInfo, BlockHolder.PoseSet>>();
                    foreach (var instance in response.blockInstances())
                    {
                        using (instance)
                        {
                            var blockInfo = new BlockController.BlockInfo
                            {
                                ID = instance.blockId(),
                                Name = instance.name(),
                            };
                            blocks.Add(MapHolder.OnLocalize(blockInfo));
                            blocksInfo.Add(Tuple.Create(blockInfo, new BlockHolder.PoseSet
                            {
                                BlockToCamera = instance.pose().ToUnityPose(),
                                CameraToVIOOrigin = default(Pose?)
                            }));
                        }
                    }
                    MapHolder.OnTrack(blocksInfo);

                    MegaTrackerLocalizationStatus status;
                    switch (response.localizeStatus())
                    {
                        case CloudLocalizerStatus.UnknownError:
                            status = MegaTrackerLocalizationStatus.UnknownError;
                            break;
                        case CloudLocalizerStatus.Found:
                            status = MegaTrackerLocalizationStatus.Found;
                            break;
                        case CloudLocalizerStatus.NotFound:
                            status = MegaTrackerLocalizationStatus.NotFound;
                            break;
                        case CloudLocalizerStatus.RequestIntervalTooLow:
                            status = MegaTrackerLocalizationStatus.RequestIntervalTooLow;
                            break;
                        case CloudLocalizerStatus.RequestTimeout:
                            status = MegaTrackerLocalizationStatus.RequestTimeout;
                            break;
                        case CloudLocalizerStatus.QpsLimitExceeded:
                            status = MegaTrackerLocalizationStatus.QpsLimitExceeded;
                            break;
                        default:
                            throw new Exception("Unknown status: " + response.localizeStatus());
                    }
                    LocalizationUpdate?.Invoke(new LocalizationResponse
                    {
                        Timestamp = timestamp,
                        Status = status,
                        Blocks = blocks,
                        ServerCalculationDuration = response.serverCalculationDuration(),
                        ServerResponseDuration = response.serverResponseDuration(),
                        ErrorMessage = string.IsNullOrEmpty(response.exceptionInfo()) ? null : response.exceptionInfo(),
                    });
                });
            }
        }

        private void RequestLocationPermission()
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                LocationManager.SetLocationPermission(null);
                StartCoroutine(CheckLocationPermission());
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
#if UNITY_ANDROID
                if (Permission.HasUserAuthorizedPermission(Permission.FineLocation))
                {
                    LocationManager.SetLocationPermission(true);
                }
                else
                {
#if UNITY_2020_2_OR_NEWER
                    var callbacks = new PermissionCallbacks();
                    callbacks.PermissionGranted += (_) => { if (LocationManager) { LocationManager.SetLocationPermission(true); } };
                    callbacks.PermissionDenied += (_) => { if (LocationManager) { LocationManager.SetLocationPermission(false); } };
                    callbacks.PermissionDeniedAndDontAskAgain += (_) => { if (LocationManager) { LocationManager.SetLocationPermission(false); } };
                    Permission.RequestUserPermission(Permission.FineLocation, callbacks);
#else
                    Permission.RequestUserPermission(Permission.FineLocation);
                    LocationManager.SetLocationPermission(null);
#endif
                }
                StartCoroutine(CheckLocationPermission());
#endif
            }
        }

        private IEnumerator CheckLocationPermission()
        {
            while (!LocationManager.IsPermissionGranted.HasValue) { yield return null; }
            if (!LocationManager.IsPermissionGranted.Value)
            {
                throw new UIPopupException("Location permission not granted");
            }
        }

        private IEnumerator HandleLocation()
        {
            double timestamp = 0;
            while (true)
            {
                while (locationResultSink == null) { yield return null; }
                while (!LocationManager || !LocationManager.CurrentResult.HasValue) { yield return null; }
                var loc = LocationManager.CurrentResult.Value;
                if (loc.timestamp == timestamp) { yield return null; }
                timestamp = loc.timestamp;
                locationResultSink.handle(new LocationResult(loc.latitude, loc.longitude, loc.altitude, loc.horizontalAccuracy, loc.verticalAccuracy, true, true, true));
                yield return null;
            }
        }
#endif

        private void HandleProximityLocation(ProximityLocationResult pLocation)
        {
            if (Tracker == null) { return; }
            if (proximityLocationResultSink == null)
            {
                proximityLocationResultSink = Tracker.proximityLocationResultSink();
            }
            proximityLocationResultSink.handle(pLocation);
        }

        private IEnumerator GetFrameSourceType()
        {
            while (true)
            {
                if (Tracker == null || !arSession || arSession.Assembly == null || !arSession.Assembly.FrameSource) { yield break; }
                if (arSession.Assembly.FrameSource.Type.OnSome) { break; }
                yield return null;
            }
            Tracker.setInputFrameSourceType(arSession.Assembly.FrameSource.Type.Value);
        }

        private static void NotifyEmptyConfig(MegaLocalizationServiceConfig config)
        {
            if (string.IsNullOrEmpty(config.ServerAddress) ||
                string.IsNullOrEmpty(config.APIKey) ||
                string.IsNullOrEmpty(config.APISecret) ||
                string.IsNullOrEmpty(config.AppID))
            {
                throw new UIPopupException(
                    "Service config (for authentication) NOT set, please set" + Environment.NewLine +
                    "globally in EasyAR Settings (Project Settings > EasyAR) or" + Environment.NewLine +
                    "locally on <MegaTrackerFrameFilter> Component." + Environment.NewLine +
                    "Get from EasyAR Develop Center (www.easyar.com) -> CLS -> Database Details.");
            }
        }

        /// <summary>
        /// <para xml:lang="en">Service config for <see cref="easyar.CloudLocalizer"/>.</para>
        /// <para xml:lang="zh"><see cref="easyar.CloudLocalizer"/>服务器配置。</para>
        /// </summary>
        [Serializable]
        public class MegaLocalizationServiceConfig
        {
            /// <summary>
            /// <para xml:lang="en">Server Address, go to EasyAR Develop Center (https://www.easyar.com) for details.</para>
            /// <para xml:lang="zh">服务器地址，详见EasyAR开发中心（https://www.easyar.cn）。</para>
            /// </summary>
            public string ServerAddress = string.Empty;
            /// <summary>
            /// <para xml:lang="en">API Key, go to EasyAR Develop Center (https://www.easyar.com) for details.</para>
            /// <para xml:lang="zh">API Key，详见EasyAR开发中心（https://www.easyar.cn）。</para>
            /// </summary>
            public string APIKey = string.Empty;
            /// <summary>
            /// <para xml:lang="en">API Secret, go to EasyAR Develop Center (https://www.easyar.com) for details.</para>
            /// <para xml:lang="zh">API Secret，详见EasyAR开发中心（https://www.easyar.cn）。</para>
            /// </summary>
            public string APISecret = string.Empty;
            /// <summary>
            /// <para xml:lang="en">Mega localization service AppID, go to EasyAR Develop Center (https://www.easyar.com) for details.</para>
            /// <para xml:lang="zh">Mega定位服务AppID，详见EasyAR开发中心（https://www.easyar.cn）。</para>
            /// </summary>
            public string AppID = string.Empty;
        }

        /// <summary>
        /// <para xml:lang="en">Request time parameters.</para>
        /// <para xml:lang="zh">请求时间参数。</para>
        /// </summary>
        [Serializable]
        public class TimeParameters
        {
            /// <summary>
            /// <para xml:lang="en">Timeout in milliseconds when communicating with server.</para>
            /// <para xml:lang="zh">与服务器通信的超时时间（毫秒）。</para>
            /// </summary>
            [Tooltip("Timeout in milliseconds when communicating with server.")]
            public int Timeout = 6000;
            /// <summary>
            /// <para xml:lang="en">Expected request interval in milliseconds, with a longer value results a larger overall error.</para>
            /// <para xml:lang="zh">期望的请求间隔时间（毫秒），值越大整体误差越大。</para>
            /// </summary>
            [Tooltip("Expected request interval in milliseconds, with a longer value results a larger overall error.")]
            public int RequestInterval = 1000;
        }

        /// <summary>
        /// <para xml:lang="en">Result pose parameters.</para>
        /// <para xml:lang="zh">结果姿态类型参数。</para>
        /// </summary>
        public class ResultPoseTypeParameters
        {
            /// <summary>
            /// <para xml:lang="en">Enable localization.</para>
            /// <para xml:lang="zh">开启定位。</para>
            /// </summary>
            public bool EnableLocalization = true;
            /// <summary>
            /// <para xml:lang="en">Enable stabilization.</para>
            /// <para xml:lang="zh">开启稳定器。</para>
            /// </summary>
            public bool EnableStabilization = true;
        }

        /// <summary>
        /// <para xml:lang="en">The response of localization request.</para>
        /// <para xml:lang="zh">定位请求的响应。</para>
        /// </summary>
        public class LocalizationResponse
        {
            /// <summary>
            /// <para xml:lang="en"><see cref="InputFrame"/> timestamp when sending the request.</para>
            /// <para xml:lang="zh">发送请求时的<see cref="InputFrame"/>时间戳。</para>
            /// </summary>
            public double Timestamp;
            /// <summary>
            /// <para xml:lang="en">Localization status.</para>
            /// <para xml:lang="zh">定位状态。</para>
            /// </summary>
            public MegaTrackerLocalizationStatus Status;
#if EASYAR_ENABLE_MEGA
            /// <summary>
            /// <para xml:lang="en">Localized <see cref="BlockController"/>. There is only one block in the list if localzied. Objects transform and status in the scene does not change with event data in the same time the event is triggered.</para>
            /// <para xml:lang="zh">定位到的<see cref="BlockController"/>。定位到block时，列表中只会有一个数据。事件发生时场景中物体的位置和状态与事件中的数据无对应关系。</para>
            /// </summary>
            public List<BlockController> Blocks = new List<BlockController>();
#endif
            /// <summary>
            /// <para xml:lang="en">The duration in seconds for server response.</para>
            /// <para xml:lang="zh">服务器响应耗时（秒）。</para>
            /// </summary>
            public Optional<double> ServerResponseDuration;
            /// <summary>
            /// <para xml:lang="en">The duration in seconds for server internal calculation.</para>
            /// <para xml:lang="zh">服务器内部计算耗时（秒）。</para>
            /// </summary>
            public Optional<double> ServerCalculationDuration;
            /// <summary>
            /// <para xml:lang="en">Error message. It is filled when <see cref="Status"/> is <see cref="MegaTrackerLocalizationStatus.UnknownError"/>.</para>
            /// <para xml:lang="zh">错误信息。当<see cref="Status"/>为<see cref="MegaTrackerLocalizationStatus.UnknownError"/>时有值。</para>
            /// </summary>
            public Optional<string> ErrorMessage;
        }

        /// <summary>
        /// <para xml:lang="en">Fallback options mainly used for develop and debug.</para>
        /// <para xml:lang="en">These options are default on, because they are useful for developers to do some tests when they are new to Mega.</para>
        /// <para xml:lang="en">For real Mega experience, please</para>
        /// <para xml:lang="en">1) Turn off these options when you are on spot, or</para>
        /// <para xml:lang="en">2) Use <see cref="FramePlayer"/> to play EIF data for remote test.</para>
        /// <para xml:lang="zh">退化选项，主要用于开发和调试。</para>
        /// <para xml:lang="zh">这些选项默认开启，因为它们可以方便新了解Mega的开发者做一些测试。</para>
        /// <para xml:lang="zh">想要了解真实的Mega体验，请</para>
        /// <para xml:lang="zh">1) 关闭这些选项并在现场使用，或</para>
        /// <para xml:lang="zh">2) 通过使用<see cref="FramePlayer"/>播放EIF数据来进行远程测试。</para>
        /// </summary>
        [Serializable]
        public class FallbackOptions
        {
            /// <summary>
            /// <para xml:lang="en">Allow Mega to run without device motion tracking ability. Generally you should treat your application in "develop mode" if this option is on.</para>
            /// <para xml:lang="en">Mega will run defectively without device motion tracking ability, the behavior is not suitable for most application scenarios.</para>
            /// <para xml:lang="en">This option is default on, because it is useful for developers to do some tests on PCs if not using <see cref="FramePlayer"/> to play EIF data. Don't forget to turn it off before upload your application to app store.</para>
            /// <para xml:lang="en">Warning messages will be displayed on screen if this option is on. In some rare cases, when you want to use Mega when device motion tracking is missing, you can turn off warning message by changing <see cref="WarnAllowNoTracking"/>.</para>
            /// <para xml:lang="zh">允许Mega在没有运动跟踪能力的设备上运行。如果这个选项开启，通常你的应用应处于“开发模式”。</para>
            /// <para xml:lang="zh">在没有运动跟踪能力的设备上，Mega运行效果将是明显不完整的，其行为通常不适合大多数应用场景。</para>
            /// <para xml:lang="zh">这个选项默认开启，因为如果没有通过<see cref="FramePlayer"/>播放EIF数据，开发者仍可以在开启这个选项的时候使用电脑进行一些测试。请别忘记在应用上架前关闭这个选项。</para>
            /// <para xml:lang="zh">选项开启的时候，屏幕上会显示警告信息。在一些特殊情况下，如果你需要在没有运动跟踪能力的设备上使用Mega，可以通过修改<see cref="WarnAllowNoTracking"/>来关闭警告信息。</para>
            /// </summary>
            [Tooltip("Allow Mega to run without device motion tracking ability. Generally you should treat your application in 'develop mode' if this option is on.")]
            public bool AllowNoTracking = true;
            /// <summary>
            /// <para xml:lang="en">Allow Mega to run for non-eif remote test. You should treat your application in "develop mode" if this option is on.</para>
            /// <para xml:lang="en">Mega will run defectively for remote test if the test is not performed using <see cref="FramePlayer"/> to play EIF data. Mega will not get its best if this option is on.</para>
            /// <para xml:lang="en">This option is default on, because it is useful for developers to do some tests remotely if not using <see cref="FramePlayer"/> to play EIF data. Don't forget to turn it off before upload your application to app store, or if you can do tests with EIF data.</para>
            /// <para xml:lang="en">Warning messages will always be displayed on screen if this option is on, you are not able to turn it off.</para>
            /// <para xml:lang="en">Notice: Don't turn off this option while doing non-eif remote test, you will get very bad behavior from Mega cloud service which may lead to very wrong conclusion.</para>
            /// <para xml:lang="zh">允许Mega使用在没有eif数据的远程测试种。如果这个选项开启，你的应用应处于“开发模式”。</para>
            /// <para xml:lang="zh">如果远程测试不是通过<see cref="FramePlayer"/>播放EIF数据来完成的，Mega运行效果将是明显不完整的，其行为不适合大多数应用场景，且在选项开启的时候，Mega无法达到最佳效果。</para>
            /// <para xml:lang="zh">这个选项默认开启，因为如果没有通过<see cref="FramePlayer"/>播放EIF数据来进行远程测试，开发者仍可以在开启这个选项的时候进行一些远程测试。请别忘记在应用上架前关闭这个选项。</para>
            /// <para xml:lang="zh">选项开启的时候，屏幕上会显示警告信息。该信息无法关闭。</para>
            /// <para xml:lang="zh">注意：在不使用eif数据进行远程测试的时候，请不要关闭这个选项。否则你将得到非常差的效果，并会得出错误的结论。</para>
            /// </summary>
            [Tooltip("Allow Mega to run for non-eif remote test. You should treat your application in 'develop mode' if this option is on.")]
            public bool AllowNonEifRemote = true;
            /// <summary>
            /// <para xml:lang="en">Display warning message on screen when <see cref="AllowNoTracking"/> is true.</para>
            /// <para xml:lang="zh">当<see cref="AllowNoTracking"/>为true时在屏幕上显示警告信息。。</para>
            /// </summary>
            [Tooltip("Display warning message on screen when `AllowNoTracking` is true.")]
            public bool WarnAllowNoTracking = true;
        }

#if EASYAR_ENABLE_MEGA
        private struct Request
        {
            public bool IsInResolve;
            public float StartTime;
            public Optional<AccelerometerResult> AccelerometerResult;
            public Optional<LocationResult> LocationResult;
        }
#endif
    }
}
