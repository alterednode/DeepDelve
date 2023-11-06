using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class OnyxBasicPlayerMovement : MonoBehaviour
{
    public World world;
    public float moveSpeed;
    public Vector3 realPosition;
    public GameObject virtualCamera;
    float destinationProx = 1;
    public GameObject selectionRegionIndicator;
    SelectionRegionHandler selectionRegionHandler;

    private void Start()
    {
        selectionRegionHandler = selectionRegionIndicator.GetComponent<SelectionRegionHandler>();
    }



    private void Update()
    {
        HandleSelectionRegionStart();

        HandleMoving();

        FinalizeSelectionRegion();

        //TODO: this desperately needs to be cleaned up lol

        if (Input.GetKeyDown(KeyCode.Return))
        {
            byte newVoxel = 4;
            if (!Input.GetKey(KeyCode.LeftShift) && world.IsVoxelInWorld(realPosition))
            {
                world.GetChunkFromVector3(Vector3Int.FloorToInt(realPosition)).EditVoxel(Vector3Int.FloorToInt(realPosition), newVoxel);

            }
            else
            {
                Vector3 minpoint = selectionRegionHandler.getMinPoint();
                Vector3 maxpoint = selectionRegionHandler.getMaxPoint();

                for (int x = (int)minpoint.x; x < (int)maxpoint.x; x++)
                {
                    for (int y = (int)minpoint.y; y < (int)maxpoint.y; y++)
                    {
                        for (int z = (int)minpoint.z; z < (int)maxpoint.z; z++)
                        {
                            Vector3 blockLocation = new Vector3(x, y, z);
                            if (!world.IsVoxelInWorld(blockLocation))
                            { // dont even bother with voxels out of the world.
                                return;
                            }

                            Chunk curChunk = world.GetChunkFromVector3(blockLocation);
                            curChunk.DirectlySetVoxel(blockLocation, newVoxel);

                            bool chunkInListAlready = false;

                            for (int i = 0; i < world.chunksToUpdate.Count; i++)
                            {
                                if (world.chunksToUpdate[i].HasSameCoord(curChunk))
                                {

                                    chunkInListAlready = true;
                                }
                            }

                            if (!chunkInListAlready)
                            {
                                world.chunksToUpdate.Add(curChunk);
                            }
                            for (int i = 0; i < 6; i++)
                            {
                                if (!curChunk.IsVoxelInChunk(blockLocation + VoxelData.faceCheckVectors[i]))
                                {
                                    if (world.IsVoxelInWorld(blockLocation + VoxelData.faceCheckVectors[i]))
                                    {
                                        Chunk adjChunk = world.GetChunkFromVector3(blockLocation + VoxelData.faceCheckVectors[i]);

                                        chunkInListAlready = false;

                                        for (int j = 0; j < world.chunksToUpdate.Count; j++)
                                        {
                                            if (world.chunksToUpdate[j].HasSameCoord(adjChunk))
                                            {
                                                chunkInListAlready = true;
                                            }
                                        }

                                        if (!chunkInListAlready)
                                        {
                                            world.chunksToUpdate.Add(adjChunk);

                                        }
                                    }
                                }

                            }


                        }

                    }
                }
            }
        }

    }
    /// <summary>
    /// Detect if the player starts making a selection region (presses shift)
    /// </summary>
    void HandleSelectionRegionStart()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            selectionRegionIndicator.SetActive(true);
            selectionRegionHandler.SetStartPoint(realPosition);
        }

    }
    /// <summary>
    /// Call functions from the selection region handler to deal with everything else, after we have dealt with movement of the player
    /// </summary>
    void FinalizeSelectionRegion()
    {

        if (Input.GetKey(KeyCode.LeftShift))
        {
            selectionRegionHandler.CheckIfRealPositionMoved(realPosition);
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            selectionRegionIndicator.SetActive(false);
        }
    }


    void HandleMoving()
    {
        //Debug.Log(destinationProx + ", " + Vector3.Distance(transform.position, realPosition));


        if (destinationProx < 1)
        {
            destinationProx += Time.deltaTime * moveSpeed;
            destinationProx = Mathf.Clamp(destinationProx, 0f, 1f);
            transform.position = Vector3.Lerp(transform.position, realPosition, destinationProx);
        }

        if (destinationProx == 1)
        {
            Vector3 direction = GetNearestHorizontalVector3(virtualCamera.transform);
            GetMovement(direction);
            if (Vector3.Distance(transform.position, realPosition) > 0)
            {
                destinationProx = 0f;
            }
        }
    }


    void GetMovement(Vector3 direction)
    {
        if (Input.GetKey(KeyCode.W))
        {
            realPosition += direction;

        }

        if (Input.GetKey(KeyCode.S))
        {
            realPosition += -direction;

        }

        if (Input.GetKey(KeyCode.A))
        {

            realPosition += Quaternion.Euler(0, -90, 0) * direction;

        }

        if (Input.GetKey(KeyCode.D))
        {

            realPosition += Quaternion.Euler(0, 90, 0) * direction;

        }

        if (Input.GetKey(KeyCode.LeftControl))
        {

            realPosition += -Vector3.up;

        }
        if (Input.GetKey(KeyCode.Space))
        {
            realPosition += Vector3.up;
        }
    }

    Vector3 GetNearestHorizontalVector3(Transform transform)
    {
        float[] dirAngles = new float[4];

        dirAngles[0] = Vector3.Angle(transform.forward, Vector3.forward);
        dirAngles[1] = Vector3.Angle(transform.forward, -Vector3.right);
        dirAngles[2] = Vector3.Angle(transform.forward, Vector3.right);
        dirAngles[3] = Vector3.Angle(transform.forward, -Vector3.forward);

        int pos = 0;
        for (int i = 0; i < dirAngles.Length; i++)
        {
            if (dirAngles[i] < dirAngles[pos])
            {
                pos = i;
            }
        }
        switch (pos)
        {
            case 0:
                return Vector3.forward;
            case 1:
                return -Vector3.right;
            case 2:
                return Vector3.right;
            case 3:
                return -Vector3.forward;
            default:
                return Vector3.zero;
        }
    }

}
