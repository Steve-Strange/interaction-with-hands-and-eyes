using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProcessRecorder : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject agentObject;

    public List<GameObject> CompleteObjects = new List<GameObject>();
    public Dictionary<GameObject, int> MovingObjectStatus = new Dictionary<GameObject, int>();

    public TMP_InputField log;
    public GameObject pinchObject;
    public float MovingTime;
    public float coarseMovingTime;
    private GameObject HandPoseManager;

    void Start()
    {
        HandPoseManager = GameObject.Find("HandPoseManager");
    }

    // Update is called once per frame
    void Update()
    {
        if(CompleteObjects.Count == agentObject.GetComponent<GrabAgentObject>().ObjectsOnFrame.Count){
            log.text = "SelectionTime: " + HandPoseManager.GetComponent<HandPoseManager>().selectionTime + "\n" + 
            "coarseMovingTime: " + coarseMovingTime + "\n" + 
            "MovingTime: " + MovingTime + "\n";
            log.text += "Finished!!!!!!!!!!!!";
        }
        else{
            log.text = "CompleteObjects: " + CompleteObjects.Count + "\n";
            log.text += "agentObject.GetComponent<GrabAgentObject>().TargetObjects: " + agentObject.GetComponent<GrabAgentObject>().TargetObjects.Count + "\n";
            log.text += "FrameObjects: " + agentObject.GetComponent<GrabAgentObject>().ObjectsOnFrame.Count + "\n";
        }

        foreach (var obj in agentObject.GetComponent<GrabAgentObject>().ObjectsOnFrame){
            if(!MovingObjectStatus.ContainsKey(obj)) MovingObjectStatus.Add(obj, 0);
            // log.text += "distance: " + Vector3.Distance(obj.transform.position, agentObject.GetComponent<GrabAgentObject>().TargetObjects[obj].transform.position) + "rotation: " + RotationGap(obj, agentObject.GetComponent<GrabAgentObject>().TargetObjects[obj]) + "\n";
            if(!CompleteObjects.Contains(obj)){
                if (Vector3.Distance(obj.transform.position, agentObject.GetComponent<GrabAgentObject>().TargetObjects[obj].transform.position) < 
                    (obj.transform.GetComponent<Renderer>().bounds.size.x + obj.transform.GetComponent<Renderer>().bounds.size.y + obj.transform.GetComponent<Renderer>().bounds.size.z) / 3f
                    && MovingObjectStatus[obj] == 0){
                        MovingObjectStatus[obj] = 1;
                        obj.GetComponent<Outline>().OutlineColor = Color.yellow;
                        obj.GetComponent<Outline>().OutlineWidth = 4f;
                }
                if (Vector3.Distance(obj.transform.position, agentObject.GetComponent<GrabAgentObject>().TargetObjects[obj].transform.position) < 
                    (obj.transform.GetComponent<Renderer>().bounds.size.x + obj.transform.GetComponent<Renderer>().bounds.size.y + obj.transform.GetComponent<Renderer>().bounds.size.z) / 9f &&
                    RotationGap(obj, agentObject.GetComponent<GrabAgentObject>().TargetObjects[obj]) < 30f){
                        MovingObjectStatus[obj] = 2;
                        obj.GetComponent<Outline>().OutlineColor = Color.red;
                        obj.GetComponent<Outline>().OutlineWidth = 6f;
                }
            }

            if(MovingObjectStatus[obj]==2 && !agentObject.GetComponent<GrabAgentObject>().movingStatus){
                if(!CompleteObjects.Contains(obj)) CompleteObjects.Add(obj);
                obj.transform.position = agentObject.GetComponent<GrabAgentObject>().TargetObjects[obj].transform.position;
                obj.transform.rotation = agentObject.GetComponent<GrabAgentObject>().TargetObjects[obj].transform.rotation;
            }
        }

        
        if (agentObject.GetComponent<GrabAgentObject>().MovingObject.Count > 0)
        {
            if(MovingObjectStatus[agentObject.GetComponent<GrabAgentObject>().MovingObject[0]]==0) {
                coarseMovingTime += Time.deltaTime;
            }
            MovingTime += Time.deltaTime;
            
        }
        

    }


    float RotationGap(GameObject obj1, GameObject obj2)
    {
        return Mathf.Min(Mathf.Abs(obj1.transform.eulerAngles.x - obj2.transform.eulerAngles.x), 360 - Mathf.Abs(obj1.transform.eulerAngles.x - obj2.transform.eulerAngles.x)) + 
                Mathf.Min(Mathf.Abs(obj1.transform.eulerAngles.y - obj2.transform.eulerAngles.y), 360 - Mathf.Abs(obj1.transform.eulerAngles.y - obj2.transform.eulerAngles.y)) +
                Mathf.Min(Mathf.Abs(obj1.transform.eulerAngles.z - obj2.transform.eulerAngles.z), 360 - Mathf.Abs(obj1.transform.eulerAngles.z - obj2.transform.eulerAngles.z));
    }
}
