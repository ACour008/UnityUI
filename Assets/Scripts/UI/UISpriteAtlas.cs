using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct UISpriteLookupEntry
{
    public string guid;
    public Rect uvRect;
}

[CreateAssetMenu(fileName = "UISpriteAtlas", menuName="UI/UI Sprite Atlas")]
public class UISpriteAtlas : ScriptableObject
{
    public Texture2D atlas;

    public List<UISpriteLookupEntry> uvPositions;
}