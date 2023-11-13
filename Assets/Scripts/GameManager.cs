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

    public bool gameStarted = false;

    public GameObject MainMenu;

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
        //set the gamemanager to 0,0,0 to avoid possible issues
        transform.position = Vector3.zero;
        // grab the script from the player
        playerScript = playerObject.GetComponent<OnyxBasicPlayerMovement>();
        // grab the debugScene script
        debugScreen = debugScreenObject.GetComponent<DebugScreen>();
        // disable because it gets mad
        debugScreenObject.SetActive(false);

    }





    public void StartGame(){
if(gameStarted){
    return;
}

                Debug.LogWarning("Going to create world");
        //log the time because why not
        int startTime = System.DateTime.Now.ToUniversalTime().Millisecond;
        // create and set the active world
        activeWorld = CreateWorld(worldSize.x, worldSize.y, worldSize.z, bigChunkSize.x, bigChunkSize.y, chunkSize, new Vector3Int(0, 1, 0), materials, activeBlockset, new GenerationDeepDelve());
        Debug.LogWarning("World created In :" + (System.DateTime.Now.ToUniversalTime().Millisecond - startTime) + " Milliseconds");
        // set the world of the playerscript
        playerScript.world = activeWorld;

        //reEnable this annoying little shit
        debugScreenObject.SetActive(true);
        debugScreen.SetPlayerAndWorld(playerObject, activeWorld);

        // is this needed?
        Vector3 spawnPositionCorrectedForPlayer = activeWorld.spawnPosition + Vector3.one / 2;

        // move the player
        playerObject.transform.position = spawnPositionCorrectedForPlayer;
        // also set the position in the script so shit does not get fucked
        playerScript.realPosition = spawnPositionCorrectedForPlayer;
        Debug.LogWarning("GAME MANAGER STARTUP COMPLETE");
        MainMenu.SetActive(false);
        gameStarted = true;
    }

    public void QuitGame(){
        Application.Quit();
    }

    private void Update()
    {
        //Temporary thing for testing purposes
        if (gameStarted && activeWorld.IsPosInWorld(playerScript.RealVoxelCoord))
        {
            if((!activeWorld.IsVoxelInLoadedBigChunk(playerScript.RealVoxelCoord)))
            activeWorld.LoadBigChunkAtPos(playerScript.RealVoxelCoord);
        }
    }


    World CreateWorld(int worldXSize, int worldYSize, int worldZSize, int bigChunkWidth, int BigChunkHeight, int ChunkSize, Vector3Int spawnSide, Material[] materials, BlockType[] blockTypes, GenerationType generationType)
    {
        // Brave new world
        GameObject newWorld = new GameObject();
        // world
        World world = newWorld.AddComponent<World>();
        // setup world with size and shit
        world.Setup(worldXSize, worldYSize, worldZSize, bigChunkWidth, BigChunkHeight, ChunkSize, spawnSide, materials, blockTypes, generationType);
        // initalize it
        world.Init();
        //oog world created
        Debug.LogWarning("finished creating world");
        // definitely not returning the world obj
        return world;
    }

}
