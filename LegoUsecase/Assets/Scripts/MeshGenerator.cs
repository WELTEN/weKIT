using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Class to generate rectangular mesh on the lego models
/// </summary>
[RequireComponent(typeof (MeshFilter), typeof(MeshRenderer))]
public class MeshGenerator : MonoBehaviour {
    public int xSize, ySize;
    private Vector3[] vertices;

    private void Awake()
    {
        GenerateMesh();
    }

    private void GenerateMesh()
    {
        vertices = new Vector3[(xSize + 1) * (ySize + 1)];
    }

    /// <summary>
    /// Method to visualize the mesh with gizmos which can be view in the scene editor and is automatically invoked
    /// </summary>
    private void OnDrawGizmos()
    {
        if (vertices == null)
        {
            return;
        }
        Gizmos.color = Color.red;
        for (int i=0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], 0.1f);
        }
    }
}
