using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class VoxelBuilderGPU
{
    public ComputeShader shader;

    public static float VoxelSize = 0.5f;
    public static float HVoxelSize => VoxelSize * 0.5f;

    // Voxel Data 
    private List<Vector3Int> positionKeys = new List<Vector3Int>();
    private Dictionary<Vector3Int, VoxelPoint> mappedVoxels = new Dictionary<Vector3Int, VoxelPoint>();
    private List<Color> mappedColors = new List<Color>();

    public Mesh Build(VoxelData voxData, VoxelShape voxShape)
    {
        Vector3Int[] points = new Vector3Int[voxData.VoxelPoints.Length];
        for (int i = 0; i < voxData.VoxelPoints.Length; i++)
        {
            points[i] = voxData.VoxelPoints[i].Position;
        }

        int totalSize = (sizeof(float) * 3) * points.Length;
        ComputeBuffer buffer = new ComputeBuffer(points.Length, totalSize);

        buffer.SetData(points);

        shader.SetBuffer(0, "voxels", buffer);
        shader.SetFloat("voxelLength", points.Length);
        shader.Dispatch(0, points.Length / 10, 1, 1);

        buffer.GetData(points);

        // do stuff with returned data

        buffer.Dispose();


        // Copy data
        for (int i = 0; i < voxData.VoxelPoints.Length; i++)
        {
            VoxelPoint p = voxData.VoxelPoints[i];
            if (!mappedVoxels.ContainsKey(p.Position))
            {
                positionKeys.Add(p.Position);
                mappedVoxels.Add(p.Position, p);
            }
        }

        mappedColors.AddRange(voxData.Colors);

        // Build Mesh
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Vector3> norms = new List<Vector3>();
        List<Color> cols = new List<Color>();

        for (int i = 0; i < positionKeys.Count; i++)
        {
            VoxelPoint vox = mappedVoxels[positionKeys[i]];
            FaceDirections fd = new FaceDirections(
                 !mappedVoxels.ContainsKey(vox.Position + new Vector3Int(1, 0, 0)),
                 !mappedVoxels.ContainsKey(vox.Position + new Vector3Int(-1, 0, 0)),
                 !mappedVoxels.ContainsKey(vox.Position + new Vector3Int(0, 1, 0)),
                 !mappedVoxels.ContainsKey(vox.Position + new Vector3Int(0, -1, 0)),
                 !mappedVoxels.ContainsKey(vox.Position + new Vector3Int(0, 0, 1)),
                 !mappedVoxels.ContainsKey(vox.Position + new Vector3Int(0, 0, -1)));

            List<int> voxTris = new List<int>();
            for (int j = 0; j < fd.faces.Length; j++)
            {
                if (fd.faces[j])
                {
                    voxTris.AddRange(voxShape.FaceTriangles[j]);
                }
            }

            // Default tris
            voxTris.AddRange(voxShape.FaceTriangles[6]);

            for (int j = 0; j < voxTris.Count; j++)
            {
                tris.Add(voxTris[j] + verts.Count);
            }

            for (int j = 0; j < voxShape.Vertices.Length; j++)
            {
                verts.Add((Vector3)vox.Position * VoxelSize + (voxShape.Vertices[j] * HVoxelSize));

                Color c = new Color(1, 0, 1, 1);
                if (vox.ColorIndex >= 0 && vox.ColorIndex < mappedColors.Count)
                {
                    c = mappedColors[vox.ColorIndex];
                }
                cols.Add(c);
            }

            norms.AddRange(voxShape.Normals);
        }

        Mesh mesh = new Mesh();
        mesh.SetVertices(verts);
        mesh.SetTriangles(tris, 0);
        mesh.SetNormals(norms);
        mesh.SetColors(cols);

        return mesh;
    }
}