//================================================================================================================================
//
//  Copyright (c) 2015-2023 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
//  EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
//  and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
//
//================================================================================================================================

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace easyar
{
    /// <summary>
    /// <para xml:lang="en" access="internal">WARNING: Designed for internal tools only. Do not use this interface in your application. Accessibility Level may change in future.</para>
    /// <para xml:lang="zh" access="internal">警告：仅用于内部工具。不要在应用开发中使用这个接口。可访问级别可能会在未来产生变化。</para>
    /// </summary>
    public class ARSessionFactory
    {
        public enum ARSessionPreset
        {
            Empty,
            CameraDevice,
            ImageTracking,
            ObjectTracking,
            CloudRecognition,
            FrameSourceAll,
            MotionTrackingPreferMotionTracker,
            MotionTrackingPreferSystem,
            ImageTrackingMotionFusion,
            ObjectTrackingMotionFusion,
            SurfaceTracking,
            SparseSpatialMap,
            DenseSpatialMap,
            SparseAndDenseSpatialMap,
            Mega,
            FramePlayer,
        }

        public enum MotionTrackerPreset
        {
            PreciseAnchor,
            SpatialMap,
            Mega,
            ObjectSensing,
            MinimumResourceUsage,
        }

        public static GameObject CreateSession(ARSessionPreset preset, Func<List<GameObject>> createFrameSources = default)
        {
            var name = "AR Session (EasyAR)";
            if (preset == ARSessionPreset.Empty)
            {
                return CreateObject<ARSession>(name);
            }
            return CreateSession(name, () =>
            {
                var sources = createFrameSources == null ? CreateFrameSources(preset) : createFrameSources();
                SetupFrameSources(sources, preset);
                return sources;
            }, () => CreateFrameFilters(preset));
        }

        public static GameObject CreateSession(string name, Func<List<GameObject>> createFrameSources, Func<List<GameObject>> createFrameFilters)
        {
            var session = CreateObject<ARSession>(name);
            void parentSession(GameObject go) => go.transform.SetParent(session.transform, false);

            var sources = createFrameSources();
            if (sources != null)
            {
                if (sources.Count == 1)
                {
                    parentSession(sources[0]);
                }
                else
                {
                    var group = new GameObject("Frame Source Group");
                    foreach (var source in sources)
                    {
                        source.transform.SetParent(group.transform, false);
                    }
                    parentSession(group);
                }
            }

            var filters = createFrameFilters();
            if (filters != null)
            {
                foreach (var filter in filters)
                {
                    parentSession(filter);
                }
            }

            var player = CreateObject<FramePlayer>();
            parentSession(player);
            var recorder = CreateObject<FrameRecorder>();
            parentSession(recorder);
#if UNITY_EDITOR
            Undo.RegisterCreatedObjectUndo(session, $"Create {name}");
#endif
            return session;
        }

        public static List<GameObject> CreateFrameFilters(ARSessionPreset preset)
        {
            var filters = new List<GameObject>();
            switch (preset)
            {
                case ARSessionPreset.SparseSpatialMap:
                    filters.Add(CreateObject<SparseSpatialMapWorkerFrameFilter>());
                    break;
                case ARSessionPreset.DenseSpatialMap:
                    filters.Add(CreateDenseSpatialMapBuilder());
                    break;
                case ARSessionPreset.SparseAndDenseSpatialMap:
                    filters.Add(CreateObject<SparseSpatialMapWorkerFrameFilter>());
                    filters.Add(CreateDenseSpatialMapBuilder());
                    break;
                case ARSessionPreset.ImageTracking:
                    filters.Add(CreateObject<ImageTrackerFrameFilter>());
                    break;
                case ARSessionPreset.CloudRecognition:
                    filters.Add(CreateObject<ImageTrackerFrameFilter>());
                    filters.Add(CreateObject<CloudRecognizerFrameFilter>());
                    break;
                case ARSessionPreset.ObjectTracking:
                    filters.Add(CreateObject<ObjectTrackerFrameFilter>());
                    break;
                case ARSessionPreset.SurfaceTracking:
                    filters.Add(CreateObject<SurfaceTrackerFrameFilter>());
                    break;
                case ARSessionPreset.Mega:
                    filters.Add(CreateObject<MegaTrackerFrameFilter>());
                    break;
                case ARSessionPreset.ImageTrackingMotionFusion:
                    var filterImageTracker = CreateObject<ImageTrackerFrameFilter>();
                    filterImageTracker.GetComponent<ImageTrackerFrameFilter>().ResultType = new ImageTrackerFrameFilter.ResultParameters { EnablePersistentTargetInstance = true, EnableMotionFusion = true };
                    filters.Add(filterImageTracker);
                    break;
                case ARSessionPreset.ObjectTrackingMotionFusion:
                    var filterObjectTracker = CreateObject<ObjectTrackerFrameFilter>();
                    filterObjectTracker.GetComponent<ObjectTrackerFrameFilter>().ResultType = new ObjectTrackerFrameFilter.ResultParameters { EnablePersistentTargetInstance = true, EnableMotionFusion = true };
                    filters.Add(filterObjectTracker);
                    break;
                default:
                    break;
            }
            return filters;
        }

        public static List<GameObject> CreateFrameSources(ARSessionPreset preset)
        {
            var sources = new List<GameObject>();
            switch (preset)
            {
                case ARSessionPreset.CameraDevice:
                case ARSessionPreset.ImageTracking:
                case ARSessionPreset.CloudRecognition:
                case ARSessionPreset.ObjectTracking:
                case ARSessionPreset.SurfaceTracking:
                    sources.Add(CreateObject<CameraDeviceFrameSource>());
                    break;
                case ARSessionPreset.MotionTrackingPreferMotionTracker:
                    sources.Add(CreateObject<MotionTrackerFrameSource>());
                    sources.Add(CreateObject<AREngineFrameSource>());
                    sources.Add(CreateObject<ARCoreFrameSource>());
                    sources.Add(CreateObject<ARKitFrameSource>());
                    break;
                case ARSessionPreset.MotionTrackingPreferSystem:
                case ARSessionPreset.SparseSpatialMap:
                case ARSessionPreset.DenseSpatialMap:
                case ARSessionPreset.SparseAndDenseSpatialMap:
                case ARSessionPreset.Mega:
                    sources.Add(CreateObject<AREngineFrameSource>());
                    sources.Add(CreateObject<ARCoreFrameSource>());
                    sources.Add(CreateObject<ARKitFrameSource>());
                    sources.Add(CreateObject<MotionTrackerFrameSource>());
                    break;
                case ARSessionPreset.FrameSourceAll:
                case ARSessionPreset.ImageTrackingMotionFusion:
                case ARSessionPreset.ObjectTrackingMotionFusion:
                    sources.Add(CreateObject<AREngineFrameSource>());
                    sources.Add(CreateObject<ARCoreFrameSource>());
                    sources.Add(CreateObject<ARKitFrameSource>());
                    sources.Add(CreateObject<MotionTrackerFrameSource>());
                    sources.Add(CreateObject<CameraDeviceFrameSource>());
                    break;
                default:
                    break;
            }
            SetupFrameSources(sources, preset);
            return sources;
        }

        public static GameObject AddFrameFilter<Filter>(GameObject sessionPart) where Filter : FrameFilter
        {
            if (!IsSessionPartAndEmpty(sessionPart))
            {
                throw new InvalidOperationException($"{sessionPart} is not part of {nameof(ARSession)} or it is not empty");
            }

            GameObject go;
            if (typeof(Filter) == typeof(DenseSpatialMapBuilderFrameFilter))
            {
                go = CreateDenseSpatialMapBuilder();
            }
            else
            {
                go = new GameObject(DefaultName<Filter>(), typeof(Filter));
            }
            go.transform.SetParent(sessionPart.transform, false);
#if UNITY_EDITOR
            Undo.RegisterCreatedObjectUndo(go, $"Create {go.name}");
#endif
            return go;
        }

        public static GameObject AddFrameSource<Source>(GameObject sessionPart) where Source : FrameSource
        {
            if (!IsSessionPartAndEmpty(sessionPart))
            {
                throw new InvalidOperationException($"{sessionPart} is not part of {nameof(ARSession)} or it is not empty");
            }
            var go = new GameObject(DefaultName<Source>(), typeof(Source));
            go.transform.SetParent(sessionPart.transform, false);
#if UNITY_EDITOR
            Undo.RegisterCreatedObjectUndo(go, $"Create {go.name}");
#endif
            return go;
        }

        public static GameObject AddFramePlayer(GameObject sessionPart)
        {
            if (!IsSessionPartAndEmpty(sessionPart))
            {
                throw new InvalidOperationException($"{sessionPart} is not part of {nameof(ARSession)} or it is not empty");
            }
            var go = new GameObject(DefaultName<FramePlayer>(), typeof(FramePlayer));
            go.transform.SetParent(sessionPart.transform, false);
#if UNITY_EDITOR
            Undo.RegisterCreatedObjectUndo(go, $"Create {go.name}");
#endif
            return go;
        }

        public static GameObject AddFrameRecorder(GameObject sessionPart)
        {
            if (!IsSessionPartAndEmpty(sessionPart))
            {
                throw new InvalidOperationException($"{sessionPart} is not part of {nameof(ARSession)} or it is not empty");
            }
            var go = new GameObject(DefaultName<FrameRecorder>(), typeof(FrameRecorder));
            go.transform.SetParent(sessionPart.transform, false);
#if UNITY_EDITOR
            Undo.RegisterCreatedObjectUndo(go, $"Create {go.name}");
#endif
            return go;
        }

        public static GameObject CreateVideoRecorder()
        {
            var go = new GameObject(DefaultName<VideoRecorder>(), typeof(VideoRecorder));
#if UNITY_EDITOR
            Undo.RegisterCreatedObjectUndo(go, $"Create {go.name}");
#endif
            return go;
        }

        public static GameObject CreateController<Controller>()
        {
            var type = typeof(Controller);
            if (type != typeof(WorldRootController)
                && type != typeof(ImageTargetController)
                && type != typeof(ObjectTargetController)
                && type != typeof(SurfaceTargetController)
                && type != typeof(SparseSpatialMapRootController)
                && type != typeof(SparseSpatialMapController)
                )
            {
                throw new InvalidOperationException($"{typeof(Controller)} is not a valid Controller");
            }
            GameObject go;
            if (type == typeof(SparseSpatialMapController))
            {
                go = CreateSparseSpatialMap();
            }
            else
            {
                go = CreateObject<Controller>();
            }
#if UNITY_EDITOR
            Undo.RegisterCreatedObjectUndo(go, $"Create {go.name}");
#endif
            return go;
        }

        public static void SetupMotionTracker(GameObject source, MotionTrackerPreset preset)
        {
            var m = source.GetComponent<MotionTrackerFrameSource>();
            if (!m) { return; }
            switch (preset)
            {
                case MotionTrackerPreset.PreciseAnchor:
                    m.DesiredMotionTrackerParameters.TrackingMode = MotionTrackerCameraDeviceTrackingMode.Anchor;
                    m.DesiredMotionTrackerParameters.MinQualityLevel = MotionTrackerCameraDeviceQualityLevel.NotSupported;
                    m.DesiredMotionTrackerParameters.FocusMode = MotionTrackerCameraDeviceFocusMode.Continousauto;
                    break;
                case MotionTrackerPreset.SpatialMap:
                    m.DesiredMotionTrackerParameters.TrackingMode = MotionTrackerCameraDeviceTrackingMode.SLAM;
                    m.DesiredMotionTrackerParameters.MinQualityLevel = MotionTrackerCameraDeviceQualityLevel.Bad;
                    m.DesiredMotionTrackerParameters.FocusMode = MotionTrackerCameraDeviceFocusMode.Continousauto;
                    break;
                case MotionTrackerPreset.Mega:
                    m.DesiredMotionTrackerParameters.TrackingMode = MotionTrackerCameraDeviceTrackingMode.LargeScale;
                    m.DesiredMotionTrackerParameters.MinQualityLevel = MotionTrackerCameraDeviceQualityLevel.Good;
                    m.DesiredMotionTrackerParameters.FocusMode = MotionTrackerCameraDeviceFocusMode.Continousauto;
                    break;
                case MotionTrackerPreset.ObjectSensing:
                case MotionTrackerPreset.MinimumResourceUsage:
                    m.DesiredMotionTrackerParameters.TrackingMode = MotionTrackerCameraDeviceTrackingMode.VIO;
                    m.DesiredMotionTrackerParameters.MinQualityLevel = MotionTrackerCameraDeviceQualityLevel.NotSupported;
                    m.DesiredMotionTrackerParameters.FocusMode = MotionTrackerCameraDeviceFocusMode.Continousauto;
                    break;
                default:
                    break;
            }
        }

        public static void SetupFrameSources(List<GameObject> sources, ARSessionPreset preset)
        {
            if (sources == null) { return; }

            switch (preset)
            {
                case ARSessionPreset.CameraDevice:
                case ARSessionPreset.ImageTracking:
                case ARSessionPreset.CloudRecognition:
                case ARSessionPreset.ObjectTracking:
                    foreach (var source in sources)
                    {
                        SetupCameraDevice(source, CameraDevicePreference.PreferObjectSensing);
                    }
                    break;
                case ARSessionPreset.SurfaceTracking:
                    foreach (var source in sources)
                    {
                        SetupCameraDevice(source, CameraDevicePreference.PreferSurfaceTracking);
                    }
                    break;
                case ARSessionPreset.FrameSourceAll:
                case ARSessionPreset.MotionTrackingPreferMotionTracker:
                case ARSessionPreset.MotionTrackingPreferSystem:
                    foreach (var source in sources)
                    {
                        SetupMotionTracker(source, MotionTrackerPreset.PreciseAnchor);
                    }
                    break;
                case ARSessionPreset.SparseSpatialMap:
                case ARSessionPreset.DenseSpatialMap:
                case ARSessionPreset.SparseAndDenseSpatialMap:
                    foreach (var source in sources)
                    {
                        SetupMotionTracker(source, MotionTrackerPreset.SpatialMap);
                    }
                    break;
                case ARSessionPreset.Mega:
                    foreach (var source in sources)
                    {
                        SetupMotionTracker(source, MotionTrackerPreset.Mega);
                    }
                    break;
                case ARSessionPreset.ImageTrackingMotionFusion:
                    foreach (var source in sources)
                    {
                        SetupCameraDevice(source, CameraDevicePreference.PreferObjectSensing);
                        SetupMotionTracker(source, MotionTrackerPreset.ObjectSensing);
                    }
                    break;
                default:
                    break;
            }
        }

        public static void SetupCameraDevice(GameObject source, CameraDevicePreference preference)
        {
            var c = source.GetComponent<CameraDeviceFrameSource>();
            if (!c) { return; }
            c.CameraPreference = preference;
        }

        public static string DefaultName<Component>()
        {
            return DefaultName(typeof(Component));
        }

        public static string DefaultName(Type type)
        {
            return string.Join(" ", Regex.Split(type.Name.Replace("FrameSource", "").Replace("FrameFilter", "").Replace("Controller", ""), @"(?<!^)(?<![A-Z])(?=[A-Z])"));
        }

        public static bool IsSessionPartAndEmpty(GameObject sessionPart)
        {
            if (!sessionPart) { return false; }
            var arSession = sessionPart.GetComponent<ARSession>();
            if (!arSession)
            {
                foreach (var session in sessionPart.GetComponentsInParent<ARSession>(true))
                {
                    arSession = session;
                    break;
                }
            }
            if (!arSession) { return false; }
            if (sessionPart.GetComponent<FrameFilter>() || sessionPart.GetComponent<FrameSource>() || sessionPart.GetComponent<FramePlayer>() || sessionPart.GetComponent<FrameRecorder>()) { return false; }
            return true;
        }

        static GameObject CreateSparseSpatialMap()
        {
            var go = CreateObject<SparseSpatialMapController>();
            var controller = go.GetComponent<SparseSpatialMapController>();
            var pgo = new GameObject("Point Cloud Particle System", typeof(ParticleSystem));
            pgo.transform.SetParent(go.transform, false);

            var particle = pgo.GetComponent<ParticleSystem>();
            var main = particle.main;
            main.loop = false;
            main.startSize = 0.015f;
            main.startColor = new Color(11f / 255f, 205f / 255f, 255f / 255f, 1);
            main.scalingMode = ParticleSystemScalingMode.Hierarchy;
            main.playOnAwake = false;
            var emission = particle.emission;
            emission.enabled = false;
            var shape = particle.shape;
            shape.enabled = false;
            var renderer = particle.GetComponent<Renderer>();
#if UNITY_EDITOR
            renderer.material = AssetDatabase.LoadAssetAtPath<Material>($"Packages/{UnityPackage.Name}/Assets/Materials/PointCloudParticle.mat");
#else
            throw new NotImplementedException();
#endif
            controller.PointCloudParticleSystem = particle;
            return go;
        }

        static GameObject CreateDenseSpatialMapBuilder()
        {
            var go = CreateObject<DenseSpatialMapBuilderFrameFilter>();
            var filter = go.GetComponent<DenseSpatialMapBuilderFrameFilter>();
            var renderer = go.GetComponent<DenseSpatialMapDepthRenderer>();
#if UNITY_EDITOR
            filter.MapMeshMaterial = AssetDatabase.LoadAssetAtPath<Material>($"Packages/{UnityPackage.Name}/Assets/Materials/DenseSpatialMapMesh.mat");
            renderer.Shader = AssetDatabase.LoadAssetAtPath<Shader>($"Packages/{UnityPackage.Name}/Assets/Shaders/DenseSpatialMapDepth.shader");
#else
            throw new NotImplementedException();
#endif
            return go;
        }

        static GameObject CreateObject<Component>(string name = null)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = DefaultName<Component>();
            }
            return new GameObject(name, typeof(Component));
        }
    }
}
