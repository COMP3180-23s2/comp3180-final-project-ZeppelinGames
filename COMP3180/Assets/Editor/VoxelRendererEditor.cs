using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VoxelRenderer))]
public class VoxelRendererEditor : Editor
{
    SerializedProperty voxelDataFile;
    SerializedProperty overrideShape;
    SerializedProperty voxelShapeFile;

    SerializedProperty overrideDefaultMaterialBool;
    SerializedProperty material;

    bool editModeActive;

    void OnEnable()
    {
        overrideShape = serializedObject.FindProperty("overrideShape");
        voxelDataFile = serializedObject.FindProperty("voxelDataFile");
        voxelShapeFile = serializedObject.FindProperty("voxelShapeFile");

        overrideDefaultMaterialBool = serializedObject.FindProperty("overrideDefaultMaterial");
        material = serializedObject.FindProperty("mat");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(voxelDataFile);
        EditorGUILayout.PropertyField(overrideShape);
        if (overrideShape.boolValue)
        {
            EditorGUILayout.PropertyField(voxelShapeFile);
        }

        EditorGUILayout.PropertyField(overrideDefaultMaterialBool);
        if (overrideDefaultMaterialBool.boolValue)
        {
            EditorGUILayout.PropertyField(material);
        }

        bool edit = GUILayout.Button(editModeActive ? "Stop editing" : "Edit Voxel");

        bool refresh = GUILayout.Button("Refresh Voxel");

        bool updated = serializedObject.ApplyModifiedProperties();

        if (updated || refresh)
        {
            UpdateMesh();
        }

        if (edit)
        {
            // toggle edit mode
            editModeActive = !editModeActive;
            if (!editModeActive)
            {
                // just exited edit mode,
                // write file
                // update reference
            }
        }
    }

    void UpdateMesh()
    {
        VoxelRenderer renderer = (VoxelRenderer)target;
        if (renderer != null)
        {
            renderer.LoadMesh();
            renderer.BuildMesh();

            if (renderer.TryGetComponent(out VoxelCollider vc))
            {
                Debug.Log("Updating collider");
                vc.ResetCollidersEditor();
                vc.RefreshCollider();
            }
        }
    }
}