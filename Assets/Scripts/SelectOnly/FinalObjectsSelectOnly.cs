using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class FinalObjectsSelectOnly : MonoBehaviour
{
    public GameObject autoGenerate;
    public GameObject recorder;
    public List<GameObject> finalObj = new List<GameObject>();
    public Dictionary<GameObject, Quaternion> finalObjQ = new Dictionary<GameObject, Quaternion>();
    public TMP_Text T2, T3, T4, T5, T6;
    int wrong;
    int right;
    int round = 0;

    void Start()
    {
        wrong = 0;
        right = 0;
        round = 0;
    }
    private void Update()
    {
       
    }

    public void AddFinalObj(GameObject obj)//除了最后的逻辑之外，其他都一样
    {
        if (autoGenerate.GetComponent<autoGenerate>().targets.Contains(obj))//是我的目标物体
        {
        
            right += 1;
            recorder.GetComponent<singleSelect>().selectOneObject();
            obj.SetActive(false);
            if(right == autoGenerate.GetComponent<autoGenerate>().targetNumber)//已经选中了20个
            {
                round ++;
                right = 0;
                if(round == 2){
                    recorder.GetComponent<singleSelect>().writeFile("wrong" + wrong);
                    recorder.GetComponent<singleSelect>().finishAll();
                    autoGenerate.SetActive(false);
                }
                autoGenerate.GetComponent<autoGenerate>().reGenerate();
            }   
        }else{
            obj.SetActive(false);
            obj.GetComponent<Outline>().OutlineColor = Color.red;//标红表示点错了
            wrong++;
            recorder.GetComponent<singleSelect>().writeFile("you wrong");
        }

        //obj.tag = "FinalObject";
        
        //obj.GetComponent<Outline>().OutlineColor = Color.clear;
             // finalObjQ[obj] = obj.transform.rotation;
        // float objMaxScale = Mathf.Max(obj.transform.GetComponent<Renderer>().bounds.size.x, obj.transform.GetComponent<Renderer>().bounds.size.y, obj.transform.GetComponent<Renderer>().bounds.size.z);
        // float finalScale = 0.05f / objMaxScale;

        //obj.transform.parent = transform;
        //obj.transform.localEulerAngles = new Vector3(0, 0, 0);

        // obj.transform.localScale = new Vector3(0.1f * obj.transform.localScale.x, 0.1f * obj.transform.localScale.y, 0.01f * obj.transform.localScale.z) / objMaxScale;
        // obj.transform.localScale = new Vector3(obj.transform.localScale.x, obj.transform.localScale.y, obj.transform.localScale.z) * finalScale;
        // obj.transform.localPosition = new Vector3(0.1f * finalObj.Count - 0.35f , 0, - obj.transform.localScale.z);
        // if(SightCone.GetComponent<SightCone>().selectedObjects.Contains(obj)) SightCone.GetComponent<SightCone>().selectedObjects.Remove(obj);
        // SightCone.GetComponent<SightCone>().objectWeights[obj] = -1;
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
