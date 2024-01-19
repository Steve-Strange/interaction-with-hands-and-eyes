using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class follow : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject o;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = o.transform.position;
        transform.rotation = o.transform.rotation;
            }
}
