using UnityEngine;
using System.Collections.Generic;
using Unity.XR.PXR;
using UnityEngine.XR;
using TMPro;
using Unity.VisualScripting;
using System;


public class HandPoseManager : MonoBehaviour
{
    public TMP_InputField inputField;
    public GameObject HandRightWrist;
    private GameObject SightCone;
    public GameObject HandLeft;
    public GameObject SecondSelectionBG;
    public GameObject[] back;
    private List<WeightedObject> weightObjectsFixed = new List<WeightedObject>();

    private Dictionary<GameObject, TransformData> originalTransform = new Dictionary<GameObject, TransformData>();

    private float delayTime = 0.5f; // 延迟时间，单位为秒
    private float delayTimer = 0.0f; // 计时器

    public bool SecondSelectionState = false;
    private bool PalmPoseState = false;

    private int rowNum = 0;

    private float maxAngel = 35f;
    private float minAngel = -15f;

    void Start()
    {
        back = new GameObject[5];
        SecondSelectionBG = GameObject.Find("Objects/SecondSelectionBG");
        SightCone = GameObject.Find("SightCone");
    }

    // Update is called once per frame
    void Update()
    {
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
        SightCone.GetComponent<SightCone>().weightedObjects.Sort((a, b) => a.weight.CompareTo(b.weight));
        delayTimer = 0.0f;

        if(!SecondSelectionState){
            originalTransform.Clear();
            weightObjectsFixed = SightCone.GetComponent<SightCone>().weightedObjects;
            int i = 0;
            SecondSelectionBG.transform.position = new Vector3(0, 0.7f, 2.2f);
            foreach (var weightObj in weightObjectsFixed)
            {
                originalTransform[weightObj.obj] = new TransformData(weightObj.obj.transform.position, weightObj.obj.transform.localScale);
                weightObj.obj.GetComponent<Renderer>().material.color = SightCone.GetComponent<SightCone>().originalMaterials[weightObj.obj].color;
                weightObj.obj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                weightObj.obj.transform.position = SecondSelectionBG.transform.position + new Vector3(- SecondSelectionBG.transform.localScale.z/2, + SecondSelectionBG.transform.localScale.y/2, 0) + new Vector3(obj.transform.localScale.x * (2 * (i%5) + 1) , - obj.transform.localScale.y * (2 * (i/5) + 1), - 2 * obj.transform.localScale.z);
                i++;
            }
            rowNum = Mathf.CeilToInt(i/5);
        }
        
        SecondSelectionState = true;

    }

    public void onPalmPoseUpdate()
    {
        inputField.text += HandRightWrist.transform.rotation.eulerAngles.ToString();
        float wristRotation = HandRightWrist.transform.rotation.eulerAngles.x;
        if(wristRotation > 180f){
            wristRotation -= 360f;
        }
        
        int currentRow = Mathf.RoundToInt((wristRotation - minAngel)/(maxAngel - minAngel) * rowNum);

        inputField.text = "";
        inputField.text +="wristRotation: " + wristRotation.ToString() + "\n";
        inputField.text +="currentRow: " + currentRow.ToString() + "\n";
        inputField.text +="rowNum: " + rowNum.ToString() + "\n";

        int mark = 0;
        for(int i = 0; i < selectedObjectsFixed.Count; i++){
            if(i/5 == currentRow){
                selectedObjectsFixed[i].GetComponent<Renderer>().material.color = Color.yellow;
                back[mark++] = selectedObjectsFixed[i];
            }
            else{
                selectedObjectsFixed[i].GetComponent<Renderer>().material.color = SightCone.GetComponent<SightCone>().originalMaterials[selectedObjectsFixed[i]].color;
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
            if (originalTransform.TryGetValue(obj, out TransformData transformData))
            {
                obj.transform.position = transformData.Position;
                obj.transform.localScale = transformData.Scale;
                obj.GetComponent<Renderer>().material.color = SightCone.GetComponent<SightCone>().originalMaterials[obj].color;
            }
        }

        SecondSelectionBG.transform.position = new Vector3(0, -3f, 2.2f);
        delayTimer = 0.0f;
        selectedObjectsFixed.Clear();
        SightCone.GetComponent<SightCone>().selectedObjects.Clear();
        SightCone.transform.localScale = new Vector3(SightCone.transform.localScale.x, SightCone.transform.localScale.y, 0);
        SecondSelectionState = false;
    }

}
