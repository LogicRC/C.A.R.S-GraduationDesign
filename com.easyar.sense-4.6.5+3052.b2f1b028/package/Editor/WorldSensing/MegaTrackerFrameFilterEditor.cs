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
    [CustomEditor(typeof(MegaTrackerFrameFilter), true)]
    public class MegaTrackerFrameFilterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
#if !EASYAR_ENABLE_MEGA
            EditorGUILayout.HelpBox($"Package com.easyar.mega is required to use {nameof(MegaTrackerFrameFilter)}", MessageType.Error);
#else
            DrawDefaultInspector();

            if (!((MegaTrackerFrameFilter)target).UseGlobalServiceConfig)
            {
                var serviceConfig = serializedObject.FindProperty("ServiceConfig");
                serviceConfig.isExpanded = EditorGUILayout.Foldout(serviceConfig.isExpanded, "Service Config");
                EditorGUI.indentLevel += 1;
                if (serviceConfig.isExpanded)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("ServiceConfig.ServerAddress"), true);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("ServiceConfig.APIKey"), true);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("ServiceConfig.APISecret"), true);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("ServiceConfig.AppID"), true);
                }
                EditorGUI.indentLevel -= 1;
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("requestTimeParameters"), true);
            EditorGUI.indentLevel += 1;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("requestTimeParameters.Timeout"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("requestTimeParameters.RequestInterval"), true);
            EditorGUI.indentLevel -= 1;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("Fallbacks"), true);
            EditorGUI.indentLevel += 1;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Fallbacks.AllowNoTracking"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Fallbacks.AllowNonEifRemote"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Fallbacks.WarnAllowNoTracking"), true);
            EditorGUI.indentLevel -= 1;

            serializedObject.ApplyModifiedProperties();
#endif
        }
    }
}
