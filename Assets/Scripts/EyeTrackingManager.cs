using UnityEngine;
using System.Collections.Generic;
using Unity.XR.PXR;
using UnityEngine.XR;
using TMPro;

public class EyeTrackingManager : MonoBehaviour
{
    public Transform Origin;
    public GameObject EyeCoordinates;
    public GameObject Models;
    public Transform Greenpoint;
    public GameObject SpotLight;
    public TMP_Text Log;
    
    private Vector3 combineEyeGazeVector;
    private Vector3 combineEyeGazeOrigin;
    private Matrix4x4 headPoseMatrix;
    private Matrix4x4 originPoseMatrix;

    private Vector3 combineEyeGazeVectorInWorldSpace;
    private Vector3 combineEyeGazeOriginInWorldSpace;
    private float leftEyeOpenness;
    private float rightEyeOpenness;

    private Transform selectedObj;
    private RaycastHit hitinfo;

    private float coneAngle = 10f; // 圆锥的角度
    public Material highlightMaterial; // 高亮物体时使用的材质
    public Material originalMaterial; // 原始材质
    private List<Renderer> highlightedObjects = new List<Renderer>(); // 存储当前被高亮的物体

    private Dictionary<Renderer, Material> originalMaterials = new Dictionary<Renderer, Material>();


    // private Queue<Vector3> buffer = new Queue<Vector3>();
    // private int maxBufferSize = 20; // 队列的最大大小
    // private Vector3 blinkPosition = new Vector3(0,0,0);
    
    void Start()
    {
        combineEyeGazeVector = Vector3.zero;
        combineEyeGazeOrigin = Vector3.zero;
        originPoseMatrix = Origin.localToWorldMatrix;

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
        SpotLight.transform.rotation = Quaternion.LookRotation(combineEyeGazeVectorInWorldSpace, Vector3.up);

        GazeTargetControl(combineEyeGazeOriginInWorldSpace, combineEyeGazeVectorInWorldSpace);
    }

    void GazeTargetControl(Vector3 origin,Vector3 vector)
    {
        if (Physics.SphereCast(origin,0.0005f,vector,out hitinfo))
        {

            Greenpoint.gameObject.SetActive(true);
            Greenpoint.position = hitinfo.point;

            // buffer.Enqueue(hitinfo.point);

            // if (buffer.Count > maxBufferSize)
            // {
            //     buffer.Dequeue();
            // }

            // if(leftEyeOpenness < 0.1f && rightEyeOpenness < 0.1f)
            // {
            //     int i = 0;
            //     int sum = 0;
            //     blinkPosition = new Vector3(0,0,0);

            //     foreach (Vector3 point in buffer)
            //     {
            //         i++;
            //         blinkPosition += point * Mathf.Min(i, maxBufferSize + 1 - i);
            //         sum += Mathf.Min(i, maxBufferSize + 1 - i);
            //     }

            //     blinkPosition /= sum;
            //     Greenpoint.position = blinkPosition;
            // }

            Log.text = hitinfo.transform.name;
            Log.text += hitinfo.point;

            HighlightObjectsInCone(origin, vector);

            if (selectedObj != null && selectedObj != hitinfo.transform)
            {
                if(selectedObj.GetComponent<ETObject>()!=null)
                    selectedObj.GetComponent<ETObject>().UnFocused();
                selectedObj = null;
            }
            else if (selectedObj == null)
            {
                selectedObj = hitinfo.transform;
                if (selectedObj.GetComponent<ETObject>() != null)
                    selectedObj.GetComponent<ETObject>().IsFocused();
            }

        }
        else
        {
            if (selectedObj != null)
            {
               if (selectedObj.GetComponent<ETObject>() != null)
                    selectedObj.GetComponent<ETObject>().UnFocused();
                selectedObj = null;
            }
            Greenpoint.gameObject.SetActive(false);
        }

    }

    void HighlightObjectsInCone(Vector3 origin, Vector3 direction)
    {
        // 清除先前的高亮
        foreach (var renderer in highlightedObjects)
        {
            if (renderer != null && originalMaterials.ContainsKey(renderer))
            {
                renderer.material = originalMaterials[renderer]; // 恢复原始材质
            }
        }
        highlightedObjects.Clear();
        originalMaterials.Clear(); // 清空原始材质字典

        // 发射多条射线以模拟圆锥
        int maxRayCount = 7200;
        float maxDistance = 100f;
        for (int i = 0; i < maxRayCount; i++)
        {

            Vector3 perpendicular = Vector3.Cross(direction, Vector3.up);

            Quaternion rotation = Quaternion.AngleAxis(i%360, direction);
            Vector3 rotatedVector = rotation * perpendicular;

            // 在圆锥内随机方向
            Vector3 randomDirection = direction.normalized + rotatedVector.normalized * Mathf.Tan(coneAngle * Mathf.Deg2Rad) * (i/360)/(maxRayCount/360);
            RaycastHit[] hits = Physics.RaycastAll(origin, randomDirection.normalized, maxDistance);
            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.CompareTag("Target"))
                {
                    Renderer renderer = hit.collider.GetComponent<Renderer>();
                    if (renderer != null && !highlightedObjects.Contains(renderer))
                    {
                        // 在更改材质之前存储原始材质
                        if (!originalMaterials.ContainsKey(renderer))
                        {
                            originalMaterials[renderer] = renderer.material;
                        }

                        renderer.material = highlightMaterial;
                        highlightedObjects.Add(renderer);
                    }
                }
            }
        }
    }

}
