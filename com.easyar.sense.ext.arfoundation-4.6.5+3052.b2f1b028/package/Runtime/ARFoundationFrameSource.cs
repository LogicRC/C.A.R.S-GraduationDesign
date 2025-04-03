//================================================================================================================================
//
//  Copyright (c) 2015-2024 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
//  EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
//  and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
//
//================================================================================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using ARFoundation = UnityEngine.XR.ARFoundation;

namespace easyar
{
    /// <summary>
    /// <para xml:lang="en">A custom frame source which connects AR Foundation output to EasyAR input in the scene, providing AR Foundation support using custom camera feature of EasyAR Sense.</para>
    /// <para xml:lang="en">This frame source is one type of motion tracking device, and will output motion data in a <see cref="ARSession"/>.</para>
    /// <para xml:lang="en">``AR Foundation`` is required to use this frame source, you need to setup AR Foundation according to official documents.</para>
    /// <para xml:lang="en">This frame source will use ``ARFoundation.ARSession.CheckAvailability`` to check availability. <see cref="FrameSource.Camera"/> and <see cref="OriginCandidate"/> are also required for availability check, they will be automatically picked from scene objects if not setup. To choose frame source in runtime, you can deactive AR Foundation GameObjects and set all required values of all frame sources for availability check, and active AR Foundation GameObjects when this frame source is chosen.</para>
    /// <para xml:lang="zh">在场景中将AR Foundation 的输出连接到EasyAR输入的自定义frame source。通过EasyAR Sense的自定义相机功能提供AR Foundation支持。</para>
    /// <para xml:lang="zh">这个frame source是一种运动跟踪设备，在<see cref="ARSession"/>中会输出运动数据。</para>
    /// <para xml:lang="zh">为了使用这个frame source， ``AR Foundation`` 是必需的。你需要根据官方文档配置AR Foundation。</para>
    /// <para xml:lang="zh">这个frame source会使用 ``ARFoundation.ARSession.CheckAvailability`` 来检查可用性。在可用性检查中，<see cref="FrameSource.Camera"/> 和<see cref="OriginCandidate"/> 也是需要的，如果没有事先设置，会自动从场景物体中选择。如果要在运行时选择 frame source，可以deactive AR Foundation使用的所有GameObject，并设置所有frame source可用性检查所需要的数值，然后在这个frame source被选择后active AR Foundation 的GameObject。</para>
    /// </summary>
    public class ARFoundationFrameSource : FrameSource
    {
        /// <summary>
        /// <para xml:lang="en">If color image is used as frame input. Color image is usefull when recording a colored eif file, but not necessary for all EasyAR algorithms.</para>
        /// <para xml:lang="zh">是否使用彩色图像作为frame输入。彩色图像在需要录制彩色eif文件的时候可以使用。所有EasyAR算法都不需要使用彩色图像。</para>
        /// </summary>
        public bool EnableColorInput;

        /// <summary>
        /// <para xml:lang="en">If the device supports AR Foundation but does not have the necessary software, some platforms allow prompting the user to install or update the software. If this field is true, a software update will be attempted. If the appropriate software is not installed or out of date, and this field is false, then this frame source will not be available.</para>
        /// <para xml:lang="zh">如果设备支持AR Foundation但没有必要的软件，一些平台允许提示用户安装或更新软件。如果变量值为true，会尝试软件更新。如果系统中没有安装软件或软件过期，且变量值为false，这个frame source将是不可用的。</para>
        /// </summary>
        public bool AttemptUpdate = true;
        private bool assembled = false;

        public override Optional<InputFrameSourceType> Type
        {
            get
            {
                if (Application.platform == RuntimePlatform.Android)
                {
                    return InputFrameSourceType.ARCore;
                }
                else if (Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    return InputFrameSourceType.ARKit;
                }
                else
                {
                    return InputFrameSourceType.General;
                }
            }
        }

