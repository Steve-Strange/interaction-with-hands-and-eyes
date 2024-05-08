using UnityEngine;
public class autoAdaptManager : MonoBehaviour
{
    //控制手势的脚本
    // Start is called before the first frame update
    public GameObject recorder;//为了使时间一致，借用recorder的timer
    public GameObject index1;
    public GameObject index2;
    public GameObject middle1;
    void Start()
    {
        
    }
    private float angleGap = 0f;
    private int mark = 0;
    private float maxAngle = 0;
    private float minAngle = 0;
    // Update is called once per frame
    void Update()
    {
        if(recorder.GetComponent<singleSelect>().timer == 1f)//开始后一秒开始
        {
            mark = 0;
            angleGap = 0f;
            maxAngle = 0;
            minAngle = 0;
        }
        else if(recorder.GetComponent<singleSelect>().timer > 1f && recorder.GetComponent<singleSelect>().timer < 3f)
        {
            mark += 1;
            /*angleGap += 检测手指间的角度 
            if(){
               maxAngle = ;
            }
            else if ()
                    {
            minAngle = ;

                    }*/
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
