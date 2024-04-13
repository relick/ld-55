using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float acceleration = 5.0f;
    public float airAcceleration = 2.0f;
    public float maxSpeed = 5.0f;
    public float rotateSpeed = 500.0f;
    public float jumpHeight = 2.0f;
    public float gravity = -9.81f;

    private Vector3 curVel = Vector3.zero;
    private Vector3 curAccel = Vector3.zero;
    private float curYVel = 0.0f;
    private CharacterController controller;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        bool isGrounded = controller.isGrounded;

        curAccel = Vector3.zero;
        if(Input.GetKey(KeyCode.W))
        {
            curAccel += transform.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            curAccel -= transform.forward;
        }
        if (Input.GetKey(KeyCode.D))
        {
            curAccel += transform.right;
        }
        if (Input.GetKey(KeyCode.A))
        {
            curAccel -= transform.right;
        }

        curAccel.Normalize();

        if (isGrounded)
        {
            if (curAccel == Vector3.zero)
            {
                curAccel = -curVel * acceleration;
            }
            else
            {
                curAccel *= acceleration;
            }
        }
        else
        {
            if (curAccel == Vector3.zero)
            {
                curAccel = -curVel * airAcceleration;
            }
            else
            {
                curAccel *= airAcceleration;
            }
        }

        curVel += curAccel * Time.deltaTime;
        curVel = Vector3.ClampMagnitude(curVel, maxSpeed);

        controller.Move(curVel * Time.deltaTime);

        if (isGrounded)
        {
            if (curYVel < 0.0f)
            {
                curYVel = 0.0f;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                curYVel += Mathf.Sqrt(-2 * gravity * jumpHeight);
            }
        }

        curYVel += gravity * Time.deltaTime;

        controller.Move(new Vector3(0, curYVel * Time.deltaTime, 0));

        float xRotation = Input.GetAxis("Mouse X");
        xRotation *= rotateSpeed * Time.deltaTime;
        transform.Rotate(0, xRotation, 0);
    }
}
