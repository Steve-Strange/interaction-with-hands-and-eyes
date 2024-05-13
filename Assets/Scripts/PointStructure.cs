using UnityEngine;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;

[System.Serializable]
public class LineStructure
{
    public Vector3 point1;
    public Vector3 point2;
    public List<GameObject> lineObjects;

    public LineStructure(Vector3 point1, Vector3 point2, List<GameObject> lineObjects)
    {
        this.point1 = point1;
        this.point2 = point2;
        this.lineObjects = lineObjects;
    }

    public float PointDistanceToLine(GameObject obj)
    {
        if (lineObjects.Contains(obj)) return 99999;

        Vector3 lineDirection = (point2 - point1).normalized;
        Vector3 pointToLine = obj.transform.position - point1;

        float distance = Vector3.Cross(pointToLine, lineDirection).magnitude;
        Debug.Log("distance: " + distance);
        return distance;
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
        // GameObject parent = GameObject.Find("FinishedObjects");
        // foreach (Transform child in parent.transform)
        // {
        //     FinishedObjects.Add(child.gameObject);

        //     FitLines(child.gameObject, (child.gameObject.transform.GetComponent<Renderer>().bounds.size.x + child.gameObject.transform.GetComponent<Renderer>().bounds.size.y + child.gameObject.transform.GetComponent<Renderer>().bounds.size.z));
        // }
        

        // foreach (var line in lineRenderers)
        // {
        //     foreach (var obj in line.Key.lineObjects)
        //     {
        //         Debug.Log(line.Value.name + obj.name);
        //     }
        // }
    }

    void Update(){


    }

    public void FitLines(GameObject objects)
    {
        float threshold = (objects.transform.GetComponent<Renderer>().bounds.size.x + objects.transform.GetComponent<Renderer>().bounds.size.y + objects.transform.GetComponent<Renderer>().bounds.size.z) / 2;
        Debug.Log("objects:" + objects.name);
        int flag = 0;
        foreach (var line in lineStructures)
        {
            if(line.PointDistanceToLine(objects) < threshold){
                // line.averagePosition = (line.averagePosition * line.lineObjects.Count + objects.transform.position) / (line.lineObjects.Count + 1);
                if(!line.lineObjects.Contains(objects)) line.lineObjects.Add(objects);
                FitLine(line);
                GameObject edgeObj1 = new GameObject();
                GameObject edgeObj2 = new GameObject();
                float distance1 = 0;
                float distance2 = 0;
                

                foreach (var obj in line.lineObjects)
                {
                    if(Vector3.Dot(obj.transform.position - (line.point1 + line.point2)/2, line.point1-line.point2) > 0){
                        if(Vector3.Distance(obj.transform.position, (line.point1 + line.point2)/2) > distance1)
                        {
                            distance1 = Vector3.Distance(obj.transform.position, (line.point1 + line.point2)/2);
                            edgeObj1 = obj;
                        }
                    }
                    else{
                        if(Vector3.Distance(obj.transform.position, (line.point1 + line.point2)/2) > distance2)
                        {
                            distance2 = Vector3.Distance(obj.transform.position, (line.point1 + line.point2)/2);
                            edgeObj2 = obj;
                        }
                    }
                }

                foreach (var obj in line.lineObjects)
                {
                    if(obj != edgeObj1 && obj != edgeObj2){
                        FinishedObjects.Remove(obj);
                    }
                }
                
                
                if(lineRenderers[line] != null) {

                    Destroy(lineRenderers[line]);
                    CreateLineWithLineRenderer(line);
                }
                flag = 1;
                break;
            }
        }

        // Debug.Log("flag:" + flag);
        if(flag == 0){
            for(int i=0; i<FinishedObjects.Count; i++)
            {
                GameObject finishedObj = FinishedObjects[i];
                // Debug.Log("finishedObj:" + finishedObj.name);
                Vector3 averagePosition = (objects.transform.position + finishedObj.transform.position) / 2;
                LineStructure tempLine = new LineStructure(averagePosition, finishedObj.transform.position - objects.transform.position, new List<GameObject>(){objects, finishedObj});
                for(int j=i+1; j<FinishedObjects.Count; j++)
                {
                    GameObject finishedObj2 = FinishedObjects[j];
                    // Debug.Log("finishedObj2:" + finishedObj2.name);
                    if(tempLine.PointDistanceToLine(finishedObj2) < threshold){
                        // tempLine.averagePosition = (tempLine.averagePosition * tempLine.lineObjects.Count + finishedObj2.transform.position) / (tempLine.lineObjects.Count + 1);
                        if(!tempLine.lineObjects.Contains(finishedObj2)) tempLine.lineObjects.Add(finishedObj2);
                        // Debug.Log("lineObjects.count:" + tempLine.lineObjects.Count);
                        FitLine(tempLine);
                    }
                }

                // Debug.Log("count:" + tempLine.lineObjects.Count);
                if(tempLine.lineObjects.Count >= 3){
                    lineStructures.Add(tempLine);
                    // Debug.Log("lineCount: " + lineStructures.Count);
                    CreateLineWithLineRenderer(tempLine);
                }
            
            }
        }
    }