        /// <summary>
        /// <para xml:lang="en">Optimize ARCameraManager.currentConfiguration if it does not meet basic requirement for some of EasyAR features like Mega. Turn it off if you want full control of configuration choosing.</para>
        /// <para xml:lang="zh">在ARCameraManager.currentConfiguration 不满足部分EasyAR功能（比如Mega）的基础需求时进行优化。如果你需要完全控制配置，你需要关掉这个选项。</para>
        /// </summary>
        public bool OptimizeConfigurationForTracking = true;

        /// <summary>
        /// <para xml:lang="en">The object Camera move against, will be automatically get from the scene.</para>
        /// <para xml:lang="zh">相机运动的相对物体，如果没设置，将会自动从场景中获取。</para>
        /// </summary>
        public MonoBehaviour OriginCandidate
        {
            get => originCandidate;
            set
            {
                if (assembled) { return; }
                originCandidate = value;
            }
        }

        private static IReadOnlyList<ARSession.ARCenterMode> availableCenterMode = new List<ARSession.ARCenterMode> { ARSession.ARCenterMode.SessionOrigin, ARSession.ARCenterMode.FirstTarget, ARSession.ARCenterMode.SpecificTarget };
        [SerializeField, HideInInspector]
        private MonoBehaviour originCandidate;
        private double curTimestamp;
        private int cameraOrientation;
        private BufferPool bufferPool;
        private int bufferSize;
        private ARFoundation.ARCameraManager cameraManager;
        private Action<Pose> newFrame;
        private ARFoundation.CameraFacingDirection currentFacingDirection;
        private Optional<bool> isAvailable;

        public override Optional<bool> IsAvailable { get { return isAvailable; } }

        public override int BufferCapacity
        {
            get => bufferCapacity;
            set
            {
                bufferCapacity = value;
                if (bufferPool == null || bufferPool.capacity() == bufferCapacity) { return; }
                bufferPool.Dispose();
                bufferPool = new BufferPool(bufferSize, bufferCapacity);
            }
        }

        public override GameObject Origin { get => originCandidate ? originCandidate.gameObject : null; }

        public override bool IsCameraUnderControl { get { return false; } }

        public override IReadOnlyList<ARSession.ARCenterMode> AvailableCenterMode { get => availableCenterMode; }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (cameraManager)
            {
                cameraManager.frameReceived += OnCameraFrameReceived;
            }
            Application.onBeforeRender += OnBeforeRender;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (cameraManager)
            {
                cameraManager.frameReceived -= OnCameraFrameReceived;
            }
            Application.onBeforeRender -= OnBeforeRender;
        }

        private void OnDestroy()
        {
            bufferPool?.Dispose();
        }

        public override void OnAssemble(ARSession session)
        {
            base.OnAssemble(session);
#if EASYAR_ENABLE_XRORIGIN_AR
            if (originCandidate is Unity.XR.CoreUtils.XROrigin xrOrigin)
            {
                if (xrOrigin && (xrOrigin.RequestedTrackingOriginMode != Unity.XR.CoreUtils.XROrigin.TrackingOriginMode.Device || xrOrigin.CameraYOffset != 0))
                {
                    xrOrigin.RequestedTrackingOriginMode = Unity.XR.CoreUtils.XROrigin.TrackingOriginMode.Device;
                    xrOrigin.CameraYOffset = 0;
                    Debug.LogWarning($"force XROrigin (RequestedTrackingOriginMode = {xrOrigin.RequestedTrackingOriginMode}, CameraYOffset = {xrOrigin.CameraYOffset}) when using EasyAR");
                }
            }
#endif
            cameraManager = Camera.GetComponent<ARFoundation.ARCameraManager>();
            StartCoroutine(ChooseARFoundationConfig());
            if (enabled)
            {
                cameraManager.frameReceived += OnCameraFrameReceived;
            }
            cameraOrientation = CameraOrientation();
            assembled = true;
            SetupOrigin();
        }

