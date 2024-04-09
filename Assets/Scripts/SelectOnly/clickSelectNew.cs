using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class clickSelectNew : MonoBehaviour
{
    private List<GameObject> selectedRow = new List<GameObject>();
    public GameObject Palm;
    public GameObject HandPoseManager, thumb0, thumb3,
        index0, index3,
        middle0, middle3;

    private float[] angle = new float[5];
    private float[] angleLast = new float[5];
    private float[] gap = new float[5];
    public RaycastHit thumb, index, middle, ring, little;
    public TMP_InputField log;
    // public TMP_InputField log;
    private GameObject SightCone;

    //private float angle, angle1, angle2, angle3, angle4, angleLast;
    //private float angleLast1 = 1, angleLast2, angleLast3, angleLast4;


    public GameObject FinalObjects;
    void Start()
    {
        //StartCoroutine("UpdateVelocity");
        SightCone = GameObject.Find("SightCone");
        StartCoroutine(GetFingerAngle());
    }
    // Update is called once per frame

    private IEnumerator GetFingerAngle()
    {
        while (true)
        {
            // 这里写你要每隔0.1秒执行一次的代码
            angle[0] = Vector3.Angle(Palm.transform.up, thumb0.transform.position - thumb3.transform.position);
            angle[1] = Vector3.Angle(Palm.transform.up, index0.transform.position - index3.transform.position);
            angle[2] = Vector3.Angle(Palm.transform.up, middle0.transform.position - middle3.transform.position);

            gap[0] = angleLast[0] - angle[0];
            gap[1] = angleLast[1] - angle[1];
            gap[2] = angleLast[2] - angle[2];

            log.text = "Thumb: " + gap[0] + "\nIndex: " + gap[1] + "\nMiddle: " + gap[2];

            angleLast[0] = angle[0];
            angleLast[1] = angle[1];
            angleLast[2] = angle[2];

            // 等待0.1秒
            yield return new WaitForSeconds(0.1f);
        }
    }
    void Update()
    {

    }
}
