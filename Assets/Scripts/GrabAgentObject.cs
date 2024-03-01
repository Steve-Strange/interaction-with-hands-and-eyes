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
    private bool grabStatus;
    private bool movingStatus;
    private Vector3 originalPosition;
    private Vector3 lastPosition;

    public TMP_InputField log;

    public List<GameObject> MovingObject = new List<GameObject>();
    private GameObject originalParent;
    private float movingScale;
    private GameObject ConnectorManager;
    public bool AutoAdjustStatus = false;


    public List<GameObject> FinishedObjects = new List<GameObject>();
    public Dictionary<GameObject, GameObject> TargetObjects = new Dictionary<GameObject, GameObject>();

    bool initFlag = false;

    void Start()
    {
        originalParent = transform.parent.gameObject;
        originalPosition = transform.localPosition;
        ConnectorManager = GameObject.Find("ConnectorManager");
        TargetObjects = new Dictionary<GameObject, GameObject>();
    }

    void Update()
    {
        
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
        pinchStatus = Vector3.Distance(rightIndex.transform.position, rightThumb.transform.position) < 0.02f;

        if (pinchStatus && grabStatus && !movingStatus)
        {
            transform.position = rightIndex.transform.position;
            transform.parent = rightIndex.transform;
            lastPosition = rightIndex.transform.position;
            movingStatus = true;
        }

        if ((!pinchStatus || !grabStatus) && movingStatus)
        {
            movingStatus = false;
            transform.parent = originalParent.transform;
            transform.localPosition = originalPosition;
        }

        
        log.text = Vector3.Distance(leftIndex.transform.position, leftThumb.transform.position).ToString();
        log.text += "\n" + AutoAdjustStatus.ToString();
        log.text += "\n pinchStatus: " + pinchStatus + "\n" + "grabStatus: " + grabStatus + "\n" + "movingStatus: " + movingStatus;
        log.text += "\n" + TargetObjects.Count.ToString();
        log.text += "\n" + TargetObjects[MovingObject[0]].name.ToString() + " " + MovingObject[0].name.ToString() + " " + Vector3.Distance(MovingObject[0].transform.position, TargetObjects[MovingObject[0]].transform.position).ToString();

        if (movingStatus)
        {
            log.text += "\n" + "Moving...";

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

            // Update the original position
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

            if (MovingObject.Count > 0)
            {
                if (Vector3.Distance(MovingObject[0].transform.position, TargetObjects[MovingObject[0]].transform.position) < 0.1f)
                {
                    MovingObject[0].transform.position = TargetObjects[MovingObject[0]].transform.position;
                    FinishedObjects.Add(MovingObject[0]);
                    MovingObject.RemoveAt(0);
                    AutoAdjustStatus = true;
                }
            }
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == pinchObject)
        {
            grabStatus = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == pinchObject)
        {
            grabStatus = false;
        }
    }
}
