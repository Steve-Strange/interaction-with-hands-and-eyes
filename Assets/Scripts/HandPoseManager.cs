using UnityEngine;
using System.Collections.Generic;
using Unity.XR.PXR;
using UnityEngine.XR;
using TMPro;
using Unity.VisualScripting;
using System;
using System.Linq;
using UnityEngine.Timeline;


public class HandPoseManager : MonoBehaviour
{
    public GameObject HandRightWrist;
    private GameObject SightCone;
    private GameObject SecondSelectionBG;
    private GameObject ConnectorManager;
    public GameObject collide;
    private GameObject frame;
    public List<GameObject> selectedRow = new List<GameObject>();
    // private List<GameObject> selectedObjectsFixed = new List<GameObject>();

    private Dictionary<GameObject, TransformData> originalTransform = new Dictionary<GameObject, TransformData>();
    public GameObject FinalObjects;
    private GameObject EyeTrackingManager;

    public TMP_InputField Log;

    private float delayTime = 0.5f; // 延迟时间，单位为秒
    private float delayTimer = 0.0f; // 计时器

    public bool SecondSelectionState = false;
    public bool PalmPoseState = false;

    private int rowNum = 5;
    private int columnNum = 3;

    private float maxAngel = 55f;
    private float minAngel = 0f;

    public bool SelectionStatus = true;

    private Dictionary<GameObject, float> sortedObjectWeights = new Dictionary<GameObject, float>();


    public GameObject StartSelect;
    public GameObject clickSelect;
    public int phase = 0;
    float timer = 0;
    bool finishFlag = false;

    void Start()
    {
        SecondSelectionBG = GameObject.Find("Objects/SecondSelectionBG");
        SightCone = GameObject.Find("SightCone");
        EyeTrackingManager = GameObject.Find("EyeTrackingManager");
        frame = GameObject.Find("frame");
        ConnectorManager = GameObject.Find("ConnectorManager");
        // StartSelect = GameObject.Find("HandPoses/HandPoseGenerator/StartSelect");
        // clickSelect = GameObject.Find("clickSelect");
    }

    // Update is called once per frame
    void Update()
    {
        Log.text = "FinishFlag: " + finishFlag.ToString() + "\n" + "Phase: " + phase.ToString() + "\n" + "Timer: " + timer.ToString() + "\n";
        if(!PalmPoseState){
            delayTimer += Time.deltaTime;
            if (delayTimer > delayTime && SecondSelectionState && SelectionStatus)
            {
                onPalmPoseExitDelay();
            }
        }
        
    }

    public void onPalmPoseStart()
    {
        PalmPoseState = true;
        delayTimer = 0.0f;

        if(!SecondSelectionState && SelectionStatus){
            originalTransform.Clear();
            // selectedObjectsFixed = SightCone.GetComponent<SightCone>().selectedObjects;
            int i = 0;
            SecondSelectionBG.transform.position = new Vector3(0, 0.7f, 2.2f);
            
            sortedObjectWeights = SightCone.GetComponent<SightCone>().objectWeights.OrderByDescending(kv => kv.Value).Take(15).ToDictionary(kv => kv.Key, kv => kv.Value);

            foreach (var obj in sortedObjectWeights)
            {
                if(obj.Key == EyeTrackingManager.GetComponent<EyeTrackingManager>().blinkSelectedObject || FinalObjects.GetComponent<FinalObjects>().finalObj.Contains(obj.Key)) continue;
                originalTransform[obj.Key] = new TransformData(obj.Key.transform.position, obj.Key.transform.localScale);
                obj.Key.GetComponent<Outline>().OutlineColor = Color.clear; 
                obj.Key.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                obj.Key.transform.position = SecondSelectionBG.transform.position + 
                    new Vector3(- SecondSelectionBG.transform.localScale.z/2, + SecondSelectionBG.transform.localScale.y/2, 0) + 
                    new Vector3(obj.Key.transform.localScale.x * (2 * (i%columnNum) + 1) , - obj.Key.transform.localScale.y * (2 * (i/columnNum) + 1), - 2 * obj.Key.transform.localScale.z);
                i++;
            }

        }
        
        SecondSelectionState = true;

    }

