using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Interactible : MonoBehaviour
{
    public enum Type
    {
        Pickup,
        Book,
        Note,
    };

    public Type type;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Highlight(bool highlight)
    {

    }

    [Multiline]
    public string[] pages;
    public string Page(int i)
    {
        if(i >= pages.Length)
        {
            return "";
        }
        else
        {
            return pages[i];
        }
    }

    public int PageCount() { return pages.Length; }
}
