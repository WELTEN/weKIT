using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.VR.WSA;

/// <summary>
/// class for loading Images dynamically for a manual
/// </summary>
public class ManualBehaviour : MonoBehaviour
{
    int filecount = 1;
    GameObject g;
    MeshRenderer rend;

    // Use this for initialization
    void Start()
    {
        TurnPageOn();
    }

    void TurnAllPageOff()
    {
        rend.enabled = false;
        Debug.Log("meshturnedoff");
    }

   
    void TurnPageOn()
    {
        if (g)
        {
            TurnAllPageOff();
        }

        g = GameObject.FindGameObjectWithTag("Page " + filecount);
        rend = g.GetComponent<MeshRenderer>();
        rend.enabled = true;
        Debug.Log("mesh turned on");

    }
    

    void AssignNext()
    {
        if (filecount <= 6)
        {
            filecount += 1;          
        }
        TurnPageOn();
    }

    void AssignPrevious()
    {
        if (filecount > 1)
        {
            filecount -= 1;          
        }
        TurnPageOn();
    }
}
