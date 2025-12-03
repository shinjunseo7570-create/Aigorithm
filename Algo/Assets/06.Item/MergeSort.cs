using UnityEngine;

public static class MergeSort
{
    

    public static void Sort(ItemData[] list)
    {
        ItemData[] sorted = new ItemData[list.Length];
        Merge_Sort(list, sorted, 0, list.Length - 1);
    }

    public static void Merge_Sort(ItemData[] list, ItemData[] sorted, int left, int right)
    {
        if(left < right)
        {
            int mid = (left + right) / 2;
            Merge_Sort(list, sorted, left, mid);
            Merge_Sort(list, sorted, mid + 1, right);
            Merge(list, sorted, left, mid, right);
        }
    }

    public static void Merge(ItemData[] list, ItemData[] sorted, int left, int mid, int right)
    {
        int i = left;
        int j = mid + 1;
        int k = left;

        while (i <= mid && j <= right)
        {
            if (list[i].grade <= list[j].grade)
                sorted[k++] = list[i++];
            else
                sorted[k++] = list[j++];
        }

        while (i <= mid)
            sorted[k++] = list[i++];

        while (j <= right)
            sorted[k++] = list[j++];

        for (int l = left; l <= right; l++)
            list[l] = sorted[l];
    }   
}
