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

    public GameObject frame;
    // public TMP_InputField log;
    public GameObject pinchObject;
    public float MovingTime;
    public float coarseMovingTime;
    private GameObject HandPoseManager;

    private string folderPath;
    private string filePath;
    private string fileName;
    private bool finishStatus = false;
    public string userName;
    private GameObject MovingRecorder;
    private float stayInTimer = 0f;


    void Start()
    {
        HandPoseManager = GameObject.Find("HandPoseManager");
        MovingRecorder = GameObject.Find("MovingRecorder");

        // 获取外部存储器的路径
        folderPath = Application.persistentDataPath;
        fileName = userName + "-" + System.DateTime.Now.ToString("MM.dd-HH:mm:ss") + ".txt";

        // 设置文件路径和名称
        filePath = Path.Combine(folderPath, fileName);
    }

    // Update is called once per frame
    void Update()
    {
        
        if(CompleteObjects.Count == agentObject.GetComponent<GrabAgentObject>().ObjectsOnFrame.Count){

            frame.GetComponent<frame>().line.positionCount = 0;


            // log.text = "SelectionTime: " + HandPoseManager.GetComponent<HandPoseManager>().selectionTime + "\n" + 
            // "coarseMovingTime: " + coarseMovingTime + "\n" + 
            // "MovingTime: " + MovingTime + "\n";
            // log.text += "Finished!!!!!!!!!!!!";
            if(!finishStatus)
            {
                File.WriteAllText(filePath, "Selection Time: " + HandPoseManager.GetComponent<HandPoseManager>().selectionTime + "\n" + 
                                "Coarse Moving Time: " + coarseMovingTime + "\n" + "Moving Time: " + MovingTime + "\n" + MovingRecorder.GetComponent<MovingRecorder>().output
                                );
                finishStatus = true;
            }
        }
        else{
            // log.text = "CompleteObjects: " + CompleteObjects.Count + "\n";
            // log.text += "agentObject.GetComponent<GrabAgentObject>().TargetObjects: " + agentObject.GetComponent<GrabAgentObject>().TargetObjects.Count + "\n";
            // log.text += "FrameObjects: " + agentObject.GetComponent<GrabAgentObject>().ObjectsOnFrame.Count + "\n";

        }

        foreach (var obj in agentObject.GetComponent<GrabAgentObject>().ObjectsOnFrame){
            if(!MovingObjectStatus.ContainsKey(obj)) MovingObjectStatus.Add(obj, 0);
            // log.text += "distance: " + Vector3.Distance(obj.transform.position, agentObject.GetComponent<GrabAgentObject>().TargetObjects[obj].transform.position) + "rotation: " + RotationGap(obj, agentObject.GetComponent<GrabAgentObject>().TargetObjects[obj]) + "\n";
            if(!CompleteObjects.Contains(obj)){
                if (Vector3.Distance(obj.transform.position, agentObject.GetComponent<GrabAgentObject>().TargetObjects[obj].transform.position) < 
                    (obj.transform.GetComponent<Renderer>().bounds.size.x + obj.transform.GetComponent<Renderer>().bounds.size.y + obj.transform.GetComponent<Renderer>().bounds.size.z) / 9f &&
                    RotationGap(obj, agentObject.GetComponent<GrabAgentObject>().TargetObjects[obj]) < 30f){
                        MovingObjectStatus[obj] = 2;
                        obj.GetComponent<Outline>().OutlineColor = Color.red;
                        obj.GetComponent<Outline>().OutlineWidth = 6f;
                }
                else if (Vector3.Distance(obj.transform.position, agentObject.GetComponent<GrabAgentObject>().TargetObjects[obj].transform.position) < 
                    (obj.transform.GetComponent<Renderer>().bounds.size.x + obj.transform.GetComponent<Renderer>().bounds.size.y + obj.transform.GetComponent<Renderer>().bounds.size.z) / 3f){
                        MovingObjectStatus[obj] = 1;
                        obj.GetComponent<Outline>().OutlineColor = Color.yellow;
                        obj.GetComponent<Outline>().OutlineWidth = 4f;
                }
                else {
                    MovingObjectStatus[obj] = 0;
                    obj.GetComponent<Outline>().OutlineColor = Color.clear;
                }
            }

            if(MovingObjectStatus[obj]==2) {
                stayInTimer += Time.deltaTime;
            }
            else{
                stayInTimer = 0f;
            }
            if(stayInTimer > 0.3f && !agentObject.GetComponent<GrabAgentObject>().movingStatus){
                if(!CompleteObjects.Contains(obj)) CompleteObjects.Add(obj);
                obj.transform.position = agentObject.GetComponent<GrabAgentObject>().TargetObjects[obj].transform.position;
                obj.transform.rotation = agentObject.GetComponent<GrabAgentObject>().TargetObjects[obj].transform.rotation;
            }
        }

        
        if (agentObject.GetComponent<GrabAgentObject>().MovingObject.Count > 0)
        {
            if(MovingObjectStatus[agentObject.GetComponent<GrabAgentObject>().MovingObject[0]]==0) {
                coarseMovingTime += Time.deltaTime;
            }
            MovingTime += Time.deltaTime;
            
        }

    }


    float RotationGap(GameObject obj1, GameObject obj2)
    {
        return Mathf.Min(Mathf.Abs(obj1.transform.eulerAngles.x - obj2.transform.eulerAngles.x), 360 - Mathf.Abs(obj1.transform.eulerAngles.x - obj2.transform.eulerAngles.x)) + 
                Mathf.Min(Mathf.Abs(obj1.transform.eulerAngles.y - obj2.transform.eulerAngles.y), 360 - Mathf.Abs(obj1.transform.eulerAngles.y - obj2.transform.eulerAngles.y)) +
                Mathf.Min(Mathf.Abs(obj1.transform.eulerAngles.z - obj2.transform.eulerAngles.z), 360 - Mathf.Abs(obj1.transform.eulerAngles.z - obj2.transform.eulerAngles.z));
    }

}
