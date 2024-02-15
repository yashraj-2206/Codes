using System;

namespace Array
{
    /// <summary>
    /// Class containing methods for taking input, displaying and soring of 2d array
    /// </summary>
    internal class Demo
    {
        /// <summary>
        /// Runs demo program code for Array program
        /// </summary>
        public void Run()
        {
            try
            {
                // Display heading
                DisplayHeading("Sorting of two dimensional array");

                // Geting number of rows from user
                int rowCount = GetUserRowCount();

                // Get user array from user inputs
                int[,] unsortedArray = GetArray(rowCount, Constants.COLUMN_COUNT);

                // Display array
                Console.WriteLine("\n\nYou have entered below array values:");
                DisplayArray(unsortedArray);

                // Ask for column to sort
                ColumnName columnToBeSorted = GetColumnNameToBeSorted();

                // Ask for sorting order
                SortingOrder sortingOrder = GetSortingOrder();

                // New Array to store sorted elements
                int[,] sortedArray = new int[rowCount, Constants.COLUMN_COUNT];

                // sorting according to desired condition
                if (sortingOrder == SortingOrder.Ascending)
                {
                    sortedArray = SortColumnElementsInAscending(unsortedArray, columnToBeSorted);
                }
                else if (sortingOrder == SortingOrder.Descending)
                {
                    sortedArray = SortColumnElementsInDescending(unsortedArray, columnToBeSorted);
                }

                // Display sorted array
                Console.WriteLine("\n\nSorted 2D array:");
                DisplayArray(sortedArray);
            }
            catch (Exception e)
            {
                Console.WriteLine("Array sorting operation failed\n");
                Console.WriteLine($"Detail Error: {e.Message}\nStack Trace: {e.StackTrace}");
            }
        }

        /// <summary>
        /// Taking input and validation for number of rows
        /// </summary>
        /// <returns> Number of rows user wants </returns>
        public int GetUserRowCount()
        {
            int rowCount;

            // Keep asking till correct value is returned
            while (true)
            {
                Console.Write($"\nInput array row count[{Constants.MIN_ROW_COUNT} to {Constants.MAX_ROW_COUNT}]: ");

                bool isValidRowCount = int.TryParse(Console.ReadLine(), out rowCount);

                // Checking condition for acceptable row count
                if (isValidRowCount && Constants.MIN_ROW_COUNT <= rowCount && rowCount <= Constants.MAX_ROW_COUNT)
                {
                    Console.WriteLine();
                    return rowCount;
                }
                else
                {
                    Console.WriteLine("\nEntered array size is invalid.");
                }
            }
        }

        /// <summary>
        /// Takes input, validates and stores the elements from user
        /// </summary>
        /// <param name="userElements"> Array to be stored in </param>
        /// <param name="rowCount"> Number of rows of the array </param>
        public int[,] GetArray(int rowCount, int columnCount)
        {
            int[,] array = new int[rowCount, columnCount];

            // Iterate over each row in 2d array
            for (int i = 0; i < rowCount; i++)
            {
                // Iterate over each position in the current row
                for (int j = 0; j < columnCount; j++)
                {
                    // Validate the user input until valid value is entered
                    while (true)
                    {
                        Console.Write($"Enter element for [{i},{j}]: ");

                        // Prompt the user to enter a valid integer
                        if (int.TryParse(Console.ReadLine(), out array[i, j]))
                        {
                            Console.WriteLine();
                            break;
                        }
                        else
                        {
                            Console.WriteLine("\nEntered array element is invalid.\n");
                        }
                    }
                }
            }

            return array;
        }

        /// <summary>
        /// To display elements of the 2D array
        /// </summary>
        /// <param name="array"> Array to be displayed </param>
        public void DisplayArray(int[,] array)
        {

            int maxLengthOfIntegerInput = 0;
            int columnCount = array.GetLength(1);

            // Gets the length of the longest integer in the array
            foreach (int i in array)
            {
                int currentElementLength = i.ToString().Length;

                // Check if current element has greater length than the maxLengthIntegerInput
                if (currentElementLength > maxLengthOfIntegerInput)
                {
                    maxLengthOfIntegerInput = currentElementLength;
                }

            }

            // Padding 
            Console.WriteLine($"\n\n\t{"X".PadLeft(maxLengthOfIntegerInput / 2)}{"Y".PadLeft(maxLengthOfIntegerInput + 2)}");
            Console.WriteLine($"\t{new string('-', 2 * maxLengthOfIntegerInput + 2)}");

            int iteration = 0;

            // Iterate the array and display the elements
            foreach (int i in array)
            {
                string padding = new string(' ', (maxLengthOfIntegerInput - i.ToString().Length) / 2);

                // Checking for the end of row
                if (iteration % columnCount == 0)
                {
                    Console.Write($"\t{padding}{i}{padding}  ");
                }
                else
                {
                    Console.WriteLine($"{padding}{i}");
                }

                iteration++;
            }
        }

