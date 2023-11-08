using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //TODO: change these to handle switching between worlds
    public GameObject activeWorldObject;
    public World activeWorld;
    
    public GameObject playerObject;
    public OnyxBasicPlayerMovement playerScript;

    public GameObject debugScreenObject;
    public DebugScreen debugScreen;

    public BlockType[] activeBlockset;

    bool STARTDOINSHIT = false;


    void Start()
    {
        playerScript = playerObject.GetComponent<OnyxBasicPlayerMovement>();
        debugScreen = debugScreenObject.GetComponent<DebugScreen>();
        debugScreenObject.SetActive(false);
        STARTDOINSHIT=true;

        //temporary values until they get shoved in a config file

    }

    private void Update()
    {
        if(STARTDOINSHIT)
        {
            activeWorld = CreateWorld(8, 8, 8, 4, 8, 16, new Vector3Int(0, 1, 0), activeBlockset, new GenerationSimple());
            debugScreenObject.SetActive(true);
            debugScreen.SetPlayerAndWorld(playerObject, activeWorld);
            STARTDOINSHIT=false;
            
        }
    }


    World CreateWorld(int worldXSize, int worldYSize, int worldZSize, int bigChunkWidth, int BigChunkHeight, int ChunkSize, Vector3Int spawnSide, BlockType[] blockTypes, GenerationType generationType)
    {
        GameObject newWorld = new GameObject();
        World world = newWorld.AddComponent<World>();
        world.Setup(worldXSize, worldYSize, worldZSize, bigChunkWidth, BigChunkHeight, ChunkSize, spawnSide, blockTypes, generationType);
        world.Init();
        return world;
    }

}
