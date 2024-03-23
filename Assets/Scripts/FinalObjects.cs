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

        float objMaxScale = Mathf.Max(obj.transform.GetComponent<Renderer>().bounds.size.x, obj.transform.GetComponent<Renderer>().bounds.size.y, obj.transform.GetComponent<Renderer>().bounds.size.z);
                
        obj.transform.localScale = new Vector3(0.1f * obj.transform.localScale.x, 0.1f * obj.transform.localScale.y, 0.01f * obj.transform.localScale.z) / objMaxScale;
        // obj.transform.localScale = new Vector3(obj.transform.GetComponent<Renderer>().bounds.size.x / transform.localScale.x, obj.transform.GetComponent<Renderer>().bounds.size.y / transform.localScale.y, obj.transform.GetComponent<Renderer>().bounds.size.z / transform.localScale.z) * finalScale / objMaxScale;
        obj.transform.localPosition = new Vector3(0.05f * 2 * finalObj.Count - transform.localScale.x/2 , 0, - 2 * 0.05f);
        if(SightCone.GetComponent<SightCone>().selectedObjects.Contains(obj)) SightCone.GetComponent<SightCone>().selectedObjects.Remove(obj);
        SightCone.GetComponent<SightCone>().objectWeights[obj] = -1;
        
        finalObj.Add(obj);
    }
    
    public void RearrangeFinalObj()
    {
        for(int i = 0; i < finalObj.Count; i++)
        {
            finalObj[i].transform.localPosition = new Vector3(finalObj[i].transform.localScale.x * 2 * i - transform.localScale.x/2 , 0, - 2 * finalObj[i].transform.localScale.z);
        }
    }
}
