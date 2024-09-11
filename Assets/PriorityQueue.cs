using System;
using System.Collections.Generic;
using UnityEngine;


public class PriorityQueue<T>
{
    List<T> items = new List<T>();
    Func<T, T, int> compareFunc;

    public PriorityQueue(Func<T, T, int> compareFunc)
    {
        items = new List<T>();
        this.compareFunc = compareFunc;
    }

    public int Count
    {
        get { return items.Count; }
    }


    public void Add(T item)
    {
        items.Add(item);
        int index = items.Count - 1;
        while (index > 0)
        {
            int parentIndex = (index - 1) / 2;
            if (compareFunc(items[index], items[parentIndex]) < 0)
            {
                // Swap the items
                
                T temp = items[index];
                items[index] = items[parentIndex];
                items[parentIndex] = temp;
                
                index = parentIndex;
            }
            else
            {
                break;
            }
        }
    }

    public T Peek()
    {
        try
        {
            return items[0];
        }
        catch (Exception e)
        {
            Debug.Log ("Heap is empty");
            Console.WriteLine(e);
            throw;
        }
        
    }

    public T Pop()
    {
        T result;
        try
        {
            result = items[0];

        }
        catch (Exception e)
        {
            
                Debug.Log ("Heap is empty");
                Console.WriteLine(e);
                throw;
            
        }
        int lastIndex = items.Count - 1;
        items[0] = items[lastIndex];
        items.RemoveAt(lastIndex);

        lastIndex--;

        int index = 0;
        while (true)
        {
            int child1Index = index * 2 + 1;
            int child2Index = index * 2 + 2;

            if (child1Index > lastIndex)
            {
                break;
            }

            int smallerChildIndex = child1Index;

            if (child2Index <= lastIndex && compareFunc(items[child2Index], items[child1Index]) < 0)
            {
                smallerChildIndex = child2Index;
            }

            if (compareFunc(items[smallerChildIndex], items[index]) < 0)
            {
                T temp = items[smallerChildIndex];
                items[smallerChildIndex] = items[index];
                items[index] = temp;

                index = smallerChildIndex;
            }
            else
            {
                break;
            }
        }

        return result;
    }
}

