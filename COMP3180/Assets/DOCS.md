### VoxelPoint
Stores point voxel should be built around, aligned to grid as well as its colour index.

### VoxelData
Data for full voxel mesh. Contains all the points in the voxel and colours for the mesh

### VoxelShape
Data for the shape of individual voxels. 

### Voxel Builder
Combines VoxelData and VoxelShape to generate meshes. Defines the grid size of the voxels.

### Voxel Manager
Creates Voxel Data, Shape and Builder classes to create mesh to be applied to MeshRenderer

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

## Voxel File Format
#### Comment (#)
Creates comments within the file. Not read.   
**Format:** `# [COMMENT];`   
**Example:** `# Voxel Data File;`

#### Voxel Points (p)
Creates voxel point. Makes up basic mesh shape. Requires 4 pieces of data: position (grid aligned) x,y,z and index of colour from Voxel Colour data. Position data should only contain grid aligned points (rounded/evenly spaced).     
**Format:** `p x,y,z,c;`   
**Example:** `p 1,1,1,0;`

#### Voxel Colours (c)
Creates voxel color. Used to vertex color voxel points. Requires 4 pieces of data: r,g,b,a in 0-255 range.
**Format:** `c r,g,b,a;`   
**Example:** `c 255,0,0,255;`

#### Voxel Shape Vertex (v)
Creates voxel shape vertex. Requires 3 pieces of data: x,y,z.  
**Format:** `v x,y,z;`   
**Example:** `v 10.2, 1, -5.1;`

#### Voxel Shape Normal (n)
Creates voxel shape normal. Requires 3 pieces of data: x,y,z (normalised).  
**Format:** `n x,y,z;`   
**Example:** `n 0.5,0.15,1;`

#### Voxel Shape Triangles (u,d,f,b,l,r)
Creates voxel shape triangle dependant on neighbour voxels. These triangles will be added based on if neighbours exist. Triangles have clockwise winding order (following [Unity spec](https://docs.unity3d.com/2021.2/Documentation/Manual/AnatomyofaMesh.html#index-data)). Letters correspond to direction:  
- U: Up (0,1,0)
- D: Down (0,-1,0)
- F: Forward (1,0,0)
- B: Back (-1,0,0)
- L: Left (0,0,-1)
- R: Right (0,0,1) 
- A: Default (Always applied)

**Format:** `u i;j;k;`   
**Example:** `u 0;1;2;`


### Example VoxelData
```py
# Vox Mesh;
# Start points;
p 0,0,0,0;0,1,0,1;1,1,0,2;0,2,0,1;3,1,0,0;4,5,1,0;

# Start color;
c 255,0,0,255;255,255,0,127;0,255,255,255;127,127,0,255;
```


### Example VoxelShape
```py
# Vox Shape;
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