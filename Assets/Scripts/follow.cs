using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class follow : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject o;
    Vector3 gap;
    void Start()
    {
        gap = new Vector3(0.0027f, -0.00039f, -0.008f);
        gap = new Vector3(0.00308f, 0.00111f, -0.00612f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = o.transform.position;
        transform.rotation = o.transform.rotation;
    }
}
