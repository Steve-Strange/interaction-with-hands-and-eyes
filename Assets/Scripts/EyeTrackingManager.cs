using UnityEngine;
using System.Collections.Generic;
using Unity.XR.PXR;
using UnityEngine.XR;
using TMPro;
using System.Linq;
using Unity.VisualScripting;

public class EyeTrackingManager : MonoBehaviour
{
    public Transform Origin;
    private Vector3 combineEyeGazeVector;
    private Vector3 combineEyeGazeOrigin;
    private Matrix4x4 headPoseMatrix;
    private Matrix4x4 originPoseMatrix;
    public GameObject centerSphere;

    public Vector3 combineEyeGazeVectorInWorldSpace;
    public Vector3 combineEyeGazeOriginInWorldSpace;
    private float leftEyeOpenness;
    private float rightEyeOpenness;

    //private Transform selectedObj;
    private RaycastHit hitInfo;

    private GameObject SightCone;
    //private float coneAngle = 10f; // 圆锥的角度
    private GameObject HandPoseManager;
    public GameObject eyeSelectedObject;
    private Queue<GameObject> eyeSelectedObjectBuffer = new Queue<GameObject>();
    private Queue<Vector3> eyeTrackingPositionBuffer = new Queue<Vector3>();
    private int maxBufferSize = 10; // 队列的最大大小

    public GameObject blinkSelectedObject;
    private float closeEyesTimer = 0f;
    private float closeEyesTime = 0.35f;
    public bool isEyesOpen = true;
    private GameObject clickSelect;

    public GameObject FinalObjects;
    // public TMP_InputField log;
    public GameObject GrabAgentObject;
    public GameObject TimeRecorder;


    public void AddOutline(GameObject target, Color color)
    {
        if (target.GetComponent<Outline>() == null)
        {
            target.AddComponent<Outline>();
            target.GetComponent<Outline>().OutlineColor = color;
        }
    }

    void Start()
    {
        combineEyeGazeVector = Vector3.zero;
        combineEyeGazeOrigin = Vector3.zero;
        originPoseMatrix = Origin.localToWorldMatrix;
        TimeRecorder = GameObject.Find("TimeRecorder");
        HandPoseManager = GameObject.Find("HandPoseManager");
        SightCone = GameObject.Find("SightCone");
        clickSelect = GameObject.Find("clickSelect");
    }

    void Update()
    {
        PXR_EyeTracking.GetHeadPosMatrix(out headPoseMatrix);
        PXR_EyeTracking.GetCombineEyeGazeVector(out combineEyeGazeVector);
        PXR_EyeTracking.GetCombineEyeGazePoint(out combineEyeGazeOrigin);

        combineEyeGazeOriginInWorldSpace = originPoseMatrix.MultiplyPoint(headPoseMatrix.MultiplyPoint(combineEyeGazeOrigin));
        combineEyeGazeVectorInWorldSpace = originPoseMatrix.MultiplyVector(headPoseMatrix.MultiplyVector(combineEyeGazeVector));


        SightCone.transform.position = combineEyeGazeOriginInWorldSpace;
        SightCone.transform.rotation = Quaternion.LookRotation(combineEyeGazeVectorInWorldSpace, Vector3.up);
        // log.text = "origin: " + combineEyeGazeOriginInWorldSpace + "\n" + "vector: " + combineEyeGazeVectorInWorldSpace + "\n";
        // log.text += !HandPoseManager.GetComponent<HandPoseManager>().SecondSelectionState + " " + PXR_EyeTracking.GetLeftEyeGazeOpenness(out leftEyeOpenness) + " " + PXR_EyeTracking.GetRightEyeGazeOpenness(out rightEyeOpenness) + "\n";

        if (!HandPoseManager.GetComponent<HandPoseManager>().SecondSelectionState &&
            PXR_EyeTracking.GetLeftEyeGazeOpenness(out leftEyeOpenness) &&
            PXR_EyeTracking.GetRightEyeGazeOpenness(out rightEyeOpenness))
        {
            isEyesOpen = leftEyeOpenness > 0.99f && rightEyeOpenness > 0.99f;
            if (isEyesOpen)
            {
                if (closeEyesTimer > closeEyesTime) BlinkSelect();
                GazeTargetControl(combineEyeGazeOriginInWorldSpace, combineEyeGazeVectorInWorldSpace);
                eyeSelectedObjectBuffer.Enqueue(eyeSelectedObject);
                eyeTrackingPositionBuffer.Enqueue(centerSphere.transform.position);
                if (eyeSelectedObjectBuffer.Count > maxBufferSize)
                {
                    eyeSelectedObjectBuffer.Dequeue();
                    eyeTrackingPositionBuffer.Dequeue();
                }
                // log.text = "eyeSelectedObjectBuffer: " + eyeTrackingPositionBuffer.ToArray()[0] + "\n";
                closeEyesTimer = 0;
            }
            else
            {
                closeEyesTimer += Time.deltaTime;
            }
        }
    }

