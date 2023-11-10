using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class BigChunk
{
    World world;
    public BigChunkCoord bigCoord;
    public GameObject bigChunkObject;

    public readonly int _bigChunkWidth;
    public readonly int _bigChunkHeight;

    public Chunk[,,] chunks;

    public bool isLoaded = false; // maybe change name of this to something more accurate.

    GameObject QuadParent;




    //TODO: needs a thing to render a face on the side facing a loaded chunk

    public BigChunk(BigChunkCoord coord, World world)
    {
        this.world = world;
        bigCoord = coord;

        chunks = new Chunk[world._bigChunkWidth, world._bigChunkHeight, world._bigChunkWidth];

        bigChunkObject = new GameObject();
        bigChunkObject.transform.SetParent(world.gameObject.transform);
        bigChunkObject.transform.position = new Vector3(
            bigCoord.x * world._bigChunkWidth * world._chunkSize,
            bigCoord.y * world._bigChunkHeight * world._chunkSize,
            bigCoord.z * world._bigChunkWidth * world._chunkSize
            );
        bigChunkObject.transform.name = "BigChunk " + bigCoord.x + ", " + bigCoord.y + ", " + bigCoord.z;


    }

    public void Load()
    {
        GenerateChunks();
        isLoaded = true;
    }

    public void GenerateChunks()
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
    }

    public void ReGenerateQuads()
    {
        GameObject.Destroy(QuadParent);

        if (isLoaded)
        {
            //check each "face" and if there is a unloaded chunk there, create a quad to hide it;

            QuadParent = new GameObject();
            QuadParent.transform.SetParent(this.bigChunkObject.transform);
            QuadParent.transform.position = this.bigChunkObject.transform.position;
            QuadParent.name = "Quads";

            for (int i = 0; i < 6; i++)
            {
                BigChunkCoord coordToCheck = new BigChunkCoord(
                    bigCoord.x + (int)(VoxelData.faceCheckVectors[i].x),
                    bigCoord.y + (int)(VoxelData.faceCheckVectors[i].y),
                    bigCoord.z + (int)(VoxelData.faceCheckVectors[i].z)
                    );

                Debug.Log("might be creating quad");
                // can this be moved into the next thing without
                if (world.IsBigChunkCoordInWorld(coordToCheck) && !world.isBigChunkLoaded(coordToCheck))
                {
                    Debug.Log("creating Quad");
                    GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    quad.transform.SetParent(QuadParent.transform);
                    quad.transform.position = QuadParent.transform.position;

                    //logic to move and scale th thing correctly based on i
                    //how the hell do I do that

                    //


                }
            }
        }
    }

    private void CreateNewChunk(int x, int y, int z)
    {
        chunks[x, y, z] = new Chunk(new ChunkCoord(x, y, z), world, this);
        world.chunksToCreate.Add(chunks[x, y, z]); //TODO: enqueue would probably be better
    }
}
