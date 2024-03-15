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

    public TMP_InputField log;

    public List<GameObject> MovingObject = new List<GameObject>();
    // private GameObject originalParent;
    private float movingScale;
    private GameObject ConnectorManager;
    public bool AutoAdjustStatus = false;

    public List<GameObject> FinishedObjects = new List<GameObject>();
    public Dictionary<GameObject, GameObject> TargetObjects = new Dictionary<GameObject, GameObject>();
    public Dictionary<GameObject, int> MovingObjectStatus = new Dictionary<GameObject, int>();

    bool initFlag = false;

    public float coarseMovingTime;
    public float accurateMovingTime;

    void Start()
    {
        // originalParent = transform.parent.gameObject;
        originalPosition = transform.localPosition;
        ConnectorManager = GameObject.Find("ConnectorManager");
        TargetObjects = new Dictionary<GameObject, GameObject>();
    }

    void Update()
    {
        grabStatus = pinchObject.GetComponent<pinch>().agentMovingStatus;
        if (!initFlag)
        {
            
            foreach (var obj in ConnectorManager.GetComponent<ConnectorManager>().Objects)
            {
               
                if (!ConnectorManager.GetComponent<ConnectorManager>().emptyObjects.Contains(obj))
                {
                  
                    TargetObjects[obj] = GameObject.Find(obj.name + " (1)");
                  
                    Debug.Log(TargetObjects[obj].name);
                
                    initFlag = true;
                }

            }
        }
      
        movingScale = Mathf.Pow(Vector3.Distance(leftIndex.transform.position, leftThumb.transform.position), 1.5f) * 1000;
        pinchStatus = Vector3.Distance(rightIndex.transform.position, rightThumb.transform.position) < 0.014f;

        if (pinchStatus && grabStatus!=0 && !movingStatus)
        {
            lastPosition = rightIndex.transform.position;
            movingStatus = true;
        }

        if ((!pinchStatus || grabStatus==0) && movingStatus)
        {
            movingStatus = false;
            transform.parent = null;
            transform.localPosition = originalPosition;
            if(MovingObject.Count > 0) transform.rotation = MovingObject[0].transform.rotation;
            else transform.rotation = Quaternion.identity;
            
        }
        log.text = "rotation gap: " + rotationGap(MovingObject[0], TargetObjects[MovingObject[0]]) + "\n" + "position gap: " + Vector3.Distance(MovingObject[0].transform.position, TargetObjects[MovingObject[0]].transform.position) + "\n"; 
        log.text += "MovingObject: " + MovingObject[0].name + "\n" + "TargetObjects: " + TargetObjects[MovingObject[0]].name + "\n" + "FinishedObjects: " + FinishedObjects.Count + "\n";
        log.text += "onFrame.Count:" + pinchObject.GetComponent<collide>().onFrame.Count + "\n"; 

        // log.text += "pinchStatus: " + pinchStatus + "\n" + "grabStatus: " + grabStatus + "\n" + "movingStatus: " + movingStatus;

        if (movingStatus)
        {
            // MovingObject[0].GetComponent<Outline>().OutlineColor = Color.clear;  ???
            foreach (var obj in ConnectorManager.GetComponent<ConnectorManager>().Objects)
                if (!ConnectorManager.GetComponent<ConnectorManager>().emptyObjects.Contains(obj))
                {
                    if (obj != MovingObject[0])
                    {
                        TargetObjects[obj].SetActive(false);
                        obj.SetActive(false);
                    }
                }
            // log.text += "\n" + "Moving...";
            if(grabStatus == 1){
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

        }
        else
        {
            foreach (var obj in ConnectorManager.GetComponent<ConnectorManager>().Objects)
                if (!ConnectorManager.GetComponent<ConnectorManager>().emptyObjects.Contains(obj))
                {
                    TargetObjects[obj].SetActive(true);
                    obj.SetActive(true);
                }
            if(MovingObjectStatus[MovingObject[0]] == 2){
                MovingObject[0].transform.position = TargetObjects[MovingObject[0]].transform.position;
                if(!FinishedObjects.Contains(MovingObject[0])) FinishedObjects.Add(MovingObject[0]);
                if(FinishedObjects.Count == pinchObject.GetComponent<collide>().onFrame.Count){
                    log.text = "coarseMovingTime: " + coarseMovingTime + "\n" + "accurateMovingTime: " + accurateMovingTime + "\n";
                }
                MovingObject.RemoveAt(0);
                AutoAdjustStatus = true;
            }
        }

        if (MovingObject.Count > 0)
        {
            if(MovingObjectStatus[MovingObject[0]]==0) coarseMovingTime += Time.deltaTime;
            else if(MovingObjectStatus[MovingObject[0]]==1) accurateMovingTime += Time.deltaTime;

            if (Vector3.Distance(MovingObject[0].transform.position, TargetObjects[MovingObject[0]].transform.position) < 
                (MovingObject[0].transform.GetComponent<Renderer>().bounds.size.x + MovingObject[0].transform.GetComponent<Renderer>().bounds.size.y + MovingObject[0].transform.GetComponent<Renderer>().bounds.size.z) / 3f
                && MovingObjectStatus[MovingObject[0]] == 0){
                    MovingObjectStatus[MovingObject[0]] = 1;
                    MovingObject[0].GetComponent<Outline>().OutlineColor = Color.yellow;
                    MovingObject[0].GetComponent<Outline>().OutlineWidth = 4f;
            }
            if (Vector3.Distance(MovingObject[0].transform.position, TargetObjects[MovingObject[0]].transform.position) < 
                (MovingObject[0].transform.GetComponent<Renderer>().bounds.size.x + MovingObject[0].transform.GetComponent<Renderer>().bounds.size.y + MovingObject[0].transform.GetComponent<Renderer>().bounds.size.z) / 9f &&
                rotationGap(MovingObject[0], TargetObjects[MovingObject[0]]) < 30f){
                    MovingObjectStatus[MovingObject[0]] = 2;
                    MovingObject[0].GetComponent<Outline>().OutlineColor = Color.red;
                    MovingObject[0].GetComponent<Outline>().OutlineWidth = 6f;

            }
        }

    }

    float rotationGap(GameObject obj1, GameObject obj2)
    {
        return Mathf.Min(Mathf.Abs(obj1.transform.eulerAngles.x - obj2.transform.eulerAngles.x), 360 - Mathf.Abs(obj1.transform.eulerAngles.x - obj2.transform.eulerAngles.x)) + 
                Mathf.Min(Mathf.Abs(obj1.transform.eulerAngles.y - obj2.transform.eulerAngles.y), 360 - Mathf.Abs(obj1.transform.eulerAngles.y - obj2.transform.eulerAngles.y)) +
                Mathf.Min(Mathf.Abs(obj1.transform.eulerAngles.z - obj2.transform.eulerAngles.z), 360 - Mathf.Abs(obj1.transform.eulerAngles.z - obj2.transform.eulerAngles.z));
    }


}
