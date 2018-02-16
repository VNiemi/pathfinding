using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heap<T> where  T:IHeapItem<T>
{
    T[] items;
    int itemCount;

    public Heap(int heapSize)
    {
        items = new T[heapSize];
    }

    public void Add(T item)
    {
        item.HeapIndex = itemCount;
        items[itemCount] = item;
        SortUp(item);
        itemCount++;
    }

    void SortUp(T item)
    {
        int parentIndex = (item.HeapIndex - 1) / 2;
        while (true)
        {

            T parentItem = items[parentIndex];
            if(item.CompareTo(parentItem)>0)
            {
                Swap(item, parentItem);
            }
            else
            {
                break;
            }
            parentIndex = (item.HeapIndex - 1) / 2;
        }
    }

    void Swap(T a, T b)
    {
        items[a.HeapIndex] = b;
        items[b.HeapIndex] = a;

        int temp = a.HeapIndex;
        a.HeapIndex = b.HeapIndex;
        b.HeapIndex = temp;
    }

    public T RemoveFirst()
    {
        T first = items[0];
        itemCount--;
        items[0] = items[itemCount];
        items[0].HeapIndex = 0;
        SortDown(items[0]);
        return first;
    }

    void SortDown(T item)
    {
        while(true)
        {
            int left = (item.HeapIndex * 2) + 1;
            int right = (item.HeapIndex * 2) + 2;

            int swap = 0;

            if (left < itemCount)
            {
                swap = left;
                if (right < itemCount)
                {
                    if (items[left].CompareTo(items[right]) < 0)
                        swap = right;
                }

                if (item.CompareTo(items[swap]) < 0)
                    Swap(item, items[swap]);
                else
                    return;
            }
            else
                return;
        }
    }

    public void UpdateItem(T item)
    {
        SortUp(item);
    }

    public int Count
    {
        get
        {
            return itemCount;
        }
    }

    public bool Contains(T item)
    {
        return Equals(items[item.HeapIndex], item);
    }

    public void Clear()
    {
        // Clearing heap like this probably messes up the garbage collector.
        // In this case it should work just fine since the nodes are static anyway.
        itemCount = 0;
    }
    
}

public interface IHeapItem<T> : System.IComparable<T>
{
    int HeapIndex
    {
        get;
        set;
    }
}