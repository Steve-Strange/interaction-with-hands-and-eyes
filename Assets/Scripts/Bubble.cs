using System.Runtime;
using System.Net.Mail;
using System.Net.Sockets;
using System.Transactions;
using System.Collections.ObjectModel;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.PXR;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    public GameObject[] objects;
    public GameObject index;
    public GameObject middle;
    public GameObject ring;
    public  GameObject[] LINE= new GameObject[3];
    LineRenderer[] line = new LineRenderer[3];
    float[] IntD = new float[1000];
    float[] ConD = new float[1000];
    float radius;
    int i,j;

   public GameObject[] target = new GameObject[3];

    public int mark;
   
    // Start is called before the first frame update

    private SphereCollider _sphereCollider;
    [SerializeField] private Material _material;
    [SerializeField] private Material defaultMaterial;
    public Material targetM;
    public Material notargetM;


    [SerializeField] private List<MeshRenderer> _meshRenderers;

    private void Awake()
    {
        _sphereCollider = GetComponent<SphereCollider>();
    }


   /* private void OnDrawGizmos()
    {
        if (_sphereCollider == null)
            _sphereCollider = GetComponent<SphereCollider>();
        float radius = _sphereCollider.radius;

        for (int angel = 0; angel < 360; angel += 2)
        {
            double x = Math.Cos(angel * 1.0f);
            double z = Math.Sin(angel * 1.0f);
            Vector3 v3 = Vector3.one;
            v3.y = 0;
            v3.x = (float)x * radius;
            v3.z = (float)z * radius;
            Vector3 target = transform.position + v3;
            Debug.DrawLine(transform.position, target, Color.magenta);
        }

        ResetMaterial();
        int layer = LayerMask.GetMask("BoxCollider");
        // Collider[] colliders = Physics.OverlapSphere(transform.position, radius, layer);

        //得到所有在碰撞体内部的碰撞体
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        foreach (var item in colliders)
        {
            item.GetComponent<Outline>().outlineColor = Color.red;
        }
    }*/


    void ResetMaterial(){
        foreach (var mr in _meshRenderers)
        {
            mr.material = defaultMaterial;
        }
    }


    void Start()
    {
        mark = 0;
        foreach (var item in LINE)
        {
          line[mark++] = item.GetComponent<LineRenderer>();
        }//得到三条画线的
        
             line[0].startColor = Color.blue;
             line[0].endColor = Color.blue;
             line[0].startWidth = 0.01f;
             line[0].endWidth = 0.01f;

             line[1].startColor = Color.white;
             line[1].endColor = Color.white;
            line[1].startWidth = 0.01f;
            line[1].endWidth = 0.01f;

            line[2].startColor = Color.red;
            line[2].endColor = Color.red;
            line[2].startWidth = 0.01f;
            line[2].endWidth = 0.01f;
    }
    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < 1000; i++)
        {
            IntD[i] = 1000000000000;
            ConD[i] = 0;
        }

            for (int i =0;i<objects.Length;i++)
        {
            var vertices = objects[i].GetComponent<MeshFilter>().sharedMesh.vertices;//Vector3[]
            
            foreach (var v in vertices){
                var worldPos = objects[i].transform.TransformPoint(v);
                var dis = (worldPos-transform.position).magnitude;
                
                if(IntD[i] > dis){
                    IntD[i] = dis;
                }//物体所有点的最近
                if(ConD[i] < dis){
                    ConD[i] = dis;
                }//物体所有点的最远
            }
        }
        float min = 1000000;
        int k = 0;
        for(int i =0;i<objects.Length;i++){
                if(min>IntD[i]){
                    min = IntD[i];
                    k = i;
                }
        }
        min = 1000000;
        int k2 = 0;
        for(int i =0;i<objects.Length; i++){
            if(i!=k)
                if(min>IntD[i]){
                    min = IntD[i];
                    k2 = i;
                }
        }
        i = k;
        j = k2;

        if (ConD[i] < IntD[j]){
            radius = ConD[i];
        }        
        else{
            radius = IntD[j];
        }//两者间更小的那个
        var size = transform.GetComponent<Renderer>().bounds.size;//现在球的size
        radius = 2 * radius * transform.localScale.x / size.x;
        transform.localScale = new Vector3 (radius, radius, radius);

        //检测出所有包含在这个sphere中的物体
        UpdataTarget();
        UpdateLine();//更新画线，需要更新了target之后

    }
    public void UpdateLine()
    {
        line[0].SetPosition(0, index.transform.position);
        line[0].SetPosition(1, target[0].transform.position);
        line[1].SetPosition(0, middle.transform.position);
        line[1].SetPosition(1, target[1].transform.position);
        line[2].SetPosition(0, ring.transform.position);
        line[2].SetPosition(1, target[2].transform.position);
        //注意，三个target可能是一个物体
    }
    public void UpdataTarget()
    {
        //得到所有在碰撞体内部的碰撞体
        Collider[] colliders = Physics.OverlapSphere(transform.position, transform.GetComponent<Renderer>().bounds.size.x/2);
        Debug.Log(transform.GetComponent<Renderer>().bounds.size.x / 2);
        foreach (var item in objects)
        {
            item.gameObject.GetComponent<MeshRenderer>().material = notargetM;
        }
        int targetNum = 0;//最多三个目标
        foreach (var item in colliders)
        if(item.gameObject.name!="bubble")
        {
            if (targetNum < 3) {
                    targetNum++;
                    Debug.Log(item.gameObject.name);
                    item.gameObject.GetComponent<MeshRenderer>().material = targetM;
                    for(int i = targetNum-1; i <= 2; i++) { 
                    target[i] = item.gameObject;
                    }
            }
        }
    }
}



