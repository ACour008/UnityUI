using System;
using System.Collections.Generic;
using UnityEngine;

public enum SizingType
{
    // Explict size
    Fixed,
    // Whatever is available
    Fill,
    // Checks children first
    FitContent
}

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
[ExecuteAlways]
public abstract class UIComponent : MonoBehaviour
{
    public RectTransform rectTransform { get; private set; }

    [Header("Sizing")]
    public SizingType sizingX = SizingType.FitContent;
    public SizingType sizingY = SizingType.FitContent;

    public float width;
    public float height;

    public Vector2 measuredSize = new Vector2(0, 0);
    public Vector2 contentSize;

    protected virtual void Awake()
    {
        this.rectTransform = GetComponent<RectTransform>();
        this.rectTransform.anchorMin = Vector2.zero;
        this.rectTransform.anchorMax = Vector2.zero;
        this.rectTransform.pivot = Vector2.zero;
    }

    public Vector2 Measure(Vector2 availableSize)
    {
        OnMeasureChildren(availableSize);
        OnMeasure(availableSize);
        return measuredSize;
    }

    public virtual void OnMeasureChildren(Vector2 availableSize) { }

    public abstract void OnMeasure(Vector2 availableSize);

    public void Arrange(Rect finalRect)
    {
        OnArrange(finalRect);
    }

    public abstract void OnArrange(Rect finalRect);

    public void BuildMesh(UIMeshBuilder meshBuilder)
    {
        OnBuildMesh(meshBuilder);
    }

    public virtual void OnBuildMesh(UIMeshBuilder meshBuilder) { }
}
