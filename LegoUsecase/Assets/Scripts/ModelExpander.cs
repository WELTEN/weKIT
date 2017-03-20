using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// class for expanding the 3d model based on the parent gameobject which holds all the subcomponenet 3d models
/// The methods is activated on airtap 
/// </summary>
public class ModelExpander : MonoBehaviour {
    /// <summary>
    /// array for holding all the 3d models
    /// </summary>
    List<Transform> legoBlock;
    bool isExpanded=false;
    Bounds modelBounds;
    
	// Use this for initialization
	void Start () {
        legoBlock = new List<Transform>();
        modelBounds = gameObject.GetComponent<MeshRenderer>().bounds;
        //Iterate through the first depth of children and add them to list
        foreach (Transform T in gameObject.GetComponentsInChildren<Transform>())
        {
            if (T.parent == gameObject.transform)
            {
                legoBlock.Add(T);
            }
        }
    }
    /// <summary>
    /// Method to expand the 3d model by racasting in the direction from the central point of the parent object in the direction of the child objects.
    /// </summary>
    void ExpandModel()
    {
        Vector3 modelCenter = gameObject.GetComponent<MeshRenderer>().bounds.center;
        
        if (isExpanded == false)
        {
            foreach (Transform g in legoBlock)
            {
                //calculate the vectore between 2 points and increase it by a scalar
                Vector3 direction = g.localPosition - modelCenter;
                Debug.Log(direction);
                g.localPosition +=  direction * 0.5f;

            }  
        }
        else if(isExpanded==true)
        {
            foreach (Transform g in legoBlock)
            {
                //calculate the vectore between 2 points and increase it by a scalar
                Vector3 direction = g.localPosition - modelCenter;
                Debug.Log(direction);
                g.localPosition -= direction * 0.5f;
            }
        }

        isExpanded = !isExpanded;
    }
}
