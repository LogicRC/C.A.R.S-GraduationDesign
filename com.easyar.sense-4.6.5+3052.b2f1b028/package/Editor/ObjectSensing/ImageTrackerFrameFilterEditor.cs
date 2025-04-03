//================================================================================================================================
//
//  Copyright (c) 2015-2023 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
//  EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
//  and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
//
//================================================================================================================================

using UnityEngine;
using UnityEditor;

namespace easyar
{
    [CustomEditor(typeof (ImageTrackerFrameFilter), true)]
    public class ImageTrackerFrameFilterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            var simultaneousNum = serializedObject.FindProperty("simultaneousNum");
            EditorGUILayout.PropertyField(simultaneousNum, new GUIContent("Simultaneous Target Number"), true);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("resultType"));
            EditorGUI.indentLevel += 1;
            var enablePersistentTargetInstance = serializedObject.FindProperty("resultType.EnablePersistentTargetInstance");
            var enableMotionFusion = serializedObject.FindProperty("resultType.EnableMotionFusion");
            EditorGUILayout.PropertyField(enablePersistentTargetInstance);
            EditorGUILayout.PropertyField(enableMotionFusion);
            EditorGUI.indentLevel -= 1;

            serializedObject.ApplyModifiedProperties();
            ((ImageTrackerFrameFilter)target).SimultaneousNum = simultaneousNum.intValue;
            ((ImageTrackerFrameFilter)target).ResultType.EnablePersistentTargetInstance = enablePersistentTargetInstance.boolValue;
            ((ImageTrackerFrameFilter)target).ResultType.EnableMotionFusion = enableMotionFusion.boolValue;
        }
    }
}
