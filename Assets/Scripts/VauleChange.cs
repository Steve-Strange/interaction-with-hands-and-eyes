using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VauleChange : MonoBehaviour
{
    private float angle;
    public float Angle
    {
        get { return angle; }
        set
        {
            if(value != angle)
            {
                angleChange();
            }
            angle = value;
        }


    }
    //定义的委托
    public delegate void MyValueChanged(object sender, EventArgs e);
    //与委托相关联的事件
    public event MyValueChanged OnMyValueChanged;
    //事件触发函数

    private void angleChange()
    {
        if (OnMyValueChanged != null)
        {
            OnMyValueChanged(this, null);

        }

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
