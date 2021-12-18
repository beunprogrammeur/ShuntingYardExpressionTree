using ShuntingYardAlgorithm;
using ShuntingYardAlgorithm.Expression;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Calculator
{
    class Program
    {
        static void Main(string[] args)
        {
            var variables = new Dictionary<string, Func<double>> 
            { 
                { "pi",    () => Math.PI },
                { "unix",  () => DateTimeOffset.Now.ToUnixTimeSeconds() },
                { "epoch", () => DateTimeOffset.Now.ToUnixTimeMilliseconds() },
            };


            string input = null;
            if(!args.Any())
            {
                Console.WriteLine("available variables:");
                foreach(var pair in variables)
                {
                    Console.WriteLine($"{pair.Key}: {pair.Value()}");
                }
                Console.WriteLine();
                Console.WriteLine("supported operators: +, -, *, /, ()");
                Console.WriteLine();
                Console.Write("type your formula: ");
                input = Console.ReadLine();
            }
            else
            {
                input = args[0];
            }

            var parser = new Parser();

            foreach (var variable in variables)
            {
                parser.RegisterVariable(new RelayProperty(variable.Key, variable.Value));
            }

            var formula = parser.ParseFormula(input);
            var result  = formula?.Value;
            Console.WriteLine($"result: {result}");
        }
    }
}
