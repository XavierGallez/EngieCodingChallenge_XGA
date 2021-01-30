using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;

namespace Engie
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // In a true web application the jsonString would forwarded from the rest api
                foreach (var f in Directory.EnumerateFiles("../../../example_payloads"))
                {
                    var jsonString = File.ReadAllText(f);
                    var loadComputer = new EngieCCComputer.LoadComputer(JsonSerializer.Deserialize<EngieCCComputer.Payload>(jsonString));
                    // Write summary on screen
                    {
                        Console.WriteLine("Payload file: " + f);
                        Console.WriteLine("Total available power in plants: " + loadComputer.availablePower + " MW");
                        Console.WriteLine("Required power: " + loadComputer.load + " MW");
                    }
                    // Perform optimization
                    var plantsAndCost = loadComputer.Optimize();
                    // Write result on sceen
                    {
                        Console.WriteLine("Power distribution:");
                        plantsAndCost.Item1.ForEach(lr => Console.WriteLine(" - " + lr.ToString()));
                        Console.WriteLine("Total: " + plantsAndCost.Item1.Sum(lr => lr.p) + " MW, cost: " + String.Format("{0:0,0}", plantsAndCost.Item2) + " euros");
                        Console.WriteLine("\n----------\n");
                    }
                    // Write results in json file
                    File.WriteAllText(
                        "../../../results/res_" + Path.GetFileName(f),
                        JsonSerializer.Serialize(plantsAndCost.Item1, new JsonSerializerOptions() { WriteIndented = true }));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

            }
            finally
            {

            }
        }
    }
}