        public override IEnumerator CheckAvailability()
        {
            if (!Camera && !PickCamera())
            {
                isAvailable = false;
                yield break;
            }
            if (Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer)
            {
                isAvailable = false;
                yield break;
            }
            if (ARFoundation.ARSession.state <= ARFoundation.ARSessionState.CheckingAvailability)
            {
                yield return ARFoundation.ARSession.CheckAvailability();
            }
            if (ARFoundation.ARSession.state == ARFoundation.ARSessionState.NeedsInstall && AttemptUpdate)
            {
                yield return ARFoundation.ARSession.Install();
            }
            while (ARFoundation.ARSession.state == ARFoundation.ARSessionState.Installing)
            {
                yield return null;
            }

            isAvailable = ARFoundation.ARSession.state >= ARFoundation.ARSessionState.Ready;
        }

        public override Camera PickCamera()
        {
            if (SetupOrigin())
            {
#if EASYAR_ENABLE_XRORIGIN_AR
                if (OriginCandidate is Unity.XR.CoreUtils.XROrigin xrOrigin)
                {
                    return xrOrigin ? xrOrigin.Camera : null;
                }
#endif
#if EASYAR_ENABLE_ARSESSIONORIGIN
#pragma warning disable 612, 618
                if (OriginCandidate is UnityEngine.XR.ARFoundation.ARSessionOrigin sessionOrigin)
                {
                    return sessionOrigin ? sessionOrigin.GetComponentsInChildren<Camera>(true).Where(c => c.GetComponent<UnityEngine.XR.ARFoundation.ARCameraManager>()).SingleOrDefault() : null;
                }
#pragma warning restore 612, 618
#endif
            }
            return null;
        }

        protected override bool IsValidCamera(Camera cam)
        {
            if (!cam) { return false; }

            if (SetupOrigin())
            {
#if EASYAR_ENABLE_XRORIGIN_AR
                if (OriginCandidate is Unity.XR.CoreUtils.XROrigin xrOrigin)
                {
                    return xrOrigin ? cam == xrOrigin.Camera : false;
                }
#endif
#if EASYAR_ENABLE_ARSESSIONORIGIN
#pragma warning disable 612, 618
                if (OriginCandidate is UnityEngine.XR.ARFoundation.ARSessionOrigin sessionOrigin)
                {
                    return sessionOrigin ? cam == sessionOrigin.GetComponentsInChildren<Camera>(true).Where(c => c.GetComponent<UnityEngine.XR.ARFoundation.ARCameraManager>()).SingleOrDefault() : false;
                }
#pragma warning restore 612, 618
#endif
            }
            else
            {
#if EASYAR_ENABLE_XRORIGIN_AR
                var xrOrigin = cam.GetComponentInParent<Unity.XR.CoreUtils.XROrigin>();
                if (!xrOrigin)
                {
                    var parent = cam.transform.parent;
                    while (parent)
                    {
                        xrOrigin = parent.GetComponent<Unity.XR.CoreUtils.XROrigin>();
                        if (xrOrigin) { break; }
                        parent = parent.parent;
                    }
                }
                if (xrOrigin)
                {
                    if (OriginCandidate && OriginCandidate != xrOrigin) { return false; }
                    return cam == xrOrigin.Camera;
                }
#endif
#if EASYAR_ENABLE_ARSESSIONORIGIN
#pragma warning disable 612, 618
                var sessionOrigin = cam.GetComponent<ARFoundation.ARSessionOrigin>();
                if (!sessionOrigin)
                {
                    var parent = cam.transform.parent;
                    while (parent)
                    {
                        sessionOrigin = parent.GetComponent<ARFoundation.ARSessionOrigin>();
                        if (sessionOrigin) { break; }
                        parent = parent.parent;
                    }
                }
                if (sessionOrigin)
                {
                    if (OriginCandidate && OriginCandidate != sessionOrigin) { return false; }
                    return cam.GetComponent<ARFoundation.ARCameraManager>();
                }
#pragma warning restore 612, 618
#endif
            }
            return false;
        }

