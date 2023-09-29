using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[RequireComponent(typeof(VoxelRenderer))]
public class VoxelManager : MonoBehaviour
{
    [SerializeField] private TextAsset voxelDataFile;
    [SerializeField] private TextAsset voxelShapeFile;

    [SerializeField] private Material mat;

    private VoxelRenderer voxelRenderer;

    private VoxelData voxData;
    private VoxelShape voxShape;

    private void Start()
    {
        UpdateMesh();
    }

    void OnValidate()
    {
        UpdateMesh();
    }

    public void UpdateMesh()
    {
        if (voxelRenderer == null)
        {
            if (!TryGetComponent(out VoxelRenderer vr))
            {
                return;
            }
            voxelRenderer = vr;
        }

        if (voxelShapeFile == null || voxelDataFile == null)
        {
            return;
        }

        if (voxData == null)
        {
            voxData = new VoxelData(voxelDataFile);
        }
        if (voxShape == null)
        {
            voxShape = new VoxelShape(voxelShapeFile);
        }

        voxelRenderer.VoxelShape = voxShape;
        voxelRenderer.VoxelData = voxData;
    }

    void FullFracture()
    {
        this.gameObject.SetActive(false);
        for (int i = 0; i < voxData.VoxelPoints.Length; i++)
        {
            CreateSingleVoxel(voxData.VoxelPoints[i].Position, voxData.Colors[voxData.VoxelPoints[i].ColorIndex]);
        }
    }

    public void CreateSingleVoxel(Vector3Int pos, Color c)
    {
        GameObject go = new GameObject("Voxel");
        
        go.AddComponent<Rigidbody>();

        // Set material
        MeshRenderer mr = go.AddComponent<MeshRenderer>();
        mr.material = mat;

        // Set position
        go.transform.position = (Vector3)pos * VoxelBuilder.VoxelSize + transform.position;

        // Add voxel renderer
        VoxelRenderer vr = go.AddComponent<VoxelRenderer>();
        vr.VoxelShape = new VoxelShape(voxelShapeFile);
        vr.VoxelData = new VoxelData(new VoxelPoint[] { new VoxelPoint(Vector3Int.zero, 0) }, new Color[] { c });

        // Add voxel collider
        go.AddComponent<VoxelCollider>();
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
             new GUIButtonEvent("Rebuild", () => {
                voxelRenderer.VoxelData = voxData;
                voxelRenderer.VoxelShape = voxShape;
             }, "Rebuilding mesh..."),

              new GUIButtonEvent("Complete Fracture", () => {
                    FullFracture();
             }, "Fracturing..."),
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
