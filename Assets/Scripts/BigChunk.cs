using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BigChunk 
{
    World world;
    public GameObject bigChunkObject;

    public readonly int _bigChunkWidth;
    public readonly int _bigChunkHeight;

    public Chunk[,,] chunks;

    public bool isLoaded = false; // maybe change name of this to something more accurate.






    //TODO: needs a thing to render a face on the side facing a loaded chunk

    public BigChunk(BigChunkCoord coord, World world)
    {
        this.world = world;

        chunks = new Chunk[world._bigChunkWidth, world._bigChunkHeight, world._bigChunkWidth];

        bigChunkObject = new GameObject();
        bigChunkObject.transform.SetParent(world.gameObject.transform);
        bigChunkObject.transform.position = new Vector3(coord.x * world._bigChunkWidth, coord.y * world._bigChunkHeight, coord.z * world._bigChunkWidth);
        bigChunkObject.transform.name = "BigChunk " + coord.x + ", " + coord.y + ", " + coord.z;


    }

    public void Load()
    {
        for (int y = world._bigChunkHeight - 1; y >= 0; y--)
        {
            for (int x = 0; x < world._bigChunkWidth; x++)
            {
                for (int z = 0; z < world._bigChunkWidth; z++)
                {

                    CreateNewChunk(x, y, z);

                }
            }
        }
        isLoaded = true;
    }

    private void CreateNewChunk(int x, int y, int z)
    {
        chunks[x, y, z] = new Chunk(new ChunkCoord(x, y, z), world, this);
        world.chunksToCreate.Add(chunks[x, y, z]); //TODO: enqueue would probably be better
    }
}
