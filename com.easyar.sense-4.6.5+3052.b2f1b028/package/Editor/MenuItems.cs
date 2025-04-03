//================================================================================================================================
//
//  Copyright (c) 2015-2023 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
//  EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
//  and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
//
//================================================================================================================================

using UnityEditor;

namespace easyar
{
    class MenuItems
    {
        const int pMega = 20;
        const int pSpatialMap = 21;
        const int pMotionTracking = 22;
        const int pSurfaceTracking = 23;
        const int pImageTracking = 24;
        const int pObjectTracking = 25;
        const int pFrame = 26;
        const int pVideo = 27;
        const int pSession = 30;
        const string mRoot = "GameObject/EasyAR Sense/";
        const string mSession = "AR Session (Preset)/";
        const string mMega = "Mega/";
        const string mSpatialMap = "SpatialMap/";
        const string mMotionTracking = "Motion Tracking/";
        const string mSurfaceTracking = "Surface Tracking/";
        const string mImageTracking = "Image Tracking/";
        const string mObjectTracking = "Object Tracking/";
        const string mEif = "Frame Recording and Playback/";
        const string mVideo = "Video/";
#if EASYAR_ENABLE_MEGA
        const int pMegaSense = 30;
        const string menuPathMega = "GameObject/EasyAR Mega/Sense/";
#endif

        #region ARSession

        [MenuItem(mRoot + mSession + "AR Session (Empty)", priority = pSession)]
        static void ARSession() => ARSessionFactory.CreateSession(ARSessionFactory.ARSessionPreset.Empty);

        [MenuItem(mRoot + mSession + "AR Session (Mega Preset)", priority = pSession)]
        [MenuItem(mRoot + mMega + "AR Session (Mega Preset)", priority = pMega)]
#if EASYAR_ENABLE_MEGA
        [MenuItem(menuPathMega + "AR Session (Mega Preset)", priority = pMegaSense)]
#endif
        static void ARSessionPresetMega() => ARSessionFactory.CreateSession(ARSessionFactory.ARSessionPreset.Mega);

        [MenuItem(mRoot + mSession + "AR Session (Sparse SpatialMap Preset)", priority = pSession)]
        [MenuItem(mRoot + mSpatialMap + "AR Session (Sparse SpatialMap Preset)", priority = pSpatialMap)]
        static void ARSessionPresetSparseSpatialMap() => ARSessionFactory.CreateSession(ARSessionFactory.ARSessionPreset.SparseSpatialMap);

        [MenuItem(mRoot + mSession + "AR Session (Dense SpatialMap Preset)", priority = pSession)]
        [MenuItem(mRoot + mSpatialMap + "AR Session (Dense SpatialMap Preset)", priority = pSpatialMap)]
        static void ARSessionPresetDenseSpatialMap() => ARSessionFactory.CreateSession(ARSessionFactory.ARSessionPreset.DenseSpatialMap);

        [MenuItem(mRoot + mSession + "AR Session (Sparse and Dense SpatialMap Preset)", priority = pSession)]
        [MenuItem(mRoot + mSpatialMap + "AR Session (Sparse and Dense SpatialMap Preset)", priority = pSpatialMap)]
        static void ARSessionPresetSparseAndDenseSpatialMap() => ARSessionFactory.CreateSession(ARSessionFactory.ARSessionPreset.SparseAndDenseSpatialMap);

        [MenuItem(mRoot + mSession + "AR Session (Motion Tracking Preset) : System First", priority = pSession)]
        [MenuItem(mRoot + mMotionTracking + "AR Session (Motion Tracking Preset) : System First", priority = pMotionTracking)]
        static void ARSessionPresetMotionTrackingPreferSystem() => ARSessionFactory.CreateSession(ARSessionFactory.ARSessionPreset.MotionTrackingPreferSystem);

        [MenuItem(mRoot + mSession + "AR Session (Motion Tracking Preset) : Motion Tracker First", priority = pSession)]
        [MenuItem(mRoot + mMotionTracking + "AR Session (Motion Tracking Preset) : Motion Tracker First", priority = pMotionTracking)]
        static void ARSessionPresetMotionTrackingPreferMotionTracker() => ARSessionFactory.CreateSession(ARSessionFactory.ARSessionPreset.MotionTrackingPreferMotionTracker);

