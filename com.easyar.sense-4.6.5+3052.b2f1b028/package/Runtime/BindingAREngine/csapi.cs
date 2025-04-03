//=============================================================================================================================
//
// Copyright (c) 2015-2023 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
// EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
// and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
//
//=============================================================================================================================

#if UNITY_ANDROID && !UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
#if ENABLE_IL2CPP
using AOT;
#endif

namespace easyar.arengineinterop
{
    internal static partial class Detail
    {
#if UNITY_IOS && !UNITY_EDITOR
        public const String BindingLibraryName = "__Internal";
#else
        public const String BindingLibraryName = "AREngineInterop";
#endif
    }

    public abstract class RefBase : IDisposable
    {
        internal IntPtr cdata_;
        internal Action<IntPtr> deleter_;
        internal delegate void Retainer(IntPtr This, out IntPtr Return);
        internal Retainer retainer_;

        internal RefBase(IntPtr cdata, Action<IntPtr> deleter, Retainer retainer)
        {
            cdata_ = cdata;
            deleter_ = deleter;
            retainer_ = retainer;
        }

        internal IntPtr cdata
        {
            get
            {
                if (cdata_ == IntPtr.Zero) { throw new ObjectDisposedException(GetType().FullName); }
                return cdata_;
            }
        }

        ~RefBase()
        {
            if ((cdata_ != IntPtr.Zero) && (deleter_ != null))
            {
                deleter_(cdata_);
                cdata_ = IntPtr.Zero;
                deleter_ = null;
                retainer_ = null;
            }
        }

        public void Dispose()
        {
            if ((cdata_ != IntPtr.Zero) && (deleter_ != null))
            {
                deleter_(cdata_);
                cdata_ = IntPtr.Zero;
                deleter_ = null;
                retainer_ = null;
            }
            GC.SuppressFinalize(this);
        }

        protected abstract object CloneObject();
        public RefBase Clone()
        {
            return (RefBase)(CloneObject());
        }
    }

