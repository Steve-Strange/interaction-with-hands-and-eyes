using UnityEngine;
using System.Collections.Generic;
using Unity.XR.PXR;
using UnityEngine.XR;
using TMPro;
using Unity.VisualScripting;


public class HandPoseManager : MonoBehaviour
{
    public TMP_InputField inputField;
    public TMP_Text inputField2;
    public GameObject HandRightWrist;
    public GameObject HandLeft;

    public GameObject EyeTrackingManager;
    private List<GameObject> selectedObjectsFixed = new List<GameObject>();

    public bool SecondSelectionFlag = false;

    void Start()
    {
        EyeTrackingManager = GameObject.Find("EyeTrackingManager");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onPalmPoseStart(){
        selectedObjectsFixed = EyeTrackingManager.GetComponent<EyeTrackingManager>().selectedObjects;
        foreach (var obj in selectedObjectsFixed)
        {
            inputField2.text += obj.name + "  ";
        }
        SecondSelectionFlag = true;

    }

    public void onPalmPoseUpdate()
    {
        inputField.text = HandRightWrist.transform.rotation.eulerAngles.ToString();
    }

    public void onPalmPoseExit(){
        selectedObjectsFixed.Clear();
        SecondSelectionFlag = false;
    }
    
}