    private void FitLine(LineStructure line)
    {
        List<GameObject> lineObjects = line.lineObjects;
        int n = lineObjects.Count;

        // 如果点数小于2,直接返回
        if (n < 2)
        {
            // Debug.LogWarning("Not enough points to fit a line.");
            return;
        }

        // RANSAC参数
        int maxIterations = 30; // 最大迭代次数
        float threshold = 2f; // 距离阈值
        int minInliers = (int)(n * 0.5f); // 最小内点数量

        // 初始化最佳直线
        Vector3 bestPoint1 = Vector3.zero;
        Vector3 bestPoint2 = Vector3.zero;
        int bestInliers = 0;

        // RANSAC迭代
        for (int iter = 0; iter < maxIterations; iter++)
        {
            // 随机选择两个点
            int idx1 = UnityEngine.Random.Range(0, n);
            int idx2 = (idx1 + UnityEngine.Random.Range(1, n)) % n;
            Vector3 point1 = lineObjects[idx1].transform.position;
            Vector3 point2 = lineObjects[idx2].transform.position;

            // 计算当前直线
            Vector3 lineDirection = point2 - point1;

            int inliers = 0;
            List<GameObject> inlierObjects = new List<GameObject>();

            // 统计内点数量
            foreach (var obj in lineObjects)
            {
                float distance = PointDistanceToLine(obj.transform.position, point1, lineDirection);
                if (distance < threshold)
                {
                    inliers++;
                    inlierObjects.Add(obj);
                }
            }

            // 更新最佳直线
            if (inliers > bestInliers)
            {
                bestInliers = inliers;
                bestPoint1 = point1;
                bestPoint2 = point2;
                line.lineObjects = inlierObjects;
            }

            // 如果内点数量已经足够大,可以提前结束迭代
            if (bestInliers >= minInliers)
            {
                break;
            }
        }

        // 对最后的内点进行直线拟合
        if (bestInliers >= minInliers)
        {
            FitLineWithLeastSquares(line);
        }
        else
        {
            // Debug.LogWarning("No reliable line found.");
        }
    }

    private void FitLineWithLeastSquares(LineStructure line)
    {
        List<GameObject> lineObjects = line.lineObjects;
        Vector3 averagePosition = GetAveragePosition(lineObjects);

        // 计算协方差矩阵
        float covXX = 0, covXY = 0, covXZ = 0;
        foreach (var obj in lineObjects)
        {
            Vector3 position = obj.transform.position - averagePosition;
            covXX += position.x * position.x;
            covXY += position.x * position.y;
            covXZ += position.x * position.z;
        }

        // 避免除以零
        float detXX = covXX;
        if (Mathf.Abs(detXX) < Mathf.Epsilon)
        {
            Debug.LogWarning("Determinant is too small, cannot fit line.");
            return;
        }

        // 计算特征向量
        float a = covXY / detXX;
        float c = covXZ / detXX;
        float b = -a / Mathf.Sqrt(a * a + 1 + c * c);

        // 更新直线表示
        line.point1 = averagePosition - new Vector3(b, a, c) * 10f;
        line.point2 = averagePosition + new Vector3(b, a, c) * 10f;
        
    }

    private Vector3 GetAveragePosition(List<GameObject> objects)
    {
        float sumX = 0, sumY = 0, sumZ = 0;
        foreach (var obj in objects)
        {
            Vector3 position = obj.transform.position;
            sumX += position.x;
            sumY += position.y;
            sumZ += position.z;
        }
        return new Vector3(sumX / objects.Count, sumY / objects.Count, sumZ / objects.Count);
    }

    private Vector3 ProjectPointOnLine(Vector3 point, Vector3 linePoint1, Vector3 linePoint2)
    {
        Vector3 lineDirection = (linePoint2 - linePoint1).normalized;
        Vector3 pointToLine = point - linePoint1;
        float t = Vector3.Dot(pointToLine, lineDirection);
        return linePoint1 + lineDirection * t;
    }

    private float PointDistanceToLine(Vector3 point, Vector3 linePoint, Vector3 lineDirection)
    {
        Vector3 pointToLine = point - linePoint;
        return Vector3.Cross(lineDirection, pointToLine).magnitude / lineDirection.magnitude;
    }
        
    public GameObject CreateLineWithLineRenderer(LineStructure line)
    {
        float averageWidth = 0.0f;
        foreach (var obj in line.lineObjects)
        {
            averageWidth += obj.transform.localScale.x + obj.transform.localScale.y + obj.transform.localScale.z;
        }
        averageWidth /= line.lineObjects.Count * 12;
        float width = averageWidth; // 你可以根据需要设置不同的宽度

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
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;
        foreach (var obj in line.lineObjects)
        {
            Vector3 projectedPoint = ProjectPointOnLine(obj.transform.position, line.point1, line.point2);
            float dist = Vector3.Distance(obj.transform.position, projectedPoint);
            if (minPoint == Vector3.zero || dist < Vector3.Distance(minPoint, projectedPoint))
            {
                minPoint = obj.transform.position;
            }
            if (maxPoint == Vector3.zero || dist > Vector3.Distance(maxPoint, projectedPoint))
            {
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
        collider.radius = width;
        collider.height = Vector3.Distance(minPoint, maxPoint) * 7;
        collider.direction = 2; // 沿 Z 轴延伸

        // 设置线条位置和旋转
        lineObj.transform.position = (minPoint + maxPoint) / 2f;
        lineObj.transform.rotation = Quaternion.FromToRotation(Vector3.forward, maxPoint - minPoint);

        // 将线条对象存储到字典中
        lineRenderers[line] = lineObj;

        return lineObj;
    }
}
