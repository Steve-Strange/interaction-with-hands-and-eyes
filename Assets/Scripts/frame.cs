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

    Vector3[] points = new Vector3[20];
    PolygonCollider2D polygonCollider;

    public void creatRect()//2d-������
    {
        
        points = new Vector3[20];
        line = GetComponent<LineRenderer>();
        forward = head.transform.forward.normalized;
        right = head.transform.right.normalized;
        up = head.transform.up.normalized;
        Vector3 center = head.transform.position;// + forward * dis;
        line.positionCount=4;
        line.SetPosition(0, new Vector3(0, 0, 0));
        points[0]=(new Vector3(0, 0, 0));
        points[1] = (new Vector3(0, 1, 0));
        points[2]=(new Vector3(1, 1, 0));
        points[3]=(new Vector3(1, 0, 0));
        line.SetPosition(1, new Vector3(0, 1, 0));
        line.SetPosition(2, new Vector3(1, 1, 0));
        line.SetPosition(3, new Vector3(1, 0, 0));
        //line.c
        line.startWidth = (float)0.02;
        line.endWidth = (float)0.02;
        addColliderToLine(new Vector3(0, 0, 0), new Vector3(0, 1, 0));
            addColliderToLine(new Vector3(0, 1, 0), new Vector3(1, 1, 0));
            addColliderToLine(new Vector3(1, 1, 0), new Vector3(1, 0, 0));
        addColliderToLine(new Vector3(1, 0, 0), new Vector3(0, 0, 0));
        // mesh = new Mesh();
        // line.BakeMesh(mesh, camera, true);
        // gameObject.AddComponent<MeshCollider>();
        /*this.GetComponent<MeshCollider>().sharedMesh = mesh;*/
        /*     one.transform.localScale = new Vector3(1, 0.01f, 0.01f);
            one.AddComponent<BoxCollider>();

            two.transform.localScale = new Vector3(0.01f, 1f, 0.01f);
            two.AddComponent<BoxCollider>();

            three.transform.localScale = new Vector3(1, 0.01f, 0.01f);
            three.AddComponent<BoxCollider>();

            four.transform.localScale = new Vector3(0.01f, 1f, 0.01f);
            four.AddComponent<BoxCollider>();
            one.transform.position = new Vector3(0.5f, 0, 0);
            two.transform.position = new Vector3(1, 0.5f, 0);
            three.transform.position = new Vector3(0.5f, 1, 0);
            four.transform.position = new Vector3(0, 0.5f, 0);
            List<Vector2> colliderPath = GetColliderPath(points);
             List<Vector2> one = new List<Vector2>();
             one.Add(new Vector2(0, 0));
             one.Add(new Vector2(0, 1));
             one.Add(new Vector2(1, 1));
             one.Add(new Vector2(1, 0));
             Debug.Log("yes");
             int i = 0;
             for(;i<colliderPath.Count;i++){

                 Debug.Log(colliderPath[i]);
             }
             polygonCollider.SetPath(0, colliderPath.ToArray());
     */
    }
    private void addColliderToLine(Vector3 startPos,Vector3 endPos)
    {
        BoxCollider col = new GameObject("Collider").AddComponent<BoxCollider>();
        col.transform.parent = line.transform; // Collider is added as child object of line
        float lineLength = Vector3.Distance(startPos, endPos); // length of line
        col.size = new Vector3(lineLength, 0.001f, 0.001f); // size of collider is set where X is length of line, Y is width of line, Z will be set as per requirement
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
    public float R;
    public int N;
    public void creatCircle()
    {
        for (int i = 0; i < N + 1; i++)
        {
            float x = R * Mathf.Cos((360f / N * i) * Mathf.Deg2Rad) + transform.position.x; //确定x坐标
            float z = R * Mathf.Sin((360f / N * i) * Mathf.Deg2Rad) + transform.position.z; //确定z坐标
            GetComponent<LineRenderer>().SetPosition(i, new Vector3(x, transform.position.y, z));
        }
    }

    void Start()
    {
        polygonCollider = gameObject.GetComponent<PolygonCollider2D>();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
private void addColliderToLine()
    {
        BoxCollider col = new GameObject("Collider").AddComponent<BoxCollider>();
        col.transform.parent = line.transform; // Collider is added as child object of line
        /*    float lineLength = Vector3.Distance(startPos, endPos); // length of line
        col.size = new Vector3(lineLength, 0.1f, 1f); // size of collider is set where X is length of line, Y is width of line, Z will be set as per requirement
        Vector3 midPoint = (startPos + endPos) / 2;
        col.transform.position = midPoint; // setting position of collider object
        // Following lines calculate the angle between startPos and endPos
        float angle = (Mathf.Abs(startPos.y - endPos.y) / Mathf.Abs(startPos.x - endPos.x));
        if ((startPos.y < endPos.y && startPos.x > endPos.x) || (endPos.y < startPos.y && endPos.x > startPos.x))
        {
            angle *= -1;
        }
        angle = Mathf.Rad2Deg * Mathf.Atan(angle);
        col.transform.Rotate(0, 0, angle);*/
    }
}
