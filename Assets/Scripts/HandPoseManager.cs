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
    private GameObject ConnectorManager;
    public GameObject collide;
    public GameObject frameManager;
    private GameObject frame;
    public TMP_Text T;
    public List<GameObject> selectedRow = new List<GameObject>();
    public GameObject emptyBlock;
    // private List<GameObject> selectedObjectsFixed = new List<GameObject>();
    public Dictionary<GameObject, Vector3> objScale = new Dictionary<GameObject, Vector3>();
    public Dictionary<GameObject, TransformData> originalTransform = new Dictionary<GameObject, TransformData>();
    
    private GameObject EyeTrackingManager;
    public GameObject AgentObject;
    public GameObject RubbishBin;


    private float delayPalmExitTime = 0.6f;
    private float delayThumbHoldTimer = 0.4f;
    private float delayThumbExitTimer = 0.4f;
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
    private Transform Objects;
    public bool initFlag = false;
    public List<GameObject> objectsWithTargets = new List<GameObject>();

    private string m_logEntries ;

    private bool m_IsVisible = false;

    private Rect m_WindowRect = new Rect(0, 0, Screen.width, Screen.height);

    private Vector2 m_scrollPositionText = Vector2.zero;

    void Start()
    {
        Application.logMessageReceived += (string condition, string stackTrace, LogType type) =>
        {
            if (type == LogType.Exception || type == LogType.Error)
            {
             
                m_logEntries+=string.Format("{0}\n{1}", condition, stackTrace);
            }
        };

        T.text = m_logEntries;


        // SecondSelectionBG = GameObject.Find("Objects/SecondSelectionBG");
        SightCone = GameObject.Find("SightCone");
        EyeTrackingManager = GameObject.Find("EyeTrackingManager");
        frame = GameObject.Find("frame");
        ConnectorManager = GameObject.Find("ConnectorManager");
        objScale.Add(GameObject.Find("frame/1"), new Vector3(0, 0, 0));
        objScale.Add(GameObject.Find("frame/2"), new Vector3(0, 0, 0));
        objScale.Add(GameObject.Find("frame/3"), new Vector3(0, 0, 0));
        objScale.Add(GameObject.Find("frame/4"), new Vector3(0, 0, 0));
        objScale.Add(GameObject.Find("frame/5"), new Vector3(0, 0, 0));
        objScale.Add(GameObject.Find("frame/6"), new Vector3(0, 0, 0));
        objScale.Add(GameObject.Find("frame/7"), new Vector3(0, 0, 0));
        objScale.Add(GameObject.Find("frame/8"), new Vector3(0, 0, 0));
        // StartSelect = GameObject.Find("HandPoses/HandPoseGenerator/StartSelect");
        // clickSelect = GameObject.Find("clickSelect");

        Objects = GameObject.Find("Objects").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(!initFlag){
            initFlag = true;
            foreach (Transform obj in Objects)
            {
                if(obj.CompareTag("Target")) {
                    originalTransform[obj.gameObject] = new TransformData(obj.position, obj.rotation, obj.localScale);
                    objScale[obj.gameObject] = obj.localScale;
                    if(GameObject.Find("Objects/" + obj.name + " (1)"))
                    {
                        objectsWithTargets.Add(obj.gameObject);
                    }
                }
            }
            if(phase == 0){
                selectionTime += Time.deltaTime;
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
        PalmPoseState = true;
        delayTimer = 0.0f;

        if(!SecondSelectionState && phase == 0){
            // selectedObjectsFixed = SightCone.GetComponent<SightCone>().selectedObjects;
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
        if(!SecondSelectionState && phase == 0) return;
        float wristRotation = HandRightWrist.transform.rotation.eulerAngles.x;
        if(wristRotation > 180f){
            wristRotation -= 360f;
        }
        wristRotation = - wristRotation;
        
        int currentRow = Mathf.RoundToInt((wristRotation - minAngel)/(maxAngel - minAngel) * rowNum);

        if(wristRotation < minAngel) currentRow = 0;
        if(wristRotation > maxAngel) currentRow = rowNum - 1;

        // Log.text = "";
        // Log.text +="wristRotation: " + wristRotation.ToString() + "\n";
        // Log.text +="currentRow: " + currentRow.ToString() + "\n";
        // Log.text +="rowNum: " + rowNum.ToString() + "\n";
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
        if(thumbHoldTimer > delayThumbHoldTimer && !finishFlag){
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
                collide.GetComponent<collide>().enabled = true; 
                TimeRecorder.SetActive(false);
                StartSelect.SetActive(false);
                clickSelect.SetActive(false);
                SightCone.SetActive(false);
                EyeTrackingManager.SetActive(false);
                onPalmPoseExitDelay();
                SecondSelectionBG.SetActive(false);
                foreach (var obj in SightCone.GetComponent<SightCone>().selectedObjects)
                {
                    obj.GetComponent<Outline>().OutlineColor = Color.clear;
                }
                //collide.GetComponent<collide>().enabled = true;
                //collide.GetComponent<collide>().frame.GetComponent<frame>().creatRect();
                frameManager.SetActive(true);
                AgentObject.SetActive(false);
                RubbishBin.SetActive(true);
                collide.GetComponent<collide>().getFinalObject();
                break;
            case 2://放大，开始移动
                AgentObject.SetActive(true);
                frame.GetComponent<frame>().DestroyColliders();//把碰撞箱全部消除防止误触
                FinalObjects.SetActive(false);
                TimeRecorder.SetActive(true);
                collide.GetComponent<collide>().anchorChoose();
                frameManager.SetActive(false);
                RubbishBin.SetActive(false);
                ConnectorManager.GetComponent<ConnectorManager>().reverse();  //bug
                collide.GetComponent<collide>().enabled = false;
                break;
            case 3:
                foreach (var obj in ConnectorManager.GetComponent<ConnectorManager>().newObjects)
                {
                    obj.SetActive(false);
                }
                ConnectorManager.GetComponent<ConnectorManager>().newObjects.Clear();
                ConnectorManager.GetComponent<ConnectorManager>().frameAgent.SetActive(false);
                collide.GetComponent<collide>().onFrame.Clear();
                phase = 0;
                StartSelect.SetActive(true);
                clickSelect.SetActive(true);
                StartCoroutine(clickSelect.GetComponent<clickSelect>().GetFingerAngle());
                SightCone.SetActive(true);
                EyeTrackingManager.SetActive(true);
                FinalObjects.SetActive(true);
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
