
// Priority Queue<T>
public class Heap<T> where T : IHeapItem<T>
{
    private T[] items;
    private int count;

    public int Count => count;
    public int Capacity => items.Length;

    public Heap(int maxSize)
    {
        items = new T[maxSize];
    }

    public void Add(T item)
    {
        item.HeapIndex = count;
        items[count] = item;
        SortUp(item);
        count++;
    }
    public T RemoveFirst()
    {
        T firstItem = items[0];
        count--;
        items[0] = items[count];
        items[0].HeapIndex = 0;
        SortDown(items[0]);
        return firstItem;
    }
    public void UpdateItem(T item)
    {
        SortUp(item);
    }
    public bool Contains(T item)
    {
        return count > item.HeapIndex && Equals(items[item.HeapIndex], item);
    }
    public void Clear()
    {
        count = 0;
    }
    private void SortDown(T item)
    {
        while (true)
        {
            int leftChild = item.HeapIndex * 2 + 1;
            int rightChild = item.HeapIndex * 2 + 2;
            int swapIndex;

            if (leftChild < count)
            {
                swapIndex = leftChild;

                if (rightChild < count && items[leftChild].CompareTo(items[rightChild]) < 0)
                {
                    swapIndex = rightChild;
                }

                if (item.CompareTo(items[swapIndex]) < 0)
                {
                    Swap(item, items[swapIndex]);
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }
    }
    private void SortUp(T item)
    {
        int parentIndex = (item.HeapIndex - 1) / 2;

        while (true)
        {
            T parentItem = items[parentIndex];
            if (item.CompareTo(parentItem) <= 0)
                break;

            Swap(item, parentItem);
            parentIndex = (item.HeapIndex - 1) / 2;
        }
    }
    private void Swap(T itemA, T itemB)
    {
        items[itemA.HeapIndex] = itemB;
        items[itemB.HeapIndex] = itemA;

        (itemA.HeapIndex, itemB.HeapIndex) = (itemB.HeapIndex, itemA.HeapIndex);
    }
}