using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
public class singleSelect : MonoBehaviour
{
    [SerializeField]
    string userName;
    public GameObject[] targetGameobject;//存所有的需要被操作的目标物体
    // Start is called before the first frame update
    private float timer = 0;
    private float selectTime = 0;//目前为止的总选择时间
    private float manipulateTime = 0;//目前为止的总操纵时间
    private float coarseManipulateTime = 0;
    private float fineManipulateTime = 0;//目前为止的总精细操纵时间
    public float beginSelectTime;//开始选择的时间
    public float beginManipulateTime;//开始操纵的时间
    public float beginFineManipulateTime;//开始精细操纵的时间
    public TMP_Text t;
    public TMP_Text t2;
    public TMP_Text t3;
    private string folderPath;
    private string filePath;
    private string fileName;
    private string logs;
    void Start()
    {
        logs = "";
        timer = 0;
        // 获取外部存储器的路径
        folderPath = Application.persistentDataPath;
        fileName = userName + "-" + System.DateTime.Now.ToString("MM.dd-HH:mm:ss") + ".txt";

        // 设置文件路径和名称
        filePath = Path.Combine(folderPath, fileName);

    }
    private bool begin = true;
    // Update is called once per frame
    void Update()
    {
        t.text = timer.ToString();
        t2.text = selectTime.ToString();
        t3.text = manipulateTime.ToString();
        timer += Time.deltaTime;
        if(timer > 3 && begin == true )//开始三秒之后开始计时
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
        logs += a  + "\n";
    }
    public void selectOneObject()//把每次选择的时间加上
    { 
        var temp = timer - beginSelectTime;
        selectTime += temp;
        beginManipulateTime = timer;
        logs += "thisSelectionTime: " + temp + "\n";
        logs += "allSelectionTime: " + selectTime + "\n";
    }
    public void finishOneObject()//把每次操纵时间加上
    {
        var temp = timer - beginManipulateTime;
        manipulateTime += temp;
        var temp2 = timer - beginFineManipulateTime;
        fineManipulateTime += temp2;
        beginSelectTime = timer;
        var temp3 = temp - temp2;
        coarseManipulateTime += temp3;

        logs += "thisCoarseManipulateTime: " + temp3 + "\n";
        logs += "allCoarseManipulateTime: " + coarseManipulateTime + "\n";
        logs += "thisFineManipulateTime: " + temp2 + "\n";
        logs += "allFineManipulateTime: " + fineManipulateTime + "\n";
        logs += "thisManipulateTime: " + temp + "\n";
        logs += "allManipulateTime: " + manipulateTime + "\n";
    }
    public void finishCoarseOneObject()//把每次粗操纵时间加上
    {
        coarseManipulateTime += timer - beginManipulateTime;
        beginFineManipulateTime = timer;
    }
}