        [MenuItem(mRoot + mSession + "AR Session (Surface Tracking Preset)", priority = pSession)]
        [MenuItem(mRoot + mSurfaceTracking + "AR Session (Surface Tracking Preset)", priority = pSurfaceTracking)]
        static void ARSessionPresetSurfaceTracking() => ARSessionFactory.CreateSession(ARSessionFactory.ARSessionPreset.SurfaceTracking);

        [MenuItem(mRoot + mSession + "AR Session (Image Tracking Preset)", priority = pSession)]
        [MenuItem(mRoot + mImageTracking + "AR Session (Image Tracking Preset)", priority = pImageTracking)]
        static void ARSessionPresetImageTracking() => ARSessionFactory.CreateSession(ARSessionFactory.ARSessionPreset.ImageTracking);

        [MenuItem(mRoot + mSession + "AR Session (Image Tracking with Motion Fusion Preset)", priority = pSession)]
        [MenuItem(mRoot + mImageTracking + "AR Session (Image Tracking with Motion Fusion Preset)", priority = pImageTracking)]
        static void ARSessionPresetImageTrackingMotionFusion() => ARSessionFactory.CreateSession(ARSessionFactory.ARSessionPreset.ImageTrackingMotionFusion);

        [MenuItem(mRoot + mSession + "AR Session (CRS Preset)", priority = pSession)]
        [MenuItem(mRoot + mImageTracking + "AR Session (CRS Preset)", priority = pImageTracking)]
        static void ARSessionPresetCloudRecognizer() => ARSessionFactory.CreateSession(ARSessionFactory.ARSessionPreset.CloudRecognition);

        [MenuItem(mRoot + mSession + "AR Session (Object Tracking Preset)", priority = pSession)]
        [MenuItem(mRoot + mObjectTracking + "AR Session (Object Tracking Preset)", priority = pObjectTracking)]
        static void ARSessionPresetObjectTracking() => ARSessionFactory.CreateSession(ARSessionFactory.ARSessionPreset.ObjectTracking);

        [MenuItem(mRoot + mSession + "AR Session (Object Tracking with Motion Fusion Preset)", priority = pSession)]
        [MenuItem(mRoot + mObjectTracking + "AR Session (Object Tracking with Motion Fusion Preset)", priority = pObjectTracking)]
        static void ARSessionPresetObjectTrackingMotionFusion() => ARSessionFactory.CreateSession(ARSessionFactory.ARSessionPreset.ObjectTrackingMotionFusion);

        [MenuItem(mRoot + mSession + "AR Session (Frame Player Only Preset)", priority = pSession)]
        [MenuItem(mRoot + mEif + "AR Session (Frame Player Only Preset)", priority = pFrame)]
        static void ARSessionPresetFramePlayer() => ARSessionFactory.CreateSession(ARSessionFactory.ARSessionPreset.FramePlayer);


        [MenuItem(mRoot + mSession + "AR Session (FrameSource Preset) : Camera Device Only", priority = pSession)]
        static void ARSessionPresetFrameSourceCameraDeviceOnly() => ARSessionFactory.CreateSession(ARSessionFactory.ARSessionPreset.CameraDevice);

        [MenuItem(mRoot + mSession + "AR Session (FrameSource Preset) : All", priority = pSession)]
        static void ARSessionPresetFrameSourceAll() => ARSessionFactory.CreateSession(ARSessionFactory.ARSessionPreset.FrameSourceAll);

        #endregion

        #region FrameSource
        [MenuItem(mRoot + mSurfaceTracking + "Frame Source : Camera Device (Surface Tracking)", priority = pSurfaceTracking)]
        static void CameraDevicePreferSurfaceTracking() => ARSessionFactory.SetupCameraDevice(ARSessionFactory.AddFrameSource<CameraDeviceFrameSource>(Selection.activeGameObject), CameraDevicePreference.PreferSurfaceTracking);

        [MenuItem(mRoot + mImageTracking + "Frame Source : Camera Device (Object Sensing)", priority = pImageTracking)]
        [MenuItem(mRoot + mObjectTracking + "Frame Source : Camera Device (Object Sensing)", priority = pObjectTracking)]
        static void CameraDevicePreferObjectSensing() => ARSessionFactory.SetupCameraDevice(ARSessionFactory.AddFrameSource<CameraDeviceFrameSource>(Selection.activeGameObject), CameraDevicePreference.PreferObjectSensing);

