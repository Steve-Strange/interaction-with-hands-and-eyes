using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConnectorManager : MonoBehaviour
{

    public GameObject frame;
    public GameObject collide;
    public Vector3 frameCenter;
    public Vector3 frameScale;

    private Vector3 originalOffset;
    private Vector3 newOffset;
    public List<GameObject> Objects = new List<GameObject>();
    public List<GameObject> emptyObjects = new List<GameObject>();
    public Dictionary<GameObject, Vector3> vectorToCenter = new Dictionary<GameObject, Vector3>();

    public TMP_Text T;
    public TMP_Text t1;
    public TMP_Text t2;
    public GameObject AgentObject;

    // public TMP_InputField log;

    // private LineRenderer lineRenderer;
    // public Color lineColor = Color.red; // 设置默认颜色
    // public float lineWidth = 0.1f; // 设置默认宽度
    void Start()
    {
        emptyObjects = new List<GameObject>();
    }
    void Update()
    {
       // log.text = "frameCenter: " + frameCenter + "\n" + "frameScale: " + frameScale + "\n" + "originalOffset: " + originalOffset + "\n" + "newOffset: " + newOffset;
        if(AgentObject.GetComponent<GrabAgentObject>().AutoAdjustStatus){
        
            if(AgentObject.GetComponent<GrabAgentObject>().FinishedObjects.Count == 1)
            {
                frameCenter = AgentObject.GetComponent<GrabAgentObject>().FinishedObjects[0].transform.position - vectorToCenter[AgentObject.GetComponent<GrabAgentObject>().FinishedObjects[0]];
            }
            else if(AgentObject.GetComponent<GrabAgentObject>().FinishedObjects.Count == 2)
            {
                newOffset = AgentObject.GetComponent<GrabAgentObject>().FinishedObjects[0].transform.position - AgentObject.GetComponent<GrabAgentObject>().FinishedObjects[1].transform.position;
                frameScale.x = originalOffset.x == 0 ? 0 : Mathf.Abs(newOffset.x/originalOffset.x);
                frameScale.y = originalOffset.y == 0 ? 0 : Mathf.Abs(newOffset.y/originalOffset.y);
                frameScale.z = originalOffset.z == 0 ? 0 : Mathf.Abs(newOffset.z/originalOffset.z);
                
                // frameScale = new Vector3(Mathf.Abs(newOffset.x/originalOffset.x), Mathf.Abs(newOffset.y/originalOffset.y), Mathf.Abs(newOffset.z/originalOffset.z));
                frameCenter = AgentObject.GetComponent<GrabAgentObject>().FinishedObjects[0].transform.position - 
                new Vector3(vectorToCenter[AgentObject.GetComponent<GrabAgentObject>().FinishedObjects[0]].x * frameScale.x ,vectorToCenter[AgentObject.GetComponent<GrabAgentObject>().FinishedObjects[0]].y * frameScale.y, vectorToCenter[AgentObject.GetComponent<GrabAgentObject>().FinishedObjects[0]].z * frameScale.z);
            }

            foreach (var obj in Objects)
            {
                obj.transform.position = frameCenter + new Vector3(vectorToCenter[obj].x * frameScale.x, vectorToCenter[obj].y * frameScale.y, vectorToCenter[obj].z * frameScale.z);
            }
            AgentObject.GetComponent<GrabAgentObject>().AutoAdjustStatus = false;
            frame.GetComponent<frame>().updateFrame();
        }
    }  
    public void reverse()
    {
        
        Objects = collide.GetComponent<collide>().onFrame;//得到框上所有物体信息
        emptyObjects.Clear();
        var cor = frame.GetComponent<frame>().cor;

        frameScale = new Vector3(1, 1, 1);//初始化

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

        foreach (var obj in Objects)
        {
            vectorToCenter[obj] = obj.transform.position - frame.GetComponent<frame>().center;//小框上的信息
        }

        frameCenter = frame.GetComponent<frame>().center + Vector3.forward * 2 - Vector3.up * 0.8f;//新的中心
        T.text = frameCenter.ToString();
        foreach (var obj in Objects){
            obj.transform.position = frameCenter + 20*rotate(vectorToCenter[obj],frame.GetComponent<frame>().right, 90);
            t2.text = frame.GetComponent<frame>().right.ToString();
            t1.text = rotate(vectorToCenter[obj], frame.GetComponent<frame>().right, 90).ToString();
            obj.transform.localScale *= 20;
        }//放置物体,恢复原来大小
        

        frame.GetComponent<frame>().updateFrame();//更新框
        
        //第三阶段初始化
        foreach (var obj in Objects)
        {
            vectorToCenter[obj] = obj.transform.position - frameCenter;
        }

        originalOffset = AgentObject.GetComponent<GrabAgentObject>().MovingObject[0].transform.position - AgentObject.GetComponent<GrabAgentObject>().MovingObject[1].transform.position;
    }

    public Vector3 rotate(Vector3 source, Vector3 axis, float angle)
    {
        Quaternion q = Quaternion.AngleAxis(angle, axis);// 旋转系数
        return q * source;// 返回目标点
    }

}
