using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;
using static UnityEngine.ParticleSystem;

public class ClickSelect : MonoBehaviour
{
    private List<GameObject> selectedRow = new List<GameObject>();
    public GameObject HandPoseManager, hand, thumb0, thumb1, thumb2, thumb3,
        index0, index1, index2, index3, index4,
        middle0, middle1, middle2, middle3,
        ring0, ring1, ring2, ring3,
        little0, little1, little2, little3;

    public RaycastHit thumb, index, middle, ring, little;
    public TMP_Text T, T2, T3, T4, T5, T6;
    public TMP_InputField log;
    private GameObject temp, temp1, temp2, temp3, temp4;

    private LineRenderer[] lines;
    private float angle, angle1, angle2, angle3, angle4, angleLast;
    private float angleLast1 = 1, angleLast2, angleLast3, angleLast4;
    private Vector3 vLast;
    private float clickThreashold = 0.1f;

    public GameObject FinalObjects;
    // Start is called before the first frame update

    void Start()
    {
        lines = new LineRenderer[5];//添加组件
        InvokeRepeating("RepeatedMethod", 1f, 0.4f);
    }
    private void RepeatedMethod()
    {
        selectedRow = HandPoseManager.GetComponent<HandPoseManager>().selectedRow;
        float d = culculate(thumb1, thumb2, thumb3);
        angleLast = d;
     
        Vector3 v1 = thumb2.transform.position - thumb3.transform.position;
        Vector3 v2 = index2.transform.position - thumb3.transform.position;
        vLast = new Vector3(v1.y * v2.z - v2.y * v1.z, v2.x * v1.z - v1.x * v2.z, (v1.x * v2.y - v2.x * v1.y));

        float d1 = culculate(index1, index2, index3);
        angleLast1 = d1;

        float d2 = culculate(middle1, middle2, middle3);
        angleLast2 = d2;

        float d3 = culculate(ring1, ring2, ring3);
        angleLast3 = d3;

        float d4 = culculate(little1, little2, little3);
        angleLast4 = d4;

    }
    private float culculate(GameObject one,GameObject two,GameObject three)//计算夹角
    {
        var first = one.transform.position - two.transform.position;
        var second = three.transform.position - two.transform.position;
        angle = Vector3.Dot(first,second)/(first.magnitude*second.magnitude);
        return angle;
    }
    public static Outline Load_Scripts<Outline>(GameObject target) where Outline : Component
    {
        if (target.GetComponent<Outline>() == null)
        {
            target.AddComponent<Outline>();
        }
        return target.GetComponent<Outline>();
    }

