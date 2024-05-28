using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class autoGenerate : MonoBehaviour
{
    //to randomly make six bubble turn green;
    private List<GameObject> allObjects = new List<GameObject>();
    public List<GameObject> targets  = new List<GameObject>();
    public Dictionary<GameObject, Quaternion> rotations = new Dictionary<GameObject, Quaternion>();
    public Dictionary<GameObject, Vector3> poses = new Dictionary<GameObject, Vector3>();
    public GameObject father;
    public int targetNumber = 15;//每次的目标物体
    void Start()
    {
        if(GameObject.Find("recorder").GetComponent<singleSelect>().sampleType !=0)
        {
            gameObject.SetActive(false);

        }
        else
        {
            FindChild(father);
            var testArray = GetRandomSequence(allObjects.Count, targetNumber);
            for (int i = 0;i < targetNumber; i++)
            {
                allObjects[testArray[i]].GetComponent<Renderer>().material.color = Color.green;
                targets.Add(allObjects[testArray[i]]);
                rotations[allObjects[testArray[i]]] = allObjects[testArray[i]].transform.rotation;
                poses[allObjects[testArray[i]]] = allObjects[testArray[i]].transform.position;
            }

        }
        
    }
    public void genOneBubble()
    {
        var testArray = GetRandomSequence(allObjects.Count, 1);
        allObjects[testArray[0]].GetComponent<Renderer>().material.color = Color.green;
        targets.Add(allObjects[testArray[0]]);
    }
    public void genOne(){//直接用number替换对应坑位，每次不remove了
                         //每次有一个物体被选择就调用这个
        var testArray = GetRandomSequence(allObjects.Count, 1);
        allObjects[testArray[0]].GetComponent<Renderer>().material.color = Color.green;
        targets.Add(allObjects[testArray[0]]);
        /*while (true)
        {
            var testArray = GetRandomSequence(allObjects.Count, 1);
            if (!handPoseManagerSelectOnly.GetComponent<HandPoseManagerSelectOnly>().sorted15ObjectWeights.ContainsKey(allObjects[testArray[0]]))
            { 
                allObjects[testArray[0]].GetComponent<Renderer>().material.color= Color.green;
                targets.Add(allObjects[testArray[0]]);
                break;
            } 
        }*/

    }
    public void reGenerate()
    {
        targets.Clear();
        rotations.Clear();
        poses.Clear();
        foreach (var item in allObjects){
            item.SetActive(true);
            AddOutline(item, Color.clear);
            item.GetComponent<Renderer>().material.color = Color.blue;
        }//恢复最原始的状态

        var testArray = GetRandomSequence(allObjects.Count, targetNumber);
        for (int i = 0;i < targetNumber; i++)
        {
            allObjects[testArray[i]].GetComponent<Renderer>().material.color = Color.green;
            targets.Add(allObjects[testArray[i]]);
        }

    }
    void FindChild(GameObject child)
    {
        //����forѭ�� ��ȡ�����µ�ȫ��������
        for (int c = 0; c < child.transform.childCount; c++)
        {
            child.transform.GetChild(c).GetComponent<Renderer>().material.color = Color.blue;//初始化
            allObjects.Add(child.transform.GetChild(c).gameObject);
        }
    }
    // <summary>
    /// 随机抽取随机数
    /// </summary>
    /// <param name="total"></param>
    /// <param name="count"></param>
    // <returns></returns>
    int[] GetRandomSequence(int total,int count)
    {
        int[] sequence = new int[total];
        int[] output = new int[count];

        for (int i = 0;i <total;i++)
        {
            sequence[i] = i;
        }
        int end = total - 1;
        for (int i = 0; i < count;i++)
        {
            //随机一个数，每随机一次，随机区间-1
            int num = Random.Range(0,end + 1);
            output[i] = sequence[num];
            //将区间最后一个数赋值到取到的数上
            sequence[num] = sequence[end];
            end--;
        }
        return output;
    }
    public void AddOutline(GameObject target, Color color)
    {
        if (target.GetComponent<Outline>() == null)
        {
            target.AddComponent<Outline>();

        }
        target.GetComponent<Outline>().OutlineColor = color;
    }
}
