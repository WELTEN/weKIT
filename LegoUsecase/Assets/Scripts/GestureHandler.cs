using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureHandler : MonoBehaviour {
    private bool isActive=false;
    
    
	// Use this for initialization
	void Start () {
            
    }

    /// <summary>
    /// ONselect call is the call send by the gazegesturemanager on clicked event
    /// </summary>
    public void OnSelect()
    {
       
        isActive = true;
        //GameObject.FindGameObjectWithTag("bridge").SendMessage("ExpandModel");
        Debug.Log("ouch! clicked");
    }

    public void OnDeselect()
    {
        return;
    }

    // Update is called once per frame
    void Update () {
        //if isActive rotate the parent gameobject
        if (isActive)
        {
            isActive = false;
        }
    }
}
