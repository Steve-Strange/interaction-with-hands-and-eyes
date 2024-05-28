using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class Bubble : MonoBehaviour
{
    public GameObject recorder;
    public GameObject autoGenerate; 
    public GameObject grabAgentObject;
    public GameObject Objects;
    public List<GameObject> objects =  new List<GameObject>();
    LineRenderer l;
    public GameObject palm;
    private Vector3 palmOrigin;
    private Vector3 bubbleOrigin;
    public GameObject index;
    public GameObject middle;
    public GameObject ring;    
    public GameObject 
         index1, index2, index3,
         middle1, middle2, middle3,
         ring1, ring2, ring3;
    public GameObject[] Lines = new GameObject[3];
    private LineRenderer[] line = new LineRenderer[3];
    float[] IntD = new float[1000];
    float[] ConD = new float[1000];
    float radius; 
    int layerMask;
    int i, j;
    private GameObject[] target = new GameObject[3];
    private float[] d = new float[5];
    private float[] ad = new float[5];
    private float[] angleLast = new float[5];
    int wrongTime;
    public bool selectingObject;//当前是否有选中物体
    private int finishNumber;
    private int needNumber;
    int round;

    [SerializeField] private List<MeshRenderer> _meshRenderers;

    private void Awake(){
      //  transform.position = temp.transform.position;
        round = 0;
        layerMask = 1 << 7;   
        objects = new List<GameObject>();
        //在仅操纵和移动的场景下
        if(recorder.GetComponent<singleSelect>().sampleType !=0 )
        {
        FindChilds(GameObject.Find("manipulate"));
        FindChilds(GameObject.Find("others"));
        }else
        {
        FindChilds(Objects);//在仅选择下
        }
      
        //
        finishNumber = 0;
        selectingObject = false;
        needNumber = autoGenerate.GetComponent<autoGenerate>().targetNumber;

    }
    public void FindChilds(GameObject OBJ)
    {
        for (int c = 0; c < OBJ.transform.childCount; c++)
        {
  
                objects.Add(OBJ.transform.GetChild(c).gameObject);//不要把目标位置物体也算进去
           // else
               // FindChilds(Objects.transform.GetChild(c).gameObject);
        }
    }
    private string m_logEntries = "";
    public void changeRadius()
    {
      //  t.text = "101";
        for (int i = 0; i < 1000; i++)
        {
            IntD[i] = 1000000000000;
            ConD[i] = 0;
        }
       // t.text = "111";
        for (int i = 0; i < objects.Count; i++)
            if(objects[i].GetComponent<MeshFilter>()!=null)
        {
       //     t.text = objects[i].GetComponent<MeshFilter>().sharedMesh.vertices.Length.ToString();
            var vertices = objects[i].GetComponent<MeshFilter>().sharedMesh.vertices;//Vector3[]
               // Debug.Log(vertices.Length);       //     t.text = "131";
            foreach (var v in vertices)
            {
            //    t.text = "141";
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
     //   t.text = "12";
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
       // t.text = "13";
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
    //    t.text = "14";
        if (ConD[i] < IntD[j])
        {
            radius = ConD[i];
        }
        else
        {
            radius = IntD[j];
        }//两者间更小的那个
      
        var size = transform.GetComponent<Renderer>().bounds.size;//现在球的size
      //  radius = 2 * radius * transform.localScale.x / size.x;  
        Debug.Log(radius);
        transform.localScale = new Vector3(radius, radius, radius);
    }

    void Start()
    {

        Application.logMessageReceived += (string condition, string stackTrace, LogType type) =>
        {
            if (type == LogType.Exception || type == LogType.Error)
            {
         
                m_logEntries += (string.Format("{0}\n{1}", condition, stackTrace));
            }
        };



        if (recorder.GetComponent<singleSelect>().sampleType == 1)
        {//仅操纵，根本就不出现这个脚本
            gameObject.SetActive(false);
        }
        selectingObject = false;
        InvokeRepeating("RepeatedMethod", 1f, 0.6f);
        time = 0;
        l = GetComponent<LineRenderer>();
        l.startWidth = 0.01f;
        l.endWidth = 0.01f;
        int mark = 0;
        foreach (var item in Lines)
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
        // t.text = "guandiao";
        gameObject.SetActive(false);
        //gameObject.GetComponent<MeshRenderer>().enabled = false;
        //球关掉
    }
    public void awakeTheBubble()
    {
        l.startWidth = 0.01f;
        l.endWidth = 0.01f;
        foreach (var item in line)
        {
            item.startWidth = 0.005f;
            item.endWidth = 0.005f;
        }//线段可视化关掉
        gameObject.GetComponent<MeshRenderer>().enabled = true;
        //球关掉
    }
    bool init = true;
    void Update()
    {
        // t.text = m_logEntries.ToString();
        l.SetPosition(0, palm.transform.position);
        l.SetPosition(1, -palm.transform.up* 100);
      //  l.startWidth = 0.01f;
     //   l.endWidth = 0.01f;
        //l.SetPosition(0, palm.transform.position + 0.05f * Vector3.up);todo在手掌心周围多加一点范围
        //l.SetPosition(1, -palm.transform.up* 100 + 0.05f * Vector3.up);
        // 
        //  if (init) { //
        //      transform.position = objects[0].transform.position; 
        //     init = false;
        //  }

        if(!selectingObject)
        {         decideCenter();
                  follow();
                  changeRadius();
                   //检测出所有包含在这个sphere中的物体
                   UpdataTarget();
                    UpdateLine();//更新画线，需要更新了target之后
                    ChooseObject();
        }
    }
    public void decideCenter()
    {
        Ray ray = new Ray(palm.transform.position, -palm.transform.up);//手掌心向前发出射线
         // Ray ray1 = new Ray(palm.transform.position + 0.05f * Vector3.up, -palm.transform.up);//手掌心向前发出射线
        //Ray ray2 = new Ray(palm.transform.position - 0.05f * Vector3.up, -palm.transform.up);//手掌心向前发出射线
        //Ray ray3 = new Ray(palm.transform.position + 0.05f * Vector3.up, -palm.transform.up);//手掌心向前发出射线
        ///Ray ray4 = new Ray(palm.transform.position - 0.05f * Vector3.up, -palm.transform.up);//手掌心向前发出射线
        RaycastHit hitInfo;
        //声明一个RaycastHit结构体，存储碰撞信息
        if (Physics.Raycast(ray, out hitInfo, int.MaxValue, layerMask) )//&& init)
        {
            init = false;
            Debug.Log(hitInfo.collider.gameObject.name);
            if ((hitInfo.collider.gameObject.transform.position - bubbleOrigin).magnitude > 2)
            {
                bubbleOrigin = hitInfo.collider.gameObject.transform.position;
                palmOrigin = palm.transform.position;
                transform.position = bubbleOrigin;
            }
          
        }/*else if (Physics.Raycast(ray1, out hitInfo, int.MaxValue, layerMask))// && init)
        {
            init = false;
            Debug.Log(hitInfo.collider.gameObject.name);
            if ((hitInfo.collider.gameObject.transform.position - bubbleOrigin).magnitude > 1)
            {
                bubbleOrigin = hitInfo.collider.gameObject.transform.position;
                palmOrigin = palm.transform.position;
                transform.position = bubbleOrigin;
            }
          
        }else        if (Physics.Raycast(ray2, out hitInfo, int.MaxValue, layerMask))//&& init)
        {
            init = false;
            Debug.Log(hitInfo.collider.gameObject.name);
            if ((hitInfo.collider.gameObject.transform.position - bubbleOrigin).magnitude > 1)
            {
                bubbleOrigin = hitInfo.collider.gameObject.transform.position;
                palmOrigin = palm.transform.position;
                transform.position = bubbleOrigin;
            }
          
        }else        if (Physics.Raycast(ray3, out hitInfo, int.MaxValue, layerMask))//&& init)
        {
            init = false;
            Debug.Log(hitInfo.collider.gameObject.name);
            if ((hitInfo.collider.gameObject.transform.position - bubbleOrigin).magnitude > 1)
            {
                bubbleOrigin = hitInfo.collider.gameObject.transform.position;
                palmOrigin = palm.transform.position;
                transform.position = bubbleOrigin;
            }
          
        }else        if (Physics.Raycast(ray4, out hitInfo, int.MaxValue, layerMask))//&& init)
        {
            init = false;
            Debug.Log(hitInfo.collider.gameObject.name);
            if ((hitInfo.collider.gameObject.transform.position - bubbleOrigin).magnitude > 1)
            {
                bubbleOrigin = hitInfo.collider.gameObject.transform.position;
                palmOrigin = palm.transform.position;
                transform.position = bubbleOrigin;
            }
          
        }*/
    }
        public void follow()
    {
        transform.position = 8 * (palm.transform.position - palmOrigin) + bubbleOrigin;
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
       // t.text = "2";

        Collider[] colliders = Physics.OverlapSphere(transform.position, transform.GetComponent<Renderer>().bounds.size.x/2,layerMask);
     /**/   foreach (var item in objects)
        {
            AddOutline(item.gameObject, Color.clear);
        }
      //  t.text = "3";
        int targetNum = 0;//最多三个目标
        foreach (var item in colliders)
        {
           // t.text = "4";
            if (targetNum < 3) {
                    targetNum++;
                    AddOutline(item.gameObject, Color.yellow);
                    item.GetComponent<Outline>().OutlineWidth = 2f;
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
        bool[] mark = new bool[4];

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
           // 
            if(mark[select] == true){
                select -= 1;
                if(recorder.GetComponent<singleSelect>().sampleType == 2){//select + manipulate

                  //  t.text = "2";
                    selectingObject = true;
                    time = 0;
                  //  t.text = "3";
                    AddOutline(target[select],Color.blue);
                  //  t.text = "4";
                    grabAgentObject.SetActive(true);
                 //   t.text = target[select].name;
                    grabAgentObject.GetComponent<GrabAgentObjectBubble>().MovingObject.Add(target[select]);
                  //  t.text = "6";
                    recorder.GetComponent<singleSelect>().writeFile("selectObject:" + target[select].name);
                  //  t.text = "7";
                    recorder.GetComponent<singleSelect>().selectOneObject();
                  //  t.text = "8";
                    killTheBubble();  
                }else  if(recorder.GetComponent<singleSelect>().sampleType == 0)//select
                {
                   // t.text = "right";
                    if (autoGenerate.GetComponent<autoGenerate>().targets.Contains((target[select])))//选中的是需要的物体
                    {
                        target[select].GetComponent<Renderer>().material.color = Color.blue;//这个是随机颜色变绿，选中后颜色变回蓝色
                        recorder.GetComponent<singleSelect>().writeFile("selectObject:" + target[select].name);
                        recorder.GetComponent<singleSelect>().selectOneObject();
                        autoGenerate.GetComponent<autoGenerate>().targets.Remove(target[select]);//防止多选
                        finishNumber += 1;
                        
                        if(finishNumber == needNumber)
                        {
                            finishNumber = 0;
                            autoGenerate.GetComponent<autoGenerate>().reGenerate();
                            round += 1;
                            if (round == 2){
                                recorder.GetComponent<singleSelect>().writeFile("precision:"  + 30 * 1.0 / (wrongTime * 1.0 + 30) * 1.0);
                                recorder.GetComponent<singleSelect>().finishAll();//这个时候将总体的位移和角度变化也都写入
                                autoGenerate.SetActive(false); 
                            }
                              
                        }
                    }
                    else{
                        wrongTime += 1;
                    }
                    selectingObject = false;
                    target[select].SetActive(false);
                }else if(recorder.GetComponent<singleSelect>().sampleType == 1)//manipulate_only
                {

                }
            }
            else{
                selectingObject = false;
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