        unsafe void OnCameraFrameReceived(ARFoundation.ARCameraFrameEventArgs eventArgs)
        {
            if (!arSession || !cameraManager || bufferCapacity <= 0) { return; }
            if (ARFoundation.ARSession.state <= ARFoundation.ARSessionState.Ready) { return; }

            if (!cameraManager.TryGetIntrinsics(out var intrinsics)) { return; }
            if (!cameraManager.TryAcquireLatestCpuImage(out var cameraImage)) { return; }

            Buffer buffer;
            Vec2I size;
            var timestamp = eventArgs.timestampNs ?? (long)(cameraImage.timestamp * 1e9);

            var pixelSize = new Vector2Int();
            PixelFormat pixelFormat;

            using (cameraImage)
            {
                if (timestamp == curTimestamp) { return; }

                curTimestamp = timestamp;
                size = new Vec2I(cameraImage.width, cameraImage.height);
                var planeY = cameraImage.GetPlane(0);
                var planeU = default(UnityEngine.XR.ARSubsystems.XRCpuImage.Plane?);
                var planeV = default(UnityEngine.XR.ARSubsystems.XRCpuImage.Plane?);
                var Y = new IntPtr(planeY.data.GetUnsafePtr());
                var U = IntPtr.Zero;
                var V = IntPtr.Zero;

                if (!EnableColorInput || cameraImage.format == UnityEngine.XR.ARSubsystems.XRCpuImage.Format.OneComponent8)
                {
                    pixelSize = new Vector2Int(planeY.rowStride, cameraImage.height);
                    pixelFormat = PixelFormat.Gray;
                }
                else if (cameraImage.format == UnityEngine.XR.ARSubsystems.XRCpuImage.Format.AndroidYuv420_888)
                {
                    if (cameraImage.planeCount < 3)
                    {
                        throw new InvalidOperationException($"Insufficient planeCount for {cameraImage.format}: {cameraImage.planeCount}");
                    }
                    planeU = cameraImage.GetPlane(1);
                    planeV = cameraImage.GetPlane(2);
                    U = new IntPtr(planeU.Value.data.GetUnsafePtr());
                    V = new IntPtr(planeV.Value.data.GetUnsafePtr());
                    pixelSize = ImageUtil.GetPixelSize(Y, U, V, planeY.rowStride);
                    pixelFormat = ImageUtil.CheckPixelFormat(Y, U, V, planeY.pixelStride, planeU.Value.pixelStride, planeV.Value.pixelStride, planeY.rowStride, planeU.Value.rowStride, planeV.Value.rowStride);
                }
                else if (cameraImage.format == UnityEngine.XR.ARSubsystems.XRCpuImage.Format.IosYpCbCr420_8BiPlanarFullRange)
                {
                    if (cameraImage.planeCount < 2)
                    {
                        throw new InvalidOperationException($"Insufficient planeCount for {cameraImage.format}: {cameraImage.planeCount}");
                    }
                    planeU = cameraImage.GetPlane(1);
                    U = new IntPtr(planeU.Value.data.GetUnsafePtr());
                    pixelSize = ImageUtil.GetPixelSize(Y, U, U + 1, planeY.rowStride);
                    pixelFormat = PixelFormat.YUV_NV12;
                }
                else
                {
                    throw new InvalidOperationException($"Unsupported format: {cameraImage.format}");
                }

                var yLen = pixelSize.x * pixelSize.y;
                var uvLen = yLen / 2;
                var bufferBlockSize = (pixelFormat == PixelFormat.Gray) ? yLen : uvLen * 3;

                if (bufferSize != bufferBlockSize)
                {
                    bufferSize = bufferBlockSize;
                    bufferPool?.Dispose();
                    bufferPool = new BufferPool(bufferSize, bufferCapacity);
                }
                var bufferO = bufferPool.tryAcquire();
                if (bufferO.OnNone) { return; }

                buffer = bufferO.Value;
                ImageUtil.FillImageBuffer(Tuple.Create(Y, planeY.data.Length), Tuple.Create(U, planeU.HasValue ? planeU.Value.data.Length : 0), Tuple.Create(V, planeV.HasValue ? planeV.Value.data.Length : 0), pixelSize, pixelFormat, buffer);
            }

            var screenRotation = arSession.Assembly.Display.Rotation;
            var trackingStatus = ARFoundation.ARSession.state == ARFoundation.ARSessionState.SessionTracking ? MotionTrackingStatus.Tracking : MotionTrackingStatus.NotTracking;
            if (currentFacingDirection != cameraManager.currentFacingDirection)
            {
                cameraOrientation = CameraOrientation();
                currentFacingDirection = cameraManager.currentFacingDirection;
            }

            newFrame = (pose) =>
            {
                using (var cameraParameters = new CameraParameters(size, new Vec2F(intrinsics.focalLength.x, intrinsics.focalLength.y), new Vec2F(intrinsics.principalPoint.x, intrinsics.principalPoint.y), CameraDeviceType.Back, cameraOrientation))
                using (buffer)
                using (var image = Image.create(buffer, pixelFormat, size.data_0, size.data_1, pixelSize.x, pixelSize.y))
                {
                    var displayCompensation = Quaternion.Euler(0, 0, -cameraParameters.imageOrientation(screenRotation));
                    var pe = new Pose(Vector3.zero, displayCompensation).GetTransformedBy(pose).ToEasyARPose();
                    using (var frame = InputFrame.create(image, cameraParameters, timestamp * 1e-9, pe, trackingStatus))
                    {
                        sink.handle(frame);
                    }
                }
            };
        }

