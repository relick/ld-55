using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraY : MonoBehaviour
{
    public float rotateSpeed = 500.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float yRotation = -Input.GetAxis("Mouse Y");
        yRotation *= rotateSpeed * Time.deltaTime;
        float curAngle = transform.localRotation.eulerAngles.x;
        float newAngle = curAngle + yRotation;
        if(newAngle > 180)
        {
            newAngle -= 360;
        }
        transform.localRotation = Quaternion.Euler(Mathf.Clamp(newAngle, -85, 85), 0, 0);
    }
}
