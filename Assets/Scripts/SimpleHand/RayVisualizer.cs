using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayVisualizer : MonoBehaviour
{
    // Start is called before the first frame update
    private LineRenderer line;
    public GameObject finger1;
    public GameObject finger2;
    public GameObject target;
    void Start()
    {
        line = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray(finger1.transform.position, finger1.transform.position - finger2.transform.position);
        line.SetPosition(0, finger1.transform.position);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, int.MaxValue))
        {
            line.SetPosition(1, hit.transform.position);
        
            target = hit.transform.gameObject;
            
        }
        else
        {
            line.SetPosition(1, finger1.transform.position + 100 * (finger1.transform.position - finger2.transform.position));
            target = null;
        }
    }
}
