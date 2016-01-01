using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HomericLibrary;

namespace HomericConsoleTest
{
    public class Program
    {
        private static HomericExpression formula;

        public static void Main(string[] args)
        {
            Console.WriteLine("HOMERIC :: Type 'info' to show visible commands or 'exit' to close application.");
            while (true)
            {
                string line = Console.ReadLine().ToLower();
                string[] lineArr = line.Split(' ');

                if (lineArr[0].Equals("info"))
                {
                    Console.WriteLine(
                        "HOMERIC Command line sample application commands:\n" +
                        "current                         : Show current expression\n" +
                        "new <formula>                   : Create new expression without parameters\n" +
                        "new <formula> -args <arguments> : Create new expression with arguments\n" +
                        "calculate                       : Calculate expression (all arguments are 0 by default)\n" +
                        "calculate <values>              : Calculate with custom values\n" +
                        "exit                            : Close the application"
                        );
                }

                if (lineArr[0].Equals("exit"))
                {
                    break;
                }

                if (line.Equals("current"))
                {
                    if (formula != null)
                    {
                        Console.WriteLine(formula);
                    }
                    else
                    {
                        Console.WriteLine("You don't have any formula.");
                    }
                }

                if (lineArr[0].Equals("new"))
                {
                    if (lineArr.Length > 1)
                    {
                        string expression = string.Empty;
                        List<string> arguments = new List<string>();
                        for (int i = 1; i < lineArr.Length; ++i)
                        {
                            if (lineArr[i] == "-args")
                            {
                                if (i < lineArr.Length - 1)
                                {
                                    for (int j = i + 1; j < lineArr.Length; ++j)
                                    {
                                        arguments.Add(lineArr[j]);
                                    }
                                    break;
                                }
                            }
                            else
                            {
                                expression += lineArr[i];
                            }
                        }
                        try
                        {
                            formula = new HomericExpression(expression, arguments.ToArray());
                            Console.WriteLine("Formula created successfully!");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error creating formula. Details:\n{0}", e.Message);
                        }
                    }
                }

                if (lineArr[0].Equals("calculate"))
                {
                    var values = new List<double>();
                    if (lineArr.Length > 1)
                    {
                        for (int i = 1; i < lineArr.Length; ++i)
                        {
                            try
                            {
                                values.Add(double.Parse(lineArr[i]));
                            }
                            catch
                            {
                                Console.WriteLine("Error parsing data. '{0}' is not a number or has invalid format", lineArr[i]);
                            }
                        }
                    }
                    try
                    {
                        double result;
                        if (lineArr.Length > 0)
                        {
                            result = formula.Calculate(values.ToArray());
                        }
                        else
                        {
                            result = formula.Calculate();
                        }
                        Console.WriteLine("Result: {0}", result);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error calculating formula. Details:\n{0}", e.Message);
                    }
                }
            }
        }
    }
}
