using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

public class World : MonoBehaviour //TODO: turn this into not a monobehaviour once threading done and we can remove the coroutines


{

    public GameObject worldObject;

    /// <summary>
    /// X size of world in BigChunks
    /// </summary>
    public  int _worldXSize;
    /// <summary>
    /// Y size of world in BigChunks
    /// </summary>
    public  int _worldYSize;
    /// <summary>
    /// Z size of world in BigChunks
    /// </summary>
    public  int _worldZSize;

    /// <summary>
    /// Width of BigChunks in Chunks
    /// </summary>
    public  int _bigChunkWidth;
    /// <summary>
    /// Height of BigChunks in Chunks
    /// </summary>
    public  int _bigChunkHeight;

    /// <summary>
    /// Size of axisis in voxels (1m)
    /// </summary>
    public  int _chunkSize;

    public  int _worldXLengthInVoxels;
    public  int _worldYLengthInVoxels;
    public  int _worldZLengthInVoxels;

    /// <summary>
    /// player spawns in this side 
    /// (0,0,0) is the middle of the world
    /// (0,1,0) is the top middle of the 
    /// </summary>
    public Vector3Int _spawnSide = new Vector3Int(0, 0, 0);

    public Vector3 spawnPosition;

    public GenerationType generationType;

    public Material material;
    public Material transparentMaterial;

    public BlockType[] _blocktypes;

    // should this be converted to a dict to allow for literal infinite world?
    // TODO: learn more about datatypes i guess lol
    BigChunk[,,] bigChunks;

    [SerializeField]
    public List<Chunk> chunksToUpdate = new List<Chunk>();
    public bool isUpdatingChunks = false;
    [SerializeField]
    public List<Chunk> chunksToCreate = new List<Chunk>();
    public bool isCreatingChunks = false;

    public bool setup = false;



    
    private void Update()
    {

        //convert
        if (chunksToUpdate.Count > 0 && !isUpdatingChunks)
            StartCoroutine("UpdateChunks");


        // this should be moved to Big Chunk
        if (chunksToCreate.Count > 0 && !isCreatingChunks)
            StartCoroutine("CreateChunks");

    }
    public void Setup(int worldXSize, int worldYSize, int worldZSize, int bigChunkWidth, int BigChunkHeight, int ChunkSize, Vector3Int spawnSide, Material[] materials, BlockType[] blockTypes, GenerationType generationType)
    {
        material = materials[0];
        transparentMaterial = materials[1];

        this.transform.position = Vector3.zero;
        this.name = "World";
        //set all these variables
        _worldXSize = worldXSize;
        _worldYSize = worldYSize;
        _worldZSize = worldZSize;
        _bigChunkWidth = bigChunkWidth;
        _bigChunkHeight = BigChunkHeight;
        _chunkSize = ChunkSize;
        _spawnSide = spawnSide;

        _blocktypes = blockTypes;

        _worldXLengthInVoxels = _chunkSize * _bigChunkWidth * _worldXSize;
        _worldYLengthInVoxels = _chunkSize * _bigChunkHeight * _worldYSize;
        _worldZLengthInVoxels = _chunkSize * _bigChunkWidth * _worldZSize;


        //make a bigChunk array of the right Size
        bigChunks = new BigChunk[
       _worldXSize,
       _worldYSize,
       _worldZSize
        ];

        Debug.Log("starting world"); //ooh boy we starting

        //using the SpawnSide
        spawnPosition = new Vector3(
            ((_worldXLengthInVoxels / 2) + (_worldXLengthInVoxels / 2 * spawnSide.x) - spawnSide.x ),
            ((_worldYLengthInVoxels / 2) + (_worldYLengthInVoxels / 2 * spawnSide.y) - spawnSide.y ),
            ((_worldZLengthInVoxels / 2) + (_worldZLengthInVoxels / 2 * spawnSide.z) - spawnSide.z )
        );

        this.generationType = generationType;

        setup = true;
    }

    public void Init()
    {
        //  kinda suprised this is all this needs
        //  HAHA LOL NVM
        //  and again lol
        //  acutally now its easier than before
        BigChunkCoord spawnBigChunkcoord = GetBigChunkCoordFromVector3(spawnPosition);
        CreateBigChunk(spawnBigChunkcoord.x, spawnBigChunkcoord.y, spawnBigChunkcoord.z);

        LoadBigChunkAtPos(spawnPosition);
    }

