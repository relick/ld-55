using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Player : MonoBehaviour
{
    public GameSystem gameSystem;
    private bool paused = false;
    private bool wasPaused = false;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        controller = GetComponent<CharacterController>();
        cursorEnabledID = Shader.PropertyToID("_CursorEnabled");
        cursor.SetFloat(cursorEnabledID, 1);
        highlightPropID = Shader.PropertyToID("_CursorHighlight");
        cursor.SetFloat(highlightPropID, 0);
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
        if(paused)
        {
            return;
        }

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

    public CameraY myCam;
    public Material cursor;
    public float reach = 1.0f;
    public GameObject holdParent;
    private Interactible currentHighlight;
    private int highlightPropID = -1;
    private int cursorEnabledID = -1;
    private GameObject holdingObject;
    private Transform holdingPrevParent;
    private Vector3 holdingPrevPos;
    private Quaternion holdingPrevRot;

    void Interaction()
    {
        if(paused)
        {
            return;
        }
        if (wasPaused)
        {
            wasPaused = false;
            return;
        }

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
                    cursor.SetFloat(highlightPropID, 1);

                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        Pickup(info.collider.gameObject, other);
                    }
                }
                else if (currentHighlight != null)
                {
                    currentHighlight.Highlight(false);
                    currentHighlight = null;
                    cursor.SetFloat(highlightPropID, 0);
                }
            }
            else if (currentHighlight != null)
            {
                currentHighlight.Highlight(false);
                currentHighlight = null;
                cursor.SetFloat(highlightPropID, 0);
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

    void Pickup(GameObject obj, Interactible i)
    {
        cursor.SetFloat(highlightPropID, 0);

        if (i.type == Interactible.Type.Book)
        {
            gameSystem.OpenBook(i);
        }
        else
        {

            holdingObject = obj;
            holdingPrevParent = holdingObject.transform.parent;
            holdingPrevPos = holdingObject.transform.localPosition;
            holdingPrevRot = holdingObject.transform.localRotation;
            holdingObject.transform.parent = holdParent.transform;
            holdingObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }
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

    public void Pause()
    {
        Cursor.lockState = CursorLockMode.None;
        cursor.SetFloat(cursorEnabledID, 0);
        paused = true;
        wasPaused = true;
        myCam.Pause();
    }

    public void Unpause()
    {
        Cursor.lockState = CursorLockMode.Locked;
        cursor.SetFloat(cursorEnabledID, 1);
        paused = false;
        myCam.Unpause();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        Interaction();
    }

    public GameObject summonCircle;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == summonCircle)
        {
            gameSystem.RestoreColour();
        }
    }
}
