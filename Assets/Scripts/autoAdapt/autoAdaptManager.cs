using UnityEngine;
public class autoAdaptManager : MonoBehaviour
{
    //�������ƵĽű�
    // Start is called before the first frame update
    public GameObject recorder;//Ϊ��ʹʱ��һ�£�����recorder��timer
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
        if(recorder.GetComponent<singleSelect>().timer == 1f)//��ʼ��һ�뿪ʼ
        {
            mark = 0;
            angleGap = 0f;
            maxAngle = 0;
            minAngle = 0;
        }
        else if(recorder.GetComponent<singleSelect>().timer > 1f && recorder.GetComponent<singleSelect>().timer < 3f)
        {
            mark += 1;
            /*angleGap += �����ָ��ĽǶ� 
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
