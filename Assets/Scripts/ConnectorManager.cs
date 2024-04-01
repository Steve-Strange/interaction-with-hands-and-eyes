using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class ConnectorManager : MonoBehaviour
{

    public GameObject frame;
    public GameObject collide;
    public Vector3 frameCenter;
    public Vector3 frameScale;
    private GameObject HandPoseManager;

    private Vector3 originalOffset1, originalOffset2;
    private Vector3 newOffset;
    public List<GameObject> Objects = new List<GameObject>();
    public List<GameObject> emptyObjects = new List<GameObject>();
    public Dictionary<GameObject, Vector3> vectorToCenter = new Dictionary<GameObject, Vector3>();

    // public TMP_Text T;
    // public TMP_Text t1;
    // public TMP_Text t2;
    public GameObject AgentObject;
    private Quaternion FrameRotation;
    public GameObject TimeRecorder;
    public GameObject frameAgent;

    private Dictionary<GameObject, GameObject> TargetObjects = new Dictionary<GameObject, GameObject>();
    public List<GameObject> newObjects = new List<GameObject>();

    // public TMP_InputField log;

    // private LineRenderer lineRenderer;
    // public Color lineColor = Color.red; // 设置默认颜色
    // public float lineWidth = 0.1f; // 设置默认宽度
    void Start()
    {
        emptyObjects = new List<GameObject>();
        HandPoseManager = GameObject.Find("HandPoseManager");
    }
    void Update()
    {
    //     log.text = "frameCenter: " + frameCenter + "\n" + "frameScale: " + frameScale + "\n" + "frameRotation: " + FrameRotation;
        if(AgentObject.GetComponent<GrabAgentObject>().AutoAdjustStatus){

            if(AgentObject.GetComponent<GrabAgentObject>().FinishedObjects.Count == 1)
            {
                frameCenter = AgentObject.GetComponent<GrabAgentObject>().FinishedObjects[0].transform.position - vectorToCenter[AgentObject.GetComponent<GrabAgentObject>().FinishedObjects[0]];
            }
            else if(AgentObject.GetComponent<GrabAgentObject>().FinishedObjects.Count == 2){
                newOffset = AgentObject.GetComponent<GrabAgentObject>().FinishedObjects[0].transform.position - AgentObject.GetComponent<GrabAgentObject>().FinishedObjects[1].transform.position;
                FrameRotation = Quaternion.FromToRotation(originalOffset1, newOffset).normalized;
                frameScale *= newOffset.magnitude / originalOffset1.magnitude;
                // Vector3 rotatedVectorToCenter = FrameRotation * vectorToCenter[AgentObject.GetComponent<GrabAgentObject>().FinishedObjects[0]];
                // frameCenter = AgentObject.GetComponent<GrabAgentObject>().FinishedObjects[0].transform.position - 
                //  new Vector3(rotatedVectorToCenter.x * frameScale.x ,rotatedVectorToCenter.y * frameScale.y, rotatedVectorToCenter.z * frameScale.z);

                frameCenter = AgentObject.GetComponent<GrabAgentObject>().FinishedObjects[0].transform.position - FrameRotation * 
                 new Vector3(vectorToCenter[AgentObject.GetComponent<GrabAgentObject>().FinishedObjects[0]].x * frameScale.x,
                            vectorToCenter[AgentObject.GetComponent<GrabAgentObject>().FinishedObjects[0]].y * frameScale.y,
                            vectorToCenter[AgentObject.GetComponent<GrabAgentObject>().FinishedObjects[0]].z * frameScale.z);
            }
            else if(AgentObject.GetComponent<GrabAgentObject>().FinishedObjects.Count == 3){
                newOffset = Quaternion.Inverse(FrameRotation).normalized * (AgentObject.GetComponent<GrabAgentObject>().FinishedObjects[0].transform.position - AgentObject.GetComponent<GrabAgentObject>().FinishedObjects[2].transform.position);
                frameScale.x = originalOffset2.x == 0 ? 0 : Mathf.Abs(newOffset.x/originalOffset2.x);
                frameScale.y = originalOffset2.y == 0 ? 0 : Mathf.Abs(newOffset.y/originalOffset2.y);
                frameScale.z = originalOffset2.z == 0 ? 0 : Mathf.Abs(newOffset.z/originalOffset2.z);

                // Vector3 rotatedVectorToCenter = FrameRotation * vectorToCenter[AgentObject.GetComponent<GrabAgentObject>().FinishedObjects[0]];
                // frameCenter = AgentObject.GetComponent<GrabAgentObject>().FinishedObjects[0].transform.position - 
                //  new Vector3(rotatedVectorToCenter.x * frameScale.x ,rotatedVectorToCenter.y * frameScale.y, rotatedVectorToCenter.z * frameScale.z);

                frameCenter = AgentObject.GetComponent<GrabAgentObject>().FinishedObjects[0].transform.position - FrameRotation * 
                 new Vector3(vectorToCenter[AgentObject.GetComponent<GrabAgentObject>().FinishedObjects[0]].x * frameScale.x,
                            vectorToCenter[AgentObject.GetComponent<GrabAgentObject>().FinishedObjects[0]].y * frameScale.y,
                            vectorToCenter[AgentObject.GetComponent<GrabAgentObject>().FinishedObjects[0]].z * frameScale.z);
            }

            // foreach (var obj in Objects)
            // {
            //     Vector3 rotatedVectorToCenter = FrameRotation * vectorToCenter[obj];
            //     obj.transform.position = frameCenter + new Vector3(rotatedVectorToCenter.x * frameScale.x ,rotatedVectorToCenter.y * frameScale.y, rotatedVectorToCenter.z * frameScale.z);
            // }

            foreach (var obj in Objects)
            {
                obj.transform.position = frameCenter + FrameRotation * new Vector3(vectorToCenter[obj].x * frameScale.x, vectorToCenter[obj].y * frameScale.y, vectorToCenter[obj].z * frameScale.z);
            }


            // foreach (var obj in collide.GetComponent<collide>().onFrame)
            // {
            //      if (Vector3.Distance(obj.transform.position, TargetObjects[obj].transform.position) < (obj.transform.GetComponent<Renderer>().bounds.size.x + obj.transform.GetComponent<Renderer>().bounds.size.y + 
            //         obj.transform.GetComponent<Renderer>().bounds.size.z) / 6f &&
            //     Vector3.Distance(obj.transform.eulerAngles, TargetObjects[obj].transform.eulerAngles) < 90f)
            //     {
            //         obj.transform.position = TargetObjects[obj].transform.position;
            //         obj.transform.eulerAngles = TargetObjects[obj].transform.eulerAngles;
            //     }
            // }


            TargetObjects = AgentObject.GetComponent<GrabAgentObject>().TargetObjects;
            AgentObject.GetComponent<GrabAgentObject>().AutoAdjustStatus = false;
            frame.GetComponent<frame>().updateFrame();
        }
    }
    public void reverse()
    {
        Objects = new List<GameObject>(collide.GetComponent<collide>().onFrame);
        foreach (var obj in Objects)
        {
            TimeRecorder.GetComponent<TimeRecorder>().MovingObjectStatus.Add(obj, 0);
            obj.GetComponent<Outline>().outlineColor = Color.clear;
        }

        emptyObjects.Clear();
        var cor = frame.GetComponent<frame>().cor;

        frameScale = new Vector3(1, 1, 1);//初始化

        foreach (var obj in Objects)
        {
            obj.transform.parent = null;
            GameObject newObj = Instantiate(obj, obj.transform.position, obj.transform.rotation);
            newObj.tag = "AgentObject";
            newObj.name = obj.name + " Agent";
            newObjects.Add(newObj);
            Objects.Remove(newObj);
        }
        frameAgent = Instantiate(frame, frame.transform.position, frame.transform.rotation);
        frameAgent.name = "frameAgent";
        frameAgent.GetComponent<frame>().enabled = false;

        if(frame.GetComponent<frame>().Frame == "rect")
        {
            for(int i = 0; i <= 3; i++)
            {
                Objects.Add(cor[i]);
                emptyObjects.Add(cor[i]);
            }
        }else if(frame.GetComponent<frame>().Frame == "tri")
        {
            for(int i = 0; i <= 2; i++)
            {
                Objects.Add(cor[i]);
                emptyObjects.Add(cor[i]);
            }
        }else if(frame.GetComponent<frame>().Frame == "pen")
        {
            for(int i = 0; i <= 4; i++)
            {
                Objects.Add(cor[i]);
                emptyObjects.Add(cor[i]);
            }
        }else if(frame.GetComponent<frame>().Frame == "para")
        {
            for(int i = 0; i <= 3; i++)
            {
                Objects.Add(cor[i]);
                emptyObjects.Add(cor[i]);
            }

        }
        else if (frame.GetComponent<frame>().Frame == "cube")
        {
            for (int i = 0; i <= 7; i++)
            {
                Objects.Add(cor[i]);
                emptyObjects.Add(cor[i]);
            }

        }

        frameCenter = frame.GetComponent<frame>().center + Vector3.forward * 2 ;//新的中心
        var temp = frame.GetComponent<frame>().objSize;
        float  scale = 10f * temp;//小框架变成大框架之后放大的倍数
        foreach (var obj in Objects){
            if(!obj.CompareTag("AgentObject"))
            {
                vectorToCenter[obj] = obj.transform.position - frame.GetComponent<frame>().center;//小框上的信息
                obj.transform.position = frameCenter + scale * vectorToCenter[obj]; 
                obj.transform.parent = null;
                obj.transform.localScale = HandPoseManager.GetComponent<HandPoseManager>().objScale[obj];
                vectorToCenter[obj] = obj.transform.position - frame.GetComponent<frame>().center;//小框上的信息
            }
        }//放置物体,恢复原来大小


        frame.GetComponent<frame>().updateFrame();//更新框

        originalOffset1 = AgentObject.GetComponent<GrabAgentObject>().MovingObject[0].transform.position - AgentObject.GetComponent<GrabAgentObject>().MovingObject[1].transform.position;
        originalOffset2 = AgentObject.GetComponent<GrabAgentObject>().MovingObject[0].transform.position - AgentObject.GetComponent<GrabAgentObject>().MovingObject[2].transform.position;

    }

    public Vector3 rotate(Vector3 source, Vector3 axis, float angle)
    {
        Quaternion q = Quaternion.AngleAxis(angle, axis);// 旋转系数
        return q * source;// 返回目标点
    }

}
