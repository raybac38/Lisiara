using System;
using System.Collections.Generic;

public class PriorityQueue<T>
{
    private List<(T item, float priority)> heap = new List<(T, float)>();

    public int Count => heap.Count;

    public void Enqueue(T item, float priority)
    {
        heap.Add((item, priority));
        int i = heap.Count - 1;
        while (i > 0)
        {
            int parent = (i - 1) / 2;
            if (heap[parent].priority <= priority) break;
            heap[i] = heap[parent];
            i = parent;
        }
        heap[i] = (item, priority);
    }

    public T Dequeue()
    {
        if (heap.Count == 0) throw new InvalidOperationException("Queue is empty.");
        var root = heap[0].item;
        var last = heap[heap.Count - 1];
        heap.RemoveAt(heap.Count - 1);

        int i = 0;
        while (i * 2 + 1 < heap.Count)
        {
            int left = i * 2 + 1;
            int right = i * 2 + 2;
            int smallest = (right < heap.Count && heap[right].priority < heap[left].priority) ? right : left;

            if (heap[smallest].priority >= last.priority) break;
            heap[i] = heap[smallest];
            i = smallest;
        }

        if (heap.Count > 0)
            heap[i] = last;

        return root;
    }

    public T Peek()
    {
        if (heap.Count == 0) throw new InvalidOperationException("Queue is empty.");
        return heap[0].item;
    }

    public void Clear() => heap.Clear();
}
