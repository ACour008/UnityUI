using System.Collections.Generic;
using UnityEngine;

public enum AlignDirection
{
    Horizontal,
    Vertical
}

public enum AxisAlignment
{
    Start,
    Center,
    End
}

public class UIContainer : UIComponent
{
    public AlignDirection direction;
    public UIBorder margin;
    public UIBorder padding;
    public float childGap;
    public AxisAlignment childAlignment;

    List<UIComponent> _children = new List<UIComponent>();
    public List<UIComponent> children
    {
        get
        {
            _children.Clear();
            foreach (Transform child in this.transform)
            {
                if (child.gameObject.activeInHierarchy && child.TryGetComponent<UIComponent>(out var component))
                    _children.Add(component);
            }

            return _children;
        }
    }

    public override void OnArrange(Rect finalRect)
    {
        rectTransform.anchoredPosition = finalRect.position;
        rectTransform.sizeDelta = finalRect.size;

        var innerWidth = finalRect.width - padding.left - padding.right;
        var innerHeight = finalRect.height - padding.top - padding.bottom;

        float mainAxisSpace = 0;
        int fillCount = 0;
        float fillSpace = 0;
        float childMainSize = 0;
        float childCrossSize = 0;
        float childCrossPosition = 0;
        float cursor = 0;

        if (direction == AlignDirection.Vertical)
        {
            mainAxisSpace = innerHeight - (children.Count - 1) * childGap;
            cursor = padding.top;

            foreach (var child in children)
            {
                if (child.sizingY != SizingType.Fill)
                    mainAxisSpace -= child.measuredSize.y;
                else
                    fillCount++;
            }

            if (fillCount > 0)
                fillSpace = mainAxisSpace / (float)fillCount;
            
            foreach (var child in children)
            {
                childMainSize = child.sizingY == SizingType.Fill ? fillSpace : child.measuredSize.y;
                childCrossSize = child.sizingX == SizingType.Fill ? innerWidth : child.measuredSize.x;     
                
                if (childAlignment == AxisAlignment.Start)
                    childCrossPosition = padding.left;
                else if (childAlignment == AxisAlignment.Center)
                    childCrossPosition = (innerWidth - childMainSize) / 2f;
                else if (childAlignment == AxisAlignment.End)
                    childCrossPosition = innerWidth - childMainSize - padding.right;

                var childRect = new Rect(childCrossPosition, cursor, childCrossSize, childMainSize);
                child.Arrange(childRect);
                cursor += childCrossSize + childGap;
            }
        }
        else if (direction == AlignDirection.Horizontal)
        {
            mainAxisSpace = innerWidth - (children.Count - 1) * childGap;
            cursor = padding.left;

            foreach (var child in children)
            {
                if (child.sizingX != SizingType.Fill)
                    mainAxisSpace -= child.measuredSize.x;
                else
                    fillCount++;
            }

            if (fillCount > 0)
                fillSpace = mainAxisSpace / (float)fillCount;

            foreach (var child in children)
            {
                childMainSize = child.sizingX == SizingType.Fill ? fillSpace : child.measuredSize.x;
                childCrossSize = child.sizingY == SizingType.Fill ? innerHeight : child.measuredSize.y;
                
                if (childAlignment == AxisAlignment.Start)
                    childCrossPosition = innerHeight - childCrossSize - padding.top;
                else if (childAlignment == AxisAlignment.Center)
                    childCrossPosition = (innerHeight - childMainSize) / 2f;
                else if (childAlignment == AxisAlignment.End)
                    childCrossPosition = padding.bottom;

                var childRect = new Rect(cursor, childCrossPosition, childCrossSize, childMainSize);
                child.Arrange(childRect);
                cursor += childCrossSize + childGap;
            }
        }
    }

    public override void OnMeasureChildren(Vector2 availableSize)
    {
        Vector2 size = availableSize;
        size.x -= padding.left + padding.right;
        size.y -= padding.top + padding.bottom;

        foreach (var child in children)
            child.Measure(size);
    }

    public override void OnMeasure(Vector2 availableSize)
    {
        contentSize = new Vector2(0f, 0f);
        if (direction == AlignDirection.Vertical)
        {
            foreach (var child in children)
            {
                if (child.sizingY != SizingType.Fill)
                    contentSize.y += child.measuredSize.y;
                contentSize.x = Mathf.Max(contentSize.x, child.measuredSize.x);
            }
            contentSize.y += padding.top + padding.bottom;
            contentSize.x += padding.left + padding.right;
        }
        else if (direction == AlignDirection.Horizontal)
        {
            foreach (var child in children)
            {
                if (child.sizingX != SizingType.Fill)
                    contentSize.x += child.measuredSize.x;
                contentSize.y = Mathf.Max(contentSize.y, child.measuredSize.y);
            }
            contentSize.x += padding.left + padding.right;
            contentSize.y += padding.top + padding.bottom;
        }

        if (sizingX == SizingType.Fixed)
            measuredSize.x = width;
        else if (sizingX == SizingType.Fill)
            measuredSize.x = availableSize.x;
        else if (sizingX == SizingType.FitContent)
            measuredSize.x = contentSize.x;
        
        if (sizingY == SizingType.Fixed)
            measuredSize.y = height;
        else if (sizingY == SizingType.Fill)
            measuredSize.y = availableSize.y;
        else if (sizingY == SizingType.FitContent)
            measuredSize.y = contentSize.y;
    }
}