using UnityEngine;
using TMPro;
using Unity.XR.PXR;
using UnityEngine.XR;

public class EyeTracking : MonoBehaviour
{
    private LineRenderer lineRenderer; // Used for drawing the ray in the scene
    public TMP_InputField inputField;
    public GameObject head;

    public GameObject head2;


    void Start()
    {
        // Initialize LineRenderer
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        if(lineRenderer == null)
            lineRenderer = gameObject.AddComponent<LineRenderer>();

        // Initialize eye tracking.
        PXR_MotionTracking.WantEyeTrackingService();
        InitializeEyeTracking();
    }

    void Update()
    {
        // Check if the device supports eye tracking.
        EyeTrackingMode eyeTrackingMode = EyeTrackingMode.PXR_ETM_NONE;
        bool isSupported = false;
        int supportedModesCount = 0;
        TrackingStateCode trackingState = (TrackingStateCode)PXR_MotionTracking.GetEyeTrackingSupported(ref isSupported, ref supportedModesCount, ref eyeTrackingMode);
        
        if(isSupported)
        {
            Debug.Log("Eye tracking is supported on this device.");
            // Start Eye Tracking
            EyeTrackingStartInfo startInfo = new EyeTrackingStartInfo
            {
                needCalibration = 1,
                mode = eyeTrackingMode
            };
            PXR_MotionTracking.StartEyeTracking(ref startInfo);

            // Get the eye tracking data
            EyeTrackingDataGetInfo getDataInfo = new EyeTrackingDataGetInfo
            {
                displayTime = 0,
                flags = EyeTrackingDataGetFlags.PXR_EYE_DEFAULT
                        | EyeTrackingDataGetFlags.PXR_EYE_POSITION
                        | EyeTrackingDataGetFlags.PXR_EYE_ORIENTATION
            };
            EyeTrackingData eyeTrackingData = new EyeTrackingData();
            PXR_MotionTracking.GetEyeTrackingData(ref getDataInfo, ref eyeTrackingData);

            inputField.text= "";

            inputField.text += eyeTrackingData.eyeDatas[2].pose.Position.ToVector3().ToString() + "\n";

            inputField.text += eyeTrackingData.eyeDatas[2].pose.Orientation.ToQuat().ToString() + "\n";


            if (eyeTrackingData.eyeDatas[0].openness > 0.1f && eyeTrackingData.eyeDatas[0].openness > 0.1f) // Adjust openness threshold as needed
            {
                inputField.text += "open" + "\n";
                Vector3 origin = eyeTrackingData.eyeDatas[2].pose.Position.ToVector3();
                Vector3 direction = eyeTrackingData.eyeDatas[2].pose.Orientation.ToQuat() * Vector3.forward;

                gameObject.transform.position = origin + direction * 5;

                inputField.text += origin + "\n";
                inputField.text += direction + "\n";

                // Draw the ray from eyes
                lineRenderer.enabled = true;
                lineRenderer.startWidth = 0.001f;
                lineRenderer.endWidth = 0.001f;
                lineRenderer.SetPosition(0, origin);
                lineRenderer.SetPosition(1, origin + direction * 5); // Modify the length of the ray as needed
            }
            else
            {
                // Eyes are closed, disable the ray
                lineRenderer.enabled = false;
            }
        }
    }

    void InitializeEyeTracking()
    {
        // Perform any initialization or calibration required for eye tracking
        // This can be custom-defined depending on the specifics of your application and device
    }

    void OnDisable()
    {
        // Stop eye tracking when the script or GameObject is disabled
        EyeTrackingStopInfo stopInfo = new EyeTrackingStopInfo();
        PXR_MotionTracking.StopEyeTracking(ref stopInfo);
    }
}
