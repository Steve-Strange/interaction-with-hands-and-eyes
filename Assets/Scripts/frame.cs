using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class frame : MonoBehaviour
{
    // Start is called before the first frame update
    private LineRenderer line;
    private Vector3 forward;//ʵ�ֳ���
    private Vector3 right;
    private Vector3 up;
    public GameObject head;
    public float dis = 0.01f;
    private Mesh mesh;
    public float lenth;
    public float width;
    public float height;
    public Camera camera;
    public float R = 0.1f;
    public int N = 40;
    public GameObject FinalObjects;

    Vector3[] points = new Vector3[20];
    Vector3 center;
    
    public void creatRect()//2d
    {
   
        points = new Vector3[4];

        forward = head.transform.forward.normalized;
        right = head.transform.right.normalized;
        up = head.transform.up.normalized;

        center = head.transform.position + forward * dis;

        line.positionCount=4;
        
        points[0]=(new Vector3(0, 0, 0));
        points[1] = (new Vector3(0, 1, 0));
        points[2]=(new Vector3(1, 1, 0));
        points[3]=(new Vector3(1, 0, 0));
        line.SetPosition(0, new Vector3(0, 0, 0));
        line.SetPosition(1, new Vector3(0, 1, 0));
        line.SetPosition(2, new Vector3(1, 1, 0));
        line.SetPosition(3, new Vector3(1, 0, 0));

        addColliderToLine(new Vector3(0, 0, 0), new Vector3(0, 1, 0));
            addColliderToLine(new Vector3(0, 1, 0), new Vector3(1, 1, 0));
            addColliderToLine(new Vector3(1, 1, 0), new Vector3(1, 0, 0));
        addColliderToLine(new Vector3(1, 0, 0), new Vector3(0, 0, 0));
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