    internal static partial class Detail
    {
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_String_from_utf8(IntPtr begin, IntPtr end, out IntPtr Return);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_String_from_utf8_begin(IntPtr begin, out IntPtr Return);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr easyar_arengineinterop_String_begin(IntPtr This);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr easyar_arengineinterop_String_end(IntPtr This);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_String_copy(IntPtr This, out IntPtr Return);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_String__dtor(IntPtr This);

        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_AREngineCameraDevice__ctor(out IntPtr Return);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool easyar_arengineinterop_AREngineCameraDevice_isAvailable();
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool easyar_arengineinterop_AREngineCameraDevice_isDeviceSupported();
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int easyar_arengineinterop_AREngineCameraDevice_bufferCapacity(IntPtr This);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_AREngineCameraDevice_setBufferCapacity(IntPtr This, int capacity);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_AREngineCameraDevice_setInputFrameHandler(IntPtr This, FunctorOfVoidFromInputFrame inputFrameHandler);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_AREngineCameraDevice_setFocusMode(IntPtr This, AREngineCameraDeviceFocusMode focusMode);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_AREngineCameraDevice_setHighResMode(IntPtr This, [MarshalAs(UnmanagedType.I1)] bool enableHighResMode);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool easyar_arengineinterop_AREngineCameraDevice_start(IntPtr This);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_AREngineCameraDevice_stop(IntPtr This);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_AREngineCameraDevice_close(IntPtr This);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_AREngineCameraDevice_onPause(IntPtr This);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool easyar_arengineinterop_AREngineCameraDevice_onResume(IntPtr This);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_AREngineCameraDevice__dtor(IntPtr This);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_AREngineCameraDevice__retain(IntPtr This, out IntPtr Return);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr easyar_arengineinterop_AREngineCameraDevice__typeName(IntPtr This);

        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_AREngineDeviceListDownloader__ctor(out IntPtr Return);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_AREngineDeviceListDownloader_download(IntPtr This, OptionalOfInt timeoutMilliseconds, IntPtr callbackScheduler, FunctorOfVoidFromAREngineDeviceListDownloadStatusAndOptionalOfString onCompleted);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_AREngineDeviceListDownloader__dtor(IntPtr This);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_AREngineDeviceListDownloader__retain(IntPtr This, out IntPtr Return);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr easyar_arengineinterop_AREngineDeviceListDownloader__typeName(IntPtr This);

        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_Buffer_wrap(IntPtr ptr, int size, FunctorOfVoid deleter, out IntPtr Return);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_Buffer_create(int size, out IntPtr Return);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr easyar_arengineinterop_Buffer_data(IntPtr This);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int easyar_arengineinterop_Buffer_size(IntPtr This);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_Buffer_memoryCopy(IntPtr src, IntPtr dest, int length);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool easyar_arengineinterop_Buffer_tryCopyFrom(IntPtr This, IntPtr src, int srcIndex, int index, int length);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool easyar_arengineinterop_Buffer_tryCopyTo(IntPtr This, int index, IntPtr dest, int destIndex, int length);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_Buffer_partition(IntPtr This, int index, int length, out IntPtr Return);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_Buffer__dtor(IntPtr This);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_Buffer__retain(IntPtr This, out IntPtr Return);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr easyar_arengineinterop_Buffer__typeName(IntPtr This);

        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_BufferDictionary__ctor(out IntPtr Return);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int easyar_arengineinterop_BufferDictionary_count(IntPtr This);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool easyar_arengineinterop_BufferDictionary_contains(IntPtr This, IntPtr path);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_BufferDictionary_tryGet(IntPtr This, IntPtr path, out OptionalOfBuffer Return);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_BufferDictionary_set(IntPtr This, IntPtr path, IntPtr buffer);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool easyar_arengineinterop_BufferDictionary_remove(IntPtr This, IntPtr path);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_BufferDictionary_clear(IntPtr This);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_BufferDictionary__dtor(IntPtr This);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_BufferDictionary__retain(IntPtr This, out IntPtr Return);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr easyar_arengineinterop_BufferDictionary__typeName(IntPtr This);

        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_CallbackScheduler__dtor(IntPtr This);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_CallbackScheduler__retain(IntPtr This, out IntPtr Return);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr easyar_arengineinterop_CallbackScheduler__typeName(IntPtr This);

        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_DelayedCallbackScheduler__ctor(out IntPtr Return);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool easyar_arengineinterop_DelayedCallbackScheduler_runOne(IntPtr This);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_DelayedCallbackScheduler__dtor(IntPtr This);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_DelayedCallbackScheduler__retain(IntPtr This, out IntPtr Return);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr easyar_arengineinterop_DelayedCallbackScheduler__typeName(IntPtr This);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_castDelayedCallbackSchedulerToCallbackScheduler(IntPtr This, out IntPtr Return);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_tryCastCallbackSchedulerToDelayedCallbackScheduler(IntPtr This, out IntPtr Return);

        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_ImmediateCallbackScheduler_getDefault(out IntPtr Return);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_ImmediateCallbackScheduler__dtor(IntPtr This);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_ImmediateCallbackScheduler__retain(IntPtr This, out IntPtr Return);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr easyar_arengineinterop_ImmediateCallbackScheduler__typeName(IntPtr This);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_castImmediateCallbackSchedulerToCallbackScheduler(IntPtr This, out IntPtr Return);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_tryCastCallbackSchedulerToImmediateCallbackScheduler(IntPtr This, out IntPtr Return);

        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_CameraParameters__ctor(Vec2I imageSize, Vec2F focalLength, Vec2F principalPoint, CameraDeviceType cameraDeviceType, int cameraOrientation, out IntPtr Return);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern Vec2I easyar_arengineinterop_CameraParameters_size(IntPtr This);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern Vec2F easyar_arengineinterop_CameraParameters_focalLength(IntPtr This);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern Vec2F easyar_arengineinterop_CameraParameters_principalPoint(IntPtr This);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern CameraDeviceType easyar_arengineinterop_CameraParameters_cameraDeviceType(IntPtr This);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int easyar_arengineinterop_CameraParameters_cameraOrientation(IntPtr This);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_CameraParameters_createWithDefaultIntrinsics(Vec2I imageSize, CameraDeviceType cameraDeviceType, int cameraOrientation, out IntPtr Return);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_CameraParameters_getResized(IntPtr This, Vec2I imageSize, out IntPtr Return);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int easyar_arengineinterop_CameraParameters_imageOrientation(IntPtr This, int screenRotation);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool easyar_arengineinterop_CameraParameters_imageHorizontalFlip(IntPtr This, [MarshalAs(UnmanagedType.I1)] bool manualHorizontalFlip);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern Matrix44F easyar_arengineinterop_CameraParameters_projection(IntPtr This, float nearPlane, float farPlane, float viewportAspectRatio, int screenRotation, [MarshalAs(UnmanagedType.I1)] bool combiningFlip, [MarshalAs(UnmanagedType.I1)] bool manualHorizontalFlip);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern Matrix44F easyar_arengineinterop_CameraParameters_imageProjection(IntPtr This, float viewportAspectRatio, int screenRotation, [MarshalAs(UnmanagedType.I1)] bool combiningFlip, [MarshalAs(UnmanagedType.I1)] bool manualHorizontalFlip);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern Vec2F easyar_arengineinterop_CameraParameters_screenCoordinatesFromImageCoordinates(IntPtr This, float viewportAspectRatio, int screenRotation, [MarshalAs(UnmanagedType.I1)] bool combiningFlip, [MarshalAs(UnmanagedType.I1)] bool manualHorizontalFlip, Vec2F imageCoordinates);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern Vec2F easyar_arengineinterop_CameraParameters_imageCoordinatesFromScreenCoordinates(IntPtr This, float viewportAspectRatio, int screenRotation, [MarshalAs(UnmanagedType.I1)] bool combiningFlip, [MarshalAs(UnmanagedType.I1)] bool manualHorizontalFlip, Vec2F screenCoordinates);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool easyar_arengineinterop_CameraParameters_equalsTo(IntPtr This, IntPtr other);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_CameraParameters__dtor(IntPtr This);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_CameraParameters__retain(IntPtr This, out IntPtr Return);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr easyar_arengineinterop_CameraParameters__typeName(IntPtr This);

        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int easyar_arengineinterop_Engine_schemaHash();
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool easyar_arengineinterop_Engine_initialize(IntPtr licenseKey);

        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int easyar_arengineinterop_InputFrame_index(IntPtr This);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_InputFrame_image(IntPtr This, out IntPtr Return);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool easyar_arengineinterop_InputFrame_hasCameraParameters(IntPtr This);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_InputFrame_cameraParameters(IntPtr This, out IntPtr Return);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool easyar_arengineinterop_InputFrame_hasTemporalInformation(IntPtr This);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern double easyar_arengineinterop_InputFrame_timestamp(IntPtr This);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool easyar_arengineinterop_InputFrame_hasSpatialInformation(IntPtr This);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern Matrix44F easyar_arengineinterop_InputFrame_cameraTransform(IntPtr This);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern MotionTrackingStatus easyar_arengineinterop_InputFrame_trackingStatus(IntPtr This);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_InputFrame_create(IntPtr image, IntPtr cameraParameters, double timestamp, Matrix44F cameraTransform, MotionTrackingStatus trackingStatus, out IntPtr Return);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_InputFrame_createWithImageAndCameraParametersAndTemporal(IntPtr image, IntPtr cameraParameters, double timestamp, out IntPtr Return);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_InputFrame_createWithImageAndCameraParameters(IntPtr image, IntPtr cameraParameters, out IntPtr Return);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_InputFrame_createWithImage(IntPtr image, out IntPtr Return);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_InputFrame__dtor(IntPtr This);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_InputFrame__retain(IntPtr This, out IntPtr Return);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr easyar_arengineinterop_InputFrame__typeName(IntPtr This);

        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_FrameFilterResult__dtor(IntPtr This);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_FrameFilterResult__retain(IntPtr This, out IntPtr Return);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr easyar_arengineinterop_FrameFilterResult__typeName(IntPtr This);

        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_OutputFrame__ctor(IntPtr inputFrame, IntPtr results, out IntPtr Return);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int easyar_arengineinterop_OutputFrame_index(IntPtr This);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_OutputFrame_inputFrame(IntPtr This, out IntPtr Return);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_OutputFrame_results(IntPtr This, out IntPtr Return);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_OutputFrame__dtor(IntPtr This);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_OutputFrame__retain(IntPtr This, out IntPtr Return);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr easyar_arengineinterop_OutputFrame__typeName(IntPtr This);

        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_FeedbackFrame__ctor(IntPtr inputFrame, OptionalOfOutputFrame previousOutputFrame, out IntPtr Return);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_FeedbackFrame_inputFrame(IntPtr This, out IntPtr Return);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_FeedbackFrame_previousOutputFrame(IntPtr This, out OptionalOfOutputFrame Return);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_FeedbackFrame__dtor(IntPtr This);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_FeedbackFrame__retain(IntPtr This, out IntPtr Return);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr easyar_arengineinterop_FeedbackFrame__typeName(IntPtr This);

        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_Image__ctor(IntPtr buffer, PixelFormat format, int width, int height, out IntPtr Return);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_Image_buffer(IntPtr This, out IntPtr Return);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern PixelFormat easyar_arengineinterop_Image_format(IntPtr This);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int easyar_arengineinterop_Image_width(IntPtr This);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int easyar_arengineinterop_Image_height(IntPtr This);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int easyar_arengineinterop_Image_pixelWidth(IntPtr This);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int easyar_arengineinterop_Image_pixelHeight(IntPtr This);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_Image_create(IntPtr buffer, PixelFormat format, int width, int height, int pixelWidth, int pixelHeight, out IntPtr Return);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_Image__dtor(IntPtr This);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_Image__retain(IntPtr This, out IntPtr Return);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr easyar_arengineinterop_Image__typeName(IntPtr This);

        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_ListOfOptionalOfFrameFilterResult__ctor(IntPtr begin, IntPtr end, out IntPtr Return);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_ListOfOptionalOfFrameFilterResult__dtor(IntPtr This);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void easyar_arengineinterop_ListOfOptionalOfFrameFilterResult_copy(IntPtr This, out IntPtr Return);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int easyar_arengineinterop_ListOfOptionalOfFrameFilterResult_size(IntPtr This);
        [DllImport(BindingLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern OptionalOfFrameFilterResult easyar_arengineinterop_ListOfOptionalOfFrameFilterResult_at(IntPtr This, int index);

        private static Dictionary<String, Func<IntPtr, RefBase>> TypeNameToConstructor = new Dictionary<String, Func<IntPtr, RefBase>>
        {
            { "AREngineCameraDevice", cdata => new AREngineCameraDevice(cdata, easyar_arengineinterop_AREngineCameraDevice__dtor, easyar_arengineinterop_AREngineCameraDevice__retain) },
            { "AREngineDeviceListDownloader", cdata => new AREngineDeviceListDownloader(cdata, easyar_arengineinterop_AREngineDeviceListDownloader__dtor, easyar_arengineinterop_AREngineDeviceListDownloader__retain) },
            { "Buffer", cdata => new Buffer(cdata, easyar_arengineinterop_Buffer__dtor, easyar_arengineinterop_Buffer__retain) },
            { "BufferDictionary", cdata => new BufferDictionary(cdata, easyar_arengineinterop_BufferDictionary__dtor, easyar_arengineinterop_BufferDictionary__retain) },
            { "CallbackScheduler", cdata => new CallbackScheduler(cdata, easyar_arengineinterop_CallbackScheduler__dtor, easyar_arengineinterop_CallbackScheduler__retain) },
            { "DelayedCallbackScheduler", cdata => new DelayedCallbackScheduler(cdata, easyar_arengineinterop_DelayedCallbackScheduler__dtor, easyar_arengineinterop_DelayedCallbackScheduler__retain) },
            { "ImmediateCallbackScheduler", cdata => new ImmediateCallbackScheduler(cdata, easyar_arengineinterop_ImmediateCallbackScheduler__dtor, easyar_arengineinterop_ImmediateCallbackScheduler__retain) },
            { "CameraParameters", cdata => new CameraParameters(cdata, easyar_arengineinterop_CameraParameters__dtor, easyar_arengineinterop_CameraParameters__retain) },
            { "InputFrame", cdata => new InputFrame(cdata, easyar_arengineinterop_InputFrame__dtor, easyar_arengineinterop_InputFrame__retain) },
            { "FrameFilterResult", cdata => new FrameFilterResult(cdata, easyar_arengineinterop_FrameFilterResult__dtor, easyar_arengineinterop_FrameFilterResult__retain) },
            { "OutputFrame", cdata => new OutputFrame(cdata, easyar_arengineinterop_OutputFrame__dtor, easyar_arengineinterop_OutputFrame__retain) },
            { "FeedbackFrame", cdata => new FeedbackFrame(cdata, easyar_arengineinterop_FeedbackFrame__dtor, easyar_arengineinterop_FeedbackFrame__retain) },
            { "Image", cdata => new Image(cdata, easyar_arengineinterop_Image__dtor, easyar_arengineinterop_Image__retain) },
        };

        public class AutoRelease : IDisposable
        {
            private List<Action> actions;

            public void Add(Action deleter)
            {
                if (actions == null) { actions = new List<Action>(); }
                actions.Add(deleter);
            }
            public T Add<T>(T p, Action<T> deleter)
            {
                if (p.Equals(default(T))) { return p; }
                if (actions == null) { actions = new List<Action>(); }
                actions.Add(() => deleter(p));
                return p;
            }

            public void Dispose()
            {
                if (actions != null)
                {
                    foreach (var a in actions)
                    {
                        a();
                    }
                    actions = null;
                }
            }
        }

        public static IntPtr String_to_c(AutoRelease ar, string s)
        {
            if (s == null) { throw new ArgumentNullException(); }
            var bytes = System.Text.Encoding.UTF8.GetBytes(s);
            var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            try
            {
                var beginPtr = Marshal.UnsafeAddrOfPinnedArrayElement(bytes, 0);
                var endPtr = new IntPtr(beginPtr.ToInt64() + bytes.Length);
                var returnValue = IntPtr.Zero;
                easyar_arengineinterop_String_from_utf8(beginPtr, endPtr, out returnValue);
                return ar.Add(returnValue, easyar_arengineinterop_String__dtor);
            }
            finally
            {
                handle.Free();
            }
        }
        public static IntPtr String_to_c_inner(string s)
        {
            if (s == null) { throw new ArgumentNullException(); }
            var bytes = System.Text.Encoding.UTF8.GetBytes(s);
            var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            try
            {
                var beginPtr = Marshal.UnsafeAddrOfPinnedArrayElement(bytes, 0);
                var endPtr = new IntPtr(beginPtr.ToInt64() + bytes.Length);
                var returnValue = IntPtr.Zero;
                easyar_arengineinterop_String_from_utf8(beginPtr, endPtr, out returnValue);
                return returnValue;
            }
            finally
            {
                handle.Free();
            }
        }
        public static String String_from_c(AutoRelease ar, IntPtr ptr)
        {
            if (ptr == IntPtr.Zero) { throw new ArgumentNullException(); }
            ar.Add(ptr, easyar_arengineinterop_String__dtor);
            IntPtr beginPtr = easyar_arengineinterop_String_begin(ptr);
            IntPtr endPtr = easyar_arengineinterop_String_end(ptr);
            var length = (int)(endPtr.ToInt64() - beginPtr.ToInt64());
            var bytes = new byte[length];
            Marshal.Copy(beginPtr, bytes, 0, length);
            return System.Text.Encoding.UTF8.GetString(bytes);
        }
        public static String String_from_cstring(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero) { throw new ArgumentNullException(); }
            var length = 0;
            while (true)
            {
                var b = Marshal.ReadByte(ptr, length);
                if (b == 0) { break; }
                length += 1;
            }
            var bytes = new byte[length];
            Marshal.Copy(ptr, bytes, 0, length);
            return System.Text.Encoding.UTF8.GetString(bytes);
        }

        public static T Object_from_c<T>(IntPtr ptr, Func<IntPtr, IntPtr> typeNameGetter)
        {
            if (ptr == IntPtr.Zero) { throw new ArgumentNullException(); }
            var typeName = String_from_cstring(typeNameGetter(ptr));
            if (!TypeNameToConstructor.ContainsKey(typeName)) { throw new InvalidOperationException("ConstructorNotExistForType"); }
            var ctor = TypeNameToConstructor[typeName];
            var o = ctor(ptr);
            return (T)(Object)(o);
        }
        public static TValue map<TKey, TValue>(this TKey v, Func<TKey, TValue> f)
        {
            return f(v);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct FunctorOfVoidFromInputFrame
        {
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)] public delegate void FunctionDelegate(IntPtr state, IntPtr arg0, out IntPtr exception);
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)] public delegate void DestroyDelegate(IntPtr _state);

