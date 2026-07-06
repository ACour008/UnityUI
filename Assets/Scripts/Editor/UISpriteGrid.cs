using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UISpriteGrid
{
    class Dimension
    {
        public int size;
        public int index;

        public Dimension(int size, int index)
        {
            this.size = size;
            this.index = index;
        }
    }

    public UISpriteGrid(int width, int height)
    {
        stride = width;
        rows.Add(new Dimension(height, 0));
        columns.Add(new Dimension(width, 0));
        data = new bool[width * height];
    }

    List<Dimension> rows = new List<Dimension>();
    List<Dimension> columns = new List<Dimension>();
    bool[] data;
    int stride;

    public int rowsCount => rows.Count;
    public int columnsCount => columns.Count;

    public int GetDataLocation(int x, int y) => stride * y + x;
    
    public int GetRowHeight(int y) => rows[y].size;
    public int GetColumnWidth(int x) => columns[x].size;

    public bool Get(int x, int y)
    {
        int rowIndex = rows[y].index;
        int columnIndex = columns[x].index;

        return data[GetDataLocation(columnIndex, rowIndex)];
    }

    public void Set(int x, int y, bool val)
    {
        int rowIndex = rows[y].index;
        int columnIndex = columns[x].index;
        data[GetDataLocation(columnIndex, rowIndex)] = val;
    }

    public void InsertRow(int atY, int oldRowHeight)
    {
        int rowIndex = rows[atY].index;
        for (int i = 0; i < columns.Count; i++)
            data[GetDataLocation(i, rows.Count)] = data[GetDataLocation(i, rowIndex)];

        Dimension old = rows[atY];
        rows.Insert(atY, new Dimension(old.size - oldRowHeight, rows.Count));
        rows[atY + 1].size = oldRowHeight;
    }

    public void InsertColumn(int atX, int oldRowWidth)
    {
        int columnIndex = columns[atX].index;
        for (int i = 0; i < rows.Count; i++)
            data[GetDataLocation(columns.Count, i)] = data[GetDataLocation(columnIndex, i)];
        
        Dimension old = columns[atX];
        columns.Insert(atX, new Dimension(old.size - oldRowWidth, columns.Count));
        columns[atX + 1].size = oldRowWidth;
    }

    public void AddRow(int height)
    {
        rows.Add(new Dimension(height, columns.Count));
    }

    public void AddColumn(int width)
    {
        columns.Add(new Dimension(width, rows.Count));
    }
}