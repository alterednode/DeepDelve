using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

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

        string debugText = "The Deep Delve";
        debugText += "\n";
        debugText += frameRate + " fps";
        debugText += "\n";
        debugText += "Soft XYZ: X: " + world.player.transform.position.x + "Y: " + world.player.transform.position.y + "Z: " + world.player.transform.position.z;
        debugText += "\n";
        debugText += "Hard XYZ: X: " + playerScript.realPosition.x + "Y: " + playerScript.realPosition.y + "Z: " + playerScript.realPosition.z;
        debugText += "\n";
        debugText += "Chunk: " + world.GetChunkCoordFromVector3(playerScript.realPosition).ToString();
        debugText += "\n";
        debugText += "Selected Block: ID: " + playerScript.selectedBlockID + " Name: " + world.blocktypes[playerScript.selectedBlockID].blockName;
        debugText += "\n";
        debugText += "/\\ Change with numkeys";



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