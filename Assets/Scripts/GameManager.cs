using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class GameManager : MonoBehaviour
{
    //TODO: change these to handle switching between worlds
    public GameObject activeWorldObject;
    public World activeWorld;

    public GameObject playerObject;
    public OnyxBasicPlayerMovement playerScript;

    public GameObject debugScreenObject;
    public DebugScreen debugScreen;

    public Material[] materials;

    public BlockType[] activeBlockset;

    public bool STARTDOINSHIT = false;

    /// <summary>
    /// amount of x,y,z big chunks in the world
    /// </summary>
    public Vector3Int worldSize;
    /// <summary>
    /// XZ,Y size of the Big chunks (in chunks)
    /// </summary>
    public Vector2Int bigChunkSize;
    /// <summary>
    /// Size of chunk (in voxels)
    /// </summary>
    public int chunkSize;

    void Start()
    {
        transform.position = Vector3.zero;
        Debug.LogWarning("START START START");
        playerScript = playerObject.GetComponent<OnyxBasicPlayerMovement>();
        debugScreen = debugScreenObject.GetComponent<DebugScreen>();
        debugScreenObject.SetActive(false);
        STARTDOINSHIT = false;
        Debug.LogWarning("STARTDOINSHIT START START");

        int startTime = System.DateTime.Now.ToUniversalTime().Millisecond;
        activeWorld = CreateWorld(worldSize.x, worldSize.y, worldSize.z, bigChunkSize.x, bigChunkSize.y, chunkSize, new Vector3Int(0, 1, 0), materials, activeBlockset, new GenerationDeepDelve());
        Debug.LogWarning("World created In :" + (System.DateTime.Now.ToUniversalTime().Millisecond - startTime) + " Milliseconds");

        playerScript.world = activeWorld;
        debugScreenObject.SetActive(true);
        debugScreen.SetPlayerAndWorld(playerObject, activeWorld);

        Vector3 spawnPositionCorrectedForPlayer = activeWorld.spawnPosition + Vector3.one / 2;


        playerObject.transform.position = spawnPositionCorrectedForPlayer;
        playerScript.realPosition = spawnPositionCorrectedForPlayer;
        Debug.LogWarning("GAME MANAGER STARTUP COMPLETE");

        //temporary values until they get shoved in a config file

    }

    private void Update()
    {
        //Temporary thing for testing purposes
        if (activeWorld.IsPosInWorld(playerScript.RealVoxelCoord))
        {
            if((!activeWorld.IsVoxelInLoadedBigChunk(playerScript.RealVoxelCoord)))
            activeWorld.LoadBigChunk(playerScript.RealVoxelCoord);
        }
    }


    World CreateWorld(int worldXSize, int worldYSize, int worldZSize, int bigChunkWidth, int BigChunkHeight, int ChunkSize, Vector3Int spawnSide, Material[] materials, BlockType[] blockTypes, GenerationType generationType)
    {
        GameObject newWorld = new GameObject();
        World world = newWorld.AddComponent<World>();
        world.Setup(worldXSize, worldYSize, worldZSize, bigChunkWidth, BigChunkHeight, ChunkSize, spawnSide, materials, blockTypes, generationType);
        world.Init();
        Debug.LogWarning("finished creating world");
        return world;
    }

}
