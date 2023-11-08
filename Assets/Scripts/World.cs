using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class World : MonoBehaviour
{
    /// <summary>
    /// X size of world in BigChunks
    /// </summary>
    public readonly int _worldXSize;
    /// <summary>
    /// Y size of world in BigChunks
    /// </summary>
    public readonly int _worldYSize;
    /// <summary>
    /// Z size of world in BigChunks
    /// </summary>
    public readonly int _worldZSize;

    /// <summary>
    /// Width of BigChunks in Chunks
    /// </summary>
    public readonly int _bigChunkWidth;
    /// <summary>
    /// Height of BigChunks in Chunks
    /// </summary>
    public readonly int _bigChunkHeight;

    /// <summary>
    /// Size of axisis in voxels (1m)
    /// </summary>
    public readonly int _chunkSize;

    /// <summary>
    /// player spawns in this side 
    /// (0,0,0) is the middle of the world
    /// (0,1,0) is the top middle of the 
    /// </summary>
    public Vector3Int _spawnSide = new Vector3Int(0, 0, 0);

    public Vector3 spawnPosition;

    public GameObject player;

    public Material material;
    public Material transparentMaterial;

    public BlockType[] blocktypes;


    BigChunk[,,] bigChunks;

    [SerializeField]
    public List<Chunk> chunksToUpdate = new List<Chunk>();
    public bool isUpdatingChunks = false;
    [SerializeField]
    public List<ChunkCoord> chunksToCreate = new List<ChunkCoord>();
    public bool isCreatingChunks = false;



    public World(int worldXSize, int worldYSize, int worldZSize, int bigChunkWidth, int BigChunkHeight, int ChunkSize, Vector3Int spawnSide)
    {
        //set all these variables
        _worldXSize = worldXSize;
        _worldYSize = worldYSize;
        _worldZSize = worldZSize;
        _bigChunkWidth = bigChunkWidth;
        _bigChunkHeight = BigChunkHeight;
        _chunkSize = ChunkSize;
        _spawnSide = spawnSide;


        //make a bigChunk array of the right Size
        bigChunks = new BigChunk[
       _worldXSize, 
       _worldYSize,
       _worldZSize
    ];

        Debug.Log("starting start"); //ooh boy we starting

        //using the SpawnSide
        spawnPosition = new Vector3(
            ((_worldXSize / 2) + (_worldXSize * spawnSide.x)),
            ((_worldYSize / 2) + (_worldYSize * spawnSide.y)),
            ((_worldZSize / 2) + (_worldZSize * spawnSide.z))
        );
        
    }



    private void Update()
    {

        //convert
        if (chunksToUpdate.Count > 0 && !isUpdatingChunks)

            StartCoroutine("UpdateChunks");


        // this should be moved to Big Chunk
        if (chunksToCreate.Count > 0 && !isCreatingChunks)
            StartCoroutine("CreateChunks");

    }

 
    public ChunkCoord GetChunkCoordFromVector3(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x / (_chunkSize * _bigChunkWidth));
        int y = Mathf.FloorToInt(pos.y / (_chunkSize * _bigChunkHeight));
        int z = Mathf.FloorToInt(pos.z / (_chunkSize * _bigChunkWidth));
        return new ChunkCoord(x, y, z);
    }

    public BigChunkCoord GetBigChunkCoordFromVector3(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x / VoxelData.ChunkWidth) % _chunkSize;
        int y = Mathf.FloorToInt(pos.y / VoxelData.ChunkHeight) % _chunkSize;
        int z = Mathf.FloorToInt(pos.z / VoxelData.ChunkWidth) % _chunkSize;
        return new BigChunkCoord(x, y, z);
    }
    public BigChunk GetBigChunkFromVector3(Vector3 pos)
    {
        BigChunkCoord bigChunkLocation = GetBigChunkCoordFromVector3(pos);
        return bigChunks[bigChunkLocation.x, bigChunkLocation.y, bigChunkLocation.z];
    }
    public Chunk GetChunkFromVector3(Vector3 pos)
    {
        BigChunk bigChunk = GetBigChunkFromVector3(pos);
        ChunkCoord subBigLocation = GetChunkCoordFromVector3(pos);
        return bigChunk.chunks[subBigLocation.x, subBigLocation.y, subBigLocation.z];
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
            Debug.Log("Updating chunk" + chunksToUpdate[0].coord.ToString());
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


        if (pos.y >= VoxelData.WorldHeightInVoxels - VoxelData.ChunkHeight)
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
            ChunkCoord coordinate = GetChunkCoordFromVector3((Vector3)pos);
            return !(chunks[coordinate.x, coordinate.y, coordinate.z] == null);
        }
        else
        {
            return false;
        }
    }
}
