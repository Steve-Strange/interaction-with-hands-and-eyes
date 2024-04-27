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


public class HandPoseManagerManipulationOnly : MonoBehaviour
{   public GameObject FinalObjects;
    public GameObject emptyBlock;
    // private List<GameObject> selectedObjectsFixed = new List<GameObject>();
    public Dictionary<GameObject, Vector3> objScale = new Dictionary<GameObject, Vector3>();
    public Dictionary<GameObject, TransformData> originalTransform = new Dictionary<GameObject, TransformData>();
    
    private GameObject EyeTrackingManager;
    //public TMP_Text Phase;

    // public TMP_InputField log;
    public GameObject AgentObject;
    public GameObject RubbishBin;

    private float delayTime = 1f; // 延迟时间，单位为秒
    private float delayTimer = 0.0f; // 计时器

    public bool SecondSelectionState = false;
    public bool PalmPoseState = false;

    public GameObject StartSelect;
    public GameObject clickSelect;
    public GameObject TimeRecorder;
    public int phase = 1;
    float thumbHoldTimer = 0;
    float thumbExitTimer = 0;
    bool thumbHoldState = false;
    bool finishFlag = false;
    public float selectionTime;
    private Transform Objects;
    bool initFlag = false;
    public List<GameObject> objectsWithTargets = new List<GameObject>();

    void Start()
    {
        // SecondSelectionBG = GameObject.Find("Objects/SecondSelectionBG");
        EyeTrackingManager = GameObject.Find("EyeTrackingManager");

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
                        FinalObjects.GetComponent<FinalObjects>().AddFinalObj(obj.gameObject);
                    }
                }
            }
            if(phase == 0){
                selectionTime += Time.deltaTime;
            }
            // log.text += originalTransform.Count;
            ChangePhase(phase++);
        }
        
        if(!thumbHoldState){
            thumbExitTimer += Time.deltaTime;
            if(thumbExitTimer > delayTime){
                finishFlag = false;
                thumbHoldTimer = 0;
            }
        }
    }

    public void OnFinishPoseUpdate(){
        thumbExitTimer = 0;
        thumbHoldState = true;
        thumbHoldTimer += Time.deltaTime;
        if(thumbHoldTimer > 0.4f && !finishFlag){
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
                FinalObjects.SetActive(true);
                TimeRecorder.SetActive(false);
                StartSelect.SetActive(false);
                clickSelect.SetActive(false);
                EyeTrackingManager.SetActive(false);
                //collide.GetComponent<collide>().enabled = true;
                //collide.GetComponent<collide>().frame.GetComponent<frame>().creatRect();
                AgentObject.SetActive(false);
                RubbishBin.SetActive(true);
                break;
            case 2:
                AgentObject.SetActive(true);
                FinalObjects.SetActive(false);
                TimeRecorder.SetActive(true);
                RubbishBin.SetActive(false);
                break;
            case 3:
                phase = 0;
                StartSelect.SetActive(true);
                clickSelect.SetActive(true);
                break;
            default:
                break;
        }
    }

}
