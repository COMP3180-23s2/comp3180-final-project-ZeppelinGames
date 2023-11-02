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
    SerializedProperty voxelBreakType;

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
        EditorApplication.playModeStateChanged += EditorApplication_playModeStateChanged;

        overrideShape = serializedObject.FindProperty("overrideShape");
        voxelDataFile = serializedObject.FindProperty("voxelDataFile");
        voxelShapeFile = serializedObject.FindProperty("voxelShapeFile");
        voxelBreakType = serializedObject.FindProperty("breakType");

        overrideDefaultMaterialBool = serializedObject.FindProperty("overrideDefaultMaterial");
        material = serializedObject.FindProperty("mat");

        UpdateVoxel();
    }

    private void EditorApplication_playModeStateChanged(PlayModeStateChange state)
    {
        switch (state)
        {
            case PlayModeStateChange.EnteredPlayMode:
                DeleteEditorGO();
                break;
        }
    }

    private void OnDisable()
    {
        DeleteEditorGO();
    }

    void DeleteEditorGO()
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
        EditorGUILayout.PropertyField(voxelBreakType);

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

            if (discard)
            {
                LoadAndUpdate();
            }

            if (save)
            {
                string dataPath = $"./{AssetDatabase.GetAssetPath(renderer.VoxelDataFile)}";

                StreamWriter writer = new StreamWriter(dataPath);

                VoxelData vd = renderer.VoxelData;
                Debug.Log($"Writing {vd.VoxelPoints.Length} points to file");

                // write verts
                writer.Write('p');
                for (int i = 0; i < vd.VoxelPoints.Length; i++)
                {
                    string data = $"{vd.VoxelPoints[i].Position.x},{vd.VoxelPoints[i].Position.y},{vd.VoxelPoints[i].Position.z},{vd.VoxelPoints[i].ColorIndex};";
                    writer.Write(data);
                }

                // write colors
                writer.Write('c');
                for (int i = 0; i < vd.Colors.Length; i++)
                {
                    string data = $"{(int)(vd.Colors[i].r * 255f)},{(int)(vd.Colors[i].g * 255f)},{(int)(vd.Colors[i].b * 255f)}, {(int)(vd.Colors[i].a * 255f)};";
                    writer.Write(data);
                }

                writer.Close();

                //AssetDatabase.ImportAsset(dataPath);
                AssetDatabase.Refresh();
                renderer.BuildMesh();
            }
        }
        else
        {
            if (renderer.VoxelDataFile == null)
            {
                edit = GUILayout.Button("Create Voxel Data");

                if (edit && !editModeActive)
                {
                    if (renderer.VoxelDataFile == null)
                    {
                        string savePath = EditorUtility.SaveFilePanel("New Voxel File", Application.dataPath, "NewVoxel", "txt");
                        if (savePath.Length > 0)
                        {
                            TextAsset newTextAsset = CreateNewVoxelDataFile(savePath);
                            renderer.VoxelDataFile = newTextAsset;
                            LoadAndUpdate();
                        }
                        else
                        {
                            Debug.LogWarning("Save data file cancelled");
                            editModeActive = false;
                            return;
                        }
                    }
                }
            }
            else if (renderer.VoxelData.VoxelPoints.Length == 0)
            {
                edit = GUILayout.Button("Create Voxel Data");

                if (edit && !editModeActive)
                {
                    string dataPath = $"./{AssetDatabase.GetAssetPath(renderer.VoxelDataFile)}";
                    StreamWriter writer = new StreamWriter(dataPath, false);
                    writer.WriteLine(defaultFileContents);
                    writer.Close();
                }
            }
            else
            {
                edit = GUILayout.Button("Edit Voxel");
            }
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
                if (voxelDataEditor == null)
                {
                    GameObject editorGO = new GameObject("EditorGO");
                    voxelDataEditor = editorGO.AddComponent<VoxelDataEditor>();
                    voxelDataEditor.SetVoxel(renderer);
                }
                else
                {
                    UpdateVoxel();
                    voxelDataEditor.SetVoxel(renderer);
                }
            }
            else
            {
                if (voxelDataEditor != null)
                {
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

    void LoadAndUpdate()
    {
        VoxelRenderer renderer = (VoxelRenderer)target;
        if (renderer != null)
        {
            renderer.LoadMesh();
            renderer.BuildMesh();

            if (renderer.TryGetComponent(out VoxelCollider vc))
            {
             /*   vc.ResetCollidersEditor();
                vc.BuildCollider();*/
            }
        }
    }

    void UpdateVoxel()
    {
        VoxelRenderer renderer = (VoxelRenderer)target;
        if (renderer != null)
        {
            renderer.BuildMesh();

            if (renderer.TryGetComponent(out VoxelCollider vc))
            {
                vc.ResetCollidersEditor();
                vc.BuildCollider();
            }
        }
    }
}