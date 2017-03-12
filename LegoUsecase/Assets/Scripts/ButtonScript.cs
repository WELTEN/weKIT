using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour {
    Camera cameraObject;
    void OnSelect()
    {
        cameraObject = Camera.main;
        cameraObject.GetComponent<CapturePhoto>().SendMessage("Capture");

    }
}