    void PreloadNearbyChunks(BigChunkCoord coord)
    {
        for(int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                for ( int z = -1; z <= 1; z++)
                {
                    BigChunkCoord newCoord = new BigChunkCoord(x + coord.x, y + coord.y, z + coord.z);
                    if (IsBigChunkCoordInWorld(newCoord))
                    {
                        if (bigChunks[newCoord.x , newCoord.y , newCoord.z]==null)
                        {
                            CreateBigChunk(newCoord.x , newCoord.y, newCoord.z);
                        }
                    }
                }
            }
        }

    }

    void CreateBigChunk(int x, int y, int z)
    {
        BigChunkCoord coord = new BigChunkCoord(x, y, z);
        bigChunks[x,y,z] = new BigChunk(coord, this);
    }

    public void LoadBigChunkAtPos(Vector3 pos)
    {
        BigChunk bigChunk = GetBigChunkFromVector3(pos);
        bigChunk.Load();
        PreloadNearbyChunks(bigChunk.bigCoord);
        UpdateBigChunkQuads(bigChunk.bigCoord);
    }

    private void UpdateBigChunkQuads(BigChunkCoord bigCoord)
    {
        bigChunks[bigCoord.x, bigCoord.y, bigCoord.z].ReGenerateQuads();
        for(int i = 0; i < 6; i++)
        {
            BigChunkCoord toUpdate = new BigChunkCoord(
                bigCoord.x + (int)(VoxelData.faceCheckVectors[i].x),
                bigCoord.y + (int)(VoxelData.faceCheckVectors[i].y),
                bigCoord.z + (int)(VoxelData.faceCheckVectors[i].z) );
            if (IsBigChunkCoordInWorld(toUpdate))
            {
                bigChunks[toUpdate.x, toUpdate.y, toUpdate.z].ReGenerateQuads();
            }
        }
    }

    public ChunkCoord GetChunkCoordFromVector3(Vector3 pos)
    {
        int x = (Mathf.FloorToInt(pos.x / _chunkSize)) % _bigChunkWidth;
        int y = (Mathf.FloorToInt(pos.y / _chunkSize)) % _bigChunkHeight;
        int z = (Mathf.FloorToInt(pos.z / _chunkSize)) % _bigChunkWidth;

        return new ChunkCoord(x, y, z);
    }

    public BigChunkCoord GetBigChunkCoordFromVector3(Vector3 pos)
    {

        int x = Mathf.FloorToInt(pos.x / (_chunkSize * _bigChunkWidth));
        int y = Mathf.FloorToInt(pos.y / (_chunkSize * _bigChunkHeight));
        int z = Mathf.FloorToInt(pos.z / (_chunkSize * _bigChunkWidth));
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


    //REWRITE: thread this
    /// <summary>
    /// Runs the Init() part of a chunk
    /// </summary>
    /// <returns></returns>
    IEnumerator CreateChunks()
    {
        isCreatingChunks = true;
        while (chunksToCreate.Count > 0)
        {
            chunksToCreate[0].Init();
            chunksToCreate.RemoveAt(0);
            yield return null;
        }

        isCreatingChunks = false;
    }

    //REWRITE: i dunno if this stays here

    /// <summary>
    /// Updates the mesh of a chunk
    /// </summary>
    /// <returns></returns>
    IEnumerator UpdateChunks()
    {
        isUpdatingChunks = true;
        while (chunksToUpdate.Count > 0)
        {
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
    public byte GenerateVoxel(Vector3 pos)
    {

        if (!IsPosInWorld(pos))
        {
            Debug.LogWarning("Tried to generate Voxel not in world at: " + pos.ToString());
            return 0;
        }

        return generationType.GenerateVoxel(pos, this);
    }

    public byte GetVoxel(Vector3 pos)
    {
        if (!IsPosInWorld(pos))
        {//if the pos is not in the world it is air
            return 0;
        }
        if (IsVoxelInLoadedChunk(pos))
        {
            Chunk curChunk = GetChunkFromVector3(pos);
            return curChunk.GetVoxel(pos);
        }
        else
        {
            return GenerateVoxel(pos);
        }
    }




    public bool IsPosInWorld(Vector3 pos)
    {
        if (
            pos.x >= 0
            && pos.x < _worldXLengthInVoxels
            && pos.y >= 0
            && pos.y < _worldYLengthInVoxels
            && pos.z >= 0
            && pos.z < _worldZLengthInVoxels
        )
            return true;
        else
            return false;
    }

    public bool IsBigChunkCoordInWorld(BigChunkCoord coord)
    {
        if (
    coord.x >= 0
    && coord.x < _worldXSize
    && coord.y >= 0
    && coord.y < _worldYSize
    && coord.z >= 0
    && coord.z < _worldZSize
)
            return true;
        else
            return false;
    }

    public bool IsVoxelInLoadedBigChunk(Vector3 pos)
    {
        if (IsPosInWorld(pos))
        {
            BigChunk bigChunk = GetBigChunkFromVector3(pos);
            if(!(bigChunk == null))
            {
                return bigChunk.isLoaded;
            }
        }
            return false;
    }

    public bool isBigChunkLoaded(BigChunkCoord coord)
    {
        if (!(bigChunks[coord.x,coord.y, coord.z] == null))
        {
            return bigChunks[coord.x, coord.y, coord.z].isLoaded;
        }
        else
        {
            return false;
        }
    }

    public bool IsVoxelInLoadedChunk(Vector3 pos)
    {
        if (IsVoxelInLoadedBigChunk(pos))
        {
            Chunk chunk = GetChunkFromVector3(pos);
          if (!(chunk == null))
            {
                return chunk.isVoxelMapPopulated;
            }
        }
            return false;
    }
}
