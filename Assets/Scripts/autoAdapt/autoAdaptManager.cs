using UnityEngine;
using TMPro;
public class autoAdaptManager : MonoBehaviour
{
    //�������ƵĽű�
    public GameObject recorder;//Ϊ��ʹʱ��һ�£�����recorder��timer
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
         //   beginTime.text = "Ready";//����ʱ��
            maxMark = 0;
            minMark = 0;    
        }
        else if(recorder.GetComponent<singleSelect>().timer >= 1){//��1-6s֮�ڣ������ֲ����ſ�
            var temp = HandRightWrist.transform.rotation.eulerAngles.x;
           // beginTime.text = (6-recorder.GetComponent<singleSelect>().timer).ToString();
            if (temp>180){//��ʱ��ˮƽ������б
                temp = 360 - temp;//������б�ĽǶ�
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

        if(recorder.GetComponent<singleSelect>().timer == 1f)//��ʼ��һ�뿪ʼ
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
    //��ʼ���룬���������ſ�����ƽ���Ƕ�-->������͵��������ſ�����ֵ
    void changeBackboard()
    {
        //����

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
