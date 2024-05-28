using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using static UnityEngine.UISystemProfilerApi;

public class MovingRecorder : MonoBehaviour
{
    //会记录每次移动的数据和最后全部加在一起的总数据
    [SerializeField]
    string userName;
    public GameObject head;
    public GameObject leftHand;
    public GameObject rightHand;
    public GameObject emptyHead;
    public GameObject emptyRight;
    public GameObject emptyLeft;
    private Dictionary<GameObject, Vector3> lastPosition = new Dictionary<GameObject, Vector3>();
    private Dictionary<GameObject, Quaternion> lastRotation = new Dictionary<GameObject, Quaternion>();
    private Dictionary<GameObject, float> totalDistanceMoved = new Dictionary<GameObject, float>();
    private Dictionary<GameObject, float> totalAngleRotated = new Dictionary<GameObject, float>();
    private string filePath;
    private string logs;
    // Start is called before the first frame update
    void Start()
    {
        //新建一个文件来存
        logs = "";
        var folderPath = Application.persistentDataPath;
        var fileName = userName + "-" + "move_" + System.DateTime.Now.ToString("MMddHHmmss") + ".txt";
        filePath = Path.Combine(folderPath, fileName);
        emptyHead = new GameObject("emptyHead");
        emptyRight = new GameObject("emptyRight");
        emptyLeft = new GameObject("emptyLeft");


        lastPosition[head] = head.transform.position;
        lastPosition[leftHand] = leftHand.transform.position;
        lastPosition[rightHand] = rightHand.transform.position;
        lastRotation[head] = head.transform.rotation;
        lastRotation[leftHand] = leftHand.transform.rotation;
        lastRotation[rightHand] = rightHand.transform.rotation;
        totalDistanceMoved[head] = 0f;
        totalDistanceMoved[leftHand] = 0f;
        totalDistanceMoved[rightHand] = 0f;
        
        totalAngleRotated[emptyHead] = 0f;
        totalDistanceMoved[emptyHead] = 0f;
        totalAngleRotated[emptyRight] = 0f;
        totalDistanceMoved[emptyRight] = 0f;
        totalAngleRotated[emptyLeft] = 0f;
        totalDistanceMoved[emptyLeft] = 0f;

        totalAngleRotated[head] = 0f;
        totalAngleRotated[leftHand] = 0f;
        totalAngleRotated[rightHand] = 0f;
    }
    public string MovingData;
    public void finishAll()
    {
        MovingData = "Moved Distance: \n" + "head: " + totalDistanceMoved[emptyHead] + "\n" + "right:" + totalDistanceMoved[emptyRight] + "\n" + "left:" + totalDistanceMoved[emptyLeft] + "\n" +
                 "Rotated Angle: \n" + "head: " + totalAngleRotated[emptyHead] + "\n" + "right:" + totalAngleRotated[emptyRight] + "\n" + "left:" + totalAngleRotated[emptyLeft];
        logs +="all distance and rotate angle\n"+ "Moved Distance: \n" + "head: " + totalDistanceMoved[emptyHead] + "\n" + "right:" + totalDistanceMoved[emptyRight] + "\n" + "left:" + totalDistanceMoved[emptyLeft] + "\n" +
                 "Rotated Angle: \n" + "head: " + totalAngleRotated[emptyHead] + "\n" + "right:" + totalAngleRotated[emptyRight] + "\n" + "left:" + totalAngleRotated[emptyLeft];
        File.WriteAllText(filePath, logs);
    }
    public void restart()
    {
        //首先记录上一个物体的数据
        logs += "Moved Distance: \n" + "head: " + totalDistanceMoved[head] + "\n" + "right:" + totalDistanceMoved[rightHand] + "\n" + "left:" + totalDistanceMoved[leftHand] + "\n" +
                 "Rotated Angle: \n" + "head: " + totalAngleRotated[head] + "\n" + "right:" + totalAngleRotated[rightHand] + "\n" + "left:" + totalAngleRotated[leftHand]+"\n";
        //记录总数据
        totalDistanceMoved[emptyHead]  += totalDistanceMoved[head];
        totalDistanceMoved[emptyLeft] += totalDistanceMoved[leftHand];
        totalDistanceMoved[emptyRight] += totalDistanceMoved[rightHand];

        totalAngleRotated[emptyHead] += totalAngleRotated[head];
        totalAngleRotated[emptyLeft] += totalAngleRotated[leftHand];
        totalAngleRotated[emptyRight] += totalAngleRotated[rightHand];
        
        //清空数据
        totalAngleRotated[head] = 0f;
        totalAngleRotated[leftHand] = 0f;
        totalAngleRotated[rightHand] = 0f;

        totalDistanceMoved[head] = 0f;
        totalDistanceMoved[leftHand] = 0f;
        totalDistanceMoved[rightHand] = 0f;


    }
    // Update is called once per frame
    void Update()
    {
        RecordMovementAndRotation(head);
        RecordMovementAndRotation(leftHand);
        RecordMovementAndRotation(rightHand);
    }

    void RecordMovementAndRotation(GameObject obj)
    {
        float distanceMoved = Vector3.Distance(obj.transform.position, lastPosition[obj]);
        totalDistanceMoved[obj] += distanceMoved;
        lastPosition[obj] = obj.transform.position;

        float angleRotated = Quaternion.Angle(obj.transform.rotation, lastRotation[obj]);
        totalAngleRotated[obj] += angleRotated;
        lastRotation[obj] = obj.transform.rotation;
    }

}
