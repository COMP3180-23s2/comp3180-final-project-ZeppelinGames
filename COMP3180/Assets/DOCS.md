# Script Documentation

## Voxel Point (`Scripts/Voxel/VoxelPoint.cs`, Class)
#### Description
- Stores position of voxel
- Stores colour index mapped to VoxelData's colour palette.
#### Properties
| Properties | Description |
| ------------- | ------------ |  
| LocalPosition (Vector3) | Position of voxel scaled to match global voxel scale |  
| Position (Vector3Int) | Grid aligned position |
| ColorIndex (int) | Index of colour from VoxelData colour palette |  

#### Constructors
| Constructor | Description |
| ----------- | ----------- |
| `public VoxelPoint(Vector3Int v, int cIn)` | Creates VoxelPoint from Vector3Int |
| `public VoxelPoint(int x, int y, int z, int cIn)` | Creates VoxelPoint by populating Vector3Int with x,y,z components | 

#### Methods
| Method | Description |
| ------ | ----------- |
| `public Vector3 WorldPosition(Transform t)` | Converts `Position` to worldspace |

<hr>

## Voxel Parser (`Scripts/Voxel/VoxelParser.cs`, Class)
#### Description
- Reads voxel data files and converts it to relevant data

#### Properties
| Properties | Description |
| ------------- | ------------ |  
| stateKeys (Dictionary<char, LoadState>) | Corresponds character read from file to read state of parser |

#### Methods
| Method | Description |
| ------ | ----------- |
| `public static void Parse(string contents, out Vector3[] verts, out VoxelPoint[] points, out int[] tris, out Vector3[] norms, out Color[] cols, out int[] up, out int[] down, out int[] xp, out int[] xn, out int[] zp, out int[] zn, out int[] dTris` | Loads voxel and mesh data from string `contents` |
| `static VoxelPoint ParseVoxelPoint(float[] data)` | Parses VoxelPoint data from data buffer while parsing file |
| `static Vector3 ParseVector3(float[] data)` | Parses Vector3 data from data buffer while parsing file |
| `static Color ParseColor(float[] data)` | Parses Color data from data buffer while parsing file |
| `static int ParseInt(float[] data)` | Parses Int data from data buffer while parsing file |

<hr>

### Voxel Data (`Scripts/Voxel/VoxelData.cs`, Class)
#### Description
- Contains all the data for generating mesh.
- Stores VoxelPoints, color palette and mappings of voxel positions

#### Properties
| Properties | Description |
| ------------- | ------------ |  
| VoxelMap (Dictionary<Vector3Int, VoxelPoint>) | Maps VoxelPoint positions to Vector3Int key for faster lookup |
| VoxelPositions (Vector3Int[]) | Array of all voxel positions |
| VoxelPoints (VoxelPoint[]) | Array of all VoxelPoints that make up voxel shape |
| Colors (Color[]) | Array of colours used as a colour palette |
| voxelDataFile (TextAsset) | File the VoxelData was loaded from (if any) |

#### Constructors
| Constructor | Description |
| ----------- | ----------- |
| `public VoxelData(TextAsset voxelDataFile)` | Reads and loads VoxelData from file |
| `public VoxelData(VoxelPoint[] points, Color[] colors)` | Reads and loads VoxelData from array of VoxelPoints and colors |

#### Methods
| Method | Description |
| ------ | ----------- |
| `void UpdateMap()` | Updates VoxelMap dictionary after loading data |

<hr>

## Voxel Shape (`Scripts/Voxel/VoxelShape.cs`, Class)
#### Description
- Data for the shape of individual voxels. 

#### Properties
| Properties | Description |
| ------------- | ------------ |  
| Verticies (Vector3[]) | |
| Normals (Vector3[]) | |
| FaceTriangles (int[][]) | |
| voxelShapeFile (TextAsset) | |

#### Constructors
| Constructor | Description |
| ----------- | ----------- |
| `public VoxelShape(TextAsset voxelShapeFile)` | |
| `public VoxelShape(Vector3[] verts, Vector3[] norms, int[][] tris)` | |

<hr>

## Voxel Builder (`Scripts/Voxel/VoxelBuilder.cs`, Class)
#### Description
- Combines VoxelData and VoxelShape to generate meshes. 
- Defines the grid size of the voxels.

#### Properties
| Properties | Description |
| ------------- | ------------ |  
| VoxelSize (float) | |
| HVoxelSize (float) | |

