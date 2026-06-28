using System.Collections.Generic;
using UnityEngine;

public class TriangleRendererManager : Singleton<TriangleRendererManager>
{   
    List<TriangleRenderer> renderers = new List<TriangleRenderer>();

    public void Register(TriangleRenderer renderer)
    {
        renderers.Add(renderer);
    }    

    public void Unregister(TriangleRenderer renderer)
    {
        renderers.Remove(renderer);
    }

    public List<TriangleRenderer> GetAllRenderers() => new List<TriangleRenderer>(renderers);
}