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
    private bool movingStatus;
    private Vector3 originalPosition;
    private Vector3 lastPosition;

    // public TMP_InputField log;

    public List<GameObject> MovingObject = new List<GameObject>();
    // private GameObject originalParent;
    private float movingScale;
    private GameObject ConnectorManager;
    public bool AutoAdjustStatus = false;

    public List<GameObject> FinishedObjects = new List<GameObject>();
    public Dictionary<GameObject, GameObject> TargetObjects = new Dictionary<GameObject, GameObject>();

    bool initFlag = false;

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
      
        movingScale = Vector3.Distance(leftIndex.transform.position, leftThumb.transform.position) * 100;
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
        // log.text = "rotation gap: " + Vector3.Distance(MovingObject[0].transform.eulerAngles, TargetObjects[MovingObject[0]].transform.eulerAngles) + "\n" + "position gap: " + Vector3.Distance(MovingObject[0].transform.position, TargetObjects[MovingObject[0]].transform.position); 

        // log.text = "pinchStatus: " + pinchStatus + "\n" + "grabStatus: " + grabStatus + "\n" + "movingStatus: " + movingStatus;

        if (movingStatus)
        {
            // log.text += "\n" + "Moving...";
            if(grabStatus == 1){
                transform.position = rightIndex.transform.position;
                transform.parent = rightIndex.transform;
                foreach (var obj in ConnectorManager.GetComponent<ConnectorManager>().Objects)
                    if (!ConnectorManager.GetComponent<ConnectorManager>().emptyObjects.Contains(obj))
                    {
                        if (obj != MovingObject[0])
                        {
                            TargetObjects[obj].SetActive(false);
                            obj.SetActive(false);
                        }
                    }

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

            if (MovingObject.Count > 0 && FinishedObjects.Count < 3)
            {
                if (Vector3.Distance(MovingObject[0].transform.position, TargetObjects[MovingObject[0]].transform.position) < 0.1f &&
                Vector3.Distance(MovingObject[0].transform.eulerAngles, TargetObjects[MovingObject[0]].transform.eulerAngles) < 90f)
                {
                    MovingObject[0].transform.position = TargetObjects[MovingObject[0]].transform.position;
                    foreach (var obj in pinchObject.GetComponent<collide>().onFrame)
                    {
                        obj.transform.eulerAngles = TargetObjects[MovingObject[0]].transform.eulerAngles;
                         if (Vector3.Distance(obj.transform.position, TargetObjects[obj].transform.position) < 0.1f &&
                        Vector3.Distance(obj.transform.eulerAngles, TargetObjects[obj].transform.eulerAngles) < 90f)
                        {
                            obj.transform.position = TargetObjects[obj].transform.position;
                            obj.transform.eulerAngles = TargetObjects[obj].transform.eulerAngles;
                        }
                    }
                    FinishedObjects.Add(MovingObject[0]);
                    MovingObject.RemoveAt(0);
                    AutoAdjustStatus = true;
                }

            }
        }

    }

}
