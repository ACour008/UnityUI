using UnityEngine;

[RequireComponent(typeof(RectTransform))]
[ExecuteAlways]
public class UICanvas : MonoBehaviour
{
    float pixelPerPoint => Screen.height / 2160f;
    RectTransform rectTransform;

    UIMeshBuilder meshBuilder = new UIMeshBuilder();

    // For testing purposes.
    public UIComponent root;

    public void Awake()
    {
        UIManager.instance.canvas = this;
    }

    public void OnEnable()
    {
        rectTransform = GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.zero;
        rectTransform.pivot = Vector2.zero;
        rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height) / pixelPerPoint;
        transform.localScale = Vector3.one * pixelPerPoint;
        Refresh();
    }

    public void Refresh()
    {
        if (!root)
            return;

        Vector2 canvasSize = new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y);
        root.Measure(canvasSize);
        root.Arrange(new Rect(Vector2.zero, canvasSize));
        root.BuildMesh(meshBuilder);
    }

    public Mesh GetMesh()
    {
        return meshBuilder.GetMesh();
    }
}