        [MenuItem(mRoot + mMotionTracking + "Frame Source : Motion Tracker (Precise Anchor)", priority = pMotionTracking)]
        static void MotionTrackerPreciseAnchor() => ARSessionFactory.SetupMotionTracker(ARSessionFactory.AddFrameSource<MotionTrackerFrameSource>(Selection.activeGameObject), ARSessionFactory.MotionTrackerPreset.PreciseAnchor);

        [MenuItem(mRoot + mMotionTracking + "Frame Source : Motion Tracker (Mega)", priority = pMotionTracking)]
        static void MotionTrackerMega() => ARSessionFactory.SetupMotionTracker(ARSessionFactory.AddFrameSource<MotionTrackerFrameSource>(Selection.activeGameObject), ARSessionFactory.MotionTrackerPreset.Mega);

        [MenuItem(mRoot + mMotionTracking + "Frame Source : Motion Tracker (SpatialMap)", priority = pMotionTracking)]
        static void MotionTrackerPreciseAnchorSpatialMap() => ARSessionFactory.SetupMotionTracker(ARSessionFactory.AddFrameSource<MotionTrackerFrameSource>(Selection.activeGameObject), ARSessionFactory.MotionTrackerPreset.SpatialMap);

        [MenuItem(mRoot + mMotionTracking + "Frame Source : Motion Tracker (Object Sensing)", priority = pMotionTracking)]
        static void MotionTrackerObjectSensing() => ARSessionFactory.SetupMotionTracker(ARSessionFactory.AddFrameSource<MotionTrackerFrameSource>(Selection.activeGameObject), ARSessionFactory.MotionTrackerPreset.ObjectSensing);

        [MenuItem(mRoot + mMotionTracking + "Frame Source : Motion Tracker (Minimum Resource Usage)", priority = pMotionTracking)]
        static void MotionTrackerMinimumResourceUsage() => ARSessionFactory.SetupMotionTracker(ARSessionFactory.AddFrameSource<MotionTrackerFrameSource>(Selection.activeGameObject), ARSessionFactory.MotionTrackerPreset.MinimumResourceUsage);


        [MenuItem(mRoot + mMotionTracking + "Frame Source : ARCore", priority = pMotionTracking)]
        static void ARCore() => ARSessionFactory.AddFrameSource<ARCoreFrameSource>(Selection.activeGameObject);

        [MenuItem(mRoot + mMotionTracking + "Frame Source : ARKit", priority = pMotionTracking)]
        static void ARKit() => ARSessionFactory.AddFrameSource<ARKitFrameSource>(Selection.activeGameObject);

        [MenuItem(mRoot + mMotionTracking + "Frame Source : AREngine", priority = pMotionTracking)]
        static void AREngine() => ARSessionFactory.AddFrameSource<AREngineFrameSource>(Selection.activeGameObject);

        [MenuItem(mRoot + mMotionTracking + "Origin : World Root", priority = pMotionTracking)]
        static void WorldRoot() => ARSessionFactory.CreateController<WorldRootController>();
        #endregion

        #region FrameFilter

        [MenuItem(mRoot + mSurfaceTracking + "Frame Filter : Surface Tracker", priority = pSurfaceTracking)]
        static void SurfaceTracker() => ARSessionFactory.AddFrameFilter<SurfaceTrackerFrameFilter>(Selection.activeGameObject);

        [MenuItem(mRoot + mSurfaceTracking + "Target : Surface Target", priority = pSurfaceTracking)]
        static void SurfaceTarget() => ARSessionFactory.CreateController<SurfaceTargetController>();

        [MenuItem(mRoot + mImageTracking + "Frame Filter : Image Tracker", priority = pImageTracking)]
        static void ImageTracker() => ARSessionFactory.AddFrameFilter<ImageTrackerFrameFilter>(Selection.activeGameObject);

        [MenuItem(mRoot + mImageTracking + "Target : Image Target", priority = pImageTracking)]
        static void ImageTarget() => ARSessionFactory.CreateController<ImageTargetController>();

        [MenuItem(mRoot + mImageTracking + "Frame Filter : Cloud Recognizer", priority = pImageTracking)]
        static void CloudRecognizer() => ARSessionFactory.AddFrameFilter<CloudRecognizerFrameFilter>(Selection.activeGameObject);

        [MenuItem(mRoot + mObjectTracking + "Frame Filter : Object Tracker", priority = pObjectTracking)]
        static void ObjectTracker() => ARSessionFactory.AddFrameFilter<ObjectTrackerFrameFilter>(Selection.activeGameObject);

