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

    public void ChekcIfRealPositionMoved(Vector3 realPosition)
    {
        if (!endPoint.Equals(realPosition))
        {


            endPoint = realPosition;

            MakeMesh();

        }
    }

    public void SetStartPoint(Vector3 _startPoint)
    {
        endPoint = -_startPoint;
        startPoint = _startPoint;
    }

    void MakeMesh()
    {



    }
}
