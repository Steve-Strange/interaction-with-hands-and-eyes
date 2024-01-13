using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;
using static UnityEngine.ParticleSystem;

public class clickSelect : MonoBehaviour
{
    public GameObject hand;
    public GameObject thumb0;
    public GameObject thumb1;
    public GameObject thumb2;
    public GameObject thumb3;
    public GameObject index0;
    public GameObject index1;
    public GameObject index2;
    public GameObject index3;
    public GameObject index4;
    public GameObject middle0;
    public GameObject middle1;
    public GameObject middle2;
    public GameObject middle3;
    public GameObject ring0;
    public GameObject ring1;
    public GameObject ring2;
    public GameObject ring3;
    public GameObject little0;
    public GameObject little1;
    public GameObject little2;
    public GameObject little3;
    public RaycastHit thumb;
    public RaycastHit index;
    public RaycastHit middle;
    public RaycastHit ring;
    public RaycastHit little;
    public GameObject temp;
    public GameObject temp1;
    public GameObject temp2;
    public GameObject temp3;
    public GameObject temp4;
    public TMP_Text T;
    public TMP_Text T2;
    public TMP_Text T3;
    public TMP_Text T4;
    private LineRenderer[] lines;//线的定义
    private float angle;
    private float angle1;
    private float angle2;
    private float angle3;
    private float angle4;
    private float angleLast;
    private float angleLast1=1;
    private float angleLast2;
    private float angleLast3;
    private float angleLast4;
    private Vector3 vLast;
    // Start is called before the first frame update
    void Start()
    {
        lines = new LineRenderer[5];//添加组件
        InvokeRepeating("RepeatedMethod", 1f, 0.4f);
    }
    private void RepeatedMethod()
    {
       // float d = culculate(index2, thumb2, thumb1);
        Vector3 v = thumb2.transform.position - thumb3.transform.position;
        // float d = Vector3.Dot(v, hand.transform.up) / (v.magnitude * hand.transform.up.magnitude);
        //float d = (thumb0.transform.position - little2.transform.position).magnitude;
        float d = culculate(thumb1, thumb2, thumb3);
        angleLast = d;
        //T2.text  = angleLast1.ToString();
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
    public GameObject test;
    int mark = 0;
    // Update is called once per frame
    void Update()
    {   //手指沿手指关节发出射线，指尖和第一个指节
        //
        //T3.text = (angleLast1-culculate(index1, index2, index3)).ToString();
        Vector3 v1 = thumb2.transform.position - thumb3.transform.position;
        Vector3 v2 = index2.transform.position - thumb3.transform.position;
        //Vector3 v = new Vector3(v1.y * v2.z - v2.y * v1.z, v2.x * v1.z - v1.x * v2.z, (v1.x * v2.y - v2.x * v1.y));
        Vector3 v = thumb1.transform.position - thumb2.transform.position;
        //float d = Vector3.Dot(v, hand.transform.up) / (v.magnitude * hand.transform.up.magnitude);
        float d=culculate(thumb1, thumb2, thumb3);
         T.text = (d - angleLast).ToString();
        //float d = (thumb0.transform.position - little2.transform.position).magnitude;
        //T2.text = d.ToString();
       // T3.text = hand.transform.up.y.ToString();
        //T4.text = hand.transform.up.z.ToString();
        if (d-angleLast>0.07)
        {
        T4.text = "yes";
        }
        else
        {
           T4.text = "no";
        }
        
        /* float d1 = culculate(index1, index2, index3);
        if (angleLast1 - d1 > 0.2 || angleLast1 - d1<-0.2)
        {
            T.text = "Yes";
            Load_Scripts<Outline>(test);
        }
        else
        { T.text = "No";
            Unload_Scripts<Outline>(test);
        }
        
       float d2 = culculate(middle1, middle2, middle3);
        if (angleLast2 - d2 > 0.2 || angleLast2 - d2 < -0.2)
        {
            T2.text = "Yes";
            Load_Scripts<Outline>(test);
        }
        else
        {
            T2.text = "No";
            Unload_Scripts<Outline>(test);
        }
        float d3 = culculate(ring1, ring2, ring3);
        if (angleLast3 - d3 > 0.2 || angleLast3 - d3 < -0.2)
        {
            T3.text = "Yes";
            Load_Scripts<Outline>(test);
        }
        else
        {
            T3.text = "No";
            Unload_Scripts<Outline>(test);
        }
        float d4 = culculate(little1, little2, little3);
        if (angleLast4 - d4 > 0.2 || angleLast4 - d4 < -0.2)
        {
            T4.text = "Yes";
            Load_Scripts<Outline>(test);
        }
        else
        {
            T4.text = "No";
            Unload_Scripts<Outline>(test);
        }
         Ray ray0 = new Ray(thumb0.transform.position, thumb0.transform.position - thumb1.transform.position);
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
