using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class World : MonoBehaviour
{
    public bool simpleGen = false;
    public bool regenWorld = false;


    public GameObject player;
    public Vector3 spawnPosition; //set in Start()

    public Material material;
    public Material transparentMaterial;
    public BlockType[] blocktypes;


    Chunk[,,] chunks = new Chunk[
        VoxelData.WorldWidthChunks,
        VoxelData.WorldHeightChunks,
        VoxelData.WorldWidthChunks
    ];

    [SerializeField]
    public List<Chunk> chunksToUpdate = new List<Chunk>();
    public bool isUpdatingChunks = false;
    [SerializeField]
    public List<ChunkCoord> chunksToCreate = new List<ChunkCoord>();
    public bool isCreatingChunks = false;

    private void Start()
    {
        Debug.Log("starting start");
        spawnPosition = new Vector3(
            VoxelData.WorldWidthInVoxels / 2,
            VoxelData.WorldHeightInVoxels + 5,
            VoxelData.WorldWidthInVoxels / 2
        );
        GenerateWorld();
        Debug.Log("world gened");
        
    }

    private void Update()
    {


        if (regenWorld)
        {
            regenWorld = false;

            Vector3 oldplayerPos = player.transform.position;
            GenerateWorld();
            player.transform.position = oldplayerPos;
            for (int x = 0; x < VoxelData.WorldWidthChunks - 1; x++)
            {
                for(int y = 0; y < VoxelData.WorldHeightChunks -1; y++)
                {
                    for(int z = 0; z <  VoxelData.WorldWidthChunks - 1; z++)
                    {
                        chunksToUpdate.Add(chunks[x, y, z]);
                    }
                }
            }
        }




        if (chunksToUpdate.Count > 0 && !isUpdatingChunks)

            StartCoroutine("UpdateChunks");



        if (chunksToCreate.Count > 0 && !isCreatingChunks)
            StartCoroutine("CreateChunks");

    }

    public ChunkCoord GetChunkCoordFromVector3(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x / VoxelData.ChunkWidth);
        int y = Mathf.FloorToInt(pos.y / VoxelData.ChunkHeight);
        int z = Mathf.FloorToInt(pos.z / VoxelData.ChunkWidth);

        return new ChunkCoord(x, y, z);
    }

    public Chunk GetChunkFromVector3(Vector3 pos)
    {

        int x = Mathf.FloorToInt(pos.x / VoxelData.ChunkWidth);
        int y = Mathf.FloorToInt(pos.y / VoxelData.ChunkHeight);
        int z = Mathf.FloorToInt(pos.z / VoxelData.ChunkWidth);
        return chunks[x, y, z];

    }
    void GenerateWorld()
    {

        for (int x = 0; x < VoxelData.WorldWidthChunks; x++)
        {
            for (int z = 0; z < VoxelData.WorldWidthChunks; z++)
            {
                for (int y = 0; y < VoxelData.WorldHeightChunks; y++)
                {
                    CreateNewChunk(x, y, z);
                }
            }
        }

        player.transform.position = spawnPosition + new Vector3(.5f, .5f, .5f); //account for offset of player
        player.GetComponent<OnyxBasicPlayerMovement>().realPosition = player.transform.position;
    }


    IEnumerator CreateChunks()
    {
        isCreatingChunks = true;
        while (chunksToCreate.Count > 0)
        {
            chunks[chunksToCreate[0].x, chunksToCreate[0].y, chunksToCreate[0].z].Init();
            chunksToCreate.RemoveAt(0);
            yield return null;
        }

        isCreatingChunks = false;
    }

    IEnumerator UpdateChunks()
    {
        isUpdatingChunks = true;
        while (chunksToUpdate.Count > 0)
        {
            Debug.Log("Updating chunk");
            chunksToUpdate[0].UpdateChunk();
            chunksToUpdate.RemoveAt(0);
            yield return null;
        }

        isUpdatingChunks = false;
    }

    public byte GetVoxel(Vector3 pos)
    {

        if (!IsVoxelInWorld(pos))
        {
            return 0;
        }

        if (simpleGen)
        {
            return 1;
        }


        if (pos.y > VoxelData.WorldHeightInVoxels - VoxelData.ChunkHeight)
        {
            return 0;
        }
        //TODO: find better noise and improve cutoffs for where ores spawn
        float noise = Perlin.Noise(
            pos.x / VoxelData.PosPerlinScaling + 0.5f,
            pos.y / VoxelData.PosPerlinScaling + 0.5f,
            pos.z / VoxelData.PosPerlinScaling + 0.5f
        );

        if (noise > VoxelData.oreTreshold)
        {
            //Debug.Log("ore1");
            return 2;
        }
        else if (noise < -1 * VoxelData.oreTreshold)
        {
            //Debug.Log("ore2");
            return 3;
        }
        else
        {
            return 1;
        }
    }

    void CreateNewChunk(int x, int y, int z)
    {
        chunks[x, y, z] = new Chunk(new ChunkCoord(x, y, z), this);
        chunksToCreate.Add(new ChunkCoord(x, y, z));
    }


    bool IsChunkInWorld(ChunkCoord coord)
    {
        if (
            coord.x > 0
            && coord.x < VoxelData.WorldWidthChunks - 1
            && coord.y > 0
            && coord.y < VoxelData.WorldHeightChunks - 1
            && coord.z > 0
            && coord.z < VoxelData.WorldWidthChunks - 1
        )
            return true;
        else
            return false;
    }

    public bool IsVoxelInWorld(Vector3 pos)
    {
        if (
            pos.x >= 0
            && pos.x < VoxelData.WorldWidthInVoxels
            && pos.y >= 0
            && pos.y < VoxelData.WorldHeightInVoxels
            && pos.z >= 0
            && pos.z < VoxelData.WorldWidthInVoxels
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
    public bool unimportant;
    public bool transparency_I_think;

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
