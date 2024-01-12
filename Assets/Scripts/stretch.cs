using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class stretch : MonoBehaviour
{
 
    public TMP_Text text;
    public GameObject ob;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void stretch1()
    {
        text.text = "yes";
    }
    public void stretch2()
    {
        text.text = "no";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
