using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pinch : MonoBehaviour
{
    public GameObject thumb;
    public GameObject index;
    public bool ispinch = false;

    // Update is called once per frame
    void Update()
    {
        float f = (thumb.transform.position - index.transform.position).magnitude;
        if (f < 0.02f)
            ispinch = true;
        else
            ispinch = false;


    
    }
}
