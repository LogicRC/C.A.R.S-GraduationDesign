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
    [CustomEditor(typeof (ObjectTrackerFrameFilter), true)]
    public class ObjectTrackerFrameFilterEditor : Editor
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
            ((ObjectTrackerFrameFilter)target).SimultaneousNum = simultaneousNum.intValue;
            ((ObjectTrackerFrameFilter)target).ResultType.EnablePersistentTargetInstance = enablePersistentTargetInstance.boolValue;
            ((ObjectTrackerFrameFilter)target).ResultType.EnableMotionFusion = enableMotionFusion.boolValue;
        }
    }
}
