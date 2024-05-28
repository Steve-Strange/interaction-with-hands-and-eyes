using UnityEngine;
using System.Collections.Generic;
using Unity.XR.PXR;
using UnityEngine.XR;
using TMPro;
using Unity.VisualScripting;
using System;
using System.Linq;
using UnityEngine.Timeline;
using System.Diagnostics;


public class HandPoseManager : MonoBehaviour
{   public GameObject FinalObjects;
    public GameObject HandRightWrist;
    private GameObject SightCone;
    public GameObject SecondSelectionBG;
   
    // public TMP_Text T2;
    public List<GameObject> selectedRow = new List<GameObject>();
    public GameObject emptyBlock;
    // private List<GameObject> selectedObjectsFixed = new List<GameObject>();
    public Dictionary<GameObject, Vector3> objScale = new Dictionary<GameObject, Vector3>();
    public Dictionary<GameObject, TransformData> originalTransform = new Dictionary<GameObject, TransformData>();
    
    private GameObject EyeTrackingManager;
    public GameObject AgentObject;
    public GameObject RubbishBin;


    private float delayPalmExitTime = 0.4f;
    private float delayThumbHoldTimer = 0.3f;
    private float delayThumbExitTimer = 0.3f;
    private float delayTimer = 0.0f;
    float thumbHoldTimer = 0;
    float thumbExitTimer = 0;

    public bool SecondSelectionState = false;
    public bool PalmPoseState = false;

    private int rowNum = 5;
    private int columnNum = 3;

    private float maxAngel = 50f;
    private float minAngel = 10f;

    private Dictionary<GameObject, float> sorted15ObjectWeights = new Dictionary<GameObject, float>();
    private Dictionary<GameObject, float> sortedRemainObjectWeights = new Dictionary<GameObject, float>();

    public GameObject StartSelect;
    public GameObject clickSelect;
    public GameObject TimeRecorder;
    public int phase = 0;
    bool thumbHoldState = false;
    bool finishFlag = false;
    public float selectionTime;
    public float movingTime;
    private Transform Objects;
    public bool initFlag = false;
    // public TMP_InputField log;
    public List<GameObject> objectsWithTargets = new List<GameObject>();
    public GameObject StartSelectPose;

    void Start()
    {
        SightCone = GameObject.Find("SightCone");
        EyeTrackingManager = GameObject.Find("EyeTrackingManager");

        Objects = GameObject.Find("Objects").transform;
    }

    // Update is called once per frame
    void Update()
    {
        //log.text = "phase: " + phase.ToString() + "\n";
      //  log.text += "timer:" + delayTimer.ToString() + "\n";
      //  log.text += "SecondSelectionState: " + SecondSelectionState.ToString() + "\n";
    
        if(phase == 0){
            selectionTime += Time.deltaTime;
        }
        else if(phase == 1){
            movingTime += Time.deltaTime;
        }

        // T.text = m_logEntries;
        //2.text = phase.ToString();
        if (!initFlag){
            initFlag = true;
            foreach (Transform obj in Objects)
            {
                if(obj.CompareTag("Target")) {
                    originalTransform[obj.gameObject] = new TransformData(obj.position, obj.rotation, obj.localScale);
                    if(GameObject.Find("Objects/" + obj.name + " (1)") && objectsWithTargets.Contains(obj.gameObject) == false){
                        objectsWithTargets.Add(obj.gameObject);
                        AgentObject.GetComponent<GrabAgentObject>().TargetObjects[obj.gameObject] = GameObject.Find("Objects/" + obj.name + " (1)");

                    }
                }
            }
            // log.text += originalTransform.Count;
        }
        
        // Log.text = "rowNum: " + rowNum.ToString() + "\n" + "sorted15ObjectWeights: " + sorted15ObjectWeights.Count.ToString() + "currentRow: " + selectedRow.ToString() + "\n";
        if(!PalmPoseState && phase == 0){
            delayTimer += Time.deltaTime;
            if (delayTimer > delayPalmExitTime && SecondSelectionState)
            {
                onPalmPoseExitDelay();
            }
        }
        if(!thumbHoldState){
            thumbExitTimer += Time.deltaTime;
            if(thumbExitTimer > delayThumbExitTimer){
                finishFlag = false;
                thumbHoldTimer = 0;
            }
        }
    }