#### Methods
| Method | Description |
| ------ | ----------- |
| `public static Mesh Build(VoxelData voxData, VoxelShape voxShape)` | | 
| `public static void Build(VoxelData voxData, VoxelShape voxShape, out Vector3[] vOut, out int[] tOut, out Vector3[] nOut, out Color[] cOut)` | |
| `public static VoxelRenderer NewRenderer(VoxelPoint[] points, Color[] cols, out Rigidbody rig, Transform transform = null, VoxelRenderer copy = null)` | |

<hr>

## Voxel Renderer (`Scripts/Voxel/VoxelRenderer.cs`, Component)
#### Description
- Main voxel rendering component
- Manages mesh renderer and filter
- Manages voxel subcomponents (VoxelCollider)

#### Properties
| Properties | Description |
| ------------- | ------------ |  
| | |

#### Constructors
| Constructor | Description |
| ----------- | ----------- |

#### Methods
| Method | Description |
| ------ | ----------- |
| | |

<hr>

## Voxel Fracturer (`Scripts/Voxel/VoxelFracturer.cs`, Component)
#### Description
- Handles raycasting to voxels
- Handles defining which voxel to be fractured

#### Properties
| Properties | Description |
| ------------- | ------------ |  
| | |

#### Constructors
| Constructor | Description |
| ----------- | ----------- |

#### Methods
| Method | Description |
| ------ | ----------- |
| | |

<hr>

## Voxel Collider (`Scripts/Voxel/VoxelCollider.cs`, Component)
#### Description
- Generates and updates voxel colliders

#### Properties
| Properties | Description |
| ------------- | ------------ |  
| | |

#### Constructors
| Constructor | Description |
| ----------- | ----------- |

#### Methods
| Method | Description |
| ------ | ----------- |
| | |

<hr>

## Voxel Renderer Editor (`Editor/VoxelRendererEditor.cs`, Editor Tool)
#### Description
- Handles GUI and Editor updates for `VoxelRenderer` component
- Manages creating new voxel data files.
- Manages entering and exiting exit mode
- Defines default Voxel creating GameObject (`CreateVoxel` function)

#### Properties
| Properties | Description |
| ------------- | ------------ |  
| | |

#### Constructors
| Constructor | Description |
| ----------- | ----------- |

#### Methods
| Method | Description |
| ------ | ----------- |
| | |

<hr>

## Voxel Data Editor (`Scripts/VoxelEditor/VoxelDataEditor.cs`, Editor Tool)
#### Description
- Manages in Editor editing of voxel data
- Handles GUI and Gizmos for Scene view Editor tools

#### Properties
| Properties | Description |
| ------------- | ------------ |  
| | |

#### Constructors
| Constructor | Description |
| ----------- | ----------- |

#### Methods
| Method | Description |
| ------ | ----------- |
| | |

<hr>

# Voxel File Format
#### Comment (#)
Creates comments within the file. Not read. 
| Format | Example |
| ------ | ------- |
| `# [COMMENT];` | `# Voxel Data File;` |
#### Voxel Points (p)
Creates voxel point. Makes up basic mesh shape. Requires 4 pieces of data: position (grid aligned) x,y,z and index of colour from Voxel Colour data. Position data should only contain grid aligned points (rounded/evenly spaced).   
| Format | Example |
| ------ | ------- |  
| `p x,y,z,c;` | `p 1,1,1,0;` |

#### Voxel Colours (c)
Creates voxel color. Used to vertex color voxel points. Requires 4 pieces of data: r,g,b,a in 0-255 range.
| Format | Example |
| ------ | ------- |
| `c r,g,b,a;` | `c 255,0,0,255;` |

#### Voxel Shape Vertex (v)
Creates voxel shape vertex. Requires 3 pieces of data: x,y,z.  
| Format | Example |
| ------ | ------- |
| `v x,y,z;` | `v 10.2, 1, -5.1;` |

#### Voxel Shape Normal (n)
Creates voxel shape normal. Requires 3 pieces of data: x,y,z (normalised).  
| Format | Example |
| ------ | ------- |
| `n x,y,z;` | `n 0.5,0.15,1;` |

#### Voxel Shape Triangles (u,d,f,b,l,r)
Creates voxel shape triangle dependant on neighbour voxels. These triangles will be added based on if neighbours exist. Triangles have clockwise winding order (following [Unity spec](https://docs.unity3d.com/2021.2/Documentation/Manual/AnatomyofaMesh.html#index-data)). Letters correspond to direction:  
- U: Up (0,1,0)
- D: Down (0,-1,0)
- F: Forward (1,0,0)
- B: Back (-1,0,0)
- L: Left (0,0,-1)
- R: Right (0,0,1) 
- A: Default (Always applied)

| Format | Example |
| ------ | ------- |
| `u i;j;k;` | `u 0;1;2;` |


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