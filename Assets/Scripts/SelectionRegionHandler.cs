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
            //this could probably be done better?
            minPoint = Vector3.Min(startPoint, endPoint);
            maxPoint = Vector3.Max(startPoint, endPoint);

            // wanted to use
            minPoint = Vector3Int.FloorToInt(minPoint);
            maxPoint = Vector3Int.CeilToInt(maxPoint);



            SetSize();

        }
    }

    public void SetStartPoint(Vector3 _startPoint)
    {
        endPoint = -_startPoint; // ensure that somehow endPoint isnt in an annoying spot at when CheckIfRealPositionMoved run
        startPoint = _startPoint;
    }

    void SetSize()
    {
        transform.position = (minPoint + maxPoint) / 2;
        Vector3 size = new Vector3(Mathf.Abs(minPoint.x - maxPoint.x), Mathf.Abs(minPoint.y - maxPoint.y), Mathf.Abs(minPoint.z - maxPoint.z));
        size += new Vector3(extraSize, extraSize, extraSize);
        transform.localScale = size;
    }
}
