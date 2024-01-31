using System.Runtime.Intrinsics;
using System.Numerics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collide : MonoBehaviour
{
    public TMPro.TMP_Text t;
    public GameObject pinch;
    public GameObject frame;
    private pinch p;
    public string Frame;//the mark of frame
    public GameObject FinalObjects;
    public List<GameObject> finalObj;//get all objects from last level
    
    public List<GameObject> anchor;//all anchor object,hilight them,we can only contact with them

    public List<GameObject> onFrame;//object already on now frame
    // Start is called before the first frame update

    private List<GameObject> rectCorner;// leftup rightup rightdown leftdown
    private List<GameObject> triCorner;// leftup rightup rightdown leftdown
    private List<GameObject> circle;// 圆用三个三角形
    private Vector3[] rect;
    private Vector3[] para;
    private Vector3[] tri;
    private void OnCollisionEnter(Collision collision){

        ContactPoint contact = collision.contacts[0];                                                                                                                                                      
        t.text = collision.gameObject.name;
        
        if(p.ispinch)//keep pinch,keep manipulate,todo make p more wending
        { 
          finalObj[0].transform.position = contact.point;
          finalObj[0].transform.parent = collision.gameObject.transform;
          
          if(frame.GetComponent<frame>().Frame == "rect"){
            rect = frame.GetComponent<frame>().rectCorner;
            for(int i = 0 ;i <= 3 ;i++)
            if((finalObj[0].transform.position-rect[i]).magnitude < 0.01)
            {
                finalObj[0].transform.position = rect[i];
                rectCorner[i] = finalObj[0];
            }
          }

          if(frame.GetComponent<frame>().Frame == "tri"){
            tri = frame.GetComponent<frame>().triCorner;
            for(int i = 0 ;i <= 2 ;i++)
            if((finalObj[0].transform.position-tri[i]).magnitude < 0.01)
            {
                finalObj[0].transform.position = tri[i];
                triCorner[i] = finalObj[0];
            }
          }          
          
          
          onFrame.Add(finalObj[0]);
          finalObj.RemoveAt(0);
        }
    }
    void Start()
    {
        p = pinch.GetComponent<pinch>();
        finalObj =  FinalObjects.GetComponent<FinalObjects >().finalObj;
    }
    void anchorChoose()
    {
        anchor.Clear();
        if(Frame == "rect"){
        // select three object by distance ，add position correct in the corner（make object right at the corner）
        if(rectCorner[0] && rectCorner[2])
        {
          anchor.Add(rectCorner[0]);
          anchor.Add(rectCorner[2]);
        }
        else if(rectCorner[1] && rectCorner[3]){
          anchor.Add(rectCorner[1]);
          anchor.Add(rectCorner[3]);
        }
        }

        //foreach i in anchor{
            //高亮
        //}
    }
    // Update is called once per frame
    void Update()
    {
        
    }


    private Vector3 CalculateTriangleOutCircleCenter(Vector3 A, Vector3 B, Vector3 C)
    {
        float Xa = A.x;
        float Ya = A.y;
        float Za = A.z;
 
        float Xb = B.x;
        float Yb = B.y;
        float Zb = B.z;
 
        float Xc = C.x;
        float Yc = C.y;
        float Zc = C.z;
 
        Vector3 D = (A + C) / 2;
        float Xd = D.x;
        float Yd = D.y;
        float Zd = D.z;
 
        //单位法向量AN
        Vector3 AB = B - A;
        Vector3 AC = C - A;
        Vector3 AN = Vector3.Cross(AB, AC).normalized;
 
        float u = AN.x;
        float v = AN.y;
        float w = AN.z;
 
        //构建三元一次方程参数
        float a = u;
        float b = v;
        float c = w;
        float d = u * Xa + v * Ya + w * Za;
 
        float e = Xc - Xa;
        float f = Yc - Ya;
        float g = Zc - Za;
        float h = (Xc - Xa) * (Xc + Xa) / 2 + (Yc - Ya) * (Yc + Ya) / 2 + (Zc - Za) * (Zc + Za) / 2;
 
        float k = 2 * Xb - 2 * Xa;
        float l = 2 * Yb - 2 * Ya;
        float m = 2 * Zb - 2 * Za;
        float n = Xb * Xb - Xa * Xa + Yb * Yb - Ya * Ya + Zb * Zb - Za * Za;
 
        float[] equa = CalculateTernaryEquation(a, b, c, d, e, f, g, h, k, l, m, n);
        Vector3 P = new Vector3(equa[0], equa[1], equa[2]);
        return P;
    }
 
    private float[] CalculateTernaryEquation(float a, float b, float c, float d, float e, float f, float g, float h, float k, float l, float m, float n)
    {
        float z = ((d * e - a * h) * (f * k - e * l) - (h * k - e * n) * (b * e - a * f)) / ((c * e - a * g) * (f * k - e * l) - (b * e - a * f) * (g * k - e * m));
        float y = ((d * e - a * h) * (g * k - e * m) - (h * k - e * n) * (c * e - a * g)) / ((b * e - a * f) * (g * k - e * m) - (f * k - e * l) * (c * e - a * g));
        float x = 0;
        if (a != 0)
            x = (d - b * y - c * z) / a;
        else if (e != 0)
            x = (h - f * y - g * z) / e;
        else if (k != 0)
            x = (n - l * y - m * z) / k;
        return new float[] { x, y, z };
    }

}
