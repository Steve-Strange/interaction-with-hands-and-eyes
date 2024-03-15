using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GrabAgentObjectBareHand : MonoBehaviour
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

    public TMP_InputField log;

    public List<GameObject> MovingObject = new List<GameObject>();
    // private GameObject originalParent;
    private float movingScale;


    public List<GameObject> FinishedObjects = new List<GameObject>();
    // public Dictionary<GameObject, GameObject> TargetObjects = new Dictionary<GameObject, GameObject>();
    public Dictionary<GameObject, int> MovingObjectStatus = new Dictionary<GameObject, int>();

    bool initFlag = false;

    void Start()
    {
        // originalParent = transform.parent.gameObject;
        originalPosition = transform.localPosition;
        // TargetObjects = new Dictionary<GameObject, GameObject>();
    }

    void Update()
    {
        grabStatus = pinchObject.GetComponent<pinch>().agentMovingStatus;
        // if (!initFlag)//初始化
        // {
       
        //     foreach (var obj in ConnectorManager1.GetComponent<ConnectorManager1>().Objects)
        //     {
               
        //         if (!ConnectorManager1.GetComponent<ConnectorManager1>().emptyObjects.Contains(obj))
        //         {
                  
        //             TargetObjects[obj] = GameObject.Find(obj.name + " (1)");
                  
        //             Debug.Log(TargetObjects[obj].name);
                
        //             initFlag = true;
        //         }

        //     }//改成直接在grab里一一对应
        // }
      
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
        // log.text = "rotation gap: " + rotationGap(MovingObject[0], TargetObjects[MovingObject[0]]) + "\n" + "position gap: " + Vector3.Distance(MovingObject[0].transform.position, TargetObjects[MovingObject[0]].transform.position) + "\n"; 
        // log.text += "MovingObject: " + MovingObject[0].name + "\n" + "TargetObjects: " + TargetObjects[MovingObject[0]].name + "\n" + "FinishedObjects: " + FinishedObjects.Count + "\n";
        // log.text += "onFrame.Count:" + pinchObject.GetComponent<collide>().onFrame.Count + "\n"; 

        if (movingStatus)
        {
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
    }

}