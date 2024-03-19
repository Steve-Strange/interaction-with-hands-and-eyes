using System;
using System.Drawing;
using System.IO.Enumeration;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;
using static UnityEngine.ParticleSystem;

public class autoGenerate : MonoBehaviour
{
    //to randomly make six bubble turn green;
    private List<GameObject> allObjects = new List<GameObject>();
    public List<GameObject> targets  = new List<GameObject>();
    public GameObject father;
    void Start()
    {
        FindChild(father);
        testArray = GetRandomSequence(allObjects.Count,6);
        for (int i = 0;i < 6;i++)
        {
            allObjects[testArray[i]].GetComponent<MeshRenderer>().Color = Color.green;
            targets.Add(allObjects[testArray[i]]);
        }

    }
    public void reGenerate()
    {
        targets.Clear();
        testArray = GetRandomSequence(allObjects.Count,6);
        for (int i = 0;i < 6;i++)
        {
            allObjects[testArray[i]].GetComponent<MeshRenderer>().Color = Color.green;
            targets.Add(allObjects[testArray[i]]);
        }
    }
    void FindChild(GameObject child)
    {
        //����forѭ�� ��ȡ�����µ�ȫ��������
        for (int c = 0; c < child.transform.childCount; c++)
        {
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
}
