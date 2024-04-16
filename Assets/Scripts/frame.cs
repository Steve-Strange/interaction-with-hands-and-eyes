using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class frame : MonoBehaviour
{
    public LineRenderer line;
    public GameObject connectorManager;
    public GameObject[] cor;//透明物体，其位置用于标记顶点；
    public GameObject collideObject;
    // public TMP_Text t;

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
    public float objSize;
    void resize()//change the frame size due to number
    {
        float averageSize = 0f;
        foreach (var obj in collideObject.GetComponent<collide>().finalObj)
        {
            averageSize += (obj.transform.GetComponent<Renderer>().bounds.size.x + obj.transform.GetComponent<Renderer>().bounds.size.y + 
                            obj.transform.GetComponent<Renderer>().bounds.size.z) / 3;
        }

        objSize = averageSize / collideObject.GetComponent<collide>().finalObj.Count;
        number = 3;
        gap =  objSize;

        //大小应该和尺寸以及数量有关
       if(Frame == "rect"){
            //number 表示每个边上的object + gap数量
            number = Mathf.CeilToInt((collideObject.GetComponent<collide>().finalObj.Count-4)/4f) + 1;//todo 改其他边的计算方式
            if(number <= 0)
                number = 1;
            rectheight = (objSize + gap) * number;
            rectlenth =  (objSize + gap) * number;
       }else if(Frame == "circle"){
            number = collideObject.GetComponent<collide>().finalObj.Count;
            if(number == 0)
                number = 1;
            R = number * (objSize + gap)/(2*pi);
       }else if(Frame == "tri"){
            number = Mathf.CeilToInt((collideObject.GetComponent<collide>().finalObj.Count-3) / 3f) + 1;
            if(number == 0)
                number = 1;
            triedge = (objSize + gap) * number;
       }else if(Frame == "para")
       {
            number = Mathf.CeilToInt((collideObject.GetComponent<collide>().finalObj.Count - 4) / 4f) + 1;
            if (number == 0)
                number = 1;
            paraheight = (objSize + gap) * number;
            paralenth = (objSize + gap) * number;
        }
        else if(Frame == "pen")
        {
            number = Mathf.CeilToInt((collideObject.GetComponent<collide>().finalObj.Count -5)/ 5f) + 1;
            if(number == 0)
                number = 1;
            penedge = (objSize + gap) * number / (2 *  Mathf.Cos((54) * Mathf.Deg2Rad) ); 
        }
        else if(Frame == "cube")
        {
            number = Mathf.CeilToInt((collideObject.GetComponent<collide>().finalObj.Count - 12) / 12f) + 1;
            if(number == 0)
                number = 1;
            cubeheight = (objSize + gap) * number;
            cubelenth = (objSize + gap) * number;
            cubelenth = (objSize + gap) * number;
            
        }else if(Frame == "star")
        {
            number = Mathf.CeilToInt((collideObject.GetComponent<collide>().finalObj.Count - 5) / 5f) + 1;
            if(number == 0)
                number = 1;
            penedge = (objSize + gap) * number / (2 *  Mathf.Cos((54) * Mathf.Deg2Rad) ); 
        }
    }
    void decideEachPosition()//change the frame size from number
    {
       // int Min = 2;
        int Max = 2 * number;
       if(Frame == "rect"){//todo 改其他的
            var X = (rectCorner[1]- rectCorner[0]).normalized;
            var Y = (rectCorner[3]- rectCorner[0]).normalized;
            rectPosition.Clear();
            for(int j = Max;j<=Max;j++)
            {
                float temp  = rectheight * 1.0f / (j * 1.0f);
                for (int i = 1; i <= j - 1; i++)
                {
                    rectPosition.Add(rectCorner[0] + temp * i * X);
                }
                for (int i = 1; i <= j - 1; i++)
                {
                    rectPosition.Add(rectCorner[3] + temp * i * X);
                }
                for (int i = 1; i <= j - 1; i++)
                {
                    rectPosition.Add(rectCorner[0] + temp * i * Y);
                }
                for (int i = 1; i <= j - 1; i++)
                {
                    rectPosition.Add(rectCorner[1] + temp * i * Y);
                }
            }
        }
       else if(Frame == "circle"){//已改
            circlePosition.Clear();
            for (int j = Max; j <= Max; j++){
                for (int i = 0; i < j ; i++){
                    float x = R * Mathf.Cos((360f / number * i) * Mathf.Deg2Rad); //确定x坐标
                    float z = R * Mathf.Sin((360f / number * i) * Mathf.Deg2Rad); //确定z坐标
                    Vector3 now = center + right * x + forward * z;
                    circlePosition.Add(now);
                }
            }

       }else if(Frame == "tri"){

            var X = (triCorner[1]-triCorner[0]).normalized;
            var Y = (triCorner[2]-triCorner[1]).normalized;
            var Z = (triCorner[2]-triCorner[0]).normalized;
            triPosition.Clear();
            for (int j = Max; j <= Max; j++)
            {
                float temp = triedge * 1.0f / (j * 1.0f);
                for (int i = 1 ; i<= j-1 ;i++)
                {
                    triPosition.Add(triCorner[0] + temp * i * X);
                }
                for (int i = 1 ; i<= j-1 ;i++)
                {
                    triPosition.Add(triCorner[1] + temp * i * Y);
                }
                for (int i = 1 ; i<= j -1 ;i++)
                {
                    triPosition.Add(triCorner[0] + temp * i * Z);
                }
            }
       }else if(Frame == "para")
       {

            var X = (paraCorner[1]-paraCorner[0]).normalized;
            var Y = (paraCorner[3]- paraCorner[0]).normalized;
            paraPosition.Clear();
            for(int i = 1 ; i<= Max-1 ;i++)
            {
                paraPosition.Add(paraCorner[0] + (gap + objSize) * i * X);
            }
            for (int i = 1 ; i<= Max - 1 ;i++)
            {
                paraPosition.Add(paraCorner[3] + (gap + objSize) * i * X);
            }
            for (int i = 1 ; i<= Max -1 ;i++)
            {
                paraPosition.Add(paraCorner[0] + (gap + objSize) * i * Y);
            }
            for (int i = 1 ; i<= Max -1 ;i++)
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
                float temp = penedge * 1.0f / (Max * 1.0f);
                for (int i = 1 ; i<= Max - 1 ;i++)
                {
                penPosition.Add(penCorner[0] + temp * i * e1);
                }
                for (int i = 1 ; i<= Max - 1 ;i++)
                {
                penPosition.Add(penCorner[1] + temp * i * e2);
                }
                for (int i = 1 ; i<= Max - 1 ;i++)
                {
                    penPosition.Add(penCorner[2] + temp * i * e3);
                }
                for (int i = 1 ; i<= Max - 1 ;i++)
                {
                    penPosition.Add(penCorner[3] + temp * i * e4);
                }
                for (int i = 1 ; i<= Max - 1;i++)
                {
                    penPosition.Add(penCorner[4] + temp * i * e5);
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
        //每次选择都当作重新开始，第一次放东西之后，框消失
        DestroyColliders();
        collideObject.GetComponent<collide>().first = true;

        Frame = "rect";
        gameObject.GetComponent<LineRenderer>().endWidth = 0.01f;
        gameObject.GetComponent<LineRenderer>().startWidth = 0.01f;
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
        rectCorner[0] = center + forward / 2*rectheight-right/2*rectlenth;
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
        DestroyColliders();
        collideObject.GetComponent<collide>().first = true;
        Frame = "circle";
        gameObject.GetComponent<LineRenderer>().endWidth = 0.01f;
        gameObject.GetComponent<LineRenderer>().startWidth = 0.01f;
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

        decideEachPosition();

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
        DestroyColliders();
        collideObject.GetComponent<collide>().first = true;
        Frame = "tri";
        gameObject.GetComponent<LineRenderer>().endWidth = 0.01f;
        gameObject.GetComponent<LineRenderer>().startWidth = 0.01f;
        clear();
        resize();
        dis = 0.4f;
        triCorner = new Vector3[3];

        forward = head.transform.forward.normalized;
        // right = new Vector3(head.transform.right.normalized.x, 0, head.transform.right.normalized.z).normalized;
        right = Vector3.right;

        up = Vector3.up;

        center = head.transform.position + forward * dis - up * 0.2f;

        forward = Vector3.Cross(right, up).normalized;

        line.positionCount = 3;

        triCorner[0] = center + forward * (float)(triedge / Math.Sqrt(3));
        triCorner[1] = center - forward * (float)(triedge / Math.Sqrt(3) / 2) - right * ( triedge /2);
        triCorner[2] = center - forward * (float)(triedge / Math.Sqrt(3) / 2) + right * ( triedge /2);


        for(int i=0;i<=2;i++){//拿来画的虚拟物体
            cor[i].transform.position = triCorner[i];
        }

        decideEachPosition();

        for (int i = 0;i<=2;i++){
            line.SetPosition(i,triCorner[i]);
            if(i!=2){
                addColliderToLine(triCorner[i],triCorner[i+1]);
            }
            else{
                addColliderToLine(triCorner[2],triCorner[0]);
            }
        }

    }
    public void createPentagon()
    {
        DestroyColliders();
        collideObject.GetComponent<collide>().first = true;
        dis = 0.4f;
        gameObject.GetComponent<LineRenderer>().endWidth = 0.01f;
        gameObject.GetComponent<LineRenderer>().startWidth = 0.01f;
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
        {//拿来画的虚拟物体
            cor[i].transform.position = penCorner[i];
        }

        decideEachPosition();

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

    public void createStar()
{
        DestroyColliders();
        collideObject.GetComponent<collide>().first = true;
        gameObject.GetComponent<LineRenderer>().endWidth = 0.01f;
        gameObject.GetComponent<LineRenderer>().startWidth = 0.01f;
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

        starCorner[0] = center + forward * staredge;
        starCorner[1] = center + forward * staredge * Mathf.Cos((72) * Mathf.Deg2Rad) + right * staredge * Mathf.Sin((72) * Mathf.Deg2Rad);
        starCorner[2] = center - forward * staredge / 2 / Mathf.Cos((36) * Mathf.Deg2Rad) + right * staredge * Mathf.Sin((36) * Mathf.Deg2Rad);
        starCorner[3] = center - forward * staredge / 2 / Mathf.Cos((36) * Mathf.Deg2Rad) - right * staredge * Mathf.Sin((36) * Mathf.Deg2Rad);
        starCorner[4] = center + forward * staredge * Mathf.Cos((72) * Mathf.Deg2Rad) - right * staredge * Mathf.Sin((72) * Mathf.Deg2Rad);

        line.positionCount = 5;
        for (int i = 0; i <= 4; i++)
        {//拿来画的虚拟物体
            cor[i].transform.position = starCorner[i];
        }

        decideEachPosition();


        line.SetPosition(0, starCorner[4]);
        line.SetPosition(1, starCorner[1]);
        line.SetPosition(2, starCorner[3]);
        line.SetPosition(3, starCorner[0]);
        line.SetPosition(4, starCorner[2]);
        addColliderToLine(starCorner[4], starCorner[1]);
        addColliderToLine(starCorner[1], starCorner[3]);
        addColliderToLine(starCorner[3], starCorner[0]);
        addColliderToLine(starCorner[0], starCorner[2]);
        addColliderToLine(starCorner[2], starCorner[4]);

}
/**/
    public void createPara()
    {
        DestroyColliders();
        collideObject.GetComponent<collide>().first = true;
        dis = 0.4f;
        gameObject.GetComponent<LineRenderer>().endWidth = 0.01f;
        gameObject.GetComponent<LineRenderer>().startWidth = 0.01f;
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

        DestroyColliders();
        collideObject.GetComponent<collide>().first = true;
        dis = 0.6f;
        gameObject.GetComponent<LineRenderer>().endWidth = 0.01f;
        gameObject.GetComponent<LineRenderer>().startWidth = 0.01f;
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

    public void updateFrame(){
        gameObject.GetComponent<LineRenderer>().enabled = true;
        gameObject.GetComponent<LineRenderer>().endWidth = 0.01f;
        gameObject.GetComponent<LineRenderer>().startWidth = 0.01f;
        var anchor = collideObject.GetComponent<collide>().anchor;
        setLines();
        if (Frame == "rect"){//根据透明物体来画
            redoRect();}
        else if(Frame == "circle"){//用三个锚点画
            redoCircle(anchor);}
        else if(Frame == "tri"){
            redoTri();}
        else if(Frame == "pen"){
            redoPen();}
        else if(Frame == "para"){
            redoPara();}
        gameObject.GetComponent<LineRenderer>().endWidth = 0.01f;
        gameObject.GetComponent<LineRenderer>().startWidth = 0.01f;
    }
    public void redoRect(){
        //setLines();
        line.positionCount = 4;
        for (int i = 0; i <= 3; i++){
            line.SetPosition(i, cor[i].transform.position);
        }
    }
    public void redoCircle(List<GameObject> anchor)
    {
        //setLines();
        Vector3 centerr = CalculateTriangleOutCircleCenter(anchor[0].transform.position, anchor[1].transform.position, anchor[2].transform.position);

        // t.text = centerr.ToString()+anchor[0].transform.name + ":    " + anchor[0].transform.position +" \n"+ anchor[1].transform.name+ ":    " + anchor[1].transform.position +" \n"+ anchor[2].transform.name+ ":    " + anchor[2].transform.position;
        float R = (anchor[0].transform.position - centerr).magnitude;


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
            now = centerr + right * x + forward * z;
            line.SetPosition(i, now);
        }

    }
    public void redoTri()
    {
        setLines();
        line.positionCount = 3;
            for(int i = 0;i<=2;i++){
            line.SetPosition(i,cor[i].transform.position);
        }
    }
    public void redoPen()
    {
        setLines();
        line.positionCount = 5;
            for(int i = 0;i<=5;i++){
            line.SetPosition(i,cor[i].transform.position);
        }
    }
    public void redoStar()
    {
        setLines();
        line.positionCount = 5;
            for(int i = 0;i<=5;i++){
            line.SetPosition(i,cor[i].transform.position);
        }
    }
    public void redoPara()
    {
        setLines();
        line.positionCount = 4;
            for(int i = 0;i<=3;i++){
            line.SetPosition(i,cor[i].transform.position);
        }
    }

    public void DestroyColliders(){

        var cols = GameObject.FindGameObjectsWithTag("cols");
        if(cols != null)
        {
        foreach(var col in cols)
        {
            Destroy(col.gameObject);
        }

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
    private void addColliderToLine(Vector3 startPos,Vector3 endPos)
    {

        BoxCollider col = new GameObject("Edge").AddComponent<BoxCollider>();
        col.tag = "cols";
        col.transform.parent = line.transform; // Collider is added as child object of line
        float lineLength = Vector3.Distance(startPos, endPos); // length of line
        col.size = new Vector3(lineLength, 0.02f, 0.02f); // size of collider is set where X is length of line, Y is width of line, Z will be set as per requirement
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
    void clear(){
        collideObject.GetComponent<collide>().onFrame.Clear();//清除留在框上的物体
        for (int i = 0; i <= 7; i++)
        {
            collideObject.GetComponent<collide>().rectMark[i] = 0;
            collideObject.GetComponent<collide>().triMark[i] = 0;
            //collideObject.GetComponent<collide>().circleMark[i] = 0;
            collideObject.GetComponent<collide>().paraMark[i] = 0;
            collideObject.GetComponent<collide>().penMark[i] = 0;
            collideObject.GetComponent<collide>().cubeMark[i] = 0;
        }
    }
    public void setLines()
    {
        LineSetProperties(line);
        LineSetProperties(line2);
        LineSetProperties(line3);
        LineSetProperties(line4);
        LineSetProperties(line5);
    }
    public void LineSetProperties(LineRenderer line)
    {
        line.startWidth = 0.01f;
        line.endWidth = 0.01f;
        line.startColor = Color.black;
        line.endColor = Color.black;
        line.material = lineMaterial;
    }
    public void AddOutline(GameObject target, Color color)
    {
        if (target.GetComponent<Outline>() == null)
        {
            target.AddComponent<Outline>();
        }
        target.GetComponent<Outline>().OutlineColor = color;
    }
}
