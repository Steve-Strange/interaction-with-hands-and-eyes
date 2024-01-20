using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;
using static UnityEngine.ParticleSystem;

public class clickSelect : MonoBehaviour
{
    private List<GameObject> selectedRow = new List<GameObject>();
    public GameObject HandPoseManager, hand, thumb0, thumb1, thumb2, thumb3,
        index0, index1, index2, index3, index4,
        middle0, middle1, middle2, middle3,
        ring0, ring1, ring2, ring3,
        little0, little1, little2, little3;
    private float[] d = new float[5];
    private float[] ad = new float[5];
    private float[] angleLast = new float[5];
    private float[] gap = new float[5];
    public RaycastHit thumb, index, middle, ring, little;
    public TMP_Text T, T2, T3, T4, T5, T6;
    public TMP_InputField log;
    private GameObject temp, temp1, temp2, temp3, temp4;

    private LineRenderer[] lines;
    //private float angle, angle1, angle2, angle3, angle4, angleLast;
    //private float angleLast1 = 1, angleLast2, angleLast3, angleLast4;
    private Vector3 vLast;
    private float clickThreashold = 0.1f;

    public GameObject FinalObjects;
    // Start is called before the first frame update
Vector3 Last;
    void Start()
    {
        lines = new LineRenderer[5];//添加组件
        InvokeRepeating("RepeatedMethod", 1f, 0.6f);
        Last = thumb1.transform.position;
        StartCoroutine("UpdateVelocity");
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
 if (-d> 0.95f)
            ad[2] = d;
        d = culculate(ring1, ring2, ring3);
      
            angleLast[3] = d; 
        if (-d > 0.95f)
            ad[3] = d;

        d = culculate(little1, little2, little3);
       
            angleLast[4]= d;
         if (-d> 0.95f)
            ad[4] = d;

    }
    private float culculate(GameObject one,GameObject two,GameObject three)//计算夹角
    {
        var first = one.transform.position - two.transform.position;
        var second = three.transform.position - two.transform.position;
        float angle = Vector3.Dot(first,second)/(first.magnitude*second.magnitude);
        return angle;
    }
    int time = 0;
    Vector3 v;
    Vector3 lastV;
    IEnumerator UpdateVelocity()
    {
        Vector3 lastPosition = thumb1.transform.position;
        Vector3 newPositon;


        while (true)
        {
            yield return 0;
            //获得速度
            newPositon = thumb1.transform.position;
            v = (newPositon - lastPosition) / 0.02f;//一帧是0.02s，所以这里除以0.02f
            lastPosition = newPositon;
          
        }
    }

    // Update is called once per frame
    void Update()
    {
        
        selectedRow = HandPoseManager.GetComponent<HandPoseManager>().selectedRow;
        log.text = string.Join(",", FinalObjects.GetComponent<FinalObjects>().finalObj);

        /*if(selectedRow[0])
            T2.text = selectedRow[0].name;
        if (selectedRow[1])
            T3.text = selectedRow[1].name;
        if (selectedRow[2])
            T4.text = selectedRow[2].name;
        if (selectedRow[3])
            T5.text = selectedRow[3].name;
        if (selectedRow[4])
            T6.text = selectedRow[4].name;

        T.text = culculate(index1, index2, index3).ToString();
        T2.text = culculate(middle1, middle2, middle3).ToString();
        T3.text = culculate(ring1, ring2, ring3).ToString();
        T4.text = culculate(little1, little2, little3).ToString();        T.text = (d[0] - angleLast[0]).ToString();
            T2.text = (d[1] - angleLast[1]).ToString();
            T3.text = (d[2] - angleLast[2]).ToString();
            T4.text = (d[3] - angleLast[3]).ToString();
            T5.text = (d[4] - angleLast[4]).ToString();*/
        time += 1;
        if (time > 20)
            time = 12;
        bool[] mark = new bool[5];
        if (time>10&&HandPoseManager.GetComponent<HandPoseManager>().SecondSelectionState && HandPoseManager.GetComponent<HandPoseManager>().PalmPoseState)
        {        d[0] = culculate(thumb1, thumb2, thumb3);//0.96-0.6
            
        d[1] = culculate(index1, index2, index3);
        d[2] = culculate(middle1, middle2, middle3);
        d[3] = culculate(ring1, ring2, ring3);
        d[4] = culculate(little1, little2, little3);
            

            
            mark[0] = false;
            mark[1]= false;
            mark[2] = false;
            mark[3] = false;
            mark[4] = false;

            if (d[0]-angleLast[0]> 0.12 && v.magnitude > 0.25)
            {
                mark[0] = true;


            }
            T.text = mark[0].ToString();
            T2.text = (d[0] - angleLast[0]).ToString();
            Last = thumb1.transform.position;

            T2.text = (d[0] - ad[0]).ToString();
            if (  d[1]- angleLast[1]> 0.3)//0.99-0.7(小于0.7)
            {

                mark[1] = true;

                
            }
        
            if ( d[2]- angleLast[2] > 0.3)//小于0.7
            {

                mark[2] = true;


            }            
            if (d[3]-angleLast[3]  > 0.2)//小于0.7
            {
                mark[3] = true;
                
            }
           if (d[4]-angleLast[4] > 0.3)//小于0.7
            {
                mark[4] = true;

            }

            if (! (mark[1] == true && mark[2] == true && mark[3] == true && mark[4] == true || 
                mark[0]== true && mark[1] == true && mark[2] == true && mark[3] == true && mark[4] == true ||
                mark[1] == true && mark[2] == true && mark[3]|| d[2] - angleLast[2]>0.45 && d[1] - angleLast[1] > 0.45 ||
                d[0] - angleLast[0] > 0.45 && d[1] - angleLast[1] > 0.45 || d[2] - angleLast[2] > 0.45 && d[0] - angleLast[0] > 0.45))//防止握拳头
            {
                
                float max = -1;
                int i = 0;
                int select = 0;
                for (i=0;i<=4;i++)
                {
                    if(d[i]-angleLast[i]>max)
                    {
                        max = d[i] - angleLast[i];
                        select = i;
                    }

                } //T2.text = select.ToString();
                if(!(select==2&&mark[1]==true&&mark[3]==true))
                if (!FinalObjects.GetComponent<FinalObjects>().finalObj.Contains(selectedRow[select]) && mark[select] ==true)
                {
                    FinalObjects.GetComponent<FinalObjects>().AddFinalObj(selectedRow[select]);
                    selectedRow[select] = null; 
                        time = 0;
                }


               


            }






        }
    
      
    }
}
