using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    }

    void HandleSelectionRegionStart()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            selectionRegionIndicator.SetActive(true);
            selectionRegionHandler.SetStartPoint(realPosition);
        }

    }

    void FinalizeSelectionRegion()
    {

        if (Input.GetKey(KeyCode.LeftShift))
        {
            selectionRegionHandler.ChekcIfRealPositionMoved(realPosition);
        }
        if(Input.GetKeyUp(KeyCode.LeftShift)) {
            selectionRegionIndicator.SetActive(false);
        }
    }


    void HandleMoving()
    {
        Debug.Log(destinationProx + ", " + Vector3.Distance(transform.position, realPosition));


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

            realPosiiton += -Vector3.up;

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
