using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float speed = 5.0f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 rotation = transform.eulerAngles;

        if (Input.GetKey(KeyCode.W))
            transform.localPosition += transform.forward * Time.deltaTime * speed;
        
        if (Input.GetKey(KeyCode.S))
            transform.localPosition -= transform.forward * Time.deltaTime * speed;

        if (Input.GetKey(KeyCode.A))
            transform.localPosition -= transform.right * Time.deltaTime * speed;

        if (Input.GetKey(KeyCode.D))
            transform.localPosition += transform.right * Time.deltaTime * speed;

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
            rotation.y += Input.GetAxis("Horizontal") * speed * Time.deltaTime; 

        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.UpArrow))
            rotation.x += Input.GetAxis("Horizontal") * speed * Time.deltaTime;

        transform.eulerAngles = rotation;
    }
}
