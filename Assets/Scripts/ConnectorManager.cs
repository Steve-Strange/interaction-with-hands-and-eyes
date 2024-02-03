using System.Collections.Generic;
using UnityEngine;

public class ConnectorManager : MonoBehaviour
{
    public GameObject cube1;
    public GameObject cube2;
    public GameObject cube3;
    public GameObject cube4;

    public Vector3 frameCenter;
    public Vector3 frameScale;
    public List<GameObject> Objects = new List<GameObject>();
    public Dictionary<GameObject, Vector3> vectorToCenter = new Dictionary<GameObject, Vector3>();

    public GameObject AgentObject;

    // private LineRenderer lineRenderer;
    // public Color lineColor = Color.red; // 设置默认颜色
    // public float lineWidth = 0.1f; // 设置默认宽度

    void Start()
    {
        frameCenter = (cube1.transform.position + cube2.transform.position + cube3.transform.position + cube4.transform.position) / 4;
        frameScale.x = Mathf.Abs(cube1.transform.position.x - frameCenter.x);
        frameScale.y = Mathf.Abs(cube1.transform.position.y - frameCenter.y);
        frameScale.z = Mathf.Abs(cube1.transform.position.z - frameCenter.z);
        Objects.Add(cube1);
        Objects.Add(cube2);
        Objects.Add(cube3);
        Objects.Add(cube4);

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

        if(AgentObject.GetComponent<GrabAgentObject>().AutoAdjustStatus){
        
            if(AgentObject.GetComponent<GrabAgentObject>().FinishedObjects.Count == 1)
            {
                frameCenter = AgentObject.GetComponent<GrabAgentObject>().FinishedObjects[0].transform.position + vectorToCenter[AgentObject.GetComponent<GrabAgentObject>().FinishedObjects[0]];
            }
            else if(AgentObject.GetComponent<GrabAgentObject>().FinishedObjects.Count == 2)
            {
                frameScale = (AgentObject.GetComponent<GrabAgentObject>().FinishedObjects[0].transform.position - AgentObject.GetComponent<GrabAgentObject>().FinishedObjects[1].transform.position) / 2;
            }

            foreach (var obj in Objects)
            {
                obj.transform.position = frameCenter + new Vector3(vectorToCenter[obj].x * frameScale.x, vectorToCenter[obj].y * frameScale.y, vectorToCenter[obj].z * frameScale.z);
            }
        }

    }
}
