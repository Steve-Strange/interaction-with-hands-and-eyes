using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalObjectsSelectOnly : MonoBehaviour
{
    public GameObject autoGenerate;
    public GameObject recorder;
   // private GameObject SightCone;
    public List<GameObject> finalObj = new List<GameObject>();
    public Dictionary<GameObject, Quaternion> finalObjQ = new Dictionary<GameObject, Quaternion>();
   // private float finalScale = 0.05f;
    int wrong;
    int right;
    int all;

    // Start is called before the first frame update
    void Start()
    {
        all = autoGenerate.GetComponent<autoGenerate>().targetNumber;
        wrong = 0;
        right = 0;
     //   SightCone = GameObject.Find("SightCone");
    }

    // Update is called once per frame
    void Update()
    {
        // transform.LookAt(Camera.main.transform.position);

    }

    public void AddFinalObj(GameObject obj)
    {
        if (autoGenerate.GetComponent<autoGenerate>().targets.Contains(obj))
        {
            autoGenerate.GetComponent<autoGenerate>().targets.Remove(obj);//点了之后下次再点会算错误
            right += 1;
            recorder.GetComponent<singleSelect>().selectOneObject();

            obj.GetComponent<Renderer>().material.color = Color.blue;
            if(right == all)
            {
                recorder.GetComponent<singleSelect>().writeFile("wrong" + wrong);
                recorder.GetComponent<singleSelect>().finishAll();
            }
        }else
        {
            wrong++;
            recorder.GetComponent<singleSelect>().writeFile("you wrong");
        }
        /*obj.tag = "FinalObject";
        finalObjQ[obj] = obj.transform.rotation;
        obj.GetComponent<Outline>().OutlineColor = Color.clear;
        obj.transform.parent = transform;
        obj.transform.localEulerAngles = new Vector3(0, 0, 0);
        obj.transform.localScale = new Vector3(finalScale / transform.localScale.x, finalScale / transform.localScale.y, finalScale / transform.localScale.z);
        obj.transform.localPosition = new Vector3(obj.transform.localScale.x * 2 * finalObj.Count - transform.localScale.x/2 , 0, - 2 * obj.transform.localScale.z);
        if(SightCone.GetComponent<SightCone>().selectedObjects.Contains(obj)) SightCone.GetComponent<SightCone>().selectedObjects.Remove(obj);
        SightCone.GetComponent<SightCone>().objectWeights[obj] = -1;//选择之后物体不会消失，还是能被选
        
        finalObj.Add(obj);*/
    }
    
 /*   public void RearrangeFinalObj()
    {
        for(int i = 0; i < finalObj.Count; i++)
        {
            finalObj[i].transform.localPosition = new Vector3(finalObj[i].transform.localScale.x * 2 * i - transform.localScale.x/2 , 0, - 2 * finalObj[i].transform.localScale.z);
        }
    }*/
}
