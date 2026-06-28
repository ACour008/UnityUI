using System.Collections.Generic;
using UnityEngine;

public class UIMeshBuilder
{
    Mesh mesh;

    List<Vector3> allVertices = new List<Vector3>();
    List<int> allIndices = new List<int>();
    
    public void BuildQuad(UIComponent component)
    {
        int vertexCount = allVertices.Count;
        AddVertices(component);
        AddIndices(vertexCount);
    }

    void AddVertices(UIComponent component)
    {
        Vector3[] vertices = new Vector3[4];

        // bottom-left (0), top-left (1), top-right (2), bottom-right (3)
        component.rectTransform.GetLocalCorners(vertices);
        allVertices.AddRange(vertices);
        // for (int i = 0; i < vertices.Length; i++)
        // {
        //     var v = component.transform.localToWorldMatrix.MultiplyPoint3x4(vertices[i]);
        //     Debug.Log(v);
        //     allVertices.Add(v);
        // }
    }

    void AddIndices(int indexOffset)
    {
        allIndices.Add(indexOffset);
        allIndices.Add(indexOffset + 2);
        allIndices.Add(indexOffset + 1);
        allIndices.Add(indexOffset + 3);
        allIndices.Add(indexOffset + 2);
        allIndices.Add(indexOffset);
    }

    public Mesh GetMesh()
    {
        if (!mesh)
            mesh = new Mesh();
        
        mesh.Clear();
        mesh.vertices = allVertices.ToArray();
        mesh.triangles = allIndices.ToArray();
        mesh.RecalculateBounds();

        return mesh;
    }
}
