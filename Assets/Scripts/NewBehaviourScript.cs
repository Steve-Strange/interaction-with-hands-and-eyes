using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public TMP_Text rotation1;
    public TMP_Text rotation2;
    public TMP_Text rotation3;
    public GameObject xrrig;
    public GameObject camera_off;
    public GameObject camera;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rotation1.text = xrrig.transform.rotation.y.ToString();
        rotation2.text = camera_off.transform.rotation.y.ToString();
        rotation3.text = camera.transform.rotation.y.ToString();
    }
}
