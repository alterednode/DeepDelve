using TMPro;
using UnityEngine;

public class DebugScreen : MonoBehaviour
{

    public TMP_Text text;
    public World world;
    float frameRate;
    float timer;
    public GameObject player;
    public OnyxBasicPlayerMovement playerScript;
    GameManager gameManager;
    




    // Start is called before the first frame update
    void Start()
    {

        text = gameObject.GetComponent<TMP_Text>();
    }

    public void SetPlayerAndWorld(GameObject newPlayer, World newWorld)
    {
        player = newPlayer;
        playerScript = player.GetComponent<OnyxBasicPlayerMovement>();

        world = newWorld;
    }

    // Update is called once per frame
    void Update()
    {
        string debugText = "The Deep Delve \nDebug Screen";
        debugText += "\n";
        debugText += frameRate + " fps";
        debugText += "\n";
        debugText += "Resolution: " + Screen.width + "x" + Screen.height;
        debugText += "\n";
        debugText += "Soft XYZ: \nX: " + playerScript.VoxelCoord.x.ToString("n2") + "Y: " + playerScript.VoxelCoord.y.ToString("n2") + "Z: " + playerScript.VoxelCoord.z.ToString("n2");
        debugText += "\n";
        debugText += "Hard XYZ: \nX: " + playerScript.RealVoxelCoord.x + "Y: " + playerScript.RealVoxelCoord.y + "Z: " + playerScript.RealVoxelCoord.z;
        debugText += "\n";
        debugText += "Chunk: " + world.GetChunkCoordFromVector3(playerScript.RealVoxelCoord).ToString();
        debugText += "\n";
        debugText += "BigChunk: " + world.GetBigChunkCoordFromVector3(playerScript.RealVoxelCoord).ToString();
        debugText += "\n";
        debugText += "Selected Block: \nID: " + playerScript.selectedBlockID + " \nName: " + world._blocktypes[playerScript.selectedBlockID].blockName;
        debugText += "\n";
        debugText += "/\\ Change with numkeys";
        debugText += "\n";
        if (world.IsVoxelInLoadedBigChunk(playerScript.RealVoxelCoord))
        {
            debugText += "Player Inside Block:";
            debugText += "\n";
            debugText += "ID: " + world.GetVoxel(playerScript.RealVoxelCoord) + "\nName: " + world._blocktypes[world.GetVoxel(playerScript.RealVoxelCoord)].blockName;
        }
        else
        {
            debugText += "Player not in Loaded BigChunk\n\n";
        }
        debugText += "\n";
        if (world.isCreatingChunks)
        {
            debugText += "Actively Creating Chunks";
        }
        else
        {
            debugText += "Not creating any chunks";
        }
        debugText += "\n";
        if (world.isUpdatingChunks)
        {
            debugText += "Actively Updating Chunks";
        }
        else
        {
            debugText += "Not Updating any chunks";
        }


        text.text = debugText;

        if (timer > 1f)
        {

            frameRate = (int)(1f / Time.unscaledDeltaTime);
            timer = 0;

        }
        else
            timer += Time.deltaTime;
    }
}
