using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BareHandVVIR : MonoBehaviour
    {//可以尝试后续加上眼神注视

        public GameObject bubble;
        public GameObject mainCameraObject; // VR相机对象,需要拖入

        //我的虚拟手掌
        public GameObject rightHand; // 需要拖入

        // 单手操作空间相关信息
        public float userArmLength; // 用户手臂长度，需要输入
        public float userBoomLength; // 用户大臂长度，用于计算手肘位置

        // 视口相关信息
        private Camera mainCamera;
        private float halfFov; // 垂直FOV一半，弧度
        private float aspectFov; // 视锥体的宽高比
        public float farPlaneValue;

        // 父物体为Head的手柄对象复制
        private GameObject localHandle;

        /// <summary>
        /// 操作流程相关变量
        /// </summary>
        // 1.手柄按键获取肩膀位置
        private GameObject userShoulder; // 用户肩膀对象，需要拖入
        
        // 2.手柄按键更新头部朝向（肩膀为其子物体）
        private GameObject userHead;
        //public SteamVR_Action_Boolean updateUserHead; // 更新单手操作空间朝向
        // 3.手柄按键更新视口朝向
        //2,3改为双pinch自动化
        private List<Vector3> cameraInfo; // 坐标、前方、上方、右方
        //public SteamVR_Action_Boolean updateUserFov; // 更新主相机位置和朝向
        //利用射线眨眼来选择
        private GameObject targetObject;
       
       

        // 凝视相关脚本
        // 眼部跟踪信息
        //public GameObject GazeRaySample;
        //private SRanipal_GazeRaySample_v2 sRanipalGazeSample;

        // 采样相关变量
        //public GameObject standardObject; // 用于计算误差的目标物体
        //public bool isSampleData = false;
        //private SMSampler smSamplerScript; // 采样脚本


        void Start()
        {
            mainCamera = Camera.main;
            halfFov = (mainCamera.fieldOfView * 0.5f) * Mathf.Deg2Rad;
            aspectFov = mainCamera.aspect;

          

            cameraInfo = new List<Vector3>(new Vector3[4]);
           

            // 初始化凝视脚本
            //sRanipalGazeSample = GazeRaySample.GetComponent<SRanipal_GazeRaySample_v2>();
            // 初始化采样脚本
            //if (isSampleData)
           // {
           //     smSamplerScript = GetComponent<SMSampler>();
           // }



        }
        public GameObject rightPinch1;
        public GameObject rightPinch2;
        public GameObject leftPinch1;
        public GameObject leftPinch2;
    bool ispinch(GameObject a, GameObject b)
    {

        if((a.transform.position-b.transform.position).magnitude < 0.01)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    void Update()
        {
            UpdateLocalHandle();//localhandle与右手柄位置同步
        if (ispinch(rightPinch1,rightPinch2)) { 
            GetShoulderPosition();//改成固定的向量
                                  }
            if(ispinch(rightPinch1, rightPinch2) && ispinch(leftPinch1, leftPinch2)) { 
            UpdateSHSForward();//右手b

            UpdateUserFov();//右手A
}
            /* 此时有目标物体被选中*/
            if (bubble.GetComponent<Bubble>().selectingObject){
                targetObject = bubble.GetComponent<Bubble>().choose;
                TranslateObjectBySyncMapping();
            }
  
        }

        /// <summary>
        /// 更新手柄复制对象的世界坐标，与手柄同步
        /// </summary>
        void UpdateLocalHandle()
        {
            localHandle.transform.position = rightHand.transform.position;
        }

        /// <summary>
        /// 获取肩膀世界坐标
        /// </summary>
        void GetShoulderPosition(){
                    var temp = new Vector3(0,0,0);
                    // temp 通过测试获得
                    userShoulder.transform.localPosition = temp;}

        /// <summary>
        /// 将单手操作空间的朝向与VR相机朝向同步
        /// </summary>
        void UpdateSHSForward(){
                Vector3 eyeEulerAngle = new Vector3(0, mainCamera.transform.eulerAngles.y, mainCamera.transform.eulerAngles.z);
                userHead.transform.eulerAngles = eyeEulerAngle;
                // 同步空间位置
                userHead.transform.position = mainCamera.transform.position;   
        }

        /// <summary>
        /// 将用户视野朝向与VR相机朝向同步
        /// </summary>
        void UpdateUserFov(){
                cameraInfo[0] = mainCamera.transform.position;
                cameraInfo[1] = mainCamera.transform.forward;
                cameraInfo[2] = mainCamera.transform.right;
                cameraInfo[3] = mainCamera.transform.up;      
        }
        /// <summary>
        /// 根据手柄局部坐标计算虚拟对象位置，单手操作空间为倒立半球
        /// </summary>
        /// <param name="p_h"></param>
        /// <returns></returns>
        Vector3 GetSMSObjectPositionHemi(Vector3 p_h)
        {
            float d_np = mainCamera.nearClipPlane; // distance_near_plane
            float d_fp = mainCamera.farClipPlane; // distance_far_plane
            d_fp = farPlaneValue; // 测试时规定
            float d_fn = d_fp - d_np; // 平截头体的高
            float r_arm = 1.1f * (userArmLength - userBoomLength); // 适当放大userArmLength

            Vector3 p_shoulder = userShoulder.transform.localPosition; // 肩膀局部坐标
            Vector3 p_elbow = p_shoulder - userBoomLength * Vector3.down; // 手肘坐标=肩膀坐标-大臂长度*(0,-1,0)
            Vector3 p_camera = cameraInfo[0]; // 相机位置

            // 计算zo

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

            // 计算z_object处的屏幕长宽
            float height_object = z_object * Mathf.Tan(halfFov);
            float width_object = height_object * aspectFov;



            float r_xy_2 = Mathf.Pow(r_arm, 2) - Mathf.Pow(r_arm - z_abs_dis, 2);
            if (r_xy_2 < 0) { Debug.LogError("映射圆面半径计算小于0,请检查坐标初始化.."); }
            float r_xy = Mathf.Sqrt(r_xy_2); // 映射圆面半径

            // 计算x_object
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

            // 计算y_object

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

            // 计算p_o
            Vector3 p_object = p_camera + cameraInfo[1] * z_object + cameraInfo[2] * x_object + cameraInfo[3] * y_object;
            return p_object;
        }


        /// <summary>
        /// 物体和手柄进行位置映射，在两个空间中进行同步
        /// </summary>
        void TranslateObjectBySyncMapping()
        {
            if (targetObject!=null)
            {
                targetObject.transform.position = GetSMSObjectPositionHemi(localHandle.transform.localPosition);//根据手柄的位置进行移动
            }
        }
    }


