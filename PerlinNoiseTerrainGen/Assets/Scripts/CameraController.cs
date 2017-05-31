using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    float speed = 100.0f;
    float sprint = 250.0f;
    float maxSpeed = 1000.0f;
    float sensitivity = 0.25f;
    private Vector3 previousMousePosition = new Vector3(255, 255, 255);
    private float totalRun = 1.0f;

    void Update()
    {
        previousMousePosition = Input.mousePosition - previousMousePosition;
        previousMousePosition = new Vector3(-previousMousePosition.y * sensitivity, previousMousePosition.x * sensitivity, 0);
        previousMousePosition = new Vector3(transform.eulerAngles.x + previousMousePosition.x, transform.eulerAngles.y + previousMousePosition.y, 0);
        transform.eulerAngles = previousMousePosition;
        previousMousePosition = Input.mousePosition;

        Vector3 input = GetInput();
        if (Input.GetKey(KeyCode.LeftShift))
        {
            totalRun += Time.deltaTime;
            input = input * totalRun * sprint;
            input.x = Mathf.Clamp(input.x, -maxSpeed, maxSpeed);
            input.y = Mathf.Clamp(input.y, -maxSpeed, maxSpeed);
            input.z = Mathf.Clamp(input.z, -maxSpeed, maxSpeed);
        }
        else
        {
            totalRun = Mathf.Clamp(totalRun * 0.5f, 1, 1000);
            input *= speed;
        }

        input *= Time.deltaTime;
        transform.Translate(input);

    }

    Vector3 GetInput()
    {

        Vector3 p_Velocity = new Vector3(0, 0, 0);

        if (Input.GetKey(KeyCode.W))
        {
            p_Velocity.z += 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            p_Velocity.z -= 1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            p_Velocity.x -= 1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            p_Velocity.x += 1;
        }
        return p_Velocity;
    }


    
}
