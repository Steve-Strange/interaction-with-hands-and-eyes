using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class frame : MonoBehaviour
{
    private LineRenderer line;
    public GameObject collideObject;

    private Vector3 forward;//ʵ�ֳ���
    private Vector3 right;
    private Vector3 up;
    public float dis = 0.2f;
    public GameObject head;
    public string Frame;
    
    //rect
    public float rectlenth = 0.3f;
    public float rectheight= 0.3f;

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
    float cubelenth = 0.1f;
    float    cubeheight = 0.1f;
    float cubewidth = 0.1f;
    public GameObject line_2;
    public GameObject line_3;
    public GameObject line_4;
    public GameObject line_5;
    public GameObject line_6;
    private LineRenderer line2;
    private LineRenderer line3;
    private LineRenderer line4;
    private LineRenderer line5;
    private LineRenderer line6;

    

    Vector3 center;
    
    public void creatRect()//2d, just make the origin frame
    {
        dis = 0.4f;

        //rect
        rectlenth = 0.1f;
        rectheight = 0.1f;

        Frame = "rect";

        rectCorner = new Vector3[4];

        forward = head.transform.forward.normalized;
        right = head.transform.right.normalized;
        up = head.transform.up.normalized;

        center = head.transform.position + forward * dis;

        line.positionCount = 5;
      /*  rectCorner[0] = center + up/2*rectheight-right/2*rectlenth;
        rectCorner[1] = center + up/2*rectheight+right/2*rectlenth;
        rectCorner[2] = center - up/2*rectheight+right/2*rectlenth;
        rectCorner[3] = center - up/2*rectheight-right/2*rectlenth;
*/
        rectCorner[0] = new Vector3(-0.1f, 0.1f, 0);
        rectCorner[1] = new Vector3(0.1f, 0.1f, 0);
        rectCorner[2] = new Vector3(0.1f, -0.1f, 0);
        rectCorner[3] = new Vector3(-0.1f, -0.1f, 0);
        for (int i = 0;i<=3;i++){
            line.SetPosition(i,rectCorner[i]);
            if(i!=3){
                addColliderToLine(rectCorner[i],rectCorner[i+1]);
            }
            else{
                Debug.Log(1);
                addColliderToLine(rectCorner[3],rectCorner[0]);
                line.SetPosition(4, rectCorner[0]);
            }
        }
    }
    public void createPara()
    {
        dis = 0.4f;

        //pingxing
        angle = 45;
        rectlenth = 0.1f;
        rectheight = 0.1f;

        Frame = "para";

        paraCorner = new Vector3[4];

        forward = head.transform.forward.normalized;
        right = head.transform.right.normalized;
        up = head.transform.up.normalized;

        center = head.transform.position + forward * dis;
        float sinx = Mathf.Cos((angle) * Mathf.Deg2Rad);
        float cosx = Mathf.Cos((angle) * Mathf.Deg2Rad);
        line.positionCount = 5;
        rectCorner[0] = center + up/2*rectheight*sinx-right/2*(rectlenth-cosx);
        rectCorner[1] = center + up/2*rectheight*sinx-right/2*(rectlenth+cosx);
        rectCorner[2] = center - up/2*rectheight*sinx+right/2*(rectlenth-cosx);
        rectCorner[3] = center - up/2*rectheight-right/2*(rectlenth+cosx);
        for(int i = 0;i<=3;i++){
            line.SetPosition(i,rectCorner[i]);
            if(i!=3){
                addColliderToLine(rectCorner[i],rectCorner[i+1]);
            }
            else{
                Debug.Log(1);
                addColliderToLine(rectCorner[3],rectCorner[0]);
                line.SetPosition(4, rectCorner[0]);
            }
        }

    }
    public void createTri()
    {
        dis = 0.4f;

        //triangle original 
        triedge = 0.1f;

        Frame = "tri";

        Vector3[] rectCorner = new Vector3[3];

        forward = head.transform.forward.normalized;
        right = head.transform.right.normalized;
        up = head.transform.up.normalized;

        center = head.transform.position + forward * dis;

        line.positionCount = 4;

        rectCorner[0] = center + up * (float)(triedge / Math.Sqrt(3));
        rectCorner[1] = center - up* (float)(triedge / Math.Sqrt(3) / 2) - right * ( triedge /2);
        rectCorner[2] = center - up* (float)(triedge / Math.Sqrt(3) / 2) + right * ( triedge /2);

        for(int i = 0;i<=2;i++){
            line.SetPosition(i,rectCorner[i]);
            if(i!=2){
                addColliderToLine(rectCorner[i],rectCorner[i+1]);
            }
            else{
                Debug.Log(1);
                addColliderToLine(rectCorner[2],rectCorner[0]);
                line.SetPosition(3, rectCorner[0]);
            }
        }        

    }
    public void createPentagon()
{
        dis = 0.4f;

        //triangle original 
        penedge = 0.1f;

        Frame = "pen";

        penCorner = new Vector3[5];

        forward = head.transform.forward.normalized;
        right = head.transform.right.normalized;
        up = head.transform.up.normalized;

        center = head.transform.position + forward * dis;

        line.positionCount = 6;

        penCorner[0] = center + up * penedge * Mathf.Cos((36) * Mathf.Deg2Rad);
        penCorner[1] =  center + up * penedge * Mathf.Cos((36) * Mathf.Deg2Rad) * Mathf.Cos((18) * Mathf.Deg2Rad) + right * penedge * Mathf.Sin((54) * Mathf.Deg2Rad) ;
        penCorner[2] = center + up * penedge /2  / Mathf.Tan((36) * Mathf.Deg2Rad)  + right * penedge /2  ;
        penCorner[3] = center + up * penedge /2  / Mathf.Tan((36) * Mathf.Deg2Rad)  - right * penedge /2  ;
        penCorner[4] =  center + up * penedge * Mathf.Cos((36) * Mathf.Deg2Rad) * Mathf.Cos((18) * Mathf.Deg2Rad) - right * penedge * Mathf.Sin((54) * Mathf.Deg2Rad) ;


        for(int i = 0;i<=4;i++){
            line.SetPosition(i,penCorner[i]);
            if(i!=4){
                addColliderToLine(penCorner[i],penCorner[i+1]);
            }
            else{
                Debug.Log(1);
                addColliderToLine(penCorner[4],penCorner[0]);
                line.SetPosition(4, penCorner[0]);
            }
        }        
}
public void createCube()// cant draw a cube at one time?->cube render manage more lineRender
{
        dis = 0.6f;

        //cube
        cubelenth = 0.1f;
        cubeheight = 0.1f;
        cubewidth = 0.1f;

        Frame = "cube";
        //
        line2 = line_2.GetComponent<LineRenderer>();
        line3 = line_3.GetComponent<LineRenderer>();
        line4 = line_4.GetComponent<LineRenderer>();
        line5 = line_5.GetComponent<LineRenderer>();
        line6 = line_6.GetComponent<LineRenderer>();

        Vector3[] cubeCorner = new Vector3[8];

        forward = head.transform.forward.normalized;
        right = head.transform.right.normalized;
        up = head.transform.up.normalized;

        center = head.transform.position + forward * dis;

        line.positionCount = 9;
        rectCorner[0] = center + up/2*rectheight-right/2*rectlenth + forward * cubewidth/2;
        rectCorner[1] = center + up/2*rectheight+right/2*rectlenth + forward * cubewidth/2;
        rectCorner[2] = center - up/2*rectheight+right/2*rectlenth + forward * cubewidth/2;
        rectCorner[3] = center - up/2*rectheight-right/2*rectlenth + forward * cubewidth/2;
        
        rectCorner[4] = center + up/2*rectheight-right/2*rectlenth - forward * cubewidth/2;
        rectCorner[5] = center + up/2*rectheight+right/2*rectlenth - forward * cubewidth/2;
        rectCorner[6] = center - up/2*rectheight+right/2*rectlenth - forward * cubewidth/2;
        rectCorner[7] = center - up/2*rectheight-right/2*rectlenth - forward * cubewidth/2;
        for(int i = 0;i<=3;i++){
            line.SetPosition(i,rectCorner[i]);
            if(i!=3){
                addColliderToLine(rectCorner[i],rectCorner[i+1]);
            }
            else{
                Debug.Log(1);
                addColliderToLine(rectCorner[3],rectCorner[0]);
                line.SetPosition(4, rectCorner[0]);
            }
        }

        for(int i = 4;i<=7;i++){
            line2.SetPosition(i,rectCorner[i]);
            if(i!=7){
                addColliderToLine(rectCorner[i],rectCorner[i+1]);
            }
            else{
                Debug.Log(1);
                addColliderToLine(rectCorner[7],rectCorner[4]);
                line2.SetPosition(4, rectCorner[4]);
            }
        }
        line3.SetPosition(0, cubeCorner[0]);
        line3.SetPosition(1, cubeCorner[4]);

        line4.SetPosition(0, cubeCorner[1]);
        line4.SetPosition(1, cubeCorner[5]);
        line5.SetPosition(0, cubeCorner[2]);
        line5.SetPosition(1, cubeCorner[6]);
        line6.SetPosition(0, cubeCorner[3]);
        line6.SetPosition(1, cubeCorner[7]);


}
    //距离等比例变化
    public void redoRect(string type)//visualize the frame by object position
    {
        // get the original plane by previous points
        Vector3 f = ((rectCorner[0]-rectCorner[1])).normalized;
        Vector3 m = ((rectCorner[0]-rectCorner[3])).normalized;
        if(type == "1")// 左上，右下
        {
            List<GameObject> anchor = collideObject.GetComponent<collide>().anchor;
            rectCorner[0] = anchor[0].transform.position;
            rectCorner[2] = anchor[1].transform.position;
            Vector3 temp = rectCorner[0]-rectCorner[2];
            rectheight = Vector3.Dot(temp , m);
            rectlenth = Vector3.Dot(temp, f);
            rectCorner[1] = rectCorner[0] - f * rectlenth;
            rectCorner[2] = rectCorner[0] - m * rectheight;
            
            for(int i = 0;i<=3;i++){
            line.SetPosition(i,rectCorner[i]);
            if(i!=3){
                resizeColliderToline(collider[i],rectCorner[i],rectCorner[i+1]);
            }
            else{
                resizeColliderToline(collider[i],rectCorner[3],rectCorner[0]);
            }
        }
            

        }

    }
    //
    public void redoCircle()
    {
        List<GameObject> anchor = collideObject.GetComponent<collide>().anchor;
        Vector3 center =  CalculateTriangleOutCircleCenter(anchor[0].transform.position, anchor[1].transform.position, anchor[2].transform.position);
        float R = (anchor[0].transform.position - center).magnitude;


        center = head.transform.position + forward * dis;

        line.positionCount = N + 1;



        Vector3 last = new Vector3(0, 0, 0);
        Vector3 now;


        for (int i = 0; i < N + 1; i++)
        {
            float x = R * Mathf.Cos((360f / N * i) * Mathf.Deg2Rad); //确定x坐标
            float z = R * Mathf.Sin((360f / N * i) * Mathf.Deg2Rad); //确定z坐标
            now = center + right * x + up * z;
            line.SetPosition(i, now);
            if (i != 0)
                addColliderToLine(last, now);
            last = now;
        }
        float xx = R * Mathf.Cos((0) * Mathf.Deg2Rad); //确定x坐标
        float zz = R * Mathf.Sin((0) * Mathf.Deg2Rad); //确定z坐标
        now = center + right * xx + up * zz;
        addColliderToLine(last, now);


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

    private void resizeColliderToline(BoxCollider col, Vector3 startPos,Vector3 endPos)
    {
        float lineLength = Vector3.Distance(startPos, endPos); // length of line
        col.size = new Vector3(lineLength, 0.0001f, 0.0001f); // size of collider is set where X is length of line, Y is width of line, Z will be set as per requirement
        Vector3 midPoint = (startPos + endPos) / 2;
        col.transform.position = midPoint; // setting position of collider object
        // Following lines calculate the angle between startPos and endPos
        float angle = (Mathf.Abs(startPos.y - endPos.y) / Mathf.Abs(startPos.x - endPos.x));
        if ((startPos.y < endPos.y && startPos.x > endPos.x) || (endPos.y < startPos.y && endPos.x > startPos.x))
        {
            angle *= -1;
        }
        angle = Mathf.Rad2Deg * Mathf.Atan(angle);
        col.transform.Rotate(0, 0, angle);

    }
    private void addColliderToLine(Vector3 startPos,Vector3 endPos)
    {
        
        BoxCollider col = new GameObject("Edge").AddComponent<BoxCollider>();
        collider.Add(col);
        col.transform.parent = line.transform; // Collider is added as child object of line
        float lineLength = Vector3.Distance(startPos, endPos); // length of line
        col.size = new Vector3(lineLength, 0.0001f, 0.0001f); // size of collider is set where X is length of line, Y is width of line, Z will be set as per requirement
        Vector3 midPoint = (startPos + endPos) / 2;
        col.transform.position = midPoint; // setting position of collider object
        // Following lines calculate the angle between startPos and endPos
        float angle = (Mathf.Abs(startPos.y - endPos.y) / Mathf.Abs(startPos.x - endPos.x));
        if ((startPos.y < endPos.y && startPos.x > endPos.x) || (endPos.y < startPos.y && endPos.x > startPos.x))
        {
            angle *= -1;
        }
        angle = Mathf.Rad2Deg * Mathf.Atan(angle);
        col.transform.Rotate(0, 0, angle);
    }




    public void createCircle()
    {   
        dis = 0.4f;

        //ciecle original 
        N = 40;
        R = 0.1f;

        Frame = "circle";


        forward = head.transform.forward.normalized;
        right = head.transform.right.normalized;
        up = head.transform.up.normalized;

        center = head.transform.position + forward * dis;

        line.positionCount = N + 1;


    
        Vector3 last = new Vector3(0, 0, 0);
        Vector3 now;


        for (int i = 0; i < N + 1; i++){
            float x = R * Mathf.Cos((360f / N * i) * Mathf.Deg2Rad); //确定x坐标
            float z = R * Mathf.Sin((360f / N * i) * Mathf.Deg2Rad); //确定z坐标
            now = center + right * x + up * z;
            line.SetPosition(i, now);
            if(i!=0)
                addColliderToLine(last, now);
            last = now;
        }
        float xx = R * Mathf.Cos((0) * Mathf.Deg2Rad) ; //确定x坐标
        float zz = R * Mathf.Sin((0) * Mathf.Deg2Rad) ; //确定z坐标
        now = center + right * xx + up * zz;
        addColliderToLine(last, now);
    }

    void Start()
    {
        line = GetComponent<LineRenderer>();
        line.startWidth = 0.01f;
        line.endWidth = 0.01f;
        


}
    // Update is called once per frame
    void Update()
    {
        
    }
}
