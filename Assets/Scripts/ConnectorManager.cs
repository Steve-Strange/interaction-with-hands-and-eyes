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
    public Dictionary<GameObject, Vector3> vectorToCenter = new Dictionary<GameObject, Vector3>();

    public GameObject AgentObject;

   // public TMP_InputField log;

    // private LineRenderer lineRenderer;
    // public Color lineColor = Color.red; // 设置默认颜色
    // public float lineWidth = 0.1f; // 设置默认宽度
    public void getFrameCenter()
    {
        frameCenter = frame.GetComponent<frame>().center;
    }
    public void getOriginalOffset()
    {
        originalOffset = AgentObject.GetComponent<GrabAgentObject>().MovingObject[0].transform.position - AgentObject.GetComponent<GrabAgentObject>().MovingObject[1].transform.position;
    }
    void Start()
    {
        

        Objects = collide.GetComponent<collide>().onFrame;

        frameScale = new Vector3(1, 1, 1);

        foreach (var obj in Objects)
        {
            vectorToCenter[obj] = obj.transform.position - frameCenter;
        }

        // lineRenderer = GetComponent<LineRenderer>();
        // lineRenderer.startWidth = lineWidth;
        // lineRenderer.endWidth = lineWidth;

        // // 设置LineRenderer的颜色
        // lineRenderer.startColor = lineColor;
        // lineRenderer.endColor = lineColor;
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
}