    public void onPalmPoseUpdate()
    {
        if(!SecondSelectionState && SelectionStatus) return;
        float wristRotation = HandRightWrist.transform.rotation.eulerAngles.x;
        if(wristRotation > 180f){
            wristRotation -= 360f;
            wristRotation = - wristRotation;
        }
        
        int currentRow = Mathf.RoundToInt(rowNum - (wristRotation - minAngel)/(maxAngel - minAngel) * rowNum);

        // Log.text = "";
        // Log.text +="wristRotation: " + wristRotation.ToString() + "\n";
        // Log.text +="currentRow: " + currentRow.ToString() + "\n";
        // Log.text +="rowNum: " + rowNum.ToString() + "\n";
        selectedRow.Clear();

        int i = 0;
        foreach (var obj in sortedObjectWeights)
        {
            if(i/columnNum == currentRow){
                if(!FinalObjects.GetComponent<FinalObjects>().finalObj.Contains(obj.Key)){
                    obj.Key.GetComponent<Outline>().OutlineColor = Color.yellow; 
                    selectedRow.Add(obj.Key);
                }
            }
            else{
                obj.Key.GetComponent<Outline>().OutlineColor = Color.clear; 
            }
            i++;
        }
        // for(int i = 0; i < selectedObjectsFixed.Count; i++){
        //     if(i/5 == currentRow){
        //         if(!FinalObjects.GetComponent<FinalObjects>().finalObj.Contains(selectedObjectsFixed[i]))
        //             selectedObjectsFixed[i].GetComponent<Outline>().OutlineColor = Color.yellow; 
        //         selectedRow.Add(selectedObjectsFixed[i]);
        //     }
        //     else{
        //         selectedObjectsFixed[i].GetComponent<Outline>().OutlineColor = Color.clear; 
        //     }
        // }
    }

    public void onPalmPoseExit()
    {
        PalmPoseState = false;
    }

    public void onPalmPoseExitDelay()
    {
        foreach (var obj in sortedObjectWeights)
        {
            if (originalTransform.TryGetValue(obj.Key, out TransformData transformData) && 
                !FinalObjects.GetComponent<FinalObjects>().finalObj.Contains(obj.Key))
            {
                obj.Key.transform.position = transformData.Position;
                obj.Key.transform.localScale = transformData.Scale;
            }
        }

        SecondSelectionBG.transform.position = new Vector3(0, -3f, 2.2f);
        delayTimer = 0.0f;
        // selectedObjectsFixed.Clear();
        foreach (var obj in SightCone.GetComponent<SightCone>().selectedObjects)
        {
            obj.GetComponent<Outline>().OutlineColor = Color.clear;
        }
        SightCone.GetComponent<SightCone>().selectedObjects.Clear();
        
        var sightCone = SightCone.GetComponent<SightCone>();

        // 复制字典的键列表
        List<GameObject> keys = sightCone.objectWeights.Keys.ToList();

        foreach (var key in keys)
        {
            sightCone.objectWeights[key] = 0;
        }

        SecondSelectionState = false;
    }
    

    public void OnFinishPoseUpdate(){
        timer += Time.deltaTime;
        if(timer > 0.6 && !finishFlag){
            if(phase == 0){
                StartSelect.SetActive(false);
                clickSelect.SetActive(false);
                SightCone.SetActive(false);
                EyeTrackingManager.SetActive(false);
                foreach (var obj in SightCone.GetComponent<SightCone>().selectedObjects)
                {
                    obj.GetComponent<Outline>().OutlineColor = Color.clear;
                }
                phase = 1;
                collide.GetComponent<collide>().getFinalObject();
            }
           /* else if(phase == 1){
                phase = 2;
                frame.GetComponent<frame>().reverse();
                ConnectorManager.GetComponent<ConnectorManager>().getFrameCenter();
                ConnectorManager.GetComponent<ConnectorManager>().getOriginalOffset();

            }*/
            timer = 0;
            finishFlag = true;
        }
    }

    public void OnFinishPoseEnd(){
        finishFlag = false;
        timer = 0;
    }

}
