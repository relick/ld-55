using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class CameraY : MonoBehaviour
{
    public float rotateSpeed = 500.0f;

    private bool paused = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public float yaw = 0.0f;
    private float curAngle = 0.0f;
    public float yLimit = 0.1f;

    // Update is called once per frame
    void Update()
    {
        if(paused)
        {
            return;
        }

        float yRotation = -Input.GetAxis("Mouse Y");
        float newAngle = curAngle;
        if (Mathf.Abs(yRotation) > yLimit)
        {
            yRotation *= rotateSpeed;
            newAngle += yRotation;
        }
        if (newAngle > 180)
        {
            newAngle -= 360;
        }
        transform.rotation = Quaternion.Euler(Mathf.Clamp(newAngle, -85, 85), yaw, 0);
        curAngle = newAngle;
    }
    public void Pause()
    {
        paused = true;
    }

    public void Unpause()
    {
        paused = false;
    }
}
