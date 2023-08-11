using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MeshToVoxEditorWindow : EditorWindow
{
    [MenuItem("Tools/Mesh to Vox")]
    public static void ShowWindow()
    {
        GetWindow(typeof(MeshToVoxEditorWindow));
    }

    private TextAsset fileObject;

    void OnGUI()
    {
        fileObject = (TextAsset)EditorGUILayout.ObjectField(fileObject, typeof(TextAsset), false);

        // load and read that shit
        // add data, dist between verts
    }
}