using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator2 : MonoBehaviour
{
    public Color32 color;
    private List<int> indices = new List<int>();
    private List<Vector3> positions = new List<Vector3>();
    private List<Color32> colors = new List<Color32>();

    // Use this for initialization
    void Awake()
    {
        var r = 0.5f;
        var ltb = new Vector3(-r, r, r);
        var ltf = new Vector3(-r, r, -r);
        var lbb = new Vector3(-r, -r, r);
        var lbf = new Vector3(-r, -r, -r);
        var rtb = new Vector3(r, r, r);
        var rtf = new Vector3(r, r, -r);
        var rbb = new Vector3(r, -r, r);
        var rbf = new Vector3(r, -r, -r);

        GenerateQuad(ltf, rtf, lbf, rbf, color); // front
        GenerateQuad(ltb, ltf, lbb, lbf, color); // left
        GenerateQuad(rtb, ltb, rbb, lbb, color); // back
        GenerateQuad(rtf, rtb, rbf, rbb, color); // right
        GenerateQuad(lbf, rbf, lbb, rbb, color); // bottom
        GenerateQuad(ltb, rtb, ltf, rtf, color); // top

        var mesh = new Mesh
        {
            name = "My Cube Mesh",
            vertices = positions.ToArray(),
            triangles = indices.ToArray(),
            colors32 = colors.ToArray()
        };
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    private void GenerateCube(Color32 color)
    {
        
    }

    private void GenerateQuad(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, Color32 color)
    {
        // indices
        var i = positions.Count;
        indices.Add(i);
        indices.Add(i + 1);
        indices.Add(i + 2);
        indices.Add(i + 2);
        indices.Add(i + 1);
        indices.Add(i + 3);

        // colors
        colors.Add(color);
        colors.Add(color);
        colors.Add(color);
        colors.Add(color);

        // positions
        positions.Add(p1);
        positions.Add(p2);
        positions.Add(p3);
        positions.Add(p4);
    }
}
