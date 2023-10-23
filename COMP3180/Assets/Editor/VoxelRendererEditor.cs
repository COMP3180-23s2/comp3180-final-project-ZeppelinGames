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

    private HashSet<Vector3Int> editingPoints = new HashSet<Vector3Int>();
    private VoxelRenderer currentTarget;

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
    }

    public override void OnInspectorGUI()
    {
        VoxelRenderer renderer = (VoxelRenderer)target;
        if(renderer == null)
        {
            return;
        }

        if (renderer != currentTarget)
        {
            // Target changed. Save if editing

            // Update current
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

            if(save || discard)
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

            GUILayout.EndHorizontal();
        }
        else
        {
            edit = GUILayout.Button("Edit Voxel");
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
                // start editing
                EditMode();

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
                        editModeActive = false;
                        return;
                    }
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

    void EditMode()
    {

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
                Debug.Log("Updating collider");
                vc.ResetCollidersEditor();
                vc.RefreshCollider();
            }
        }
    }
}