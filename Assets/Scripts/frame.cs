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
    //public TMP_Text t;
   /* private void OnCollisionEnter(Collision collision)
    {
      
        
        t.text  = collision.gameObject.name;

      
    }*/
    public void creatRect()//2d-������
    {
        //t.text = "hahahah";
        forward = head.transform.forward.normalized;
        right = head.transform.right.normalized;
        up = head.transform.up.normalized;
        Vector3 center = head.transform.position + forward * dis;
        line.SetPosition(0, center + up * 1);
        line.SetPosition(1, center - up * 1);
        line.startWidth = (float)0.003;
        line.endWidth = (float)0.003;
        mesh = new Mesh();
        line.BakeMesh(mesh, camera, true);
        gameObject.AddComponent<MeshCollider>();
        this.GetComponent<MeshCollider>().sharedMesh = mesh;
    }
    void Start()
    {
        line = GetComponent<LineRenderer>();
        forward = head.transform.forward.normalized;
        right = head.transform.right.normalized;
        up = head.transform.up.normalized;
        Vector3 center = head.transform.position;// + forward * dis;
        line.SetPosition(0, new Vector3(0,0,0));
        line.SetPosition(1, new Vector3(0,100, 0));
        line.startWidth = (float)0.003;
        line.endWidth = (float)0.003;
        mesh = new Mesh();
        line.BakeMesh(mesh, camera, true);
        gameObject.AddComponent<MeshCollider>();
        this.GetComponent<MeshCollider>().sharedMesh = mesh;
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