        [MenuItem(mRoot + mObjectTracking + "Target : Object Target", priority = pObjectTracking)]
        static void ObjectTarget() => ARSessionFactory.CreateController<ObjectTargetController>();


        [MenuItem(mRoot + mSpatialMap + "Frame Filter : Sparse SpatialMap Worker", priority = pSpatialMap)]
        static void SparseSpatialMapWorker() => ARSessionFactory.AddFrameFilter<SparseSpatialMapWorkerFrameFilter>(Selection.activeGameObject);

        [MenuItem(mRoot + mSpatialMap + "Frame Filter : Dense SpatialMap Builder", priority = pSpatialMap)]
        static void DenseSpatialMapBuilder() => ARSessionFactory.AddFrameFilter<DenseSpatialMapBuilderFrameFilter>(Selection.activeGameObject);

        [MenuItem(mRoot + mSpatialMap + "Map : Sparse SpatialMap", priority = pSpatialMap)]
        static void SparseSpatialMap() => ARSessionFactory.CreateController<SparseSpatialMapController>();

        [MenuItem(mRoot + mSpatialMap + "Map Root : Sparse SpatialMap Root", priority = pSpatialMap)]
        static void SparseSpatialMapRoot() => ARSessionFactory.CreateController<SparseSpatialMapRootController>();


        [MenuItem(mRoot + mMega + "Frame Filter : Mega Tracker", priority = pMega)]
#if EASYAR_ENABLE_MEGA
        [MenuItem(menuPathMega + "Frame Filter : Mega Tracker", priority = pMegaSense)]
#endif
        static void CloudLocalizer() => ARSessionFactory.AddFrameFilter<MegaTrackerFrameFilter>(Selection.activeGameObject);
        #endregion

        #region Extra
        [MenuItem(mRoot + mEif + "Frame Source : Frame Player", priority = pFrame)]
        static void FramePlayer() => ARSessionFactory.AddFramePlayer(Selection.activeGameObject);

        [MenuItem(mRoot + mEif + "Frame Recorder", priority = pFrame)]
        static void FrameRecorder() => ARSessionFactory.AddFrameRecorder(Selection.activeGameObject);


        [MenuItem(mRoot + mVideo + "Video Recorder", priority = pVideo)]
        static void VideoRecorder() => ARSessionFactory.CreateVideoRecorder();
        #endregion

        [MenuItem(mRoot + mSession + "AR Session (Empty)", true)]
        [MenuItem(mRoot + mSession + "AR Session (Mega Preset)", true)]
        [MenuItem(mRoot + mMega + "AR Session (Mega Preset)", true)]
        [MenuItem(mRoot + mSession + "AR Session (Sparse SpatialMap Preset)", true)]
        [MenuItem(mRoot + mSpatialMap + "AR Session (Sparse SpatialMap Preset)", true)]
        [MenuItem(mRoot + mSession + "AR Session (Dense SpatialMap Preset)", true)]
        [MenuItem(mRoot + mSpatialMap + "AR Session (Dense SpatialMap Preset)", true)]
        [MenuItem(mRoot + mSession + "AR Session (Sparse and Dense SpatialMap Preset)", true)]
        [MenuItem(mRoot + mSpatialMap + "AR Session (Sparse and Dense SpatialMap Preset)", true)]
        [MenuItem(mRoot + mSession + "AR Session (Motion Tracking Preset) : Motion Tracker First", true)]
        [MenuItem(mRoot + mMotionTracking + "AR Session (Motion Tracking Preset) : Motion Tracker First", true)]
        [MenuItem(mRoot + mSession + "AR Session (Motion Tracking Preset) : System First", true)]
        [MenuItem(mRoot + mMotionTracking + "AR Session (Motion Tracking Preset) : System First", true)]
        [MenuItem(mRoot + mSession + "AR Session (Surface Tracking Preset)", true)]
        [MenuItem(mRoot + mSurfaceTracking + "AR Session (Surface Tracking Preset)", true)]
        [MenuItem(mRoot + mSession + "AR Session (Image Tracking Preset)", true)]
        [MenuItem(mRoot + mImageTracking + "AR Session (Image Tracking Preset)", true)]
        [MenuItem(mRoot + mSession + "AR Session (Image Tracking with Motion Fusion Preset)", true)]
        [MenuItem(mRoot + mImageTracking + "AR Session (Image Tracking with Motion Fusion Preset)", true)]
        [MenuItem(mRoot + mSession + "AR Session (CRS Preset)", true)]
        [MenuItem(mRoot + mImageTracking + "AR Session (CRS Preset)", true)]
        [MenuItem(mRoot + mSession + "AR Session (Object Tracking Preset)", true)]
        [MenuItem(mRoot + mObjectTracking + "AR Session (Object Tracking Preset)", true)]
        [MenuItem(mRoot + mSession + "AR Session (Object Tracking with Motion Fusion Preset)", true)]
        [MenuItem(mRoot + mObjectTracking + "AR Session (Object Tracking with Motion Fusion Preset)", true)]
        [MenuItem(mRoot + mSession + "AR Session (Frame Player Only Preset)", true)]
        [MenuItem(mRoot + mEif + "AR Session (Frame Player Only Preset)", true)]
        [MenuItem(mRoot + mSession + "AR Session (FrameSource Preset) : Camera Device Only", true)]
        [MenuItem(mRoot + mSession + "AR Session (FrameSource Preset) : All", true)]
        [MenuItem(mRoot + mMotionTracking + "Origin : World Root", true)]
        [MenuItem(mRoot + mSurfaceTracking + "Target : Surface Target", true)]
        [MenuItem(mRoot + mImageTracking + "Target : Image Target", true)]
        [MenuItem(mRoot + mObjectTracking + "Target : Object Target", true)]
        [MenuItem(mRoot + mSpatialMap + "Map : Sparse SpatialMap", true)]
        [MenuItem(mRoot + mSpatialMap + "Map Root : Sparse SpatialMap Root", true)]
        [MenuItem(mRoot + mVideo + "Video Recorder", true)]
#if EASYAR_ENABLE_MEGA
        [MenuItem(menuPathMega + "AR Session (Mega Preset)", true)]
#endif
        static bool MenuValidateRootObject() => !Selection.activeGameObject;

