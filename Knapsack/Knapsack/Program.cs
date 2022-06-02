using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Knapsack
{
    internal class Item
    {
        public int Value { get; set; }
        public int Weight { get; set; }
        public override string ToString()
        {
            return $"V: {Value}, W: {Weight}";
        }
    }

    internal class Program
    {
        public static string FilePath;
        public static int Capacity;
        public static Item[] Items = new Item[30];
        public static int[] CharacteristicVector;
        public static double NumberOfAllSolutions;

        public static int NumberOfFeasibleSolutions,
            CurrentWeight,
            CurrentValue,
            TotalWeightOfItemsInKnapsack,
            TotalValueOfItemsInKnapsack;

        public static readonly int ItemIsTaken = 1;

        private static readonly Stopwatch Stopwatch = new();

        private static async Task Main(string[] args)
        {
            if (args.Length <= 0)
                throw new ArgumentException("File not found!");
            FilePath = args[0];
            await ReadFromFileAsync();
            Console.WriteLine("Working...");
            var timeSpan = Brute_force();
            DisplaySummary(timeSpan);
        }


        public static async Task ReadFromFileAsync()
        {
            
            using (var reader = new StreamReader(FilePath))
            {
                _ = int.TryParse(await reader.ReadLineAsync(), out Capacity);
                string line;
                int test = 0;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    var values = line.Split(" ");
                    var item = new Item
                    {
                        Value = int.Parse(values[0]),
                        Weight = int.Parse(values[1])
                    };
                    Items[test] = item;
                    test++;
                }
            }
        }

        public static TimeSpan Brute_force()
        {
            Stopwatch.Start();
            NumberOfAllSolutions = Math.Pow(2, Items.Length);
            for (var i = 0; i < NumberOfAllSolutions; i++)
            {
                var binaryConvertedNumbers = ConvertToBinary(i);
                if (CheckFeasibility(binaryConvertedNumbers)) //Capacity >= CurrentWeight;
                {
                    //NumberOfFeasibleSolutions++;
                    if (CheckOptimality(binaryConvertedNumbers)) //CurrentValue >= TotalValueOfItemsInKnapsack;
                    {
                        TotalValueOfItemsInKnapsack = CurrentValue;
                        TotalWeightOfItemsInKnapsack = CurrentWeight;
                        CharacteristicVector = binaryConvertedNumbers;
                    }
                }
            }

            var timeSpan = Stopwatch.Elapsed;
            Stopwatch.Stop();
            return timeSpan;
        }

        public static int[] ConvertToBinary(int i)
        {
            var result = new int[Items.Length];
            var test = 0;
            while (i > 0)
            {
                var rest = i % 2;
                i /= 2;
                result[test] = rest;
            }

            return result;
        }

        public static bool CheckFeasibility(int[] binaryValues)
        {
            CurrentWeight = 0;
            var i = 0;
            foreach (var item in Items)
            {
                if (binaryValues.ElementAtOrDefault(i) == ItemIsTaken)
                    CurrentWeight += item.Weight;
                i++;
            }

            return Capacity >= CurrentWeight;
        }

        public static bool CheckOptimality(int[] binaryValues)
        {
            CurrentValue = 0;
            var i = 0;
            foreach (var item in Items)
            {
                if (binaryValues.ElementAtOrDefault(i) == ItemIsTaken)
                    CurrentValue += item.Value;
                i++;
            }

            return CurrentValue >= TotalValueOfItemsInKnapsack;
        }

        public static void DisplaySummary(TimeSpan timeSpan)
        {
            Console.Clear();
            Console.WriteLine("Program stopped after: {0:00}:{1:00}:{2:00}", timeSpan.Minutes, timeSpan.Seconds,
                timeSpan.Milliseconds);
            Console.WriteLine(
                $"Total number of solutions: {NumberOfAllSolutions}\nNumber of feasible solutions: 0");
            Console.WriteLine("**************************************");
            Console.WriteLine(
                $"Optimal Solution: \nKnapsack capacity: {Capacity}\nTotal weight of elements in Knapsack: {TotalWeightOfItemsInKnapsack}" +
                $"\nTotal value of elements in Knapsack: {TotalValueOfItemsInKnapsack}\nFinal characteristic vector: {string.Join(",", CharacteristicVector)}");
        }
    }
}