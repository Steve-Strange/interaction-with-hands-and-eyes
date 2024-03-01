using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubbishBin : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject Pinch;
    private GameObject HandPoseManager;
    void Start()
    {
        HandPoseManager = GameObject.Find("HandPoseManager");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCollisionEnter(Collision collision){
        if(collision.gameObject.CompareTag("FinalObject") && Pinch.GetComponent<pinch>().ispinch == true){
            collision.gameObject.GetComponent<Outline>().OutlineColor = Color.red;
        }
    }

    public void OnCollisionStay(Collision collision){
        if(collision.gameObject.CompareTag("FinalObject")){
            if(Pinch.GetComponent<pinch>().ispinch == false){
                collision.gameObject.tag = "Target";

                if (HandPoseManager.GetComponent<HandPoseManager>().originalTransform.TryGetValue(collision.gameObject, out TransformData transformData))
                {
                    collision.gameObject.transform.position = transformData.Position;
                    collision.gameObject.transform.localScale = transformData.Scale;
                    collision.gameObject.GetComponent<Outline>().OutlineColor = Color.clear;
                }
            }
        }
    }
}
