using UnityEngine;

public class UIImage : UIComponent
{
    public Sprite sprite;
    public override void OnArrange(Rect finalRect)
    {
        rectTransform.anchoredPosition = finalRect.position;
        rectTransform.sizeDelta = finalRect.size;
    }

    public override void OnMeasure(Vector2 availableSize)
    {        
        if (sprite)
        {
            measuredSize.x = sprite.rect.width;
            measuredSize.y = sprite.rect.height;
        }
        else
        {
            measuredSize = Vector2.zero;
        }
    }

    public override void OnBuildMesh(UIMeshBuilder meshBuilder)
    {
        meshBuilder.AddQuad(this);
        if (!mesh)
            return;

        mesh.Clear();
        
        var quad = meshBuilder.BuildQuad(this);
        mesh.vertices = quad.vertices;
        mesh.triangles = quad.indices;
    }
}