using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(VoxelRenderer))]
public class RandomVoxel : MonoBehaviour
{
    [SerializeField] private int voxelCount = 64;
    [SerializeField] private int area = 16;

    private VoxelRenderer rend;
    private Color[] colArr = new Color[] { Color.white };

    VoxelPoint[] vps;

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<VoxelRenderer>();
        vps = new VoxelPoint[voxelCount];
    }

    // Update is called once per frame
    void Update()
    {
        // generate points
        for (int i = 0; i < vps.Length; i++)
        {
            vps[i] = new VoxelPoint(new Vector3Int(
                Random.Range(0, area),
                Random.Range(0, area),
                Random.Range(0, area)), 0);
        }

        rend.BuildMesh(new VoxelData(vps, colArr));
    }
}
