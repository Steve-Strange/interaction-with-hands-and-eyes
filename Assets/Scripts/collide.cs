
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collide : MonoBehaviour
{
    public GameObject connectorManager;
    public GameObject GrabAgent;
    public GameObject pinch;
    public GameObject frameButton;
    public GameObject handPoseManager;
    public GameObject thumb;
    public GameObject index;
    public bool ispinch = false;
    private pinch p;

    public TMPro.TMP_Text T;

    public TMPro.TMP_Text t;
    public TMPro.TMP_Text t2;

    public GameObject frame;  
    public string frameMark;//the mark of frame
    public string type;

    public GameObject FinalObjects;
    public List<GameObject> finalObj;//get all objects from last level
    
    public List<GameObject> anchor;//all anchor object,hilight them,we can only contact with them

    public List<GameObject> onFrame;//object already on now frame

    public GameObject[] rect = new GameObject[4];// leftup rightup rightdown leftdown
    private Vector3[] rectCorner;
    public int[] rectMark = { 0, 0, 0, 0, 0, 0, 0, 0 };
   
    public GameObject[] tri = new GameObject[3];// leftup rightup rightdown leftdown
    private Vector3[] triCorner;
    public int[] triMark = { 0, 0, 0, 0, 0, 0, 0, 0 };

    public GameObject[] circle = new GameObject[5];// 圆用前三个
    public int mark = 0;

    public GameObject[] para = new GameObject[2];// 
    private Vector3[] paraCorner;
    public int[] paraMark = { 0, 0, 0, 0, 0, 0, 0, 0 };

    public GameObject[] pen = new GameObject[2];//
    private Vector3[] penCorner;
    public int[] penMark = { 0, 0, 0, 0, 0, 0, 0, 0 };

    public GameObject[] cube = new GameObject[8];// 
    private Vector3[] cubeCorner;
    public int[] cubeMark = { 0, 0, 0, 0, 0, 0, 0, 0 };

    private GameObject now;

    public GameObject temp;
    int label = 0;

    private void OnCollisionEnter(Collision collision){
       // T.text = collision.gameObject.name;
        if (collision.gameObject.name == "Edge")
        {
            now = finalObj[0];
            label = 0;
            ContactPoint contact = collision.contacts[0];
            if (p.ispinch & finalObj.Count != 0)
            {
                now.transform.position = contact.point;
                now.transform.parent = collision.gameObject.transform;
            }
        }
        
    }
    private void OnCollisionStay(Collision collision){
        if (collision.gameObject.name == "Edge")
        {
            ContactPoint contact = collision.contacts[0];
            if (p.ispinch & finalObj.Count != 0)
            {
                now.transform.position = contact.point;
                now.transform.parent = collision.gameObject.transform;
               
                label = 1;
            }
            t2.text = now.name;
            now.GetComponent<Outline>().OutlineColor = Color.clear;
            t2.text = "2222";
            if (frame.GetComponent<frame>().Frame == "rect"){
                t2.text = "3333";
                rectCorner = frame.GetComponent<frame>().rectCorner;//line 
                for (int i = 0; i <= 3; i++)
                    if ((now.transform.position - rectCorner[i]).magnitude < 0.01){//有资格当anchor的变成蓝色
                        now.GetComponent<Outline>().OutlineColor = Color.blue;
                    }
               
            }
            if (frame.GetComponent<frame>().Frame == "circle"){
                var temp = now.transform.position - frame.GetComponent<frame>().center;
                now.transform.position = temp.normalized * frame.GetComponent<frame>().R + frame.GetComponent<frame>().center;
            }
            if (frame.GetComponent<frame>().Frame == "tri"){
                triCorner = frame.GetComponent<frame>().triCorner;
                for (int i = 0; i <= 2; i++)
                    if ((now.transform.position - triCorner[i]).magnitude < 0.01)
                    {
                        now.GetComponent<Outline>().OutlineColor = Color.blue;
                    }
            }
            if (frame.GetComponent<frame>().Frame == "pen"){

                penCorner = frame.GetComponent<frame>().penCorner;

                for (int i = 0; i <= 4; i++)
                    if ((now.transform.position - penCorner[i]).magnitude < 0.01){
                        now.GetComponent<Outline>().OutlineColor = Color.blue;
                    }
            }

            if (frame.GetComponent<frame>().Frame == "para"){
                paraCorner = frame.GetComponent<frame>().paraCorner;
                for (int i = 0; i <= 3; i++)
                    if ((now.transform.position - paraCorner[i]).magnitude < 0.01){
                        now.GetComponent<Outline>().OutlineColor = Color.blue;
                    }
            }
            if (frame.GetComponent<frame>().Frame == "cube"){//要能确定新的长宽高
                cubeCorner = frame.GetComponent<frame>().cubeCorner;
                for (int i = 0; i <= 7; i++)
                    if ((now.transform.position - cubeCorner[i]).magnitude < 0.01){
                        now.GetComponent<Outline>().OutlineColor = Color.blue;
                    }
            }
        }
    }
    private void settleDown()
    {
        finalObj[0].GetComponent<Outline>().OutlineColor = Color.clear;
        if (frame.GetComponent<frame>().Frame == "rect")//解决空指针出错的问题
                {

                    rectCorner = frame.GetComponent<frame>().rectCorner;//line 

                    for (int i = 0; i <= 3; i++)
                        if ((finalObj[0].transform.position - rectCorner[i]).magnitude < 0.01)
                        {//有资格当anchor的变成蓝色
                            finalObj[0].transform.position = rectCorner[i];
                            rect[i] = finalObj[0];
                            rectMark[i] = 1;
                            finalObj[0].GetComponent<Outline>().OutlineColor = Color.blue;
                        }
                }

        if (frame.GetComponent<frame>().Frame == "circle")
        {
            var temp = finalObj[0].transform.position - frame.GetComponent<frame>().center;
            finalObj[0].transform.position = temp.normalized * frame.GetComponent<frame>().R + frame.GetComponent<frame>().center;
            //任意三个就可以
            if (mark < 3)
            {
                circle[mark] = finalObj[0];
                finalObj[0].GetComponent<Outline>().OutlineColor = Color.blue;
            }
            mark++;
        }
        if (frame.GetComponent<frame>().Frame == "tri")
                {

                    triCorner = frame.GetComponent<frame>().triCorner;

                    for (int i = 0; i <= 2; i++)
                        if ((finalObj[0].transform.position - triCorner[i]).magnitude < 0.01)
                        {
                            finalObj[0].transform.position = triCorner[i];
                            tri[i] = finalObj[0];
                            triMark[i] = 1;
                            finalObj[0].GetComponent<Outline>().OutlineColor = Color.blue;
                        }
                }


                if (frame.GetComponent<frame>().Frame == "pen")
                {

                    penCorner = frame.GetComponent<frame>().penCorner;

                    for (int i = 0; i <= 4; i++)
                        if ((finalObj[0].transform.position - penCorner[i]).magnitude < 0.01)
                        {
                            finalObj[0].transform.position = penCorner[i];
                            pen[i] = finalObj[0];
                            penMark[i] = 1;
                            finalObj[0].GetComponent<Outline>().OutlineColor = Color.blue;
                        }
                }

                if (frame.GetComponent<frame>().Frame == "para")
                {

                    paraCorner = frame.GetComponent<frame>().paraCorner;

                    for (int i = 0; i <= 3; i++)
                        if ((finalObj[0].transform.position - paraCorner[i]).magnitude < 0.01)
                        {
                            finalObj[0].transform.position = paraCorner[i];
                            para[i] = finalObj[0];
                            paraMark[i] = 1;
                            finalObj[0].GetComponent<Outline>().OutlineColor = Color.blue;
                        }
                }
                if (frame.GetComponent<frame>().Frame == "cube")
                {//要能确定新的长宽高

                    cubeCorner = frame.GetComponent<frame>().cubeCorner;

                    for (int i = 0; i <= 7; i++)
                        if ((finalObj[0].transform.position - cubeCorner[i]).magnitude < 0.01)
                        {
                            finalObj[0].transform.position = cubeCorner[i];
                            cube[i] = finalObj[0];
                            finalObj[0].GetComponent<Outline>().OutlineColor = Color.blue;
                        }
                }

                onFrame.Add(finalObj[0]);
                finalObj.RemoveAt(0);
                label = 0;
         
        }
  
    public void getFinalObject()
    {
     
        finalObj = FinalObjects.GetComponent<FinalObjects>().finalObj;

    }
    private List<string> m_logEntries = new List<string>();
    private void Update()
    {
      //  t2.text = m_logEntries[m_logEntries.Count-1];
        if(handPoseManager.GetComponent<HandPoseManager>().phase == 1 && label == 1 && !p.ispinch && now == finalObj[0])
        {
            label = 0;
            settleDown();
            label = 0;
        }
        t.text = FinalObjects.GetComponent<FinalObjects>().finalObj.Count.ToString();
    }
    
    void Start()
    {
        p = pinch.GetComponent<pinch>();

        Application.logMessageReceived += (string condition, string stackTrace, LogType type) =>
        {
            if (type == LogType.Exception || type == LogType.Error)
            {
                m_logEntries.Add(string.Format("{0}\n{1}", condition, stackTrace));
            }
        };
        rect = new GameObject[8];
        circle = new GameObject[8];
        tri = new GameObject[8];
        para = new GameObject[8];
        pen = new GameObject[8];
        cube = new GameObject[8];
    }


    public void anchorChoose()
    {
        anchor.Clear();
        foreach (var item in onFrame){
            item.GetComponent<Outline>().outlineColor = Color.clear;
        }
        GrabAgent.GetComponent<GrabAgentObject>().MovingObject.Clear();
        if (frame.GetComponent<frame>().Frame == "rect"){
            /*GrabAgent.GetComponent<GrabAgentObject>().MovingObject.Add(onFrame[0]);
            GrabAgent.GetComponent<GrabAgentObject>().MovingObject.Add(onFrame[1]);//!!!改这里
        
            anchor.Add(onFrame[0]);
            anchor.Add(onFrame[1]);*/
            if(rectMark[0]==1 && rectMark[2]==1){         
              anchor.Add(rect[0]);
              anchor.Add(rect[2]);
              GrabAgent.GetComponent<GrabAgentObject>().MovingObject.Add(rect[0]);
              GrabAgent.GetComponent<GrabAgentObject>().MovingObject.Add(rect[2]);
            }
            else if(rectMark[1] == 1 && rectMark[3] == 1)
            {
              anchor.Add(rect[1]);
              anchor.Add(rect[3]);
                GrabAgent.GetComponent<GrabAgentObject>().MovingObject.Add(rect[1]);
                GrabAgent.GetComponent<GrabAgentObject>().MovingObject.Add(rect[3]);
            }
        }        
        if (frame.GetComponent<frame>().Frame == "circle"){
         
            anchor.Add(circle[0]);
           
            anchor.Add(circle[1]);
        
            anchor.Add(circle[2]);
          
            GrabAgent.GetComponent<GrabAgentObject>().MovingObject.Add(circle[0]);
            GrabAgent.GetComponent<GrabAgentObject>().MovingObject.Add(circle[1]);
        }
        if (frame.GetComponent<frame>().Frame == "tri"){
                anchor.Add(tri[0]);
                anchor.Add(tri[1]);
                anchor.Add(tri[2]);
                GrabAgent.GetComponent<GrabAgentObject>().MovingObject.Add(tri[0]);
                GrabAgent.GetComponent<GrabAgentObject>().MovingObject.Add(tri[1]);
        }

        if (frame.GetComponent<frame>().Frame == "para"){
            if (paraMark[0]==1 && paraMark[2]==1)
            {
                anchor.Add(para[0]);
                anchor.Add(para[2]);
                GrabAgent.GetComponent<GrabAgentObject>().MovingObject.Add(para[0]);
                GrabAgent.GetComponent<GrabAgentObject>().MovingObject.Add(para[2]);
            }
            else if (paraMark[1]==1 && paraMark[3] ==1 )
            {
                anchor.Add(para[1]);
                anchor.Add(para[3]);
                GrabAgent.GetComponent<GrabAgentObject>().MovingObject.Add(para[1]);
                GrabAgent.GetComponent<GrabAgentObject>().MovingObject.Add(para[3]);
            }
        }
        if (frame.GetComponent<frame>().Frame == "pen"){
            int k=0;
            for(int i = 0;i<=4;i++)
            for(int j = 0;j<=4;j++)
            if(i!=j&&penMark[i]==1 && penMark[j]==1&k==0)
            {
                anchor.Add(pen[i]);
                anchor.Add(pen[j]);
                GrabAgent.GetComponent<GrabAgentObject>().MovingObject.Add(pen[i]);
                GrabAgent.GetComponent<GrabAgentObject>().MovingObject.Add(pen[j]);
                k=1;
            }
        }
        if (frame.GetComponent<frame>().Frame == "cube"){
            // need three anchor to calculate 
            if (cube[0] && cube[2] && cube[5])//0，2，5
            {
                anchor.Add(cube[0]);
                anchor.Add(cube[2]);
                anchor.Add(cube[5]);
            }
            else if (cube[0] && cube[6] && cube[2])//0，2，6
            {
                anchor.Add(cube[0]);
                anchor.Add(cube[2]);
                anchor.Add(cube[6]);
            }
            else if (cube[0] && cube[4] && cube[2])//0，2，4
            {
                anchor.Add(cube[0]);
                anchor.Add(cube[2]);
                anchor.Add(cube[4]);
            }
            else if (cube[0] && cube[7] && cube[2])//0，2，7
            {
                anchor.Add(cube[0]);
                anchor.Add(cube[2]);
                anchor.Add(cube[7]);
            }
            else if (cube[1] && cube[3] && cube[4])//1，3，4
            {
                anchor.Add(cube[1]);
                anchor.Add(cube[3]);
                anchor.Add(cube[4]);
            }
            else if (cube[1] && cube[3] && cube[2])//1，3，5
            {
                anchor.Add(cube[1]);
                anchor.Add(cube[3]);
                anchor.Add(cube[5]);
            }
            else if (cube[1] && cube[3] && cube[6])//1，3，6
            {
                anchor.Add(cube[1]);
                anchor.Add(cube[3]);
                anchor.Add(cube[6]);
            }
            else if (cube[1] && cube[3] && cube[7])//1，3，7
            {
                anchor.Add(cube[1]);
                anchor.Add(cube[3]);
                anchor.Add(cube[7]);
            }
            else if (cube[0] && cube[4] && cube[6])//4，6，0
            {
                anchor.Add(cube[0]);
                anchor.Add(cube[4]);
                anchor.Add(cube[6]);
            }
            else if (cube[1] && cube[4] && cube[6])//4，6，1
            {
                anchor.Add(cube[1]);
                anchor.Add(cube[4]);
                anchor.Add(cube[6]);
            }
            else if (cube[4] && cube[6] && cube[2])//4，6，2
            {
                anchor.Add(cube[2]);
                anchor.Add(cube[4]);
                anchor.Add(cube[6]);
            }
            else if (cube[4] && cube[3] && cube[6])//4，6，3
            {
                anchor.Add(cube[3]);
                anchor.Add(cube[4]);
                anchor.Add(cube[6]);
            }
            else if (cube[5] && cube[7] && cube[0])//5，7，0
            {
                anchor.Add(cube[0]);
                 anchor.Add(cube[5]);
                anchor.Add(cube[7]);
            }
            else if (cube[1] && cube[7] && cube[5])//5，7，1
            {
                anchor.Add(cube[1]);
                 anchor.Add(cube[5]);
                anchor.Add(cube[7]);
            }
            else if (cube[7] && cube[5] && cube[2])//5，7，2
            {
                anchor.Add(cube[2]);
                 anchor.Add(cube[5]);
                anchor.Add(cube[7]);
            }
            else if (cube[5] && cube[3] && cube[7])//5，7，3
            {
                anchor.Add(cube[3]);
                anchor.Add(cube[5]);
                anchor.Add(cube[7]);
            }
            GrabAgent.GetComponent<GrabAgentObject>().MovingObject[0] = anchor[0];
            GrabAgent.GetComponent<GrabAgentObject>().MovingObject[1] = anchor[1];
            //maybe need three
            //connectorManager.GetComponent<ConnectorManager>().cube3 = anchor[2];
        }
        T.text = "anchor5";
        /**/
        foreach (var obj in anchor){
                obj.GetComponent<Outline>().OutlineColor = Color.green;
        }
        T.text = "anchor6";
    }
    // Update is called once per frame
  



}
                                                                                                                                                                                                                                                          