using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        controller = GetComponent<CharacterController>();
        defaultSizeID = Shader.PropertyToID("_DefaultSize");
        cursor.SetFloat(defaultSizeID, 0.02f);
    }

    public float acceleration = 5.0f;
    public float airAcceleration = 2.0f;
    public float maxSpeed = 10.0f;
    public float maxRunSpeed = 25.0f;
    public float rotateSpeed = 500.0f;
    public float jumpHeight = 2.0f;
    public float gravity = -9.81f;
    public float groundedTimerThreshold = 0.25f;

    private Vector3 curVel = Vector3.zero;
    private Vector3 curAccel = Vector3.zero;
    private float curYVel = 0.0f;
    private CharacterController controller;
    private float groundedTimer = 0.0f;
    void Movement()
    {
        bool isGrounded = controller.isGrounded;
        if(isGrounded)
        {
            groundedTimer = groundedTimerThreshold;
        }
        else if(groundedTimer > 0.0f)
        {
            groundedTimer -= Time.deltaTime;
            isGrounded = true;
        }

        curAccel = Vector3.zero;
        if (Input.GetKey(KeyCode.W))
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

        bool running = Input.GetKey(KeyCode.LeftShift);

        curVel += curAccel * Time.deltaTime;
        curVel = Vector3.ClampMagnitude(curVel, running ? maxRunSpeed : maxSpeed);

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

    public GameObject myCam;
    public Material cursor;
    public float reach = 1.0f;
    public GameObject holdParent;
    private Interactible currentHighlight;
    private int defaultSizeID = -1;
    private GameObject holdingObject;
    private Transform holdingPrevParent;
    private Vector3 holdingPrevPos;
    private Quaternion holdingPrevRot;

    void Interaction()
    {
        if (holdingObject == null)
        {
            Ray selectionRay = new Ray(myCam.transform.position, myCam.transform.forward);
            RaycastHit info;
            bool hit = Physics.Raycast(selectionRay, out info);
            if (hit && Vector3.Distance(info.point, myCam.transform.position) < reach)
            {
                Interactible other = info.collider.gameObject.GetComponent<Interactible>();
                if (other != null)
                {
                    if (currentHighlight != null)
                    {
                        currentHighlight.Highlight(false);
                    }
                    currentHighlight = other;
                    currentHighlight.Highlight(true);
                    cursor.SetFloat(defaultSizeID, 0.05f);

                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        Pickup(info.collider.gameObject);
                    }
                }
            }
            else if (currentHighlight != null)
            {
                currentHighlight.Highlight(false);
                currentHighlight = null;
                cursor.SetFloat(defaultSizeID, 0.02f);
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Drop();
            }
        }
    }

    void Pickup(GameObject obj)
    {
        holdingObject = obj;
        holdingPrevParent = holdingObject.transform.parent;
        holdingPrevPos = holdingObject.transform.localPosition;
        holdingPrevRot = holdingObject.transform.localRotation;
        holdingObject.transform.parent = holdParent.transform;
        holdingObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }
    
    void Drop()
    {
        if(holdingObject != null)
        {
            holdingObject.transform.parent = holdingPrevParent;
            holdingObject.transform.SetLocalPositionAndRotation(holdingPrevPos, holdingPrevRot);
            holdingObject = null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        Interaction();
    }
}
