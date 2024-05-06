using System.Collections;
using System.IO;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Threading;

public class TimeRecorder : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject agentObject;

    public List<GameObject> CompleteObjects = new List<GameObject>();
    public Dictionary<GameObject, int> MovingObjectStatus = new Dictionary<GameObject, int>();

    // public TMP_InputField log;
    public GameObject pinchObject;
    public float coarseMovingTime;
    private GameObject HandPoseManager;

    private string folderPath;
    private string filePath;
    private string fileName;
    private bool finishStatus = false;
    public string userName;
    private GameObject MovingRecorder;


    void Start()
    {
        HandPoseManager = GameObject.Find("HandPoseManager");
        MovingRecorder = GameObject.Find("MovingRecorder");

        // 获取外部存储器的路径
        folderPath = Application.persistentDataPath;
        fileName = userName + "-" + System.DateTime.Now.ToString("MMddHHmmss") + ".txt";

        // 设置文件路径和名称
        filePath = Path.Combine(folderPath, fileName);
    }

    // Update is called once per frame
    void Update()
    {
        // log.text = "SelectionTime: " + HandPoseManager.GetComponent<HandPoseManager>().selectionTime + "\n";
        // log.text += "placingTime" + HandPoseManager.GetComponent<HandPoseManager>().placingTime + "\n" + 
        //     "coarseMovingTime: " + coarseMovingTime + "\n" + 
        //     "MovingTime: " + HandPoseManager.GetComponent<HandPoseManager>().movingTime + "\n";

        // log.text += "CompleteObjects: " + CompleteObjects.Count + "\n";
        // log.text += "objectsWithTargets: " + HandPoseManager.GetComponent<HandPoseManager>().objectsWithTargets.Count + "\n";
        if(CompleteObjects.Count == HandPoseManager.GetComponent<HandPoseManager>().objectsWithTargets.Count){

            // log.text += "Finished!!!!!!!!!!!!";
            if(!finishStatus)
            {
                File.WriteAllText(filePath, "Selection Time: " + HandPoseManager.GetComponent<HandPoseManager>().selectionTime + "\n" + 
                                    "Placing Time: " + HandPoseManager.GetComponent<HandPoseManager>().placingTime + "\n" +
                                    "Coarse Moving Time: " + coarseMovingTime + "\n" + 
                                    "Moving Time: " + HandPoseManager.GetComponent<HandPoseManager>().movingTime + "\n" + 
                                    MovingRecorder.GetComponent<MovingRecorder>().output
                                );

                finishStatus = true;
            }
        }
        
        if (agentObject.GetComponent<GrabAgentObject>().MovingObject.Count > 0)
        {
            if(MovingObjectStatus[agentObject.GetComponent<GrabAgentObject>().MovingObject[0]]==0) {
                coarseMovingTime += Time.deltaTime;
            }
        }

    }


    float RotationGap(GameObject obj1, GameObject obj2)
    {
        return Mathf.Min(Mathf.Abs(obj1.transform.eulerAngles.x - obj2.transform.eulerAngles.x), 360 - Mathf.Abs(obj1.transform.eulerAngles.x - obj2.transform.eulerAngles.x)) + 
                Mathf.Min(Mathf.Abs(obj1.transform.eulerAngles.y - obj2.transform.eulerAngles.y), 360 - Mathf.Abs(obj1.transform.eulerAngles.y - obj2.transform.eulerAngles.y)) +
                Mathf.Min(Mathf.Abs(obj1.transform.eulerAngles.z - obj2.transform.eulerAngles.z), 360 - Mathf.Abs(obj1.transform.eulerAngles.z - obj2.transform.eulerAngles.z));
    }

}
