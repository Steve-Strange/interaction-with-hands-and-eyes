using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class frameManager : MonoBehaviour
{
    public GameObject frame;
    public TMP_Text t;
    public TMP_Text t2;
    public GameObject FinalObjects;
    private bool releaseState = false;
    // Start is called before the first frame update

    void Start()
    {
        
    }

    void Update()
    {
        t2.text = "ReleaseState: " + releaseState.ToString();

        if(releaseState == true && gameObject.GetComponent<pinch>().ispinch == false){
            releaseState = false;
        }


    }
    public void OnCollisionStay(Collision collision)
    {
        if(gameObject.GetComponent<pinch>().ispinch == true && releaseState == false){
            releaseState = true;
            if(collision.gameObject.name  == "rect"){
                t.text = "Rect";
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
    }

}
