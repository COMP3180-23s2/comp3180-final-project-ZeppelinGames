using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class VoxelEditor : EditorWindow
{
	[MenuItem("Window/Voxel Editor")]
	public static void ShowWindow()
	{
		GetWindow(typeof(VoxelEditor));
	}

	private PreviewRenderUtility previewUtility;
	private GameObject baseObject;

	private GameObject targetObject;

	private List<GameObject> voxels = new List<GameObject>();

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

		if (targetObject != null)
		{
			DestroyImmediate(targetObject);
		}
		if (baseObject != null)
		{
			DestroyImmediate(baseObject);
		}

		for (int i = 0; i < voxels.Count; i++)
		{
			if (voxels[i] != null)
			{
				DestroyImmediate(voxels[i]);
			}
		}
	}

	private void SetupPreviewScene()
	{
        // Add light
        Light mainLight = new GameObject().AddComponent<Light>();
        mainLight.type = LightType.Directional;
        mainLight.transform.eulerAngles = new Vector3(50f, -30f, 0);

        baseObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        baseObject.transform.position = Vector3.zero;

        targetObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        targetObject.transform.position = Vector3.one;

        // Since we want to manage this instance ourselves, hide it
        // from the current active scene, but remember to also destroy it.
        baseObject.hideFlags = HideFlags.HideAndDontSave;

        // Register objects
        previewUtility.AddSingleGO(mainLight.gameObject);
        previewUtility.AddSingleGO(baseObject);
        previewUtility.AddSingleGO(targetObject);

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

		voxels.Add(newCube);
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
                        for (int i = 0; i < voxels.Count; i++)
                        {
							// check every voxel face that is facing out ray origin
							Plane facePlane = new Plane(voxels[i].transform.position, voxels[i].transform.position.magnitude);
							Ray camPointRay = previewUtility.camera.ScreenPointToRay(e.mousePosition);
							facePlane.Raycast(camPointRay, out float dist);
                        }

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
	}
}