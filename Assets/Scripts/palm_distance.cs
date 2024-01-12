using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class palm_distance : MonoBehaviour
{
    public GameObject leftPalm;
    public GameObject head;
    public TMP_Text distance_text;
    public TMP_Text forward_x;
    public TMP_Text forward_z;
    public TMP_Text palm_x;
    public TMP_Text palm_z;
    public TMP_Text gap_x;
    public TMP_Text gap_z;
    public Vector3 forward;
    public GameObject boxCol;//ע�ӵ���������
    public float length;
    public float height;
    public float width;
    public float dMin;
    public float dMax;
    public float palmMin;
    public float palmStart;
    public float palmNow;
    public float dcenter;//ע�ӵ�����������뵱ǰ�����ƽ��ľ���
   
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
      //�������Ҫ�ĳ���������ʱ
           forward = -head.transform.forward;
           forward.y = 0;
           forward = forward.normalized;
          // head ��λ��ҲҪ�̶���ͷ�ƶ���Ӱ���ֲ�λ���ж�
        float z = leftPalm.transform.position.z - head.transform.position.z;
        float x = leftPalm.transform.position.x - head.transform.position.x;
        Vector3 dis = new Vector3(x,  0,z);
        float distance = Vector3.Dot(dis,forward);
        distance_text.text = distance.ToString();
        palm_x.text = leftPalm.transform.position.x.ToString();
        palm_z.text = leftPalm.transform.position.z.ToString();
        forward_x.text = forward.x.ToString();
        forward_z.text = forward.z.ToString();
        gap_x.text = x.ToString();
        gap_z.text = z.ToString();
    }
}
