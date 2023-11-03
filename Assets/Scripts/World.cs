using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{

    public Transform player;
    public Vector3 spawnposition; //set in Start()

    public Material material;
    public BlockType[] blocktypes;

    Chunk[,,] chunks = new Chunk[VoxelData.WorldWidthChunks, VoxelData.WorldHeightChunks, VoxelData.WorldWidthChunks];//REMINDER turn into 3d array to make vertical chunks

    private void Start()
    {
        GenerateStartArea();
        
    }

    public static readonly int ViewDistanceChunks = 5;




    void GenerateStartArea()
    //TODO: update to generate more y layers
    {
        for (int x = 0; x < VoxelData.WorldWidthChunks; x++)
        { //in tutorial this is world size chunks
            for (int z = 0; z < VoxelData.WorldWidthChunks; z++) //here too
            {
                for (int y = 0; y < VoxelData.WorldHeightChunks; y++)
                {
                    CreateNewChunk(x, y, z);
                }
            }

        }
    }

    public byte GetVoxel(Vector3 pos)
    {
        if (!IsVoxelInWorld(pos))
        {
            return 0;
        }



        if (pos.y < 1)
        {
            return 1;
        }
        else if (pos.y == VoxelData.WorldHeightInVoxels - 1)
        {
            float randomFloat = Random.value;
            if (randomFloat < .5)
                return 3; //voxel/block ids are stored as byte (1-255)
            else
                return 4;

        }
        else
            return 2;

    }

    void CreateNewChunk(int x, int y, int z)
    {
        chunks[x, y, z] = new Chunk(new ChunkCoord(x, y, z), this);
    }

    void CreateNewChunk(int x, int z) // for purposes of having code match tutorial better
    {
        CreateNewChunk(x, 0, z);
    }

    bool IsChunkInWorld(ChunkCoord coord)
    {
        if (coord.x > 0 && coord.x < VoxelData.WorldWidthChunks - 1 &&
            coord.y > 0 && coord.y < VoxelData.WorldHeightChunks - 1 &&
            coord.z > 0 && coord.z < VoxelData.WorldWidthChunks - 1)
            return true;
        else
            return false;
    }

    bool IsVoxelInWorld(Vector3 pos)
    {
        if( pos.x >= 0 && pos.x < VoxelData.WorldWidthInVoxels &&
            pos.y >= 0 && pos.y < VoxelData.WorldHeightInVoxels &&
            pos.z >= 0 && pos.z < VoxelData.WorldWidthInVoxels
            )
            return true;
        else 
            return false;
    }

}

[System.Serializable]
public class BlockType
{

    public string blockName;
    public bool isSolid;

    [Header("Texture Values")]
    public int backFaceTexture;
    public int frontFaceTexture;
    public int topFaceTexture;
    public int bottomFaceTexture;
    public int leftFaceTexture;
    public int rightFaceTexture;

    // Back, Front, Top, Bottom, Left, Right

    public int GetTextureID(int faceIndex)
    {

        switch (faceIndex)
        {

            case 0:
                return backFaceTexture;
            case 1:
                return frontFaceTexture;
            case 2:
                return topFaceTexture;
            case 3:
                return bottomFaceTexture;
            case 4:
                return leftFaceTexture;
            case 5:
                return rightFaceTexture;
            default:
                Debug.Log("Error in GetTextureID; invalid face index");
                return 0;


        }

    }
}
