using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// class for expanding the 3d model based on the parent gameobject which holds all the subcomponenet 3d models
/// The methods is called by the speech manager 
/// </summary>
public class ModelExpander : MonoBehaviour {
    /// <summary>
    /// array for holding all the 3d models
    /// </summary>
    Transform[] modelObjectPos;
    bool isExpanded=false;
    
	// Use this for initialization
	void Start () {
        modelObjectPos = gameObject.GetComponentsInChildren<Transform>();
	}
    /// <summary>
    /// Method to expand the 3d model by racasting in the direction from the central point of the parent object in the direction of the child objects.
    /// </summary>
    void ExpandModel()
    {
        if (isExpanded == false)
        {
            foreach (Transform g in modelObjectPos)
            {
                //calculate the vectore between 2 points and increase it by a scalar
                Vector3 direction = gameObject.transform.position - g.position;
                    g.localPosition += direction + Vector3.Normalize(direction) * 0.25f;
                    isExpanded = true;
            }  
        }
        else
        {
            foreach (Transform g in modelObjectPos)
            {
                //calculate the vectore between 2 points and increase it by a scalar
                Vector3 direction = gameObject.transform.position - g.position;
                g.localPosition -= direction + Vector3.Normalize(direction) * 0.25f;
                isExpanded = false;
            }
        }
    }
}