        /// <summary>
        /// Takes input and validates the column to be sorted
        /// </summary>
        /// <returns> The column name which the user wants to sort </returns>
        public ColumnName GetColumnNameToBeSorted()
        {
            string firstColumn = ColumnName.X.ToString();
            string secondColumn = ColumnName.Y.ToString();

            // Prompt user to enter a valid column name
            while (true)
            {
                Console.Write($"\nOn which column you want to sort(Enter {firstColumn}/{firstColumn.ToLower()} or {secondColumn}/{secondColumn.ToLower()}): ");

                string columnName = Console.ReadLine().ToUpper();
 
                switch (columnName)
                {
                    case Constants.FIRST_COLUMN_NAME:
                        return ColumnName.X;

                    case Constants.SECOND_COLUMN_NAME:
                        return ColumnName.Y;

                    default:
                        Console.WriteLine("\nEntered sorting column is invalid");
                        break;
                }
            }
        }

        /// <summary>
        /// Get the order to sort the areray
        /// </summary>
        /// <returns> The order to sort the array </returns>
        public SortingOrder GetSortingOrder()
        {
            string ascending = SortingOrder.Ascending.ToString();
            string descending = SortingOrder.Descending.ToString();

            // Prompt user to enter a valid sorting order 
            while (true)
            {
                Console.Write($"Which sorting type you want to apply(Enter {(int)SortingOrder.Ascending} for {ascending}, {(int)SortingOrder.Descending} for {descending}): ");


                // Prompt user to enter non-negative and valid input
                if (int.TryParse(Console.ReadLine(), out int sortingType) && sortingType >= 0)
                {
                    if (sortingType == (int)SortingOrder.Ascending)
                    {
                        return SortingOrder.Ascending;
                    }
                    else if (sortingType == (int)SortingOrder.Descending)
                    {
                        return SortingOrder.Descending;
                    }
                    else
                    {
                        Console.WriteLine("\nEntered sorting order is out of the predefined range.\n");
                    }
                }
                else
                {
                    Console.WriteLine("\nEntered sorting order is invalid\n");
                }
            }
        }

        /// <summary>
        /// To sort column elements in ascending order
        /// </summary>
        /// <param name="unsortedArray"> Array to be sorted </param>
        /// <param name="columnToBeSorted"> Column to be sorted </param>
        public int[,] SortColumnElementsInAscending(int[,] unsortedArray, ColumnName columnToBeSorted)
        {
            int[,] sortedArray = unsortedArray;
            int rowCount = sortedArray.GetLength(0);
            int columnCount = unsortedArray.GetLength(1);

            // Bubble sort
            for (int i = 0; i < (rowCount - 1); i++)
            {
                for (int j = 0; j < (rowCount - 1); j++)
                {
                    if (sortedArray[j, (int)columnToBeSorted] > sortedArray[j + 1, (int)columnToBeSorted])
                    {
                        // Swaping the wrongly ordered elements with all the resr column value for the current row
                        for (int k = 0; k < columnCount; k++)
                        {
                            (sortedArray[j + 1, k], sortedArray[j, k]) = (sortedArray[j, k], sortedArray[j + 1, k]);
                        }
                    }
                }
            }

            return sortedArray;
        }

        /// <summary>
        /// To sort column elements in ascending order
        /// </summary>
        /// <param name="unsortedArray"> Array to be sorted </param>
        /// <param name="columnToBeSorted"> Column to be sorted </param>
        public int[,] SortColumnElementsInDescending(int[,] unsortedArray, ColumnName columnToBeSorted)
        {
            int[,] sortedArray = unsortedArray;
            int rowCount = sortedArray.GetLength(0);
            int columnCount = sortedArray.GetLength(1);

            // Bubble sort
            for (int i = 0; i < (rowCount - 1); i++)
            {
                for (int j = 0; j < (rowCount - 1); j++)
                {
                    if (sortedArray[j, (int)columnToBeSorted] < sortedArray[j + 1, (int)columnToBeSorted])
                    {
                        // Swaping the wrongly ordered elements with all the resr column value for the current row
                        for (int k = 0; k < columnCount; k++)
                        {
                            (sortedArray[j + 1, k], sortedArray[j, k]) = (sortedArray[j, k], sortedArray[j + 1, k]);
                        }
                    }
                }
            }

            return sortedArray;
        }


        /// <summary>
        /// To display paterend heading
        /// </summary>
        /// <param name="message"> Message to display in heading </param>
        public void DisplayHeading(string message)
        {
            Console.WriteLine(Constants.BORDER);
            Console.WriteLine(message);
            Console.WriteLine(Constants.BORDER);
        }

    }
}
