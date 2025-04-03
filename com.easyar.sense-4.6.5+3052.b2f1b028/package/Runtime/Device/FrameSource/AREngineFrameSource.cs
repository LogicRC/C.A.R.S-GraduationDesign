//================================================================================================================================
//
//  Copyright (c) 2015-2023 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
//  EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
//  and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
//
//================================================================================================================================

#if UNITY_ANDROID && !UNITY_EDITOR
using System;
using System.Collections;
#endif
using UnityEngine;

namespace easyar
{
    /// <summary>
    /// <para xml:lang="en">A custom frame source which connects AREngine camera device output to EasyAR input in the scene, providing AR Engine support using custom camera feature of EasyAR Sense.</para>
    /// <para xml:lang="en">This frame source is one type of motion tracking device, and will output motion data in a <see cref="ARSession"/>.</para>
    /// <para xml:lang="en">``Huawei AR Engine Unity SDK`` is NOT required to use this frame source.</para>
    /// <para xml:lang="en">To choose frame source in runtime, you can deactive Camera GameObject and set all required values of all frame sources for availability check, and active Camera GameObject when this frame source is chosen.</para>
    /// <para xml:lang="zh">在场景中将AREngine相机设备的输出连接到EasyAR输入的自定义frame source。通过EasyAR Sense的自定义相机功能提供华为AR Engine支持。</para>
    /// <para xml:lang="zh">这个frame source是一种运动跟踪设备，在<see cref="ARSession"/>中会输出运动数据。</para>
    /// <para xml:lang="zh">这个frame source不使用 ``华为 AR Engine Unity SDK`` ，无需添加。</para>
    /// <para xml:lang="zh">如果要在运行时选择 frame source，可以deactive Camera GameObject，并设置所有frame source可用性检查所需要的数值，然后在这个frame source被选择后active Camera GameObject。</para>
    /// </summary>
    public class AREngineFrameSource : FrameSource
    {
        /// <summary>
        /// <para xml:lang="en">Set focus mode when device is opening. Note: auto focus switch may not work on some devices due to hardware or system limitation.</para>
        /// <para xml:lang="zh">设置打开设备时的对焦模式。注意：受硬件或系统限制，自动对焦开关在一些设备上可能无效。</para>
        /// </summary>
        public bool AutoFocus = true;

        private bool assembled;
        [SerializeField, HideInInspector]
        private WorldRootController worldRoot;

#if UNITY_ANDROID && !UNITY_EDITOR
        private arengineinterop.AREngineCameraDevice device;
        private bool willOpen;
        private bool disableAutoOpen;
        private WorldRootController worldRootCache;
        private GameObject worldRootObject;

        public override Optional<bool> IsAvailable
        {
            get
            {
                if (Application.platform == RuntimePlatform.Android && EasyARSettings.Instance && EasyARSettings.Instance.DisableARCoreAREngine)
                {
                    GUIPopup.EnqueueMessage("AREngine is manually disabled. See EasyAR > Sense > Configuration > Disable ARCore AREngine.", 3);
                    return false;
                }
                if (EasyARSettings.Instance && EasyARSettings.Instance.AREngineSDK == EasyARSettings.AREngineType.Disabled)
                {
                    return false;
                }
                return arengineinterop.AREngineCameraDevice.isAvailable();
            }
        }

        public override int BufferCapacity
        {
            get
            {
                if (device != null)
                {
                    return device.bufferCapacity();
                }
                return bufferCapacity;
            }
            set
            {
                bufferCapacity = value;
                device?.setBufferCapacity(value);
            }
        }
#else
        public override Optional<bool> IsAvailable { get => false; }
#endif

        public override Optional<InputFrameSourceType> Type { get => InputFrameSourceType.AREngine; }

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

        public override GameObject Origin { get => worldRoot ? worldRoot.gameObject : null; }

#if UNITY_ANDROID && !UNITY_EDITOR
        protected override void Awake()
        {
            base.Awake();
            if (worldRoot) { return; }
            worldRootCache = FindObjectOfType<WorldRootController>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            device?.setFocusMode(AutoFocus ? arengineinterop.AREngineCameraDeviceFocusMode.Auto : arengineinterop.AREngineCameraDeviceFocusMode.Fixed);
            device?.start();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            device?.stop();
        }

