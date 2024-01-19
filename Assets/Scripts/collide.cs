using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collide : MonoBehaviour
{
    public TMPro.TMP_Text t;
    public GameObject pinch;
    private pinch p;
    public GameObject now;
    
    // Start is called before the first frame update
    private void OnCollisionEnter(Collision collision)
    {


        t.text = collision.gameObject.name;
        if(p.ispinch)//修改，如何更好在线上
        { 
          now.transform.position = collision.collider.ClosestPoint(transform.position);
        }

           

       

    }
    void Start()
    {
        p = pinch.GetComponent<pinch>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