        [BeforeRenderOrder(100)]
        void OnBeforeRender()
        {
            if (!Camera) { return; }
            newFrame?.Invoke(new Pose(Camera.transform.localPosition, Camera.transform.localRotation));
            newFrame = null;
        }

        private int CameraOrientation()
        {
            var orientation = 0;
#if UNITY_ANDROID && !UNITY_EDITOR
            var index = cameraManager.currentFacingDirection != ARFoundation.CameraFacingDirection.User ? 0 : 1;
            if (Application.platform == RuntimePlatform.Android)
            {
                using (var cameraInfo = new AndroidJavaObject("android.hardware.Camera$CameraInfo"))
                using (var cameraClass = new AndroidJavaClass("android.hardware.Camera"))
                {
                    cameraClass.CallStatic("getCameraInfo", index, cameraInfo);
                    orientation = cameraInfo.Get<int>("orientation");
                }
            }
#else
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                orientation = 90;
            }
            else
            {
                orientation = 0;
            }
#endif
            return orientation;
        }

        private bool SetupOrigin()
        {
#if EASYAR_ENABLE_XRORIGIN_AR
            if (originCandidate is Unity.XR.CoreUtils.XROrigin xrOrigin && xrOrigin) { return true; }
            xrOrigin = FindObjectOfType<Unity.XR.CoreUtils.XROrigin>();
            if (xrOrigin)
            {
                originCandidate = xrOrigin;
                return true;
            }
#endif
#if EASYAR_ENABLE_ARSESSIONORIGIN
#pragma warning disable 612, 618
            if (originCandidate is UnityEngine.XR.ARFoundation.ARSessionOrigin candidate && candidate) { return true; }
            candidate = FindObjectOfType<UnityEngine.XR.ARFoundation.ARSessionOrigin>();
            if (candidate)
            {
                originCandidate = candidate;
                return true;
            }
#pragma warning restore 612, 618
#endif
            return false;
        }

        IEnumerator ChooseARFoundationConfig()
        {
            if (!OptimizeConfigurationForTracking)
            {
                yield break;
            }
            yield return new WaitUntil(() => cameraManager && cameraManager.currentConfiguration.HasValue);
            var currentConfiguration = cameraManager.currentConfiguration.Value;
            if (currentConfiguration.width < 960 && currentConfiguration.height < 960)
            {
                while (true)
                {
                    if (!cameraManager) { yield break; }

                    using (var configurations = cameraManager.GetConfigurations(Unity.Collections.Allocator.Temp))
                    {
                        if (!configurations.IsCreated || (configurations.Length <= 0))
                        {
                            yield return null;
                            continue;
                        }

                        foreach (var config in configurations)
                        {
                            if (config.width >= 960 || config.height >= 960)
                            {
                                try
                                {
                                    cameraManager.currentConfiguration = config;
                                }
                                catch (Exception) { }
                                break;
                            }
                        }
                        break;
                    }
                }
            }
        }
    }
}
