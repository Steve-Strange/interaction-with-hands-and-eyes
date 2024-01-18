using UnityEngine;
using System.Collections.Generic;
using Unity.XR.PXR;
using UnityEngine.XR;
using TMPro;
using System.Linq;

public class EyeTrackingManager : MonoBehaviour
{   
    public Transform Origin;
    public GameObject Models;
    public Transform Greenpoint;
    public GameObject SpotLight;

    public TMP_InputField Log;
    
    private Vector3 combineEyeGazeVector;
    private Vector3 combineEyeGazeOrigin;
    private Matrix4x4 headPoseMatrix;
    private Matrix4x4 originPoseMatrix;

    public Vector3 combineEyeGazeVectorInWorldSpace;
    public Vector3 combineEyeGazeOriginInWorldSpace;
    private float leftEyeOpenness;
    private float rightEyeOpenness;

    private Transform selectedObj;
    private RaycastHit hitInfo;

    private GameObject SightCone;
    private float coneAngle = 10f; // 圆锥的角度

    public GameObject HandPoseManager;
    public Material highlightMaterial;
    public GameObject eyeSelectedObject;
    public Material originalMaterial;


    private Queue<GameObject> eyeSelectedObjectBuffer = new Queue<GameObject>();
    private int maxBufferSize = 10; // 队列的最大大小
    
    public GameObject blinkSelectedObject;
    private float closeEyesTime = 0f;
    public bool isEyesOpen = true;
    
    void Start()
    {
        combineEyeGazeVector = Vector3.zero;
        combineEyeGazeOrigin = Vector3.zero;
        originPoseMatrix = Origin.localToWorldMatrix;
        HandPoseManager = GameObject.Find("HandPoseManager");
        SightCone = GameObject.Find("SightCone");
    }

    void Update()
    {
        PXR_EyeTracking.GetLeftEyeGazeOpenness(out leftEyeOpenness);
        PXR_EyeTracking.GetRightEyeGazeOpenness(out rightEyeOpenness);

        PXR_EyeTracking.GetHeadPosMatrix(out headPoseMatrix);
        PXR_EyeTracking.GetCombineEyeGazeVector(out combineEyeGazeVector);
        PXR_EyeTracking.GetCombineEyeGazePoint(out combineEyeGazeOrigin);

        combineEyeGazeOriginInWorldSpace = originPoseMatrix.MultiplyPoint(headPoseMatrix.MultiplyPoint(combineEyeGazeOrigin));
        combineEyeGazeVectorInWorldSpace = originPoseMatrix.MultiplyVector(headPoseMatrix.MultiplyVector(combineEyeGazeVector));

        SpotLight.transform.position = combineEyeGazeOriginInWorldSpace;
        SpotLight.transform.rotation = 
        Quaternion.LookRotation(combineEyeGazeVectorInWorldSpace, Vector3.up);
        SightCone.transform.position = combineEyeGazeOriginInWorldSpace;
        SightCone.transform.rotation = Quaternion.LookRotation(combineEyeGazeVectorInWorldSpace, Vector3.up);

        isEyesOpen = leftEyeOpenness > 0.01f && rightEyeOpenness > 0.01f;

        if(!HandPoseManager.GetComponent<HandPoseManager>().SecondSelectionState){
            if(isEyesOpen){
                GazeTargetControl(combineEyeGazeOriginInWorldSpace, combineEyeGazeVectorInWorldSpace);
                eyeSelectedObjectBuffer.Enqueue(eyeSelectedObject);
                if(eyeSelectedObjectBuffer.Count > maxBufferSize){
                    eyeSelectedObjectBuffer.Dequeue();  
                }

                closeEyesTime = 0;
            }
            else {
                closeEyesTime += Time.deltaTime;
            }

            if(closeEyesTime > 0.25f){
                BlinkSelect();
            }
        }
    }

    void GazeTargetControl(Vector3 origin,Vector3 vector)
    {
        int layerMask = 1;
        if(Physics.SphereCast(origin, 0.1f, vector, out hitInfo, 20f, layerMask)){
            Log.text += "eyeSelectedObject: " + eyeSelectedObject + "\n" + "hitInfo: " + hitInfo.collider.gameObject + "\n";

            if(hitInfo.collider.gameObject.tag == "Target")
            {
                if(eyeSelectedObject != hitInfo.collider.gameObject){
                    if(eyeSelectedObject)
                    {
                        if(SightCone.GetComponent<SightCone>().selectedObjects.Contains(eyeSelectedObject))
                        {
                            eyeSelectedObject.GetComponent<Renderer>().material = SightCone.GetComponent<SightCone>().highlightMaterial;
                        }
                        else
                        {
                            eyeSelectedObject.GetComponent<Renderer>().material = originalMaterial;
                        }
                    }
                    originalMaterial = hitInfo.collider.gameObject.GetComponent<Renderer>().material;
                    eyeSelectedObject = hitInfo.collider.gameObject;
                    eyeSelectedObject.GetComponent<Renderer>().material = highlightMaterial;
                }
            }

        }
            
    }


    void BlinkSelect(){
        
        blinkSelectedObject = FindMostFrequentElement(eyeSelectedObjectBuffer);
        blinkSelectedObject.SetActive(false);
    }

    GameObject FindMostFrequentElement(Queue<GameObject> list)
    {
        Dictionary<GameObject, int> frequencyMap = new Dictionary<GameObject, int>();

        // 统计每个元素的出现次数
        int i = 0;
        foreach (var element in list)
        {
            if(i < maxBufferSize/2){
                if (frequencyMap.ContainsKey(element))
                {
                    frequencyMap[element]++;
                }
                else
                {
                    frequencyMap[element] = 1;
                }
            }
            else break;
            i++;
        }

        // 找出出现次数最多的元素
        GameObject mostFrequentElement = frequencyMap.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;

        return mostFrequentElement;
    }
}
