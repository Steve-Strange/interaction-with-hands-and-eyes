using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collide : MonoBehaviour
{
    public TMPro.TMP_Text t;
    public GameObject pinch;
    private pinch p;
    public GameObject FinalObjects;
    public List<GameObject> finalObj;
    // Start is called before the first frame update
    private void OnCollisionEnter(Collision collision)
    {

        ContactPoint contact = collision.contacts[0];                                                                                                                                                      
        t.text = collision.gameObject.name;
        
        if(p.ispinch)//修改，如何更好在线上
        { 
          finalObj[0].transform.position = contact.point;
          finalObj.RemoveAt(0);
        }

           

       

    }
    void Start()
    {
        p = pinch.GetComponent<pinch>();
        finalObj =  FinalObjects.GetComponent<FinalObjects >().finalObj;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
   

}
