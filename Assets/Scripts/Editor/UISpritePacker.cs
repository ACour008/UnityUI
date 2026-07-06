using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Pool;

// Grid splitting algorithm for rect packing adapted from
// https://www.david-colson.com/2020/03/10/exploring-rect-packing.html

public class UISpritePacker
{
    const int kAtlasSize = 2064;
    const int kMaxSpriteSize = 1024;
    const int kMaxAtlasSize = 8192;
    static int currentAtlasSize;
    const string kSavePath = "Data/UI";

    [MenuItem("Tools/Pack UI Sprites")]
    public static void PackSprites()
    {
        currentAtlasSize = kAtlasSize;
        var atlas = new Texture2D(kAtlasSize, kAtlasSize, TextureFormat.RGBA32, false);
        var uvPositions = new List<UISpriteLookupEntry>();

        var guidStrings = AssetDatabase.FindAssets("", new string[] {"Assets/Sprites"});
        if (guidStrings.Length == 0)
        {
            Debug.Log("No sprites to pack. Aborting");
            return;
        }
        
        Debug.Log($"Preparing to pack {guidStrings.Length} sprites");

        List<Texture2D> textures = new List<Texture2D>();
        foreach (var guidString in guidStrings)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(guidString);
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
            textures.Add(texture);
        }

        // Sort by height
        textures.Sort((a, b) => b.height.CompareTo(a.height));

        Debug.Log("Packing sprites...");
        UISpriteGrid grid = new UISpriteGrid(kAtlasSize, kAtlasSize);

        bool TryPackTexture(Texture2D texture)
        {
            bool done = false;
            int yPos = 0;
            for (int y = 0; y < grid.rowsCount && !done; y++)
            {
                int xPos = 0;
                for (int x = 0; x < grid.columnsCount && !done; x++)
                {
                    Vector2Int leftOverSize = Vector2Int.zero;
                    Vector2Int requiredNodes = Vector2Int.zero;

                    if (CanBePlaced(grid, new Vector2Int(x, y), new Vector2Int(texture.width, texture.height), out requiredNodes, out leftOverSize))
                    {
                        done = true;
                        RenderTexture rt = RenderTexture.GetTemporary(texture.width, texture.height, 0, RenderTextureFormat.ARGB32);
                        Graphics.Blit(texture, rt);

                        RenderTexture prev = RenderTexture.active;
                        RenderTexture.active = rt;

                        Texture2D readable = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
                        readable.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
                        readable.Apply();

                        RenderTexture.active = prev;
                        RenderTexture.ReleaseTemporary(rt);

                        var path = AssetDatabase.GetAssetPath(texture);
                        var guid = AssetDatabase.GUIDFromAssetPath(path).ToString();
                        var uvPosition = new Rect(xPos, yPos, texture.width, texture.height);
                        uvPositions.Add(
                            new UISpriteLookupEntry()
                            {
                                guid = guid,
                                uvRect = uvPosition,
                            }
                        );

                        var pixels = readable.GetPixels();
                        atlas.SetPixels(xPos, yPos, texture.width, texture.height, pixels);

                        int xFarRightColumn = x + requiredNodes.x - 1;
                        grid.InsertColumn(xFarRightColumn, leftOverSize.x);

                        int yFarBottomRow = y + requiredNodes.y - 1;
                        grid.InsertRow(yFarBottomRow, leftOverSize.y);

                        for (int i = x + requiredNodes.x - 1; i >= x; i--)
                        {
                            for (int j = y + requiredNodes.y - 1; j >= y; j--)
                                grid.Set(i, j, true);
                        }
                        break;
                    }
                    xPos += grid.GetColumnWidth(x);
                }
                yPos += grid.GetRowHeight(y);
            }

            return done;
        }

        foreach (var texture in textures)
        {
            if (texture.width > kMaxSpriteSize || texture.height > kMaxSpriteSize)
            {
                Debug.LogWarning($"Texture exceeds the max sprite size of {kMaxSpriteSize}. Skipping over texture ({texture.name})");
                continue;
            }

            bool done = TryPackTexture(texture);
            while (!done)
            {
                if (currentAtlasSize * 2 > kMaxAtlasSize)
                {
                    Debug.LogError("Atlas size exceeds max size. Aborting...");
                    return;
                }
                grid.AddRow(currentAtlasSize);
                grid.AddColumn(currentAtlasSize);

                currentAtlasSize *= 2;
                var newAtlas = new Texture2D(currentAtlasSize, currentAtlasSize, TextureFormat.RGBA32, false);
                var pixels = atlas.GetPixels();
                newAtlas.SetPixels(0, 0, atlas.width, atlas.height, pixels);
                atlas = newAtlas;
                done = TryPackTexture(texture);
            }
        }

        string directory = $"{Application.dataPath}/{kSavePath}";
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        byte[] bytes = ImageConversion.EncodeToPNG(atlas);
        File.WriteAllBytes(directory + "/atlas.png", bytes);
        AssetDatabase.Refresh();

        Debug.Log("Sprites packed. Saving to asset...");
        var spriteAtlas = ScriptableObject.CreateInstance<UISpriteAtlas>();
        spriteAtlas.atlas = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Data/UI/atlas.png");

        List<UISpriteLookupEntry> normalized = new List<UISpriteLookupEntry>();
        foreach (var uvPos in uvPositions)
        {
            normalized.Add(new UISpriteLookupEntry()
            {
                guid = uvPos.guid,
                uvRect = new Rect()
                {
                    x = (float)uvPos.uvRect.x / currentAtlasSize,
                    y = (float)uvPos.uvRect.y / currentAtlasSize,
                    width = (float)uvPos.uvRect.width / currentAtlasSize,
                    height = (float)uvPos.uvRect.height / currentAtlasSize
                }
            });
        }
        spriteAtlas.uvPositions = normalized;
        
        AssetDatabase.CreateAsset(spriteAtlas, "Assets/Data/UI/UISpriteAtlas.asset");
        AssetDatabase.SaveAssets(); 
        AssetDatabase.Refresh();
        AssetImporter.GetAtPath("Assets/Data/UI/UISpriteAtlas.asset").assetBundleName = "ui";
        
        Debug.Log($"Complete. Sprite atlas saved at {directory}");
    }

    static bool CanBePlaced(UISpriteGrid grid, Vector2Int desiredNode, Vector2Int desiredRectSize, out Vector2Int requiredNodes, out Vector2Int remainingSize)
    {
        int foundWidth = 0;
        int foundHeight = 0;

        int trialX = desiredNode.x;
        int trialY = desiredNode.y;

        requiredNodes = Vector2Int.zero;
        remainingSize = Vector2Int.zero;

        while (foundHeight < desiredRectSize.y)
        {
            trialX = desiredNode.x;
            foundWidth = 0;

            // ran out of space
            if (trialY >= grid.rowsCount)
                return false;

            while (foundWidth < desiredRectSize.x)
            {
                if (trialX >= grid.columnsCount)
                    return false;

                if (grid.Get(trialX, trialY))
                    return false;
                
                foundWidth += grid.GetColumnWidth(trialX);
                trialX++;
            }
            foundHeight = grid.GetRowHeight(trialY);
            trialY++;
        }

        if ((trialX - desiredNode.x) <= 0 || (trialY - desiredNode.y) <= 0)
            return false;
        
        requiredNodes = new Vector2Int(trialX - desiredNode.x, trialY - desiredNode.y);
        remainingSize = new Vector2Int(foundWidth - desiredRectSize.x, foundHeight - desiredRectSize.y);
        return true;
    }
}