using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
public class singleSelect : MonoBehaviour
{
    [SerializeField]
    string name;
    public GameObject[] targetGameobject;//�����е���Ҫ��������Ŀ������
    // Start is called before the first frame update
    private float timer = 0;
    private float selectTime = 0;
    private float manipulateTime = 0;
    public float beginSelectTime;//��ʼѡ���ʱ��
    public float beginManipulateTime;//��ʼ���ݵ�ʱ��
    public TMP_Text t;
    public TMP_Text t2;
    public TMP_Text t3;
    void Start()
    {
        timer = 0;
        var path = Application.dataPath;
        Debug.Log(Application.dataPath);
        if(!Directory.Exists(path + "/datas/" + name))
            Directory.CreateDirectory(path + "/datas/" + name);
        //���ж��Ƿ���ڣ��ٴ���
        if (!File.Exists(Application.dataPath + "/datas/" + name + "/TextRead.txt"))
        {
            FileStream fileStream = new FileStream(Application.dataPath + "/datas/" + name + "/TextRead.txt", FileMode.OpenOrCreate);
            fileStream.Close();
        }

    }
    private bool begin = true;
    // Update is called once per frame
    void Update()
    {
        t.text = timer.ToString();
        t2.text = selectTime.ToString();
        t3.text = manipulateTime.ToString();
        timer += Time.deltaTime;
        if(timer > 3 && begin == true )//��ʼ����֮��ʼ��ʱ
        {
            beginSelectTime = timer;
            begin = false;

        }
    }
    public void selectOneObject()//��ÿ��ѡ���ʱ�����
    {
        selectTime += timer - beginSelectTime;
        beginManipulateTime = timer;
    }
    public void finishOneObject()//��ÿ�β���ʱ�����
    {
        manipulateTime += timer - beginManipulateTime;
        beginSelectTime = timer;
    }
}
