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

    private MeshFilter meshFilter;

    private void Start()
    {
        meshFilter = GetComponent<MeshFilter>();

        VoxelData voxelData = new VoxelData(voxelDataFile);
        VoxelShape voxelShape = new VoxelShape(voxelShapeFile);
        Mesh mesh = voxelBuilder.Build(voxelData, voxelShape);
        meshFilter.mesh = mesh;
    }

    /* [SerializeField] private TextAsset voxFile;

     [SerializeField] private float voxelSize = 0.25f;

     private List<Vector3Int> positionKeys = new List<Vector3Int>();
     private Dictionary<Vector3Int, Voxel> mappedVoxels = new Dictionary<Vector3Int, Voxel>();
     private List<Color> mappedColors = new List<Color>();

     private MeshFilter meshFilter;

     [ContextMenu("Edit Model")]
     public void EditModel()
     {
         VoxelEditor.ShowVoxelEditor(this);
     }

     private void Start()
     {
         meshFilter = GetComponent<MeshFilter>();
         LoadVoxel();
         BuildVoxel();
     }

     public void RemoveVox(Voxel vox)
     {
         if (mappedVoxels.ContainsKey(vox.LocalPosition))
         {
             mappedVoxels.Remove(vox.LocalPosition);
             BuildVoxel();
         }
     }

     public void UpdateVoxel(Voxel vox)
     {
         // update surrounding voxels
     }

     public void FindGroups()
     {
         HashSet<Vector3Int> check = new HashSet<Vector3Int>();

         List<List<Voxel>> groups = new List<List<Voxel>>();

         // pick random voxel (first one)
         // see what its connected to
         // see what their connected to

         // once complete
         // see if check.Len == all Voxels.Len
         // if not, another group exists.
         // check vox not in checked list


     }

     public void UpdateAllIndividual()
     {
         for (int i = 0; i < positionKeys.Count; i++)
         {
             Voxel v = mappedVoxels[positionKeys[i]];
             if (v.FaceDirs.faces != null && v.FaceDirs.FaceCount == v.FaceDirs.faces.Length)
             {
                 // cube seperate
                 GameObject voxCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                 voxCube.transform.position = v.Position.v3;
                 voxCube.AddComponent<Rigidbody>();

                 if (voxCube.TryGetComponent(out MeshFilter mf))
                 {

                 }

                 mappedVoxels.Remove(v.LocalPosition);
                 positionKeys.Remove(v.LocalPosition);

                 i--;
             }
         }
     }

     private void OnValidate()
     {
         if (meshFilter == null)
         {
             meshFilter = GetComponent<MeshFilter>();
         }

         LoadVoxel();
         BuildVoxel();
     }

     public void BuildVoxelRaw()
     {

     }

     public void BuildVoxel()
     {
         List<Vector3> verts = new List<Vector3>();
         List<int> tris = new List<int>();
         List<Vector3> norms = new List<Vector3>();
         List<Color> cols = new List<Color>();

         for (int i = 0; i < positionKeys.Count; i++)
         {
             Voxel vox = mappedVoxels[positionKeys[i]];

             FaceDirections fd = new FaceDirections(
                  !mappedVoxels.ContainsKey(vox.LocalPosition + new Vector3Int(1, 0, 0)),
                  !mappedVoxels.ContainsKey(vox.LocalPosition + new Vector3Int(-1, 0, 0)),
                  !mappedVoxels.ContainsKey(vox.LocalPosition + new Vector3Int(0, 1, 0)),
                  !mappedVoxels.ContainsKey(vox.LocalPosition + new Vector3Int(0, -1, 0)),
                  !mappedVoxels.ContainsKey(vox.LocalPosition + new Vector3Int(0, 0, 1)),
                  !mappedVoxels.ContainsKey(vox.LocalPosition + new Vector3Int(0, 0, -1)));

             vox.Build(out Vector3[] v, out int[] t, out Vector3[] n, fd);

             for (int j = 0; j < t.Length; j++)
             {
                 tris.Add(t[j] + verts.Count);
             }
             for (int j = 0; j < v.Length; j++)
             {
                 verts.Add(vox.Position + v[j]);

                 Color c = new Color(1, 0, 1, 1);
                 if (vox.ColorIndex >= 0 && vox.ColorIndex < mappedColors.Count)
                 {
                     c = mappedColors[vox.ColorIndex];
                 }
                 cols.Add(c);
             }
             norms.AddRange(n);
         }

         Mesh mesh = new Mesh();
         mesh.SetVertices(verts);
         mesh.SetTriangles(tris, 0);
         mesh.SetNormals(norms);
         mesh.SetColors(cols);

         meshFilter.sharedMesh = mesh;
     }

     public void LoadVoxel()
     {
         positionKeys.Clear();
         mappedVoxels.Clear();
         mappedColors.Clear();

         string contents = voxFile.text;

         bool writeVertex = true;
         int[] data = new int[4];
         int dataIndex = 0;
         string rawData = "";

         bool inComment = false;

         for (int i = 0; i < contents.Length; i++)
         {
             if (inComment)
             {
                 if (contents[i] == ';')
                 {
                     inComment = false;
                 }
                 continue;
             }

             switch (contents[i])
             {
                 case '#':
                     // skip till ;
                     inComment = true;
                     break;
                 case 'v':
                     writeVertex = true;
                     break;

                 case 'c':
                     writeVertex = false;
                     break;

                 case ',':
                     int.TryParse(rawData, out data[dataIndex]);
                     dataIndex++;
                     rawData = "";
                     break;

                 case ';':
                     int.TryParse(rawData, out data[dataIndex]);

                     if (writeVertex)
                     {
                         Vector3Int pos = new Vector3Int(data[0], data[1], data[2]);
                         if (!mappedVoxels.ContainsKey(pos))
                         {
                             positionKeys.Add(pos);
                             mappedVoxels.Add(pos, new Voxel(new Vector3Rounded(pos, voxelSize), pos, data[3]));
                         }
                     }
                     else
                     {
                         mappedColors.Add(new Color(data[0] / 255f, data[1] / 255f, data[2] / 255f, data[3] / 255));
                     }

                     dataIndex = 0;
                     rawData = "";
                     break;

                 default:
                     rawData += contents[i];
                     break;
             }
         }
     }

     private struct GUIButtonEvent
     {
         public string buttonText;
         public Action action;

         public GUIButtonEvent(string buttonText, Action action)
         {
             this.buttonText = buttonText;
             this.action = action;
         }
     }

     GUIButtonEvent[] buttonEvents;
     private void Awake()
     {
         buttonEvents = new GUIButtonEvent[] {
             new GUIButtonEvent("Rebuild", () => { BuildVoxel(); }),
             new GUIButtonEvent("Reload", () => { LoadVoxel(); }),
             new GUIButtonEvent("Update Loose", () =>
             {
                 UpdateAllIndividual();
                 BuildVoxel();
             })
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
                 }
                 y -= 30f;
             }
         }
     }*/
}
