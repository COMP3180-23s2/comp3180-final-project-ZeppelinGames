using System.Collections;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class VoxelDataEditor : MonoBehaviour
{
#if UNITY_EDITOR
    enum VoxelEditorState
    {
        ADDING,
        REMOVING,
        PAINTING
    };

    private VoxelEditorState voxelEditorState = VoxelEditorState.ADDING;

    private VoxelRenderer voxel;
    private VoxelCollider voxelCol;
    private bool hadCollider;

    Vector2 paletteSize = new Vector2(24, 24);
    Vector2 paletteOffset = new Vector2(4, 4);

    int currColorIndex = 0;

    bool framed;
    bool drawHelper;

    Vector3 voxelAddPos;
    Vector3 voxelRemovePos;


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
            //vcAdded.BuildCollider();
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
            //vc.ResetCollidersEditor();
            DestroyImmediate(vc);
        }

        SceneView.duringSceneGui -= OnSceneGUI;
    }

    void OnSceneGUI(SceneView view)
    {
        if (!framed)
        {
            framed = true;
            view.FrameSelected();
        }

        if (voxel == null || voxel.VoxelData == null)
        {
            return;
        }

        Camera sceneViewCam = SceneView.lastActiveSceneView.camera;

        // Setup handles
        int id = GUIUtility.GetControlID(FocusType.Passive);
        HandleUtility.AddDefaultControl(id);

        // Draw GUI
        Handles.BeginGUI();
        GUILayout.BeginVertical();
        DrawTools(sceneViewCam);
        DrawPalette(sceneViewCam, (int)paletteSize.y);
        GUILayout.EndVertical();
        Handles.EndGUI();

        // Manage inputs
        ManageInputs(id);
    }

    void DrawTools(Camera sceneViewCam)
    {
        Rect toolsRect = new Rect(16, 16, sceneViewCam.pixelWidth, 48);

        GUILayout.BeginArea(toolsRect);
        GUILayout.BeginHorizontal();
        bool add = GUILayout.Button(
            "Add"
        );

        bool remove = GUILayout.Button(
            "Remove"
        );

        bool paint = GUILayout.Button(
            "Paint"
        );
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.EndArea();

        if (add)
        {
            // change to add voxel mode
            voxelEditorState = VoxelEditorState.ADDING;
        }

        if (remove)
        {
            // change to remove voxel mode
            voxelEditorState = VoxelEditorState.REMOVING;
        }

        if (paint)
        {
            // change to paint mode
            voxelEditorState = VoxelEditorState.PAINTING;
        }
    }

    void DrawPalette(Camera sceneViewCam, int yOffset = 0)
    {
        bool needsUpdate = false;

        // Draw Palette
        Rect paletteRect = new Rect(16, 16 + yOffset, sceneViewCam.pixelWidth, 48);

        GUILayout.BeginArea(paletteRect);
        EditorGUI.DrawRect(new Rect((currColorIndex * paletteSize.x) + (currColorIndex * 8), 0, 32, 32), Color.black);

        int colorCount = voxel.VoxelData.Colors.Length;
        for (int i = 0; i < colorCount; i++)
        {
            // Draw color field
            Color newCol = EditorGUI.ColorField(
                new Rect((i * paletteSize.x) + (i * 8) + paletteOffset.x, paletteOffset.y, paletteSize.x, paletteSize.y),
                GUIContent.none,
                voxel.VoxelData.Colors[i],
                false,
                true,
                false
            );

            // Change selected colour
            if(GUI.Toggle(
                new Rect((i * paletteSize.x) + (i * 8) + paletteOffset.x + (paletteSize.x / 4f), paletteOffset.y + 24, paletteSize.x, paletteSize.y),
                i == currColorIndex,
                GUIContent.none))
            {
                currColorIndex = i;
            }

            // Update colours
            if (newCol != voxel.VoxelData.Colors[i])
            {
                currColorIndex = i;
                voxel.VoxelData.Colors[i] = newCol;
                needsUpdate = true;
            }
        }

        bool addNewColor = GUI.Button(
            new Rect((colorCount * paletteSize.x) + (colorCount * 8) + paletteOffset.x, paletteOffset.y, paletteSize.x, paletteSize.y),
            "+"
            );

        GUILayout.EndArea();

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
    }

    void ManageInputs(int id)
    {
        Event e = Event.current;
        switch (e.GetTypeForControl(id))
        {
            case EventType.MouseMove:
                UpdateMarker();
                break;

            case EventType.MouseDown:
                if (!drawHelper)
                {
                    break;
                }

                // LMB
                if (e.button == 0)
                {
                    ToolEvent();
                    e.Use();
                }
                break;

            case EventType.MouseDrag:
                UpdateMarker();
                if (e.button == 0)
                {
                    ToolEvent();
                }
                break;
        }
    }

    void ToolEvent()
    {
        Vector3Int newPoint = voxel.WorldToLocalVoxel(voxelAddPos);
        switch (voxelEditorState)
        {
            case VoxelEditorState.ADDING:
                AddVoxel(newPoint);
                break;
            case VoxelEditorState.REMOVING:
                RemoveVoxel(newPoint);
                break;
            case VoxelEditorState.PAINTING:
                PaintVoxel(newPoint);
                break;
        }
    }

    void AddVoxel(Vector3Int newPoint)
    {
        if (HasPoint(newPoint))
        {
            return;
        }

        VoxelPoint[] newVoxels = new VoxelPoint[voxel.VoxelData.VoxelPoints.Length + 1];
        voxel.VoxelData.VoxelPoints.CopyTo(newVoxels, 0);
        newVoxels[newVoxels.Length - 1] = new VoxelPoint(newPoint, currColorIndex);

        voxel.BuildMesh(new VoxelData(newVoxels, voxel.VoxelData.Colors));
        if (voxelCol != null)
        {
            //voxelCol.BuildCollider();
        }
    }

    bool HasPoint(Vector3Int p)
    {
        for (int i = 0; i < voxel.VoxelData.VoxelPoints.Length; i++)
        {
            if (voxel.VoxelData.VoxelPoints[i].Position == p)
            {
                return true;
            }
        }
        return false;
    }

    Vector3 hitpoint = Vector3.zero;
    void UpdateMarker()
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        RaycastHit? hit = HandleUtility.RaySnap(ray) as RaycastHit?;

        drawHelper = hit != null && hit.Value.transform == voxel.transform;

        if (drawHelper)
        {
            hitpoint = hit.Value.point;
            switch (voxelEditorState)
            {
                case VoxelEditorState.ADDING:
                    voxelAddPos = hit.Value.point + (hit.Value.normal * VoxelBuilder.HVoxelSize);
                    break;
                case VoxelEditorState.REMOVING:
                    voxelAddPos = hit.Value.point - (hit.Value.normal * VoxelBuilder.HVoxelSize);
                    break;
                case VoxelEditorState.PAINTING:
                    voxelAddPos = hit.Value.point - (hit.Value.normal * VoxelBuilder.HVoxelSize);
                    break;
            }
            voxelAddPos = voxel.RoundToVoxelPosition(voxelAddPos);
        }
    }

    void RemoveVoxel(Vector3Int newPoint)
    {
        int removeIndex = GetClosestVoxelIndexTo(newPoint);
        if (removeIndex < 0)
        {
            return;
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
           // voxelCol.BuildCollider();
        }
    }

    void PaintVoxel(Vector3Int newPoint)
    {
        int paintIndex = GetClosestVoxelIndexTo(newPoint);
        if (paintIndex < 0)
        {
            return;
        }

        voxel.VoxelData.VoxelPoints[paintIndex].ColorIndex = currColorIndex;
        voxel.BuildMesh();
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
            switch (voxelEditorState)
            {
                case VoxelEditorState.ADDING:
                    Gizmos.color = new Color(0, 1, 0, 0.5f);
                    break;
                case VoxelEditorState.REMOVING:
                    Gizmos.color = new Color(1, 0, 0, 0.5f);
                    break;
                case VoxelEditorState.PAINTING:
                    Gizmos.color = voxel.VoxelData.Colors[currColorIndex];
                    break;
            }
            Gizmos.DrawCube(voxelAddPos, new Vector3(1.1f, 1.1f, 1.1f) * VoxelBuilder.VoxelSize);
            Gizmos.DrawSphere(hitpoint, 0.1f * VoxelBuilder.VoxelSize);
        }
    }
#endif
}