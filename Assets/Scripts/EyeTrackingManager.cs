using UnityEngine;
using System.Collections.Generic;
using Unity.XR.PXR;
using UnityEngine.XR;
using TMPro;

public class EyeTrackingManager : MonoBehaviour
{
    public Transform Origin;
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
    public Material highlightMaterial;
    public List<GameObject> selectedObjects = new List<GameObject>(); // 存储当前被高亮的物体

    public Dictionary<GameObject, Material> originalMaterials = new Dictionary<GameObject, Material>();

    public GameObject HandPoseManager;

    // private Queue<Vector3> buffer = new Queue<Vector3>();
    // private int maxBufferSize = 20; // 队列的最大大小
    // private Vector3 blinkPosition = new Vector3(0,0,0);
    
    void Start()
    {
        combineEyeGazeVector = Vector3.zero;
        combineEyeGazeOrigin = Vector3.zero;
        originPoseMatrix = Origin.localToWorldMatrix;
        HandPoseManager = GameObject.Find("HandPoseManager");
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

            Log.text = hitinfo.transform.name;
            Log.text += hitinfo.point;

            if(!HandPoseManager.GetComponent<HandPoseManager>().SecondSelectionState){
                SelectObjectInCone(origin, vector);
            }
            
        }
    }

    void SelectObjectInCone(Vector3 origin, Vector3 direction)
    {
        // 清除先前的高亮
        foreach (var obj in selectedObjects)
        {
            if (obj != null && originalMaterials.ContainsKey(obj))
            {
                obj.GetComponent<Renderer>().material = originalMaterials[obj]; // 恢复原始材质
            }
        }
        selectedObjects.Clear();
        originalMaterials.Clear(); // 清空原始材质字典

        // 发射多条射线以模拟圆锥
        int maxRayCount = 3600;
        float maxDistance = 100f;
        for (int i = 0; i < maxRayCount; i++)
        {

            Vector3 perpendicular = Vector3.Cross(direction, Vector3.up);

            Quaternion rotation = Quaternion.AngleAxis(i%180, direction);
            Vector3 rotatedVector = rotation * perpendicular;

            // 在圆锥内随机方向
            Vector3 randomDirection = direction.normalized + rotatedVector.normalized * Mathf.Tan(coneAngle * Mathf.Deg2Rad) * (i/180)/(maxRayCount/180);
            RaycastHit[] hits = Physics.RaycastAll(origin, randomDirection.normalized, maxDistance);
            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.CompareTag("Target"))
                {
                    GameObject hitIObj = hit.collider.gameObject;
                    if (hitIObj != null && !selectedObjects.Contains(hitIObj))
                    {
                        // 在更改材质之前存储原始材质
                        if (!originalMaterials.ContainsKey(hitIObj))
                        {
                            originalMaterials[hitIObj] = hitIObj.GetComponent<Renderer>().material;
                        }

                        hitIObj.GetComponent<Renderer>().material = highlightMaterial;
                        selectedObjects.Add(hitIObj);
                    }
                }
            }
        }
    }

}
