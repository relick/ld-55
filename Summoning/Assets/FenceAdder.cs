using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FenceAdder : MonoBehaviour
{
    public GameObject fenceParent;
    public GameObject fence;
    public GameObject fenceBeams;
    public float radius = 150.0f;

    // Start is called before the first frame update
    void Start()
    {
        float circum = Mathf.PI * radius * 2.0f;
        int numFences = (int)(circum / 1.2f);
        Vector3 firstPos = Vector3.zero;
        Vector3 prevPos = Vector3.zero;
        for(int i = 0; i < numFences; ++i)
        {
            float angle = 2 * Mathf.PI * i / numFences;
            RaycastHit info;
            bool hit = Physics.Raycast(new Ray(transform.position + new Vector3(radius * Mathf.Cos(angle), 512, radius * Mathf.Sin(angle)), Vector3.down), out info);
            if(hit)
            {
                Vector3 pos = info.point - new Vector3(0, 0.2f, 0);
                Quaternion rot = Quaternion.AxisAngle(Vector3.up, -angle);
                if (i > 0)
                {
                    Vector3 beamsPos = (pos + prevPos) / 2.0f;
                    beamsPos.y = Mathf.Min(pos.y, prevPos.y);
                    Instantiate(fenceBeams, beamsPos, rot, fenceParent.transform);
                }
                else
                {
                    firstPos = pos;
                }

                if (i == numFences - 1)
                {
                    Vector3 beamsPos = (pos + firstPos) / 2.0f;
                    beamsPos.y = Mathf.Min(pos.y, firstPos.y);
                    Instantiate(fenceBeams, beamsPos, rot, fenceParent.transform);
                }

                Instantiate(fence, pos, rot, fenceParent.transform);
                prevPos = pos;
            }
        }
    }
}
