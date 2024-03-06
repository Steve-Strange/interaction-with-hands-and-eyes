using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class frame : MonoBehaviour
{
    private LineRenderer line;
    public GameObject connectorManager;
    public GameObject[] cor;//透明物体，其位置用于标记顶点；
    public GameObject collideObject;
    public TMP_Text t;


    private Vector3 forward;//第2阶段生成框那个瞬间所用的
    private Vector3 right;
    private Vector3 up;

    private float dis = 0.2f;
    public GameObject head;
    public string Frame;

    //rect
    private float rectlenth = 0.3f;
    private float rectheight= 0.3f;

    public Vector3[] rectCorner = new Vector3[4];// save the for corner
                                                 // tri
    private float triedge;
    public Vector3[] triCorner = new Vector3[3];
    //circle
   public float R = 0.1f;
    private int N = 40;
    public Vector3[] circle = new Vector3[3];
    //para
    private float angle;
    public Vector3[] paraCorner = new Vector3[4];
    private float paralenth;
    private float paraheight;
    //pen
    public Vector3[] penCorner = new Vector3[5];
    private float penedge;
    //star
    public Vector3[] starCorner = new Vector3[5];
    private float staredge;
    //cube
    public Vector3[] cubeCorner = new Vector3[8];
    float cubelenth = 0.1f;
    float cubeheight = 0.1f;
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
    public Material lineMaterial;

    public int number;

    float pi = 3.1415926F;
    float gap;
    float objSize;
    void resize()//change the frame size from number
    {
        objSize = collideObject.GetComponent<collide>().finalObj[0].transform.GetComponent<Renderer>().bounds.size.x;
        number = 3;
        gap =  objSize;

        //大小应该和尺寸以及数量有关
       if(Frame == "rect"){
            number = (int)Mathf.Ceil(collideObject.GetComponent<collide>().finalObj.Count/4f);
            if(number == 0)
                number = 1;
            rectheight = (objSize + gap) * number;
            rectlenth =  (objSize + gap) * number;
       }else if(Frame == "circle"){
            number = collideObject.GetComponent<collide>().finalObj.Count;
            if(number == 0)
                number = 1;
            R = collideObject.GetComponent<collide>().finalObj.Count * (objSize + gap)/(2*pi);
       }else if(Frame == "tri"){
            number = (int)Mathf.Ceil(collideObject.GetComponent<collide>().finalObj.Count / 3f);
            if(number == 0)
                number = 1;
            triedge = (objSize + gap) * number;
       }else if(Frame == "para")
       {
            number = (int)Mathf.Ceil(collideObject.GetComponent<collide>().finalObj.Count / 4f);
            if(number == 0)
                number = 1;
            paraheight = (objSize + gap) * number;
            paralenth = (objSize + gap) * number;
        }
        else if(Frame == "pen")
        {
            number = (int)Mathf.Ceil(collideObject.GetComponent<collide>().finalObj.Count / 5f);
            if(number == 0)
                number = 1;
            penedge = (objSize + gap) * number / (2 *  Mathf.Cos((54) * Mathf.Deg2Rad) ); 
            //边长还要换算
        }
        else if(Frame == "cube")
        {
            number = (int)Mathf.Ceil(collideObject.GetComponent<collide>().finalObj.Count / 12f);
            if(number == 0)
                number = 1;
            cubeheight = (objSize + gap) * number;
            cubelenth = (objSize + gap) * number;
            cubelenth = (objSize + gap) * number;
            
        }else if(Frame == "star")
        {
            number = (int)Mathf.Ceil(collideObject.GetComponent<collide>().finalObj.Count / 5f);
            if(number == 0)
                number = 1;
            penedge = (objSize + gap) * number / (2 *  Mathf.Cos((54) * Mathf.Deg2Rad) ); 
            //边长还要换算
        }
    }
    void decideEachPosition()//change the frame size from number
    {
       if(Frame == "rect"){
            var X = (rectCorner[1]-rectCorner[0]).normalized;
            var Y = (rectCorner[3]- rectCorner[0]).normalized;
            rectPosition.Clear();
            for(int i = 1 ; i<= number-1 ;i++)
            {
                rectPosition.Add(rectCorner[0] + (gap + objSize) * i * X);
            }
            for (int i = 1 ; i<= number-1 ;i++)
            {
                rectPosition.Add(rectCorner[3] + (gap + objSize) * i * X);
            }
            for (int i = 1 ; i<= number-1 ;i++)
            {
                rectPosition.Add(rectCorner[0] + (gap + objSize) * i * Y);
            }
            for (int i = 1 ; i<= number-1 ;i++)
            {
                rectPosition.Add(rectCorner[1] + (gap + objSize) * i * Y);
            }

        }
       else if(Frame == "circle"){
            circlePosition.Clear();
            for (int i = 0; i < number ; i++){
                float x = R * Mathf.Cos((360f / number * i) * Mathf.Deg2Rad); //确定x坐标
                float z = R * Mathf.Sin((360f / number * i) * Mathf.Deg2Rad); //确定z坐标
                Vector3 now = center + right * x + forward * z;
                circlePosition.Add(now);
                
            }
       }else if(Frame == "tri"){

            var X = (triCorner[1]-triCorner[0]).normalized;
            var Y = (triCorner[2]-triCorner[1]).normalized;
            var Z = (triCorner[2]-triCorner[0]).normalized;
            triPosition.Clear();
            for(int i = 1 ; i<= number-1 ;i++)
            {
                triPosition.Add(triCorner[0] + (gap + objSize) * i * X);
            }
            for (int i = 1 ; i<= number-1 ;i++)
            {
                triPosition.Add(triCorner[1] + (gap + objSize) * i * Y);
            }
            for (int i = 1 ; i<= number-1 ;i++)
            {
                triPosition.Add(triCorner[0] + (gap + objSize) * i * Z);
            }


       }else if(Frame == "para")
       {

            var X = (paraCorner[1]-paraCorner[0]).normalized;
            var Y = (paraCorner[3]- paraCorner[0]).normalized;
            paraPosition.Clear();
            for(int i = 1 ; i<= number-1 ;i++)
            {
                paraPosition.Add(paraCorner[0] + (gap + objSize) * i * X);
            }
            for (int i = 1 ; i<= number-1 ;i++)
            {
                paraPosition.Add(paraCorner[3] + (gap + objSize) * i * X);
            }
            for (int i = 1 ; i<= number-1 ;i++)
            {
                paraPosition.Add(paraCorner[0] + (gap + objSize) * i * Y);
            }
            for (int i = 1 ; i<= number-1 ;i++)
            {
                paraPosition.Add(paraCorner[1] + (gap + objSize) * i * Y);
            }

       }else if(Frame == "pen")
       {
            var e1 = (penCorner[1]-penCorner[0]).normalized;
            var e2 = (penCorner[2]- penCorner[1]).normalized;
            var e3 = (penCorner[3]-penCorner[2]).normalized;
            var e4 = (penCorner[4]- penCorner[3]).normalized;
            var e5 = (penCorner[0]- penCorner[4]).normalized;
            penPosition.Clear();
            for(int i = 1 ; i<= number-1 ;i++)
            {
                paraPosition.Add(penCorner[0] + (gap + objSize) * i * e1);
            }
            for (int i = 1 ; i<= number-1 ;i++)
            {
                paraPosition.Add(penCorner[1] + (gap + objSize) * i * e2);
            }
            for (int i = 1 ; i<= number-1 ;i++)
            {
                paraPosition.Add(penCorner[2] + (gap + objSize) * i * e3);
            }
            for (int i = 1 ; i<= number-1 ;i++)
            {
                paraPosition.Add(penCorner[3] + (gap + objSize) * i * e4);
            }
            for (int i = 1 ; i<= number-1 ;i++)
            {
                paraPosition.Add(penCorner[4] + (gap + objSize) * i * e5);
            }

       }else if(Frame == "cube")
       {

       }else if(Frame == "star")
       {

       }




    }
    void Start(){
        line = GetComponent<LineRenderer>();
        line2 = line_2.GetComponent<LineRenderer>();
        line3 = line_3.GetComponent<LineRenderer>();
        line4 = line_4.GetComponent<LineRenderer>();
        line5 = line_5.GetComponent<LineRenderer>();
        line6 = line_6.GetComponent<LineRenderer>();
        LineSetProperties(line);
        LineSetProperties(line2);
        LineSetProperties(line3);
        LineSetProperties(line4);
        LineSetProperties(line5);
        LineSetProperties(line6);
    }
    public Vector3 center;
    public List<Vector3> rectPosition = new List<Vector3>();
    public List<Vector3> circlePosition = new List<Vector3>();
    public List<Vector3> triPosition = new List<Vector3>();
    public List<Vector3> penPosition = new List<Vector3>();
    public List<Vector3> starPosition = new List<Vector3>();
    public List<Vector3> cubePosition = new List<Vector3>();
    public List<Vector3> paraPosition = new List<Vector3>();


    public void creatRect(){
        Frame = "rect";
        clear();
        resize();


      //  Debug.Log(rectheight);
        dis = 0.4f;
        rectCorner = new Vector3[4];

        forward = head.transform.forward.normalized;
        //right = new Vector3(head.transform.right.normalized.x,0, head.transform.right.normalized.z).normalized;
        right = Vector3.right;
        up = Vector3.up;

        center = head.transform.position + forward * dis - up * 0.2f;

        forward = Vector3.Cross(right, up).normalized;


        line.positionCount = 4;
        rectCorner[0] = center + forward /2*rectheight-right/2*rectlenth;
        rectCorner[1] = center + forward / 2*rectheight+right/2*rectlenth;
        rectCorner[2] = center - forward / 2*rectheight+right/2*rectlenth;
        rectCorner[3] = center - forward / 2*rectheight-right/2*rectlenth;

        decideEachPosition();

        for(int i=0;i<=3;i++){
            cor[i].transform.position = rectCorner[i];
        }


        for (int i = 0;i<=3;i++){
            line.SetPosition(i,rectCorner[i]);
            if(i!=3){
               addColliderToLine(rectCorner[i],rectCorner[i+1]);
            }
            else{
                addColliderToLine(rectCorner[3],rectCorner[0]);
            }
        }
    }
    public void createCircle()
    {
        Frame = "circle";
        clear();
        resize();
        dis = 0.4f;
        N = 40;

        collideObject.GetComponent<collide>().mark = 0;


        forward = head.transform.forward.normalized;
        // right = new Vector3(head.transform.right.normalized.x, 0, head.transform.right.normalized.z).normalized;
        right = Vector3.right;

        up = Vector3.up;

        center = head.transform.position + forward * dis - up * 0.2f;

        forward = Vector3.Cross(right, up).normalized;

        line.positionCount = N + 1;

        Vector3 last = new Vector3(0, 0, 0);
        Vector3 now;

        for (int i = 0; i <= N ; i++)
        {
            float x = R * Mathf.Cos((360f / N * i) * Mathf.Deg2Rad); //确定x坐标
            float z = R * Mathf.Sin((360f / N * i) * Mathf.Deg2Rad); //确定z坐标
            now = center + right * x + forward * z;
            line.SetPosition(i, now);
            if (i != 0)
                addColliderToLine(last, now);
            last = now;
        }
    }
    public void createTri()
    {
        dis = 0.4f;

        Frame = "tri";
        clear();
        resize();
        rectCorner = new Vector3[3];

        forward = head.transform.forward.normalized;
        // right = new Vector3(head.transform.right.normalized.x, 0, head.transform.right.normalized.z).normalized;
        right = Vector3.right;

        up = Vector3.up;

        center = head.transform.position + forward * dis - up * 0.2f;

        forward = Vector3.Cross(right, up).normalized;

        line.positionCount = 3;

        rectCorner[0] = center + forward * (float)(triedge / Math.Sqrt(3));
        rectCorner[1] = center - forward * (float)(triedge / Math.Sqrt(3) / 2) - right * ( triedge /2);
        rectCorner[2] = center - forward * (float)(triedge / Math.Sqrt(3) / 2) + right * ( triedge /2);


        for(int i=0;i<=2;i++){
            cor[i].transform.position = rectCorner[i];
        }

        for(int i = 0;i<=2;i++){
            line.SetPosition(i,rectCorner[i]);
            if(i!=2){
                addColliderToLine(rectCorner[i],rectCorner[i+1]);
            }
            else{
                addColliderToLine(rectCorner[2],rectCorner[0]);
            }
        }

    }
    public void createPentagon()
    {
        dis = 0.4f;

        //triangle original
        penedge = 0.3f;//不是边而是顶点到中心距离

        Frame = "pen";
        clear();
        resize();

        penCorner = new Vector3[5];

        forward = head.transform.forward.normalized;
        // right = new Vector3(head.transform.right.normalized.x, 0, head.transform.right.normalized.z).normalized;
        right = Vector3.right;

        up = Vector3.up;

        center = head.transform.position + forward * dis - up * 0.2f;

        forward = Vector3.Cross(right, up).normalized;

        line.positionCount = 5;

        penCorner[0] = center + forward * penedge ;
        penCorner[1] = center + forward * penedge * Mathf.Cos((72) * Mathf.Deg2Rad)  + right * penedge * Mathf.Sin((72) * Mathf.Deg2Rad);
        penCorner[2] = center - forward * penedge / 2 / Mathf.Cos((36) * Mathf.Deg2Rad) + right * penedge * Mathf.Sin((36) * Mathf.Deg2Rad);
        penCorner[3] = center - forward * penedge / 2 / Mathf.Cos((36) * Mathf.Deg2Rad) - right * penedge * Mathf.Sin((36) * Mathf.Deg2Rad);
        penCorner[4] = center + forward * penedge * Mathf.Cos((72) * Mathf.Deg2Rad) - right * penedge * Mathf.Sin((72) * Mathf.Deg2Rad);


        for (int i = 0; i <= 4; i++)
        {
            line.SetPosition(i, penCorner[i]);
            if (i != 4){
                addColliderToLine(penCorner[i], penCorner[i + 1]);
            }
            else{
                addColliderToLine(penCorner[4], penCorner[0]);
            }
        }
    }
/*
    public void createStar()
{

        dis = 0.4f;

        staredge = 0.3f;//不是边而是顶点到中心距离

        Frame = "star";
        clear();
        resize();

        starCorner = new Vector3[5];

        forward = head.transform.forward.normalized;
        // right = new Vector3(head.transform.right.normalized.x, 0, head.transform.right.normalized.z).normalized;
        right = Vector3.right;

        up = Vector3.up;

        center = head.transform.position + forward * dis - up * 0.2f;

        forward = Vector3.Cross(right, up).normalized;

        line.positionCount = 5;


line.SetPosition(0, penCorner[4]);
line.SetPosition(1, penCorner[1]);
line.SetPosition(2, penCorner[3]);
line.SetPosition(3, penCorner[0]);
line.SetPosition(4, penCorner[2]);
addColliderToLine(penCorner[4], penCorner[1]);
addColliderToLine(penCorner[1], penCorner[3]);
addColliderToLine(penCorner[3], penCorner[0]);
addColliderToLine(penCorner[0], penCorner[2]);
addColliderToLine(penCorner[2], penCorner[4]);

}
*/

    public void createPara()
    {
        dis = 0.4f;

        //pingxing
        angle = 45;

        Frame = "para";
        clear();
        resize();

        paraCorner = new Vector3[4];

        forward = head.transform.forward.normalized;
        // right = new Vector3(head.transform.right.normalized.x, 0, head.transform.right.normalized.z).normalized;
        right = Vector3.right;

        up = Vector3.up;

        center = head.transform.position + forward * dis - up * 0.2f;

        forward = Vector3.Cross(right, up).normalized;
        float sinx = Mathf.Cos((angle) * Mathf.Deg2Rad);
        float cosx = Mathf.Cos((angle) * Mathf.Deg2Rad);
        line.positionCount = 5;
        rectCorner[0] = center + forward / 2 * rectheight * sinx - right / 2 * (rectlenth - cosx);
        rectCorner[1] = center + forward / 2 * rectheight * sinx - right / 2 * (rectlenth + cosx);
        rectCorner[2] = center - forward / 2 * rectheight * sinx + right / 2 * (rectlenth - cosx);
        rectCorner[3] = center - forward / 2 * rectheight - right / 2 * (rectlenth + cosx);

        for (int i = 0; i <= 3; i++)
        {
            cor[i].transform.position = rectCorner[i];
        }
        for (int i = 0; i <= 3; i++)
        {
            line.SetPosition(i, rectCorner[i]);
            if (i != 3)
            {
                addColliderToLine(rectCorner[i], rectCorner[i + 1]);
            }
            else
            {
                Debug.Log(1);
                addColliderToLine(rectCorner[3], rectCorner[0]);
                line.SetPosition(4, rectCorner[0]);
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

        cubeCorner = new Vector3[8];

        Frame = "cube";
        clear();
        resize();

        forward = head.transform.forward.normalized;
        // right = new Vector3(head.transform.right.normalized.x, 0, head.transform.right.normalized.z).normalized;
        right = Vector3.right;

        // up = head.transform.up.normalized;
        up = Vector3.up;


        center = head.transform.position + forward * dis;
        forward = Vector3.Cross(right, up).normalized;

        line.positionCount = 4;
        line2.positionCount = 4;

        cubeCorner[0] = center + forward / 2 * cubeheight - right / 2 * cubelenth + up * cubewidth / 2;
        cubeCorner[1] = center + forward / 2 * cubeheight + right / 2 * cubelenth + up * cubewidth / 2;
        cubeCorner[2] = center - forward / 2 * cubeheight + right / 2 * cubelenth + up * cubewidth / 2;
        cubeCorner[3] = center - forward / 2 * cubeheight - right / 2 * cubelenth + up * cubewidth / 2;

        cubeCorner[4] = center + forward / 2 * cubeheight - right / 2 * cubelenth - up * cubewidth / 2;
        cubeCorner[5] = center + forward / 2 * cubeheight + right / 2 * cubelenth - up * cubewidth / 2;
        cubeCorner[6] = center - forward / 2 * cubeheight + right / 2 * cubelenth - up * cubewidth / 2;
        cubeCorner[7] = center - forward / 2 * cubeheight - right / 2 * cubelenth - up * cubewidth / 2;
        for (int i = 0; i <= 7; i++)
        {
            cor[i].transform.position = cubeCorner[i];
        }
        for (int i = 0; i <= 3; i++)
        {
            line.SetPosition(i, cubeCorner[i]);
            if (i != 3)
            {
                addColliderToLine(cubeCorner[i], cubeCorner[i + 1]);
            }
            else
            {
                addColliderToLine(cubeCorner[3], cubeCorner[0]);
            }
        }

        for (int i = 4; i <= 7; i++)
        {
            line2.SetPosition(i - 4, cubeCorner[i]);
            if (i != 7)
            {
                addColliderToLine(cubeCorner[i], cubeCorner[i + 1]);
            }
            else
            {
                addColliderToLine(cubeCorner[7], cubeCorner[4]);
            }
        }
        line3.SetPosition(0, cubeCorner[0]);
        line3.SetPosition(1, cubeCorner[4]);
        addColliderToLine(cubeCorner[0], cubeCorner[4]);
        line4.SetPosition(0, cubeCorner[1]);
        line4.SetPosition(1, cubeCorner[5]);
        addColliderToLine(cubeCorner[1], cubeCorner[5]);
        line5.SetPosition(0, cubeCorner[2]);
        line5.SetPosition(1, cubeCorner[6]);
        addColliderToLine(cubeCorner[2], cubeCorner[6]);
        line6.SetPosition(0, cubeCorner[3]);
        line6.SetPosition(1, cubeCorner[7]);
        addColliderToLine(cubeCorner[3], cubeCorner[7]);
    }

    public void updateFrame()
    {
        var anchor = collideObject.GetComponent<collide>().anchor;
        if(Frame == "rect"){//根据透明物体来画
            redoRect();}
        else if(Frame == "circle"){//用三个锚点画
            redoCircle(anchor);}
        else if(Frame == "tri"){
            redoTri();}
        else if(Frame == "pen"){
            redoPen();}
        else if(Frame == "para"){
            redoPara();}

    }
    public void redoRect(){
        line.positionCount = 4;
        for (int i = 0; i <= 3; i++){
            line.SetPosition(i, cor[i].transform.position);
        }
    }
    public void redoCircle(List<GameObject> anchor)
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
    public void redoTri()
    {
            line.positionCount = 3;
            for(int i = 0;i<=2;i++){
            line.SetPosition(i,cor[i].transform.position);
        }
    }
    public void redoPen()
    {
            line.positionCount = 5;
            for(int i = 0;i<=5;i++){
            line.SetPosition(i,cor[i].transform.position);
        }
    }
    public void redoStar()
    {
            line.positionCount = 5;
            for(int i = 0;i<=5;i++){
            line.SetPosition(i,cor[i].transform.position);
        }
    }
    public void redoPara()
    {
            line.positionCount = 4;
            for(int i = 0;i<=3;i++){
            line.SetPosition(i,cor[i].transform.position);
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
        col.transform.parent = line.transform; // Collider is added as child object of line
        float lineLength = Vector3.Distance(startPos, endPos); // length of line
        col.size = new Vector3(lineLength, 0.001f, 0.001f); // size of collider is set where X is length of line, Y is width of line, Z will be set as per requirement
        Vector3 midPoint = (startPos + endPos) / 2;
        col.transform.position = midPoint; // setting position of collider object
        // Following lines calculate the angle between startPos and endPos
        float angle = ( Mathf.Abs(startPos.z - endPos.z)/Mathf.Abs(startPos.x - endPos.x));
        if ((startPos.z < endPos.z && startPos.x < endPos.x) || (endPos.z <startPos.z && endPos.x < startPos.x))
        {
            angle *= -1;
        }
        angle = Mathf.Rad2Deg * Mathf.Atan(angle);

        col.transform.Rotate(0, angle,0);
    }
    void clear()
    {
        for (int i = 0; i <= 7; i++)
        {
            collideObject.GetComponent<collide>().rectMark[i] = 0;
            collideObject.GetComponent<collide>().triMark[i] = 0;
            // collideObject.GetComponent<collide>().circleMark[i] = 0;
            collideObject.GetComponent<collide>().paraMark[i] = 0;
            collideObject.GetComponent<collide>().penMark[i] = 0;
            collideObject.GetComponent<collide>().cubeMark[i] = 0;
        }
    }
    public void LineSetProperties(LineRenderer line)
    {
        line.startWidth = 0.02f;
        line.endWidth = 0.02f;
        line.startColor = Color.black;
        line.endColor = Color.black;
        line.material = lineMaterial;
    }
    /*public void redoPara(string type)//两个锚点，动其中一个的时候另一个不动
  {
      List<GameObject> anchor = collideObject.GetComponent<collide>().anchor;

              // get the original plane by previous points
      Vector3 f = ((paraCorner[0]-paraCorner[1])).normalized;
      Vector3 m = ((paraCorner[0]-paraCorner[3])).normalized;
      if(type == "1")// 左上，右下
      {
          paraCorner[0] = anchor[0].transform.position;
          paraCorner[2] = anchor[1].transform.position;
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


  }*/
}
