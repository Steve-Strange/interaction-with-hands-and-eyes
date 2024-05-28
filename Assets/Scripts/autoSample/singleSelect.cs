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
    public GameObject MovingRecorder;
    // Start is called before the first frame update
    public float timer = 0;
    private float selectTime = 0;//ĿǰΪֹ����ѡ��ʱ��
    private float manipulateTime = 0;//ĿǰΪֹ���ܲ���ʱ��
    private float coarseManipulateTime = 0;
    private float fineManipulateTime = 0;//ĿǰΪֹ���ܾ�ϸ����ʱ��
    public float beginSelectTime;//��ʼѡ���ʱ��
    public float beginManipulateTime;//��ʼ���ݵ�ʱ��
    public float beginFineManipulateTime;//��ʼ��ϸ���ݵ�ʱ��
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
        timer += Time.deltaTime;
        if(timer > 5 && begin == true )//��ʼ����֮��ʼ��ʱ
        {
            beginSelectTime = timer;
            begin = false;
        }
    } 
    public void writeFile(string a)
    {
        logs += a + "\n";
    }
    public void finishAll()
    {
        MovingRecorder.GetComponent<MovingRecorder>().finishAll();//记录动作的结束
        writeFile(MovingRecorder.GetComponent<MovingRecorder>().MovingData);//将动作的总移动写入当前的记录中
        File.WriteAllText(filePath, logs);
    }
   
    public void selectOneObject()//��ÿ��ѡ���ʱ�����
    {
        var gap = timer - beginSelectTime;
        if (sampleType == 0)
        {//仅选择
            beginSelectTime = timer;
            MovingRecorder.GetComponent<MovingRecorder>().restart();
        }
        else
        {
            beginManipulateTime = timer;
        }

        logs += "thisSelectionTime:" + gap + "    ";
        logs += "allSelectionTime:" + timer + "\n";
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

        logs += "thisCoarseManipulateTime:" + temp3 + "    ";
        logs += "allCoarseManipulateTime:" + coarseManipulateTime + "\n";
        logs += "thisFineManipulateTime:" + temp2 + "    ";
        logs += "allFineManipulateTime:" + fineManipulateTime + "\n";
        logs += "thisManipulateTime:" + temp + "    ";
        logs += "allManipulateTime:" + manipulateTime + "\n";
        MovingRecorder.GetComponent<MovingRecorder>().restart();
    }
    public void finishCoarseOneObject()//��ÿ�δֲ���ʱ�����
    {
        coarseManipulateTime += (timer - beginManipulateTime);
        beginFineManipulateTime = timer;
    }
}
