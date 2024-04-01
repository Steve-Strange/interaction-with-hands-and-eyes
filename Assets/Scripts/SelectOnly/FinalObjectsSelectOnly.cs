using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class FinalObjectsSelectOnly : MonoBehaviour
{
    public GameObject autoGenerate;
    public GameObject recorder;
    private GameObject SightCone;
    // private GameObject SightCone;
    public List<GameObject> finalObj = new List<GameObject>();
    public Dictionary<GameObject, Quaternion> finalObjQ = new Dictionary<GameObject, Quaternion>();
    // private float finalScale = 0.05f;
    public TMP_Text T2, T3, T4, T5, T6;
    int wrong;
    int right;
    public int all;

    void Start()
    {
        SightCone = GameObject.Find("SightCone");
        wrong = 0;
        right = 0;
    }
    private void Update()
    {
        
        T2.text = right.ToString(); 
    }

    public void AddFinalObj(GameObject obj)//除了最后的逻辑之外，其他都一样
    {
        if (autoGenerate.GetComponent<autoGenerate>().targets.Contains(obj))//有
        {
            autoGenerate.GetComponent<autoGenerate>().targets.Remove(obj);//点了之后下次再点会算错误
            right += 1;
            recorder.GetComponent<singleSelect>().selectOneObject();
            obj.GetComponent<Renderer>().material.color = Color.blue;
            autoGenerate.GetComponent <autoGenerate>().genOne();//生成一个新的目标物体
            if(right == all)//已经选中了15个
            {
            
               /* foreach (var item in autoGenerate.GetComponent<autoGenerate>().targets)
                {
                 
                    item.transform.rotation = autoGenerate.GetComponent<autoGenerate>().rotations[item];
                    item.transform.position = autoGenerate.GetComponent<autoGenerate>().poses[item];
                }
                autoGenerate.GetComponent<autoGenerate>().reGenerate();
                if (round == 5)
                {}*/
                    recorder.GetComponent<singleSelect>().writeFile("wrong" + wrong);
                    recorder.GetComponent<singleSelect>().finishAll();
                    autoGenerate.SetActive(false);
            }
        }else
        {
            wrong++;
            recorder.GetComponent<singleSelect>().writeFile("you wrong");
        }

        //obj.tag = "FinalObject";
       // finalObjQ[obj] = obj.transform.rotation;
        obj.GetComponent<Outline>().OutlineColor = Color.clear;
        
       // float objMaxScale = Mathf.Max(obj.transform.GetComponent<Renderer>().bounds.size.x, obj.transform.GetComponent<Renderer>().bounds.size.y, obj.transform.GetComponent<Renderer>().bounds.size.z);
       // float finalScale = 0.05f / objMaxScale;

        //obj.transform.parent = transform;
        //obj.transform.localEulerAngles = new Vector3(0, 0, 0);

        // obj.transform.localScale = new Vector3(0.1f * obj.transform.localScale.x, 0.1f * obj.transform.localScale.y, 0.01f * obj.transform.localScale.z) / objMaxScale;
       // obj.transform.localScale = new Vector3(obj.transform.localScale.x, obj.transform.localScale.y, obj.transform.localScale.z) * finalScale;
       // obj.transform.localPosition = new Vector3(0.1f * finalObj.Count - 0.35f , 0, - obj.transform.localScale.z);
       // if(SightCone.GetComponent<SightCone>().selectedObjects.Contains(obj)) SightCone.GetComponent<SightCone>().selectedObjects.Remove(obj);
       /// SightCone.GetComponent<SightCone>().objectWeights[obj] = -1;
       // finalObj.Add(obj);
    }
    
    public void RearrangeFinalObj()
    {
        for(int i = 0; i < finalObj.Count; i++)
        {
            finalObj[i].transform.localPosition = new Vector3(finalObj[i].transform.localScale.x * 2 * i - transform.localScale.x/2 , 0, - 2 * finalObj[i].transform.localScale.z);
        }
    }
}
