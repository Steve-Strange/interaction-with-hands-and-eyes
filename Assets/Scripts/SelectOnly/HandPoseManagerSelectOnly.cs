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

    public GameObject collide;

    public List<GameObject> selectedRow = new List<GameObject>();
    public GameObject emptyBlock;
    // private List<GameObject> selectedObjectsFixed = new List<GameObject>();
    public Dictionary<GameObject, Vector3> objScale = new Dictionary<GameObject, Vector3>();
    public Dictionary<GameObject, TransformData> originalTransform = new Dictionary<GameObject, TransformData>();

    private GameObject EyeTrackingManager;
    //public TMP_Text Phase;

    // public TMP_InputField Log;


    private float delayTime = 1f; // 延迟时间，单位为秒
    private float delayTimer = 0.0f; // 计时器

    public bool SecondSelectionState = false;
    public bool PalmPoseState = false;

    private int rowNum = 5;
    private int columnNum = 3;

    private float maxAngel = 50f;
    private float minAngel = 10f;
    public bool SelectionStatus = true;

    private Dictionary<GameObject, float> sorted15ObjectWeights = new Dictionary<GameObject, float>();
    private Dictionary<GameObject, float> sortedRemainObjectWeights = new Dictionary<GameObject, float>();

    public GameObject StartSelect;
    public GameObject clickSelect;
    public int phase = 0;
    float thumbHoldTimer = 0;
    float thumbExitTimer = 0;
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
            if (delayTimer > delayTime && SecondSelectionState && SelectionStatus)
            {
                onPalmPoseExitDelay();
            }
        }
        if (!thumbHoldState)
        {
            thumbExitTimer += Time.deltaTime;
            if (thumbExitTimer > delayTime)
            {
                finishFlag = false;
                thumbHoldTimer = 0;
            }
        }
    }

    public void onPalmPoseStart()
    {
        PalmPoseState = true;
        delayTimer = 0.0f;

        if (!SecondSelectionState && SelectionStatus)
        {
           
            int i = 0;
            SecondSelectionBG.SetActive(true);


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

    public void onPalmPoseUpdate()
    {
        if (!SecondSelectionState && SelectionStatus) return;
        float wristRotation = HandRightWrist.transform.rotation.eulerAngles.x;
        if (wristRotation > 180f)
        {
            wristRotation -= 360f;
        }
        wristRotation = -wristRotation;

        int currentRow = Mathf.RoundToInt((wristRotation - minAngel) / (maxAngel - minAngel) * rowNum);

        if (wristRotation < minAngel) currentRow = 0;
        if (wristRotation > maxAngel) currentRow = rowNum - 1;

        // Log.text = "";
        // Log.text +="wristRotation: " + wristRotation.ToString() + "\n";
        // Log.text +="currentRow: " + currentRow.ToString() + "\n";
        // Log.text +="rowNum: " + rowNum.ToString() + "\n";
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
