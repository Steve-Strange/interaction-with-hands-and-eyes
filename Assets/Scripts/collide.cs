
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

    public TMPro.TMP_Text t;
    public TMPro.TMP_Text t2;

    public GameObject frame;  
    public string frameMark;//the mark of frame
    public string type;

    public GameObject FinalObjects;
    public List<GameObject> finalObj;//get all objects from last level
    
    public List<GameObject> anchor;//all anchor object,hilight them,we can only contact with them

    public List<GameObject> onFrame;//object already on now frame

    public List<GameObject> rect = new List<GameObject>();// leftup rightup rightdown leftdown
    private Vector3[] rectCorner;
    public List<GameObject> tri = new List<GameObject>();// leftup rightup rightdown leftdown
    private Vector3[] triCorner;
    public List<GameObject> circle = new List<GameObject>();// 圆用前三个
    private Vector3[] circleCorner;
    public int mark = 0;
    public List<GameObject> para = new List<GameObject>();// 
    private Vector3[] paraCorner;
    public List<GameObject> pen = new List<GameObject>();//
    private Vector3[] penCorner;
    public List<GameObject> cube = new List<GameObject>();// 
    private Vector3[] cubeCorner;
    private void OnCollisionStay(Collision collision)
    {
        ContactPoint contact = collision.contacts[0];
        if (p.ispinch & finalObj.Count != 0) 
        { 
            finalObj[0].transform.position = contact.point;
            finalObj[0].transform.parent = collision.gameObject.transform;
        }
    }
    private void OnCollisionExit(Collision collision){                                                                                                                                                   
       t2.text = finalObj.Count.ToString();
        if(ispinch)
        { 

          
          if(frame.GetComponent<frame>().Frame == "rect"){

            rectCorner = frame.GetComponent<frame>().rectCorner;//line 

            for(int i = 0 ;i <= 3 ;i++)
            if((finalObj[0].transform.position- rectCorner[i]).magnitude < 0.01){//有资格当anchor的变成蓝色
                finalObj[0].transform.position = rectCorner[i];
                rect[i] = finalObj[0];
                finalObj[0].GetComponent<Outline>().OutlineColor = Color.blue;
                    }
          }

          if(frame.GetComponent<frame>().Frame == "tri"){

                triCorner = frame.GetComponent<frame>().triCorner;

            for(int i = 0 ;i <= 2 ;i++)
            if((finalObj[0].transform.position- triCorner[i]).magnitude < 0.01){
                finalObj[0].transform.position = triCorner[i];
                tri[i] = finalObj[0];
                finalObj[0].GetComponent<Outline>().OutlineColor = Color.blue;
                    }
          }          

          if(frame.GetComponent<frame>().Frame == "circle"){
            //任意三个就可以
            if(mark<3){
                    circleCorner[mark] = finalObj[0].transform.position;
                    finalObj[0].GetComponent<Outline>().OutlineColor = Color.blue;
            } 
            mark++;
          }          

          if(frame.GetComponent<frame>().Frame == "pen"){

                penCorner = frame.GetComponent<frame>().penCorner;

            for(int i = 0 ;i <= 4 ;i++)
            if((finalObj[0].transform.position- penCorner[i]).magnitude < 0.01){
                finalObj[0].transform.position = penCorner[i];
                pen[i] = finalObj[0];
                finalObj[0].GetComponent<Outline>().OutlineColor = Color.blue;
                    }
          }  

          if(frame.GetComponent<frame>().Frame == "para"){

                paraCorner = frame.GetComponent<frame>().paraCorner;

            for(int i = 0 ;i <= 3 ;i++)
            if((finalObj[0].transform.position- paraCorner[i]).magnitude < 0.01){
                finalObj[0].transform.position = paraCorner[i];
                para[i] = finalObj[0];
                finalObj[0].GetComponent<Outline>().OutlineColor = Color.blue;
                    }
          }  
          if(frame.GetComponent<frame>().Frame == "cube"){//要能确定新的长宽高

                cubeCorner = frame.GetComponent<frame>().cubeCorner;

            for(int i = 0 ;i <= 7 ;i++)
            if((finalObj[0].transform.position- cubeCorner[i]).magnitude < 0.01){
                finalObj[0].transform.position = cubeCorner[i];
                cube[i] = finalObj[0];
                finalObj[0].GetComponent<Outline>().OutlineColor = Color.blue;
                    }
          }                            
          
          onFrame.Add(finalObj[0]);
          finalObj.RemoveAt(0);
        }
    }
    public void getFinalObject()
    {
        t2.text = "get";
        finalObj = FinalObjects.GetComponent<FinalObjects>().finalObj;

    }
    private void Update()
    {
        if (handPoseManager.GetComponent<HandPoseManager>().phase == 1)
        {
            frameButton.SetActive(true);
        }
        else
        {
         frameButton.SetActive(false);
        }
    }
    void Start()
    {
        p = pinch.GetComponent<pinch>();
        InvokeRepeating("RepeatedMethod", 1f, 0.5f);
    }
    private void RepeatedMethod()
    {
        float f = (thumb.transform.position - index.transform.position).magnitude;
        if (f < 0.01)
            ispinch = true;
        else
            ispinch = false;
        t.text = ispinch.ToString();
    }

        void anchorChoose()
    {
        anchor.Clear();
        if(frame.GetComponent<frame>().Frame == "rect"){
        if(rect[0] && rect[2]){
                type = "2"; 
          anchor.Add(rect[0]);
          anchor.Add(rect[2]);
          GrabAgent.GetComponent<GrabAgentObject>().MovingObject[0] = rect[0];
          GrabAgent.GetComponent<GrabAgentObject>().MovingObject[1] = rect[2];}
        else if(rect[1] && rect[3]){
                type = "1";
          anchor.Add(rect[1]);
          anchor.Add(rect[3]);
          GrabAgent.GetComponent<GrabAgentObject>().MovingObject[0] = rect[1];
          GrabAgent.GetComponent<GrabAgentObject>().MovingObject[1] = rect[3];}
        }
        if (frame.GetComponent<frame>().Frame == "tri"){
                anchor.Add(tri[0]);
                anchor.Add(tri[1]);
                //anchor.Add(tri[2]);
                GrabAgent.GetComponent<GrabAgentObject>().MovingObject[0] = tri[0];
                GrabAgent.GetComponent<GrabAgentObject>().MovingObject[1] = tri[1];
        }
        if (frame.GetComponent<frame>().Frame == "circle"){
                anchor.Add(circle[0]);
                anchor.Add(circle[1]);
                anchor.Add(circle[2]);
            GrabAgent.GetComponent<GrabAgentObject>().MovingObject[0] = circle[0];
            GrabAgent.GetComponent<GrabAgentObject>().MovingObject[1] = circle[1];
        }
        if (frame.GetComponent<frame>().Frame == "para"){
            if (para[0] && para[2])
            {
                anchor.Add(para[0]);
                anchor.Add(para[2]);
                GrabAgent.GetComponent<GrabAgentObject>().MovingObject[0] = para[0];
                GrabAgent.GetComponent<GrabAgentObject>().MovingObject[1] = para[2];
            }
            else if (para[1] && para[3])
            {
                anchor.Add(para[1]);
                anchor.Add(para[3]);
                GrabAgent.GetComponent<GrabAgentObject>().MovingObject[0] = para[1];
                GrabAgent.GetComponent<GrabAgentObject>().MovingObject[1] = para[3];
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
      /**/  foreach (var obj in anchor){
                obj.GetComponent<Outline>().OutlineColor = Color.green;
        }
    }
    // Update is called once per frame
  



}
                                                                                                                                                                                                                                                          