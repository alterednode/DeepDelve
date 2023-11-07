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
    public ChunkCoord(int _x, int _y, int _z)
    {
        x = _x;
        y = _y;
        z = _z;


    }
    public ChunkCoord(int _x, int _z) : //TODO: learn why this is the right way, chatGPT spat this out :)
        this(_x, 0, _z)
    {
    }
}
