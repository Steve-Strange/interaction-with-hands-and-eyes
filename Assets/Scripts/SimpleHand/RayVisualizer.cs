using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class RayVisualizer : MonoBehaviour
{
    //  注意，物体要有碰撞箱
    // Start is called before the first frame update
    private LineRenderer line;
    public GameObject finger1;
    public GameObject finger2;
    public GameObject target;
    //public TMP_Text T;
    int layerMask;

    void Start()
    {
        line = GetComponent<LineRenderer>();
        line.startWidth = 0.01f;
        line.endWidth = 0.01f;
        layerMask = 1 << 7;
        //layerMask = ~layerMask;
    }

    // Update is called once per frame
    void Update()
    {
        var direction = finger1.transform.position - finger2.transform.position;
        Ray ray = new Ray(finger2.transform.position, direction);
        line.SetPosition(0, finger2.transform.position);
        line.SetPosition(1, finger2.transform.position + 100 * direction);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit,1000 ,layerMask))
        {
            line.SetPosition(1, hit.transform.position);
            if(target!=null && target!=hit.transform.gameObject){
                AddOutline(target, Color.clear);
            }
            
            target = hit.transform.gameObject;
            // T.text = "yes";
            AddOutline(target, Color.red);
        }
        else{
            // T.text = "no";
            AddOutline(target, Color.clear);
            line.SetPosition(1, finger2.transform.position + 100 * direction);
            target = null;
        }

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
