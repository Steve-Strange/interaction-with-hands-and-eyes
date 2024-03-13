using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugSize : MonoBehaviour
{
    // Start is called before the first frame update

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("localScale: " + gameObject.transform.localScale);
        Debug.Log("lossyScale: " + gameObject.transform.lossyScale);
        Mesh mesh = gameObject.GetComponent<MeshFilter>().mesh;
        Vector3 realScale = gameObject.transform.localScale;
        Vector3 realSize = new Vector3(mesh.bounds.size.x * realScale.x, mesh.bounds.size.y * realScale.y, mesh.bounds.size.z * realScale.z);
        Debug.Log("realSize: " + realSize);
    }
}
