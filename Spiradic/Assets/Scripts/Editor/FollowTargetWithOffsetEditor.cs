// using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(FollowTargetWithOffset))]
public class FollowTargetWithOffsetEditor : Editor {
    private ReorderableList _list;

    private void OnEnable() {
        _list = new ReorderableList(serializedObject,
                serializedObject.FindProperty("_followInfos"),
                true, true, true, true);
    }

    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        serializedObject.Update();
        _list.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
}