using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class select : MonoBehaviour
{
    [SerializeField]
    public LayerMask LayerMask;//�޶�������Ʒ�㼶

    public List<GameObject> detectsAll;//���ڵ����弯��,�����շ��ص���Ʒ����
    public List<GameObject> detects;//���ڵ����弯��,��ÿ��ѡ�񷵻ص���Ʒ����
    public GameObject distance;//��ø�������������,���С
    public GameObject EyeDetect;//�����۶��������
    public GameObject head;//�����

    bool m_Started = false;//�Ƿ�ѿ��߻��Ƴ���

    private palm_distance palmDistance;//distance�Ϲ��صĽű�

    private bool multi = false;//�Ƿ��ڶ��ѡ����
    private bool selected = false;//�Ƿ���ѡ����
    private List<GameObject> withinCameras;//��ǰ������ɼ���������Ʒ����
    // Start is called before the first frame update
    void Start()
    {
        palmDistance = distance.GetComponent<palm_distance>();//��ȡ�ű�
    }

    // Update is called once per frame
    void Update()
    {
        if (selected)
        {
            detects = new List<GameObject>();//ÿ�θ�����ѡ�����壬�������
            Vector3 scale = new Vector3(palmDistance.length, palmDistance.width, palmDistance.height);
            var center = (palmDistance.width / 2 - palmDistance.dcenter) * palmDistance.forward + palmDistance.boxCol.transform.position;
            Collider[] objs = Physics.OverlapBox(center, scale / 2, head.transform.rotation, LayerMask);
            //��һ��������ߴ�Сɶ��˼
            foreach (var obj in objs)
            {
                detects.Add(obj.gameObject);//����ͨ����ײ���õ�ǰ�����gameObject
        }

        }
        
    }
}
