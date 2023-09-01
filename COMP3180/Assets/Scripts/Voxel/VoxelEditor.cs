using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public static class VoxelEditor
{
    public static void ShowVoxelEditor(VoxelBuilder go)
    {
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();

        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        Selection.activeObject = go.gameObject;
        SceneView.lastActiveSceneView.FrameSelected();
    }
}