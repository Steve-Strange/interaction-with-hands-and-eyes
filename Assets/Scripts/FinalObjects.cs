using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalObjects : MonoBehaviour
{

    private GameObject SightCone;
    public List<GameObject> finalObj = new List<GameObject>();
    public Dictionary<GameObject, Quaternion> finalObjQ = new Dictionary<GameObject, Quaternion>();

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
        
        float objMaxScale = Mathf.Max(obj.transform.GetComponent<Renderer>().bounds.size.x, obj.transform.GetComponent<Renderer>().bounds.size.y, obj.transform.GetComponent<Renderer>().bounds.size.z);
        float finalScale = 0.05f / objMaxScale;

        obj.transform.parent = transform;
        obj.transform.localEulerAngles = new Vector3(0, 0, 0);

        // obj.transform.localScale = new Vector3(0.1f * obj.transform.localScale.x, 0.1f * obj.transform.localScale.y, 0.01f * obj.transform.localScale.z) / objMaxScale;
        obj.transform.localScale = new Vector3(obj.transform.localScale.x, obj.transform.localScale.y, obj.transform.localScale.z) * finalScale;
        obj.transform.localPosition = new Vector3(0.1f * finalObj.Count - 0.35f , 0, - obj.transform.localScale.z / 2);
        if(SightCone.GetComponent<SightCone>().selectedObjects.Contains(obj)) SightCone.GetComponent<SightCone>().selectedObjects.Remove(obj);
        SightCone.GetComponent<SightCone>().objectWeights[obj] = -1;
        
        finalObj.Add(obj);
        finalObj[0].GetComponent<Outline>().OutlineColor = Color.green;
    }
    
    public void RearrangeFinalObj()
    {
        for(int i = 0; i < finalObj.Count; i++)
        {
            finalObj[i].transform.localPosition = new Vector3(0.1f * i - 0.35f, 0, - finalObj[i].transform.localScale.z / 2);
            if(i == 0) finalObj[i].GetComponent<Outline>().OutlineColor = Color.green;//第一个颜色变绿
            else finalObj[i].GetComponent<Outline>().OutlineColor = Color.clear;
        }
    }
}
