using UnityEngine;
using TMPro;
public class autoAdaptManager : MonoBehaviour
{
    //控制手势的脚本
    public GameObject recorder;//为了使时间一致，借用recorder的timer
    public GameObject HandRightWrist;
    public GameObject handPoseManager;
    public GameObject index1;
    public GameObject index2;
    public GameObject middle1;
   // public TMP_Text beginTime;
   // public TMP_Text angle;
   // public TMP_Text t1;
   // public TMP_Text t2;
    void Start()
    {
       // beginTime.text = 5.ToString();
    }
    private float angleGap = 0f;
    private int mark = 0;
    private int maxMark = 0;
    private int minMark = 0;
    private float maxWristAngle = 0;
    private float minWristAngle = 0;
    private float maxWristAngles = 0;
    private float minWristAngles = 0;
    // Update is called once per frame
    void Update()
    {
    
        if (recorder.GetComponent<singleSelect>().timer > 0.01){//6){
            handPoseManager.GetComponent<HandPoseManagerSelectOnly>().minAngel = -10;// minWristAngles / minMark;
            handPoseManager.GetComponent<HandPoseManagerSelectOnly>().maxAngel = 60;// maxWristAngles / maxMark;
        }
        else if(recorder.GetComponent<singleSelect>().timer < 1)
        {
         //   beginTime.text = "Ready";//缓冲时间
            maxMark = 0;
            minMark = 0;    
        }
        else if(recorder.GetComponent<singleSelect>().timer >= 1){//在1-6s之内，调整手部和张开
            var temp = HandRightWrist.transform.rotation.eulerAngles.x;
           // beginTime.text = (6-recorder.GetComponent<singleSelect>().timer).ToString();
            if (temp>180){//此时是水平向上倾斜
                temp = 360 - temp;//向上倾斜的角度
                if (temp > maxWristAngle)
                {
                    maxWristAngle = temp;
                    maxWristAngles += maxWristAngle;
                    maxMark += 1;
                }
            }else{
                temp = 0 - temp;
                if (temp < minWristAngle)
                {
                        minWristAngle = temp;
                        minWristAngles += minWristAngle;
                        minMark += 1;
                }
            }

        }

        if(recorder.GetComponent<singleSelect>().timer == 1f)//开始后一秒开始
        {
            mark = 0;
            angleGap = 0f;
            maxWristAngle = 0;
            minWristAngle = 0;
        }
        else if(recorder.GetComponent<singleSelect>().timer > 1f && recorder.GetComponent<singleSelect>().timer < 3f)
        {
            mark += 1;
        }else if(recorder.GetComponent<singleSelect>().timer >= 3f)
        {
            changeBackboard();
            changeAngle();
        }

    }
    //开始三秒，保持手掌张开，测平均角度-->大于其就当作手掌张开的阈值
    void changeBackboard()
    {
        //计算

    }
    void changeAngle()
    {

    }
    float claculateTheAngle(Vector3 one, Vector3 two,Vector3 three)
    {
        var dot = Vector3.Dot((one - two),three-two)/((one-two).magnitude*(three-two).magnitude);
        return Mathf.Acos(dot) * Mathf.Rad2Deg;
    }
}
