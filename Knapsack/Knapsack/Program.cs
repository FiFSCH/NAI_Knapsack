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
        public static List<Item> Items;
        public static List<int> CharacteristicVector;
        public static double NumberOfAllSolutions;

        public static int NumberOfFeasibleSolutions,
            CurrentWeight,
            CurrentValue,
            TotalWeightOfItemsInKnapsack,
            TotalValueOfItemsInKnapsack;

        private static readonly Stopwatch Stopwatch = new();
        private static async Task Main(string[] args)
        {
            if (args.Length <= 0)
                throw new ArgumentException("File not found!");
            FilePath = args[0];
            await ReadFromFileAsync();
            var timeSpan = Brute_force();
            DisplaySummary(timeSpan);
        }


        public static async Task ReadFromFileAsync()
        {
            Items = new List<Item>();
            using (var reader = new StreamReader(FilePath))
            {
                _ = int.TryParse(await reader.ReadLineAsync(), out Capacity);
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    var values = line.Split(" ");
                    var item = new Item
                    {
                        Value = int.Parse(values[0]),
                        Weight = int.Parse(values[1])
                    };
                    Items.Add(item);
                }
            }
        }

        public static TimeSpan Brute_force()
        {
            Stopwatch.Start();
            NumberOfAllSolutions = Math.Pow(2, Items.Count);
            for (var i = 0; i < NumberOfAllSolutions; i++)
            {
                var binaryConvertedNumbers = ConvertToBinary(i);
                if (CheckFeasibility(binaryConvertedNumbers)) //Capacity >= CurrentWeight;
                {
                    NumberOfFeasibleSolutions++;
                    if (CheckOptimality(binaryConvertedNumbers)) //CurrentValue >= TotalValueOfItemsInKnapsack;
                    {
                        TotalValueOfItemsInKnapsack = CurrentValue;
                        TotalWeightOfItemsInKnapsack = CurrentWeight;
                        CharacteristicVector = binaryConvertedNumbers;
                    }
                }
            }

            var timeSpan = Stopwatch.Elapsed;
            return timeSpan;
        }
        
        public static List<int> ConvertToBinary(int i)
        {
            var result = new List<int>();
            while (i > 0)
            {
                var rest = i % 2;
                i /= 2;
                result.Add(rest);
            }

            return result;
        }

        public static bool CheckFeasibility(List<int> binaryValues)
        {
            var totalWeight = 0;
            var i = 0;
            foreach (var item in Items)
            {
                if (binaryValues.ElementAtOrDefault(i) == 1)
                    totalWeight += item.Weight;
                i++;
            }

            CurrentWeight = totalWeight;
            return Capacity >= CurrentWeight;
        }

        public static bool CheckOptimality(List<int> binaryValues)
        {
            var totalValue = 0;
            var i = 0;
            foreach (var item in Items)
            {
                if (binaryValues.ElementAtOrDefault(i) == 1)
                    totalValue += item.Value;
                i++;
            }

            CurrentValue = totalValue;
            return CurrentValue >= TotalValueOfItemsInKnapsack;
        }

        public static void DisplaySummary(TimeSpan timeSpan)
        {
            Stopwatch.Stop();
            Console.WriteLine("Program stopped after: {0:00}:{1:00}:{2:00}", timeSpan.Minutes, timeSpan.Seconds,
                timeSpan.Milliseconds);
            Console.WriteLine(
                $"Total number of solutions: {NumberOfAllSolutions}\nNumber of feasible solutions: {NumberOfFeasibleSolutions}");
            Console.WriteLine("**************************************");
            Console.WriteLine(
                $"Knapsack capacity: {Capacity}\nTotal weight of elements in Knapsack: {TotalWeightOfItemsInKnapsack}" +
                $"\nTotal value of elements in Knapsack: {TotalValueOfItemsInKnapsack}\nFinal characteristic vector: {String.Join(",", CharacteristicVector)}");
        }
    }
}