    // public static void Unload_Scripts<Outline>(GameObject target) where Outline : Component
    // {
    //     if (target.GetComponent<Outline>() != null)
    //     {
    //         GameObject.Destroy(target.GetComponent<Outline>() as Object);
    //     }
    // }
    // Update is called once per frame
    void Update()
    {   //手指沿手指关节发出射线，指尖和第一个指节

        log.text = string.Join(",", FinalObjects.GetComponent<FinalObjects>().finalObj);

        if(selectedRow[0])
            T2.text = selectedRow[0].name;
        if (selectedRow[1])
            T3.text = selectedRow[1].name;
        if (selectedRow[2])
            T4.text = selectedRow[2].name;
        if (selectedRow[3])
            T5.text = selectedRow[3].name;
        if (selectedRow[4])
            T6.text = selectedRow[4].name;

        if(HandPoseManager.GetComponent<HandPoseManager>().SecondSelectionState)
        {

            float d = culculate(thumb1, thumb2, thumb3);
            if (d - angleLast > clickThreashold / 3.5)
            {
                T.text = selectedRow[0].name;

                if(!FinalObjects.GetComponent<FinalObjects>().finalObj.Contains(selectedRow[0])) 
                    FinalObjects.GetComponent<FinalObjects>().AddFinalObj(selectedRow[0]);

            }

            
            float d1 = culculate(index1, index2, index3);
            if (angleLast1 - d1 > clickThreashold / 1.4)
            {
                //T.text = "yes";
                T.text = selectedRow[1].name;
                if(!FinalObjects.GetComponent<FinalObjects>().finalObj.Contains(selectedRow[1])) 
                    FinalObjects.GetComponent<FinalObjects>().AddFinalObj(selectedRow[1]);
                
            }
        
            
            float d2 = culculate(middle1, middle2, middle3);
            float d3 = culculate(ring1, ring2, ring3);
            float d4 = culculate(little1, little2, little3);
            if (angleLast2 - d2 > clickThreashold / 1.5)
            {
                T.text = selectedRow[2].name;
                if(!FinalObjects.GetComponent<FinalObjects>().finalObj.Contains(selectedRow[2])) 
                    FinalObjects.GetComponent<FinalObjects>().AddFinalObj(selectedRow[2]);
            }
            else if (angleLast4 - d4 > clickThreashold / 2)
            {
                T.text = selectedRow[4].name;
                if(!FinalObjects.GetComponent<FinalObjects>().finalObj.Contains(selectedRow[4])) 
                    FinalObjects.GetComponent<FinalObjects>().AddFinalObj(selectedRow[4]);

            }
            else if (angleLast3 - d3 > clickThreashold / 1.2)
            {
                T.text = selectedRow[3].name;
                if(!FinalObjects.GetComponent<FinalObjects>().finalObj.Contains(selectedRow[3]))
                    FinalObjects.GetComponent<FinalObjects>().AddFinalObj(selectedRow[3]);
                
            }
    
            
            
        }
    
          /*  Ray ray0 = new Ray(thumb0.transform.position, thumb0.transform.position - thumb1.transform.position);
         Ray ray1 = new Ray(index0.transform.position, index0.transform.position - index1.transform.position);
         Ray ray2 = new Ray(middle0.transform.position, middle0.transform.position - middle1.transform.position);
         Ray ray3 = new Ray(ring0.transform.position, ring0.transform.position - ring1.transform.position);
         Ray ray4 = new Ray(little0.transform.position, little0.transform.position - little1.transform.position);

         LineRenderer line = temp.GetComponent<LineRenderer>();
         LineRenderer line1 = temp1.GetComponent<LineRenderer>();
         LineRenderer line2 = temp2.GetComponent<LineRenderer>();
         LineRenderer line3 = temp3.GetComponent<LineRenderer>();
         LineRenderer line4 = temp4.GetComponent<LineRenderer>();

         if (Physics.Raycast(ray0, out thumb)){
             line.positionCount = 2;
             line.enabled = true;
             line.startWidth = (float)0.003;
             line.endWidth = (float)0.003;
             line.SetPosition(0, thumb0.transform.position);
             line.SetPosition(1, thumb.transform.position);
         }else
         {
             line = temp.GetComponent<LineRenderer>();
             line.enabled = true;
             line.positionCount = 2;
             line.startWidth=(float)0.003;
             line.endWidth = (float)0.003;
             line.SetPosition(0, thumb0.transform.position);
             line.SetPosition(1, thumb0.transform.position + (thumb0.transform.position - thumb1.transform.position) * 100000);
         }
         if (Physics.Raycast(ray1, out index)){
             line1.positionCount = 2;
             line1.enabled = true;
             line1.startWidth = (float)0.003;
             line1.endWidth = (float)0.003;
             line1.SetPosition(0, index0.transform.position);
             line1.SetPosition(1, index.transform.position);
         }
         else{
             line1 = temp1.GetComponent<LineRenderer>();
             line1.enabled = true;
             line1.positionCount = 2;
             line1.startWidth = (float)0.01;
             line1.endWidth = (float)0.01;
             line1.SetPosition(0, index0.transform.position);
             line1.SetPosition(1, index0.transform.position + (index0.transform.position - index1.transform.position) * 100000);
         }
         if (Physics.Raycast(ray2, out middle)){
             Debug.Log(middle.collider.gameObject.name);
         }
         if (Physics.Raycast(ray3, out ring)){
             Debug.Log(ring.collider.gameObject.name);
         }
         if (Physics.Raycast(ray4, out little)){
             Debug.Log(little.collider.gameObject.name);
         }

        */

    }
}
