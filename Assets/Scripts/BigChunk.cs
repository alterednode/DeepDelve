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

            Vector3 centerOfBigChunk = new Vector3(
                        world._bigChunkWidth * world._chunkSize / 2,
                        world._bigChunkHeight * world._chunkSize / 2,
                        world._bigChunkWidth * world._chunkSize / 2
                        );

            for (int i = 0; i < 6; i++)
            {
                BigChunkCoord coordToCheck = new BigChunkCoord(
                    bigCoord.x + (int)(VoxelData.faceCheckVectors[i].x),
                    bigCoord.y + (int)(VoxelData.faceCheckVectors[i].y),
                    bigCoord.z + (int)(VoxelData.faceCheckVectors[i].z)
                    );

                // can this be moved into the next thing without
                if (world.IsBigChunkCoordInWorld(coordToCheck) && !world.isBigChunkLoaded(coordToCheck))
                {
                    //this works pretty good, a few parts might want to be changed

                    GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    quad.transform.SetParent(QuadParent.transform);

                   
                    
                    float newX = centerOfBigChunk.x * VoxelData.faceCheckVectors[i].x;
                    float newY = centerOfBigChunk.y * VoxelData.faceCheckVectors[i].y;
                    float newZ = centerOfBigChunk.z * VoxelData.faceCheckVectors[i].z;

                    quad.transform.position = centerOfBigChunk + new Vector3(newX, newY, newZ) + bigChunkObject.transform.position;

                    quad.transform.LookAt(quad.transform.position + VoxelData.faceCheckVectors[i]);
                    float width = world._bigChunkWidth * world._chunkSize;
                    float height = world._bigChunkHeight * world._chunkSize;

                    Vector2 scale;
                    // if the quad is not on top or on the bottom of the thing
                    if ((VoxelData.faceCheckVectors[i]).y == 0 ){
                        scale = new Vector2(width, height);
                    }
                    else
                    {
                        scale = new Vector2(width, width);
                    }



                    quad.transform.localScale = new Vector3(scale.x, scale.y, 1);





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
