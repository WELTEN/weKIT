using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshDisplay : MonoBehaviour {
    Mesh mesh;
    Material material;
    public void Update()
    {
        mesh = this.GetComponent<Mesh>();
        material = this.GetComponent<Material>();
        // will make the mesh appear in the scene at origin position
        Graphics.DrawMesh(mesh, Vector3.zero, Quaternion.identity, material, 0);
    }
}
