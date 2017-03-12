using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureHandler : MonoBehaviour {
    private bool isActive;
    
    
	// Use this for initialization
	void Start () {
            
    }

    /// <summary>
    /// ONselect call is the call send by the gazegesturemanager on clicked event
    /// </summary>
    public void OnSelect()
    {

        gameObject.GetComponent<MeshRenderer>().material.shader = Shader.Find("Toon / Basic Outline");
        isActive = true;
        Debug.Log("ouch! clicked");
    }

    public void OnDeselect()
    {
        gameObject.GetComponent<MeshRenderer>().material.shader = Shader.Find("Diffuse");
    }

    // Update is called once per frame
    void Update () {
        //if isActive rotate the parent gameobject
        if (isActive)
        {
            //this.GetComponentInParent<Transform>().Rotate(0, 90, 0);
            GameObject.FindGameObjectWithTag("LegoModel").transform.Rotate(0, 90, 0);
            isActive = false;
        }
        

    }
}
