using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct VoxelPoint
{
    public Vector3Int Position;
    public int ColorIndex;

    public Vector3 WorldPosition => (Vector3)this.Position * VoxelBuilder.VoxelSize;

    public VoxelPoint(Vector3Int v, int cIn)
    {
        this.Position = v;
        this.ColorIndex = cIn;
    }

    public VoxelPoint(int x, int y, int z, int cIn)
    {
        this.Position = new Vector3Int(x, y, z);
        this.ColorIndex = cIn;
    }
}

public static class VoxParser
{ 
    enum LoadState
    {
        NONE,
        COMMENT,
        VERTEX,
        POINT,
        TRIANGLE,
        NORMAL,
        COLOR,

        DEFAULT_TRIS,
        UP_TRIS,
        DOWN_TRIS,
        XP_TRIS,
        XN_TRIS,
        ZP_TRIS,
        ZN_TRIS,
    };

    static Dictionary<char, LoadState> stateKeys = new Dictionary<char, LoadState>()
    {
        { '#', LoadState.COMMENT },
        { 'v', LoadState.VERTEX },
        { 'p', LoadState.POINT },
        { 't', LoadState.TRIANGLE },
        { 'n', LoadState.NORMAL },
        { 'c', LoadState.COLOR },

        { 'a', LoadState.DEFAULT_TRIS },
        { 'u', LoadState.UP_TRIS },
        { 'd', LoadState.DOWN_TRIS },
        { 'f', LoadState.XP_TRIS },
        { 'b', LoadState.XN_TRIS },
        { 'l', LoadState.ZP_TRIS },
        { 'r', LoadState.ZN_TRIS },
    };
    public static void Parse(
        string contents,
        out Vector3[] verts,
        out VoxelPoint[] points,
        out int[] tris,
        out Vector3[] norms,
        out Color[] cols,
        out int[] up,
        out int[] down,
        out int[] xp,
        out int[] xn,
        out int[] zp,
        out int[] zn,
        out int[] dTris)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<VoxelPoint> voxelPoints = new List<VoxelPoint>();
        List<int> triangles = new List<int>();
        List<Vector3> normals = new List<Vector3>();
        List<Color> colors = new List<Color>();

        List<int> defaultTris = new List<int>();
        List<int> upTris = new List<int>();
        List<int> downTris = new List<int>();
        List<int> xpTris = new List<int>();
        List<int> xnTris = new List<int>();
        List<int> zpTris = new List<int>();
        List<int> znTris = new List<int>();

        float[] data = new float[4];
        int dataIndex = 0;
        string rawData = "";

        int readLine = 1;
        int readChar = 0;
        int lineChar = 0;

        LoadState state = LoadState.NONE;
        LoadState prevState = LoadState.NONE;

        try
        {
            for (int i = 0; i < contents.Length; i++)
            {
                readChar = i;
                lineChar++;
                if (contents[i] == '\n')
                {
                    readLine++;
                    lineChar = 0;
                    continue;
                }
                if (char.IsWhiteSpace(contents[i]))
                {
                    continue;
                }

                if (state == LoadState.COMMENT)
                {
                    if (contents[i] == ';')
                    {
                        state = prevState;
                    }
                    continue;
                }

                if (stateKeys.ContainsKey(contents[i]))
                {
                    prevState = state;
                    state = stateKeys[contents[i]];
                    continue;
                }

                switch (contents[i])
                {
                    case ',':
                        if (!float.TryParse(rawData, out data[dataIndex]))
                        {
                            data[dataIndex] = 0;
                        }
                        dataIndex++;
                        rawData = "";
                        break;

                    case ';':
                        if (!float.TryParse(rawData, out data[dataIndex]))
                        {
                            data[dataIndex] = 0;
                        }

                        switch (state)
                        {
                            case LoadState.POINT:
                                voxelPoints.Add(ParseVoxelPoint(data));
                                break;

                            case LoadState.VERTEX:
                                vertices.Add(ParseVector3(data));
                                break;

                            case LoadState.NORMAL:
                                normals.Add(ParseVector3(data));
                                break;
                            case LoadState.TRIANGLE:
                                triangles.Add(ParseInt(data));
                                break;
                            case LoadState.COLOR:
                                colors.Add(ParseColor(data));
                                break;

                            case LoadState.UP_TRIS:
                                upTris.Add(ParseInt(data));
                                break;
                            case LoadState.DOWN_TRIS:
                                downTris.Add(ParseInt(data));
                                break;
                            case LoadState.XP_TRIS:
                                xpTris.Add(ParseInt(data));
                                break;
                            case LoadState.XN_TRIS:
                                xnTris.Add(ParseInt(data));
                                break;
                            case LoadState.ZP_TRIS:
                                zpTris.Add(ParseInt(data));
                                break;
                            case LoadState.ZN_TRIS:
                                znTris.Add(ParseInt(data));
                                break;
                            case LoadState.DEFAULT_TRIS:
                                defaultTris.Add(ParseInt(data));
                                break;
                        }

                        dataIndex = 0;
                        rawData = "";
                        break;

                    default:
                        rawData += contents[i];
                        break;
                }
            }
        } catch (Exception e)
        {
            Debug.LogError($"Failed voxel parsing at Ln: {readLine}, Ch:{lineChar} {readChar}");
            Debug.Log($"Data Index: {dataIndex}\nRaw Data: {rawData}");
            Debug.LogError(e);
        }

        verts = vertices.ToArray();
        points = voxelPoints.ToArray();
        norms = normals.ToArray();
        tris = triangles.ToArray();
        cols = colors.ToArray();

        up = upTris.ToArray();
        down = downTris.ToArray();
        xp = xpTris.ToArray();
        xn = xnTris.ToArray();
        zp = zpTris.ToArray();
        zn = znTris.ToArray();
        dTris = defaultTris.ToArray();
    }

    static VoxelPoint ParseVoxelPoint(float[] data)
    {
        return new VoxelPoint(new Vector3Int((int)data[0], (int)data[1], (int)data[2]), (int)data[3]);
    }

    static Vector3 ParseVector3(float[] data)
    {
        return new Vector3Int((int)data[0], (int)data[1], (int)data[2]);
    }

    static Color ParseColor(float[] data)
    {
        return new Color(data[0] / 255f, data[1] / 255f, data[2] / 255f, data[3] / 255);
    }

    static int ParseInt(float[] data)
    {
        return (int)data[0];
    }
}
