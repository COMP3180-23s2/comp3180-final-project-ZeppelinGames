using System.Collections;
using System.Collections.Generic;
using System.IO;
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

    private static string defaultFileContents = @"# New Voxel Mesh; # Points; p0,0,0,0; c255,255,255,255;";

    [MenuItem("GameObject/3D Object/Voxel", priority = 6)]
    public static void CreateVoxel()
    {
        GameObject newVoxelGO = EditorUtility.CreateGameObjectWithHideFlags("Voxel", HideFlags.None);
        VoxelRenderer vr = newVoxelGO.AddComponent<VoxelRenderer>();

        Selection.activeObject = newVoxelGO;
    }

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
            if (editModeActive)
            {
                // start editing
                VoxelRenderer renderer = (VoxelRenderer)target;
                if (renderer.VoxelDataFile == null)
                {
                    string savePath = EditorUtility.SaveFilePanel("New Voxel File", Application.dataPath, "NewVoxel", "txt");
                    if (savePath.Length > 0)
                    {
                        TextAsset newTextAsset = CreateNewVoxelDataFile(savePath);
                        AssetDatabase.Refresh();

                        renderer.VoxelDataFile = newTextAsset;
                        renderer.LoadMesh();
                        renderer.BuildMesh();
                    }
                    else
                    {
                        editModeActive = false;
                        return;
                    }
                }
            }
            else
            {
                // just exited edit mode,
                // write file
                // update reference
            }
        }
    }

    TextAsset CreateNewVoxelDataFile(string savePath)
    {
        string filename = Path.GetFileName(savePath);

        // create new text asset
        TextAsset newTextAsset = new TextAsset(defaultFileContents);
        newTextAsset.name = filename;

        // write default voxel data to file
        StreamWriter writer = new StreamWriter(savePath);
        writer.WriteLine(newTextAsset.text);
        writer.Close();

        return newTextAsset;
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