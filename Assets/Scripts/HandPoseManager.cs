using UnityEngine;
using System.Collections.Generic;
using Unity.XR.PXR;
using UnityEngine.XR;
using TMPro;
using Unity.VisualScripting;
using System;


public class HandPoseManager : MonoBehaviour
{
    public GameObject HandRightWrist;
    private GameObject SightCone;
    private GameObject SecondSelectionBG;
    public List<GameObject> selectedRow = new List<GameObject>();
    private List<GameObject> selectedObjectsFixed = new List<GameObject>();

    private Dictionary<GameObject, TransformData> originalTransform = new Dictionary<GameObject, TransformData>();
    public GameObject FinalObjects;
    private GameObject EyeTrackingManager;

    public TMP_InputField Log;

    private float delayTime = 0.5f; // 延迟时间，单位为秒
    private float delayTimer = 0.0f; // 计时器

    public bool SecondSelectionState = false;
    public bool PalmPoseState = false;

    private int rowNum = 0;

    private float maxAngel = 45f;
    private float minAngel = 20f;

    void Start()
    {
        SecondSelectionBG = GameObject.Find("Objects/SecondSelectionBG");
        SightCone = GameObject.Find("SightCone");
        EyeTrackingManager = GameObject.Find("EyeTrackingManager");
    }

    // Update is called once per frame
    void Update()
    {
        Log.text = "SecondSelectionState: " + SecondSelectionState.ToString() + "\n" + "PalmPoseState: " + PalmPoseState.ToString() + "\n";
        if(!PalmPoseState){
            delayTimer += Time.deltaTime;
            if (delayTimer > delayTime && SecondSelectionState)
            {
                onPalmPoseExitDelay();
            }
        }
        
    }

    public void onPalmPoseStart()
    {
        PalmPoseState = true;
        delayTimer = 0.0f;

        if(!SecondSelectionState){
            originalTransform.Clear();
            selectedObjectsFixed = SightCone.GetComponent<SightCone>().selectedObjects;
            int i = 0;
            SecondSelectionBG.transform.position = new Vector3(0, 0.7f, 2.2f);
            foreach (var obj in selectedObjectsFixed)
            {
                if(obj == EyeTrackingManager.GetComponent<EyeTrackingManager>().blinkSelectedObject) continue;
                originalTransform[obj] = new TransformData(obj.transform.position, obj.transform.localScale);
                obj.GetComponent<Outline>().OutlineColor = Color.clear; 
                obj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                obj.transform.position = SecondSelectionBG.transform.position + 
                    new Vector3(- SecondSelectionBG.transform.localScale.z/2, + SecondSelectionBG.transform.localScale.y/2, 0) + 
                    new Vector3(obj.transform.localScale.x * (2 * (i%5) + 1) , - obj.transform.localScale.y * (2 * (i/5) + 1), - 2 * obj.transform.localScale.z);
                i++;
            }
            rowNum = Mathf.CeilToInt(i/5);
        }
        
        SecondSelectionState = true;

    }

    public void onPalmPoseUpdate()
    {
        if(!SecondSelectionState) return;
        float wristRotation = HandRightWrist.transform.rotation.eulerAngles.x;
        if(wristRotation > 180f){
            wristRotation -= 360f;
            wristRotation = - wristRotation;
        }
        
        int currentRow = Mathf.RoundToInt(rowNum - (wristRotation - minAngel)/(maxAngel - minAngel) * rowNum);

        Log.text = "";
        Log.text +="wristRotation: " + wristRotation.ToString() + "\n";
        Log.text +="currentRow: " + currentRow.ToString() + "\n";
        Log.text +="rowNum: " + rowNum.ToString() + "\n";
        selectedRow.Clear();
        for(int i = 0; i < selectedObjectsFixed.Count; i++){
            if(i/5 == currentRow){
                selectedObjectsFixed[i].GetComponent<Outline>().OutlineColor = Color.yellow; 
                selectedRow.Add(selectedObjectsFixed[i]);
            }
            else{
                selectedObjectsFixed[i].GetComponent<Outline>().OutlineColor = Color.clear; 
            }
        }
    }

    public void onPalmPoseExit()
    {
        PalmPoseState = false;
    }

    public void onPalmPoseExitDelay()
    {
        foreach (var obj in selectedObjectsFixed)
        {
            if (originalTransform.TryGetValue(obj, out TransformData transformData) && !FinalObjects.GetComponent<FinalObjects>().finalObj.Contains(obj))
            {
                obj.transform.position = transformData.Position;
                obj.transform.localScale = transformData.Scale;
            }
        }

        SecondSelectionBG.transform.position = new Vector3(0, -3f, 2.2f);
        delayTimer = 0.0f;
        selectedObjectsFixed.Clear();
        foreach (var obj in SightCone.GetComponent<SightCone>().selectedObjects)
        {
            obj.GetComponent<Outline>().OutlineColor = Color.clear;
        }
        SightCone.GetComponent<SightCone>().selectedObjects.Clear();

        SecondSelectionState = false;
    }
}
