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
        Debug.Log($"{this} OnBuildMesh");
        meshBuilder.BuildQuad(this);
    }
}