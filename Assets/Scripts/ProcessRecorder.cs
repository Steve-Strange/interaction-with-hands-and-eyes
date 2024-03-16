using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProcessRecorder : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject agentObject;

    public List<GameObject> CompleteObjects = new List<GameObject>();
    public Dictionary<GameObject, GameObject> TargetObjects;
    public Dictionary<GameObject, int> MovingObjectStatus = new Dictionary<GameObject, int>();

    public List<GameObject> MovingObject;
    public TMP_InputField log;
    public GameObject pinchObject;
    public float accurateMovingTime;
    private GameObject HandPoseManager;

    void Start()
    {
        MovingObject = agentObject.GetComponent<GrabAgentObject>().MovingObject;
        TargetObjects = agentObject.GetComponent<GrabAgentObject>().TargetObjects;
        HandPoseManager = GameObject.Find("HandPoseManager");
    }

    // Update is called once per frame
    void Update()
    {
        // if(CompleteObjects.Count == pinchObject.GetComponent<collide>().onFrame.Count){
        //     log.text = "SelectionTime: " + HandPoseManager.GetComponent<HandPoseManager>().selectionTime + "\n" + 
        //     "coarseMovingTime: " + HandPoseManager.GetComponent<HandPoseManager>().coarseMovingTime + "\n" + 
        //     "accurateMovingTime: " + accurateMovingTime + "\n";
        //     log.text += "Finished!!!!!!!!!!!!";
        // }
        // else{
        //     log.text = "CompleteObjects: " + CompleteObjects.Count + "\n";
        //     log.text += "TargetObjects: " + TargetObjects.Count + "\n";
        //     log.text += "onFrame: " + pinchObject.GetComponent<collide>().onFrame.Count + "\n";
        // }

        if (MovingObject.Count > 0 && MovingObjectStatus[MovingObject[0]]==1 || MovingObjectStatus[MovingObject[0]]==2)
        {
            accurateMovingTime += Time.deltaTime;
        }

        foreach (var obj in pinchObject.GetComponent<collide>().onFrame){
            log.text += "distance: " + Vector3.Distance(obj.transform.position, TargetObjects[obj].transform.position) + "rotation: " + RotationGap(obj, TargetObjects[obj]) + "\n";
            if(!CompleteObjects.Contains(obj)){
                if (Vector3.Distance(obj.transform.position, TargetObjects[obj].transform.position) < 
                    (obj.transform.GetComponent<Renderer>().bounds.size.x + obj.transform.GetComponent<Renderer>().bounds.size.y + obj.transform.GetComponent<Renderer>().bounds.size.z) / 3f
                    && MovingObjectStatus[obj] == 0){
                        MovingObjectStatus[obj] = 1;
                        obj.GetComponent<Outline>().OutlineColor = Color.yellow;
                        obj.GetComponent<Outline>().OutlineWidth = 4f;
                }
                if (Vector3.Distance(obj.transform.position, TargetObjects[obj].transform.position) < 
                    (obj.transform.GetComponent<Renderer>().bounds.size.x + obj.transform.GetComponent<Renderer>().bounds.size.y + obj.transform.GetComponent<Renderer>().bounds.size.z) / 9f &&
                    RotationGap(obj, TargetObjects[obj]) < 30f){
                        MovingObjectStatus[obj] = 2;
                        obj.GetComponent<Outline>().OutlineColor = Color.red;
                        obj.GetComponent<Outline>().OutlineWidth = 6f;
                }
            }

            if(MovingObjectStatus[obj]==2 && !agentObject.GetComponent<GrabAgentObject>().movingStatus){
                if(!CompleteObjects.Contains(obj)) CompleteObjects.Add(obj);
                obj.transform.position = TargetObjects[obj].transform.position;
                obj.transform.rotation = TargetObjects[obj].transform.rotation;
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
