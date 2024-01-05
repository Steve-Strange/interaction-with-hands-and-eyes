using System.Collections;
using System.Collections.Generic;
using Unity.XR.PXR;
using UnityEngine;
using UnityEngine.SpatialTracking;

public class PoseDetectVisualizer : MonoBehaviour
{
    Pose HMDPose;
    HandJointLocations rightHandJointLocations;
    [System.Serializable]
    class poseVisulizeGroup
    {
        public PXR_HandPose poseState;
        public ParticleSystem particle;
    }
    [SerializeField]
    private List<poseVisulizeGroup> PoseStates = new List<poseVisulizeGroup>();


    void Awake()
    {
        for(int i = 0; i < PoseStates.Count; i++)
        {
            int id = i;
            PoseStates[i].poseState.handPoseStart.AddListener(()=>poseEvent(id));
        }
    }

    void poseEvent(int id)
    {
        PXR_HandTracking.GetJointLocations(HandType.HandRight, ref rightHandJointLocations);
        Vector3 rightHandPos = rightHandJointLocations.jointLocations[(int)HandJoint.JointWrist].pose.Position.ToVector3();

        GetNodePoseData(UnityEngine.XR.XRNode.CenterEye, out HMDPose);

        transform.position = ((HMDPose.position + HMDPose.forward * (Vector3.Distance(HMDPose.position, rightHandPos) + 0.3f)) + rightHandPos) / 2 + Vector3.up * 0.05f;
        PoseStates[id].particle.gameObject.SetActive(true);
        PoseStates[id].particle.Play(true);
    }
    List<UnityEngine.XR.XRNodeState> nodeStates = new List<UnityEngine.XR.XRNodeState>();
    PoseDataFlags GetNodePoseData(UnityEngine.XR.XRNode node, out Pose resultPose)
    {
        PoseDataFlags retData = PoseDataFlags.NoData;
        UnityEngine.XR.InputTracking.GetNodeStates(nodeStates);
        foreach (UnityEngine.XR.XRNodeState nodeState in nodeStates)
        {
            if (nodeState.nodeType == node)
            {
                if (nodeState.TryGetPosition(out resultPose.position))
                {
                    retData |= PoseDataFlags.Position;
                }
                if (nodeState.TryGetRotation(out resultPose.rotation))
                {
                    retData |= PoseDataFlags.Rotation;
                }
                return retData;
            }
        }
        resultPose = Pose.identity;
        return retData;
    }


}
