using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using System;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(VoxelRenderer))]
public class VoxelRendererEditor : Editor
{
    SerializedProperty voxelDataFile;
    SerializedProperty overrideShape;
    SerializedProperty voxelShapeFile;

    SerializedProperty overrideDefaultMaterialBool;
    SerializedProperty material;

    private static bool editModeActive;
    private static VoxelDataEditor voxelDataEditor;

    private static string defaultFileContents = @"# New Voxel Mesh; # Points; p0,0,0,0; c255,255,255,255;";

    private static VoxelRenderer currentTarget;

    [MenuItem("GameObject/3D Object/Voxel", priority = 6)]
    public static void CreateVoxel()
    {
        GameObject newVoxelGO = EditorUtility.CreateGameObjectWithHideFlags("Voxel", HideFlags.None);
        VoxelRenderer vr = newVoxelGO.AddComponent<VoxelRenderer>();
        VoxelCollider vc = newVoxelGO.AddComponent<VoxelCollider>();

        Selection.activeObject = newVoxelGO;
    }

    void OnEnable()
    {
        overrideShape = serializedObject.FindProperty("overrideShape");
        voxelDataFile = serializedObject.FindProperty("voxelDataFile");
        voxelShapeFile = serializedObject.FindProperty("voxelShapeFile");

        overrideDefaultMaterialBool = serializedObject.FindProperty("overrideDefaultMaterial");
        material = serializedObject.FindProperty("mat");
        
        UpdateVoxel();
    }

    private void OnDisable()
    {
        if (voxelDataEditor != null)
        {
            DestroyImmediate(voxelDataEditor.gameObject);
        }
        voxelDataEditor = null;
    }

    public override void OnInspectorGUI()
    {
        VoxelRenderer renderer = (VoxelRenderer)target;
        if (renderer == null)
        {
            return;
        }

        if (renderer != currentTarget)
        {
            // prompt to save/discard 
            editModeActive = false;
            currentTarget = renderer;
        }

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

        bool edit = false;

        if (editModeActive)
        {
            GUILayout.BeginHorizontal();

            bool discard = GUILayout.Button("Discard Changes");
            bool save = GUILayout.Button("Save Voxel");

            GUILayout.EndHorizontal();

            if (save || discard)
            {
                edit = true;
            }

            if (save)
            {
                string dataPath = $"./{AssetDatabase.GetAssetPath(renderer.VoxelDataFile)}";

                string saveData = "";

                StreamWriter writer = new StreamWriter(dataPath);
                writer.WriteLine("new contents");
                writer.Close();
            }

        }
        else
        {
            edit = GUILayout.Button(renderer.VoxelDataFile == null ? "Create Voxel Data" : "Edit Voxel");
        }

        bool refresh = GUILayout.Button("Refresh Voxel");

        bool updated = serializedObject.ApplyModifiedProperties();

        if (updated || refresh)
        {
            UpdateVoxel();
        }

        if (edit)
        {
            // toggle edit mode
            editModeActive = !editModeActive;

            if (editModeActive)
            {
                // Create new data file if one doesnt exist
                if (renderer.VoxelDataFile == null)
                {
                    string savePath = EditorUtility.SaveFilePanel("New Voxel File", Application.dataPath, "NewVoxel", "txt");
                    if (savePath.Length > 0)
                    {
                        TextAsset newTextAsset = CreateNewVoxelDataFile(savePath);
                        renderer.VoxelDataFile = newTextAsset;
                        UpdateVoxel();
                    }
                    else
                    {
                        Debug.LogWarning("Save data file cancelled");
                        editModeActive = false;
                        return;
                    }
                }


                if (voxelDataEditor == null)
                {
                    GameObject editorGO = new GameObject("EditorGO");
                    Debug.Log($"Created {editorGO.name}");
                    voxelDataEditor = editorGO.AddComponent<VoxelDataEditor>();
                    voxelDataEditor.SetVoxel(renderer);
                }
                else
                {
                    voxelDataEditor.SetVoxel(renderer);
                }
            }
            else
            {
                if (voxelDataEditor != null)
                {
                    Debug.Log("Destroyed");
                    DestroyImmediate(voxelDataEditor.gameObject);
                    voxelDataEditor = null;
                }
            }
        }
    }

    TextAsset CreateNewVoxelDataFile(string savePath)
    {
        string relativePath = FileUtil.GetProjectRelativePath(savePath);
        string filename = Path.GetFileName(savePath);

        // write default voxel data to file
        StreamWriter writer = new StreamWriter(savePath);
        writer.WriteLine(defaultFileContents);
        writer.Close();

        AssetDatabase.ImportAsset(relativePath);

        // create new text asset
        TextAsset newTextAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(relativePath);
        newTextAsset.name = filename;

        return newTextAsset;
    }

    void UpdateVoxel()
    {
        VoxelRenderer renderer = (VoxelRenderer)target;
        if (renderer != null)
        {
            renderer.LoadMesh();
            renderer.BuildMesh();

            if (renderer.TryGetComponent(out VoxelCollider vc))
            {
                vc.ResetCollidersEditor();
                vc.RefreshCollider();
            }
        }
    }
}