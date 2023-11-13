using UnityEngine;
using Cinemachine;

public class OnyxBasicPlayerMovement : MonoBehaviour
{

    public World world;
    public float moveSpeed;
    public Vector3 realPosition;
    public byte selectedBlockID = 0;

    public float cameraMoveSpeed = 300;
    public float zoomSpeed = 70;
    public float minZoom = 1;
    public float maxZoom = 30;
    public GameObject virtualCamera;
    public CinemachineFramingTransposer cinemachineFramingTransposer;
    public CinemachinePOV cinemachinePOV;

    public GameManager gameManager;

    float destinationProx = 1;
    public float destinationProxResetValue = 0.7f;
    public GameObject selectionRegionIndicator;
    SelectionRegionHandler selectionRegionHandler;

    readonly Vector3[] compassVectors = new Vector3[]
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
        if(!gameManager.MainMenu.active){

        AAAASOMEONEPRESSEDANUMBERKEY();

        HandleCamera();

        HandleSelectionRegionStart();

        HandleMoving();

        FinalizeSelectionRegion();

        DoVoxelPlacement();
        }

        CheckOtherInput();
    }

    public Vector3 RealVoxelCoord
    {
        get { return realPosition - new Vector3(0.5f,0.5f,0.5f) ; }
    }
    public Vector3 VoxelCoord
    {
        get { return transform.position - new Vector3(0.5f, 0.5f, 0.5f); }
    }
    private void DoVoxelPlacement()
    {

        //TODO: this desperately needs to be cleaned up lol

        if (Input.GetKey(KeyCode.Return) || Input.GetMouseButton(0)) // dont know if keeping mouse button placement forever
        {

            if (!Input.GetKey(KeyCode.LeftShift) && world.IsPosInWorld(realPosition))
            {
                //place single block because SelecitonRegion not active (shift not 
                world.GetChunkFromVector3(Vector3Int.FloorToInt(realPosition)).EditVoxel(Vector3Int.FloorToInt(realPosition), selectedBlockID);

            }
            else
            {

                // a bunch of this should probabyl be moved into funtions in SelectionRegionHandler idk lol

                // get the smalles x,y,z and largest x,y,z to be corners
                Vector3 minpoint = selectionRegionHandler.GetPreciseMinPoint();
                Vector3 maxpoint = selectionRegionHandler.GetPreciseMaxPoint();


                // for each voxel contained within the min and max point set it to the new blockID (selectedBlockID)
                
                for (int x = (int)minpoint.x; x < (int)maxpoint.x; x++)
                {
                    for (int y = (int)minpoint.y; y < (int)maxpoint.y; y++)
                    {
                        for (int z = (int)minpoint.z; z < (int)maxpoint.z; z++)
                        {

                            Vector3 blockLocation = new Vector3(x, y, z);


                            if (!world.IsPosInWorld(blockLocation))
                            { // dont even bother with voxels out of the world.
                                return;
                            }
                            if (!world.IsVoxelInLoadedBigChunk(blockLocation))
                            {
                                return;
                            }
                            Chunk curChunk = world.GetChunkFromVector3(blockLocation);
                            curChunk.DirectlySetVoxel(blockLocation, selectedBlockID);




                        }

                    }
                }
                


                
                 // This needs to be redone
                 
                 
                minpoint -= Vector3.one;
                maxpoint += Vector3.one + new Vector3(world._chunkSize, world._chunkSize, world._chunkSize);

                for (int x = ((int)minpoint.x); x < (int)maxpoint.x ; x+= world._chunkSize)
                {
                    for (int y = (int)minpoint.y; y < (int)maxpoint.y; y+= world._chunkSize)
                    {
                        for (int z = (int)minpoint.z; z < (int)maxpoint.z; z+= world._chunkSize)
                        {
                           Vector3 location =  new Vector3(x, y, z);

                            if(world.IsVoxelInLoadedBigChunk(location))
                            world.chunksToUpdate.Add(world.GetChunkFromVector3(location));




                        }

                    }
                }
                
            }
        }
    }

    private void AAAASOMEONEPRESSEDANUMBERKEY()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedBlockID = 0;
        }else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedBlockID = 4;
        }else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            selectedBlockID = 1;
        }else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            selectedBlockID = 2;
        }else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            selectedBlockID = 3;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            selectedBlockID = 4;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            selectedBlockID = 5;
        }
    }

    private void CheckOtherInput(){
        if(Input.GetKeyDown(KeyCode.Escape)){
            gameManager.MainMenu.SetActive(!gameManager.MainMenu.active);
        }
    }

    private void HandleCamera()
    {
        //dont require right click to change zoom
        ChangeCameraDistanceFromPlayer();

        if (Input.GetMouseButton(1))
        {
            ChangeCameraRotationSpeedTo(cameraMoveSpeed);
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
            cinemachineFramingTransposer.m_CameraDistance += cinemachineFramingTransposer.m_CameraDistance * Time.deltaTime * zoomSpeed;
            cinemachineFramingTransposer.m_CameraDistance = Mathf.Clamp(cinemachineFramingTransposer.m_CameraDistance, minZoom, maxZoom);
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
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
            selectionRegionHandler.DealWithPlayerMovement(realPosition, transform.position);
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
                destinationProx = destinationProxResetValue;
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
