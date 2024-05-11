using UnityEngine;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;

[System.Serializable]
public class LineStructure
{
    public Vector3 averagePosition;
    public Vector3 lineVector;
    public List<GameObject> lineObjects;

    public LineStructure(Vector3 averagePosition, Vector3 lineVector, List<GameObject> lineObjects)
    {
        this.averagePosition = averagePosition;
        this.lineVector = lineVector;
        this.lineObjects = lineObjects;
    }

    public float PointDistanceToLine(GameObject obj)
    {
        if(lineObjects.Contains(obj)) return 99999;
        Vector3 pointOnLine = averagePosition + Vector3.Project(obj.transform.position - averagePosition, lineVector);
        Debug.Log("distance: " + Vector3.Distance(obj.transform.position, pointOnLine));
        return Vector3.Distance(obj.transform.position, pointOnLine);
    }
}

[System.Serializable]
public class Circle3D
{
    public float r;
    public Vector3 center;
    public List<Vector3> points;

    public Circle3D(float r, Vector3 center, List<Vector3> points)
    {
        this.r = r;
        this.center = center;
        this.points = points;
    }
}


public class PointStructure : MonoBehaviour
{
    public List<GameObject> FinishedObjects = new List<GameObject>();
    public List<LineStructure> lineStructures = new List<LineStructure>();
    public Material lineMaterial;
    private Dictionary<LineStructure, GameObject> lineRenderers = new Dictionary<LineStructure, GameObject>();


    void Start(){
        GameObject parent = GameObject.Find("FinishedObjects");
        foreach (Transform child in parent.transform)
        {
            FinishedObjects.Add(child.gameObject);
            FitLines(child.gameObject, 0.3f);
        }
        

    }

    void Update(){


    }

    public void FitLines(GameObject objects, float threshold)
    {
        Debug.Log("objects:" + objects.name);
        int flag = 0;
        foreach (var line in lineStructures)
        {
            if(line.PointDistanceToLine(objects) < threshold){
                Debug.LogWarning("here0");
                // line.averagePosition = (line.averagePosition * line.lineObjects.Count + objects.transform.position) / (line.lineObjects.Count + 1);
                line.lineObjects.Add(objects);
                FitLine(line);
                GameObject edgeObj1 = new GameObject();
                GameObject edgeObj2 = new GameObject();
                float distance1 = 0;
                float distance2 = 0;

                foreach (var obj in line.lineObjects)
                {
                    if(Vector3.Dot(obj.transform.position - line.averagePosition, line.lineVector) > 0){
                        if(Vector3.Distance(obj.transform.position, line.averagePosition) > distance1)
                        {
                            distance1 = Vector3.Distance(obj.transform.position, line.averagePosition);
                            edgeObj1 = obj;
                        }
                    }
                    else{
                        if(Vector3.Distance(obj.transform.position, line.averagePosition) > distance2)
                        {
                            distance2 = Vector3.Distance(obj.transform.position, line.averagePosition);
                            edgeObj2 = obj;
                        }
                    }
                }
                Debug.LogWarning("here1");

                foreach (var obj in line.lineObjects)
                {
                    if(obj != edgeObj1 && obj != edgeObj2){
                        FinishedObjects.Remove(obj);
                    }
                }
                
                
                Debug.LogWarning("here2");
                Debug.LogWarning(lineRenderers[line]);
                if(lineRenderers[line] != null) {
                    Debug.LogWarning("here");
                    Destroy(lineRenderers[line]);
                    CreateLineWithLineRenderer(line);
                }
                Debug.LogWarning("here3");
                flag = 1;
                break;
            }
        }

        Debug.Log("flag:" + flag);
        if(flag == 0){
            for(int i=0; i<FinishedObjects.Count; i++)
            {
                GameObject finishedObj = FinishedObjects[i];
                Debug.Log("finishedObj:" + finishedObj.name);
                Vector3 averagePosition = (objects.transform.position + finishedObj.transform.position) / 2;
                LineStructure tempLine = new LineStructure(averagePosition, finishedObj.transform.position - objects.transform.position, new List<GameObject>(){objects, finishedObj});
                for(int j=i+1; j<FinishedObjects.Count; j++)
                {
                    GameObject finishedObj2 = FinishedObjects[j];
                    Debug.Log("finishedObj2:" + finishedObj2.name);
                    if(tempLine.PointDistanceToLine(finishedObj2) < threshold){
                        // tempLine.averagePosition = (tempLine.averagePosition * tempLine.lineObjects.Count + finishedObj2.transform.position) / (tempLine.lineObjects.Count + 1);
                        tempLine.lineObjects.Add(finishedObj2);
                        Debug.Log("lineObjects.count:" + tempLine.lineObjects.Count);
                        FitLine(tempLine);
                    }
                }

                Debug.Log("count:" + tempLine.lineObjects.Count);
                if(tempLine.lineObjects.Count >= 3){
                    lineStructures.Add(tempLine);
                    Debug.Log("lineCount: " + lineStructures.Count);
                    CreateLineWithLineRenderer(tempLine);
                }
            
            }
        }
    }


