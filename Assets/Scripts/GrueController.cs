using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrueController : MonoBehaviour
{

    public float speedTravel = 1f;
    public float speedRotate = 50f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Z))
        {
            transform.Translate(-Vector3.forward * speedTravel * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector3.forward * speedTravel * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(Vector3.up, -Time.deltaTime * speedRotate);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.up, Time.deltaTime * speedRotate);
        }
    }
}
