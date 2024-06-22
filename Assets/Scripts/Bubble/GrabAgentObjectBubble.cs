using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GrabAgentObjectBubble : MonoBehaviour
{
    public GameObject recorder;
    private List<GameObject> manipulateObjects = new List<GameObject>();//放所有需要操纵的待选物体，为了仅移动的方法
    public GameObject manipulates;//��������壬��������Ҫ�����д���Ŀ��λ�õ�����
    public GameObject rightThumb;
    public GameObject rightIndex;
    public GameObject leftThumb;
    public GameObject leftIndex;
    public GameObject pinchObject;
    public GameObject bubble;
    public bool pinchStatus;
    public bool coarse = true;
    private int grabStatus;
    private bool movingStatus;
    private Vector3 originalPosition;
    private Vector3 lastPosition;
    private float movingScale;
    private int  allNumber = 0;
    private int  finishNumber = 0;
    private bool First;
    public List<GameObject> MovingObject = new List<GameObject>();
    // private GameObject originalParent;


    private Dictionary<GameObject, GameObject> TargetObjects = new Dictionary<GameObject, GameObject>();

    void Start()
    {   
        originalPosition = transform.localPosition;
        First = true;
        TargetObjects = new Dictionary<GameObject, GameObject>();
        FindChild(manipulates);
        allNumber = TargetObjects.Count;
        if(recorder.GetComponent<singleSelect>().sampleType == 0)//仅选择
        {
            gameObject.SetActive(false);
        }else if(recorder.GetComponent<singleSelect>().sampleType == 1)//仅操纵
        {

            for (int c = 0; c < manipulates.transform.childCount; c++)
            {
                manipulateObjects.Add(manipulates.transform.GetChild(c).gameObject);
            }
            MovingObject.Add(manipulateObjects[0]);
            Debug.Log(MovingObject[0].gameObject.name);
            manipulateObjects.RemoveAt(0);
            allNumber = manipulateObjects.Count;
            MovingObject[0].transform.position = new Vector3(0f, -1f, 0f);
        }
        else if(recorder.GetComponent<singleSelect>().sampleType == 2)
        {
            gameObject.SetActive(false);
        }

    }
    int MovingStatus = 0;
    float stayInTimer = 0;
    void Update()
    {
        if(finishNumber == allNumber && finishNumber != 0 && First == true){
            recorder.GetComponent<singleSelect>().finishAll();
            First = false;
        }

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
            transform.rotation = Quaternion.identity;
        }

        if (movingStatus)
        {
          
            if (grabStatus == 1)
            {
                transform.position = rightIndex.transform.position;
                transform.parent = rightIndex.transform;
                // 计算手指移动向量
                Vector3 deltaPosition = rightIndex.transform.position - lastPosition;
                // 使用相对坐标来同步位置
                MovingObject[0].transform.position += deltaPosition * movingScale;
            }
            else
            {
                float deltaRotation = Vector3.Angle(lastPosition - transform.position, rightIndex.transform.position - transform.position);
                Vector3 crossProduct = Vector3.Cross(lastPosition - transform.position, rightIndex.transform.position - transform.position);
                Vector3 rotationAxis = Vector3.zero;
                if (grabStatus == 2)
                {
                    rotationAxis = Vector3.right;
                }
                else if (grabStatus == 3)
                {
                    rotationAxis = Vector3.up;
                }
                else if (grabStatus == 4)
                {
                    rotationAxis = Vector3.forward;
                }
                float dotProduct = Vector3.Dot(crossProduct, rotationAxis);
                if (dotProduct < 0) deltaRotation = -deltaRotation;
                // 旋转代理物体
                transform.RotateAround(transform.position, rotationAxis, deltaRotation);
                // 记录这次旋转的角度
                Quaternion deltaRotationQuat = Quaternion.AngleAxis(deltaRotation, rotationAxis);
                // 操纵的物体旋转
                MovingObject[0].transform.rotation = deltaRotationQuat * MovingObject[0].transform.rotation;
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
        else//停止的时候才判断
        {
            if (Vector3.Distance(MovingObject[0].transform.position, TargetObjects[MovingObject[0]].transform.position) < (MovingObject[0].transform.GetComponent<Renderer>().bounds.size.x + MovingObject[0].transform.GetComponent<Renderer>().bounds.size.y + MovingObject[0].transform.GetComponent<Renderer>().bounds.size.z) / 9f &&
                    RotationGap(MovingObject[0], TargetObjects[MovingObject[0]]) < 30f){//此时判定为完成精选操作
                MovingStatus = 2;
                if(recorder.GetComponent<singleSelect>().sampleType == 2) { //select+manipulate
                    AddOutline(MovingObject[0], Color.red);
                    MovingObject[0].GetComponent<Outline>().OutlineWidth = 6f;
                    finishNumber += 1;
                    recorder.GetComponent<singleSelect>().finishOneObject();
                    MovingObject[0].transform.position = TargetObjects[MovingObject[0]].transform.position;
                    MovingObject[0].transform.rotation = TargetObjects[MovingObject[0]].transform.rotation;
                    bubble.SetActive(true);
                    bubble.GetComponent<Bubble>().awakeTheBubble();
                    bubble.GetComponent<Bubble>().selectingObject = false;
                    MovingObject.RemoveAt(0);
                }else if (recorder.GetComponent<singleSelect>().sampleType == 1)//manipulateOnly
                {
                    AddOutline(MovingObject[0], Color.red);
                    MovingObject[0].GetComponent<Outline>().OutlineWidth = 5f;
                    finishNumber += 1;
                    recorder.GetComponent<singleSelect>().finishOneObject();
                    MovingObject[0].transform.position = TargetObjects[MovingObject[0]].transform.position;
                    MovingObject[0].transform.rotation = TargetObjects[MovingObject[0]].transform.rotation;
                
                    MovingObject.RemoveAt(0);
                    if(manipulateObjects.Count > 0) { 
                        MovingObject.Add(manipulateObjects[0]);
                        MovingObject[0].transform.position = new Vector3(0f, -1f, 0f);
                        AddOutline(MovingObject[0], Color.green);//当前操纵的这个物体泛绿光
                        MovingObject[0].transform.position = new Vector3(0, 0, 8);
                        MovingObject[0].GetComponent<Outline>().OutlineWidth = 5f;
                        manipulateObjects.RemoveAt(0);
                    }
                    
                }
            }else if (Vector3.Distance(MovingObject[0].transform.position, TargetObjects[MovingObject[0]].transform.position) < (MovingObject[0].transform.GetComponent<Renderer>().bounds.size.x + MovingObject[0].transform.GetComponent<Renderer>().bounds.size.y + MovingObject[0].transform.GetComponent<Renderer>().bounds.size.z) / 3f)
            {
                MovingStatus = 1;
                if(coarse == false)
                {
                    recorder.GetComponent<singleSelect>().finishCoarseOneObject();
                    coarse = true;
                }
                
                AddOutline(MovingObject[0], Color.yellow);
                MovingObject[0].GetComponent<Outline>().OutlineWidth = 4f;
            }else{
                MovingStatus = 0;
                AddOutline(MovingObject[0], Color.white);
                MovingObject[0].GetComponent<Outline>().OutlineWidth = 4f;
            }
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
        for (int c = 0; c < child.transform.childCount; c++)
        {
            TargetObjects[child.transform.GetChild(c).gameObject] = GameObject.Find(child.transform.GetChild(c).gameObject.name + " (1)");
        }
    }
}