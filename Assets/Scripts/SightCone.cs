using UnityEngine;
using System.Collections.Generic;
using System;
using TMPro;
using Unity.VisualScripting;
using System.Linq;

public class WeightedObject
{
    public GameObject obj;
    public float weight;

    public WeightedObject(GameObject obj, float weight)
    {
        this.obj = obj;
        this.weight = weight;
    }
}

public class SightCone : MonoBehaviour
{
    private float coneAngle = 20f; // 圆锥的角度
    public Material highlightMaterial;
    public List<GameObject> selectedObjects = new List<GameObject>(); // 存储当前被高亮的物体
    public Dictionary<GameObject, Material> originalMaterials = new Dictionary<GameObject, Material>();
    private GameObject HandPoseManager;
    private GameObject EyeTrackingManager;
    private float MaxDepth = 20f;

    public TMP_InputField Log;
    private bool reFocus = true;
    private Queue<Vector3> orientationQueue = new Queue<Vector3>();
    private int reFoucsTime = 20;
    private float reFoucsThreshold = 10f;
    public List<WeightedObject> weightedObjects = new List<WeightedObject>();

    void Start(){
        EyeTrackingManager = GameObject.Find("EyeTrackingManager");
        HandPoseManager = GameObject.Find("HandPoseManager");
        transform.localScale = new Vector3(transform.localScale.z * Mathf.Tan(coneAngle * Mathf.Deg2Rad),
                                transform.localScale.z * Mathf.Tan(coneAngle * Mathf.Deg2Rad), transform.localScale.z);

    }

    void Update()
    {
        // orientationQueue.Enqueue(EyeTrackingManager.GetComponent<EyeTrackingManager>().combineEyeGazeVectorInWorldSpace);
        // if(orientationQueue.Count > reFoucsTime){
        //     orientationQueue.Dequeue();
        // }

        // gameObject.GetComponent<Light>().spotAngle = coneAngle;
        // float density = selectedObjects.Count / coneAngle;
        // if(coneAngle <= 27f && selectedObjects.Count <= 25 && density <= 0.3){
        //     coneAngle += 3f;
        //     transform.localScale = new Vector3(transform.localScale.z * Mathf.Tan(coneAngle * Mathf.Deg2Rad),
        //                         transform.localScale.z * Mathf.Tan(coneAngle * Mathf.Deg2Rad), transform.localScale.z);
        // }
        // else if(coneAngle >= 18f && density >= 0.5){
        //     coneAngle -= 3f;
        //     transform.localScale = new Vector3(transform.localScale.z * Mathf.Tan(coneAngle * Mathf.Deg2Rad),
        //                         transform.localScale.z * Mathf.Tan(coneAngle * Mathf.Deg2Rad), transform.localScale.z);
        // }
        // Log.text = "coneAngle: " + coneAngle + "\n" + "selectedObjects.Count: " + selectedObjects.Count + "\n" + "density: " + density;
        
        // ReFocus();

        foreach (var obj in selectedObjects)
        {
            
        }
    }

    float DistanceToLineOfSight(Vector3 point, Vector3 linePoint, Vector3 lineDirection)
    {
        Vector3 pointToLinePoint = point - linePoint;
        float distance = (pointToLinePoint - Vector3.Project(pointToLinePoint, lineDirection)).magnitude;
        return distance;
    }

    void ReFocus()
    {
        Vector3 previousOrientation = new Vector3(0,0,0);
        Vector3 currentOrientation = new Vector3(0,0,0);
        int i = 0;
        foreach (var orientation in orientationQueue)
        {
            if(i < 2f/reFoucsTime) previousOrientation += orientation * (2f/reFoucsTime);
            else currentOrientation += orientation * (2f/reFoucsTime);
            i++;
        }

        if (Vector3.Angle(previousOrientation, currentOrientation) > reFoucsThreshold && !reFocus && !HandPoseManager.GetComponent<HandPoseManager>().SecondSelectionState)
        {
            weightedObjects.Clear();
            reFocus = true;
            orientationQueue.Clear();
            if(Log.text.Length > 200) Log.text = "";
            Log.text += "\n" + "reFocus: " + reFocus + "  " + Time.time;

            // Calculate weights for each selected object based on distance from the center
            foreach (var obj in selectedObjects)
            {
                float distanceToLineOfSight = DistanceToLineOfSight(obj.transform.position, transform.position, EyeTrackingManager.GetComponent<EyeTrackingManager>().combineEyeGazeVectorInWorldSpace);
                weightedObjects.Add(new WeightedObject(obj, distanceToLineOfSight));

                obj.GetComponentInChildren<TextMeshPro>().fontSize = 5;
                obj.GetComponentInChildren<TextMeshPro>().text = distanceToLineOfSight.ToString();

            }

            // Sort the objects based on weights (ascending order)
            weightedObjects.Sort((a, b) => a.weight.CompareTo(b.weight));
            selectedObjects.Clear();

            // Perform actions for each object in the sorted order
            foreach (var weightedObj in weightedObjects)
            {
                selectedObjects.Add(weightedObj.obj);
            }

            orientationQueue.Clear();

            if (Log.text.Length > 150) Log.text = "";
            Log.text += "\n" + "reFocus: " + reFocus + "  " + Time.time;
        }
        else
        {
            reFocus = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!HandPoseManager.GetComponent<HandPoseManager>().SecondSelectionState && 
            !selectedObjects.Contains(other.gameObject) && other.gameObject.CompareTag("Target"))
        
        {
            // 新物体进入触发器
            GameObject hitObj = other.gameObject;

            if(hitObj == EyeTrackingManager.GetComponent<EyeTrackingManager>().eyeSelectedObject) return;

            // 在更改材质之前存储原始材质
            if (!originalMaterials.ContainsKey(hitObj))
            {
                originalMaterials[hitObj] = hitObj.GetComponent<Renderer>().material;
            }

            hitObj.GetComponent<Renderer>().material = highlightMaterial;
            // if(!reFocus) selectedObjects.Insert(0, hitObj);
            // else selectedObjects.Add(hitObj);

            selectedObjects.Add(hitObj);
            
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!HandPoseManager.GetComponent<HandPoseManager>().SecondSelectionState && 
            selectedObjects.Contains(other.gameObject) && other.gameObject.CompareTag("Target"))
        {
            other.gameObject.GetComponent<Renderer>().material = originalMaterials[other.gameObject]; // 恢复原始材质
            selectedObjects.Remove(other.gameObject);

        }
    }

}
