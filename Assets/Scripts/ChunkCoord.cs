using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkCoord
{
    public int x;
    public int y;
    public int z;

    public bool CompareCoord(ChunkCoord coord)
    {
        return (coord.x == x && coord.y == y && coord.z == z);
    }

    public override string ToString()
    {
        return $"{x}, {y}, {z}";
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
    public ChunkCoord(int _x, int _y, int _z)
    {
        x = _x;
        y = _y;
        z = _z;


    }
}


public class BigChunkCoord : ChunkCoord
{
    public BigChunkCoord(int _x, int _y, int _z) : base(_x, _y, _z)
    {
    }
}
