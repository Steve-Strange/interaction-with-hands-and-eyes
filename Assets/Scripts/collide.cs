using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collide : MonoBehaviour
{
    public TMPro.TMP_Text t;

    //public TMPro.TMP_Text tt;

    public GameObject pinch;
    public GameObject frame;
    private pinch p;
    public string Frame;//the mark of frame
    public GameObject FinalObjects;
    public List<GameObject> finalObj;//get all objects from last level
    
    public List<GameObject> anchor;//all anchor object,hilight them,we can only contact with them

    public List<GameObject> onFrame;//object already on now frame
    // Start is called before the first frame update

    private List<GameObject> rectCorner;// leftup rightup rightdown leftdown
    private List<GameObject> triCorner;// leftup rightup rightdown leftdown
    private List<GameObject> circle;// 圆用三个三角形
    private List<GameObject> paraCorner;// 圆用三个三角形
    private Vector3[] rect;
    private Vector3[] para;
    private Vector3[] tri;

    public GameObject test;

    private void OnCollisionEnter(Collision collision){

        ContactPoint contact = collision.contacts[0];                                                                                                                                                      
       
        
        if(p.ispinch & finalObj.Count !=0 )//keep pinch,keep manipulate,todo make p more wending
        { 
          finalObj[0].transform.position = contact.point;
          finalObj[0].transform.parent = collision.gameObject.transform;
          
          if(frame.GetComponent<frame>().Frame == "rect"){
            rect = frame.GetComponent<frame>().rectCorner;
            for(int i = 0 ;i <= 3 ;i++)
            if((finalObj[0].transform.position-rect[i]).magnitude < 0.01)
            {
                //tt.text = "yes";
                finalObj[0].transform.position = rect[i];
                rectCorner[i] = finalObj[0];
                finalObj[0].GetComponent<Outline>().OutlineColor = Color.blue;
                    }
          }

          if(frame.GetComponent<frame>().Frame == "tri"){
            tri = frame.GetComponent<frame>().triCorner;
            for(int i = 0 ;i <= 2 ;i++)
            if((finalObj[0].transform.position-tri[i]).magnitude < 0.01)
            {
                finalObj[0].transform.position = tri[i];
                triCorner[i] = finalObj[0];
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
        { Debug.Log("12W121");
            
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
        foreach (var obj in anchor){
                obj.GetComponent<Outline>().OutlineColor = Color.green;
        }
    }
    // Update is called once per frame
  



}
