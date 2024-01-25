using System.Collections;
using TMPro;
using UnityEngine;

public class GrabAgentObject : MonoBehaviour
{
    public GameObject rightThumb;
    public GameObject rightIndex;

    public GameObject leftThumb;
    public GameObject leftIndex;
    public TMP_InputField log;

    private bool pinchStatus;
    private bool grabStatus;
    private bool movingStatus;
    private Vector3 originalPosition;
    private Vector3 lastPosition;

    public GameObject TargetObject;
    private GameObject originalParent;
    private float movingScale;

    void Start()
    {
        originalParent = transform.parent.gameObject;
        originalPosition = transform.localPosition;
    }

    void Update()
    {
        movingScale = Vector3.Distance(leftIndex.transform.position, leftThumb.transform.position) * 100;
        log.text = Vector3.Distance(leftIndex.transform.position, leftThumb.transform.position).ToString();
        pinchStatus = Vector3.Distance(rightIndex.transform.position, rightThumb.transform.position) < 0.02f;

        if (pinchStatus && grabStatus && !movingStatus)
        {
            transform.parent = rightIndex.transform;
            lastPosition = rightIndex.transform.position;
            movingStatus = true;
        }

        if ((!pinchStatus || !grabStatus) && movingStatus)
        {
            movingStatus = false;
            transform.parent = originalParent.transform;
            transform.localPosition = originalPosition;
        }

        if (movingStatus)
        {
            // 计算手指的移动向量
            Vector3 deltaPosition = rightIndex.transform.position - lastPosition;

            // 使用相对坐标同步位置
            TargetObject.transform.position += deltaPosition * movingScale;
            TargetObject.transform.rotation = transform.rotation;

            // 更新原始位置
            lastPosition = rightIndex.transform.position;
        }
        log.text += "\n" + movingStatus.ToString();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == rightThumb || other.gameObject == rightIndex)
        {
            grabStatus = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == rightThumb || other.gameObject == rightIndex)
        {
            grabStatus = false;
        }
    }
}
