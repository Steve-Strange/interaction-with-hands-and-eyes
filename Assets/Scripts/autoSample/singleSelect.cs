using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
public class singleSelect : MonoBehaviour
{
    [SerializeField]
    string userName;
    [SerializeField]
    public int sampleType;
    public GameObject[] targetGameobject;//�����е���Ҫ��������Ŀ������
    // Start is called before the first frame update
    public float timer = 0;
    private float selectTime = 0;//ĿǰΪֹ����ѡ��ʱ��
    private float manipulateTime = 0;//ĿǰΪֹ���ܲ���ʱ��
    private float coarseManipulateTime = 0;
    private float fineManipulateTime = 0;//ĿǰΪֹ���ܾ�ϸ����ʱ��
    public float beginSelectTime;//��ʼѡ���ʱ��
    public float beginManipulateTime;//��ʼ���ݵ�ʱ��
    public float beginFineManipulateTime;//��ʼ��ϸ���ݵ�ʱ��
    // public TMP_Text t;
    // public TMP_Text t2;
    // public TMP_Text t3;
    private string folderPath;
    private string filePath;
    private string fileName;
    private string logs;
    void Start()
    {
        logs = "";
        timer = 0;
        // ��ȡ�ⲿ�洢����·��
        folderPath = Application.persistentDataPath;
        if(sampleType == 2){
            fileName = userName + "-" + System.DateTime.Now.ToString("MMddHHmmss") + ".txt";
        }else if(sampleType == 0){
            fileName = userName + "-" +"select_"+ System.DateTime.Now.ToString("MMddHHmmss") + ".txt";
        }else{
            fileName = userName + "-" +"manipulate_"+ System.DateTime.Now.ToString("MMddHHmmss") + ".txt";
        }
        // �����ļ�·��������
        filePath = Path.Combine(folderPath, fileName);

    }
    private bool begin = true;
    // Update is called once per frame
    void Update()
    {
       // t.text = timer.ToString();
       // t2.text = selectTime.ToString();
       // t3.text = manipulateTime.ToString();
        timer += Time.deltaTime;
        if(timer > 3 && begin == true )//��ʼ����֮��ʼ��ʱ
        {
            beginSelectTime = timer;
            begin = false;

        }
    }
    public void finishAll()
    {
        File.WriteAllText(filePath, logs);
    }
    public void writeFile(string a)
    {
        logs += a + "\n";
    }
    public void selectOneObject()//��ÿ��ѡ���ʱ�����
    {
        var gap = timer - beginSelectTime;
        if (sampleType == 0)
        {//仅选择
            beginSelectTime = timer;
        }
        else
        {
            beginManipulateTime = timer;
        }

        logs += "thisSelectionTime: " + gap + "\n";
        logs += "allSelectionTime: " + timer + "\n";
    }
    public void finishOneObject()//��ÿ�β���ʱ�����
    {
        var temp = timer - beginManipulateTime;
        manipulateTime += temp;
        var temp2 = timer - beginFineManipulateTime;
        fineManipulateTime += temp2;
        if (sampleType != 1)
        {
            beginSelectTime = timer;
        }
        if (sampleType == 1)
        {
            beginManipulateTime = timer;
        }

        var temp3 = temp - temp2;
        coarseManipulateTime = coarseManipulateTime + temp3;

        logs += "thisCoarseManipulateTime: " + temp3 + "\n";
        logs += "allCoarseManipulateTime: " + coarseManipulateTime + "\n";
        logs += "thisFineManipulateTime: " + temp2 + "\n";
        logs += "allFineManipulateTime: " + fineManipulateTime + "\n";
        logs += "thisManipulateTime: " + temp + "\n";
        logs += "allManipulateTime: " + manipulateTime + "\n\n";
    }
    public void finishCoarseOneObject()//��ÿ�δֲ���ʱ�����
    {
        coarseManipulateTime += (timer - beginManipulateTime);
        beginFineManipulateTime = timer;
    }
}
