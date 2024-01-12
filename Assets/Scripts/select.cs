using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class select : MonoBehaviour
{
    [SerializeField]
    public LayerMask LayerMask;//限定检测的物品层级

    public List<GameObject> detectsAll;//框内的物体集合,是最终返回的物品集合
    public List<GameObject> detects;//框内的物体集合,是每次选择返回的物品集合
    public GameObject distance;//获得负责检测距离的物体,框大小
    public GameObject EyeDetect;//负责眼动检测的组件
    public GameObject head;//摄像机

    bool m_Started = false;//是否把框线绘制出来

    private palm_distance palmDistance;//distance上挂载的脚本

    private bool multi = false;//是否处于多次选择中
    private bool selected = false;//是否处于选择中
    private List<GameObject> withinCameras;//当前摄像机可见的所有物品集合
    // Start is called before the first frame update
    void Start()
    {
        palmDistance = distance.GetComponent<palm_distance>();//获取脚本
    }

    // Update is called once per frame
    void Update()
    {
        if (selected)
        {
            detects = new List<GameObject>();//每次更新所选的物体，清空数组
            Vector3 scale = new Vector3(palmDistance.length, palmDistance.width, palmDistance.height);
            var center = (palmDistance.width / 2 - palmDistance.dcenter) * palmDistance.forward + palmDistance.boxCol.transform.position;
            Collider[] objs = Physics.OverlapBox(center, scale / 2, head.transform.rotation, LayerMask);
            //试一下这个三边大小啥意思
            foreach (var obj in objs)
            {
                detects.Add(obj.gameObject);//可以通过碰撞体获得当前物体的gameObject
        }

        }
        
    }
}
