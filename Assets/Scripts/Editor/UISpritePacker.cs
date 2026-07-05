using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;

public struct PackedTexture
{
    public Texture2D texture;
    public int x;
    public int y;
    public bool wasPacked;

    public int w => texture.width;
    public int h => texture.height;
}

public class UISpritePacker
{
    static Texture2D atlas;
    
    const int kAtlasSize = 2064;
    const string kSavePath = "Data/UI";

    [MenuItem("Tools/Pack UI Sprites")]
    public static void PackSprites()
    {
        atlas = new Texture2D(kAtlasSize, kAtlasSize, TextureFormat.RGBA32, false);

        var guidStrings = AssetDatabase.FindAssets("", new string[] {"Assets/Sprites"});
        if (guidStrings.Length == 0)
        {
            Debug.Log("No sprites to pack. Aboring");
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

        foreach (var texture in textures)
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
                        var pixels = texture.GetPixels();
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

            if (!done)
            {
                Debug.Log($"Not done. {texture.name}");
                continue;
            }
            else
            {
                Debug.Log($"Sprite packed: {texture.name}");
            }
        }

        string directory = $"{Application.dataPath}/{kSavePath}";
        if (!Directory.Exists(kSavePath))
            Directory.CreateDirectory(directory);

        byte[] bytes = ImageConversion.EncodeToPNG(atlas);
        File.WriteAllBytes(directory + "/atlas.png", bytes);
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