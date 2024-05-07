using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Line3D
{
    public float a, b, c, d;
    public List<Vector3> points;

    public Line3D(float a, float b, float c, float d, List<Vector3> points)
    {
        this.a = a;
        this.b = b;
        this.c = c;
        this.d = d;
        this.points = points;
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
    public static List<Line3D> FitLines(List<Vector3> points, float threshold)
    {
        List<Line3D> lines = new List<Line3D>();
        List<List<Vector3>> combinations = GetCombinations(points, 3);

        foreach (List<Vector3> combo in combinations)
        {
            Line3D line = FitLine(combo);
            List<Vector3> inliers = points.FindAll(p => DistanceToLine(p, line) < threshold);
            if (inliers.Count >= 3)
            {
                line = FitLine(inliers);
                lines.Add(line);
            }
        }

        return lines;
    }

    public static List<Circle3D> FitCircles(List<Vector3> points, float threshold)
    {
        List<Circle3D> circles = new List<Circle3D>();
        List<List<Vector3>> combinations = GetCombinations(points, 3);

        foreach (List<Vector3> combo in combinations)
        {
            Circle3D circle = FitCircle(combo);
            List<Vector3> inliers = points.FindAll(p => DistanceToCircle(p, circle) < threshold);
            if (inliers.Count >= 3)
            {
                circle = FitCircle(inliers);
                circles.Add(circle);
            }
        }

        return circles;
    }

    private static Line3D FitLine(List<Vector3> points)
    {
        float sumX = 0, sumY = 0, sumZ = 0, sumXX = 0, sumXY = 0, sumXZ = 0, sumYY = 0, sumYZ = 0, sumZZ = 0;
        int n = points.Count;

        foreach (Vector3 p in points)
        {
            sumX += p.x;
            sumY += p.y;
            sumZ += p.z;
            sumXX += p.x * p.x;
            sumXY += p.x * p.y;
            sumXZ += p.x * p.z;
            sumYY += p.y * p.y;
            sumYZ += p.y * p.z;
        }

        float a = n * sumXY - sumX * sumY;
        float b = n * sumXZ - sumX * sumZ;
        float c = n * sumYZ - sumY * sumZ;
        float d = -(a * (sumXX + sumYY + sumZZ) / n + b * sumX / n + c * sumY / n);

        return new Line3D(a, b, c, d, points);
    }

    private static Circle3D FitCircle(List<Vector3> points)
    {
        float sumX = 0, sumY = 0, sumZ = 0, sumXX = 0, sumYY = 0, sumZZ = 0;
        int n = points.Count;

        foreach (Vector3 p in points)
        {
            sumX += p.x;
            sumY += p.y;
            sumZ += p.z;
            sumXX += p.x * p.x;
            sumYY += p.y * p.y;
            sumZZ += p.z * p.z;
        }

        Vector3 center = new Vector3(sumX / n, sumY / n, sumZ / n);
        float sumR = 0;

        foreach (Vector3 p in points)
        {
            sumR += Mathf.Sqrt((p.x - center.x) * (p.x - center.x) + (p.y - center.y) * (p.y - center.y) + (p.z - center.z) * (p.z - center.z));
        }

        float r = sumR / n;

        return new Circle3D(r, center, points);
    }

    private static float DistanceToLine(Vector3 p, Line3D line)
    {
        return Mathf.Abs(line.a * p.x + line.b * p.y + line.c * p.z + line.d) / Mathf.Sqrt(line.a * line.a + line.b * line.b + line.c * line.c);
    }

    private static float DistanceToCircle(Vector3 p, Circle3D circle)
    {
        return Mathf.Sqrt((p.x - circle.center.x) * (p.x - circle.center.x) + (p.y - circle.center.y) * (p.y - circle.center.y) + (p.z - circle.center.z) * (p.z - circle.center.z)) - circle.r;
    }

    private static List<List<Vector3>> GetCombinations(List<Vector3> points, int r)
    {
        return GetCombinations(points, r, 0, new List<Vector3>(), new List<List<Vector3>>());
    }

    private static List<List<Vector3>> GetCombinations(List<Vector3> points, int r, int start, List<Vector3> curr, List<List<Vector3>> result)
    {
        if (r == 0)
        {
            result.Add(new List<Vector3>(curr));
        }
        else
        {
            for (int i = start; i <= points.Count - r; i++)
            {
                curr.Add(points[i]);
                GetCombinations(points, r - 1, i + 1, curr, result);
                curr.RemoveAt(curr.Count - 1);
            }
        }

        return result;
    }
}