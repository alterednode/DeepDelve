using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface GenerationType 
{
    byte GenerateVoxel(Vector3 pos, World world);
}

public class GenerationDeepDelve : GenerationType
{
    float perlinNoiseScaling = 9.3f;
    float oreThreshold = 0.7f;

    public byte GenerateVoxel(Vector3 pos, World world)
    {


        if (pos.y >= world._worldYLengthInVoxels - world._chunkSize)
        {
            return 0;
        }
        //TODO: find better noise and improve cutoffs for where ores spawn
        float noise = Perlin.Noise(
            pos.x / perlinNoiseScaling + 0.5f,
            pos.y / perlinNoiseScaling + 0.5f,
            pos.z / perlinNoiseScaling + 0.5f
        );

        if (noise > oreThreshold)
        {
            //Debug.Log("ore1");
            return 2;
        }
        else if (noise < -1 * oreThreshold)
        {
            //Debug.Log("ore2");
            return 3;
        }
        else
        {
            return 1;
        }
    }
}

public class GenerationEmpty : GenerationType
{
    public byte GenerateVoxel(Vector3 pos, World world)
    {
        return 0;
    }
}

public class GenerationSimple : GenerationType
{
    public byte GenerateVoxel(Vector3 pos, World world)
    {
        return 1;
    }
}
