using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class clickSelect : MonoBehaviour
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
    public TMP_Text t;
    private GameObject SightCone;
    public GameObject FinalObjects;

    float timer = 0;
    public float clickPause = 0.1f;
    void Start()
    {
        StartCoroutine(GetFingerAngle());
        SightCone = GameObject.Find("SightCone");
    }
    public IEnumerator GetFingerAngle()
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

            if (gap[0] < -9) { mark[0] = true; t.text = "1"; }
            else { mark[0] = false; t.text = "oo"; }
            if (gap[1] < -13) { mark[1] = true; t.text = "2"; }
            else { mark[1] = false; t.text = "oo"; }
            if (gap[2] < -13) { mark[2] = true; t.text = "3"; }
            else { mark[0] = false; t.text = "oo"; }

            // 等待0.1秒
            yield return new WaitForSeconds(0.05f);
        }
    }
    void Update()
    {
        
        timer += Time.deltaTime;
        if(HandPoseManager.GetComponent<HandPoseManager>().PalmPoseState)
            selectedRow = HandPoseManager.GetComponent<HandPoseManager>().selectedRow;
        else 
            selectedRow = null;
        t.text = "1";
        if (selectedRow != null)
        {
            t.text = "2";
            if (timer > clickPause && HandPoseManager.GetComponent<HandPoseManager>().SecondSelectionState && HandPoseManager.GetComponent<HandPoseManager>().PalmPoseState)
            {
                for (int i = 0; i < 3; i++)
                {
                    if(mark[i]){
                        t.text = i.ToString();
                        if (selectedRow[i]!=null && !FinalObjects.GetComponent<FinalObjects>().finalObj.Contains(selectedRow[i]) && mark[i] && selectedRow[i] != HandPoseManager.GetComponent<HandPoseManager>().emptyBlock)
                        {
                            FinalObjects.GetComponent<FinalObjects>().AddFinalObj(selectedRow[i]);
                            SightCone.GetComponent<SightCone>().objectWeights.Remove(selectedRow[i]);
                            timer = 0;
                        }
                    }
                }
            
            }
        }
    }
}