            public IntPtr _state;
            public FunctionDelegate _func;
            public DestroyDelegate _destroy;
        }
#if ENABLE_IL2CPP
        [MonoPInvokeCallback(typeof(FunctorOfVoidFromInputFrame.FunctionDelegate))]
#endif
        public static void FunctorOfVoidFromInputFrame_func(IntPtr state, IntPtr arg0, out IntPtr exception)
        {
            exception = IntPtr.Zero;
            try
            {
                using (var ar = new AutoRelease())
                {
                    var varg0 = arg0;
                    easyar_arengineinterop_InputFrame__retain(varg0, out varg0);
                    var sarg0 = Object_from_c<InputFrame>(varg0, easyar_arengineinterop_InputFrame__typeName);
                    ar.Add(() => sarg0.Dispose());
                    var f = (Action<InputFrame>)((GCHandle)(state)).Target;
                    f(sarg0);
                }
            }
            catch (Exception ex)
            {
                exception = Detail.String_to_c_inner(ex.ToString());
            }
        }
#if ENABLE_IL2CPP
        [MonoPInvokeCallback(typeof(FunctorOfVoidFromInputFrame.DestroyDelegate))]
#endif
        public static void FunctorOfVoidFromInputFrame_destroy(IntPtr _state)
        {
            ((GCHandle)(_state)).Free();
        }
        public static FunctorOfVoidFromInputFrame FunctorOfVoidFromInputFrame_to_c(Action<InputFrame> f)
        {
            if (f == null) { throw new ArgumentNullException(); }
            var s = GCHandle.Alloc(f, GCHandleType.Normal);
            return new FunctorOfVoidFromInputFrame { _state = (IntPtr)(s), _func = FunctorOfVoidFromInputFrame_func, _destroy = FunctorOfVoidFromInputFrame_destroy };
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct FunctorOfVoid
        {
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)] public delegate void FunctionDelegate(IntPtr state, out IntPtr exception);
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)] public delegate void DestroyDelegate(IntPtr _state);

            public IntPtr _state;
            public FunctionDelegate _func;
            public DestroyDelegate _destroy;
        }
#if ENABLE_IL2CPP
        [MonoPInvokeCallback(typeof(FunctorOfVoid.FunctionDelegate))]
#endif
        public static void FunctorOfVoid_func(IntPtr state, out IntPtr exception)
        {
            exception = IntPtr.Zero;
            try
            {
                using (var ar = new AutoRelease())
                {
                    var f = (Action)((GCHandle)(state)).Target;
                    f();
                }
            }
            catch (Exception ex)
            {
                exception = Detail.String_to_c_inner(ex.ToString());
            }
        }
#if ENABLE_IL2CPP
        [MonoPInvokeCallback(typeof(FunctorOfVoid.DestroyDelegate))]
#endif
        public static void FunctorOfVoid_destroy(IntPtr _state)
        {
            ((GCHandle)(_state)).Free();
        }
        public static FunctorOfVoid FunctorOfVoid_to_c(Action f)
        {
            if (f == null) { throw new ArgumentNullException(); }
            var s = GCHandle.Alloc(f, GCHandleType.Normal);
            return new FunctorOfVoid { _state = (IntPtr)(s), _func = FunctorOfVoid_func, _destroy = FunctorOfVoid_destroy };
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct OptionalOfInt
        {
            private Byte has_value_;
            public bool has_value { get { return has_value_ != 0; } set { has_value_ = (Byte)(value ? 1 : 0); } }
            public int value;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct FunctorOfVoidFromAREngineDeviceListDownloadStatusAndOptionalOfString
        {
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)] public delegate void FunctionDelegate(IntPtr state, AREngineDeviceListDownloadStatus arg0, OptionalOfString arg1, out IntPtr exception);
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)] public delegate void DestroyDelegate(IntPtr _state);

            public IntPtr _state;
            public FunctionDelegate _func;
            public DestroyDelegate _destroy;
        }
#if ENABLE_IL2CPP
        [MonoPInvokeCallback(typeof(FunctorOfVoidFromAREngineDeviceListDownloadStatusAndOptionalOfString.FunctionDelegate))]
#endif
        public static void FunctorOfVoidFromAREngineDeviceListDownloadStatusAndOptionalOfString_func(IntPtr state, AREngineDeviceListDownloadStatus arg0, OptionalOfString arg1, out IntPtr exception)
        {
            exception = IntPtr.Zero;
            try
            {
                using (var ar = new AutoRelease())
                {
                    var sarg0 = arg0;
                    var varg1 = arg1;
                    if (varg1.has_value) { easyar_arengineinterop_String_copy(varg1.value, out varg1.value); }
                    var sarg1 = varg1.map(p => p.has_value ? String_from_c(ar, p.value) : Optional<string>.Empty);
                    var f = (Action<AREngineDeviceListDownloadStatus, Optional<string>>)((GCHandle)(state)).Target;
                    f(sarg0, sarg1);
                }
            }
            catch (Exception ex)
            {
                exception = Detail.String_to_c_inner(ex.ToString());
            }
        }
#if ENABLE_IL2CPP
        [MonoPInvokeCallback(typeof(FunctorOfVoidFromAREngineDeviceListDownloadStatusAndOptionalOfString.DestroyDelegate))]
