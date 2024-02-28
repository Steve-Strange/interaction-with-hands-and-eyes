using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalObjects : MonoBehaviour
{

    private GameObject SightCone;
    public List<GameObject> finalObj = new List<GameObject>();
    public Dictionary<GameObject, Quaternion> finalObjQ = new Dictionary<GameObject, Quaternion>();
    private float finalScale = 0.05f;

    // Start is called before the first frame update
    void Start()
    {
        SightCone = GameObject.Find("SightCone");
    }

    // Update is called once per frame
    void Update()
    {
        // transform.LookAt(Camera.main.transform.position);

    }

    public void AddFinalObj(GameObject obj)
    {
        obj.tag = "FinalObject";
        finalObjQ[obj] = obj.transform.rotation;
        obj.GetComponent<Outline>().OutlineColor = Color.clear;
        obj.transform.parent = transform;
        obj.transform.localEulerAngles = new Vector3(0, 0, 0);
        obj.transform.localScale = new Vector3(finalScale / transform.localScale.x, finalScale / transform.localScale.y, finalScale / transform.localScale.z);
        obj.transform.localPosition = new Vector3(transform.localScale.x/2 - obj.transform.localScale.x * 2 * finalObj.Count, 0, 0);
        if(SightCone.GetComponent<SightCone>().selectedObjects.Contains(obj)) SightCone.GetComponent<SightCone>().selectedObjects.Remove(obj);
        SightCone.GetComponent<SightCone>().objectWeights[obj] = -1;
        
        finalObj.Add(obj);
    }
    public void RemoveFirst()
    {

    }
}
