using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class pinch : MonoBehaviour
{
    public GameObject thumb;
    public GameObject index;
    public bool ispinch = false;
    public int agentMovingStatus;
    private bool touchStatus;
    private bool exitStatus;

    // Update is called once per frame
    void Update()
    {
        float f = (thumb.transform.position - index.transform.position).magnitude;
        if (f < 0.015f)
            ispinch = true;
        else
            ispinch = false;

        if(!ispinch && exitStatus)
        {
            agentMovingStatus = 0;
            touchStatus = false;
            exitStatus = false;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if(!touchStatus){
            if(other.gameObject.CompareTag ("ColliderX")){
                agentMovingStatus = 2;
            }
            else if(other.gameObject.CompareTag ("ColliderY")){
                agentMovingStatus = 3;
            }
            else if(other.gameObject.CompareTag ("ColliderZ")){
                agentMovingStatus = 4;
            }
            else if (other.gameObject.name == "AgentObject"){
                agentMovingStatus = 1;
            }
        }
        touchStatus = true;
    }

    public void OnTriggerExit(Collider other){
        if (other.gameObject.name == "AgentObject" || other.gameObject.CompareTag ("ColliderX") || other.gameObject.CompareTag ("ColliderY") || other.gameObject.CompareTag ("ColliderZ")){
            exitStatus = true;
        }
    }
}