        [MenuItem(mRoot + mSurfaceTracking + "Frame Source : Camera Device (Surface Tracking)", true)]
        [MenuItem(mRoot + mImageTracking + "Frame Source : Camera Device (Object Sensing)", true)]
        [MenuItem(mRoot + mObjectTracking + "Frame Source : Camera Device (Object Sensing)", true)]
        [MenuItem(mRoot + mMotionTracking + "Frame Source : Motion Tracker (Precise Anchor)", true)]
        [MenuItem(mRoot + mMotionTracking + "Frame Source : Motion Tracker (Mega)", true)]
        [MenuItem(mRoot + mMotionTracking + "Frame Source : Motion Tracker (SpatialMap)", true)]
        [MenuItem(mRoot + mMotionTracking + "Frame Source : Motion Tracker (Object Sensing)", true)]
        [MenuItem(mRoot + mMotionTracking + "Frame Source : Motion Tracker (Minimum Resource Usage)", true)]
        [MenuItem(mRoot + mMotionTracking + "Frame Source : ARCore", true)]
        [MenuItem(mRoot + mMotionTracking + "Frame Source : ARKit", true)]
        [MenuItem(mRoot + mMotionTracking + "Frame Source : AREngine", true)]
        [MenuItem(mRoot + mSurfaceTracking + "Frame Filter : Surface Tracker", true)]
        [MenuItem(mRoot + mImageTracking + "Frame Filter : Image Tracker", true)]
        [MenuItem(mRoot + mImageTracking + "Frame Filter : Cloud Recognizer", true)]
        [MenuItem(mRoot + mObjectTracking + "Frame Filter : Object Tracker", true)]
        [MenuItem(mRoot + mSpatialMap + "Frame Filter : Sparse SpatialMap Worker", true)]
        [MenuItem(mRoot + mSpatialMap + "Frame Filter : Dense SpatialMap Builder", true)]
        [MenuItem(mRoot + mMega + "Frame Filter : Mega Tracker", true)]
        [MenuItem(mRoot + mEif + "Frame Source : Frame Player", true)]
        [MenuItem(mRoot + mEif + "Frame Recorder", true)]
#if EASYAR_ENABLE_MEGA
        [MenuItem(menuPathMega + "Frame Filter : Mega Tracker", true)]
#endif
        static bool MenuValidateSessionPart() => ARSessionFactory.IsSessionPartAndEmpty(Selection.activeGameObject);
    }
}
