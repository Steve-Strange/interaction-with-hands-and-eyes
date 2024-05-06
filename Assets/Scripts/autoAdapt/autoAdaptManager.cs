using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class autoAdaptManager : MonoBehaviour
{
    //控制手势的脚本
    // Start is called before the first frame update
    public GameObject recorder;//为了使时间一致，借用recorder的timer
    public GameObject temp;//检测手指间夹角
    void Start()
    {
        
    }
    private float angleGap = 0f;
    private int mark = 0;
    // Update is called once per frame
    void Update()
    {
        if(recorder.GetComponent<singleSelect>().timer == 1f)//开始后一秒开始
        {
            mark = 0;
            angleGap = 0f;
        }else if(recorder.GetComponent<singleSelect>().timer > 1f && recorder.GetComponent<singleSelect>().timer < 3f)
        {
            mark += 1;
            //angleGap += 检测手指间的角度 
        }else if(recorder.GetComponent<singleSelect>().timer >= 3f)
        {
            changeBackboard();
        }

    }
    //开始三秒，保持手掌张开，测平均角度-->大于其就当作手掌张开的阈值
    void changeBackboard()
    {
        //计算


    }
}
