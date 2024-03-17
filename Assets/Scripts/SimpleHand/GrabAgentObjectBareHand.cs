using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GrabAgentObjectBareHand : MonoBehaviour
{
    public GameObject singleSelect;
    public GameObject targets;
    public GameObject rightThumb;
    public GameObject rightIndex;

    public GameObject leftThumb;
    public GameObject leftIndex;
    public GameObject pinchObject;
    public GameObject eyeTrackingManager;
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
    public Dictionary<GameObject, GameObject> TargetObjects = new Dictionary<GameObject, GameObject>();
    public Dictionary<GameObject, int> MovingObjectStatus = new Dictionary<GameObject, int>();

    bool initFlag = false;

    void Start()
    {
        // originalParent = transform.parent.gameObject;
        originalPosition = transform.localPosition;
        TargetObjects = new Dictionary<GameObject, GameObject>();
        FindChild(targets);
    }
    public void AddOutline(GameObject target, Color color)
    {
        if (target.GetComponent<Outline>() == null)
        {
            target.AddComponent<Outline>();
        }
        target.GetComponent<Outline>().OutlineColor = color;
    }
    void FindChild(GameObject child)
    {

        //利用for循环 获取物体下的全部子物体
        for (int c = 0; c < child.transform.childCount; c++)
        {
            TargetObjects[child.transform.GetChild(c).gameObject] = GameObject.Find(child.transform.GetChild(c).gameObject.name + " (1)");;
        }
    }
    void Update()
    {
        if(MovingObject.Count>0)
        if(TargetObjects.ContainsKey(MovingObject[0]))
        if((MovingObject[0].transform.position- TargetObjects[MovingObject[0]].transform.position).magnitude < 0.01)
        //距离目标位置小于阈值
        {
            singleSelect.GetComponent<singleSelect>().finishOneObject();
            
            MovingObject[0].transform.position = TargetObjects[MovingObject[0]].transform.position;
            MovingObject[0].transform.rotation = TargetObjects[MovingObject[0]].transform.rotation;
            AddOutline(MovingObject[0],Color.green);
            eyeTrackingManager.GetComponent<EyeTrackingManagerBareHand>().mark = 0;
            eyeTrackingManager.GetComponent<EyeTrackingManagerBareHand>().rayVisualizer.GetComponent<RayVisualizer>().setLine(0.01f);
                    MovingObject.RemoveAt(0);
        }
        grabStatus = pinchObject.GetComponent<pinch>().agentMovingStatus; 
      
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