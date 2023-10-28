using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ColliderData
{
    public Vector3 position;
    public Vector3 size;

    public ColliderData(Vector3 position, Vector3 size)
    {
        this.position = position;
        this.size = size;
    }
}

[RequireComponent(typeof(VoxelRenderer))]
public class VoxelCollider : MonoBehaviour
{
    public VoxelRenderer Renderer => voxelRenderer;
    private VoxelRenderer voxelRenderer;

    private void Awake()
    {
        voxelRenderer = GetComponent<VoxelRenderer>();
    
    }

    private void Start()
    {
        /*List<ColliderData> colliderDataList = MergeGridPoints(voxelRenderer.VoxelData.VoxelPositions);
        for (int i = 0; i < colliderDataList.Count; i++)
        {
            BoxCollider bc = gameObject.AddComponent<BoxCollider>();
            bc.center = colliderDataList[i].position;
            bc.size = colliderDataList[i].size;
        }*/
    }

    /* private List<BoxCollider> colliders = new List<BoxCollider>();
     private List<int[]> pointsLinks = new List<int[]>();

     private VoxelRenderer voxRenderer;

     public VoxelRenderer Renderer => voxRenderer;

     private void Start()
     {
         BoxCollider[] cols = gameObject.GetComponents<BoxCollider>();
         for (int i = 0; i < cols.Length; i++)
         {
             Destroy(cols[i]);
         }

         voxRenderer = GetComponent<VoxelRenderer>();
         voxRenderer.meshBuildComplete += MeshBuildEnd;
         BuildCollider();
     }

     void MeshBuildEnd()
     {
         BuildCollider();
     }

     public void BuildCollider()
     {
         if (voxRenderer == null)
         {
             voxRenderer = GetComponent<VoxelRenderer>();
         }

         if (voxRenderer.VoxelData == null)
         {
             return;
         }

         VoxelPoint[] points = voxRenderer.VoxelData.VoxelPoints;
         int currPoint = 0;
         if (points.Length > colliders.Count)
         {
             int addAmount = points.Length - colliders.Count;
             for (int i = 0; i < addAmount; i++)
             {
                 BoxCollider c = GetCollider();
                 c.center = points[currPoint].LocalPosition;
                 c.size = new Vector3(0.9f, 0.9f, 0.9f) * VoxelBuilder.VoxelSize;
                 currPoint++;
             }
         }
         else if (points.Length < colliders.Count)
         {
             int remDiff = colliders.Count - points.Length;
             colliders.RemoveRange(0, remDiff);
         }

         for (int i = currPoint; i < points.Length; i++)
         {
             BoxCollider c = colliders[i];
             c.center = points[i].LocalPosition;
             c.size = new Vector3(0.9f, 0.9f, 0.9f) * VoxelBuilder.VoxelSize;

             pointsLinks.Add(new int[] { i });
         }

         *//*
                 for (int i = 0; i < colliders.Count; i++)
                 {
                     DestroyImmediate(colliders[i]);
                 }
                 colliders.Clear();
                 pointsLinks.Clear();

                 BoxCollider[] cols = gameObject.GetComponents<BoxCollider>();
                 for (int i = 0; i < cols.Length; i++)
                 {
                     DestroyImmediate(cols[i]);
                 }*//*
     }

 #if UNITY_EDITOR
     public void ResetCollidersEditor()
     {
         for (int i = 0; i < colliders.Count; i++)
         {
             DestroyImmediate(colliders[i]);
         }
         colliders.Clear();

         BoxCollider[] cols = gameObject.GetComponents<BoxCollider>();
         for (int i = 0; i < cols.Length; i++)
         {
             DestroyImmediate(cols[i]);
         }
     }

     private void OnDestroy()
     {
         if (EditorApplication.isPlaying)
         {
             return;
         }
         ResetCollidersEditor();
     }
 #endif

     BoxCollider GetCollider()
     {
         BoxCollider col = gameObject.AddComponent<BoxCollider>();
         col.hideFlags = HideFlags.HideInInspector;
         colliders.Add(col);
         return col;
     }*/
}
