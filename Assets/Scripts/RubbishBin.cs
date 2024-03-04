using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubbishBin : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject Pinch;
    private GameObject HandPoseManager;
    public GameObject FinalObjects;
    private bool releaseFlag;
    void Start()
    {
        HandPoseManager = GameObject.Find("HandPoseManager");
    }

    // Update is called once per frame
    void Update()
    {
        if(Pinch.GetComponent<pinch>().ispinch == false){
            releaseFlag = false;
        }

    }

    public void OnCollisionEnter(Collision collision){
        
    }

    public void OnCollisionStay(Collision collision){
        if(collision.gameObject.name == "pinch" && Pinch.GetComponent<pinch>().ispinch == true && releaseFlag == false){
            releaseFlag = true;
        
            if(FinalObjects.GetComponent<FinalObjects>().finalObj.Count > 0){
                GameObject deleteObj = FinalObjects.GetComponent<FinalObjects>().finalObj[0];
                FinalObjects.GetComponent<FinalObjects>().finalObj.RemoveAt(0);
                FinalObjects.GetComponent<FinalObjects>().RearrangeFinalObj();
                if (HandPoseManager.GetComponent<HandPoseManager>().originalTransform.TryGetValue(deleteObj, out TransformData transformData))
                {
                    deleteObj.transform.parent = null;
                    deleteObj.transform.position = transformData.Position;
                    deleteObj.transform.rotation = transformData.Rotation;
                    deleteObj.transform.localScale = transformData.Scale;
                }
            }
        }
        
    }
        
}

