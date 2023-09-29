using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class VoxelEditorWindow : SceneView
{
    private static EditorWindow window;
    private static Scene scene;

    [MenuItem("Window/Voxel Editor")]
    public static void ShowWindow()
    {
        if (window == null)
        {
            window = GetWindow(typeof(VoxelEditorWindow));
            Texture2D icon = EditorGUIUtility.Load("Icons/VoxelEditorWindow.png") as Texture2D;
            window.titleContent = new GUIContent("Voxel Editor", icon);
        }
        else
        {
            window.Show();
        }

        scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
        scene.name = "Voxel Editor";
        sceneClosed = false;

        EditorSceneManager.sceneClosing += EditorSceneManager_sceneClosing;
    }


    private static bool sceneClosed = false;
    private static void EditorSceneManager_sceneClosing(Scene scene, bool removingScene)
    {
        if (VoxelEditorWindow.scene == scene)
        {
            sceneClosed = true;
            Debug.Log("SCENE CLOSING");
            window.Close();
        }
    }

    protected new void OnDestroy()
    {
        CloseWindow(false, true);
    }

    protected static void CloseWindow(bool closeWindow = true, bool closeScene = true)
    {
        if (closeWindow)
        {
            Debug.Log("CLOSE CLOSING");
            window.Close();
        }
        if (closeScene)
        {
            EditorSceneManager.CloseScene(scene, true);
        }
    }

    protected override void OnGUI()
    {
        using (new GUILayout.HorizontalScope())
        {
            GUILayout.Button("Save Voxel");
            GUILayout.Button("Load Voxel");
        }

        base.OnGUI();
    }
}