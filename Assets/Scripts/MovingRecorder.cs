using System.Collections;
using System.IO;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.UISystemProfilerApi;

public class MovingRecorder : MonoBehaviour
{
    //会记录每次移动的数据和最后全部加在一起的总数据
    [SerializeField]
    string userName;
    public GameObject head;
    public GameObject leftHand;
    public GameObject rightHand;
    private Dictionary<GameObject, Vector3> lastPosition = new Dictionary<GameObject, Vector3>();
    private Dictionary<GameObject, Quaternion> lastRotation = new Dictionary<GameObject, Quaternion>();
    private Dictionary<GameObject, float> totalDistanceMoved = new Dictionary<GameObject, float>();
    private Dictionary<GameObject, float> totalAngleRotated = new Dictionary<GameObject, float>();
    private string filePath;
    private string logs;
    public string MovingData;
    // public TMP_InputField log;
    // Start is called before the first frame update
    void Start()
    {
        //新建一个文件来存
        logs = "";
        var folderPath = Application.persistentDataPath;
        var fileName = userName + "-" + SceneManager.GetActiveScene().name + "-" + "move_" + System.DateTime.Now.ToString("MMddHHmmss") + ".txt";
        filePath = Path.Combine(folderPath, fileName);

        lastPosition[head] = head.transform.position;
        lastPosition[leftHand] = leftHand.transform.position;
        lastPosition[rightHand] = rightHand.transform.position;
        lastRotation[head] = head.transform.rotation;
        lastRotation[leftHand] = leftHand.transform.rotation;
        lastRotation[rightHand] = rightHand.transform.rotation;
        totalDistanceMoved[head] = 0f;
        totalDistanceMoved[leftHand] = 0f;
        totalDistanceMoved[rightHand] = 0f;

        totalAngleRotated[head] = 0f;
        totalAngleRotated[leftHand] = 0f;
        totalAngleRotated[rightHand] = 0f;
    }
    public void finishAll()
    {

        MovingData = "Moved Distance head:" +totalDistanceMoved[head] + "\n" + "Moved Distance right:" + totalDistanceMoved[rightHand] + "\n" + "Moved Distance left:" + totalDistanceMoved[leftHand] + "\n" +
                 "Rotated Angle head:" + totalAngleRotated[head] + "\n" + "Rotated Angle right:" + totalAngleRotated[rightHand] + "\n" + "Rotated Angle left:" + totalAngleRotated[leftHand];
        logs +="Moved Distance head:" +totalDistanceMoved[head] + "\n" + "Moved Distance right:" + totalDistanceMoved[rightHand] + "\n" + "Moved Distance left:" + totalDistanceMoved[leftHand] + "\n" +
                 "Rotated Angle head:" + totalAngleRotated[head] + "\n" + "Rotated Angle right:" + totalAngleRotated[rightHand] + "\n" + "Rotated Angle left:" + totalAngleRotated[leftHand];
        File.WriteAllText(filePath, logs);
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
