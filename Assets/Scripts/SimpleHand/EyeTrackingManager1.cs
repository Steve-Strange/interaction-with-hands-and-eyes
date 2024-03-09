﻿using UnityEngine;
using System.Collections.Generic;
using Unity.XR.PXR;
using UnityEngine.XR;
using TMPro;
using System.Linq;

public class EyeTrackingManager1 : MonoBehaviour
{   
    public Transform Origin;
    public GameObject Models;
    public Transform Greenpoint;
    public GameObject SpotLight;
    public GameObject rayVisualizer;
    // public TMP_Text t;
    // public TMP_Text t2;

    private Vector3 combineEyeGazeVector;
    private Vector3 combineEyeGazeOrigin;
    private Matrix4x4 headPoseMatrix;
    private Matrix4x4 originPoseMatrix;

    public Vector3 combineEyeGazeVectorInWorldSpace;
    public Vector3 combineEyeGazeOriginInWorldSpace;
    private float leftEyeOpenness;
    private float rightEyeOpenness;
    public GameObject GrabAgentObject;
    private GameObject SightCone;

    private float closeEyesTime = 0f;
    public bool isEyesOpen = true;
    

   /* public void AddOutline(GameObject target, Color color)
    {
        if (target.GetComponent<Outline>() == null)
        {
            target.AddComponent<Outline>();
            target.GetComponent<Outline>().OutlineColor = color;
        }
    }*/
    int mark;
    void Start()
    {
        mark = 0;
        combineEyeGazeVector = Vector3.zero;
        combineEyeGazeOrigin = Vector3.zero;
        originPoseMatrix = Origin.localToWorldMatrix;
        SightCone = GameObject.Find("SightCone");
       
    }

    void Update()
    {
        // t.text = closeEyesTime.ToString();
        PXR_EyeTracking.GetHeadPosMatrix(out headPoseMatrix);
        PXR_EyeTracking.GetCombineEyeGazeVector(out combineEyeGazeVector);
        PXR_EyeTracking.GetCombineEyeGazePoint(out combineEyeGazeOrigin);

        combineEyeGazeOriginInWorldSpace = originPoseMatrix.MultiplyPoint(headPoseMatrix.MultiplyPoint(combineEyeGazeOrigin));
        combineEyeGazeVectorInWorldSpace = originPoseMatrix.MultiplyVector(headPoseMatrix.MultiplyVector(combineEyeGazeVector));

        SpotLight.transform.position = combineEyeGazeOriginInWorldSpace;
        SpotLight.transform.rotation = Quaternion.LookRotation(combineEyeGazeVectorInWorldSpace, Vector3.up);
        SightCone.transform.position = combineEyeGazeOriginInWorldSpace;
        SightCone.transform.rotation = Quaternion.LookRotation(combineEyeGazeVectorInWorldSpace, Vector3.up);

        if( PXR_EyeTracking.GetLeftEyeGazeOpenness(out leftEyeOpenness) &&
            PXR_EyeTracking.GetRightEyeGazeOpenness(out rightEyeOpenness)) {
            isEyesOpen = leftEyeOpenness > 0.99f && rightEyeOpenness > 0.99f;
            if(isEyesOpen){
                if(closeEyesTime > 0.32f) BlinkSelect();
                closeEyesTime = 0;
            }
            else {
                closeEyesTime += Time.deltaTime;
            }
        }
    }

    
    void BlinkSelect(){

        if(rayVisualizer.GetComponent<RayVisualizer>().target!=null & mark == 0)
        {
            // t2.text = "yesselect";
            mark = 1;
            GrabAgentObject.SetActive(true);
            GrabAgentObject.GetComponent<GrabAgentObject1>().MovingObject.Add(rayVisualizer.GetComponent<RayVisualizer>().target);
        }

    }

    
}
