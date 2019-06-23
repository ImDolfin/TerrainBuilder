using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    /// <summary>
    /// Movement speed
    /// </summary>
    public float movementSpeed = 5.0f;
    /// <summary>
    /// rotational speed
    /// </summary>
    public float rotationSpeed = 5.0f;

    // Update is called once per frame
    void Update()
    {
        rotate();
        move();
    }

    /// <summary>
    /// rotates the gameobject around its own axis
    /// </summary>
    private void rotate()
    {
        Vector3 rotation = transform.eulerAngles;
        if (Input.GetKey(KeyCode.LeftArrow))
            rotation.y -= rotationSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.RightArrow))
            rotation.y += rotationSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.UpArrow))
            rotation.x -= rotationSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.DownArrow))
            rotation.x += rotationSpeed * Time.deltaTime;

        transform.eulerAngles = rotation;
    }

    /// <summary>
    /// moves the game object on its axis
    /// </summary>
    private void move()
    {
        if (Input.GetKey(KeyCode.W))
            transform.localPosition += transform.forward * Time.deltaTime * movementSpeed;

        if (Input.GetKey(KeyCode.S))
            transform.localPosition -= transform.forward * Time.deltaTime * movementSpeed;

        if (Input.GetKey(KeyCode.A))
            transform.localPosition -= transform.right * Time.deltaTime * movementSpeed;

        if (Input.GetKey(KeyCode.D))
            transform.localPosition += transform.right * Time.deltaTime * movementSpeed;
    }
}
