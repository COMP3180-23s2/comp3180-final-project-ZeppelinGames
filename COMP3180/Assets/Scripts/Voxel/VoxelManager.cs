using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class VoxelManager : MonoBehaviour
{
    [SerializeField] private TextAsset voxelDataFile;
    [SerializeField] private TextAsset voxelShapeFile;

    private VoxelBuilder voxelBuilder = new VoxelBuilder();

    VoxelData voxelData;
    VoxelShape voxelShape;

    private MeshFilter meshFilter;

    private void Start()
    {
        meshFilter = GetComponent<MeshFilter>();

        voxelData = new VoxelData(voxelDataFile);
        voxelShape = new VoxelShape(voxelShapeFile);

        Mesh mesh = voxelBuilder.Build(voxelData, voxelShape);
        meshFilter.mesh = mesh;
    }

    private struct GUIButtonEvent
    {
        public string buttonText;
        public Action action;
        public string DebugText;

        public GUIButtonEvent(string buttonText, Action action, string DebugText = "")
        {
            this.buttonText = buttonText;
            this.action = action;
            this.DebugText = DebugText;
        }
    }

    GUIButtonEvent[] buttonEvents;
    private void Awake()
    {
        buttonEvents = new GUIButtonEvent[] {
             new GUIButtonEvent("Rebuild", () => { voxelBuilder.Build(voxelData, voxelShape); }, "Rebuilding mesh..."),
          };
    }

    private void OnGUI()
    {
        if (buttonEvents != null)
        {
            float y = Screen.height - 30;
            for (int i = 0; i < buttonEvents.Length; i++)
            {
                if (GUI.Button(new Rect(10, y, 100, 25), buttonEvents[i].buttonText))
                {
                    buttonEvents[i].action?.Invoke();
                    if (buttonEvents[i].DebugText.Length > 0)
                    {
                        Debug.Log(buttonEvents[i].DebugText);
                    }
                }
                y -= 30f;
            }
        }
    }
}
