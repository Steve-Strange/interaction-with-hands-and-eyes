using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
public class singleSelect : MonoBehaviour
{
    [SerializeField]
    string name;
    public GameObject[] targetGameobject;//存所有的需要被操作的目标物体
    // Start is called before the first frame update
    private float timer = 0;
    private float selectTime = 0;
    private float manipulateTime = 0;
    public float beginSelectTime;//开始选择的时间
    public float beginManipulateTime;//开始操纵的时间
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
        //先判断是否存在，再创建
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
        if(timer > 3 && begin == true )//开始三秒之后开始计时
        {
            beginSelectTime = timer;
            begin = false;

        }
    }
    public void selectOneObject()//把每次选择的时间加上
    {
        selectTime += timer - beginSelectTime;
        beginManipulateTime = timer;
    }
    public void finishOneObject()//把每次操纵时间加上
    {
        manipulateTime += timer - beginManipulateTime;
        beginSelectTime = timer;
    }
}
