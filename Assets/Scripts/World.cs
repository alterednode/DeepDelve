using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                for (int y = 0; y < VoxelData.WorldHeightChunks - 1; y++)
                {
                    for (int z = 0; z < VoxelData.WorldWidthChunks - 1; z++)
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

    /// <summary>
    /// Creates new chunks based on the world's width and height
    /// </summary>
    void GenerateWorld()
    {

        int horizMidPoint = VoxelData.WorldWidthChunks / 2;


        for (int y = VoxelData.WorldHeightChunks - 1; y >= VoxelData.WorldHeightChunks - VoxelData.startAreaHeight * 2; y--)
        {
            for (int x = horizMidPoint - VoxelData.startAreaWidth; x < horizMidPoint + VoxelData.startAreaWidth; x++)
            {
                for (int z = horizMidPoint - VoxelData.startAreaWidth; z < horizMidPoint + VoxelData.startAreaWidth; z++)
                {

                    CreateNewChunk(x, y, z);

                }
            }
        }

        player.transform.position = spawnPosition + new Vector3(.5f, .5f, .5f); //account for offset of player
        player.GetComponent<OnyxBasicPlayerMovement>().realPosition = player.transform.position;
    }

    /// <summary>
    /// Runs the Init() part of a chunk
    /// </summary>
    /// <returns></returns>
    IEnumerator CreateChunks()
    {
        isCreatingChunks = true;
        while (chunksToCreate.Count > 0)
        {
            Debug.Log("Initalizing chunk");
            chunks[chunksToCreate[0].x, chunksToCreate[0].y, chunksToCreate[0].z].Init();
            chunksToCreate.RemoveAt(0);
            yield return null;
        }

        isCreatingChunks = false;
    }

    /// <summary>
    /// Updates the mesh of a chunk
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Used for world generation, decides what a voxel is
    /// </summary>
    /// <param name="pos">
    /// Coordinate of voxel to get in Voxels
    /// </param>
    /// <returns>
    /// byte BlockID
    /// </returns>

    //maybe change to choose / generate voxel and add a getVoxel func that will get a voxel from a chunk at a position
    public byte GenerateVoxel(Vector3 pos)
    {

        if (!IsVoxelInWorld(pos))
        {
            Debug.LogWarning("Tried to generate Voxel not in world at: " + pos.ToString());
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

    public byte GetVoxel(Vector3 pos)
    {
        ChunkCoord chunkLocation = GetChunkCoordFromVector3(pos);
        if (chunks[chunkLocation.x, chunkLocation.y, chunkLocation.z] == null)
        {
            return 0;
        }

        Chunk curChunk = GetChunkFromVector3(pos);
        // if (IsVoxelInLoadedChunk(pos))
        if (curChunk.isVoxelMapPopulated)
            return curChunk.GetVoxel(pos);
        else
            return GenerateVoxel(pos);
    }


    /// <summary>
    ///  Adds a new chunk to the world, and adds it to the creation queue
    /// </summary>
    /// <param name="x">
    /// x Coordinate in chunks
    /// </param>
    /// <param name="y">
    /// y Coordinate in chunks
    /// </param>
    /// z Coordinate in chunks
    /// <param name="z"></param>
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

    public bool IsVoxelInLoadedChunk(Vector3 pos)
    {
        if (IsVoxelInWorld(pos))
        {
            return GetChunkFromVector3(pos).isVoxelMapPopulated;
        }
        else
        {
            return false;
        }
    }
}
