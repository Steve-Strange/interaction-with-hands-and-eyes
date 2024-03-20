using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;
using static UnityEngine.ParticleSystem;

public class clickSelectSelectOnly : MonoBehaviour
{
    private List<GameObject> selectedRow = new List<GameObject>();
    public GameObject HandPoseManager, hand, thumb0, thumb1, thumb2, thumb3,
        index0, index1, index2, index3, index4,
        middle0, middle1, middle2, middle3;
    private float[] d = new float[5];
    private float[] ad = new float[5];
    private float[] angleLast = new float[5];
    private float[] gap = new float[5];
    public RaycastHit thumb, index, middle, ring, little;
    // public TMP_Text  T2, T3, T4, T5, T6;
    // public TMP_InputField log;
    private GameObject SightCone;

    //private float angle, angle1, angle2, angle3, angle4, angleLast;
    //private float angleLast1 = 1, angleLast2, angleLast3, angleLast4;


    public GameObject FinalObjects;
    void Start()
    {
        InvokeRepeating("RepeatedMethod", 1f, 0.6f);
        SightCone = GameObject.Find("SightCone");
    }
    private void RepeatedMethod()
    {
        float d = culculate(thumb1, thumb2, thumb3);

        { angleLast[0] = d; }
        if (-d > 0.95f)
            ad[0] = d;

        d = culculate(index1, index2, index3);

        angleLast[1] = d;
        if (-d > 0.95f)
            ad[1] = d;

        d = culculate(middle1, middle2, middle3);

        angleLast[2] = d;
        if (-d > 0.95f)
            ad[2] = d;
    }
    private float culculate(GameObject one, GameObject two, GameObject three)//����н�
    {
        var first = one.transform.position - two.transform.position;
        var second = three.transform.position - two.transform.position;
        float angle = Vector3.Dot(first, second) / (first.magnitude * second.magnitude);
        return angle;
    }
    int time = 0;

    // Update is called once per frame
    void Update()
    {

        selectedRow = HandPoseManager.GetComponent<HandPoseManager>().selectedRow;
       
        time += 1;
        if (time > 30)
            time = 22;
        bool[] mark = new bool[5];
        if (time > 20 && HandPoseManager.GetComponent<HandPoseManager>().SecondSelectionState && HandPoseManager.GetComponent<HandPoseManager>().PalmPoseState)
        {
            d[0] = culculate(thumb1, thumb2, thumb3);//0.96-0.6   
            d[1] = culculate(index1, index2, index3);
            d[2] = culculate(middle1, middle2, middle3);

            mark[0] = false;
            mark[1] = false;
            mark[2] = false;

            if (d[0] - angleLast[0] > 0.10)
            {
                mark[0] = true;
            }
            if (d[1] - angleLast[1] > 0.25)//0.99-0.7(С��0.7)
            {
                mark[1] = true;
            }

            if (d[2] - angleLast[2] > 0.3)//С��0.7
            {
                mark[2] = true;
            }
            {

                float max = -1;
                int select = -1;
                for (int i = 0; i <= 2; i++)
                {
                    if (d[i] - angleLast[i] > max)
                    {
                        max = d[i] - angleLast[i];
                        select = i;
                    }

                }
                if (!FinalObjects.GetComponent<FinalObjectsSelectOnly>().finalObj.Contains(selectedRow[select]) && mark[select] && selectedRow[select] != HandPoseManager.GetComponent<HandPoseManagerSelectOnly>().emptyBlock)
                {
                    FinalObjects.GetComponent<FinalObjectsSelectOnly>().AddFinalObj(selectedRow[select]);
                    SightCone.GetComponent<SightConeSelectOnly>().objectWeights.Remove(selectedRow[select]);
                    time = 0;
                }
            }






        }


    }
}
