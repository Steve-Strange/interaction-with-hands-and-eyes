using System.Numerics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class frame : MonoBehaviour
{
    private LineRenderer line;

    private Vector3 forward;//ʵ�ֳ���
    private Vector3 right;
    private Vector3 up;
    public float dis = 0.01f;
    public GameObject head;
    
    //rect
    public float rectlenth = 1f;
    public float rectheight= 1f;
    public Vector3[] rectCorner;// save the for corner
    //circle
    public float height;
    public Camera camera;
    public float R = 0.1f;
    public int N = 40;

    
    public GameObject[] collider;//存添加的所有collider物体；

    Vector3[] points = new Vector3[20];
    Vector3 center;
    
    public void creatRect()//2d, just make the origin frame
    {
   
        frame = 'rect';
        points = new Vector3[4];

        forward = head.transform.forward.normalized;
        right = head.transform.right.normalized;
        up = head.transform.up.normalized;

        center = head.transform.position + forward * dis;

        line.positionCount = 4;
        rectCorner[0] = center + up/2*rectheight-right/2*rectlenth;
        rectCorner[1] = center + up/2*rectheight+right/2*rectlenth;
        rectCorner[2] = center - up/2*rectheight-right/2*rectlenth;
        rectCorner[3] = center - up/2*rectheight+right/2*rectlenth;
        for(int i = 0;i<=3;i++){
            line.SetPosition(i,rectCorner[i]);
            if(i!=3){
                addColliderToLine(rectCorner[i],rectCorner[i+1]);
            }
            else{
                addColliderToLine(rectCorner[3],rectCorner[0]);
            }
        }
    }
    //距离等比例变化
    public void redoRect(string type,)//visualize the frame by object position
    {
        // get the original plane by previous points
        Vector3 f = ((rectCorner[0]-rectCorner[1])).normalized;
        Vector3 m = ((rectCorner[0]-rectCorner[3])).normalized;
        if(type = "1")// 左上，右下
        {   rectCorner[0] = anchor[0];
            rectCorner[2] = anchor[1];
            Vector3 temp = rectCorner[0]-rectCorner[2];
            rectheight = temp * m;
            rectlenth = temp * f;
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
    private void resizeColliderToline(GameObject col, Vector3 startPos,Vector3 endPos)
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
        BoxCollider col = new GameObject("Collider").AddComponent<BoxCollider>();
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

    public void creatTrian()
    {

    }

    public void createBall()
    {



    }


    public void createCircle()
    {   
        N = 40;
        line.positionCount = N+1;
        R = 0.1f;
        Vector3 last = new Vector3(0, 0, 0);
        
        for (int i = 0; i < N + 1; i++){
            float x = R * Mathf.Cos((360f / N * i) * Mathf.Deg2Rad)+ transform.position.x; //确定x坐标
            float z = R * Mathf.Sin((360f / N * i) * Mathf.Deg2Rad)+ transform.position.z; //确定z坐标
            Vector3 now = new Vector3(x, 0, z);
            line.SetPosition(i, now);
            if(i!=0)
                addColliderToLine(last, new Vector3(x, transform.position.y, z));
            last = now;
        }
        float xx = R * Mathf.Cos((360f / N * 0) * Mathf.Deg2Rad) + transform.position.x; //确定x坐标
        float zz = R * Mathf.Sin((360f / N * 0) * Mathf.Deg2Rad) + transform.position.z; //确定z坐标
        addColliderToLine(last, new Vector3(xx, transform.position.y, zz));
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
