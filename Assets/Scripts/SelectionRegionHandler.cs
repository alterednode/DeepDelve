using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionRegionHandler : MonoBehaviour
{
    Vector3 startPoint;
    Vector3 endPoint;

    Vector3 correctedStartPoint;
    Vector3 correctedEndPoint;

    MeshFilter meshFilter;
    MeshRenderer meshRenderer;


    // Start is called before the first frame update
    void Start()
    {
        meshFilter = gameObject.GetComponent<MeshFilter>();
        meshRenderer = gameObject.GetComponent<MeshRenderer>();

    }


    // Update is called once per frame
    void Update()
    {


            
        
    }

    public void CheckIfRealPositionMoved(Vector3 realPosition)
    {
        if (!endPoint.Equals(realPosition))
        { 
            endPoint = realPosition;

            int xmod = 1;
            int ymod = 1;
            int zmod = 1;

            if (endPoint.x < startPoint.x)
                xmod = -1;
            if(endPoint.y < startPoint.y)
                ymod = -1;
            if(endPoint.z < startPoint.z)
                zmod = -1;

            correctedEndPoint.x = endPoint.x + (xmod * 0.5f);
            correctedEndPoint.y = endPoint.y + (ymod * 0.5f);
            correctedEndPoint.z = endPoint.z + (zmod * 0.5f);

            correctedStartPoint.x = startPoint.x - (xmod * 0.5f);
            correctedStartPoint.y = startPoint.y - (ymod * 0.5f);
            correctedStartPoint.z = startPoint.z - (zmod * 0.5f);







            MakeMesh();

        }
    }

    public void SetStartPoint(Vector3 _startPoint)
    {
        endPoint = -_startPoint; // ensure that somehow endPoint isnt in an annoying spot at when CheckIfRealPositionMoved run
        startPoint = _startPoint;
    }

    void MakeMesh()
    {



    }
}
