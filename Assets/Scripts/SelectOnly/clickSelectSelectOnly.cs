using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;
using static UnityEngine.ParticleSystem;

public class clickSelectSelectOnly : MonoBehaviour
{
    private List<GameObject> selectedRow = new List<GameObject>();
    public GameObject Palm;
    public GameObject HandPoseManager, thumb0, thumb3,
        index0, index3,
        middle0, middle3;

    private float[] angle = new float[5];
    private float[] angleLast = new float[5];
    private float[] gap = new float[5];
    private bool[] mark = new bool[5];
    public RaycastHit thumb, index, middle, ring, little;
    // public TMP_InputField log;
    // public TMP_Text  T2, T3, T4, T5, T6;
    private GameObject SightCone;
    public GameObject FinalObjects;

    float timer = 0;
    public float clickPause = 0.1f;
    void Start()
    {
        StartCoroutine(GetFingerAngle());
        SightCone = GameObject.Find("SightCone");
    }

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

            // log.text = "Thumb: " + gap[0] + "\nIndex: " + gap[1] + "\nMiddle: " + gap[2];

            angleLast[0] = angle[0];
            angleLast[1] = angle[1];
            angleLast[2] = angle[2];

            if(gap[0]<-9) mark[0] = true;
            else mark[0] = false;
            if(gap[1]<-15) mark[1] = true;
            else mark[1] = false;
            if(gap[2]<-15) mark[2] = true;
            else mark[2] = false;

            // 等待0.1秒
            yield return new WaitForSeconds(0.05f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        // log.text = mark[0] + " " + mark[1] + " " + mark[2];
        selectedRow = HandPoseManager.GetComponent<HandPoseManagerSelectOnly>().selectedRow;

        if (timer > clickPause && HandPoseManager.GetComponent<HandPoseManagerSelectOnly>().SecondSelectionState && HandPoseManager.GetComponent<HandPoseManagerSelectOnly>().PalmPoseState)
        {
            for (int i = 0; i < 3; i++)
            {
                if(mark[i]){
                    if (!FinalObjects.GetComponent<FinalObjectsSelectOnly>().finalObj.Contains(selectedRow[i]) && mark[i] && selectedRow[i] != HandPoseManager.GetComponent<HandPoseManagerSelectOnly>().emptyBlock)
                    {
                        FinalObjects.GetComponent<FinalObjectsSelectOnly>().AddFinalObj(selectedRow[i]);
                        SightCone.GetComponent<SightConeSelectOnly>().objectWeights.Remove(selectedRow[i]);
                        timer = 0;
                    }
                }
            }
            
        }
    }
}