#endif
        public static void FunctorOfVoidFromAREngineDeviceListDownloadStatusAndOptionalOfString_destroy(IntPtr _state)
        {
            ((GCHandle)(_state)).Free();
        }
        public static FunctorOfVoidFromAREngineDeviceListDownloadStatusAndOptionalOfString FunctorOfVoidFromAREngineDeviceListDownloadStatusAndOptionalOfString_to_c(Action<AREngineDeviceListDownloadStatus, Optional<string>> f)
        {
            if (f == null) { throw new ArgumentNullException(); }
            var s = GCHandle.Alloc(f, GCHandleType.Normal);
            return new FunctorOfVoidFromAREngineDeviceListDownloadStatusAndOptionalOfString { _state = (IntPtr)(s), _func = FunctorOfVoidFromAREngineDeviceListDownloadStatusAndOptionalOfString_func, _destroy = FunctorOfVoidFromAREngineDeviceListDownloadStatusAndOptionalOfString_destroy };
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct OptionalOfString
        {
            private Byte has_value_;
            public bool has_value { get { return has_value_ != 0; } set { has_value_ = (Byte)(value ? 1 : 0); } }
            public IntPtr value;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct OptionalOfBuffer
        {
            private Byte has_value_;
            public bool has_value { get { return has_value_ != 0; } set { has_value_ = (Byte)(value ? 1 : 0); } }
            public IntPtr value;
        }

        public static IntPtr ListOfOptionalOfFrameFilterResult_to_c(AutoRelease ar, List<Optional<FrameFilterResult>> l)
        {
            if (l == null) { throw new ArgumentNullException(); }
            var arr = l.Select(e => e.map(p => p.OnSome ? new OptionalOfFrameFilterResult { has_value = true, value = p.Value.cdata } : new OptionalOfFrameFilterResult { has_value = false, value = default(IntPtr) })).ToArray();
            var handle = GCHandle.Alloc(arr, GCHandleType.Pinned);
            try
            {
                var beginPtr = Marshal.UnsafeAddrOfPinnedArrayElement(arr, 0);
                var endPtr = new IntPtr(beginPtr.ToInt64() + IntPtr.Size * arr.Length);
                var ptr = IntPtr.Zero;
                easyar_arengineinterop_ListOfOptionalOfFrameFilterResult__ctor(beginPtr, endPtr, out ptr);
                return ar.Add(ptr, easyar_arengineinterop_ListOfOptionalOfFrameFilterResult__dtor);
            }
            finally
            {
                handle.Free();
            }
        }
        public static List<Optional<FrameFilterResult>> ListOfOptionalOfFrameFilterResult_from_c(AutoRelease ar, IntPtr l)
        {
            if (l == IntPtr.Zero) { throw new ArgumentNullException(); }
            ar.Add(l, easyar_arengineinterop_ListOfOptionalOfFrameFilterResult__dtor);
            var size = easyar_arengineinterop_ListOfOptionalOfFrameFilterResult_size(l);
            var values = new List<Optional<FrameFilterResult>>();
            values.Capacity = size;
            for (int k = 0; k < size; k += 1)
            {
                var v = easyar_arengineinterop_ListOfOptionalOfFrameFilterResult_at(l, k);
                if (v.has_value) { easyar_arengineinterop_FrameFilterResult__retain(v.value, out v.value); }
                values.Add(v.map(p => p.has_value ? Object_from_c<FrameFilterResult>(p.value, easyar_arengineinterop_FrameFilterResult__typeName) : Optional<FrameFilterResult>.Empty));
            }
            return values;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct OptionalOfFrameFilterResult
        {
            private Byte has_value_;
            public bool has_value { get { return has_value_ != 0; } set { has_value_ = (Byte)(value ? 1 : 0); } }
            public IntPtr value;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct OptionalOfOutputFrame
        {
            private Byte has_value_;
            public bool has_value { get { return has_value_ != 0; } set { has_value_ = (Byte)(value ? 1 : 0); } }
            public IntPtr value;
        }

    }

    public enum AREngineCameraDeviceFocusMode
    {
        Auto = 0,
        Fixed = 1,
    }

    public class AREngineCameraDevice : RefBase
    {
        internal AREngineCameraDevice(IntPtr cdata, Action<IntPtr> deleter, Retainer retainer) : base(cdata, deleter, retainer)
        {
        }
        protected override object CloneObject()
        {
            var cdata_new = IntPtr.Zero;
            if (retainer_ != null) { retainer_(cdata, out cdata_new); }
            return new AREngineCameraDevice(cdata_new, deleter_, retainer_);
        }
        public new AREngineCameraDevice Clone()
        {
            return (AREngineCameraDevice)(CloneObject());
        }
        public AREngineCameraDevice() : base(IntPtr.Zero, Detail.easyar_arengineinterop_AREngineCameraDevice__dtor, Detail.easyar_arengineinterop_AREngineCameraDevice__retain)
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = IntPtr.Zero;
                Detail.easyar_arengineinterop_AREngineCameraDevice__ctor(out _return_value_);
                cdata_ = _return_value_;
            }
        }
        public static bool isAvailable()
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = Detail.easyar_arengineinterop_AREngineCameraDevice_isAvailable();
                return _return_value_;
            }
        }
        public static bool isDeviceSupported()
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = Detail.easyar_arengineinterop_AREngineCameraDevice_isDeviceSupported();
                return _return_value_;
            }
        }
        public virtual int bufferCapacity()
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = Detail.easyar_arengineinterop_AREngineCameraDevice_bufferCapacity(cdata);
                return _return_value_;
            }
        }
        public virtual void setBufferCapacity(int capacity)
        {
            using (var ar = new Detail.AutoRelease())
            {
                Detail.easyar_arengineinterop_AREngineCameraDevice_setBufferCapacity(cdata, capacity);
            }
        }
        public virtual void setInputFrameHandler(Action<InputFrame> inputFrameHandler)
        {
            using (var ar = new Detail.AutoRelease())
            {
                Detail.easyar_arengineinterop_AREngineCameraDevice_setInputFrameHandler(cdata, Detail.FunctorOfVoidFromInputFrame_to_c(inputFrameHandler));
            }
        }
        public virtual void setFocusMode(AREngineCameraDeviceFocusMode focusMode)
        {
            using (var ar = new Detail.AutoRelease())
            {
                Detail.easyar_arengineinterop_AREngineCameraDevice_setFocusMode(cdata, focusMode);
            }
        }
        public virtual void setHighResMode(bool enableHighResMode)
        {
            using (var ar = new Detail.AutoRelease())
            {
                Detail.easyar_arengineinterop_AREngineCameraDevice_setHighResMode(cdata, enableHighResMode);
            }
        }
        public virtual bool start()
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = Detail.easyar_arengineinterop_AREngineCameraDevice_start(cdata);
                return _return_value_;
            }
        }
        public virtual void stop()
        {
            using (var ar = new Detail.AutoRelease())
            {
                Detail.easyar_arengineinterop_AREngineCameraDevice_stop(cdata);
            }
        }
        public virtual void close()
        {
            using (var ar = new Detail.AutoRelease())
            {
                Detail.easyar_arengineinterop_AREngineCameraDevice_close(cdata);
            }
        }
        public virtual void onPause()
        {
            using (var ar = new Detail.AutoRelease())
            {
                Detail.easyar_arengineinterop_AREngineCameraDevice_onPause(cdata);
            }
        }
        public virtual bool onResume()
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = Detail.easyar_arengineinterop_AREngineCameraDevice_onResume(cdata);
                return _return_value_;
            }
        }
    }

    public enum AREngineDeviceListDownloadStatus
    {
        /// <summary>
        /// Download successful.
        /// </summary>
        Successful = 0,
        /// <summary>
        /// Data is already latest.
        /// </summary>
        NotModified = 1,
        /// <summary>
        /// Connection error
        /// </summary>
        ConnectionError = 2,
        /// <summary>
        /// Unexpected error
        /// </summary>
        UnexpectedError = 3,
    }

    /// <summary>
    /// AREngineDeviceListDownloader is used for download and update of device list data in AREngineCameraDevice.
    /// </summary>
    public class AREngineDeviceListDownloader : RefBase
    {
        internal AREngineDeviceListDownloader(IntPtr cdata, Action<IntPtr> deleter, Retainer retainer) : base(cdata, deleter, retainer)
        {
        }
        protected override object CloneObject()
        {
            var cdata_new = IntPtr.Zero;
            if (retainer_ != null) { retainer_(cdata, out cdata_new); }
            return new AREngineDeviceListDownloader(cdata_new, deleter_, retainer_);
        }
        public new AREngineDeviceListDownloader Clone()
        {
            return (AREngineDeviceListDownloader)(CloneObject());
        }
        public AREngineDeviceListDownloader() : base(IntPtr.Zero, Detail.easyar_arengineinterop_AREngineDeviceListDownloader__dtor, Detail.easyar_arengineinterop_AREngineDeviceListDownloader__retain)
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = IntPtr.Zero;
                Detail.easyar_arengineinterop_AREngineDeviceListDownloader__ctor(out _return_value_);
                cdata_ = _return_value_;
            }
        }
        public virtual void download(Optional<int> timeoutMilliseconds, CallbackScheduler callbackScheduler, Action<AREngineDeviceListDownloadStatus, Optional<string>> onCompleted)
        {
            using (var ar = new Detail.AutoRelease())
            {
                Detail.easyar_arengineinterop_AREngineDeviceListDownloader_download(cdata, timeoutMilliseconds.map(p => p.OnSome ? new Detail.OptionalOfInt { has_value = true, value = p.Value } : new Detail.OptionalOfInt { has_value = false, value = default(int) }), callbackScheduler.cdata, Detail.FunctorOfVoidFromAREngineDeviceListDownloadStatusAndOptionalOfString_to_c(onCompleted));
            }
        }
    }

    /// <summary>
    /// Buffer stores a raw byte array, which can be used to access image data.
    /// To access image data in Java API, get buffer from `Image`_ and copy to a Java byte array.
    /// You can always access image data since the first version of EasyAR Sense. Refer to `Image`_ .
    /// </summary>
    public class Buffer : RefBase
    {
        internal Buffer(IntPtr cdata, Action<IntPtr> deleter, Retainer retainer) : base(cdata, deleter, retainer)
        {
        }
        protected override object CloneObject()
        {
            var cdata_new = IntPtr.Zero;
            if (retainer_ != null) { retainer_(cdata, out cdata_new); }
            return new Buffer(cdata_new, deleter_, retainer_);
        }
        public new Buffer Clone()
        {
            return (Buffer)(CloneObject());
        }
        /// <summary>
        /// Wraps a raw memory block. When Buffer is released by all holders, deleter callback will be invoked to execute user-defined memory destruction. deleter must be thread-safe.
        /// </summary>
        public static Buffer wrap(IntPtr ptr, int size, Action deleter)
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = default(IntPtr);
                Detail.easyar_arengineinterop_Buffer_wrap(ptr, size, Detail.FunctorOfVoid_to_c(deleter), out _return_value_);
                return Detail.Object_from_c<Buffer>(_return_value_, Detail.easyar_arengineinterop_Buffer__typeName);
            }
        }
        /// <summary>
        /// Creates a Buffer of specified byte size.
        /// </summary>
        public static Buffer create(int size)
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = default(IntPtr);
                Detail.easyar_arengineinterop_Buffer_create(size, out _return_value_);
                return Detail.Object_from_c<Buffer>(_return_value_, Detail.easyar_arengineinterop_Buffer__typeName);
            }
        }
        /// <summary>
        /// Returns raw data address.
        /// </summary>
        public virtual IntPtr data()
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = Detail.easyar_arengineinterop_Buffer_data(cdata);
                return _return_value_;
            }
        }
        /// <summary>
        /// Byte size of raw data.
        /// </summary>
        public virtual int size()
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = Detail.easyar_arengineinterop_Buffer_size(cdata);
                return _return_value_;
            }
        }
        /// <summary>
        /// Copies raw memory. It can be used in languages or platforms without complete support for memory operations.
        /// </summary>
        public static void memoryCopy(IntPtr src, IntPtr dest, int length)
        {
            using (var ar = new Detail.AutoRelease())
            {
                Detail.easyar_arengineinterop_Buffer_memoryCopy(src, dest, length);
            }
        }
        /// <summary>
        /// Tries to copy data from a raw memory address into Buffer. If copy succeeds, it returns true, or else it returns false. Possible failure causes includes: source or destination data range overflow.
        /// </summary>
        public virtual bool tryCopyFrom(IntPtr src, int srcIndex, int index, int length)
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = Detail.easyar_arengineinterop_Buffer_tryCopyFrom(cdata, src, srcIndex, index, length);
                return _return_value_;
            }
        }
        /// <summary>
        /// Tries to copy data from Buffer to user array. If copy succeeds, it returns true, or else it returns false. Possible failure causes includes: source or destination data range overflow.
        /// </summary>
        public virtual bool tryCopyTo(int index, IntPtr dest, int destIndex, int length)
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = Detail.easyar_arengineinterop_Buffer_tryCopyTo(cdata, index, dest, destIndex, length);
                return _return_value_;
            }
        }
        /// <summary>
        /// Creates a sub-buffer with a reference to the original Buffer. A Buffer will only be released after all its sub-buffers are released.
        /// </summary>
        public virtual Buffer partition(int index, int length)
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = default(IntPtr);
                Detail.easyar_arengineinterop_Buffer_partition(cdata, index, length, out _return_value_);
                return Detail.Object_from_c<Buffer>(_return_value_, Detail.easyar_arengineinterop_Buffer__typeName);
            }
        }
        public static Buffer wrapByteArray(byte[] bytes)
        {
            var Length = bytes.Length;
            var h = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            return Buffer.wrap(h.AddrOfPinnedObject(), Length, () => h.Free());
        }
        public static Buffer wrapByteArray(byte[] bytes, int index, int length)
        {
            return wrapByteArray(bytes, index, length, () => { });
        }
        public static Buffer wrapByteArray(byte[] bytes, int index, int length, Action deleter)
        {
            if ((length < 0) || (index < 0) || (index > bytes.Length) || (index + length > bytes.Length))
            {
                throw new ArgumentException("BufferRangeOverflow");
            }
            var h = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            var ptr = new IntPtr(h.AddrOfPinnedObject().ToInt64() + index);
            return Buffer.wrap(ptr, length, () =>
            {
                h.Free();
                if (deleter != null)
                {
                    deleter();
                }
            });
        }
        public void copyFromByteArray(byte[] src)
        {
            copyFromByteArray(src, 0, 0, src.Length);
        }
        public void copyFromByteArray(byte[] src, int srcIndex, int index, int length)
        {
            var srcSize = src.Length;
            var destSize = size();
            if ((length < 0) || (srcIndex < 0) || (srcIndex > srcSize) || (srcIndex + length > srcSize) || (index < 0) || (index > destSize) || (index + length > destSize))
            {
                throw new ArgumentException("BufferRangeOverflow");
            }
            Marshal.Copy(src, srcIndex, data(), length);
        }
        public void copyToByteArray(byte[] dest)
        {
            copyToByteArray(0, dest, 0, size());
        }
        public void copyToByteArray(int index, byte[] dest, int destIndex, int length)
        {
            var srcSize = size();
            var destSize = dest.Length;
            if ((length < 0) || (index < 0) || (index > srcSize) || (index + length > srcSize) || (destIndex < 0) || (destIndex > destSize) || (destIndex + length > destSize))
            {
                throw new ArgumentException("BufferRangeOverflow");
            }
            var ptr = new IntPtr(data().ToInt64() + index);
            Marshal.Copy(ptr, dest, destIndex, length);
        }
    }

    /// <summary>
    /// A mapping from file path to `Buffer`_ . It can be used to represent multiple files in the memory.
    /// </summary>
    public class BufferDictionary : RefBase
    {
        internal BufferDictionary(IntPtr cdata, Action<IntPtr> deleter, Retainer retainer) : base(cdata, deleter, retainer)
        {
        }
        protected override object CloneObject()
        {
            var cdata_new = IntPtr.Zero;
            if (retainer_ != null) { retainer_(cdata, out cdata_new); }
            return new BufferDictionary(cdata_new, deleter_, retainer_);
        }
        public new BufferDictionary Clone()
        {
            return (BufferDictionary)(CloneObject());
        }
        public BufferDictionary() : base(IntPtr.Zero, Detail.easyar_arengineinterop_BufferDictionary__dtor, Detail.easyar_arengineinterop_BufferDictionary__retain)
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = IntPtr.Zero;
                Detail.easyar_arengineinterop_BufferDictionary__ctor(out _return_value_);
                cdata_ = _return_value_;
            }
        }
        /// <summary>
        /// Current file count.
        /// </summary>
        public virtual int count()
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = Detail.easyar_arengineinterop_BufferDictionary_count(cdata);
                return _return_value_;
            }
        }
        /// <summary>
        /// Checks if a specified path is in the dictionary.
        /// </summary>
        public virtual bool contains(string path)
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = Detail.easyar_arengineinterop_BufferDictionary_contains(cdata, Detail.String_to_c(ar, path));
                return _return_value_;
            }
        }
        /// <summary>
        /// Tries to get the corresponding `Buffer`_ for a specified path.
        /// </summary>
        public virtual Optional<Buffer> tryGet(string path)
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = default(Detail.OptionalOfBuffer);
                Detail.easyar_arengineinterop_BufferDictionary_tryGet(cdata, Detail.String_to_c(ar, path), out _return_value_);
                return _return_value_.map(p => p.has_value ? Detail.Object_from_c<Buffer>(p.value, Detail.easyar_arengineinterop_Buffer__typeName) : Optional<Buffer>.Empty);
            }
        }
        /// <summary>
        /// Sets `Buffer`_ for a specified path.
        /// </summary>
        public virtual void @set(string path, Buffer buffer)
        {
            using (var ar = new Detail.AutoRelease())
            {
                Detail.easyar_arengineinterop_BufferDictionary_set(cdata, Detail.String_to_c(ar, path), buffer.cdata);
            }
        }
        /// <summary>
        /// Removes a specified path.
        /// </summary>
        public virtual bool remove(string path)
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = Detail.easyar_arengineinterop_BufferDictionary_remove(cdata, Detail.String_to_c(ar, path));
                return _return_value_;
            }
        }
        /// <summary>
        /// Clears the dictionary.
        /// </summary>
        public virtual void clear()
        {
            using (var ar = new Detail.AutoRelease())
            {
                Detail.easyar_arengineinterop_BufferDictionary_clear(cdata);
            }
        }
    }

    /// <summary>
    /// Callback scheduler.
    /// There are two subclasses: `DelayedCallbackScheduler`_ and `ImmediateCallbackScheduler`_ .
    /// `DelayedCallbackScheduler`_ is used to delay callback to be invoked manually, and it can be used in single-threaded environments (such as various UI environments).
    /// `ImmediateCallbackScheduler`_ is used to mark callback to be invoked when event is dispatched, and it can be used in multi-threaded environments (such as server or service daemon).
    /// </summary>
    public class CallbackScheduler : RefBase
    {
        internal CallbackScheduler(IntPtr cdata, Action<IntPtr> deleter, Retainer retainer) : base(cdata, deleter, retainer)
        {
        }
        protected override object CloneObject()
        {
            var cdata_new = IntPtr.Zero;
            if (retainer_ != null) { retainer_(cdata, out cdata_new); }
            return new CallbackScheduler(cdata_new, deleter_, retainer_);
        }
        public new CallbackScheduler Clone()
        {
            return (CallbackScheduler)(CloneObject());
        }
    }

    /// <summary>
    /// Delayed callback scheduler.
    /// It is used to delay callback to be invoked manually, and it can be used in single-threaded environments (such as various UI environments).
    /// All members of this class is thread-safe.
    /// </summary>
    public class DelayedCallbackScheduler : CallbackScheduler
    {
        internal DelayedCallbackScheduler(IntPtr cdata, Action<IntPtr> deleter, Retainer retainer) : base(cdata, deleter, retainer)
        {
        }
        protected override object CloneObject()
        {
            var cdata_new = IntPtr.Zero;
            if (retainer_ != null) { retainer_(cdata, out cdata_new); }
            return new DelayedCallbackScheduler(cdata_new, deleter_, retainer_);
        }
        public new DelayedCallbackScheduler Clone()
        {
            return (DelayedCallbackScheduler)(CloneObject());
        }
        public DelayedCallbackScheduler() : base(IntPtr.Zero, Detail.easyar_arengineinterop_DelayedCallbackScheduler__dtor, Detail.easyar_arengineinterop_DelayedCallbackScheduler__retain)
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = IntPtr.Zero;
                Detail.easyar_arengineinterop_DelayedCallbackScheduler__ctor(out _return_value_);
                cdata_ = _return_value_;
            }
        }
        /// <summary>
        /// Executes a callback. If there is no callback to execute, false is returned.
        /// </summary>
        public virtual bool runOne()
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = Detail.easyar_arengineinterop_DelayedCallbackScheduler_runOne(cdata);
                return _return_value_;
            }
        }
    }

    /// <summary>
    /// Immediate callback scheduler.
    /// It is used to mark callback to be invoked when event is dispatched, and it can be used in multi-threaded environments (such as server or service daemon).
    /// All members of this class is thread-safe.
    /// </summary>
    public class ImmediateCallbackScheduler : CallbackScheduler
    {
        internal ImmediateCallbackScheduler(IntPtr cdata, Action<IntPtr> deleter, Retainer retainer) : base(cdata, deleter, retainer)
        {
        }
        protected override object CloneObject()
        {
            var cdata_new = IntPtr.Zero;
            if (retainer_ != null) { retainer_(cdata, out cdata_new); }
            return new ImmediateCallbackScheduler(cdata_new, deleter_, retainer_);
        }
        public new ImmediateCallbackScheduler Clone()
        {
            return (ImmediateCallbackScheduler)(CloneObject());
        }
        /// <summary>
        /// Gets a default immediate callback scheduler.
        /// </summary>
        public static ImmediateCallbackScheduler getDefault()
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = default(IntPtr);
                Detail.easyar_arengineinterop_ImmediateCallbackScheduler_getDefault(out _return_value_);
                return Detail.Object_from_c<ImmediateCallbackScheduler>(_return_value_, Detail.easyar_arengineinterop_ImmediateCallbackScheduler__typeName);
            }
        }
    }

    public enum CameraDeviceType
    {
        /// <summary>
        /// Unknown location
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Rear camera
        /// </summary>
        Back = 1,
        /// <summary>
        /// Front camera
        /// </summary>
        Front = 2,
    }

    /// <summary>
    /// MotionTrackingStatus describes the quality of device motion tracking.
    /// </summary>
    public enum MotionTrackingStatus
    {
        /// <summary>
        /// Result is not available and should not to be used to render virtual objects or do 3D reconstruction. This value occurs temporarily after initializing, tracking lost or relocalizing.
        /// </summary>
        NotTracking = 0,
        /// <summary>
        /// Tracking is available, but the quality of the result is not good enough. This value occurs temporarily due to weak texture or excessive movement. The result can be used to render virtual objects, but should generally not be used to do 3D reconstruction.
        /// </summary>
        Limited = 1,
        /// <summary>
        /// Tracking with a good quality. The result can be used to render virtual objects or do 3D reconstruction.
        /// </summary>
        Tracking = 2,
    }

    /// <summary>
    /// Camera parameters, including image size, focal length, principal point, camera type and camera rotation against natural orientation.
    /// </summary>
    public class CameraParameters : RefBase
    {
        internal CameraParameters(IntPtr cdata, Action<IntPtr> deleter, Retainer retainer) : base(cdata, deleter, retainer)
        {
        }
        protected override object CloneObject()
        {
            var cdata_new = IntPtr.Zero;
            if (retainer_ != null) { retainer_(cdata, out cdata_new); }
            return new CameraParameters(cdata_new, deleter_, retainer_);
        }
        public new CameraParameters Clone()
        {
            return (CameraParameters)(CloneObject());
        }
        public CameraParameters(Vec2I imageSize, Vec2F focalLength, Vec2F principalPoint, CameraDeviceType cameraDeviceType, int cameraOrientation) : base(IntPtr.Zero, Detail.easyar_arengineinterop_CameraParameters__dtor, Detail.easyar_arengineinterop_CameraParameters__retain)
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = IntPtr.Zero;
                Detail.easyar_arengineinterop_CameraParameters__ctor(imageSize, focalLength, principalPoint, cameraDeviceType, cameraOrientation, out _return_value_);
                cdata_ = _return_value_;
            }
        }
        /// <summary>
        /// Image size.
        /// </summary>
        public virtual Vec2I size()
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = Detail.easyar_arengineinterop_CameraParameters_size(cdata);
                return _return_value_;
            }
        }
        /// <summary>
        /// Focal length, the distance from effective optical center to CCD plane, divided by unit pixel density in width and height directions. The unit is pixel.
        /// </summary>
        public virtual Vec2F focalLength()
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = Detail.easyar_arengineinterop_CameraParameters_focalLength(cdata);
                return _return_value_;
            }
        }
        /// <summary>
        /// Principal point, coordinates of the intersection point of principal axis on CCD plane against the left-top corner of the image. The unit is pixel.
        /// </summary>
        public virtual Vec2F principalPoint()
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = Detail.easyar_arengineinterop_CameraParameters_principalPoint(cdata);
                return _return_value_;
            }
        }
        /// <summary>
        /// Camera device type. Default, back or front camera. On desktop devices, there are only default cameras. On mobile devices, there is a differentiation between back and front cameras.
        /// </summary>
        public virtual CameraDeviceType cameraDeviceType()
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = Detail.easyar_arengineinterop_CameraParameters_cameraDeviceType(cdata);
                return _return_value_;
            }
        }
        /// <summary>
        /// Camera rotation against device natural orientation.
        /// For Android phones and some Android tablets, this value is 90 degrees.
        /// For Android eye-wear and some Android tablets, this value is 0 degrees.
        /// For all current iOS devices, this value is 90 degrees.
        /// </summary>
        public virtual int cameraOrientation()
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = Detail.easyar_arengineinterop_CameraParameters_cameraOrientation(cdata);
                return _return_value_;
            }
        }
        /// <summary>
        /// Creates CameraParameters with default camera intrinsics. Default intrinsics are calculated by image size, which is not very precise.
        /// </summary>
        public static CameraParameters createWithDefaultIntrinsics(Vec2I imageSize, CameraDeviceType cameraDeviceType, int cameraOrientation)
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = default(IntPtr);
                Detail.easyar_arengineinterop_CameraParameters_createWithDefaultIntrinsics(imageSize, cameraDeviceType, cameraOrientation, out _return_value_);
                return Detail.Object_from_c<CameraParameters>(_return_value_, Detail.easyar_arengineinterop_CameraParameters__typeName);
            }
        }
        /// <summary>
        /// Get equivalent CameraParameters for a different camera image size.
        /// </summary>
        public virtual CameraParameters getResized(Vec2I imageSize)
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = default(IntPtr);
                Detail.easyar_arengineinterop_CameraParameters_getResized(cdata, imageSize, out _return_value_);
                return Detail.Object_from_c<CameraParameters>(_return_value_, Detail.easyar_arengineinterop_CameraParameters__typeName);
            }
        }
        /// <summary>
        /// Calculates the angle required to rotate the camera image clockwise to align it with the screen.
        /// screenRotation is the angle of rotation of displaying screen image against device natural orientation in clockwise in degrees.
        /// For iOS(UIInterfaceOrientationPortrait as natural orientation):
        /// * UIInterfaceOrientationPortrait: rotation = 0
        /// * UIInterfaceOrientationLandscapeRight: rotation = 90
        /// * UIInterfaceOrientationPortraitUpsideDown: rotation = 180
        /// * UIInterfaceOrientationLandscapeLeft: rotation = 270
        /// For Android:
        /// * Surface.ROTATION_0 = 0
        /// * Surface.ROTATION_90 = 90
        /// * Surface.ROTATION_180 = 180
        /// * Surface.ROTATION_270 = 270
        /// </summary>
        public virtual int imageOrientation(int screenRotation)
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = Detail.easyar_arengineinterop_CameraParameters_imageOrientation(cdata, screenRotation);
                return _return_value_;
            }
        }
        /// <summary>
        /// Calculates whether the image needed to be flipped horizontally. The image is rotated, then flipped in rendering. When cameraDeviceType is front, a flip is automatically applied. Pass manualHorizontalFlip with true to add a manual flip.
        /// </summary>
        public virtual bool imageHorizontalFlip(bool manualHorizontalFlip)
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = Detail.easyar_arengineinterop_CameraParameters_imageHorizontalFlip(cdata, manualHorizontalFlip);
                return _return_value_;
            }
        }
        /// <summary>
        /// Calculates the perspective projection matrix needed by virtual object rendering. The projection transforms points from camera coordinate system to clip coordinate system ([-1, 1]^4)  (including rotation around z-axis). The form of perspective projection matrix is the same as OpenGL, that matrix multiply column vector of homogeneous coordinates of point on the right, ant not like Direct3D, that matrix multiply row vector of homogeneous coordinates of point on the left. But data arrangement is row-major, not like OpenGL&#39;s column-major. Clip coordinate system and normalized device coordinate system are defined as the same as OpenGL&#39;s default.
        /// </summary>
        public virtual Matrix44F projection(float nearPlane, float farPlane, float viewportAspectRatio, int screenRotation, bool combiningFlip, bool manualHorizontalFlip)
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = Detail.easyar_arengineinterop_CameraParameters_projection(cdata, nearPlane, farPlane, viewportAspectRatio, screenRotation, combiningFlip, manualHorizontalFlip);
                return _return_value_;
            }
        }
        /// <summary>
        /// Calculates the orthogonal projection matrix needed by camera background rendering. The projection transforms points from image quad coordinate system ([-1, 1]^2) to clip coordinate system ([-1, 1]^4) (including rotation around z-axis), with the undefined two dimensions unchanged. The form of orthogonal projection matrix is the same as OpenGL, that matrix multiply column vector of homogeneous coordinates of point on the right, ant not like Direct3D, that matrix multiply row vector of homogeneous coordinates of point on the left. But data arrangement is row-major, not like OpenGL&#39;s column-major. Clip coordinate system and normalized device coordinate system are defined as the same as OpenGL&#39;s default.
        /// </summary>
        public virtual Matrix44F imageProjection(float viewportAspectRatio, int screenRotation, bool combiningFlip, bool manualHorizontalFlip)
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = Detail.easyar_arengineinterop_CameraParameters_imageProjection(cdata, viewportAspectRatio, screenRotation, combiningFlip, manualHorizontalFlip);
                return _return_value_;
            }
        }
        /// <summary>
        /// Transforms points from image coordinate system ([0, 1]^2) to screen coordinate system ([0, 1]^2). Both coordinate system is x-left, y-down, with origin at left-top.
        /// </summary>
        public virtual Vec2F screenCoordinatesFromImageCoordinates(float viewportAspectRatio, int screenRotation, bool combiningFlip, bool manualHorizontalFlip, Vec2F imageCoordinates)
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = Detail.easyar_arengineinterop_CameraParameters_screenCoordinatesFromImageCoordinates(cdata, viewportAspectRatio, screenRotation, combiningFlip, manualHorizontalFlip, imageCoordinates);
                return _return_value_;
            }
        }
        /// <summary>
        /// Transforms points from screen coordinate system ([0, 1]^2) to image coordinate system ([0, 1]^2). Both coordinate system is x-left, y-down, with origin at left-top.
        /// </summary>
        public virtual Vec2F imageCoordinatesFromScreenCoordinates(float viewportAspectRatio, int screenRotation, bool combiningFlip, bool manualHorizontalFlip, Vec2F screenCoordinates)
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = Detail.easyar_arengineinterop_CameraParameters_imageCoordinatesFromScreenCoordinates(cdata, viewportAspectRatio, screenRotation, combiningFlip, manualHorizontalFlip, screenCoordinates);
                return _return_value_;
            }
        }
        /// <summary>
        /// Checks if two groups of parameters are equal.
        /// </summary>
        public virtual bool equalsTo(CameraParameters other)
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = Detail.easyar_arengineinterop_CameraParameters_equalsTo(cdata, other.cdata);
                return _return_value_;
            }
        }
    }

    public class Engine
    {
        public static int schemaHash()
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = Detail.easyar_arengineinterop_Engine_schemaHash();
                return _return_value_;
            }
        }
        public static bool initialize(string licenseKey)
        {
            if (Detail.easyar_arengineinterop_Engine_schemaHash() != 25178734)
            {
                throw new InvalidOperationException("SchemaHashNotMatched");
            }
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = Detail.easyar_arengineinterop_Engine_initialize(Detail.String_to_c(ar, licenseKey));
                return _return_value_;
            }
        }
    }

    /// <summary>
    /// Input frame.
    /// It includes image, camera parameters, timestamp, camera transform matrix against world coordinate system, and tracking status,
    /// among which, camera parameters, timestamp, camera transform matrix and tracking status are all optional, but specific algorithms may have special requirements on the input.
    /// </summary>
    public class InputFrame : RefBase
    {
        internal InputFrame(IntPtr cdata, Action<IntPtr> deleter, Retainer retainer) : base(cdata, deleter, retainer)
        {
        }
        protected override object CloneObject()
        {
            var cdata_new = IntPtr.Zero;
            if (retainer_ != null) { retainer_(cdata, out cdata_new); }
            return new InputFrame(cdata_new, deleter_, retainer_);
        }
        public new InputFrame Clone()
        {
            return (InputFrame)(CloneObject());
        }
        /// <summary>
        /// Index, an automatic incremental value, which is different for every input frame.
        /// </summary>
        public virtual int index()
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = Detail.easyar_arengineinterop_InputFrame_index(cdata);
                return _return_value_;
            }
        }
        /// <summary>
        /// Gets image.
        /// </summary>
        public virtual Image image()
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = default(IntPtr);
                Detail.easyar_arengineinterop_InputFrame_image(cdata, out _return_value_);
                return Detail.Object_from_c<Image>(_return_value_, Detail.easyar_arengineinterop_Image__typeName);
            }
        }
        /// <summary>
        /// Checks if there are camera parameters.
        /// </summary>
        public virtual bool hasCameraParameters()
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = Detail.easyar_arengineinterop_InputFrame_hasCameraParameters(cdata);
                return _return_value_;
            }
        }
        /// <summary>
        /// Gets camera parameters.
        /// </summary>
        public virtual CameraParameters cameraParameters()
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = default(IntPtr);
                Detail.easyar_arengineinterop_InputFrame_cameraParameters(cdata, out _return_value_);
                return Detail.Object_from_c<CameraParameters>(_return_value_, Detail.easyar_arengineinterop_CameraParameters__typeName);
            }
        }
        /// <summary>
        /// Checks if there is temporal information (timestamp).
        /// </summary>
        public virtual bool hasTemporalInformation()
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = Detail.easyar_arengineinterop_InputFrame_hasTemporalInformation(cdata);
                return _return_value_;
            }
        }
        /// <summary>
        /// Timestamp. In seconds.
        /// </summary>
        public virtual double timestamp()
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = Detail.easyar_arengineinterop_InputFrame_timestamp(cdata);
                return _return_value_;
            }
        }
        /// <summary>
        /// Checks if there is spatial information (cameraTransform and trackingStatus).
        /// </summary>
        public virtual bool hasSpatialInformation()
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = Detail.easyar_arengineinterop_InputFrame_hasSpatialInformation(cdata);
                return _return_value_;
            }
        }
        /// <summary>
        /// Camera transform matrix against world coordinate system. Camera coordinate system and world coordinate system are all right-handed. For the camera coordinate system, the origin is the optical center, x-right, y-up, and z in the direction of light going into camera. (The right and up, is right and up in the camera image, which can be different from these in the natural orientation of the device.) The data arrangement is row-major, not like OpenGL&#39;s column-major.
        /// </summary>
        public virtual Matrix44F cameraTransform()
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = Detail.easyar_arengineinterop_InputFrame_cameraTransform(cdata);
                return _return_value_;
            }
        }
        /// <summary>
        /// Gets device motion tracking status: `MotionTrackingStatus`_ .
        /// </summary>
        public virtual MotionTrackingStatus trackingStatus()
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = Detail.easyar_arengineinterop_InputFrame_trackingStatus(cdata);
                return _return_value_;
            }
        }
        /// <summary>
        /// Creates an instance.
        /// </summary>
        public static InputFrame create(Image image, CameraParameters cameraParameters, double timestamp, Matrix44F cameraTransform, MotionTrackingStatus trackingStatus)
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = default(IntPtr);
                Detail.easyar_arengineinterop_InputFrame_create(image.cdata, cameraParameters.cdata, timestamp, cameraTransform, trackingStatus, out _return_value_);
                return Detail.Object_from_c<InputFrame>(_return_value_, Detail.easyar_arengineinterop_InputFrame__typeName);
            }
        }
        /// <summary>
        /// Creates an instance with image, camera parameters, and timestamp.
        /// </summary>
        public static InputFrame createWithImageAndCameraParametersAndTemporal(Image image, CameraParameters cameraParameters, double timestamp)
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = default(IntPtr);
                Detail.easyar_arengineinterop_InputFrame_createWithImageAndCameraParametersAndTemporal(image.cdata, cameraParameters.cdata, timestamp, out _return_value_);
                return Detail.Object_from_c<InputFrame>(_return_value_, Detail.easyar_arengineinterop_InputFrame__typeName);
            }
        }
        /// <summary>
        /// Creates an instance with image and camera parameters.
        /// </summary>
        public static InputFrame createWithImageAndCameraParameters(Image image, CameraParameters cameraParameters)
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = default(IntPtr);
                Detail.easyar_arengineinterop_InputFrame_createWithImageAndCameraParameters(image.cdata, cameraParameters.cdata, out _return_value_);
                return Detail.Object_from_c<InputFrame>(_return_value_, Detail.easyar_arengineinterop_InputFrame__typeName);
            }
        }
        /// <summary>
        /// Creates an instance with image.
        /// </summary>
        public static InputFrame createWithImage(Image image)
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = default(IntPtr);
                Detail.easyar_arengineinterop_InputFrame_createWithImage(image.cdata, out _return_value_);
                return Detail.Object_from_c<InputFrame>(_return_value_, Detail.easyar_arengineinterop_InputFrame__typeName);
            }
        }
    }

    /// <summary>
    /// FrameFilterResult is the base class for result classes of all synchronous algorithm components.
    /// </summary>
    public class FrameFilterResult : RefBase
    {
        internal FrameFilterResult(IntPtr cdata, Action<IntPtr> deleter, Retainer retainer) : base(cdata, deleter, retainer)
        {
        }
        protected override object CloneObject()
        {
            var cdata_new = IntPtr.Zero;
            if (retainer_ != null) { retainer_(cdata, out cdata_new); }
            return new FrameFilterResult(cdata_new, deleter_, retainer_);
        }
        public new FrameFilterResult Clone()
        {
            return (FrameFilterResult)(CloneObject());
        }
    }

    /// <summary>
    /// Output frame.
    /// It includes input frame and results of synchronous components.
    /// </summary>
    public class OutputFrame : RefBase
    {
        internal OutputFrame(IntPtr cdata, Action<IntPtr> deleter, Retainer retainer) : base(cdata, deleter, retainer)
        {
        }
        protected override object CloneObject()
        {
            var cdata_new = IntPtr.Zero;
            if (retainer_ != null) { retainer_(cdata, out cdata_new); }
            return new OutputFrame(cdata_new, deleter_, retainer_);
        }
        public new OutputFrame Clone()
        {
            return (OutputFrame)(CloneObject());
        }
        public OutputFrame(InputFrame inputFrame, List<Optional<FrameFilterResult>> results) : base(IntPtr.Zero, Detail.easyar_arengineinterop_OutputFrame__dtor, Detail.easyar_arengineinterop_OutputFrame__retain)
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = IntPtr.Zero;
                Detail.easyar_arengineinterop_OutputFrame__ctor(inputFrame.cdata, Detail.ListOfOptionalOfFrameFilterResult_to_c(ar, results), out _return_value_);
                cdata_ = _return_value_;
            }
        }
        /// <summary>
        /// Index, an automatic incremental value, which is different for every output frame.
        /// </summary>
        public virtual int index()
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = Detail.easyar_arengineinterop_OutputFrame_index(cdata);
                return _return_value_;
            }
        }
        /// <summary>
        /// Corresponding input frame.
        /// </summary>
        public virtual InputFrame inputFrame()
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = default(IntPtr);
                Detail.easyar_arengineinterop_OutputFrame_inputFrame(cdata, out _return_value_);
                return Detail.Object_from_c<InputFrame>(_return_value_, Detail.easyar_arengineinterop_InputFrame__typeName);
            }
        }
        /// <summary>
        /// Results of synchronous components.
        /// </summary>
        public virtual List<Optional<FrameFilterResult>> results()
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = default(IntPtr);
                Detail.easyar_arengineinterop_OutputFrame_results(cdata, out _return_value_);
                return Detail.ListOfOptionalOfFrameFilterResult_from_c(ar, _return_value_);
            }
        }
    }

    /// <summary>
    /// Feedback frame.
    /// It includes an input frame and a historic output frame for use in feedback synchronous components such as `ImageTracker`_ .
    /// </summary>
    public class FeedbackFrame : RefBase
    {
        internal FeedbackFrame(IntPtr cdata, Action<IntPtr> deleter, Retainer retainer) : base(cdata, deleter, retainer)
        {
        }
        protected override object CloneObject()
        {
            var cdata_new = IntPtr.Zero;
            if (retainer_ != null) { retainer_(cdata, out cdata_new); }
            return new FeedbackFrame(cdata_new, deleter_, retainer_);
        }
        public new FeedbackFrame Clone()
        {
            return (FeedbackFrame)(CloneObject());
        }
        public FeedbackFrame(InputFrame inputFrame, Optional<OutputFrame> previousOutputFrame) : base(IntPtr.Zero, Detail.easyar_arengineinterop_FeedbackFrame__dtor, Detail.easyar_arengineinterop_FeedbackFrame__retain)
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = IntPtr.Zero;
                Detail.easyar_arengineinterop_FeedbackFrame__ctor(inputFrame.cdata, previousOutputFrame.map(p => p.OnSome ? new Detail.OptionalOfOutputFrame { has_value = true, value = p.Value.cdata } : new Detail.OptionalOfOutputFrame { has_value = false, value = default(IntPtr) }), out _return_value_);
                cdata_ = _return_value_;
            }
        }
        /// <summary>
        /// Input frame.
        /// </summary>
        public virtual InputFrame inputFrame()
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = default(IntPtr);
                Detail.easyar_arengineinterop_FeedbackFrame_inputFrame(cdata, out _return_value_);
                return Detail.Object_from_c<InputFrame>(_return_value_, Detail.easyar_arengineinterop_InputFrame__typeName);
            }
        }
        /// <summary>
        /// Historic output frame.
        /// </summary>
        public virtual Optional<OutputFrame> previousOutputFrame()
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = default(Detail.OptionalOfOutputFrame);
                Detail.easyar_arengineinterop_FeedbackFrame_previousOutputFrame(cdata, out _return_value_);
                return _return_value_.map(p => p.has_value ? Detail.Object_from_c<OutputFrame>(p.value, Detail.easyar_arengineinterop_OutputFrame__typeName) : Optional<OutputFrame>.Empty);
            }
        }
    }

    /// <summary>
    /// PixelFormat represents the format of image pixel data. All formats follow the pixel direction from left to right and from top to bottom.
    /// </summary>
    public enum PixelFormat
    {
        /// <summary>
        /// Unknown
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// 256 shades grayscale
        /// </summary>
        Gray = 1,
        /// <summary>
        /// YUV_NV21
        /// </summary>
        YUV_NV21 = 2,
        /// <summary>
        /// YUV_NV12
        /// </summary>
        YUV_NV12 = 3,
        /// <summary>
        /// YUV_I420
        /// </summary>
        YUV_I420 = 4,
        /// <summary>
        /// YUV_YV12
        /// </summary>
        YUV_YV12 = 5,
        /// <summary>
        /// RGB888
        /// </summary>
        RGB888 = 6,
        /// <summary>
        /// BGR888
        /// </summary>
        BGR888 = 7,
        /// <summary>
        /// RGBA8888
        /// </summary>
        RGBA8888 = 8,
        /// <summary>
        /// BGRA8888
        /// </summary>
        BGRA8888 = 9,
    }

    /// <summary>
    /// Image stores an image data and represents an image in memory.
    /// Image raw data can be accessed as byte array. The width/height/etc information are also accessible.
    /// You can always access image data since the first version of EasyAR Sense.
    ///
    /// You can do this in iOS
    /// ::
    ///
    ///     #import &lt;easyar/buffer.oc.h&gt;
    ///     #import &lt;easyar/image.oc.h&gt;
    ///
    ///     easyar_OutputFrame * outputFrame = [outputFrameBuffer peek];
    ///     if (outputFrame != nil) {
    ///         easyar_Image * i = [[outputFrame inputFrame] image];
    ///         easyar_Buffer * b = [i buffer];
    ///         char * bytes = calloc([b size], 1);
    ///         memcpy(bytes, [b data], [b size]);
    ///         // use bytes here
    ///         free(bytes);
    ///     }
    ///
    /// Or in Android
    /// ::
    ///
    ///     import cn.easyar.*;
    ///
    ///     OutputFrame outputFrame = outputFrameBuffer.peek();
    ///     if (outputFrame != null) {
    ///         InputFrame inputFrame = outputFrame.inputFrame();
    ///         Image i = inputFrame.image();
    ///         Buffer b = i.buffer();
    ///         byte[] bytes = new byte[b.size()];
    ///         b.copyToByteArray(0, bytes, 0, bytes.length);
    ///         // use bytes here
    ///         b.dispose();
    ///         i.dispose();
    ///         inputFrame.dispose();
    ///         outputFrame.dispose();
    ///     }
    /// </summary>
    public class Image : RefBase
    {
        internal Image(IntPtr cdata, Action<IntPtr> deleter, Retainer retainer) : base(cdata, deleter, retainer)
        {
        }
        protected override object CloneObject()
        {
            var cdata_new = IntPtr.Zero;
            if (retainer_ != null) { retainer_(cdata, out cdata_new); }
            return new Image(cdata_new, deleter_, retainer_);
        }
        public new Image Clone()
        {
            return (Image)(CloneObject());
        }
        public Image(Buffer buffer, PixelFormat format, int width, int height) : base(IntPtr.Zero, Detail.easyar_arengineinterop_Image__dtor, Detail.easyar_arengineinterop_Image__retain)
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = IntPtr.Zero;
                Detail.easyar_arengineinterop_Image__ctor(buffer.cdata, format, width, height, out _return_value_);
                cdata_ = _return_value_;
            }
        }
        /// <summary>
        /// Returns buffer inside image. It can be used to access internal data of image. The content of `Buffer`_ shall not be modified, as they may be accessed from other threads.
        /// </summary>
        public virtual Buffer buffer()
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = default(IntPtr);
                Detail.easyar_arengineinterop_Image_buffer(cdata, out _return_value_);
                return Detail.Object_from_c<Buffer>(_return_value_, Detail.easyar_arengineinterop_Buffer__typeName);
            }
        }
        /// <summary>
        /// Returns image format.
        /// </summary>
        public virtual PixelFormat format()
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = Detail.easyar_arengineinterop_Image_format(cdata);
                return _return_value_;
            }
        }
        /// <summary>
        /// Returns image width.
        /// </summary>
        public virtual int width()
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = Detail.easyar_arengineinterop_Image_width(cdata);
                return _return_value_;
            }
        }
        /// <summary>
        /// Returns image height.
        /// </summary>
        public virtual int height()
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = Detail.easyar_arengineinterop_Image_height(cdata);
                return _return_value_;
            }
        }
        /// <summary>
        /// Returns image pixel width for encoding.
        /// </summary>
        public virtual int pixelWidth()
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = Detail.easyar_arengineinterop_Image_pixelWidth(cdata);
                return _return_value_;
            }
        }
        /// <summary>
        /// Returns image pixel height for encoding.
        /// </summary>
        public virtual int pixelHeight()
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = Detail.easyar_arengineinterop_Image_pixelHeight(cdata);
                return _return_value_;
            }
        }
        public static Image create(Buffer buffer, PixelFormat format, int width, int height, int pixelWidth, int pixelHeight)
        {
            using (var ar = new Detail.AutoRelease())
            {
                var _return_value_ = default(IntPtr);
                Detail.easyar_arengineinterop_Image_create(buffer.cdata, format, width, height, pixelWidth, pixelHeight, out _return_value_);
                return Detail.Object_from_c<Image>(_return_value_, Detail.easyar_arengineinterop_Image__typeName);
            }
        }
    }

    /// <summary>
    /// Square matrix of 4. The data arrangement is row-major.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix44F
    {
        /// <summary>
        /// The raw data of matrix.
        /// </summary>
        public float[] data
        {
            get
            {
                return new float[] { data_0, data_1, data_2, data_3, data_4, data_5, data_6, data_7, data_8, data_9, data_10, data_11, data_12, data_13, data_14, data_15 };
            }
            set
            {
                if (value.Length != 16) { throw new ArgumentException(); }
                this.data_0 = value[0];
                this.data_1 = value[1];
                this.data_2 = value[2];
                this.data_3 = value[3];
                this.data_4 = value[4];
                this.data_5 = value[5];
                this.data_6 = value[6];
                this.data_7 = value[7];
                this.data_8 = value[8];
                this.data_9 = value[9];
                this.data_10 = value[10];
                this.data_11 = value[11];
                this.data_12 = value[12];
                this.data_13 = value[13];
                this.data_14 = value[14];
                this.data_15 = value[15];
            }
        }
        public float data_0;
        public float data_1;
        public float data_2;
        public float data_3;
        public float data_4;
        public float data_5;
        public float data_6;
        public float data_7;
        public float data_8;
        public float data_9;
        public float data_10;
        public float data_11;
        public float data_12;
        public float data_13;
        public float data_14;
        public float data_15;

        public Matrix44F(float data_0, float data_1, float data_2, float data_3, float data_4, float data_5, float data_6, float data_7, float data_8, float data_9, float data_10, float data_11, float data_12, float data_13, float data_14, float data_15)
        {
            this.data_0 = data_0;
            this.data_1 = data_1;
            this.data_2 = data_2;
            this.data_3 = data_3;
            this.data_4 = data_4;
            this.data_5 = data_5;
            this.data_6 = data_6;
            this.data_7 = data_7;
            this.data_8 = data_8;
            this.data_9 = data_9;
            this.data_10 = data_10;
            this.data_11 = data_11;
            this.data_12 = data_12;
            this.data_13 = data_13;
            this.data_14 = data_14;
            this.data_15 = data_15;
        }
    }

    /// <summary>
    /// Square matrix of 3. The data arrangement is row-major.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix33F
    {
        /// <summary>
        /// The raw data of matrix.
        /// </summary>
        public float[] data
        {
            get
            {
                return new float[] { data_0, data_1, data_2, data_3, data_4, data_5, data_6, data_7, data_8 };
            }
            set
            {
                if (value.Length != 9) { throw new ArgumentException(); }
                this.data_0 = value[0];
                this.data_1 = value[1];
                this.data_2 = value[2];
                this.data_3 = value[3];
                this.data_4 = value[4];
                this.data_5 = value[5];
                this.data_6 = value[6];
                this.data_7 = value[7];
                this.data_8 = value[8];
            }
        }
        public float data_0;
        public float data_1;
        public float data_2;
        public float data_3;
        public float data_4;
        public float data_5;
        public float data_6;
        public float data_7;
        public float data_8;

        public Matrix33F(float data_0, float data_1, float data_2, float data_3, float data_4, float data_5, float data_6, float data_7, float data_8)
        {
            this.data_0 = data_0;
            this.data_1 = data_1;
            this.data_2 = data_2;
            this.data_3 = data_3;
            this.data_4 = data_4;
            this.data_5 = data_5;
            this.data_6 = data_6;
            this.data_7 = data_7;
            this.data_8 = data_8;
        }
    }

    /// <summary>
    /// 3 dimensional vector of double.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Vec3D
    {
        /// <summary>
        /// The raw data of vector.
        /// </summary>
        public double[] data
        {
            get
            {
                return new double[] { data_0, data_1, data_2 };
            }
            set
            {
                if (value.Length != 3) { throw new ArgumentException(); }
                this.data_0 = value[0];
                this.data_1 = value[1];
                this.data_2 = value[2];
            }
        }
        public double data_0;
        public double data_1;
        public double data_2;

        public Vec3D(double data_0, double data_1, double data_2)
        {
            this.data_0 = data_0;
            this.data_1 = data_1;
            this.data_2 = data_2;
        }
    }

    /// <summary>
    /// 4 dimensional vector of float.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Vec4F
    {
        /// <summary>
        /// The raw data of vector.
        /// </summary>
        public float[] data
        {
            get
            {
                return new float[] { data_0, data_1, data_2, data_3 };
            }
            set
            {
                if (value.Length != 4) { throw new ArgumentException(); }
                this.data_0 = value[0];
                this.data_1 = value[1];
                this.data_2 = value[2];
                this.data_3 = value[3];
            }
        }
        public float data_0;
        public float data_1;
        public float data_2;
        public float data_3;

        public Vec4F(float data_0, float data_1, float data_2, float data_3)
        {
            this.data_0 = data_0;
            this.data_1 = data_1;
            this.data_2 = data_2;
            this.data_3 = data_3;
        }
    }

    /// <summary>
    /// 3 dimensional vector of float.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Vec3F
    {
        /// <summary>
        /// The raw data of vector.
        /// </summary>
        public float[] data
        {
            get
            {
                return new float[] { data_0, data_1, data_2 };
            }
            set
            {
                if (value.Length != 3) { throw new ArgumentException(); }
                this.data_0 = value[0];
                this.data_1 = value[1];
                this.data_2 = value[2];
            }
        }
        public float data_0;
        public float data_1;
        public float data_2;

        public Vec3F(float data_0, float data_1, float data_2)
        {
            this.data_0 = data_0;
            this.data_1 = data_1;
            this.data_2 = data_2;
        }
    }

    /// <summary>
    /// 2 dimensional vector of float.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Vec2F
    {
        /// <summary>
        /// The raw data of vector.
        /// </summary>
        public float[] data
        {
            get
            {
                return new float[] { data_0, data_1 };
            }
            set
            {
                if (value.Length != 2) { throw new ArgumentException(); }
                this.data_0 = value[0];
                this.data_1 = value[1];
            }
        }
        public float data_0;
        public float data_1;

        public Vec2F(float data_0, float data_1)
        {
            this.data_0 = data_0;
            this.data_1 = data_1;
        }
    }

    /// <summary>
    /// 4 dimensional vector of int.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Vec4I
    {
        /// <summary>
        /// The raw data of vector.
        /// </summary>
        public int[] data
        {
            get
            {
                return new int[] { data_0, data_1, data_2, data_3 };
            }
            set
            {
                if (value.Length != 4) { throw new ArgumentException(); }
                this.data_0 = value[0];
                this.data_1 = value[1];
                this.data_2 = value[2];
                this.data_3 = value[3];
            }
        }
        public int data_0;
        public int data_1;
        public int data_2;
        public int data_3;

        public Vec4I(int data_0, int data_1, int data_2, int data_3)
        {
            this.data_0 = data_0;
            this.data_1 = data_1;
            this.data_2 = data_2;
            this.data_3 = data_3;
        }
    }

    /// <summary>
    /// 2 dimensional vector of int.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Vec2I
    {
        /// <summary>
        /// The raw data of vector.
        /// </summary>
        public int[] data
        {
            get
            {
                return new int[] { data_0, data_1 };
            }
            set
            {
                if (value.Length != 2) { throw new ArgumentException(); }
                this.data_0 = value[0];
                this.data_1 = value[1];
            }
        }
        public int data_0;
        public int data_1;

        public Vec2I(int data_0, int data_1)
        {
            this.data_0 = data_0;
            this.data_1 = data_1;
        }
    }

}
#endif
