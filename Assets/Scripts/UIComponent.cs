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
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[ExecuteAlways]
public abstract class UIComponent : MonoBehaviour
{
    RectTransform rt;
    public RectTransform rectTransform
    { 
        get
        {
            if (!rt)
                rt = GetComponent<RectTransform>();
            return rt;
        }
    }

    protected MeshFilter meshFilter;
    protected MeshRenderer meshRenderer;
    protected Mesh mesh;

    [Header("Sizing")]
    public SizingType sizingX = SizingType.FitContent;
    public SizingType sizingY = SizingType.FitContent;

    public float width;
    public float height;

    public Vector2 measuredSize = new Vector2(0, 0);
    public Vector2 contentSize;

    protected virtual void Awake()
    {
        Debug.Log($"{this} AWAKE");

        this.rectTransform.anchorMin = Vector2.zero;
        this.rectTransform.anchorMax = Vector2.zero;
        this.rectTransform.pivot = Vector2.zero;

        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();

        try
        {
            meshRenderer.material = UIManager.instance.uiMaterial;
            mesh = new Mesh();
            mesh.name = $"mesh_{gameObject.name}";
        }
        catch (Exception e)
        {
            Debug.LogError($"Couldn't find material. {e.StackTrace}");
        }
    }

    void OnEnable()
    {
        meshFilter.mesh = mesh;
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
