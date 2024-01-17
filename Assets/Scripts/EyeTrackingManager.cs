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
    private RaycastHit hitinfo;

    private GameObject SightCone;
    private float coneAngle = 10f; // 圆锥的角度
    public Material highlightMaterial;

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
        SpotLight.transform.rotation = Quaternion.LookRotation(combineEyeGazeVectorInWorldSpace, Vector3.up);
        SightCone.transform.position = combineEyeGazeOriginInWorldSpace;
        SightCone.transform.rotation = Quaternion.LookRotation(combineEyeGazeVectorInWorldSpace, Vector3.up);

    //    GazeTargetControl(combineEyeGazeOriginInWorldSpace, combineEyeGazeVectorInWorldSpace);
    }

    void GazeTargetControl(Vector3 origin,Vector3 vector)
    {
        if (Physics.SphereCast(origin,0.0005f,vector,out hitinfo))
        {
            Greenpoint.gameObject.SetActive(true);
            Greenpoint.position = hitinfo.point;
            
        }
    }


}
