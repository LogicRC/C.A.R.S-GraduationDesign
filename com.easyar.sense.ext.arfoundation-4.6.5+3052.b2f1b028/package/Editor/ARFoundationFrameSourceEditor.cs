//================================================================================================================================
//
//  Copyright (c) 2015-2024 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
//  EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
//  and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
//
//================================================================================================================================

using UnityEditor;
using UnityEngine;

namespace easyar
{
    [CustomEditor(typeof(ARFoundationFrameSource), true)]
    public class ARFoundationFrameSourceEditor : FrameSourceEditor
    {
        private readonly string tooltip = "Origin will be setup with FindObjectOfType result at runtime if no candidate provided.";

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var fs = target as ARFoundationFrameSource;
            MonoBehaviour candidate = null;
            var originCandidate = fs.OriginCandidate;

#if EASYAR_ENABLE_XRORIGIN_AR
            var c1 = originCandidate as Unity.XR.CoreUtils.XROrigin;
            candidate = EditorGUILayout.ObjectField(new GUIContent($"Origin Candidate ({nameof(Unity.XR.CoreUtils.XROrigin)})", tooltip), c1, typeof(Unity.XR.CoreUtils.XROrigin), true) as MonoBehaviour;
            if (candidate != c1)
            {
                fs.OriginCandidate = candidate;
                EditorUtility.SetDirty(fs);
            }
#endif

#if EASYAR_ENABLE_ARSESSIONORIGIN
#pragma warning disable 612, 618
            var c2 = originCandidate as UnityEngine.XR.ARFoundation.ARSessionOrigin;
            candidate = EditorGUILayout.ObjectField(new GUIContent($"Origin Candidate ({nameof(UnityEngine.XR.ARFoundation.ARSessionOrigin)})", tooltip), c2, typeof(UnityEngine.XR.ARFoundation.ARSessionOrigin), true) as MonoBehaviour;
            if (candidate != c2)
            {
                fs.OriginCandidate = candidate;
                EditorUtility.SetDirty(fs);
            }
#pragma warning restore 612, 618
#endif
        }
    }
}
