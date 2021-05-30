using System;
using System.Threading;
using System.Threading.Tasks;

namespace Mergesoft_threaded
{
    class Program
    {
        
        static void Main(string[] args)
        {
            // Creating a random array to sort
            MergeSort m = new MergeSort();
            int[] array = new int[10];
            array = MergeSort.createArray(array);
            Console.WriteLine("Unsorted random array: \n");
            MergeSort.showArray(array);
            Console.WriteLine("\n\nSorted array:");

            array = m.mergeSort(array);
            MergeSort.showArray(array);

        }
    }
}


class MergeSort
{
    private readonly object lockk = new object();
    public int[] mergeSort(int[] array)
    {
        int[] left;
        int[] right;
        int[] result = new int[array.Length];
        # region As this is a recursive algorithm, we need to have a base case to avoid an infinite recursion and therfore a stackoverflow
        if (array.Length <= 1)
            return array;
        #endregion

        #region The exact midpoint of our array
        int midPoint = array.Length / 2;
        #endregion

        #region Will represent our 'left' array
        left = new int[midPoint];
        #endregion

        # region if array has an even number of elements, the left and right array will have the same number of elements
        if (array.Length % 2 == 0)
            right = new int[midPoint];
        #endregion
        # region if array has an odd number of elements, the right array will have one more element than left
        else
            right = new int[midPoint + 1];
        #endregion

        # region populate left array
        for (int i = 0; i < midPoint; i++)
            left[i] = array[i];
        #endregion
        # region populate right array   
        int x = 0;
        //We start our index from the midpoint, as we have already populated the left array from 0 to midpont
            for (int i = midPoint; i < array.Length; i++)
        {
            right[x] = array[i];
            x++;
        }
        #endregion

        // there we can spread left and right array into two separate tasks:
        var leftt = Task.Factory.StartNew(() => mergeSort(left));
        var rightt = Task.Factory.StartNew(() => mergeSort(right));

        # region Recursively sort the left array THIS PART CAN BE SPLIT TO THREADS/TASKS
        left = mergeSort(left);
        #endregion
        #region Recursively sort the right array THIS PART CAN BE SPLIT TO THREADS/TASKS
        //Task.Factory.StartNew(() => right = mergeSort(right));
        right = mergeSort(right);
        #endregion
        #region Merge our two sorted arrays THIS PART HAS TO BE MERGED, CAN BE SPLIT TO THREADS/TASKS
        lock(lockk) { result = merge(left, right); }        
        return result;
        #endregion
    }

    //This method will be responsible for combining our two sorted arrays into one giant array
    public int[] merge(int[] left, int[] right)
    {
        #region Creating array, length of which is a sum of left and right arrays' lenghts
        int[] result = new int[right.Length + left.Length];
        #endregion

        int indexLeft = 0, indexRight = 0, indexResult = 0;
        //while either array still has an element
        while (indexLeft < left.Length || indexRight < right.Length)
        {
            //if both arrays have elements  
            if (indexLeft < left.Length && indexRight < right.Length)
            {
                //If item on left array is less than item on right array, add that item to the result array 
                if (left[indexLeft] <= right[indexRight])
                {
                    result[indexResult] = left[indexLeft];
                    indexLeft++;
                    indexResult++;
                }
                // else the item in the right array wll be added to the results array
                else
                {
                    result[indexResult] = right[indexRight];
                    indexRight++;
                    indexResult++;
                }
            }
            //if only the left array still has elements, add all its items to the results array
            else if (indexLeft < left.Length)
            {
                result[indexResult] = left[indexLeft];
                indexLeft++;
                indexResult++;
            }
            //if only the right array still has elements, add all its items to the results array
            else if (indexRight < right.Length)
            {
                result[indexResult] = right[indexRight];
                indexRight++;
                indexResult++;
            }
        }
        return result;
    }

    public static int[] createArray(int[] array)
    #region Creates a random array with length of provided array
    {
        Random rnd = new Random();
        //int[] array = new int[10];

        for (int i = 0; i < array.Length; i++)
        {
            array[i] = rnd.Next(1, 26);
        }

        return array;
    }
    #endregion

    public static void showArray(int[] array)
    #region Displays provided array
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (i == 0) Console.Write("{");
            Console.Write(array[i]);
            if (i == (array.Length - 1)) { Console.Write("} \n"); break; }
            Console.Write(" ");
        }
    }
    #endregion
}