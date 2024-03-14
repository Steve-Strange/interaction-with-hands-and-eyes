using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public List<GameObject> objects = new List<GameObject>();
    public GameObject Objects;
    // Start is called before the first frame update
    void Start()
    {
        objects = new List<GameObject>();
        foreach (Transform t in Objects.GetComponentsInChildren<Transform>())
        {
            objects.Add(t.gameObject);
            Debug.Log(t.gameObject.name);
        }

    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("AAA");
        for (int i = 0; i < objects.Count; i++)
        {
            Debug.Log(objects[i].GetComponent<MeshFilter>().sharedMesh.vertices.Length.ToString());
        }
    }
}
