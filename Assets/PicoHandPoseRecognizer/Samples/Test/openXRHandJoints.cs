using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.XR.PXR;

public class openXRHandJoints : MonoBehaviour
{

    public enum TrackType
    {
        Any,
        Left,
        Right
    }

    public TrackType trackType;
    public Transform tindex2;
    public Transform tindex3;
    public Transform tindex4;
    public Transform tmiddle2;
    public Transform tmiddle3;
    public Transform tmiddle4;

    private float duration;

    private GameObject leftWirst;
    public GameObject rightWirst;
    private Vector3 thumb0, thumb1, thumb2, thumb3;
    private Vector3 index0, index1, index2, index3, index4;
    private Vector3 middle0, middle1, middle2, middle3, middle4;
    private Vector3 ring0, ring1, ring2, ring3, ring4;
    private Vector3 pinky0, pinky1, pinky2, pinky3, pinky4;

    private bool thumbFlex, indexFlex, middleFlex, ringFlex, pinkyFlex;
    private bool thumbCurl, indexCurl, middleCurl, ringCurl, pinkyCurl;
    private bool thumbAbduc, indexAbduc, middleAbduc, ringAbduc, pinkyAbduc;

    private List<Vector3> leftJointPos = new List<Vector3>(new Vector3[(int)HandJoint.JointMax]);
    private List<Vector3> rightJointPos = new List<Vector3>(new Vector3[(int)HandJoint.JointMax]);
    private HandJointLocations leftHandJointLocations = new HandJointLocations();
    private HandJointLocations rightHandJointLocations = new HandJointLocations();

    private bool leftShapesVaild;
    private bool rightShapesVaild;
    private bool currentShapesVaild;
    private bool anyShapesVaild;

    private void Start()
    {
        //leftWirst = new GameObject();
        //rightWirst = new GameObject();
    }

    private void Update()
    {

        if (trackType == TrackType.Right || trackType == TrackType.Any)
        {
            PXR_HandTracking.GetJointLocations(HandType.HandRight, ref rightHandJointLocations);

            for (int i = 0; i < rightJointPos.Count; ++i)
            {
                if (rightHandJointLocations.jointLocations == null) break;

                rightJointPos[i] = rightHandJointLocations.jointLocations[i].pose.Position.ToVector3();

                if (i == (int)HandJoint.JointWrist)
                {
                    rightWirst.transform.SetPositionAndRotation(rightHandJointLocations.jointLocations[i].pose.Position.ToVector3(), rightHandJointLocations.jointLocations[i].pose.Orientation.ToQuat());
                }
            }
            rightShapesVaild = ShapesRecognizerCheck(rightJointPos, rightWirst.transform.right, rightWirst.transform.forward);

        }

        if (trackType == TrackType.Left || trackType == TrackType.Any)
        {
            PXR_HandTracking.GetJointLocations(HandType.HandLeft, ref leftHandJointLocations);

            for (int i = 0; i < leftJointPos.Count; ++i)
            {
                if (leftHandJointLocations.jointLocations == null) break;

                leftJointPos[i] = leftHandJointLocations.jointLocations[i].pose.Position.ToVector3();

                if (i == (int)HandJoint.JointWrist)
                {
                    leftWirst.transform.SetPositionAndRotation(leftHandJointLocations.jointLocations[i].pose.Position.ToVector3(), leftHandJointLocations.jointLocations[i].pose.Orientation.ToQuat());
                }
            }
            leftShapesVaild = ShapesRecognizerCheck(leftJointPos, leftWirst.transform.right, -leftWirst.transform.forward);

        }

        tindex2.position = index2;
        tindex3.position = index3;
        tindex4.position = thumb3;
        tmiddle2.position = middle2;
        tmiddle3.position = middle3;
        tmiddle4.position = middle4;

    DebugTest();
    }

    private float interval = 2f;
    private void DebugTest()
    {
        interval -= Time.deltaTime;
        if (interval < 0)
        {
            interval = 2f;
            Debug.Log("Shapes State : \n"
                + string.Format("FlexState : thumb:{0} index:{1} middle:{2} ring:{3} pinky:{4} \n", thumbFlex, indexFlex, middleFlex, ringFlex, pinkyFlex)
                + string.Format("CurlState : thumb:{0} index:{1} middle:{2} ring:{3} pinky:{4} \n", thumbCurl, indexCurl, middleCurl, ringCurl, pinkyCurl)
                + string.Format("AbducState : thumb:{0} index:{1} middle:{2} ring:{3} pinky:{4} \n", thumbAbduc, indexAbduc, middleAbduc, ringAbduc, pinkyAbduc)
                + string.Format("ShapesVaild : lefthand:{0} righthand:{1} anyhand:{2} \n", leftShapesVaild, rightShapesVaild, anyShapesVaild)
                );
        }
    }


    private bool ShapesRecognizerCheck(List<Vector3> jointPos, Vector3 wirstRight, Vector3 wirstForward)
    {
        thumb0 = jointPos[(int)HandJoint.JointThumbTip];
        thumb1 = jointPos[(int)HandJoint.JointThumbDistal];
        thumb2 = jointPos[(int)HandJoint.JointThumbProximal];
        thumb3 = jointPos[(int)HandJoint.JointThumbMetacarpal];

        index0 = jointPos[(int)HandJoint.JointIndexTip];
        index1 = jointPos[(int)HandJoint.JointIndexDistal];
        index2 = jointPos[(int)HandJoint.JointIndexIntermediate];
        index3 = jointPos[(int)HandJoint.JointIndexProximal];
        index4 = jointPos[(int)HandJoint.JointIndexMetacarpal];

        middle0 = jointPos[(int)HandJoint.JointMiddleTip];
        middle1 = jointPos[(int)HandJoint.JointMiddleDistal];
        middle2 = jointPos[(int)HandJoint.JointMiddleIntermediate];
        middle3 = jointPos[(int)HandJoint.JointMiddleProximal];
        middle4 = jointPos[(int)HandJoint.JointMiddleMetacarpal];

        ring0 = jointPos[(int)HandJoint.JointRingTip];
        ring1 = jointPos[(int)HandJoint.JointRingDistal];
        ring2 = jointPos[(int)HandJoint.JointRingIntermediate];
        ring3 = jointPos[(int)HandJoint.JointRingProximal];
        ring4 = jointPos[(int)HandJoint.JointRingMetacarpal];

        pinky0 = jointPos[(int)HandJoint.JointLittleTip];
        pinky1 = jointPos[(int)HandJoint.JointLittleDistal];
        pinky2 = jointPos[(int)HandJoint.JointLittleIntermediate];
        pinky3 = jointPos[(int)HandJoint.JointLittleProximal];
        pinky4 = jointPos[(int)HandJoint.JointLittleMetacarpal];



        return thumbFlex & indexFlex & middleFlex & ringFlex & pinkyFlex
               & thumbCurl & indexCurl & middleCurl & ringCurl & pinkyCurl
               & thumbAbduc & indexAbduc & middleAbduc & ringAbduc & pinkyAbduc;
    }
}
