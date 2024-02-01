using System.Security.Cryptography.X509Certificates;
using System.Numerics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collide : MonoBehaviour
{
    public TMPro.TMP_Text t;
    public GameObject pinch;
    public GameObject frame;
    private pinch p;
    public string frameMark;//the mark of frame
    public GameObject FinalObjects;
    public List<GameObject> finalObj;//get all objects from last level
    
    public List<GameObject> anchor;//all anchor object,hilight them,we can only contact with them

    public List<GameObject> onFrame;//object already on now frame

    private List<GameObject> rectCorner;// leftup rightup rightdown leftdown
    private Vector3[] rect;
    private List<GameObject> triCorner;// leftup rightup rightdown leftdown
    private Vector3[] tri;
    private List<GameObject> circleCorner;// 圆用前三个
    private Vector3[] circle;
    public int mark = 0;
    private List<GameObject> paraCorner;// 
    private Vector3[] para;
    private List<GameObject> penCorner;//
    private Vector3[] pen;
    private List<GameObject> cubeCorner;// 
    private Vector3[] cube;

    public GameObject test;

    private void OnCollisionEnter(Collision collision){

        ContactPoint contact = collision.contacts[0];                                                                                                                                                      
       
        
        if(p.ispinch & finalObj.Count !=0 ){ 

          finalObj[0].transform.position = contact.point;
          finalObj[0].transform.parent = collision.gameObject.transform;
          
          if(frame.GetComponent<frame>().Frame == "rect"){

            rect = frame.GetComponent<frame>().rectCorner;//line 

            for(int i = 0 ;i <= 3 ;i++)
            if((finalObj[0].transform.position-rect[i]).magnitude < 0.01){//有资格当anchor的变成蓝色
                //tt.text = "yes";
                finalObj[0].transform.position = rect[i];
                rectCorner[i] = finalObj[0];
                finalObj[0].GetComponent<Outline>().OutlineColor = Color.blue;
                    }
          }

          if(frame.GetComponent<frame>().Frame == "tri"){

            tri = frame.GetComponent<frame>().triCorner;

            for(int i = 0 ;i <= 2 ;i++)
            if((finalObj[0].transform.position-tri[i]).magnitude < 0.01){
                finalObj[0].transform.position = tri[i];
                triCorner[i] = finalObj[0];
                finalObj[0].GetComponent<Outline>().OutlineColor = Color.blue;
                    }
          }          

          if(frame.GetComponent<frame>().Frame == "circle"){
            //任意三个就可以
            if(mark<3){
                circle[mark] = finalObj[0];
                finalObj[0].GetComponent<Outline>().OutlineColor = Color.blue;
            } 
            mark++;
          }          

          if(frame.GetComponent<frame>().Frame == "pen"){

            pen = frame.GetComponent<frame>().penCorner;

            for(int i = 0 ;i <= 4 ;i++)
            if((finalObj[0].transform.position-pen[i]).magnitude < 0.01){
                finalObj[0].transform.position = pen[i];
                penCorner[i] = finalObj[0];
                finalObj[0].GetComponent<Outline>().OutlineColor = Color.blue;
                    }
          }  

          if(frame.GetComponent<frame>().Frame == "para"){

            para = frame.GetComponent<frame>().paraCorner;

            for(int i = 0 ;i <= 3 ;i++)
            if((finalObj[0].transform.position-para[i]).magnitude < 0.01){
                finalObj[0].transform.position = para[i];
                paraCorner[i] = finalObj[0];
                finalObj[0].GetComponent<Outline>().OutlineColor = Color.blue;
                    }
          }  
          if(frame.GetComponent<frame>().Frame == "cube"){//要能确定新的长宽高

            cube = frame.GetComponent<frame>().cubeCorner;

            for(int i = 0 ;i <= 7 ;i++)
            if((finalObj[0].transform.position-cube[i]).magnitude < 0.01){
                finalObj[0].transform.position = cube[i];
                paraCorner[i] = finalObj[0];
                finalObj[0].GetComponent<Outline>().OutlineColor = Color.blue;
                    }
          }                            
          
          onFrame.Add(finalObj[0]);
          finalObj.RemoveAt(0);
        }
    }
    void Start()
    {
        p = pinch.GetComponent<pinch>();
        finalObj =  FinalObjects.GetComponent<FinalObjects >().finalObj;
        test.GetComponent<Outline>().OutlineColor = Color.blue;
    } 
    void Update()
    {
        
        if (frame.GetComponent<frame>().Frame == "rect")
        {
            
            t.text = (test.transform.position - new Vector3(-0.1f,0.1f,0)).magnitude.ToString();
            rect = frame.GetComponent<frame>().rectCorner;
            for (int i = 0; i <= 3; i++)
                if ((test.transform.position - new Vector3(-0.1f, 0.1f, 0)).magnitude < 0.01)
                {
                   
                    test.transform.position = new Vector3(-0.1f, 0.1f, 0);
                    test.GetComponent<Outline>().OutlineColor = Color.blue;
                }
        }


    }
    
    void anchorChoose()
    {
        anchor.Clear();
        if(frame.GetComponent<frame>().Frame == "rect"){
        // select three object by distance ，add position correct in the corner（make object right at the corner）
        if(rectCorner[0] && rectCorner[2]){
          anchor.Add(rectCorner[0]);
          anchor.Add(rectCorner[2]);
        }
        else if(rectCorner[1] && rectCorner[3]){
          anchor.Add(rectCorner[1]);
          anchor.Add(rectCorner[3]);
        }
        }
        if (frame.GetComponent<frame>().Frame == "tri"){
                anchor.Add(triCorner[0]);
                anchor.Add(triCorner[1]);
                anchor.Add(triCorner[2]); 
        }
        if (frame.GetComponent<frame>().Frame == "circle"){
                anchor.Add(circle[0]);
                anchor.Add(circle[1]);
                anchor.Add(circle[2]);        
        }
        if (frame.GetComponent<frame>().Frame == "para"){
            if (paraCorner[0] && paraCorner[2])
            {
                anchor.Add(paraCorner[0]);
                anchor.Add(paraCorner[2]);
            }
            else if (paraCorner[1] && paraCorner[3])
            {
                anchor.Add(paraCorner[1]);
                anchor.Add(paraCorner[3]);
            }
        }
        if (frame.GetComponent<frame>().Frame == "cube"){
            // need three anchor to calculate 
            if (paraCorner[0] && paraCorner[2] && paraCorner[5])//0，2，5
            {
                anchor.Add(paraCorner[0]);
                anchor.Add(paraCorner[2]);
                anchor.Add(paraCorner[5]);
            }
            else if (paraCorner[0] && paraCorner[6] && paraCorner[2])//0，2，6
            {
                anchor.Add(paraCorner[0]);
                anchor.Add(paraCorner[2]);
                anchor.Add(paraCorner[6]);
            }
            else if (paraCorner[0] && paraCorner[4] && paraCorner[2])//0，2，4
            {
                anchor.Add(paraCorner[0]);
                anchor.Add(paraCorner[2]);
                anchor.Add(paraCorner[4]);
            }
            else if (paraCorner[0] && paraCorner[7] && paraCorner[2])//0，2，7
            {
                anchor.Add(paraCorner[0]);
                anchor.Add(paraCorner[2]);
                anchor.Add(paraCorner[7]);
            }
            else if (paraCorner[1] && paraCorner[3] && paraCorner[4])//1，3，4
            {
                anchor.Add(paraCorner[1]);
                anchor.Add(paraCorner[3]);
                anchor.Add(paraCorner[4]);
            }
            else if (paraCorner[1] && paraCorner[3] && paraCorner[2])//1，3，5
            {
                anchor.Add(paraCorner[1]);
                anchor.Add(paraCorner[3]);
                anchor.Add(paraCorner[5]);
            }
            else if (paraCorner[1] && paraCorner[3] && paraCorner[6])//1，3，6
            {
                anchor.Add(paraCorner[1]);
                anchor.Add(paraCorner[3]);
                anchor.Add(paraCorner[6]);
            }
            else if (paraCorner[1] && paraCorner[3] && paraCorner[7])//1，3，7
            {
                anchor.Add(paraCorner[1]);
                anchor.Add(paraCorner[3]);
                anchor.Add(paraCorner[7]);
            }
            else if (paraCorner[0] && paraCorner[4] && paraCorner[6])//4，6，0
            {
                anchor.Add(paraCorner[0]);
                anchor.Add(paraCorner[4]);
                anchor.Add(paraCorner[6]);
            }
            else if (paraCorner[1] && paraCorner[4] && paraCorner[6])//4，6，1
            {
                anchor.Add(paraCorner[1]);
                anchor.Add(paraCorner[4]);
                anchor.Add(paraCorner[6]);
            }
            else if (paraCorner[4] && paraCorner[6] && paraCorner[2])//4，6，2
            {
                anchor.Add(paraCorner[2]);
                anchor.Add(paraCorner[4]);
                anchor.Add(paraCorner[6]);
            }
            else if (paraCorner[4] && paraCorner[3] && paraCorner[6])//4，6，3
            {
                anchor.Add(paraCorner[3]);
                anchor.Add(paraCorner[4]);
                anchor.Add(paraCorner[6]);
            }
            else if (paraCorner[5] && paraCorner[7] && paraCorner[0])//5，7，0
            {
                anchor.Add(paraCorner[0]);
                 anchor.Add(paraCorner[5]);
                anchor.Add(paraCorner[7]);
            }
            else if (paraCorner[1] && paraCorner[7] && paraCorner[5])//5，7，1
            {
                anchor.Add(paraCorner[1]);
                 anchor.Add(paraCorner[5]);
                anchor.Add(paraCorner[7]);
            }
            else if (paraCorner[7] && paraCorner[5] && paraCorner[2])//5，7，2
            {
                anchor.Add(paraCorner[2]);
                 anchor.Add(paraCorner[5]);
                anchor.Add(paraCorner[7]);
            }
            else if (paraCorner[5] && paraCorner[3] && paraCorner[7])//5，7，3
            {
                anchor.Add(paraCorner[3]);
                anchor.Add(paraCorner[5]);
                anchor.Add(paraCorner[7]);
            } 
        }        
        foreach (var obj in anchor){
                obj.GetComponent<Outline>().OutlineColor = Color.green;
        }
    }
    // Update is called once per frame
  



}
