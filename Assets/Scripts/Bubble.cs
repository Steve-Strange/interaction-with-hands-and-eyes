using System.Runtime;
using System.Net.Mail;
using System.Net.Sockets;
using System.Transactions;
using System.Collections.ObjectModel;
using System.Threading.Tasks.Dataflow;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.PXR;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    public GameObject[] objects;
    public GameObject index;
    public GameObject middle;
    public GameObject ring;
    LineRenderer line;
    float IntD[1000];
    float ConD[1000];
    float radius;
    int i,j;
    public SkinnedMeshRenderer LeftEyeExample;
    public SkinnedMeshRenderer RightEyeExample;

    private int leftEyeBlinkIndex;
    private float leftEyeOpenness;
    public int mark;
    
    private int rightEyeBlinkIndex;
    private float rightEyeOpenness;
    // Start is called before the first frame update
    void Start()
    {
        mark = 0;
        line = getComponent<LineRenderer>();
        if(mark == 0)
        {
             line.startColor = Color.blue;
             line.endColor = Color.blue;
        }
                if(mark == 1)
        {
             line.startColor = Color.white;
             line.endColor = Color.white;
        }
                if(mark == 2)
        {
             line.startColor = Color.red;
             line.endColor = Color.red;
        }
                if(mark == 3)
        {
             line.startColor = Color.green;
             line.endColor = Color.green;
        }
    }

    // Update is called once per frame
    void Update()
    {
        line[0].SetPosition(0,index.position);
        line[0].SetPosition(1,target[0].position);
        line[1].SetPosition(0,middle.position);
        line[1].SetPosition(1,target[1].position);
        line[2].SetPosition(0,ring.position);
        line[2].SetPosition(1,target[2].position);

        for(int i =0;i<objects.count;i++)
        {
            var vertices = objects[i].GetComponent<MeshFilter>().sharedMesh.vertices//Vector3[]

            foreach (var v in vertices){
                var worldPos = objects[i].transform.TransformPoint(v);
                dis = (worldPos-Transform.position).magnitude;
                if(IntD[i] > dis){
                    IntD[i] = dis;
                }
                if(ConD[i] < dis){
                    ConD[i] = dis;
                }
            }
        }
        float min = 1000000;
        int k = 0;
        for(int i =0;i<objects.count;i++){
                if(min<IntD[i]){
                    min = IntD[i];
                    k = i;
                }
        }
        min = 1000000;
        int k2 = 0;
        for(int i =0;i<objects.count;i++){
            if(i!=k)
                if(min<IntD[i]){
                    min = IntD[i];
                    k = i;
                }
        }
        if(ConD[i] < IntD[j]){
            radius = ConD[i];
        }        
        else{
            radius = IntD[j];
        }
        
        transform.localScale = new Vector3 (radius, radius, radius);

        //检测出所有包含在这个sphere中的物体

    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class PhysicsSphereTest : MonoBehaviour
{
    private SphereCollider _sphereCollider;
    [SerializeField] private Material _material;
    [SerializeField] private Material defaultMaterial;


    [SerializeField] private List<MeshRenderer> _meshRenderers;

    private void Awake()
    {
        _sphereCollider = GetComponent<SphereCollider>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }


    private void OnDrawGizmos()
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
            v3.x = (float) x * radius;
            v3.z = (float) z * radius;
            Vector3 target = transform.position + v3;
            Debug.DrawLine(transform.position, target, Color.magenta);
        }

        ResetMaterial();
        int layer = LayerMask.GetMask("BoxCollider");
        // Collider[] colliders = Physics.OverlapSphere(transform.position, radius, layer);
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        foreach (var boid in colliders)
        {
            boid.GetComponent<MeshRenderer>().material = _material;
        }
    }


    void ResetMaterial()
    {
        foreach (var mr in _meshRenderers)
        {
            mr.material = defaultMaterial;
        }
    }
}

