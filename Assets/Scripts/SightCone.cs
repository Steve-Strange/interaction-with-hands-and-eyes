using UnityEngine;
using System.Collections.Generic;
using System;
using TMPro;
using Unity.VisualScripting;
using System.Linq;
using System.Collections;


public class SightCone : MonoBehaviour
{
    private float coneAngle = 21f; // 圆锥的角度
    public Material highlightMaterial;
    public List<GameObject> selectedObjects = new List<GameObject>(); // 存储当前被高亮的物体
    private GameObject HandPoseManager;
    private GameObject EyeTrackingManager;
    private float MaxDepth = 20f;
    public TMP_InputField Log;
    private bool reFocus = true;
    private Queue<Vector3> orientationQueue = new Queue<Vector3>();
    private int reFoucsTime = 20;
    private float reFoucsThreshold = 10f;

    public Dictionary<GameObject, float> objectWeights = new Dictionary<GameObject, float>();



    void Start(){
        EyeTrackingManager = GameObject.Find("EyeTrackingManager");
        HandPoseManager = GameObject.Find("HandPoseManager");
        transform.localScale = new Vector3(MaxDepth * Mathf.Tan(coneAngle * Mathf.Deg2Rad),
                                MaxDepth * Mathf.Tan(coneAngle * Mathf.Deg2Rad), MaxDepth);

        StartCoroutine(UpdateObjectWeights());
    }
    void Update()
    {

    }
    IEnumerator UpdateObjectWeights()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            if(!HandPoseManager.GetComponent<HandPoseManager>().SecondSelectionState 
                && EyeTrackingManager.GetComponent<EyeTrackingManager>().isEyesOpen){
                // 获取视线中心位置
                foreach (GameObject obj in selectedObjects)
                {
                    // 计算物体到视线中心的距离
                    float distanceToLineOfSight = DistanceToLineOfSight(obj.transform.position, transform.position, 
                        EyeTrackingManager.GetComponent<EyeTrackingManager>().combineEyeGazeVectorInWorldSpace);
                    
                    objectWeights[obj] += Mathf.Exp(- distanceToLineOfSight * distanceToLineOfSight * 10) * 3;
                    if(obj.GetComponentInChildren<TextMeshPro>()){
                        obj.GetComponentInChildren<TextMeshPro>().fontSize = 5;
                        obj.GetComponentInChildren<TextMeshPro>().text = objectWeights[obj].ToShortString();
                    }
                    
                }
            }
        }
    }

    float DistanceToLineOfSight(Vector3 point, Vector3 linePoint, Vector3 lineDirection)
    {
        Vector3 pointToLinePoint = point - linePoint;
        float distance = (pointToLinePoint - Vector3.Project(pointToLinePoint, lineDirection)).magnitude;
        return distance;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!HandPoseManager.GetComponent<HandPoseManager>().SecondSelectionState && 
            !selectedObjects.Contains(other.gameObject) && other.gameObject.CompareTag("Target"))
        
        {
            // 新物体进入触发器
            GameObject hitObj = other.gameObject;
            if(!objectWeights.ContainsKey(hitObj)){
                objectWeights[hitObj] = 0f;
            }
            if(hitObj == EyeTrackingManager.GetComponent<EyeTrackingManager>().eyeSelectedObject) return;
            // hitObj.GetComponent<Renderer>().material = highlightMaterial;
            hitObj.AddComponent<Outline>();
            hitObj.GetComponent<Outline>().OutlineColor = Color.red;
            selectedObjects.Add(hitObj);
            
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!HandPoseManager.GetComponent<HandPoseManager>().SecondSelectionState && 
            selectedObjects.Contains(other.gameObject) && (other.gameObject.CompareTag("Target") || other.gameObject.CompareTag("FinalObject")))
        {
            other.GetComponent<Outline>().OutlineColor = Color.clear;
            selectedObjects.Remove(other.gameObject);
        }
    }
}
