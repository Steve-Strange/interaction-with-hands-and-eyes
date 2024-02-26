using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class frameManager : MonoBehaviour
{
    public GameObject frame;
    public GameObject fm;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.name  == "rect"){
            frame.GetComponent<frame>().creatRect();
            fm.SetActive(false);
        }
        if (collision.gameObject.name == "circle")
        {
            frame.GetComponent<frame>().createCircle();
            fm.SetActive(false);
        }
        if (collision.gameObject.name == "tri")
        {
            frame.GetComponent<frame>().createTri();
            fm.SetActive(false);
        }
        if (collision.gameObject.name == "pen")
        {
            frame.GetComponent<frame>().createPentagon();
            fm.SetActive(false);
        }
        if (collision.gameObject.name == "para")
        {
            frame.GetComponent<frame>().createPara();
            fm.SetActive(false);
        }
        if (collision.gameObject.name == "cube")
        {
            frame.GetComponent<frame>().createCube();
            fm.SetActive(false);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
