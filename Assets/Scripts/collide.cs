using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collide : MonoBehaviour
{
    public TMPro.TMP_Text t;
    public GameObject pinch;
    public GameObject frame;
    private pinch p;
    public string Frame;//the mark of frame
    public GameObject FinalObjects;
    public List<GameObject> finalObj;//get all objects from last level
    
    public List<GameObject> anchor;//all anchor object,hilight them,we can only contact with them

    public GameObject[] onFrame;//object already on now frame
    // Start is called before the first frame update

    private List<GameObject> rectCorner;// leftup rightup rightdown leftdown
    private Vector3[] rect;
    private void OnCollisionEnter(Collision collision){

        ContactPoint contact = collision.contacts[0];                                                                                                                                                      
        t.text = collision.gameObject.name;
        
        if(p.ispinch)//keep pinch,keep manipulate,todo make p more wending
        { 
          finalObj[0].transform.position = contact.point;
          finalObj[0].transform.parent = collision.gameObject.transform;
          
          if(frame.GetComponent<frame>().frame == 'rect'){
            rect = frame.GetComponent<frame>().rectCorner;
            for(int i = 0 ;i <= 3 ;i++)
            if((finalObj[0].transform.position-rect[i]).magnitude < 0.01)
            {
                finalObj[0].transform.position = rect[i];
                rectCorner[i] = finalObj[0];
            }
          }
          
          
          onFrame.add(finalObj[0]);
          finalObj.RemoveAt(0);
        }
    }
    void Start()
    {
        p = pinch.GetComponent<pinch>();
        finalObj =  FinalObjects.GetComponent<FinalObjects >().finalObj;
    }
    void anchorChoose()
    {
        anchor.clear();
        if(Frame == 'rect'){
        // select three object by distance ，add position correct in the corner（make object right at the corner）
        if(rectCorner[0] && rectCorner[2])
        {
          anchor.add(rectCorner[0]);
          anchor.add(rectCorner[2]);
        }
        else if(rectCorner[1] && rectCorner[3]){
          anchor.add(rectCorner[1]);
          anchor.add(rectCorner[3]);
        }
        }

        foreach i in anchor{
            //高亮
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
   

}
