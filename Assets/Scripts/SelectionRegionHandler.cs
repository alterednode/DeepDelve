using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionRegionHandler : MonoBehaviour
{
    public float extraSize = 0.1f;

   Vector3 startPoint;
   Vector3 endPoint;
   
   Vector3 maxPoint;
   Vector3 minPoint;

    public Vector3 getMinPoint() { return minPoint; }
    public Vector3 getMaxPoint() {  return maxPoint; }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void CheckIfRealPositionMoved(Vector3 realPosition)
    {
        if (!endPoint.Equals(realPosition))
        {
            endPoint = realPosition;

            minPoint = Vector3.Min(startPoint, endPoint);
            maxPoint = Vector3.Max(startPoint, endPoint);

            minPoint = Vector3Int.FloorToInt(minPoint);
            maxPoint = Vector3Int.CeilToInt(maxPoint);



            MoveAndScale();

        }
    }

    /// <summary>
    /// Set start point to the position
    /// </summary>
    /// <param name="_startPoint"></param>
    public void SetStartPoint(Vector3 _startPoint)
    {
        endPoint = -_startPoint; // ensure that somehow endPoint isnt in an annoying spot when CheckIfRealPositionMoved run
        startPoint = _startPoint;
    }


    void MoveAndScale()
    {
        // put the center between the two points
        transform.position = (minPoint + maxPoint) / 2;
        //sidze is the difference between the two points
        Vector3 size = new Vector3(Mathf.Abs(minPoint.x - maxPoint.x), Mathf.Abs(minPoint.y - maxPoint.y), Mathf.Abs(minPoint.z - maxPoint.z));
        // add a lil bit to the scale to avoid zfighting 
        size += new Vector3(extraSize, extraSize, extraSize);
        // do the thing
        transform.localScale = size;
    }
}
