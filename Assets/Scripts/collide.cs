using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collide : MonoBehaviour
{
    public TMPro.TMP_Text t;
    // Start is called before the first frame update
    private void OnCollisionEnter(Collision collision)
    {


        t.text = collision.gameObject.name;


    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
