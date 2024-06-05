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


public class HandPoseManagerSelectOnly : MonoBehaviour
{
    public GameObject FinalObjects;
    public GameObject HandRightWrist;
    private GameObject SightCone;
    public GameObject SecondSelectionBG;
    public GameObject recorder;
    public GameObject collide;
    public TMP_Text T;
    public List<GameObject> selectedRow = new List<GameObject>();
    public GameObject emptyBlock;
    // private List<GameObject> selectedObjectsFixed = new List<GameObject>();
    public Dictionary<GameObject, Vector3> objScale = new Dictionary<GameObject, Vector3>();
    public Dictionary<GameObject, TransformData> originalTransform = new Dictionary<GameObject, TransformData>();

    private GameObject EyeTrackingManager;
    //public TMP_Text Phase;

    // public TMP_InputField Log;

    private float delayPalmExitTime = 0.5f;
    private float delayThumbHoldTimer = 0.4f;
    private float delayThumbExitTimer = 0.4f;
    private float delayTimer = 0.0f;
    float thumbHoldTimer = 0;
    float thumbExitTimer = 0;

    public bool SecondSelectionState = false;
    public bool PalmPoseState = false;

    private int rowNum = 5;
    private int columnNum = 3;

    public float maxAngel = 50f;
    public float minAngel = 10f;
    public bool SelectionStatus = true;

    public GameObject[] demoObjects;
    public Dictionary<GameObject, float> sorted15ObjectWeights = new Dictionary<GameObject, float>();
    private Dictionary<GameObject, float> sortedRemainObjectWeights = new Dictionary<GameObject, float>();

    public GameObject StartSelect;
    public GameObject clickSelect;
    public int phase = 0;
    bool thumbHoldState = false;
    bool finishFlag = false;
    public float selectionTime;

    void Start()
    {
        SightCone = GameObject.Find("SightCone");
        EyeTrackingManager = GameObject.Find("EyeTrackingManager");
    }

    // Update is called once per frame
    void Update()
    {
        if (phase == 0)
        {
            selectionTime += Time.deltaTime;
        }

        // Log.text = "rowNum: " + rowNum.ToString() + "\n" + "sorted15ObjectWeights: " + sorted15ObjectWeights.Count.ToString() + "currentRow: " + selectedRow.ToString() + "\n";
        if (!PalmPoseState)
        {
            delayTimer += Time.deltaTime;
            if (delayTimer > delayPalmExitTime && SecondSelectionState && SelectionStatus)
            {
                onPalmPoseExitDelay();
            }
        }
        if (!thumbHoldState)
        {
            thumbExitTimer += Time.deltaTime;
            if (thumbExitTimer > delayThumbExitTimer)
            {
                finishFlag = false;
                thumbHoldTimer = 0;
            }
        }
    }

