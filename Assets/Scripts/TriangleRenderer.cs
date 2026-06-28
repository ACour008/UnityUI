using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
public class TriangleRenderer : MonoBehaviour
{
    public Vector3[] vertices;
    public int[] indices;

    Vector3[] _worldVertices;
    public Vector3[] worldVertices => TransformVertices();

    public void OnEnable()
    {
        TriangleRendererManager.instance.Register(this);
        _worldVertices = new Vector3[vertices.Length];
    }

    public void OnDisable()
    {
        TriangleRendererManager.instance.Unregister(this);
    }

    Vector3[] TransformVertices()
    {
        for (int i = 0; i < _worldVertices.Length; i++)
            _worldVertices[i] = transform.localToWorldMatrix.MultiplyPoint3x4(vertices[i]);

        return _worldVertices;
    }
}
