using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeBehaviour : MonoBehaviour {

    int filecount = 1;
    GameObject g;
    MeshRenderer rend;

    // Use this for initialization
    void Start()
    {
        TurnAllPageOff();
    }

    void TurnAllPageOff()
    {
        for (int i=1; i <= 31; i++)
        {
            g = GameObject.FindGameObjectWithTag("Lego" + i);
            rend = g.GetComponent<MeshRenderer>();
            rend.enabled = false;
        }
        
        Debug.Log("meshturnedoff");
    }


    void TurnPageOn()
    {
        //if (g)
        //{
        //    TurnAllPageOff();
        //}

        g = GameObject.FindGameObjectWithTag("Lego" + filecount);
        rend = g.GetComponent<MeshRenderer>();
        rend.enabled = true;
        Debug.Log("mesh turned on");

    }


    void AssignNext()
    {
        if (filecount <= 31)
        {
            filecount += 1;
            TurnPageOn();
        }
        
    }

    void AssignPrevious()
    {
        if (filecount > 1)
        {
            filecount -= 1;
            TurnPageOn();
        }
        
    }

}
