using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Bubble : MonoBehaviour
{
    public GameObject recorder;
    public GameObject autoGenerate; 
    public GameObject grabAgentObject;
    public List<GameObject> objects =  new List<GameObject>();
   
    public GameObject Objects;
    LineRenderer l;
    public GameObject palm;
    private Vector3 palmOrigin;
    private Vector3 bubbleOrigin;
    public GameObject choose;
    public GameObject index;
    public GameObject middle;
    public GameObject ring;
    public GameObject[] LINE = new GameObject[3];
    LineRenderer[] line = new LineRenderer[3];
    float[] IntD = new float[1000];
    float[] ConD = new float[1000];
    float radius; 
    int layerMask;
    int i, j;
    public TMP_Text t;
    public GameObject[] target = new GameObject[3];
    public GameObject 
         index1, index2, index3,
         middle1, middle2, middle3,
         ring1, ring2, ring3;

    private float[] d = new float[5];
    private float[] ad = new float[5];
    private float[] angleLast = new float[5];
    int wrongTime;
    public bool selectingObject;//当前是否有选中物体
    private int finishNumber;
    private int needNumber;
    [SerializeField] private Material _material;
    [SerializeField] private Material defaultMaterial;
    public Material targetM;
    public Material notargetM;


    [SerializeField] private List<MeshRenderer> _meshRenderers;

    private void Awake(){
        layerMask = 1 << 7;   
        objects = new List<GameObject>();
        foreach (Transform t in Objects.GetComponentsInChildren<Transform>())
        if(t.name != "Objects")//todo 要排除空白物体，里面放所有需要参与计算的
        {
            objects.Add(t.gameObject);
            //Debug.Log(t.gameObject.name);
        }
        
    }
    public void changeRadius()
    {
        t.text = "101";
        for (int i = 0; i < 1000; i++)
        {
            IntD[i] = 1000000000000;
            ConD[i] = 0;
        }
        t.text = "111";
        for (int i = 0; i < objects.Count; i++)
            if(objects[i].GetComponent<MeshFilter>()!=null)
        {
            t.text = objects[i].GetComponent<MeshFilter>().sharedMesh.vertices.Length.ToString();
            var vertices = objects[i].GetComponent<MeshFilter>().sharedMesh.vertices;//Vector3[]
            t.text = "131";
            foreach (var v in vertices)
            {
                t.text = "141";
                var worldPos = objects[i].transform.TransformPoint(v);
                var dis = (worldPos - transform.position).magnitude;

                if (IntD[i] > dis)
                {
                    IntD[i] = dis;
                }//物体所有点的最近
                if (ConD[i] < dis)
                {
                    ConD[i] = dis;
                }//物体所有点的最远
            }
        }
        t.text = "12";
        float min = 1000000;
        int k = 0;
        for (int i = 0; i < objects.Count; i++)
        {
            if (min > IntD[i])
            {
                min = IntD[i];
                k = i;
            }
        }
        t.text = "13";
        min = 1000000;
        int k2 = 0;
        for (int i = 0; i < objects.Count; i++)
        {
            if (i != k)
                if (min > IntD[i])
                {
                    min = IntD[i];
                    k2 = i;
                }
        }
        i = k;
        j = k2;
        t.text = "14";
        if (ConD[i] < IntD[j])
        {
            radius = ConD[i];
        }
        else
        {
            radius = IntD[j];
        }//两者间更小的那个
        var size = transform.GetComponent<Renderer>().bounds.size;//现在球的size
        radius = 2 * radius * transform.localScale.x / size.x;
        transform.localScale = new Vector3(radius, radius, radius);
    }

    void Start()
    {
        InvokeRepeating("RepeatedMethod", 1f, 0.6f);
        l = GetComponent<LineRenderer>();
        l.startWidth = 0.005f;
        l.endWidth = 0.005f;
        int mark = 0;
        foreach (var item in LINE)
        {
          line[mark++] = item.GetComponent<LineRenderer>();
          line[mark-1].startWidth = 0.005f;
          line[mark-1].endWidth = 0.005f;
        }
    }
    // Update is called once per frame
    void killTheBubble(){
        l.startWidth = 0f;
        l.endWidth = 0f;
        foreach (var item in line)
        {
            item.startWidth = 0f;
            item.endWidth = 0f;
        }//线段可视化关掉
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        //球关掉
    }
    public void awakeTheBubble()
    {
        l.startWidth = 0.005f;
        l.endWidth = 0.005f;
        foreach (var item in line)
        {
            item.startWidth = 0.005f;
            item.endWidth = 0.005f;
        }//线段可视化关掉
        gameObject.GetComponent<MeshRenderer>().enabled = true;
        //球关掉
    }
    void Update()
    {
        l.SetPosition(0, palm.transform.position);
        l.SetPosition(1, -palm.transform.up* 100);
        //l.SetPosition(0, palm.transform.position + 0.05f * Vector3.up);todo在手掌心周围多加一点范围
        //l.SetPosition(1, -palm.transform.up* 100 + 0.05f * Vector3.up);
        decideCenter();
        follow();
        changeRadius();
        //检测出所有包含在这个sphere中的物体
        UpdataTarget();
        UpdateLine();//更新画线，需要更新了target之后
        if(!selectingObject)
            ChooseObject();
    }
    public void decideCenter()
    {
        Ray ray = new Ray(palm.transform.position, -palm.transform.up);//手掌心向前发出射线
        Ray ray1 = new Ray(palm.transform.position + 0.05f * Vector3.up, -palm.transform.up);//手掌心向前发出射线
        Ray ray2 = new Ray(palm.transform.position - 0.05f * Vector3.up, -palm.transform.up);//手掌心向前发出射线
        Ray ray3 = new Ray(palm.transform.position + 0.05f * Vector3.up, -palm.transform.up);//手掌心向前发出射线
        Ray ray4 = new Ray(palm.transform.position - 0.05f * Vector3.up, -palm.transform.up);//手掌心向前发出射线
        RaycastHit hitInfo;
        //声明一个RaycastHit结构体，存储碰撞信息
        if (Physics.Raycast(ray, out hitInfo, int.MaxValue, layerMask))
        {
            Debug.Log(hitInfo.collider.gameObject.name);
            if ((hitInfo.collider.gameObject.transform.position - bubbleOrigin).magnitude > 0.4)
            {
                bubbleOrigin = hitInfo.collider.gameObject.transform.position;
                palmOrigin = palm.transform.position;
                transform.position = bubbleOrigin;
            }
          
        }else if (Physics.Raycast(ray1, out hitInfo, int.MaxValue, layerMask))
        {
            Debug.Log(hitInfo.collider.gameObject.name);
            if ((hitInfo.collider.gameObject.transform.position - bubbleOrigin).magnitude > 0.4)
            {
                bubbleOrigin = hitInfo.collider.gameObject.transform.position;
                palmOrigin = palm.transform.position;
                transform.position = bubbleOrigin;
            }
          
        }else        if (Physics.Raycast(ray2, out hitInfo, int.MaxValue, layerMask))
        {
            Debug.Log(hitInfo.collider.gameObject.name);
            if ((hitInfo.collider.gameObject.transform.position - bubbleOrigin).magnitude > 0.4)
            {
                bubbleOrigin = hitInfo.collider.gameObject.transform.position;
                palmOrigin = palm.transform.position;
                transform.position = bubbleOrigin;
            }
          
        }else        if (Physics.Raycast(ray3, out hitInfo, int.MaxValue, layerMask))
        {
            Debug.Log(hitInfo.collider.gameObject.name);
            if ((hitInfo.collider.gameObject.transform.position - bubbleOrigin).magnitude > 0.4)
            {
                bubbleOrigin = hitInfo.collider.gameObject.transform.position;
                palmOrigin = palm.transform.position;
                transform.position = bubbleOrigin;
            }
          
        }else        if (Physics.Raycast(ray4, out hitInfo, int.MaxValue, layerMask))
        {
            Debug.Log(hitInfo.collider.gameObject.name);
            if ((hitInfo.collider.gameObject.transform.position - bubbleOrigin).magnitude > 0.4)
            {
                bubbleOrigin = hitInfo.collider.gameObject.transform.position;
                palmOrigin = palm.transform.position;
                transform.position = bubbleOrigin;
            }
          
        }
    }
        public void follow()
    {
        transform.position = palm.transform.position - palmOrigin + bubbleOrigin;
    }
    private int time = 0;
    public void UpdateLine()
    {
        if (target[0]) { 
        line[0].SetPosition(0, index.transform.position);
        line[0].SetPosition(1, target[0].transform.position);
        }
        if (target[1])
        {
            line[1].SetPosition(0, middle.transform.position);
            line[1].SetPosition(1, target[1].transform.position);
        }
        if (target[2])
        {
            line[2].SetPosition(0, ring.transform.position);
            line[2].SetPosition(1, target[2].transform.position);
        }
        //注意，三个target可能是一个物体
    }
    public void UpdataTarget()
    {
        //得到所有在碰撞体内部的碰撞体
        t.text = "2";
        Collider[] colliders = Physics.OverlapSphere(transform.position, transform.GetComponent<Renderer>().bounds.size.x/2,layerMask);
        foreach (var item in objects)
        {
            Debug.Log(item.name);
            item.gameObject.GetComponent<MeshRenderer>().material = notargetM;
        }
        t.text = "3";
        int targetNum = 0;//最多三个目标
        foreach (var item in colliders)
        //if(item.gameObject.name!="bubble")
        {
            t.text = "4";
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
    public void ChooseObject(){
        time += 1;
        if (time > 30)
            time = 22;
        bool[] mark = new bool[3];

        if (time > 20)
        { 
            d[1] = culculate(index1, index2, index3);
            d[2] = culculate(middle1, middle2, middle3);
            d[3] = culculate(ring1, ring2, ring3);


            mark[1] = false;
            mark[2] = false;
            mark[3] = false;

            if (d[1] - angleLast[1] > 0.3)//0.99-0.7(С��0.7)
            {
                mark[1] = true;
            }

            if (d[2] - angleLast[2] > 0.3)//С��0.7
            {
                mark[2] = true;
            }
            if (d[3] - angleLast[3] > 0.2)
            {
                mark[3] = true;
            }
            float max = -1;
            int select = 0;
            for (int i = 1; i <= 3; i++){
                    if (d[i] - angleLast[i] > max)
                    {
                        max = d[i] - angleLast[i];
                        select = i;
                    }
                }     
            if(mark[select] == true){

                if(recorder.GetComponent<singleSelect>().sampleType == 2){//select + manipulate
                    choose = target[select];
                    selectingObject = true;
                    time = 0;
                    AddOutline(target[select],Color.blue);
                    grabAgentObject.SetActive(true);
                    grabAgentObject.GetComponent<GrabAgentObjectBareHand>().MovingObject.Add(target[select]);
                    recorder.GetComponent<singleSelect>().writeFile("selectObject:" + target[select].name);
                    recorder.GetComponent<singleSelect>().selectOneObject();
                    killTheBubble();
                }
                else  if(recorder.GetComponent<singleSelect>().sampleType == 0)//select
                {
                    if (autoGenerate.GetComponent<autoGenerate>().targets.Contains((target[select])))//选中的是需要的物体
                    {
                        target[select].GetComponent<Renderer>().material.color = Color.blue;//这个是随机颜色变绿，选中后颜色变回蓝色
                        recorder.GetComponent<singleSelect>().writeFile("selectObject:" + target[select].name);
                        recorder.GetComponent<singleSelect>().selectOneObject();
                        finishNumber += 1;
                        if(finishNumber == needNumber)
                        {
                            recorder.GetComponent<singleSelect>().finishAll();
                        }
                    }
                    else
                    {
                        wrongTime += 1;
                        AddOutline(target[select],Color.red);
                    }
                    selectingObject = false;
                }
            }
            else{
                selectingObject = false;
                choose = null;
            }
            
        }
    }
    private void RepeatedMethod()
    {
        float d = culculate(index1, index2, index3);

        angleLast[1] = d;
        if (-d > 0.95f)
            ad[1] = d;

        d = culculate(middle1, middle2, middle3);

        angleLast[2] = d;
        if (-d > 0.95f)
            ad[2] = d;
        d = culculate(ring1, ring2, ring3);

        angleLast[3] = d;
        if (-d > 0.95f)
            ad[3] = d;
    }
    private float culculate(GameObject one, GameObject two, GameObject three)
    {
        var first = one.transform.position - two.transform.position;
        var second = three.transform.position - two.transform.position;
        float angle = Vector3.Dot(first, second) / (first.magnitude * second.magnitude);
        return angle;
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



