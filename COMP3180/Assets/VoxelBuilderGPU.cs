using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelBuilderGPU : MonoBehaviour
{
    [SerializeField] private ComputeShader compute;

    private RenderTexture tex;

    private struct VoxelGPU
    {
        public Vector3 position;
        public bool[] outDirs;

        public VoxelGPU(Vector3 position, bool[] outDirs)
        {
            this.position = position;
            this.outDirs = outDirs;
        }
    }

    void Update()
    {
        int amount = (int)(Random.value * 90) + 10;
        VoxelGPU[] data = new VoxelGPU[amount];
        for (int i = 0; i < amount; i++)
        {
            data[i] = new VoxelGPU(
                new Vector3(
                    Random.Range(0, 32),
                    Random.Range(0, 32),
                    Random.Range(0, 32)),
                new bool[6]);
        }

        int posSize = sizeof(float) * 3;
        int dirsSize = sizeof(bool) * 6;

        int totalSize = posSize + dirsSize;

        ComputeBuffer buffer = new ComputeBuffer(data.Length, totalSize);
        compute.SetBuffer(0, "voxs", buffer);
        compute.Dispatch(0, tex.width / 8, tex.height / 8, 1);
    }
}
