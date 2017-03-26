using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manual behaviour of the model to make visible each block of the model one after another
/// Possibility to jump directly to the required step
/// </summary>
public class BridgeBehaviour : MonoBehaviour {
   List<Transform> legoBlock;
   int stepNumber = 0;
   Camera cameraObject;

    // Use this for initialization
    void Start()
    {
        cameraObject = Camera.main;
        legoBlock = new List<Transform>();
        //Iterate through the first depth of children and add them to list
        foreach (Transform T in gameObject.GetComponentsInChildren<Transform>())
        {
            if (T.parent == gameObject.transform)
            {
                legoBlock.Add(T);
            }
        }

        Debug.Log(legoBlock.Count);
        //for some reason outputs 56 though i have 63 children

        TurnAllBlockOff();
    }

    /// <summary>
    /// All lego blocks must be invisible at the beggining
    /// </summary>
    void TurnAllBlockOff()
    {
        foreach (Transform T in legoBlock)
        {
            foreach (MeshRenderer rend in T.GetComponentsInChildren<MeshRenderer>())
            {
                rend.enabled = false;
            }
        }
    }

    /// <summary>
    /// Turn on the number of blocks to be made visible by using the @numberofblocks
    /// </summary>
    void TurnModelOn()
    {
        for (int i = 0; i <= stepNumber; i++)
        {
            if (i < legoBlock.Count)
            {
                foreach (MeshRenderer rend in legoBlock[i].GetComponentsInChildren<MeshRenderer>())
                {
                    rend.enabled = true;
                }
            }
        }

    }

    /// <summary>
    /// Add +1 to numberofblocks
    /// </summary>
    void AssignNext()
    {
        if (stepNumber <= legoBlock.Count)
        {
            stepNumber += 1;
            TurnModelOn();
        }else
        {
            Debug.Log("The number of Steps finished");
        }
        
    }

    void AssignPrevious()
    {
        if (stepNumber <= 0 )
        {
            stepNumber -= 1;
            TurnModelOn();
        }

    }

    void RecordStop()
    {
        Debug.Log("crap!!");
        int i = cameraObject.GetComponent<SpeechToTextManager>().StepNumber();
        if (0 <= i && i <= legoBlock.Count)
        {
            stepNumber = i;
        }
        TurnAllBlockOff();
        TurnModelOn();
    }

}
