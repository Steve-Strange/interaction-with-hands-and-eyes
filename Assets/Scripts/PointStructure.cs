using UnityEngine;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using System.Linq;

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
public class CircleStructure
{
    public float r;
    public Vector3 center;
    public Vector3 planeNormal;
    public List<GameObject> circleObjects;

    public CircleStructure(float r, Vector3 center, Vector3 planeNormal, List<GameObject> circleObjects)
    {
        this.r = r;
        this.center = center;
        this.planeNormal = planeNormal;
        this.circleObjects = circleObjects;
    }

    public float PointDistanceToCircle(GameObject obj)
    {
        Vector3 pointPosition = obj.transform.position;
        Vector3 vectorToCenter = pointPosition - center;

        float distanceToPlane = Vector3.Dot(vectorToCenter, planeNormal);
        Vector3 projectedVector = vectorToCenter - distanceToPlane * planeNormal;

        float projectedDistance = projectedVector.magnitude;

        return Mathf.Sqrt((projectedDistance - r) * (projectedDistance - r) + distanceToPlane * distanceToPlane);
    }
}


public class PointStructure : MonoBehaviour
{
    public List<GameObject> FinishedObjects = new List<GameObject>();
    public List<LineStructure> lineStructures = new List<LineStructure>();
    public List<CircleStructure> circleStructures = new List<CircleStructure>();
    public Material lineMaterial;
    private Dictionary<LineStructure, GameObject> lineRenderers = new Dictionary<LineStructure, GameObject>();


    void Start(){
        GameObject parent = GameObject.Find("FinishedObjects");
        foreach (Transform child in parent.transform)
        {
            FinishedObjects.Add(child.gameObject);

            FitLines(child.gameObject);
            FitCircles(child.gameObject);
        }
        

        foreach (var line in lineRenderers)
        {
            foreach (var obj in line.Key.lineObjects)
            {
                Debug.Log(line.Value.name + obj.name);
            }
        }

    }

    void Update(){


    }

