using UnityEngine;
using System.Collections.Generic;
using Unity.XR.PXR;
using UnityEngine.XR;
using TMPro;
using System.Linq;

public class EyeTrackingManagerBareHand : MonoBehaviour
{   
    public Transform Origin;
    public GameObject rayVisualizer;
    public TMP_Text t;
    public GameObject singelSelect;
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
    public GameObject SightCone;

    private float closeEyesTime = 0f;
    public bool isEyesOpen = true;
    
    public int mark;
    void Start()
    {
        mark = 0;
        combineEyeGazeVector = Vector3.zero;
        combineEyeGazeOrigin = Vector3.zero;
        originPoseMatrix = Origin.localToWorldMatrix;   
    }

    void Update()
    {
        //t.text = closeEyesTime.ToString();
        PXR_EyeTracking.GetHeadPosMatrix(out headPoseMatrix);
        PXR_EyeTracking.GetCombineEyeGazeVector(out combineEyeGazeVector);
        PXR_EyeTracking.GetCombineEyeGazePoint(out combineEyeGazeOrigin);

        combineEyeGazeOriginInWorldSpace = originPoseMatrix.MultiplyPoint(headPoseMatrix.MultiplyPoint(combineEyeGazeOrigin));
        combineEyeGazeVectorInWorldSpace = originPoseMatrix.MultiplyVector(headPoseMatrix.MultiplyVector(combineEyeGazeVector));

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
    public void AddOutline(GameObject target, Color color)
    {
        if (target.GetComponent<Outline>() == null)
        {
            target.AddComponent<Outline>();
            target.GetComponent<Outline>().OutlineColor = color;
        }
    }

    void BlinkSelect(){

        if(rayVisualizer.GetComponent<RayVisualizer>().target!=null & mark == 0)
        {
            rayVisualizer.GetComponent<RayVisualizer>().setLine(0f);
            t.text = "yesselect";
            mark = 1;
            singelSelect.GetComponent<singleSelect>().selectOneObject();
            AddOutline(rayVisualizer.GetComponent<RayVisualizer>().target, Color.blue);
            GrabAgentObject.SetActive(true);
            GrabAgentObject.GetComponent<GrabAgentObjectBareHand>().MovingObject.Add(rayVisualizer.GetComponent<RayVisualizer>().target);
        }

    }

    
}
