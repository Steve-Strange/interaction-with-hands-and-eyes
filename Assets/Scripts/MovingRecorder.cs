using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MovingRecorder : MonoBehaviour
{

    public GameObject head;
    public GameObject leftHand;
    public GameObject rightHand;
    private Dictionary<GameObject, Vector3> lastPosition = new Dictionary<GameObject, Vector3>();
    private Dictionary<GameObject, Quaternion> lastRotation = new Dictionary<GameObject, Quaternion>();
    private Dictionary<GameObject, float> totalDistanceMoved = new Dictionary<GameObject, float>();
    private Dictionary<GameObject, float> totalAngleRotated = new Dictionary<GameObject, float>();
    // public TMP_InputField log;
    public string output;
    // Start is called before the first frame update
    void Start()
    {
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

    // Update is called once per frame
    void Update()
    {
        // log.text = "head: " + totalDistanceMoved[head] + "right:" + totalDistanceMoved[rightHand] + "left:" + totalDistanceMoved[leftHand] + "\n";
        // log.text += "head: " + totalAngleRotated[head] + "right:" + totalAngleRotated[rightHand] + "left:" + totalAngleRotated[leftHand];
        output = "Moved Distance: \n" + "head: " + totalDistanceMoved[head] + "\n" + "right:" + totalDistanceMoved[rightHand] + "\n" + "left:" + totalDistanceMoved[leftHand] + "\n" + 
                 "Rotated Angle: \n" + "head: " + totalAngleRotated[head] + "\n" + "right:" + totalAngleRotated[rightHand] + "\n" + "left:" + totalAngleRotated[leftHand];
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
