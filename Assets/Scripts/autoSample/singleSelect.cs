using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.SceneManagement;

public class singleSelect : MonoBehaviour
{
    [SerializeField]
    string userName;
    [SerializeField]
    public int sampleType;
    public GameObject scene;
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
    private float allM;
    void Start()
    {
        logs = "";
        timer = 0;
        allM = 0f;
        // ��ȡ�ⲿ�洢����·��
        folderPath = Application.persistentDataPath;
        if(sampleType == 2){
            fileName = userName + "-" + SceneManager.GetActiveScene().name + "-" + System.DateTime.Now.ToString("MMddHHmmss") + ".txt";
        }else if(sampleType == 0){
            fileName = userName + "-" + SceneManager.GetActiveScene().name + "-" + "select_"+ System.DateTime.Now.ToString("MMddHHmmss") + ".txt";
        }else{
            fileName = userName + "-" + SceneManager.GetActiveScene().name + "-" + "manipulate_"+ System.DateTime.Now.ToString("MMddHHmmss") + ".txt";
        }
        // �����ļ�·��������
        filePath = Path.Combine(folderPath, fileName);

    }
    private bool begin = true;
    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer > 5 && begin == true )
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
        scene.SetActive(false);
        writeFile("finish all");
        writeFile("Selection Time:"+(timer-allM));
        writeFile("Manipulate Time:" + allM);
        MovingRecorder.GetComponent<MovingRecorder>().finishAll();//记录动作的结束
        writeFile(MovingRecorder.GetComponent<MovingRecorder>().MovingData);//将动作的总移动写入当前的记录中
        writeFile("all selection time:" + timer);
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
        else{
            beginManipulateTime = timer;
        }
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
        allM += timer - beginManipulateTime;
        var temp3 = temp - temp2;
        logs += "allManipulateTime:" + timer + "\n";//完成这个操纵操作时所花费的时间
        MovingRecorder.GetComponent<MovingRecorder>().restart();
    }
    public void finishCoarseOneObject()//��ÿ�δֲ���ʱ�����
    {
        logs += "FineManipulateTimeStart:" + timer + "\n";//完成粗操作，开始精细操作的时间
        //coarseManipulateTime += (timer - beginManipulateTime);
       // beginFineManipulateTime = timer;
    }
}
