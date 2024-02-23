using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class frameTest : MonoBehaviour
{
    private LineRenderer line;
    public List<GameObject> anchor;
    public GameObject connectorManager;

    private Vector3 right;
    private Vector3 forward;
    public float dis = 0.2f;
   

    //rect
    public float rectlenth = 0.3f;
    public float rectheight = 0.3f;

    public Vector3[] rectCorner = new Vector3[4];// save the for corner
    // tri
    public float triedge;
    public Vector3[] triCorner = new Vector3[3];
    //circle
    public float R = 0.1f;
    public int N = 40;
    public Vector3[] circle = new Vector3[3];
    //para
    public float angle;
    public Vector3[] paraCorner = new Vector3[4];
    //pen
    public Vector3[] penCorner = new Vector3[5];
    public float penedge;
    public List<BoxCollider> collider;//存添加的所有collider物体；
    //cube
    public Vector3[] cubeCorner = new Vector3[8];

    // Start is called before the first frame update
    void Start()
    {
        line = GetComponent<LineRenderer>();
        redoCircle();
    }

    // Update is called once per frame
    void Update()
    {
    

    }

    public void redoCircle()
    {

        Vector3 center = CalculateTriangleOutCircleCenter(anchor[0].transform.position, anchor[1].transform.position, anchor[2].transform.position);

            

        float R = (anchor[0].transform.position - center).magnitude;

        line.startWidth = 0.1f;
        line.endWidth = 0.1f;
        line.positionCount = N + 1;

        right = Vector3.right;
        forward = Vector3.forward;

        Vector3 now;

        for (int i = 0; i < N + 1; i++)
        {
            float x = R * Mathf.Cos((360f / N * i) * Mathf.Deg2Rad); //确定x坐标
            float z = R * Mathf.Sin((360f / N * i) * Mathf.Deg2Rad); //确定z坐标
            now = center + right * x + forward * z;
            line.SetPosition(i, now);
        }

    }


    private Vector3 CalculateTriangleOutCircleCenter(Vector3 A, Vector3 B, Vector3 C)
    {
        float Xa = A.x;
        float Ya = A.y;
        float Za = A.z;

        float Xb = B.x;
        float Yb = B.y;
        float Zb = B.z;

        float Xc = C.x;
        float Yc = C.y;
        float Zc = C.z;

        Vector3 D = (A + C) / 2;
        float Xd = D.x;
        float Yd = D.y;
        float Zd = D.z;

        //单位法向量AN
        Vector3 AB = B - A;
        Vector3 AC = C - A;
        Vector3 AN = Vector3.Cross(AB, AC).normalized;

        float u = AN.x;
        float v = AN.y;
        float w = AN.z;

        //构建三元一次方程参数
        float a = u;
        float b = v;
        float c = w;
        float d = u * Xa + v * Ya + w * Za;

        float e = Xc - Xa;
        float f = Yc - Ya;
        float g = Zc - Za;
        float h = (Xc - Xa) * (Xc + Xa) / 2 + (Yc - Ya) * (Yc + Ya) / 2 + (Zc - Za) * (Zc + Za) / 2;

        float k = 2 * Xb - 2 * Xa;
        float l = 2 * Yb - 2 * Ya;
        float m = 2 * Zb - 2 * Za;
        float n = Xb * Xb - Xa * Xa + Yb * Yb - Ya * Ya + Zb * Zb - Za * Za;

        float[] equa = CalculateTernaryEquation(a, b, c, d, e, f, g, h, k, l, m, n);
        Vector3 P = new Vector3(equa[0], equa[1], equa[2]);
        return P;
    }

    private float[] CalculateTernaryEquation(float a, float b, float c, float d, float e, float f, float g, float h, float k, float l, float m, float n)
    {
        float z = ((d * e - a * h) * (f * k - e * l) - (h * k - e * n) * (b * e - a * f)) / ((c * e - a * g) * (f * k - e * l) - (b * e - a * f) * (g * k - e * m));
        float y = ((d * e - a * h) * (g * k - e * m) - (h * k - e * n) * (c * e - a * g)) / ((b * e - a * f) * (g * k - e * m) - (f * k - e * l) * (c * e - a * g));
        float x = 0;
        if (a != 0)
            x = (d - b * y - c * z) / a;
        else if (e != 0)
            x = (h - f * y - g * z) / e;
        else if (k != 0)
            x = (n - l * y - m * z) / k;
        return new float[] { x, y, z };
    }
}
