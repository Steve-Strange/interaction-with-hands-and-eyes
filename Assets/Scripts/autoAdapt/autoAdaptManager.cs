using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class autoAdaptManager : MonoBehaviour
{
    //�������ƵĽű�
    // Start is called before the first frame update
    public GameObject recorder;//Ϊ��ʹʱ��һ�£�����recorder��timer
    public GameObject temp;//�����ָ��н�
    void Start()
    {
        
    }
    private float angleGap = 0f;
    private int mark = 0;
    // Update is called once per frame
    void Update()
    {
        if(recorder.GetComponent<singleSelect>().timer == 1f)//��ʼ��һ�뿪ʼ
        {
            mark = 0;
            angleGap = 0f;
        }else if(recorder.GetComponent<singleSelect>().timer > 1f && recorder.GetComponent<singleSelect>().timer < 3f)
        {
            mark += 1;
            //angleGap += �����ָ��ĽǶ� 
        }else if(recorder.GetComponent<singleSelect>().timer >= 3f)
        {
            changeBackboard();
        }

    }
    //��ʼ���룬���������ſ�����ƽ���Ƕ�-->������͵��������ſ�����ֵ
    void changeBackboard()
    {
        //����


    }
}
