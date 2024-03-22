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
    private Transform Objects;
    public TMP_InputField log;
    bool initFlag;
    // Start is called before the first frame update
    void Start()
    {
        HandPoseManager = GameObject.Find("HandPoseManager");
    }

    // Update is called once per frame
    void Update()
    {
        if(!initFlag)
        {
            foreach (var obj in HandPoseManager.GetComponent<HandPoseManager>().objectsWithTargets)
            {
                gameObject.GetComponent<FinalObjects>().AddFinalObj(obj);
            }
            log.text += gameObject.GetComponent<FinalObjects>().finalObj.Count;
            HandPoseManager.GetComponent<HandPoseManager>().phase = 1;
            HandPoseManager.GetComponent<HandPoseManager>().ChangePhase(1);
            log.text += HandPoseManager.GetComponent<HandPoseManager>().phase;
            initFlag = true;
        }
    }
}