        protected void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                device?.onPause();
            }
            else
            {
                device?.onResume();
            }
        }

        protected virtual void OnDestroy()
        {
            Close();
            if (worldRootObject) Destroy(worldRootObject);
        }
#endif

        public override void OnAssemble(ARSession session)
        {
            base.OnAssemble(session);
#if UNITY_ANDROID && !UNITY_EDITOR
            SetupOriginUsingWorldRoot();
            StartCoroutine(AutoOpen());
#endif
            assembled = true;
        }

        /// <summary>
        /// <para xml:lang="en">Open device.</para>
        /// <para xml:lang="zh">打开设备。</para>
        /// </summary>
        public void Open()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (EasyARSettings.Instance && EasyARSettings.Instance.AREngineSDK == EasyARSettings.AREngineType.Disabled)
            {
                return;
            }
            disableAutoOpen = true;
            willOpen = true;
            CameraDevice.requestPermissions(EasyARController.Scheduler, (Action<PermissionStatus, string>)((status, msg) =>
            {
                if (!willOpen)
                {
                    return;
                }
                if (status != PermissionStatus.Granted)
                {
                    throw new UIPopupException("Camera permission not granted");
                }

                Close();

                device = new arengineinterop.AREngineCameraDevice();

                if (bufferCapacity != 0)
                {
                    device.setBufferCapacity(bufferCapacity);
                }

                device.setInputFrameHandler((aFrame) =>
                {
                    if (sink == null) { return; }
                    using (var aImage = aFrame.image())
                    {
                        var aBuffer = aImage.buffer();
                        using (var buffer = Buffer.wrap(aBuffer.data(), aBuffer.size(), () => aBuffer.Dispose()))
                        using (var image = Image.create(buffer, (PixelFormat)aImage.format(), aImage.width(), aImage.height(), aImage.pixelWidth(), aImage.pixelHeight()))
                        using (var acp = aFrame.cameraParameters())
                        {
                            var acpSize = acp.size();
                            var acpFocalLength = acp.focalLength();
                            var acpPrincipalPoint = acp.principalPoint();
                            using (var cp = new CameraParameters(new Vec2I(acpSize.data[0], acpSize.data[1]), new Vec2F(acpFocalLength.data[0], acpFocalLength.data[1]), new Vec2F(acpPrincipalPoint.data[0], acpPrincipalPoint.data[1]), (CameraDeviceType)acp.cameraDeviceType(), acp.cameraOrientation()))
                            {
                                var act = aFrame.cameraTransform();
                                var ct = new Matrix44F(
                                    act.data[0], act.data[1], act.data[2], act.data[3],
                                    act.data[4], act.data[5], act.data[6], act.data[7],
                                    act.data[8], act.data[9], act.data[10], act.data[11],
                                    act.data[12], act.data[13], act.data[14], act.data[15]
                                );
                                using (var frame = InputFrame.create(image, cp, aFrame.timestamp(), ct, (MotionTrackingStatus)aFrame.trackingStatus()))
                                {
                                    sink.handle(frame);
                                }
                            }
                        }
                    }
                });
                device.setHighResMode(true);

                if (enabled)
                {
                    OnEnable();
                }
            }));
#endif
        }

        /// <summary>
        /// <para xml:lang="en">Close device.</para>
        /// <para xml:lang="zh">关闭设备。</para>
        /// </summary>
        public void Close()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            disableAutoOpen = true;
            willOpen = false;
            if (device != null)
            {
                OnDisable();
                device.close();
                device.Dispose();
                device = null;
            }
#endif
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        private IEnumerator AutoOpen()
        {
            while (!enabled)
            {
                if (disableAutoOpen) { yield break; }
                yield return null;
            }
            if (disableAutoOpen) { yield break; }
            if (IsAvailable.OnNone || !IsAvailable.Value) { throw new UIPopupException(typeof(arengineinterop.AREngineCameraDevice) + " not available"); }
            Open();
        }

        private void SetupOriginUsingWorldRoot()
        {
            if (worldRoot) { return; }
            worldRoot = worldRootCache;
            if (worldRoot) { return; }
            worldRoot = FindObjectOfType<WorldRootController>();
            if (worldRoot) { return; }
            Debug.Log($"WorldRoot not found, create from {typeof(AREngineFrameSource)}");
            worldRootObject = new GameObject("WorldRoot");
            worldRoot = worldRootObject.AddComponent<WorldRootController>();
        }
#endif
    }
}
