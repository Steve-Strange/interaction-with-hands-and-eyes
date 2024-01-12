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
    //�����ί��
    public delegate void MyValueChanged(object sender, EventArgs e);
    //��ί����������¼�
    public event MyValueChanged OnMyValueChanged;
    //�¼���������

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
