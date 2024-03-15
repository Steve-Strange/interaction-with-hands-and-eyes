using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProcessRecorder : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject agentObject;

    public List<GameObject> FinishedObjects;
    public Dictionary<GameObject, GameObject> TargetObjects;
    public Dictionary<GameObject, int> MovingObjectStatus = new Dictionary<GameObject, int>();

    public List<GameObject> MovingObject;
    public TMP_InputField log;
    public GameObject pinchObject;

    public float coarseMovingTime;
    public float accurateMovingTime;
    private GameObject Objects;

    void Start()
    {
        MovingObject = agentObject.GetComponent<GrabAgentObject>().MovingObject;
        TargetObjects = agentObject.GetComponent<GrabAgentObject>().TargetObjects;
        FinishedObjects = agentObject.GetComponent<GrabAgentObject>().FinishedObjects;
        Objects = GameObject.Find("Objects");
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!agentObject.GetComponent<GrabAgentObject>().movingStatus)
        {
            
            foreach (var obj in Objects.GetComponentsInChildren<GameObject>()){
                if(obj.CompareTag("Target")){
                    TargetObjects[obj].SetActive(true);
                    obj.SetActive(true);
                }
            }
                
            if(MovingObjectStatus[MovingObject[0]] == 2){
                MovingObject[0].transform.position = TargetObjects[MovingObject[0]].transform.position;
                if(!FinishedObjects.Contains(MovingObject[0])) FinishedObjects.Add(MovingObject[0]);
                if(FinishedObjects.Count == pinchObject.GetComponent<collide>().onFrame.Count){
                    log.text = "coarseMovingTime: " + coarseMovingTime + "\n" + "accurateMovingTime: " + accurateMovingTime + "\n";
                }
                MovingObject.RemoveAt(0);
                agentObject.GetComponent<GrabAgentObject>().AutoAdjustStatus = true;
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
