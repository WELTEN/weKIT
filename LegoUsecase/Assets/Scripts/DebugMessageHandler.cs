using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugMessageHandler : MonoBehaviour {
   Text textMesh;

    // Use this for initialization
    void Start()
    {
        textMesh = gameObject.GetComponent<Text>();
        
    }

    void OnEnable()
    {
        Application.logMessageReceived += LogMessage;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= LogMessage;
    }

    public void LogMessage(string message, string stackTrace, LogType type)
    {
        if (textMesh)
        {
            textMesh.text =  message + System.Environment.NewLine;
            
        }
        else
        {
            Debug.Log("TextMesh not found");
        }
    }
}