    public void onPalmPoseStart()
    {
        delayTimer = 0.0f;
        PalmPoseState = true;

        if(!SecondSelectionState && phase == 0){
            int i = 0;
            SecondSelectionBG.SetActive(true);

            List<GameObject> keysToModify = new List<GameObject>();

            foreach (var obj in SightCone.GetComponent<SightCone>().objectWeights)
            {
                if (sortedRemainObjectWeights.ContainsKey(obj.Key))
                {
                    keysToModify.Add(obj.Key);
                }
            }

            // 修改键对应的值
            foreach (var key in keysToModify)
            {
                SightCone.GetComponent<SightCone>().objectWeights[key] += sortedRemainObjectWeights[key] / 2;
            }
            
            sorted15ObjectWeights = SightCone.GetComponent<SightCone>().objectWeights.OrderByDescending(kv => kv.Value).Take(15).ToDictionary(kv => kv.Key, kv => kv.Value);
            sortedRemainObjectWeights = SightCone.GetComponent<SightCone>().objectWeights.OrderByDescending(kv => kv.Value).ToDictionary(kv => kv.Key, kv => kv.Value);
            foreach (var obj in sorted15ObjectWeights)
            {
                if(sortedRemainObjectWeights.ContainsKey(obj.Key)){
                    sortedRemainObjectWeights.Remove(obj.Key);
                }
            }

            rowNum = Mathf.CeilToInt(sorted15ObjectWeights.Count / columnNum);
            foreach (var obj in sorted15ObjectWeights)
            {
                if(obj.Key == EyeTrackingManager.GetComponent<EyeTrackingManager>().blinkSelectedObject || FinalObjects.GetComponent<FinalObjects>().finalObj.Contains(obj.Key)) continue;
                
                obj.Key.GetComponent<Outline>().OutlineColor = Color.clear;

                float objMaxScale = Mathf.Max(obj.Key.transform.GetComponent<Renderer>().bounds.size.x, obj.Key.transform.GetComponent<Renderer>().bounds.size.y, obj.Key.transform.GetComponent<Renderer>().bounds.size.z);
                
                obj.Key.transform.localScale = new Vector3(0.1f * obj.Key.transform.localScale.x, 0.1f * obj.Key.transform.localScale.y, 0.1f * obj.Key.transform.localScale.z) / objMaxScale;
                obj.Key.transform.localEulerAngles = new Vector3(0, 0, 0);
                obj.Key.transform.position = SecondSelectionBG.transform.position + 
                    new Vector3(- SecondSelectionBG.transform.localScale.z/2, - SecondSelectionBG.transform.localScale.y/2, -SecondSelectionBG.transform.localScale.x/2) + 
                    new Vector3(0.1f * (2 * (i%columnNum) + 1) , + 0.1f * (2 * (i/columnNum) + 1), - 0.1f);
                i++;
            }

        }
        
        SecondSelectionState = true;

    }
   // public List<GameObject> targets = new List<GameObject>();
    public void onPalmPoseUpdate()
    {
        delayTimer = 0.0f;
        if(!SecondSelectionState || phase != 0) return;
        float wristRotation = HandRightWrist.transform.rotation.eulerAngles.x;
        if(wristRotation > 180f){
            wristRotation = 360 - wristRotation;
        }
        else
        {
            wristRotation = - wristRotation;
        }


        int currentRow;
    
        if (wristRotation < minAngel) currentRow = 0;
        else if(wristRotation > maxAngel) currentRow = rowNum - 1;
        else
        {
            currentRow = Mathf.RoundToInt((wristRotation - minAngel) / (maxAngel - minAngel) * rowNum);
        }

        selectedRow.Clear();

        int i = 0;
        foreach (var obj in sorted15ObjectWeights)
        {
            if(i/columnNum == currentRow){
                if(!FinalObjects.GetComponent<FinalObjects>().finalObj.Contains(obj.Key)){
                    obj.Key.GetComponent<Outline>().OutlineColor = Color.yellow;
                    if(objectsWithTargets.Contains(obj.Key)){
                        obj.Key.GetComponent<Outline>().OutlineColor = Color.red;
                    }
                    obj.Key.GetComponent<Outline>().OutlineWidth = 5f;
                    selectedRow.Add(obj.Key);
                }
                else {
                    selectedRow.Add(emptyBlock);
                }
            }
            else{
                obj.Key.GetComponent<Outline>().OutlineColor = Color.clear; 
            }
            i++;
        }

    }

    public void onPalmPoseExit()
    {
        PalmPoseState = false;
    }

    public void onPalmPoseExitDelay()
    {
        foreach (var obj in sorted15ObjectWeights)
        {
            if (originalTransform.TryGetValue(obj.Key, out TransformData transformData) && 
                !FinalObjects.GetComponent<FinalObjects>().finalObj.Contains(obj.Key))
            {
                obj.Key.transform.position = transformData.Position;
                obj.Key.transform.localScale = transformData.Scale;
                obj.Key.GetComponent<Outline>().OutlineColor = Color.clear;
            }
        }

        SecondSelectionBG.SetActive(false);
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
        thumbExitTimer = 0;
        thumbHoldState = true;
        thumbHoldTimer += Time.deltaTime;
        if(SecondSelectionState && phase == 0) return;
        else if(thumbHoldTimer > delayThumbHoldTimer && !finishFlag){
            phase++;
            ChangePhase(phase);
            thumbHoldTimer = 0;
            finishFlag = true;
        }
    }

    public void OnFinishPoseEnd(){
        thumbHoldState = false;
    }

    public void ChangePhase(int currentPhase){

        switch (currentPhase)
        {
            case 1:
                StartSelectPose.SetActive(false);
                clickSelect.SetActive(false);
                AgentObject.SetActive(true);
                TimeRecorder.SetActive(true);
                SightCone.SetActive(false);
                break;
            case 2:
                phase = 0;
                foreach (var obj in FinalObjects.GetComponent<FinalObjects>().finalObj)
                {
                    obj.GetComponent<Outline>().OutlineColor = Color.clear;
                    obj.tag = "Target";
                }
                StartSelectPose.SetActive(true);
                StartSelect.SetActive(true);
                clickSelect.SetActive(true);
                StartCoroutine(clickSelect.GetComponent<clickSelect>().GetFingerAngle());
                SightCone.SetActive(true);
                AgentObject.SetActive(false);
                initFlag = false;
                AgentObject.GetComponent<GrabAgentObject>().initFlag = false;
                SightCone.GetComponent<SightCone>().objectWeights.Clear();
                while(FinalObjects.GetComponent<FinalObjects>().finalObj.Count > 0)
                {
                    RubbishBin.GetComponent<RubbishBin>().RemoveFinalObject();
                }
                StartCoroutine(SightCone.GetComponent<SightCone>().UpdateObjectWeights());
                break;
            default:
                break;
        }
    }

}
