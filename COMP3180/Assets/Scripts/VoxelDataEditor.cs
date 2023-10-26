using System.Collections;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class VoxelDataEditor : MonoBehaviour
{
#if UNITY_EDITOR
    private VoxelRenderer voxel;
    private VoxelCollider voxelCol;
    private bool hadCollider;

    private void Start()
    {
        if (EditorApplication.isPlaying)
        {
            Destroy(this.gameObject);
        }
    }

    public void SetVoxel(VoxelRenderer renderer)
    {
        framed = false;
        voxel = renderer;
        voxel.TryGetComponent(out voxelCol);

        if (voxel.TryGetComponent(out VoxelCollider vc))
        {
            hadCollider = true;
            Debug.Log("Has collider");
        }
        else
        {
            hadCollider = false;
            VoxelCollider vcAdded = voxel.gameObject.AddComponent<VoxelCollider>();
            vcAdded.BuildCollider();
        }
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        if (voxel != null && !hadCollider)
        {
            VoxelCollider vc = voxel.gameObject.GetComponent<VoxelCollider>();
            vc.ResetCollidersEditor();
            DestroyImmediate(vc);
        }

        SceneView.duringSceneGui -= OnSceneGUI;
    }

    int currColorIndex = 0;
    bool framed;
    bool drawHelper;
    Vector3 voxelAddPos;
    Vector3 voxelRemovePos;


    void OnSceneGUI(SceneView view)
    {
        if (!framed)
        {
            framed = true;
            view.FrameSelected();
        }

        int id = GUIUtility.GetControlID(FocusType.Passive);
        HandleUtility.AddDefaultControl(id);

        bool needsUpdate = false;

        Camera sceneViewCam = SceneView.lastActiveSceneView.camera;

        Handles.BeginGUI();
        Rect paletteRect = new Rect(16, 16, sceneViewCam.pixelWidth, 32);

        GUILayout.BeginArea(paletteRect);
        EditorGUI.DrawRect(new Rect(24 * currColorIndex, 0, 28, 28), Color.black);
        GUILayout.BeginHorizontal();
        for (int i = 0; i < voxel.VoxelData.Colors.Length; i++)
        {
            Color newCol = EditorGUILayout.ColorField(
                GUIContent.none,
                voxel.VoxelData.Colors[i],
                false,
                true,
                false,
                GUILayout.Width(24),
                GUILayout.Height(24)
                );
            if (newCol != voxel.VoxelData.Colors[i])
            {
                currColorIndex = i;
                voxel.VoxelData.Colors[i] = newCol;
                needsUpdate = true;
            }
        }

        bool addNewColor = GUILayout.Button(
            "+",
            GUILayout.Width(24),
            GUILayout.Height(24)
            );

        GUILayout.EndHorizontal();
        GUILayout.EndArea();
        Handles.EndGUI();

        if (addNewColor)
        {
            Color[] newCols = new Color[voxel.VoxelData.Colors.Length + 1];
            voxel.VoxelData.Colors.CopyTo(newCols, 0);
            newCols[newCols.Length - 1] = Color.white;
            voxel.BuildMesh(new VoxelData(voxel.VoxelData.VoxelPoints, newCols));
        }
        else
        {
            if (needsUpdate)
            {
                voxel.BuildMesh();
            }
        }

        switch (Event.current.GetTypeForControl(id))
        {
            case EventType.MouseMove:
                Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                RaycastHit? hit = HandleUtility.RaySnap(ray) as RaycastHit?;

                drawHelper = hit != null && hit.Value.transform == voxel.transform;

                if (drawHelper)
                {
                    voxelAddPos = hit.Value.point;
                    voxelAddPos += (hit.Value.normal * VoxelBuilder.HVoxelSize);
                    voxelAddPos = new Vector3(
                        Mathf.Round(voxelAddPos.x / VoxelBuilder.VoxelSize) * VoxelBuilder.VoxelSize,
                        Mathf.Round(voxelAddPos.y / VoxelBuilder.VoxelSize) * VoxelBuilder.VoxelSize,
                        Mathf.Round(voxelAddPos.z / VoxelBuilder.VoxelSize) * VoxelBuilder.VoxelSize
                        );

                    voxelRemovePos = hit.Value.point;
                    voxelRemovePos -= (hit.Value.normal * VoxelBuilder.HVoxelSize);
                    voxelRemovePos = new Vector3(
                         Mathf.Round(voxelRemovePos.x / VoxelBuilder.VoxelSize) * VoxelBuilder.VoxelSize,
                         Mathf.Round(voxelRemovePos.y / VoxelBuilder.VoxelSize) * VoxelBuilder.VoxelSize,
                         Mathf.Round(voxelRemovePos.z / VoxelBuilder.VoxelSize) * VoxelBuilder.VoxelSize
                     );
                }
                break;

            case EventType.MouseDown:
                if (!drawHelper)
                {
                    break;
                }

                // LMB
                if (Event.current.button == 0)
                {
                    // Add voxel
                    VoxelPoint[] newVoxels = new VoxelPoint[voxel.VoxelData.VoxelPoints.Length + 1];
                    voxel.VoxelData.VoxelPoints.CopyTo(newVoxels, 0);
                    newVoxels[newVoxels.Length - 1] = new VoxelPoint(voxel.WorldToLocalVoxel(voxelAddPos), currColorIndex);

                    voxel.BuildMesh(new VoxelData(newVoxels, voxel.VoxelData.Colors));
                    if (voxelCol != null)
                    {
                        voxelCol.BuildCollider();
                    }

                    Event.current.Use();
                }

                // RMB
                if (Event.current.button == 1)
                {
                    // Remove voxel
                    // find closest voxel. remove it

                    // get closest point to hit
                    Vector3Int localPos = voxel.WorldToLocalVoxel(voxelRemovePos);
                    int removeIndex = GetClosestVoxelIndexTo(localPos);
                    if (removeIndex < 0)
                    {
                        break;
                    }

                    VoxelPoint[] newVoxels = new VoxelPoint[voxel.VoxelData.VoxelPoints.Length - 1];
                    for (int i = 0, j = 0; i < voxel.VoxelData.VoxelPoints.Length; i++)
                    {
                        if (i != removeIndex)
                        {
                            newVoxels[j] = voxel.VoxelData.VoxelPoints[i];
                            j++;
                        }
                    }
                    voxel.BuildMesh(new VoxelData(newVoxels, voxel.VoxelData.Colors));
                    if (voxelCol != null)
                    {
                        voxelCol.BuildCollider();
                    }

                    Event.current.Use();
                }

                break;
        }
    }

    int GetClosestVoxelIndexTo(Vector3Int v)
    {
        for (int i = 0; i < voxel.VoxelData.VoxelPoints.Length; i++)
        {
            if (Vector3Int.Distance(v, voxel.VoxelData.VoxelPoints[i].Position) < VoxelBuilder.HVoxelSize)
            {
                return i;
            }
        }
        return -1;
    }

    // Update is called once per frame
    void OnDrawGizmos()
    {
        if (drawHelper)
        {
            Gizmos.color = voxel.VoxelData.Colors[currColorIndex];
            Gizmos.DrawCube(voxelAddPos, Vector3.one * VoxelBuilder.VoxelSize);
        }
    }
#endif
}