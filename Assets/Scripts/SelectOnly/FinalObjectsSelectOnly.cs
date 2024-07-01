using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class FinalObjectsSelectOnly : MonoBehaviour
{//���տ���ѡ���߼��ĵط�
    public GameObject autoGenerate;
    public GameObject recorder;
    public List<GameObject> finalObj = new List<GameObject>();
    public Dictionary<GameObject, Quaternion> finalObjQ = new Dictionary<GameObject, Quaternion>();
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

    public void AddFinalObj(GameObject obj)//���������߼�֮�⣬������һ��
    {
        if (autoGenerate.GetComponent<autoGenerate>().targets.Contains(obj))//���ҵ�Ŀ������
        {
        
            right += 1;
            recorder.GetComponent<singleSelect>().selectOneObject();
            obj.SetActive(false);
            if(right == 15)//�Ѿ�ѡ����20��
            {
                Debug.Log(round);
                round ++;
                right = 0;
                if(round == 2){
                    recorder.GetComponent<singleSelect>().writeFile("finish all");
                    recorder.GetComponent<singleSelect>().writeFile("precision:" + 30f / (wrong + 30f));
                    recorder.GetComponent<singleSelect>().finishAll();
                    autoGenerate.SetActive(false);
                }
                autoGenerate.GetComponent<autoGenerate>().reGenerate();
            }   
        }else{
            obj.SetActive(false);
            wrong++;
        }
    }
    
    public void RearrangeFinalObj()
    {
        for(int i = 0; i < finalObj.Count; i++)
        {
            finalObj[i].transform.localPosition = new Vector3(finalObj[i].transform.localScale.x * 2 * i - transform.localScale.x/2 , 0, - 2 * finalObj[i].transform.localScale.z);
        }
    }
}