    void GazeTargetControl(Vector3 origin, Vector3 vector)
    {
        int layerMask = 1 << 0 | 1 << 7 | 1 << 8;
        if (Physics.SphereCast(origin, 0.1f, vector, out hitInfo, 50f, layerMask))
        {
            if(hitInfo.collider.gameObject.tag == "Structure") {
                centerSphere.transform.position = hitInfo.point - (combineEyeGazeOriginInWorldSpace - hitInfo.point).normalized * hitInfo.collider.gameObject.GetComponent<CapsuleCollider>().radius;
            }
            else {
                centerSphere.transform.position = hitInfo.point;
            }
            
            centerSphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f) * Vector3.Distance(combineEyeGazeOriginInWorldSpace, hitInfo.point) / 9;

            // log.text += "\n" + "hitInfo: " + hitInfo.collider.gameObject.name;
            
            if (HandPoseManager.GetComponent<HandPoseManager>().phase == 0 && hitInfo.collider.gameObject.tag == "Target")
            {
                if (eyeSelectedObject != hitInfo.collider.gameObject)
                {
                    if (eyeSelectedObject)
                    {
                        if (SightCone.GetComponent<SightCone>().selectedObjects.Contains(eyeSelectedObject))
                        {
                            eyeSelectedObject.GetComponent<Outline>().OutlineColor = Color.red;
                        }
                        else
                        {
                            eyeSelectedObject.GetComponent<Outline>().OutlineColor = Color.clear;
                        }

                    }
                    eyeSelectedObject = hitInfo.collider.gameObject;
                    eyeSelectedObject.GetComponent<Outline>().OutlineColor = Color.yellow;
                }
            }

        }

    }

    void BlinkSelect(){

        if(HandPoseManager.GetComponent<HandPoseManager>().phase == 0){
            blinkSelectedObject = FindMostFrequentElement(eyeSelectedObjectBuffer);
            if(!clickSelect.GetComponent<clickSelect>().FinalObjects.GetComponent<FinalObjects>().finalObj.Contains(blinkSelectedObject))
            {
                clickSelect.GetComponent<clickSelect>().FinalObjects.GetComponent<FinalObjects>().AddFinalObj(blinkSelectedObject);
                TimeRecorder.GetComponent<TimeRecorder>().writeFileContext += "allSelectionTime: " + Time.time + "\n";
            
            }
            if(SightCone.GetComponent<SightCone>().selectedObjects.Contains(blinkSelectedObject)){
                SightCone.GetComponent<SightCone>().selectedObjects.Remove(blinkSelectedObject);
            }

            var sightCone = SightCone.GetComponent<SightCone>();

            // 复制字典的键列表
            List<GameObject> keys = sightCone.objectWeights.Keys.ToList();

            foreach (var key in keys)
            {
                sightCone.objectWeights[key] = 0;
            }
        }
        else if(HandPoseManager.GetComponent<HandPoseManager>().phase == 1 && GrabAgentObject.GetComponent<GrabAgentObject>().MovingObject.Count == 0 && GrabAgentObject.GetComponent<GrabAgentObject>().movingStatus == false){
            Vector3 finalEyeTrackingPosition = new Vector3(0, 0, 0);
            for(int i = 0; i < maxBufferSize / 2; i++){
                finalEyeTrackingPosition.x += eyeTrackingPositionBuffer.ToArray()[i].x / (maxBufferSize / 2);
                finalEyeTrackingPosition.y += eyeTrackingPositionBuffer.ToArray()[i].y / (maxBufferSize / 2);
                finalEyeTrackingPosition.z += eyeTrackingPositionBuffer.ToArray()[i].z / (maxBufferSize / 2);
            }

            GameObject currnetObj = FinalObjects.GetComponent<FinalObjects>().finalObj[0];
            currnetObj.transform.position = finalEyeTrackingPosition;
            currnetObj.transform.parent = null;
            currnetObj.transform.localScale = HandPoseManager.GetComponent<HandPoseManager>().originalTransform[currnetObj].Scale;
            currnetObj.transform.rotation = HandPoseManager.GetComponent<HandPoseManager>().originalTransform[currnetObj].Rotation;

            FinalObjects.GetComponent<FinalObjects>().finalObj.RemoveAt(0);
            FinalObjects.GetComponent<FinalObjects>().RearrangeFinalObj();
            GrabAgentObject.GetComponent<GrabAgentObject>().MovingObject.Add(currnetObj);
        }
    }

    GameObject FindMostFrequentElement(Queue<GameObject> list)
    {
        Dictionary<GameObject, int> frequencyMap = new Dictionary<GameObject, int>();

        // 统计每个元素的出现次数
        int i = 0;
        foreach (var element in list)
        {
            if (i < maxBufferSize / 2)
            {
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
