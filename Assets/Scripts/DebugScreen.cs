using TMPro;
using UnityEngine;

public class DebugScreen : MonoBehaviour
{

    public TMP_Text text;
    public World world;
    float frameRate;
    float timer;
    OnyxBasicPlayerMovement playerScript;
    




    // Start is called before the first frame update
    void Start()
    {
        text = gameObject.GetComponent<TMP_Text>();
        playerScript = world.player.GetComponent<OnyxBasicPlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {

        string debugText = "The Deep Delve \nDebug Screen";
        debugText += "\n";
        debugText += frameRate + " fps";
        debugText += "\n";
        debugText += "Soft XYZ: \nX: " + world.player.transform.position.x + "Y: " + world.player.transform.position.y + "Z: " + world.player.transform.position.z;
        debugText += "\n";
        debugText += "Hard XYZ: \nX: " + playerScript.realPosition.x + "Y: " + playerScript.realPosition.y + "Z: " + playerScript.realPosition.z;
        debugText += "\n";
        debugText += "Chunk: " + world.GetChunkCoordFromVector3(playerScript.realPosition).ToString();
        debugText += "\n";
        debugText += "Selected Block: \nID: " + playerScript.selectedBlockID + " \nName: " + world.blocktypes[playerScript.selectedBlockID].blockName;
        debugText += "\n";
        debugText += "/\\ Change with numkeys";
        debugText += "\n";
        if (world.IsVoxelInWorld(playerScript.realPosition))
        {
            debugText += "Player Inside Block:";
            debugText += "\n";
            debugText += "ID: " + world.GetVoxel(playerScript.realPosition) + "\nName: " + world.blocktypes[world.GetVoxel(playerScript.realPosition)].blockName;
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
