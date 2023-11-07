using UnityEngine;

public class SelectionRegionHandler : MonoBehaviour
{
    public float extraSize = 0.1f;

    Vector3 startPoint;
    Vector3 endPoint;
    Vector3 playerPosition;

    Vector3 preciseMinPoint;
    Vector3 preciseMaxPoint;


    Vector3 impreciseMaxPoint;
    Vector3 impreciseMinPoint;
                      
    public Vector3 GetPreciseMinPoint() { return preciseMinPoint; }
    public Vector3 GetPreciseMaxPoint() { return preciseMaxPoint; }
    public Vector3 GetImpreciseMinPoint() { return impreciseMaxPoint; }
    public Vector3 GetImpreciseMaxPoint() { return impreciseMinPoint; }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void DealWithPlayerMovement(Vector3 realPosition, Vector3 _playerPosition)
    {
        

        if (!endPoint.Equals(realPosition))
        {
            endPoint = realPosition;

            preciseMinPoint = Vector3.Min(startPoint, endPoint);
            preciseMaxPoint = Vector3.Max(startPoint, endPoint);

            preciseMinPoint = Vector3Int.FloorToInt(preciseMinPoint);
            preciseMaxPoint = Vector3Int.CeilToInt(preciseMaxPoint);

        }
        if(!playerPosition.Equals(_playerPosition))
        {
            playerPosition = _playerPosition;

            impreciseMinPoint = Vector3.Min(startPoint, playerPosition);
            impreciseMaxPoint = Vector3.Max(startPoint, playerPosition);

            impreciseMinPoint = Vector3Int.FloorToInt(impreciseMinPoint);
            impreciseMaxPoint = Vector3Int.CeilToInt(impreciseMaxPoint);
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
        playerPosition = -_startPoint;
        startPoint = _startPoint;
    }


    void MoveAndScale()
    {


        // put the center between the two points
        transform.position = (impreciseMinPoint + impreciseMaxPoint) / 2;
        //sidze is the difference between the two points
        Vector3 size = new Vector3(Mathf.Abs(impreciseMinPoint.x - impreciseMaxPoint.x), Mathf.Abs(impreciseMinPoint.y - impreciseMaxPoint.y), Mathf.Abs(impreciseMinPoint.z - impreciseMaxPoint.z));
        // add a lil bit to the scale to avoid zfighting 
        size += new Vector3(extraSize, extraSize, extraSize);
        // do the thing
        transform.localScale = size;
    }
}
