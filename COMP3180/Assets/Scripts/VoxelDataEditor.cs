using System.Collections;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class VoxelDataEditor : MonoBehaviour
{
    private VoxelRenderer voxel;

    private Vector3Int addedVoxels;
    private bool hadCollider;

    private Camera sceneCam;

    public void SetVoxel(VoxelRenderer renderer)
    {
        voxel = renderer;

        if (voxel.TryGetComponent(out VoxelCollider vc))
        {
            hadCollider = true;
            Debug.Log("Has collider");
        }
        else
        {
            hadCollider = false;
            VoxelCollider vcAdded = voxel.gameObject.AddComponent<VoxelCollider>();
            vcAdded.RefreshCollider();
        }
    }

    private void OnEnable()
    {
        Camera[] cams = SceneView.GetAllSceneCameras();
        if (cams.Length > 0)
        {
            sceneCam = SceneView.GetAllSceneCameras()[0];
        }

        EditorApplication.update += EditorUpdate;
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

        EditorApplication.update -= EditorUpdate;
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    bool drawHelper;
    Vector3 helperPos;
    private void EditorUpdate()
    {
        if (sceneCam != null)
        {
          /*  Vector3 mousePos = Input.mousePosition;
            mousePos.z = -10f;

            Ray mouseRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

            if (Physics.Raycast(mouseRay, out RaycastHit hit))
            {
                Debug.Log(hit.transform.name);
                drawHelper = (hit.transform == voxel.transform);
                if (drawHelper)
                {
                    helperPos = hit.point - (hit.normal * VoxelBuilder.HVoxelSize);
                }
            }*/
        }
    }


    void OnSceneGUI(SceneView view)
    {
        int id = GUIUtility.GetControlID(FocusType.Passive);
        if (Event.current.GetTypeForControl(id) == EventType.MouseMove)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("Hit object name: " + hit.collider.gameObject.name);
                Handles.CubeHandleCap(id, hit.point, Quaternion.identity, 1, EventType.Ignore);
            }
        }
    }

    // Update is called once per frame
    void OnDrawGizmos()
    {
        if (drawHelper)
        {
            Debug.Log("Drawing");
            Gizmos.DrawCube(helperPos, Vector3.one * VoxelBuilder.VoxelSize);
        }
    }
}