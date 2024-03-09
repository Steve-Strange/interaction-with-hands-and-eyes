using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class frameManager : MonoBehaviour
{
    public GameObject frame;
    // public TMP_Text t;
    // public TMP_Text t2;
    public TMP_InputField log;
    public GameObject FinalObjects;
    private bool releaseState = false;
    public GameObject Agent;

    private GameObject currentAgentObject;
    // Start is called before the first frame update

    void Start()
    {

    }

    void Update()
    {
        // t2.text = "ReleaseState: " + releaseState.ToString();

        if(releaseState == true && gameObject.GetComponent<pinch>().ispinch == false){
            releaseState = false;
        }


    }
    public void OnCollisionStay(Collision collision)
    {
        if(gameObject.GetComponent<pinch>().ispinch == true && releaseState == false){
            releaseState = true;
            if(collision.gameObject.name  == "rect"){
                // t.text = "Rect";
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

            if (collision.gameObject.CompareTag("AgentObject"))
            {
                log.text = "name: " + collision.gameObject.name + "\n";
                log.text += "count: " + Agent.GetComponent<GrabAgentObject>().FinishedObjects.Count + "\n";
                if(Agent.GetComponent<GrabAgentObject>().FinishedObjects.Count >= 3){
                    // if(currentAgentObject.TryGetComponent(out Outline outline)){
                    //     outline.OutlineColor = Color.clear;
                    // }
                    
                    
                    currentAgentObject = collision.gameObject;
                    log.text += "currentAgentObject: " + currentAgentObject.name + "\n";
                    // currentAgentObject.GetComponent<Outline>().OutlineColor = Color.red;
                    if(Agent.GetComponent<GrabAgentObject>().MovingObject.Count == 1){
                        Agent.GetComponent<GrabAgentObject>().MovingObject.RemoveAt(0);
                    }
                    Agent.GetComponent<GrabAgentObject>().MovingObject.Add(GameObject.Find(collision.gameObject.name.Replace(" Agent","")));
                    log.text += "MovingObject: " + Agent.GetComponent<GrabAgentObject>().MovingObject[0].name + "\n";
                }
            }
        }
    }

}
