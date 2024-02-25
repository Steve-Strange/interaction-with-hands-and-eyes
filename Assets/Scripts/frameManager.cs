using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class frameManager : MonoBehaviour
{
    public GameObject frame;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.name  == "rect"){
            frame.GetComponent<frame>().creatRect();
        }
        if (collision.gameObject.name == "circle")
        {
            frame.GetComponent<frame>().createCircle();
        }
        if (collision.gameObject.name == "tri")
        {
            frame.GetComponent<frame>().createTri();
        }
        if (collision.gameObject.name == "pen")
        {
            frame.GetComponent<frame>().createPentagon();
        }
        if (collision.gameObject.name == "para")
        {
            frame.GetComponent<frame>().createPara();
        }
        if (collision.gameObject.name == "cube")
        {
            frame.GetComponent<frame>().createCube();
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