    public void FitLines(GameObject obj)
    {
        float threshold = CalculateAverageSize(obj);
        int flag = 0;
        foreach (var line in lineStructures)
        {
            if(line.PointDistanceToLine(obj) < threshold){
                // line.averagePosition = (line.averagePosition * line.lineObjects.Count + obj.transform.position) / (line.lineObjects.Count + 1);
                if(!line.lineObjects.Contains(obj)) line.lineObjects.Add(obj);
                FitLine(line);
                
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
            CreateLine(obj);
            
        }
    }

    struct VectorBatch
    {
        public List<GameObject> lineObjects;
        public float bias;
        public Vector3 vector;
    };

    private void CreateLine(GameObject rootObj)
    {
        List<VectorBatch> vectorBatches = new List<VectorBatch>();
        float angleThreshold = 5f;
        for (int i = 0; i < FinishedObjects.Count; i++)
        {
            float minThreshold = 100000;
            int minVectorBatch = -1;

            for (int j = 0; j < vectorBatches.Count; j++)
            {
                float angle = Vector3.Angle(vectorBatches[j].vector, FinishedObjects[i].transform.position - rootObj.transform.position);
                if (angle > 90) angle = 180 - angle;
                if (angle < minThreshold)
                {
                    minThreshold = angle;
                    minVectorBatch = j;
                }
            }

            if (minThreshold < angleThreshold)
            {
                List<GameObject> newLineObjects = vectorBatches[minVectorBatch].lineObjects;
                newLineObjects.Add(FinishedObjects[i]);
                float angle = Vector3.Angle(vectorBatches[minVectorBatch].vector, FinishedObjects[i].transform.position - rootObj.transform.position);
                float newBias;
                Vector3 newVector;
                if (angle > 90) {
                    newBias = vectorBatches[minVectorBatch].bias + 180 - angle;;
                    newVector = (vectorBatches[minVectorBatch].vector * vectorBatches[minVectorBatch].lineObjects.Count - (FinishedObjects[i].transform.position - rootObj.transform.position).normalized).normalized;
                }
                else {
                    newBias = vectorBatches[minVectorBatch].bias + angle;
                    newVector = (vectorBatches[minVectorBatch].vector * vectorBatches[minVectorBatch].lineObjects.Count + (FinishedObjects[i].transform.position - rootObj.transform.position).normalized).normalized;
                }
                
                
                VectorBatch newBatch = new VectorBatch
                {
                    lineObjects = newLineObjects,
                    bias = newBias,
                    vector = newVector
                };
                vectorBatches[minVectorBatch] = newBatch;
            }
            else
            {
                VectorBatch newBatch = new VectorBatch
                {
                    lineObjects = new List<GameObject> { FinishedObjects[i] },
                    bias = 0f,
                    vector = (FinishedObjects[i].transform.position - rootObj.transform.position).normalized
                };
                vectorBatches.Add(newBatch);
            }
        }

        vectorBatches.Sort((a, b) =>
        {
            int countComparison = b.lineObjects.Count.CompareTo(a.lineObjects.Count);
            if (countComparison != 0)
            {
                return countComparison;
            }
            else
            {
                return a.bias.CompareTo(b.bias);
            }
        });

        if (vectorBatches.Count > 0 && vectorBatches[0].lineObjects.Count >= 3)
        {
            LineStructure line = new LineStructure(vectorBatches[0].lineObjects[0].transform.position - vectorBatches[0].vector * 10f, vectorBatches[0].lineObjects[0].transform.position + vectorBatches[0].vector * 10f , vectorBatches[0].lineObjects);
            lineStructures.Add(line);
            CreateLineWithLineRenderer(line);
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
            averageWidth += CalculateAverageSize(obj);
        }
        averageWidth /= line.lineObjects.Count * 5;
        float width = averageWidth; // 你可以根据需要设置不同的宽度

        GameObject lineObj = new GameObject("Line " + lineRenderers.Count);
        lineObj.tag = "Structure";
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
        lineRenderer.SetPosition(0, minPoint + 5*(minPoint - maxPoint));
        lineRenderer.SetPosition(1, maxPoint + 5*(maxPoint - minPoint));

        // 添加胶囊体碰撞体
        lineObj.AddComponent<CapsuleCollider>();
        CapsuleCollider collider = lineObj.GetComponent<CapsuleCollider>();
        collider.center = Vector3.zero;
        collider.radius = width;
        collider.height = Vector3.Distance(minPoint, maxPoint) * 11;
        collider.direction = 2; // 沿 Z 轴延伸

        // 设置线条位置和旋转
        lineObj.transform.position = (minPoint + maxPoint) / 2f;
        lineObj.transform.rotation = Quaternion.FromToRotation(Vector3.forward, maxPoint - minPoint);

        // 将线条对象存储到字典中
        lineRenderers[line] = lineObj;

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

        return lineObj;
    }


    public void FitCircles(GameObject obj)
    {
        float threshold = CalculateAverageSize(obj);
        bool foundCircle = false;

        foreach (var circle in circleStructures)
        {
            if (circle.PointDistanceToCircle(obj) < threshold)
            {
                if (!circle.circleObjects.Contains(obj)) 
                {
                    circle.circleObjects.Add(obj);
                    FinishedObjects.Remove(obj);
                    FitCircleToPoints(circle);
                }
                foundCircle = true;
                break;
            }
        }

        if (!foundCircle)
        {
            Debug.Log(obj.name + " Create Circle");
            CreateCircle(obj);
        }
    }

    private void CreateCircle(GameObject rootObj)
    {
        float threshold = CalculateAverageSize(rootObj);
        Dictionary<GameObject, float> distanceToRootObj = new Dictionary<GameObject, float>();
        List<GameObject> surroundingObjs = new List<GameObject>();

        foreach (GameObject finishedObj in FinishedObjects)
        {
            float distance = Vector3.Distance(finishedObj.transform.position, rootObj.transform.position);
            distanceToRootObj[finishedObj] = distance;

        }

        distanceToRootObj.OrderBy(x => x.Value);
        int maxCount = Mathf.Min(distanceToRootObj.Count, 8);
        surroundingObjs = distanceToRootObj.Keys.Take(maxCount).ToList();
        Debug.Log("surroundingObjs: " + surroundingObjs.Count);

        Dictionary<CircleStructure, float> circleBias = new Dictionary<CircleStructure, float>();
        Debug.Log("circleBias: " + circleBias.Count);

        foreach (GameObject a in surroundingObjs)
        {
            foreach (GameObject b in surroundingObjs)
            {
                if (a != b)
                {
                    foreach (GameObject c in surroundingObjs)
                    {
                        if (c != a && c != b)
                        {
                            CircleStructure circle = new CircleStructure(0, Vector3.zero, Vector3.zero, new List<GameObject> { a, b, c });
                            FitCircleToPoints(circle);
                            circleBias[circle] = 0;
                        }
                    }
                }
            }
        }
        Debug.Log("circleBias: " + circleBias.Count);

        List<CircleStructure> circleKeys = new List<CircleStructure>(circleBias.Keys);


        foreach (CircleStructure circle in circleKeys)
        {
            Debug.Log("circleObjects.count: " + circle.circleObjects.Count);
            Debug.Log("surrondingObjs.count: " + surroundingObjs.Count);
            foreach (GameObject obj in surroundingObjs)
            {
                Debug.Log("obj: " + obj.name);
                if (!circle.circleObjects.Contains(obj))
                {

                    float bias = circle.PointDistanceToCircle(obj);
                    Debug.Log("bias: " + bias + "threshold: " + threshold);
                    if (bias < threshold)
                    {
                        circle.circleObjects.Add(obj);
                        circleBias[circle] += bias;
                    }
                }
            }
        }

        List<CircleStructure> sortedCircles = circleBias.OrderByDescending(x => x.Key.circleObjects.Count)
                                                        .ThenBy(x => x.Value)
                                                        .Select(x => x.Key)
                                                        .ToList();

        Debug.Log("here");
        if (sortedCircles.Count > 0)
        {
            Debug.Log("circleObjects.count: " + sortedCircles[0].circleObjects.Count + "bias: " + circleBias[sortedCircles[0]]);
        }

        if (sortedCircles.Any() && sortedCircles[0].circleObjects.Count >= 4)
        {
            CircleStructure bestCircle = sortedCircles[0];
            foreach (var obj in bestCircle.circleObjects)
            {
                FinishedObjects.Remove(obj);
            }
            CreateCircleWithLineRenderer(bestCircle);
            circleStructures.Add(bestCircle);

        }
    }

    private float CalculateAverageSize(GameObject obj)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        Vector3 size = renderer.bounds.size;
        return (size.x + size.y + size.z) / 3f;
    }


