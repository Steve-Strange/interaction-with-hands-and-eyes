using System.Linq;
using TMPro;
using UnityEngine;
using System;

public class Logger : MonoBehaviour
{
    [SerializeField]
    Color InfoColor = Color.green;
    [SerializeField]
    Color ErrorColor = Color.red;
    [SerializeField]
    Color WarningColor = Color.yellow;

    [SerializeField]
    private TextMeshProUGUI debugAreaText = null;

    [SerializeField]
    private bool enableDebug = false;

    [SerializeField]
    private int maxLines = 15;

    void Awake()
    {
        if (debugAreaText == null)
        {
            debugAreaText = GetComponent<TextMeshProUGUI>();
        }
        debugAreaText.text = string.Empty;
    }

    void OnEnable()
    {
        debugAreaText.enabled = enableDebug;
        enabled = enableDebug;

        if (enabled)
        {
            debugAreaText.text += $"<color=\"white\">{DateTime.Now.ToString("HH:mm:ss.fff")} {this.GetType().Name} enabled</color>\n";
        }
    }

    public void Clear() => debugAreaText.text = string.Empty;

    public void LogInfo(string message)
    {
        ClearLines();
        string color = ColorUtility.ToHtmlStringRGB(InfoColor);
        //debugAreaText.text += $"<color=\"green\">{DateTime.Now.ToString("HH:mm:ss.fff")} {message}</color>\n";
        debugAreaText.text += string.Format("<color=#{0}>{1}</color>", color, DateTime.Now.ToString("HH:mm:ss.fff") + ": " + message + "\n");
    }

    public void LogError(string message)
    {
        ClearLines();
        string color = ColorUtility.ToHtmlStringRGB(ErrorColor);
        //debugAreaText.text += $"<color=\"red\">{DateTime.Now.ToString("HH:mm:ss.fff")} {message}</color>\n";
        debugAreaText.text += string.Format("<color=#{0}>{1}</color>", color, DateTime.Now.ToString("HH:mm:ss.fff") + ": " + message + "\n");
    }

    public void LogWarning(string message)
    {
        ClearLines();
        string color = ColorUtility.ToHtmlStringRGB(WarningColor);
        //debugAreaText.text += $"<color=\"yellow\">{DateTime.Now.ToString("HH:mm:ss.fff")} {message}</color>\n";
        debugAreaText.text += string.Format("<color=#{0}>{1}</color>", color, DateTime.Now.ToString("HH:mm:ss.fff") + ": " + message + "\n");
    }

    private void ClearLines()
    {
        if (debugAreaText.text.Split('\n').Count() >= maxLines)
        {
            debugAreaText.text = string.Empty;
        }
    }
}