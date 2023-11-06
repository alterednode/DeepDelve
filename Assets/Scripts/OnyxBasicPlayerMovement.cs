using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Cinemachine;
using UnityEngine.UI;
using Unity.VisualScripting;

public class OnyxBasicPlayerMovement : MonoBehaviour
{

    public World world;
    public float moveSpeed;
    public Vector3 realPosition;


    public float cameraMoveSpeed = 300;
    public float zoomSpeed = 70;
    public float minZoom = 1;
    public float maxZoom = 30;
    public GameObject virtualCamera;
    public CinemachineFramingTransposer cinemachineFramingTransposer;
    public CinemachinePOV cinemachinePOV;



    float destinationProx = 1;
    public GameObject selectionRegionIndicator;
    SelectionRegionHandler selectionRegionHandler;

    Vector3[] compassVectors = new Vector3[]
        {
        Vector3.forward,
        -Vector3.right,
        Vector3.right,
        -Vector3.forward,
        Vector3.forward + -Vector3.right,
        -Vector3.forward + -Vector3.right,
        Vector3.forward + Vector3.right,
        -Vector3.forward + Vector3.right,
        };

    private void Start()
    {
        selectionRegionHandler = selectionRegionIndicator.GetComponent<SelectionRegionHandler>();
        cinemachinePOV = virtualCamera.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachinePOV>();

        cinemachineFramingTransposer = virtualCamera.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineFramingTransposer>();
    }



    private void Update()
    {
        HandleCamera();

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
                // a bunch of this should probabyl be moved into funtions in SelectionRegionHandler idk lol

                // get the smalles x,y,z and largest x,y,z to be corners
                Vector3 minpoint = selectionRegionHandler.getMinPoint();
                Vector3 maxpoint = selectionRegionHandler.getMaxPoint();


                // for each voxel contained within the min and max point set it to the new blockID (newVoxel)

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

                            //I dont know why I had to do it this way, maybe change to list.contains at some point
                            // anyways it makes sure we dont update a chunk twice
                            for (int i = 0; i < world.chunksToUpdate.Count; i++)
                            {
                                if (world.chunksToUpdate[i].HasSameCoord(curChunk))
                                {

                                    chunkInListAlready = true;
                                }
                            }

                            // adds the chunk that the voxel is in to a list of chunks that gets automatically (visually) updated
                            if (!chunkInListAlready)
                            {
                                world.chunksToUpdate.Add(curChunk);
                            }

                            // for each of the six voxels on the faces of the altered voxel, check if they are in a different chunk as the altered voxel
                            for (int i = 0; i < 6; i++)
                            {
                                if (!curChunk.IsVoxelInChunk(blockLocation + VoxelData.faceCheckVectors[i]))
                                {
                                    // if they are, double check that that voxel is in the world
                                    if (world.IsVoxelInWorld(blockLocation + VoxelData.faceCheckVectors[i]))
                                    {
                                        // get the adjacent chunk
                                        Chunk adjChunk = world.GetChunkFromVector3(blockLocation + VoxelData.faceCheckVectors[i]);


                                        // use the same method as before for checking if the chunk is already queued to be updated, if not add it.
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

    private void HandleCamera()
    {

        if (Input.GetMouseButton(1))
        {
            ChangeCameraRotationSpeedTo(cameraMoveSpeed);
            ChangeCameraDistanceFromPlayer();
        }
        else
        {
            ChangeCameraRotationSpeedTo(0);
        }
    }

    private void ChangeCameraDistanceFromPlayer()
    {

        
        
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            Debug.Log("increacing distance");
            cinemachineFramingTransposer.m_CameraDistance += cinemachineFramingTransposer.m_CameraDistance * Time.deltaTime * zoomSpeed;
            cinemachineFramingTransposer.m_CameraDistance = Mathf.Clamp(cinemachineFramingTransposer.m_CameraDistance, minZoom, maxZoom);
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            Debug.Log("decreacing distance");
            cinemachineFramingTransposer.m_CameraDistance -= cinemachineFramingTransposer.m_CameraDistance * Time.deltaTime * zoomSpeed;
            cinemachineFramingTransposer.m_CameraDistance = Mathf.Clamp(cinemachineFramingTransposer.m_CameraDistance, minZoom, maxZoom);
        }
    }

    void ChangeCameraRotationSpeedTo(float speed)
    {
            cinemachinePOV.m_VerticalAxis.m_MaxSpeed = speed;
            cinemachinePOV.m_HorizontalAxis.m_MaxSpeed = speed;
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
        float[] dirAngles = new float[8];

        // Order:  N,W,E,S,NW,SW, NE, SE
        for (int i = 0; i < dirAngles.Length; i++)
        {
            dirAngles[i] = Vector3.Angle(transform.forward, compassVectors[i]);
        }


        int pos = 0;
        for (int i = 0; i < dirAngles.Length; i++)
        {
            if (dirAngles[i] < dirAngles[pos])
            {
                pos = i;
            }
        }

        return compassVectors[pos];
    }

}
