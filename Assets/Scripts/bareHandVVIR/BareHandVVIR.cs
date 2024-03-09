using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BareHandVVIR : MonoBehaviour
    {//���Գ��Ժ�����������ע��

        // public GameObject mainCameraObject; // VR�������,��Ҫ����
        private LineRenderer line;
        public GameObject select;

        //�ҵ���������
        public GameObject rightHand; // ��Ҫ����

        // ���ֲ����ռ������Ϣ
        public float userArmLength; // �û��ֱ۳��ȣ���Ҫ����
        public float userBoomLength; // �û���۳��ȣ����ڼ�������λ��

        // �ӿ������Ϣ
        private Camera mainCamera;
        private float halfFov; // ��ֱFOVһ�룬����
        private float aspectFov; // ��׶��Ŀ�߱�
        public float farPlaneValue;

        // ������ΪHead���ֱ�������
        public GameObject localHandle;

        /// <summary>
        /// ����������ر���
        /// </summary>
        // 1.�ֱ�������ȡ���λ��
        public GameObject userShoulder; // �û���������Ҫ����
        //public SteamVR_Action_Boolean selectUserShoulder; //��ΪԤ����
        private bool isUserShoulderSampled;
        // 2.�ֱ���������ͷ�����򣨼��Ϊ�������壩
        public GameObject userHead;
        //public SteamVR_Action_Boolean updateUserHead; // ���µ��ֲ����ռ䳯��
        // 3.�ֱ����������ӿڳ���
        //2,3��Ϊ˫pinch�Զ���
        private List<Vector3> cameraInfo; // ���ꡢǰ�����Ϸ����ҷ�
        //public SteamVR_Action_Boolean updateUserFov; // ���������λ�úͳ���
        //��������գ����ѡ��
        public GameObject targetObject;
       
       

        // ������ؽű�
        // �۲�������Ϣ
        public GameObject GazeRaySample;
        //private SRanipal_GazeRaySample_v2 sRanipalGazeSample;

        // ������ر���
        //public GameObject standardObject; // ���ڼ�������Ŀ������
        //public bool isSampleData = false;
        //private SMSampler smSamplerScript; // �����ű�


        void Start()
        {
            mainCamera = Camera.main;
            line = GetComponent<LineRenderer>();
            halfFov = (mainCamera.fieldOfView * 0.5f) * Mathf.Deg2Rad;
            aspectFov = mainCamera.aspect;

            //manipulateStatus = ManipulateStatus.UNSELECTED;
            isUserShoulderSampled = false;

            cameraInfo = new List<Vector3>(new Vector3[4]);
            //Debug.Log("camerainfo:" + cameraInfo.Count);

            // ��ʼ�����ӽű�
            //sRanipalGazeSample = GazeRaySample.GetComponent<SRanipal_GazeRaySample_v2>();
            // ��ʼ�������ű�
            //if (isSampleData)
           // {
           //     smSamplerScript = GetComponent<SMSampler>();
           // }



        }
        public GameObject rightPinch;
        public GameObject leftPinch;
    // Update is called once per frame
    void Update()
        {
            UpdateLocalHandle();//localhandle�����ֱ�λ��ͬ��
        if (rightPinch.GetComponent<pinch>().ispinch) { 
            GetShoulderPosition();//�ĳɹ̶�������
                                  }
            if(rightPinch.GetComponent<pinch>().ispinch && leftPinch.GetComponent<pinch>().ispinch) { 
            UpdateSHSForward();//����b

            UpdateUserFov();//����A
}
            /* ��ʱ��Ŀ������
            if (select.GetComponent<>().)
            {
            targetObject = select.GetComponent<>().;
            }else
        {
            targetObject = null;
        }*/
            TranslateObjectBySyncMapping();

            // ��������

            //SampleManipulateData();
        }

        /// <summary>
        /// �����ֱ����ƶ�����������꣬���ֱ�ͬ��
        /// </summary>
        void UpdateLocalHandle()
        {
            localHandle.transform.position = rightHand.transform.position;
        }

        /// <summary>
        /// ������ȡ�����������
        /// </summary>
        void GetShoulderPosition()
        {
          
                // if (!isUserShoulderSampled)
                {
                    // ��ȡ��ʱ�������������겢��ֵ�����
                    userShoulder.transform.position = rightHand.transform.position;
                    isUserShoulderSampled = true;
                    Debug.Log("�ɹ���ȡ�������");
                }
            
        }

        /// <summary>
        /// ͨ�����������ֲ����ռ�ĳ�����VR�������ͬ��
        /// </summary>
        void UpdateSHSForward()
        {
                // ֻͬ��y���x���ƫת��z�᲻����
                Vector3 eyeEulerAngle = new Vector3(0, mainCamera.transform.eulerAngles.y, mainCamera.transform.eulerAngles.z);
                userHead.transform.eulerAngles = eyeEulerAngle;
                // ͬ���ռ�λ��
                userHead.transform.position = mainCamera.transform.position;
                Debug.Log("�����˵��ֲ����ռ�ĳ���");        
        }

        /// <summary>
        /// ͨ���������û���Ұ������VR�������ͬ��
        /// </summary>
        void UpdateUserFov()
        {
                cameraInfo[0] = mainCamera.transform.position;
                cameraInfo[1] = mainCamera.transform.forward;
                cameraInfo[2] = mainCamera.transform.right;
                cameraInfo[3] = mainCamera.transform.up;
                Debug.Log("�������û�FOV��Transform:");        
        }





        /// <summary>
        /// �����ֱ��ֲ����������������ڳ��������ռ��е�λ��
        /// ���ֲ����ռ�Ϊ����
        /// </summary>
        /// <param name="p_h">�ֱ�����</param>
        /// <returns>�������������</returns>
        Vector3 GetSMSObjectPosition(Vector3 p_h)
        {
            float d_np = mainCamera.nearClipPlane; // distance_near_plane
            float d_fp = mainCamera.farClipPlane; // distance_far_plane
            d_fp = 7f; // ����ʱ�涨
            float d_fn = d_fp - d_np; // ƽ��ͷ��ĸ�
            float l_arm = userArmLength;
            Vector3 p_s = userShoulder.transform.localPosition;

            Vector3 p_c = cameraInfo[0]; // ���λ��

            //Debug.Log("p_s:" + p_s.ToString());
            //Debug.Log("p_h:" + p_h.ToString());

            // ����zo
            float z_rate = Mathf.Abs(p_h.z - p_s.z) / l_arm;
            float z_o; // ������֮����Ҫ���Է�������
            if (p_h.z > p_s.z)
            {
                z_o = d_np + d_fn * z_rate;
            }
            else
            {
                z_o = d_np - d_fn * z_rate;
            }

            // ����z_o������Ļ����
            float height_o = z_o * Mathf.Tan(halfFov);
            float width_o = height_o * aspectFov;


            // ����xo
            float l_yx = Mathf.Sqrt(Mathf.Pow(l_arm, 2) - Mathf.Pow(p_h.y - p_s.y, 2) - Mathf.Pow(p_h.z - p_s.z, 2));
            //Debug.Log("l_yx:" + l_yx.ToString());
            float rate_h_x = Mathf.Abs(p_h.x - p_s.x) / l_yx;
            //Debug.Log("rate_h_x:" + rate_h_x.ToString());
            float x_o;
            if (p_h.x > p_s.x)
            {
                x_o = (width_o / 2f) * rate_h_x;
            }
            else
            {
                x_o = -(width_o / 2f) * rate_h_x;
            }

            // ����yo
            float l_xz = Mathf.Sqrt(Mathf.Pow(l_arm, 2) - Mathf.Pow(p_h.y - p_s.y, 2) - Mathf.Pow(p_h.x - p_s.x, 2));
            float rate_h_y = Mathf.Abs(p_h.y - p_s.y) / l_xz;
            float y_o;

            if (p_h.y > p_s.y)
            {
                y_o = (height_o / 2f) * rate_h_y;
            }
            else
            {
                y_o = -(height_o / 2f) * rate_h_y;
            }

            // ����p_o
            Vector3 p_o = p_c + cameraInfo[1] * z_o + cameraInfo[2] * x_o + cameraInfo[3] * y_o;
            return p_o;

        }

        /// <summary>
        /// �����ֱ��ֲ���������������λ�ã����ֲ����ռ�Ϊ��������
        /// </summary>
        /// <param name="p_h"></param>
        /// <returns></returns>
        Vector3 GetSMSObjectPositionHemi(Vector3 p_h)
        {
            float d_np = mainCamera.nearClipPlane; // distance_near_plane
            float d_fp = mainCamera.farClipPlane; // distance_far_plane
            d_fp = farPlaneValue; // ����ʱ�涨
            float d_fn = d_fp - d_np; // ƽ��ͷ��ĸ�
            float r_arm = 1.1f * (userArmLength - userBoomLength); // �ʵ��Ŵ�userArmLength

            Vector3 p_shoulder = userShoulder.transform.localPosition; // ���ֲ�����
            Vector3 p_elbow = p_shoulder - userBoomLength * Vector3.down; // ��������=�������-��۳���*(0,-1,0)
            Vector3 p_camera = cameraInfo[0]; // ���λ��

            // ����zo

            float z_distance = p_h.z - p_elbow.z;
            float z_abs_dis;
            if (z_distance > r_arm)
            {
                z_abs_dis = r_arm;
            }
            else if (z_distance > 0)
            {
                z_abs_dis = z_distance;
            }
            else
            {
                z_abs_dis = 0;
            }

            float z_rate = z_distance / r_arm;

            float z_object = d_np + d_fn * z_rate;

            // ����z_object������Ļ����
            float height_object = z_object * Mathf.Tan(halfFov);
            float width_object = height_object * aspectFov;



            float r_xy_2 = Mathf.Pow(r_arm, 2) - Mathf.Pow(r_arm - z_abs_dis, 2);
            if (r_xy_2 < 0) { Debug.LogError("ӳ��Բ��뾶����С��0,���������ʼ��.."); }
            float r_xy = Mathf.Sqrt(r_xy_2); // ӳ��Բ��뾶

            // ����x_object
            float x_abs_dis = Mathf.Abs(p_h.x - p_elbow.x);
            float x_rate;
            if (x_abs_dis > r_xy)
            {
                x_rate = 1;
            }
            else
            {
                x_rate = x_abs_dis / r_xy;
            }

            float x_object;
            if (p_h.x > p_elbow.x)
            {
                x_object = (width_object / 2f) * x_rate;
            }
            else
            {
                x_object = -(width_object / 2f) * x_rate;
            }

            // ����y_object

            float y_abs_dis = Mathf.Abs(p_h.y - p_elbow.y);
            float y_rate;

            if (y_abs_dis > r_xy)
            {
                y_rate = 1;
            }
            else
            {
                y_rate = y_abs_dis / r_xy;
            }

            float y_object;
            if (p_h.y > p_elbow.y)
            {
                y_object = (height_object / 2f) * y_rate;
            }
            else
            {
                y_object = -(height_object / 2f) * y_rate;
            }

            // ����p_o
            Vector3 p_object = p_camera + cameraInfo[1] * z_object + cameraInfo[2] * x_object + cameraInfo[3] * y_object;
            return p_object;
        }


        /// <summary>
        /// ������ֱ�����λ��ӳ�䣬�������ռ��н���ͬ��
        /// </summary>
        void TranslateObjectBySyncMapping()
        {
            if (targetObject!=null)
            {
                targetObject.transform.position = GetSMSObjectPositionHemi(localHandle.transform.localPosition);//�����ֱ���λ�ý����ƶ�
            }
        }
    }


