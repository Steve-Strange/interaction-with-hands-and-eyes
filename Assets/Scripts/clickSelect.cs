using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class clickSelect : MonoBehaviour
{
    public GameObject thumb0;
    public GameObject thumb1;
    public GameObject thumb2;
    public GameObject thumb3;
    public GameObject index0;
    public GameObject index1;
    public GameObject index2;
    public GameObject index3;
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
    // Start is called before the first frame update
    void Start()
    {
        lines = new LineRenderer[5];//添加组件
        InvokeRepeating("RepeatedMethod", 1f, 0.5f);
    }
    private void RepeatedMethod()
    {
        angleLast1 = culculate(index1, index2, index3);
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
    // Update is called once per frame
    void Update()
    {   //手指沿手指关节发出射线，指尖和第一个指节
        if (angleLast1 - culculate(index1, index2, index3) > 0.2)
        {
            T.text = "Yes";
            Load_Scripts<Outline>(test);
        }
        else
        { T.text = "No";
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

    

    }
}