    public void FitCircleToPoints(CircleStructure circle)
    {
        List<GameObject> circleObjects = circle.circleObjects;
        if (circleObjects == null || circleObjects.Count < 3)
        {
            throw new System.ArgumentException("At least 3 points are required to fit a circle.");
        }

        // Step 1: Calculate the centroid of the points
        Vector3 centroid = Vector3.zero;
        foreach (var obj in circleObjects)
        {
            centroid += obj.transform.position;
        }
        centroid /= circleObjects.Count;

        // Step 2: Subtract the centroid from the points to remove translation
        List<Vector3> centeredPoints = new List<Vector3>();
        foreach (var obj in circleObjects)
        {
            centeredPoints.Add(obj.transform.position - centroid);
        }

        // Step 3: Perform PCA to find the normal of the best-fit plane
        Vector3 normal = CalculatePlaneNormal(centeredPoints);

        // Step 4: Project points onto the plane and fit a circle
        List<Vector2> projectedPoints = new List<Vector2>();
        Quaternion rotation = Quaternion.FromToRotation(normal, Vector3.forward);
        foreach (var point in centeredPoints)
        {
            Vector3 rotatedPoint = rotation * point;
            projectedPoints.Add(new Vector2(rotatedPoint.x, rotatedPoint.y));
        }

        Vector2 circleCenter2D;
        float radius;
        FitCircle(projectedPoints, out circleCenter2D, out radius);

        // Step 5: Transform the 2D circle center back to 3D space
        Vector3 circleCenter3D = Quaternion.Inverse(rotation) * new Vector3(circleCenter2D.x, circleCenter2D.y, 0) + centroid;

        circle.center = circleCenter3D;
        circle.r = radius;
        circle.planeNormal = normal;
        return;
    }

    private static Vector3 CalculatePlaneNormal(List<Vector3> points)
    {
        Vector3 mean = Vector3.zero;
        foreach (var point in points)
        {
            mean += point;
        }
        mean /= points.Count;

        float xx = 0, xy = 0, xz = 0;
        float yy = 0, yz = 0, zz = 0;

        foreach (var point in points)
        {
            Vector3 diff = point - mean;
            xx += diff.x * diff.x;
            xy += diff.x * diff.y;
            xz += diff.x * diff.z;
            yy += diff.y * diff.y;
            yz += diff.y * diff.z;
            zz += diff.z * diff.z;
        }

        float detX = yy * zz - yz * yz;
        float detY = xx * zz - xz * xz;
        float detZ = xx * yy - xy * xy;

        float detMax = Mathf.Max(detX, detY, detZ);

        if (detMax == detX)
        {
            return new Vector3(detX, xz * yz - xy * zz, xy * yz - xz * yy).normalized;
        }
        else if (detMax == detY)
        {
            return new Vector3(xz * yz - xy * zz, detY, xy * xz - yz * xx).normalized;
        }
        else
        {
            return new Vector3(xy * yz - xz * yy, xy * xz - yz * xx, detZ).normalized;
        }
    }

