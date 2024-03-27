using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GrabAgentObjectBareHand : MonoBehaviour
{
    public GameObject recorder;
    public List<GameObject> manipulateObjects;//放所有需要操纵的待选物体
    public GameObject targets;//��������壬��������Ҫ�����д���Ŀ��λ�õ�����
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
    private float movingScale;
    private int  allNumber = 0;
    private int  finishNumber = 0;

    public List<GameObject> MovingObject = new List<GameObject>();
    // private GameObject originalParent;


    private Dictionary<GameObject, GameObject> TargetObjects = new Dictionary<GameObject, GameObject>();

    void Start()
    {
        originalPosition = transform.localPosition;
        TargetObjects = new Dictionary<GameObject, GameObject>();
        FindChild(targets);
        allNumber = TargetObjects.Count;
        MovingObject = new List<GameObject>();
        if(recorder.GetComponent<singleSelect>().sampleType == 1){
            MovingObject.Add(manipulateObjects[0]);
            manipulateObjects.RemoveAt(0);
            allNumber = manipulateObjects.Count;
        }
    }
    void Update()
    {
        if(finishNumber == allNumber && finishNumber != 0){
            recorder.GetComponent<singleSelect>().finishAll(); 
        }
        if(MovingObject.Count>0)
            if (TargetObjects.ContainsKey(MovingObject[0])) {
                
                var obj = MovingObject[0];
                var targetPosition = TargetObjects[obj].transform.position;
                if (Vector3.Distance(obj.transform.position, targetPosition) < (obj.transform.GetComponent<Renderer>().bounds.size.x + obj.transform.GetComponent<Renderer>().bounds.size.y + obj.transform.GetComponent<Renderer>().bounds.size.z) / 3f)
                {
                    recorder.GetComponent<singleSelect>().finishCoarseOneObject();
                    AddOutline(MovingObject[0], Color.yellow);
                    obj.GetComponent<Outline>().OutlineWidth = 4f;
                }
                if (Vector3.Distance(obj.transform.position, targetPosition) < (obj.transform.GetComponent<Renderer>().bounds.size.x + obj.transform.GetComponent<Renderer>().bounds.size.y + obj.transform.GetComponent<Renderer>().bounds.size.z) / 9f &&
                    RotationGap(obj, TargetObjects[MovingObject[0]]) < 30f){

                    AddOutline(MovingObject[0], Color.red);
                    obj.GetComponent<Outline>().OutlineWidth = 6f;
                    finishNumber += 1;
                    recorder.GetComponent<singleSelect>().finishOneObject();
            
                    MovingObject[0].transform.position = TargetObjects[MovingObject[0]].transform.position;
                    MovingObject[0].transform.rotation = TargetObjects[MovingObject[0]].transform.rotation;
                    
                    eyeTrackingManager.GetComponent<EyeTrackingManagerBareHand>().mark = 0;
                    eyeTrackingManager.GetComponent<EyeTrackingManagerBareHand>().rayVisualizer.GetComponent<RayVisualizer>().setLine(0.01f);
                    MovingObject.RemoveAt(0);
                    
                    //MovingObject.Add(manipulateObjects[0]);
                    //manipulateObjects.RemoveAt(0);

                }
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
    float RotationGap(GameObject obj1, GameObject obj2)
    {
        return Mathf.Min(Mathf.Abs(obj1.transform.eulerAngles.x - obj2.transform.eulerAngles.x), 360 - Mathf.Abs(obj1.transform.eulerAngles.x - obj2.transform.eulerAngles.x)) +
                Mathf.Min(Mathf.Abs(obj1.transform.eulerAngles.y - obj2.transform.eulerAngles.y), 360 - Mathf.Abs(obj1.transform.eulerAngles.y - obj2.transform.eulerAngles.y)) +
                Mathf.Min(Mathf.Abs(obj1.transform.eulerAngles.z - obj2.transform.eulerAngles.z), 360 - Mathf.Abs(obj1.transform.eulerAngles.z - obj2.transform.eulerAngles.z));
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
        //����forѭ�� ��ȡ�����µ�ȫ��������
        for (int c = 0; c < child.transform.childCount; c++)
        {
            TargetObjects[child.transform.GetChild(c).gameObject] = GameObject.Find(child.transform.GetChild(c).gameObject.name + " (1)"); 

        }
    }
}