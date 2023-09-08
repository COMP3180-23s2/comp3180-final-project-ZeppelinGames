### VoxelPoint
Stores point voxel should be built around, aligned to grid as well as its colour index.

### VoxelData
Data for full voxel mesh. Contains all the points in the voxel and colours for the mesh

### VoxelShape
Data for the shape of individual voxels. 

### Voxel Builder
Combines VoxelData and VoxelShape to generate meshes. Defines the grid size of the voxels.

## Basic Usage
```cs
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
```

### Example VoxelData
```
# Vox Mesh
# Start points;
p0,0,0,0;0,1,0,1;1,1,0,2;0,2,0,1;3,1,0,0;4,5,1,0;

# Start color;
c255,0,0,255;255,255,0,127;0,255,255,255;127,127,0,255;
```

### Example VoxelShape
```
# Vox Shape
# verts;
v
1,1,1; -1,1,1; 1,1,-1; -1,1,-1; 
1,-1,1; -1,-1,1; 1,-1,-1; -1,-1,-1; 
1,1,1; 1,1,-1; 1,-1,1; 1,-1,-1; 
-1,1,1; -1,1,-1; -1,-1,1; -1,-1,-1; 
1,1,1; -1,1,1; 1,-1,1; -1,-1,1; 
1,1,-1; -1,1,-1; 1,-1,-1; -1,-1,-1; 

# norms;
n
0,1,0; 0,1,0; 0,1,0; 0,1,0; 
0,-1,0; 0,-1,0; 0,-1,0; 0,-1,0; 
1,0,0; 1,0,0; 1,0,0; 1,0,0; 
-1,0,0; -1,0,0; -1,0,0; -1,0,0; 
0,0,1; 0,0,1; 0,0,1; 0,0,1; 
0,0,-1; 0,0,-1; 0,0,-1; 0,0,-1; 

# tris;
u 0; 2; 3; 3; 1; 0;
d 7; 6; 4; 4; 5; 7;
f 8; 11; 9; 8; 10; 11;
b 12; 13; 15; 12; 15; 14;
l 19; 18; 16; 17; 19; 16;
r 20; 22; 23; 20; 23; 21;
```