    private void FitLine(LineStructure line)
    {
        List<GameObject> lineObjects = line.lineObjects;
        float sumX = 0, sumY = 0, sumZ = 0, sumXX = 0, sumXY = 0, sumXZ = 0;
        int n = lineObjects.Count;

        // 计算平均位置
        foreach (var obj in lineObjects)
        {
            Vector3 position = obj.transform.position;
            sumX += position.x;
            sumY += position.y;
            sumZ += position.z;
        }
        line.averagePosition = new Vector3(sumX / n, sumY / n, sumZ / n);

        // 计算直线方向向量
        foreach (var obj in lineObjects)
        {
            Vector3 position = obj.transform.position - line.averagePosition;
            sumXX += position.x * position.x;
            sumXY += position.x * position.y;
            sumXZ += position.x * position.z;
        }

        float divisor = n * sumXX - sumX * sumX;
        if (Mathf.Abs(divisor) < 1e-10f)
        {
            // 如果除数接近于0,则将直线方向向量设置为Z轴
            line.lineVector = Vector3.forward;
        }
        else
        {
            float a = (sumXX * sumY - sumX * sumXY) / divisor;
            float b = (n * sumXY - sumX * sumY) / divisor;
            float c = (sumXX * sumZ - sumX * sumXZ) / divisor;
            line.lineVector = new Vector3(b, a, c).normalized;
        }

        Debug.Log("LINE: " + line.lineVector);
        Debug.LogWarning("here");
    }
        
    public GameObject CreateLineWithLineRenderer(LineStructure line)
    {
        float width = 0.4f; // 你可以根据需要设置不同的宽度

        GameObject lineObj = new GameObject("Line " + lineRenderers.Count);
        LineRenderer lineRenderer = lineObj.AddComponent<LineRenderer>();

        // 设置线条材质
        lineRenderer.material = lineMaterial;
        lineRenderer.startColor = Color.green;
        lineRenderer.endColor = Color.green;

        // 设置线条宽度
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;

        // 找到直线上距离最远的两个点作为线条的起点和终点
        float minDist = float.MaxValue;
        float maxDist = float.MinValue;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        foreach (var obj in line.lineObjects)
        {
            Vector3 projectedPoint = line.averagePosition + Vector3.Project(obj.transform.position - line.averagePosition, line.lineVector);
            float dist = Vector3.Distance(obj.transform.position, projectedPoint);

            if (dist < minDist)
            {
                minDist = dist;
                minPoint = obj.transform.position;
            }
            if (dist > maxDist)
            {
                maxDist = dist;
                maxPoint = obj.transform.position;
            }
        }



        // 设置线条顶点
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, minPoint + 3*(minPoint - maxPoint));
        lineRenderer.SetPosition(1, maxPoint + 3*(maxPoint - minPoint));

        // 添加胶囊体碰撞体
        lineObj.AddComponent<CapsuleCollider>();
        CapsuleCollider collider = lineObj.GetComponent<CapsuleCollider>();
        collider.center = Vector3.zero;
        collider.radius = width * 1.5f;
        collider.height = Vector3.Distance(minPoint, maxPoint);
        collider.direction = 2; // 沿 Z 轴延伸

        // 设置线条位置和旋转
        lineObj.transform.position = (minPoint + maxPoint) / 2f;
        lineObj.transform.rotation = Quaternion.FromToRotation(Vector3.forward, maxPoint - minPoint);

        // 将线条对象存储到字典中
        lineRenderers[line] = lineObj;

        return lineObj;
    }
}