    private static void FitCircle(List<Vector2> points, out Vector2 center, out float radius)
    {
        float xSum = 0, ySum = 0, xxSum = 0, yySum = 0, xySum = 0, xzSum = 0, yzSum = 0;

        foreach (var point in points)
        {
            float x = point.x;
            float y = point.y;
            float z = x * x + y * y;
            xSum += x;
            ySum += y;
            xxSum += x * x;
            yySum += y * y;
            xySum += x * y;
            xzSum += x * z;
            yzSum += y * z;
        }

        float n = points.Count;
        float a = n * xxSum - xSum * xSum;
        float b = n * xySum - xSum * ySum;
        float c = n * yySum - ySum * ySum;
        float d = 0.5f * (n * xzSum - xSum * (xxSum + yySum));
        float e = 0.5f * (n * yzSum - ySum * (xxSum + yySum));

        float denom = a * c - b * b;
        if (Mathf.Abs(denom) < Mathf.Epsilon)
        {
            center = Vector2.zero;
            radius = 0;
            return;
        }

        float x0 = (d * c - b * e) / denom;
        float y0 = (a * e - b * d) / denom;

        center = new Vector2(x0, y0);
        radius = Mathf.Sqrt((xxSum + yySum - 2 * (x0 * xSum + y0 * ySum) + n * (x0 * x0 + y0 * y0)) / n);
    }

    public GameObject CreateCircleWithLineRenderer(CircleStructure circle) {
        float r = circle.r;
        Vector3 center = circle.center;
        Vector3 planeNormal = circle.planeNormal;

        float averageWidth = 0.0f;
        foreach (var obj in circle.circleObjects) {
            averageWidth += CalculateAverageSize(obj);
        }
        averageWidth /= circle.circleObjects.Count * 5;
        float width = averageWidth; // 你可以根据需要设置不同的宽度

        GameObject circleObj = new GameObject("Circle");
        circleObj.tag = "Structure";
        LineRenderer lineRenderer = circleObj.AddComponent<LineRenderer>();

        // 设置线条材质
        lineRenderer.material = lineMaterial;
        lineRenderer.startColor = Color.green;
        lineRenderer.endColor = Color.green;

        // 设置线条宽度
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;

        // 设置线条顶点
        int numVertices = 64; // 设置边数为64
        int numCapsules = 32; // 设置胶囊体数量为32
        lineRenderer.positionCount = numVertices + 1;

        // 计算顶点坐标并添加胶囊体
        for (int i = 0; i <= numVertices; i++) {
            float angle = i * 2 * Mathf.PI / numVertices;
            Vector3 offset = new Vector3(Mathf.Cos(angle) * r, Mathf.Sin(angle) * r, 0);
            Vector3 vertex = center + Quaternion.LookRotation(planeNormal) * offset;
            lineRenderer.SetPosition(i, vertex);

            // 每numVertices/numCapsules个顶点添加一个胶囊体
            if (i % (numVertices / numCapsules) == 0) {
                GameObject capsuleObj = new GameObject("Capsule");
                CapsuleCollider capsuleCollider = capsuleObj.AddComponent<CapsuleCollider>();
                capsuleCollider.radius = width / 2f;
                capsuleCollider.height = 2 * r * Mathf.PI / numCapsules;
                capsuleCollider.direction = 1;

                // 设置胶囊体的位置和旋转
                capsuleObj.transform.position = vertex;
                Vector3 tangent = Quaternion.LookRotation(planeNormal) * new Vector3(-Mathf.Sin(angle), Mathf.Cos(angle), 0);
                capsuleObj.transform.rotation = Quaternion.LookRotation(planeNormal, tangent);
            }
        }

        // 设置线条位置和旋转
        circleObj.transform.position = center;
        circleObj.transform.rotation = Quaternion.FromToRotation(Vector3.forward, planeNormal);

        return circleObj;
    }


}

