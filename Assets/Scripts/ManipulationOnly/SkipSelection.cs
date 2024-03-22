using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class SkipSelection : MonoBehaviour
{

    GameObject HandPoseManager;
    public Transform Objects;
    public TMP_InputField log;
    bool initFlag;
    // Start is called before the first frame update
    void Start()
    {
        Objects = GameObject.Find("Objects").transform;
        HandPoseManager = GameObject.Find("HandPoseManager");
    }

    // Update is called once per frame
    void Update()
    {
        if(!initFlag)
        {
            foreach (Transform obj in Objects)
            {
                if(obj.gameObject.CompareTag("Target") && GameObject.Find(obj.name + " (1)"))
                {
                    log.text += obj.name + " ";
                    gameObject.GetComponent<FinalObjects>().AddFinalObj(obj.gameObject);
                }
            }
            log.text += gameObject.GetComponent<FinalObjects>().finalObj.Count;
            HandPoseManager.GetComponent<HandPoseManager>().ChangePhase(1);
            log.text += HandPoseManager.GetComponent<HandPoseManager>().phase;
            initFlag = true;
        }
    }
}
