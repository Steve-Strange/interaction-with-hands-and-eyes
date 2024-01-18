using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;
using static UnityEngine.ParticleSystem;

public class clickSelect : MonoBehaviour
{
    private GameObject[] backup;
    public GameObject handPoseManager, hand, thumb0, thumb1, thumb2, thumb3,
        index0, index1, index2, index3, index4,
        middle0, middle1, middle2, middle3,
        ring0, ring1, ring2, ring3,
        little0, little1, little2, little3;

    public RaycastHit thumb, index, middle, ring, little;
    public TMP_Text T, T2, T3, T4, T5, T6;
    private GameObject temp, temp1, temp2, temp3, temp4;

    private LineRenderer[] lines;
    private float angle, angle1, angle2, angle3, angle4, angleLast;
    private float angleLast1 = 1, angleLast2, angleLast3, angleLast4;
    private Vector3 vLast;
    public List<GameObject> final = new List<GameObject>();
    // Start is called before the first frame update
    bool find(GameObject o)
    {
        int i;
        for(i=0; i<final.Count;i++){
        if(final[i].name == o.name)
         return false;
        }
        return true;

    }
    void Start()
    {
        lines = new LineRenderer[5];//添加组件
        InvokeRepeating("RepeatedMethod", 1f, 0.4f);
        backup = handPoseManager.GetComponent<HandPoseManager>().back;
    }
    private void RepeatedMethod()
    {

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
    /// <summary>
    /// 卸载目标脚本
    /// </summary>
    /// <param name="go"></param>
    public static void Unload_Scripts<Outline>(GameObject target) where Outline : Component
    {
        if (target.GetComponent<Outline>() != null)
        {
            GameObject.Destroy(target.GetComponent<Outline>() as Object);
        }
    }
    // Update is called once per frame
    void Update()
    {   //手指沿手指关节发出射线，指尖和第一个指节
        if(backup[0])
            T2.text = backup[0].name;
        if (backup[1])
            T3.text = backup[1].name;
        if (backup[2])
            T4.text = backup[2].name;
        if (backup[3])
            T5.text = backup[3].name;
        if (backup[4])
            T6.text = backup[4].name;

        float d = culculate(thumb1, thumb2, thumb3);
        // T.text = (d - angleLast).ToString();
        //float d = (thumb0.transform.position - little2.transform.position).magnitude;
        //T2.text = d.ToString();
       // T3.text = hand.transform.up.y.ToString();
        //T4.text = hand.transform.up.z.ToString();
        if (angleLast - d > 0.07 || angleLast - d < -0.07)
        {   
            T.text = backup[0].name;
            //不能重复选
            if(!find(backup[0]))
            {
                
                final.Add(backup[0]);
             
            }
        }

        
       float d1 = culculate(index1, index2, index3);
        if (angleLast1 - d1 > 0.2 || angleLast1 - d1 < -0.2)
        {
            //T.text = "yes";
            T.text = backup[1].name;
            if (!find(backup[1]))
            {
                final.Add(backup[1]);
                
           
            }
        }
       
        
        float d2 = culculate(middle1, middle2, middle3);
        if (angleLast2 - d2 > 0.2 || angleLast2 - d2 < -0.2)
        {
            T.text = backup[2].name;
            if (!find(backup[2]))
            {
                final.Add(backup[2]);

            }
        }
   
        float d3 = culculate(ring1, ring2, ring3);
        if (angleLast3 - d3 > 0.2 || angleLast3 - d3 < -0.2)
        {
            T.text = backup[3].name;
            if (!find(backup[3]))
            {
                final.Add(backup[3]);
            }
        }
 
        float d4 = culculate(little1, little2, little3);
        if (angleLast4 - d4 > 0.2 || angleLast4 - d4 < -0.2)
        {
            T.text = backup[4].name;
            if (!find(backup[4]))
            {
                final.Add(backup[4]);
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
