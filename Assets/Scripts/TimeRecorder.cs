using System.Collections;
using System.IO;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimeRecorder : MonoBehaviour
{
    // Start is called before the first frame update

    public List<GameObject> CompleteObjects = new List<GameObject>();
    // public TMP_InputField log;

    private GameObject HandPoseManager;

    private string folderPath;
    private string filePath;
    private string fileName;
    private bool finishStatus = false;
    public string userName;
    private GameObject MovingRecorder;
    public string writeFileContext;

    void Start()
    {
        HandPoseManager = GameObject.Find("HandPoseManager");
        MovingRecorder = GameObject.Find("MovingRecorder");

        // 获取外部存储器的路径
        folderPath = Application.persistentDataPath;
        fileName = userName + "-" + SceneManager.GetActiveScene().name + "-" + System.DateTime.Now.ToString("MMddHHmmss") + ".txt";

        // 设置文件路径和名称
        filePath = Path.Combine(folderPath, fileName);
    }

    // Update is called once per frame
    void Update()
    {
        // log.text = writeFileContext;
        // log.text = "Selection Time: " + HandPoseManager.GetComponent<HandPoseManager>().selectionTime + "\n" + 
        //                             "Manipulation Time: " + HandPoseManager.GetComponent<HandPoseManager>().movingTime;
        // log.text += "phase: " + HandPoseManager.GetComponent<HandPoseManager>().phase + "\n";

        // log.text += "placingTime" + HandPoseManager.GetComponent<HandPoseManager>().placingTime + "\n" + 
        //     "coarseMovingTime: " + coarseMovingTime + "\n" + 
        //     "MovingTime: " + HandPoseManager.GetComponent<HandPoseManager>().movingTime + "\n";

        // log.text += "CompleteObjects: " + CompleteObjects.Count + "\n";
        // log.text = "objectsWithTargets: " + HandPoseManager.GetComponent<HandPoseManager>().objectsWithTargets.Count + "\n";
        if(CompleteObjects.Count == HandPoseManager.GetComponent<HandPoseManager>().objectsWithTargets.Count && HandPoseManager.GetComponent<HandPoseManager>().objectsWithTargets.Count != 0){
            if(!finishStatus)
            {
                MovingRecorder.GetComponent<MovingRecorder>().finishAll();

                File.WriteAllText(filePath, writeFileContext + "Selection Time: " + HandPoseManager.GetComponent<HandPoseManager>().selectionTime + "\n" + 
                                    "Manipulation Time: " + HandPoseManager.GetComponent<HandPoseManager>().movingTime + "\n" + 
                                    MovingRecorder.GetComponent<MovingRecorder>().MovingData
                                );

                finishStatus = true;
            }
        }
    }

}