    public void onPalmPoseStart()
    {
        //手成掌，待选框出现，此时无延迟
        recorder.GetComponent<singleSelect>().writeFile("onPalmPoseStart:" + recorder.GetComponent<singleSelect>().timer.ToString() + "\n");
        PalmPoseState = true;
        delayTimer = 0.0f;

        if (!SecondSelectionState && SelectionStatus)
        {
           
            int i = 0;
            SecondSelectionBG.SetActive(true);//板子出现


            List<GameObject> keysToModify = new List<GameObject>();

            foreach (var obj in SightCone.GetComponent<SightConeSelectOnly>().objectWeights)
            {
                if (sortedRemainObjectWeights.ContainsKey(obj.Key))
                {
                    keysToModify.Add(obj.Key);
                }
            }

            // 修改键对应的值
            foreach (var key in keysToModify)
            {
                SightCone.GetComponent<SightConeSelectOnly>().objectWeights[key] += sortedRemainObjectWeights[key] / 2;
            }
           

            sorted15ObjectWeights = SightCone.GetComponent<SightConeSelectOnly>().objectWeights.OrderByDescending(kv => kv.Value).Take(15).ToDictionary(kv => kv.Key, kv => kv.Value);
            sortedRemainObjectWeights = SightCone.GetComponent<SightConeSelectOnly>().objectWeights.OrderByDescending(kv => kv.Value).ToDictionary(kv => kv.Key, kv => kv.Value);
            foreach (var obj in sorted15ObjectWeights)
            {
                if (sortedRemainObjectWeights.ContainsKey(obj.Key))
                {
                    sortedRemainObjectWeights.Remove(obj.Key);
                }
            }

            rowNum = Mathf.CeilToInt(sorted15ObjectWeights.Count / columnNum);
            foreach (var obj in sorted15ObjectWeights)
            {
                if (obj.Key == EyeTrackingManager.GetComponent<EyeTrackingManagerSelectOnly>().blinkSelectedObject || FinalObjects.GetComponent<FinalObjectsSelectOnly>().finalObj.Contains(obj.Key)) continue;
                originalTransform[obj.Key] = new TransformData(obj.Key.transform.position, obj.Key.transform.rotation, obj.Key.transform.localScale);
                obj.Key.GetComponent<Outline>().OutlineColor = Color.clear;
                objScale[obj.Key] = obj.Key.transform.localScale;

                float objMaxScale = Mathf.Max(obj.Key.transform.GetComponent<Renderer>().bounds.size.x, obj.Key.transform.GetComponent<Renderer>().bounds.size.y, obj.Key.transform.GetComponent<Renderer>().bounds.size.z);
                
                obj.Key.transform.localScale = new Vector3(0.1f * obj.Key.transform.localScale.x, 0.1f * obj.Key.transform.localScale.y, 0.1f * obj.Key.transform.localScale.z) / objMaxScale;
                obj.Key.transform.localEulerAngles = new Vector3(0, 0, 0);
                obj.Key.transform.position = SecondSelectionBG.transform.position +
                    new Vector3(-SecondSelectionBG.transform.localScale.z / 2, -SecondSelectionBG.transform.localScale.y / 2, -SecondSelectionBG.transform.localScale.x / 2) +
                    new Vector3(0.1f * (2 * (i % columnNum) + 1), + 0.1f * (2 * (i / columnNum) + 1), -0.1f);
                i++;
            }

        }

        SecondSelectionState = true;

    }
    public void decideTheMinAngleAndMaxAngle()
    {



    }
    public void onPalmPoseUpdate()
    {
        delayTimer = 0.0f;
        if (!SecondSelectionState || phase != 0) return;
        float wristRotation = HandRightWrist.transform.rotation.eulerAngles.x;
        if (wristRotation > 180f)
        {
            wristRotation = 360 - wristRotation;
        }
        else
        {
            wristRotation = -wristRotation;
        }
        if(minAngel < -25){
            minAngel = -25;
        }
        if(maxAngel > 60) { 
            maxAngel = 60;
        }

        int currentRow;
        T.text = minAngel.ToString()+"     "+maxAngel.ToString()+"      "+ ((wristRotation - minAngel) / (maxAngel - minAngel) * rowNum).ToString();
        if (wristRotation < minAngel) currentRow = 0;
        else if (wristRotation > maxAngel) currentRow = rowNum - 1;
        else{
            currentRow = Mathf.RoundToInt((wristRotation - minAngel) / (maxAngel - minAngel) * rowNum);
        }

        selectedRow.Clear();


        int i = 0;
        foreach (var obj in sorted15ObjectWeights)
        {
            if (i / columnNum == currentRow)
            {
                if (!FinalObjects.GetComponent<FinalObjectsSelectOnly>().finalObj.Contains(obj.Key))
                {
                    obj.Key.GetComponent<Outline>().OutlineColor = Color.yellow;
                    obj.Key.GetComponent<Outline>().OutlineWidth = 5f;
                    selectedRow.Add(obj.Key);
                }
                else
                {
                    selectedRow.Add(emptyBlock);
                }
            }
            else
            {
                obj.Key.GetComponent<Outline>().OutlineColor = Color.clear;
            }
            i++;
        }
    }

    public void onPalmPoseExit()
    {
        recorder.GetComponent<singleSelect>().writeFile("onPalmPoseExit:  " + recorder.GetComponent<singleSelect>().timer.ToString()+ "\n");
        PalmPoseState = false;
    }

    public void onPalmPoseExitDelay()
    {
        //待选框消失
        recorder.GetComponent<singleSelect>().writeFile("onSecondSelectionBGDisappear:  " + recorder.GetComponent<singleSelect>().timer.ToString()+"\n");
        foreach (var obj in sorted15ObjectWeights)
        {
            if (originalTransform.TryGetValue(obj.Key, out TransformData transformData) &&
                !FinalObjects.GetComponent<FinalObjectsSelectOnly>().finalObj.Contains(obj.Key))
            {
                obj.Key.transform.position = transformData.Position;
                obj.Key.transform.localScale = transformData.Scale;
                obj.Key.GetComponent<Outline>().OutlineColor = Color.clear;
            }
        }

        SecondSelectionBG.SetActive(false);
        delayTimer = 0.0f;
        foreach (var obj in SightCone.GetComponent<SightConeSelectOnly>().selectedObjects)
        {
            obj.GetComponent<Outline>().OutlineColor = Color.clear;
        }
        SightCone.GetComponent<SightConeSelectOnly>().selectedObjects.Clear();

        var sightCone = SightCone.GetComponent<SightConeSelectOnly>();

        // 复制字典的键列表
        List<GameObject> keys = sightCone.objectWeights.Keys.ToList();

        foreach (var key in keys)
        {
            sightCone.objectWeights[key] = 0;
        }

        SecondSelectionState = false;
    }


    public void OnFinishPoseUpdate()
    {
        thumbExitTimer = 0;
        thumbHoldState = true;
        thumbHoldTimer += Time.deltaTime;
        if (thumbHoldTimer > 0.4f && !finishFlag)
        {

            thumbHoldTimer = 0;
            finishFlag = true;
        }
    }

    public void OnFinishPoseEnd()
    {
        thumbHoldState = false;
    }

}
