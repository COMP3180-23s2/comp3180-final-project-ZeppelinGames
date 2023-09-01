using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

public class VoxelEditor : EditorWindow
{
    [MenuItem("Window/Voxel Editor")]
    public static void ShowWindow()
    {
        GetWindow(typeof(VoxelEditor));
    }

    private PreviewRenderUtility previewUtility;

    private List<Vector3Int> keys = new List<Vector3Int>();
    private Dictionary<Vector3Int, GameObject> voxels = new Dictionary<Vector3Int, GameObject>();

    private Transform cameraPivot;
    private float cameraZoom = 5f;
    private Vector2 minmaxCameraZoom = new Vector2(5f, 15f);

    private Vector2 mousePos = Vector2.zero;

    private void OnEnable()
    {
        previewUtility = new PreviewRenderUtility();
        this.wantsMouseMove = true;
        SetupPreviewScene();
    }

    private void OnDisable()
    {
        if (previewUtility != null)
        {
            previewUtility.Cleanup();
        }

        for (int i = 0; i < keys.Count; i++)
        {
            if (voxels[keys[i]] != null)
            {
                DestroyImmediate(voxels[keys[i]]);
            }
        }
        keys.Clear();
        voxels.Clear();
    }

    private void SetupPreviewScene()
    {
        // Add light
        Light mainLight = new GameObject().AddComponent<Light>();
        mainLight.type = LightType.Directional;
        mainLight.transform.eulerAngles = new Vector3(50f, -30f, 0);

        // Register objects
        previewUtility.AddSingleGO(mainLight.gameObject);

        CreateVoxel(Vector3Int.zero);
        CreateVoxel(new Vector3Int(0, 2, 1));

        previewUtility.camera.cameraType = CameraType.SceneView;

        cameraPivot = new GameObject().GetComponent<Transform>();
        cameraPivot.gameObject.hideFlags = HideFlags.HideAndDontSave;
        previewUtility.camera.transform.SetParent(cameraPivot);

        // Camera is spawned at origin, so position is in front of the cube.
        previewUtility.camera.transform.localPosition = new Vector3(-5f, 7.5f, -5f);
        previewUtility.camera.transform.eulerAngles = new Vector3(45f, 45f, 0);

        // Make camera ortho
        previewUtility.camera.orthographic = true;

        // Set clipping planes
        previewUtility.camera.nearClipPlane = 5f;
        previewUtility.camera.farClipPlane = 20f;

        Repaint();
    }

    private void CreateVoxel(Vector3Int position)
    {
        GameObject newCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        newCube.hideFlags = HideFlags.HideAndDontSave;
        newCube.transform.position = position;
        previewUtility.AddSingleGO(newCube);

        keys.Add(position);
        voxels.Add(position, newCube);
    }

    private void RemoveVoxel(Vector3Int position)
    {
        if (voxels.ContainsKey(position) && position != Vector3Int.zero)
        {
            DestroyImmediate(voxels[position]);

            keys.Remove(position);
            voxels.Remove(position);
        }
    }

    private void OnGUI()
    {
        Event e = Event.current;
        if (e != null)
        {
            switch (e.type)
            {
                case EventType.MouseDrag:
                    // middle mouse drag
                    if (e.button == 2)
                    {
                        cameraPivot.rotation *= Quaternion.Euler(0, e.delta.x, 0);
                        Repaint();
                    }
                    break;

                case EventType.ScrollWheel:
                    cameraZoom += e.delta.y;
                    cameraZoom = Mathf.Clamp(cameraZoom, minmaxCameraZoom.x, minmaxCameraZoom.y);
                    previewUtility.camera.orthographicSize = cameraZoom;
                    Repaint();
                    break;

                case EventType.MouseDown:
                    // lmb
                    if (e.button == 0)
                    {
                        // implement your own really cool raycasting here (cause unity stinky)
                        // use plane raycast + AABB to check face hit

                        // loop over every voxel (yeah ik)
                        /*for (int i = 0; i < voxels.Count; i++)
                        {
							// check every voxel face that is facing out ray origin
							Plane facePlane = new Plane(voxels[i].transform.position, voxels[i].transform.position.magnitude);
							Ray camPointRay = previewUtility.camera.ScreenPointToRay(e.mousePosition);
							facePlane.Raycast(camPointRay, out float dist);
							Debug.Log(dist);
                        }*/

                        Repaint();
                    }
                    break;

                case EventType.MouseMove:
                    mousePos = e.mousePosition;
                    Repaint();
                    break;
            }
        }


        // Render the preview scene into a texture
        Rect rect = new Rect(0, 0, base.position.width, base.position.height);
        previewUtility.BeginPreview(rect, GUIStyle.none);
        previewUtility.Render();

        Texture texture = previewUtility.EndPreview();

        GUI.DrawTexture(rect, texture);

        GUI.Label(new Rect(10, 10, 100, 100), $"{mousePos.x}, {mousePos.y}");
        bool export = GUI.Button(new Rect(10, 25, 100, 25), "Export");
        if (export)
        {
            string savePath = EditorUtility.SaveFilePanel("Save Voxel", Application.dataPath, "MyVox", "txt");
            if (savePath.Length > 0)
            {
                StreamWriter sw = new StreamWriter(savePath);
                sw.Write("v");
                for (int i = 0; i < keys.Count; i++)
                {
                    sw.Write($"{keys[i].x},{keys[i].y},{keys[i].z},0;");
                }
                sw.Close();
            }
        }
    }
}