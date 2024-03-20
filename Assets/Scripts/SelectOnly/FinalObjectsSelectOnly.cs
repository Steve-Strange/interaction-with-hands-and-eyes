using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalObjectsSelectOnly : MonoBehaviour
{
    public GameObject autoGenerate;
    public GameObject recorder;
    private GameObject SightCone;
    // private GameObject SightCone;
    public List<GameObject> finalObj = new List<GameObject>();
    public Dictionary<GameObject, Quaternion> finalObjQ = new Dictionary<GameObject, Quaternion>();
   // private float finalScale = 0.05f;
    int wrong;
    int right;
    int all;
    int round = 0;
    float finalScale = 0.05f;
    // Start is called before the first frame update
    void Start()
    {
        SightCone = GameObject.Find("SightCone");
        all = autoGenerate.GetComponent<autoGenerate>().targetNumber;
        wrong = 0;
        right = 0;
        round = 0;
    }

    public void AddFinalObj(GameObject obj)//除了最后的逻辑之外，其他都一样
    {
        if (autoGenerate.GetComponent<autoGenerate>().targets.Contains(obj))
        {
            autoGenerate.GetComponent<autoGenerate>().targets.Remove(obj);//点了之后下次再点会算错误
            right += 1;
            recorder.GetComponent<singleSelect>().selectOneObject();

            obj.GetComponent<Renderer>().material.color = Color.blue;
            if(right == all)
            {
                right = 0;
                round++;
                foreach (var item in autoGenerate.GetComponent<autoGenerate>().targets)
                {
                 
                    item.transform.rotation = autoGenerate.GetComponent<autoGenerate>().rotations[item];
                    item.transform.position = autoGenerate.GetComponent<autoGenerate>().poses[item];
                }
                if(round == 5)
                {
                    recorder.GetComponent<singleSelect>().writeFile("wrong" + wrong);
                    recorder.GetComponent<singleSelect>().finishAll();
                    autoGenerate.SetActive(false);
                }

            }
        }else
        {
            wrong++;
            recorder.GetComponent<singleSelect>().writeFile("you wrong");
        }

        obj.tag = "FinalObject";
        finalObjQ[obj] = obj.transform.rotation;
        obj.GetComponent<Outline>().OutlineColor = Color.clear;
        obj.transform.parent = transform;
        obj.transform.localEulerAngles = new Vector3(0, 0, 0);
        obj.transform.localScale = new Vector3(finalScale / transform.localScale.x, finalScale / transform.localScale.y, finalScale / transform.localScale.z);
        obj.transform.localPosition = new Vector3(obj.transform.localScale.x * 2 * finalObj.Count - transform.localScale.x / 2, 0, -2 * obj.transform.localScale.z);
        if (SightCone.GetComponent<SightConeSelectOnly>().selectedObjects.Contains(obj)) SightCone.GetComponent<SightConeSelectOnly>().selectedObjects.Remove(obj);
        SightCone.GetComponent<SightConeSelectOnly>().objectWeights[obj] = -1;

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
