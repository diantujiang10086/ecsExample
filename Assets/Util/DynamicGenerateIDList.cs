using System.Collections.Generic;
using System;

public class DynamicGenerateIDList : IDisposable
{
    private int max;
    private List<int> recycleList;

    public DynamicGenerateIDList(int capacity = 0)
    {
        recycleList = new List<int>(capacity);
    }

    public int GenerateID()
    {
        if (recycleList.Count != 0)
        {
            var id = recycleList[0];
            recycleList.RemoveAt(0);
            return id;
        }
        else
        {
            return max++;
        }
    }

    public void Recycle(int id)
    {
        recycleList.Add(id);
    }

    public void Dispose()
    {
        recycleList.Clear();
        recycleList = null;
    }
}