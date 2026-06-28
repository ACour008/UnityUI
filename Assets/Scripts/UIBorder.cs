using System;
using UnityEngine;

[Serializable]
public struct UIBorder : IEquatable<UIBorder>
{
    public float left;
    public float top;
    public float right;
    public float bottom;

    public UIBorder(Vector4 v)
    {
        left = v.x;
        top = v.y;
        right = v.z;
        bottom = v.w;
    }

    public Vector4 AsVector4() => new Vector4(left, top, right, bottom);

    public bool isZero => left == 0 && top == 0 && right == 0 && bottom == 0;

    public Rect GetInset(Rect r)
    {
        r.xMin += left;
        r.xMax -= right;
        r.yMin += bottom;
        r.yMax -= top;
        return r;
    }

    public Rect GetOutset(Rect r)
    {
        r.xMin -= left;
        r.xMax += right;
        r.yMin -= bottom;
        r.yMax += top;
        return r;
    }

    public override string ToString()
    {
        if (left == right && top == bottom)
        {
            if (left == top)
                return left.ToString();
            else
                return $"x:{left} y:{top}";
        }
        return $"l:{left} t:{top} r:{right} b:{bottom}";
    }

    public override bool Equals(object obj) => obj is UIBorder other ? this.Equals(other) : false;

    public bool Equals(UIBorder other) => left == other.left && top == other.top && right == other.right && bottom == other.bottom;

    public override int GetHashCode() => AsVector4().GetHashCode();

    public static bool operator ==(UIBorder a, UIBorder b) => a.Equals(b);
    public static bool operator !=(UIBorder a, UIBorder b) => !a.Equals(b);

}