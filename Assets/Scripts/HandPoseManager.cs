using UnityEngine;
using System.Collections.Generic;
using Unity.XR.PXR;
using UnityEngine.XR;
using TMPro;
using Unity.VisualScripting;


public class HandPoseManager : MonoBehaviour
{
    public TMP_InputField inputField;
    public TMP_Text inputField2;
    public GameObject HandRightWrist;
    public GameObject HandLeft;
    public GameObject SecondSelectionBG;

    public GameObject EyeTrackingManager;
    private List<GameObject> selectedObjectsFixed = new List<GameObject>();

    private Dictionary<GameObject, TransformData> originalTransform = new Dictionary<GameObject, TransformData>();


    public bool SecondSelectionState = false;

    void Start()
    {
        EyeTrackingManager = GameObject.Find("EyeTrackingManager");
        SecondSelectionBG = GameObject.Find("Objects/SecondSelectionBG");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onPalmPoseStart()
    {
        originalTransform.Clear();
        selectedObjectsFixed = EyeTrackingManager.GetComponent<EyeTrackingManager>().selectedObjects;
        int i = 0;
        inputField2.text = "";
        SecondSelectionBG.transform.position = new Vector3(0, 1.3f, 2.2f);
        foreach (var obj in selectedObjectsFixed)
        {
            i++;
            inputField2.text += obj.name + "  ";
            originalTransform[obj] = new TransformData(obj.transform.position, obj.transform.localScale);
            obj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            obj.transform.position = SecondSelectionBG.transform.position - new Vector3(SecondSelectionBG.transform.localScale.x / 2, SecondSelectionBG.transform.localScale.y / 2, 0) + new Vector3(obj.transform.localScale.x * (2 * (i%5) - 1) , obj.transform.localScale.y * (2 * (i/5) - 1), - 2 * obj.transform.localScale.z);
        }
        SecondSelectionState = true;

    }

    public void onPalmPoseUpdate()
    {
        inputField.text = HandRightWrist.transform.rotation.eulerAngles.ToString();
    }

    public void onPalmPoseExit()
    {
        foreach (var obj in selectedObjectsFixed)
        {
            if (originalTransform.TryGetValue(obj, out TransformData transformData))
            {
                obj.transform.position = transformData.Position;
                obj.transform.localScale = transformData.Scale;
            }
        }

        selectedObjectsFixed.Clear();
        SecondSelectionState = false;
        SecondSelectionBG.transform.position = new Vector3(0, -3f, 2.2f);
    }
    
}
