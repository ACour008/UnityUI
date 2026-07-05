using System.Collections.Generic;
using UnityEngine;

public class Quad
{
    public Vector3[] vertices = new Vector3[4];
    public int[] indices = new int[6];
}

public class UIMeshBuilder
{
    Mesh mesh;

    List<Vector3> allVertices = new List<Vector3>();
    List<int> allIndices = new List<int>();
    
    public Quad BuildQuad(UIComponent component)
    {
        var quad = new Quad();
        component.rectTransform.GetLocalCorners(quad.vertices);
        quad.indices[0] = 0;
        quad.indices[1] = 2;
        quad.indices[2] = 1;
        quad.indices[3] = 3;
        quad.indices[4] = 2;
        quad.indices[5] = 0;

        return quad;    
    }

    public void AddQuad(UIComponent component)
    {
        int vertexCount = allVertices.Count;
        AddVertices(component);
        AddIndices(vertexCount);
    }

    void AddVertices(UIComponent component)
    {
        Vector3[] vertices = new Vector3[4];

        // bottom-left (0), top-left (1), top-right (2), bottom-right (3)
        component.rectTransform.GetWorldCorners(vertices);
        allVertices.AddRange(vertices);
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
