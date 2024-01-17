using UnityEngine;
using System.Collections.Generic;
using System;
using TMPro;
using Unity.VisualScripting;
using System.Linq;

public class SightCone : MonoBehaviour
{
    private float coneAngle = 21f; // 圆锥的角度
    public Material highlightMaterial;
    public List<GameObject> selectedObjects = new List<GameObject>(); // 存储当前被高亮的物体

    public Dictionary<GameObject, Material> originalMaterials = new Dictionary<GameObject, Material>();
    private GameObject HandPoseManager;
    private GameObject EyeTrackingManager;
    private float MaxDepth = 10f;

    public TMP_InputField Log;
    private bool reFocus = true;
    private Queue<Vector3> orientationQueue = new Queue<Vector3>();
    private int reFoucsTime = 20;
    private float reFoucsThreshold = 10f;

    void Start(){
        EyeTrackingManager = GameObject.Find("EyeTrackingManager");
        HandPoseManager = GameObject.Find("HandPoseManager");
        transform.localScale = new Vector3(transform.localScale.z * Mathf.Tan(coneAngle * Mathf.Deg2Rad),
                                transform.localScale.z * Mathf.Tan(coneAngle * Mathf.Deg2Rad), transform.localScale.z);

    }

    void Update()
    {
        orientationQueue.Enqueue(EyeTrackingManager.GetComponent<EyeTrackingManager>().combineEyeGazeVectorInWorldSpace);
        if(orientationQueue.Count > reFoucsTime){
            orientationQueue.Dequeue();
        }

        if(transform.localScale.z < MaxDepth)
        {
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, MaxDepth);
        }
        else
        {
            gameObject.GetComponent<Light>().spotAngle = coneAngle;
            float density = selectedObjects.Count / coneAngle;
            if(coneAngle <= 27f && selectedObjects.Count <= 25 && density <= 0.3){
                coneAngle += 3f;
                transform.localScale = new Vector3(transform.localScale.z * Mathf.Tan(coneAngle * Mathf.Deg2Rad),
                                    transform.localScale.z * Mathf.Tan(coneAngle * Mathf.Deg2Rad), transform.localScale.z);
            }
            else if(coneAngle >= 18f && density >= 0.5){
                coneAngle -= 3f;
                transform.localScale = new Vector3(transform.localScale.z * Mathf.Tan(coneAngle * Mathf.Deg2Rad),
                                    transform.localScale.z * Mathf.Tan(coneAngle * Mathf.Deg2Rad), transform.localScale.z);
            }
            // Log.text = "coneAngle: " + coneAngle + "\n" + "selectedObjects.Count: " + selectedObjects.Count + "\n" + "density: " + density;
        }
        
        ReFoucs();
    }

    void ReFoucs(){
        Vector3 previousOrientation = new Vector3(0,0,0);
        Vector3 currentOrientation = new Vector3(0,0,0);
        int i = 0;
        foreach (var orientation in orientationQueue)
        {
            if(i < 2f/reFoucsTime) previousOrientation += orientation * (2f/reFoucsTime);
            else currentOrientation += orientation * (2f/reFoucsTime);
            i++;
        }
        if(Vector3.Angle(previousOrientation, currentOrientation) > reFoucsThreshold && !reFocus && !HandPoseManager.GetComponent<HandPoseManager>().SecondSelectionState){
            reFocus = true;
            orientationQueue.Clear();
            if(Log.text.Length > 200) Log.text = "";
            Log.text += "\n" + "reFocus: " + reFocus + "  " + Time.time;
        }
        else{
            reFocus = false;
        }
        
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (!HandPoseManager.GetComponent<HandPoseManager>().SecondSelectionState && 
            !selectedObjects.Contains(other.gameObject) && other.gameObject.CompareTag("Target"))
        
        {
            // 新物体进入触发器
            GameObject hitIObj = other.gameObject;
            // 在更改材质之前存储原始材质
            if (!originalMaterials.ContainsKey(hitIObj))
            {
                originalMaterials[hitIObj] = hitIObj.GetComponent<Renderer>().material;
            }

            hitIObj.GetComponent<Renderer>().material = highlightMaterial;
            selectedObjects.Add(hitIObj);
            
            Debug.Log("Entered Trigger: " + other.gameObject.name);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!HandPoseManager.GetComponent<HandPoseManager>().SecondSelectionState && 
            selectedObjects.Contains(other.gameObject) && other.gameObject.CompareTag("Target"))
        {
            other.gameObject.GetComponent<Renderer>().material = originalMaterials[other.gameObject]; // 恢复原始材质
            selectedObjects.Remove(other.gameObject);
            Debug.Log("Exited Trigger: " + other.gameObject.name);
        }
    }

}
