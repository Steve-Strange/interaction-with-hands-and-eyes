using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GrabAgentObject : MonoBehaviour
{
    public GameObject rightThumb;
    public GameObject rightIndex;

    public GameObject leftThumb;
    public GameObject leftIndex;
    public GameObject pinchObject;

    public bool pinchStatus;
    private int grabStatus;
    public bool movingStatus;
    private Vector3 originalPosition;
    private Vector3 lastPosition;

    // public TMP_InputField log;

    public List<GameObject> MovingObject = new List<GameObject>();
    // private GameObject originalParent;
    private float movingScale;
    public bool AutoAdjustStatus = false;

    public List<GameObject> FinishedObjects = new List<GameObject>();
    public List<Vector3> FinishedPositions = new List<Vector3>();
    public Dictionary<GameObject, GameObject> TargetObjects = new Dictionary<GameObject, GameObject>();
    public bool initFlag = false;
    public GameObject PointStructure;
    public GameObject TimeRecorder;
    public string ManipulateData;

    void Start()
    {
        // originalParent = transform.parent.gameObject;
        originalPosition = transform.localPosition;
        PointStructure = GameObject.Find("PointStructure");
    }

    void Update()
    {
        // log.text = "TargetObjects: " + TargetObjects.Count + "\n" + "MovingObjects: " + MovingObject.Count + "\n" + "FinishedObjects: " + FinishedObjects.Count + "\n";
        // log.text += "Distance: " + Vector3.Distance(MovingObject[0].transform.position, TargetObjects[MovingObject[0]].transform.position) + "\n";
        if (!initFlag)
        {
            FinishedObjects.Clear();
        }
        
        if (MovingObject.Count > 0)
        {
            if (MovingObject.Count > 0)
                MovingObject[0].GetComponent<Outline>().outlineColor = Color.green;
            grabStatus = pinchObject.GetComponent<pinch>().agentMovingStatus;

            movingScale = Mathf.Pow(Vector3.Distance(leftIndex.transform.position, leftThumb.transform.position), 1.5f) * 1000;
            pinchStatus = Vector3.Distance(rightIndex.transform.position, rightThumb.transform.position) < 0.014f;

            if (pinchStatus && grabStatus != 0 && !movingStatus)
            {
                lastPosition = rightIndex.transform.position;
                movingStatus = true;
            }

            if ((!pinchStatus || grabStatus == 0) && movingStatus)
            {
                movingStatus = false;
                transform.parent = null;
                transform.localPosition = originalPosition;
                if (MovingObject.Count > 0) transform.rotation = MovingObject[0].transform.rotation;
                else transform.rotation = Quaternion.identity;

            }

            if (movingStatus)
            {
                // log.text += "\n" + "Moving...";
                if (grabStatus == 1)
                {
                    transform.position = rightIndex.transform.position;
                    transform.parent = rightIndex.transform;

                    // Calculate finger movement vector
                    Vector3 deltaPosition = rightIndex.transform.position - lastPosition;

                    // Use relative coordinates to synchronize position
                    MovingObject[0].transform.position += deltaPosition * movingScale;

                }
                else
                {
                    float deltaRotation = Vector3.Angle(lastPosition - transform.position, rightIndex.transform.position - transform.position);
                    Vector3 crossProduct = Vector3.Cross(lastPosition - transform.position, rightIndex.transform.position - transform.position);

                    if (grabStatus == 2)
                    {
                        float dotProduct = Vector3.Dot(crossProduct, Vector3.right);
                        if (dotProduct < 0) deltaRotation = -deltaRotation;
                        transform.RotateAround(transform.position, Vector3.right, deltaRotation);
                    }
                    else if (grabStatus == 3)
                    {
                        float dotProduct = Vector3.Dot(crossProduct, Vector3.up);
                        if (dotProduct < 0) deltaRotation = -deltaRotation;
                        transform.RotateAround(transform.position, Vector3.up, deltaRotation);
                    }
                    else if (grabStatus == 4)
                    {
                        float dotProduct = Vector3.Dot(crossProduct, Vector3.forward);
                        if (dotProduct < 0) deltaRotation = -deltaRotation;
                        transform.RotateAround(transform.position, Vector3.forward, deltaRotation);
                    }

                    MovingObject[0].transform.rotation = transform.rotation;

                }

                lastPosition = rightIndex.transform.position;

                if (Vector3.Distance(MovingObject[0].transform.position, TargetObjects[MovingObject[0]].transform.position) < 
                    (MovingObject[0].transform.GetComponent<Renderer>().bounds.size.x + MovingObject[0].transform.GetComponent<Renderer>().bounds.size.y + MovingObject[0].transform.GetComponent<Renderer>().bounds.size.z) / 9f &&
                    RotationGap(MovingObject[0], TargetObjects[MovingObject[0]]) < 30f){
                        MovingObject[0].GetComponent<Outline>().OutlineColor = Color.red;
                        MovingObject[0].GetComponent<Outline>().OutlineWidth = 6f;
                }
                else if (Vector3.Distance(MovingObject[0].transform.position, TargetObjects[MovingObject[0]].transform.position) < 
                    (MovingObject[0].transform.GetComponent<Renderer>().bounds.size.x + MovingObject[0].transform.GetComponent<Renderer>().bounds.size.y + MovingObject[0].transform.GetComponent<Renderer>().bounds.size.z) / 3f){
                        MovingObject[0].GetComponent<Outline>().OutlineColor = Color.yellow;
                        MovingObject[0].GetComponent<Outline>().OutlineWidth = 4f;
                }
                else {
                    MovingObject[0].GetComponent<Outline>().OutlineColor = Color.white;
                }
            }
            else
            {
                if (Vector3.Distance(MovingObject[0].transform.position, TargetObjects[MovingObject[0]].transform.position) < 
                    (MovingObject[0].transform.GetComponent<Renderer>().bounds.size.x + MovingObject[0].transform.GetComponent<Renderer>().bounds.size.y + MovingObject[0].transform.GetComponent<Renderer>().bounds.size.z) / 9f &&
                    RotationGap(MovingObject[0], TargetObjects[MovingObject[0]]) < 30f){
                        
                        if (!FinishedObjects.Contains(MovingObject[0]))
                        FinishedObjects.Add(MovingObject[0]);
                        FinishedPositions.Add(MovingObject[0].transform.position);
                        
                        MovingObject[0].transform.position = TargetObjects[MovingObject[0]].transform.position;
                        MovingObject[0].transform.rotation = TargetObjects[MovingObject[0]].transform.rotation;
                        PointStructure.GetComponent<PointStructure>().FinishedObjects.Add(MovingObject[0]);
                        PointStructure.GetComponent<PointStructure>().FitLines(MovingObject[0]);
                        TimeRecorder.GetComponent<TimeRecorder>().CompleteObjects.Add(MovingObject[0]);
                        // ManipulateData += "FinishedObjectCount " + FinishedObjects.Count + " " +  + "\n";
                        

                        MovingObject.RemoveAt(0);
                        AutoAdjustStatus = true;
                }
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
