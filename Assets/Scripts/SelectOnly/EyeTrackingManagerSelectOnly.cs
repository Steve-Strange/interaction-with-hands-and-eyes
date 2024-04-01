using UnityEngine;
using System.Collections.Generic;
using Unity.XR.PXR;
using UnityEngine.XR;
using TMPro;
using System.Linq;

public class EyeTrackingManagerSelectOnly : MonoBehaviour
{
    public Transform Origin;
    public GameObject Models;

    private Vector3 combineEyeGazeVector;
    private Vector3 combineEyeGazeOrigin;
    private Matrix4x4 headPoseMatrix;
    private Matrix4x4 originPoseMatrix;

    public Vector3 combineEyeGazeVectorInWorldSpace;
    public Vector3 combineEyeGazeOriginInWorldSpace;
    private float leftEyeOpenness;
    private float rightEyeOpenness;

    //private Transform selectedObj;
    private RaycastHit hitInfo;

    private GameObject SightCone;
    //private float coneAngle = 10f; // 圆锥的角度

    public GameObject HandPoseManager;
    public Material highlightMaterial;
    public GameObject eyeSelectedObject;
    public Material originalMaterial;

    public GameObject FinalObjects;

    private Queue<GameObject> eyeSelectedObjectBuffer = new Queue<GameObject>();
    private int maxBufferSize = 10; // 队列的最大大小

    public GameObject blinkSelectedObject;
    private float closeEyesTime = 0f;
    public bool isEyesOpen = true;
    private GameObject clickSelect;


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

        if (!HandPoseManager.GetComponent<HandPoseManagerSelectOnly>().SecondSelectionState &&
            PXR_EyeTracking.GetLeftEyeGazeOpenness(out leftEyeOpenness) &&
            PXR_EyeTracking.GetRightEyeGazeOpenness(out rightEyeOpenness))
        {
            isEyesOpen = leftEyeOpenness > 0.99f && rightEyeOpenness > 0.99f;
            if (isEyesOpen)
            {
                if (closeEyesTime > 0.32f) BlinkSelect();
                GazeTargetControl(combineEyeGazeOriginInWorldSpace, combineEyeGazeVectorInWorldSpace);
                eyeSelectedObjectBuffer.Enqueue(eyeSelectedObject);
                if (eyeSelectedObjectBuffer.Count > maxBufferSize)
                {
                    eyeSelectedObjectBuffer.Dequeue();
                }
                closeEyesTime = 0;
            }
            else
            {
                closeEyesTime += Time.deltaTime;
            }
        }
    }

    void GazeTargetControl(Vector3 origin, Vector3 vector)
    {
        int layerMask = 1 << 7;
        if (Physics.SphereCast(origin, 0.12f, vector, out hitInfo, 50f, layerMask))
        {
            if (hitInfo.collider.gameObject.tag == "Target")
            {
                if (eyeSelectedObject != hitInfo.collider.gameObject)
                {
                    if (eyeSelectedObject)
                    {
                        if (SightCone.GetComponent<SightConeSelectOnly>().selectedObjects.Contains(eyeSelectedObject))
                        {
                            eyeSelectedObject.GetComponent<Outline>().OutlineColor = Color.red;
                        }
                        else
                        {
                            eyeSelectedObject.GetComponent<Outline>().OutlineColor = Color.clear;
                        }

                    }
                    eyeSelectedObject = hitInfo.collider.gameObject;
                    eyeSelectedObject.GetComponent<Outline>().OutlineColor = Color.green;
                }
            }

        }

    }


    void BlinkSelect()
    {
        blinkSelectedObject = FindMostFrequentElement(eyeSelectedObjectBuffer);

        clickSelect.GetComponent<clickSelectSelectOnly>().FinalObjects.GetComponent<FinalObjectsSelectOnly>().AddFinalObj(blinkSelectedObject);

        if (SightCone.GetComponent<SightConeSelectOnly>().selectedObjects.Contains(blinkSelectedObject))
        {
            SightCone.GetComponent<SightConeSelectOnly>().selectedObjects.Remove(blinkSelectedObject);
        }

        var sightCone = SightCone.GetComponent<SightConeSelectOnly>();

        // 复制字典的键列表
        List<GameObject> keys = sightCone.objectWeights.Keys.ToList();

        foreach (var key in keys)
        {
            sightCone.objectWeights[key] = 0;
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
