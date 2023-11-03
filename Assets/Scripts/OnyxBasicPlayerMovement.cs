using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnyxBasicPlayerMovement : MonoBehaviour
{



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            gameObject.transform.position = (transform.position + transform.up);
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            gameObject.transform.position = (transform.position - transform.up);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            transform.Rotate(0, 90, 0);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
           transform.Rotate(0,-90,0);
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            gameObject.transform.position = (transform.position + transform.forward);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            gameObject.transform.position = (transform.position - transform.forward);
        }
    }
}
