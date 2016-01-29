using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HomericLibrary.Tokens;
using HomericLibrary.Tokens.Values;
using System.Threading;
using System.Globalization;

namespace HomericLibrary
{
    public class HomericExpression
    {
        public string Expression { get; protected set; }
        protected List<Token> polishTokens = new List<Token>();
        protected List<Token> reversePolishTokens = new List<Token>();

        #region Dictionaries

        protected Dictionary<string, double> constants = new Dictionary<string, double>() {
            { "e", Math.E },
            { "pi", Math.PI }
        };

        protected Dictionary<string, double> variables = new Dictionary<string, double>();

        protected Dictionary<string, Func<double, double>> functions = new Dictionary<string, Func<double, double>>
            {
                { "abs", (argument) => Math.Abs(argument) },
                { "acos", (argument) => Math.Acos(argument) },
                { "actan", (argument) => Math.PI / 2 - Math.Atan(argument) },
                { "asin", (argument) => Math.Asin(argument) },
                { "atan", (argument) => Math.Atan(argument) },
                { "ceiling", (argument) => Math.Ceiling(argument) },
                { "cos", (argument) => Math.Cos(argument) },
                { "cosh", (argument) => Math.Cosh(argument) },
                { "ctan", (argument) => 1 / Math.Tan(argument) },
                { "ctanh", (argument) => 1 / Math.Tanh(argument) },
                { "fact", (argument) => fact(argument) },
                { "floor", (argument) => Math.Floor(argument) },
                { "lg", (argument) => Math.Log(argument, 10) },
                { "ln", (argument) => Math.Log(argument, Math.E) },
                { "sign", (argument) => Math.Sign(argument) },
                { "sin", (argument) => Math.Sin(argument) },
                { "sinh", (argument) => Math.Sinh(argument) },
                { "sqrt", (argument) => Math.Sqrt(argument) },
                { "tan", (argument) => Math.Tan(argument) },
                { "tanh", (argument) => Math.Tanh(argument) }
            };

        protected static double fact(double argument)
        {
            if (argument < 0)
            {
                throw new ArgumentException("Argument must be more then zero!");
            }

            double result = 1;
            for (int i = 1; i <= argument; ++i)
            {
                result *= i;
            }
            return result;
        }

        protected Dictionary<string, Func<double, double, double>> operators = new Dictionary<string, Func<double, double, double>>
            {
                { "+", (op1, op2) => op2 + op1 },
                { "-", (op1, op2) => op2 - op1 },
                { "*", (op1, op2) => op2 * op1 },
                { "/", (op1, op2) => op2 / op1 },
                { "%", (op1, op2) => op2 % op1 },
                { "^", (op1, op2) => Math.Pow(op2, op1) },
                { "(", null},
                { ")", null}
            };

        #endregion

        #region Constructors

        public HomericExpression(string expression) : this(expression, new String[] { }) { }

        public HomericExpression(string expression, params string[] vars)
        {
            // Use universal standard for parsing numbers. More info in msdn:
            // http://msdn.microsoft.com/en-us//library/system.globalization.cultureinfo.invariantculture.aspx
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            foreach (var v in vars)
            {
                if (!variables.ContainsKey(v))
                {
                    variables.Add(v, 0);
                }
                else
                {
                    throw new ArgumentException("You add the same variable multiple times");
                }
            }

            Expression = prepareExpression(expression);
            prepareTokens(Expression);
        }

        #endregion

        private void prepareTokens(string expression)
        {
            polishTokens = divideIntoTokens(expression);
            reversePolishTokens = toReversePolishNotation(polishTokens);
        }

        private string prepareExpression(string expression)
        {
            if (string.IsNullOrEmpty(expression))
            {
                throw new ArgumentException("Expression is null or empty!");
            }

            // Convert all symbols in text to lowercase 
            // and create StringBuilder object for quicker string modifying.
            StringBuilder resultStringBuilder = new StringBuilder(expression.ToLower());

            // Remove spaces
            resultStringBuilder = resultStringBuilder.Replace(" ", string.Empty);

            // Replacve ',' with '.'
            resultStringBuilder = resultStringBuilder.Replace(',', '.');

            // Resolve unary minus outside math expression. 
            // Thus, if expression starts with '-' insert '0' at the beginning.
            // e.g.: -x*sin(y) -> 0-x*sin(y)
            if (resultStringBuilder[0] == '-')
            {
                resultStringBuilder.Insert(0, "0");
            }

            // Resolve unary minus inside math expression. 
            // e.g.: Sin(-x) -> Sin(0-x)
            resultStringBuilder = resultStringBuilder.Replace("(-", "(0-");

            return resultStringBuilder.ToString();
        }

        protected List<Token> divideIntoTokens(string expression)
        {
            List<Token> tokens = new List<Token>();

            int i = 0;
            while (true)
            {
                string current = expression[i].ToString();

                if (operators.ContainsKey(current))
                {
                    tokens.Add(new Operator(current));
                    if (i < expression.Length - 1)
                    {
                        ++i;
                        continue;
                    }
                    break;
                }

                #region build something

                string something = "";
                while (!operators.ContainsKey(expression[i].ToString()))
                {
                    something += expression[i];
                    if (i < expression.Length - 1)
                    {
                        if (!operators.ContainsKey(expression[i + 1].ToString()))
                        {
                            ++i;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                #endregion

                try
                {
                    tokens.Add(parseSomething(something));
                }
                catch (Exception e)
                {
                    throw e;
                }

                if (i < expression.Length - 1)
                {
                    ++i;
                    continue;
                }
                else
                {
                    break;
                }
            }

            return tokens;
        }

        private Token parseSomething(string something)
        {
            if (variables.ContainsKey(something))
            {
                return new Variable(something);
            }

            if (constants.ContainsKey(something))
            {
                return new Constant(something);
            }

            if (functions.ContainsKey(something))
            {
                return new Function(something);
            }

            double numberOut;
            if (double.TryParse(something, out numberOut))
            {
                return new Number(something);
            }

            throw new ArgumentException("Invalid token.");
        }

        /// <summary>
        /// Converts list of tokens in polish notation into reverse polish notation
        /// </summary>
        /// <param name="polishNotation">List of tokens in polish notation</param>
        /// <returns>List of tokens in reverse polish notation</returns>
        protected List<Token> toReversePolishNotation(List<Token> polishNotation)
        {
            List<Token> reversePolishNotation = new List<Token>();
            Stack<Token> stack = new Stack<Token>();

            foreach (Token t in polishNotation)
            {
                if (t is Value)
                {
                    reversePolishNotation.Add(t);
                    continue;
                }

                if (t is Function)
                {
                    stack.Push(t);
                    continue;
                }

                if (t is Operator && t.Lexeme == "(")
                {
                    stack.Push(t);
                    continue;
                }

                if (t is Operator && t.Lexeme != ")")
                {
                    if (stack.Count != 0)
                    {
                        while (stack.Peek() is Operator && t.Priority <= stack.Peek().Priority)
                        {
                            reversePolishNotation.Add(stack.Pop());
                            if (stack.Count == 0)
                            {
                                break;
                            }
                        }
                        stack.Push(t);
                    }
                    else
                    {
                        stack.Push(t);
                    }
                    continue;
                }

                if (t is Operator && t.Lexeme == ")")
                {
                    if (stack.Count != 0)
                    {
                        while (stack.Peek().Lexeme != "(")
                        {
                            reversePolishNotation.Add(stack.Pop());
                            if (stack.Count == 0)
                            {
                                break;
                            }
                        }
                        stack.Pop();
                        if (stack.Count != 0)
                        {
                            if (stack.Peek() is Function)
                            {
                                reversePolishNotation.Add(stack.Pop());
                            }
                        }
                    }
                }
            }

            while (stack.Count != 0)
            {
                reversePolishNotation.Add(stack.Pop());
            }

            return reversePolishNotation;
        }

        /// <summary>
        /// Calculate mathematical expression without variables; general calculation algorithm
        /// </summary>
        /// <returns>The result of calculated expression</returns>
        public double Calculate()
        {
            Stack<Token> stack = new Stack<Token>();

            foreach (Token t in reversePolishTokens)
            {
                if (t is Number)
                {
                    stack.Push(t);
                    continue;
                }

                if (t is Operator)
                {
                    string operand1Str = stack.Pop().Lexeme.Replace(',', '.');
                    string operand2Str = stack.Pop().Lexeme.Replace(',', '.');
                    double operand1 = double.Parse(operand1Str, CultureInfo.InvariantCulture);
                    double operand2 = double.Parse(operand2Str, CultureInfo.InvariantCulture);
                    double resultNum = operators[t.Lexeme](operand1, operand2);
                    Token result = new Number(resultNum.ToString());
                    stack.Push(result);
                    continue;
                }

                if (t is Function)
                {
                    string argumentStr = stack.Pop().Lexeme.Replace(',', '.');
                    double argumentNum = double.Parse(argumentStr, CultureInfo.InvariantCulture);
                    double resultNum = functions[t.Lexeme](argumentNum);
                    Token result = new Number(resultNum.ToString());
                    stack.Push(result);
                    continue;
                }
                if (t is Constant)
                {
                    double constant = constants.First(c => c.Key == t.Lexeme).Value;
                    Token result = new Number(constant.ToString());
                    stack.Push(result);
                    continue;
                }
                if (t is Variable)
                {
                    KeyValuePair<string, double> v = variables.First(n => n.Key == t.Lexeme);
                    Token result = new Number(v.Value.ToString());
                    stack.Push(result);
                    continue;
                }
                else
                {
                    throw new ArgumentException("Bad expression string!");
                }
            }
            double totalResult = 0;
            try
            {
                totalResult = double.Parse(stack.Pop().Lexeme, CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Something wrong with expression. Can't calculate value. " + ex.Message);
            }

            return totalResult;
        }

        /// <summary>
        /// Calculate mathematical expression with variables.
        /// </summary>
        /// <param name="values">Varables' values</param>
        /// <returns>The result of calculated expression</returns>
        public double Calculate(params double[] values)
        {
            updateVariables(values);
            return Calculate();            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public double Calculate(Dictionary<string, double> values)
        {
            updateVariables(values);
            return Calculate();
        }

        private void updateVariables(double[] values)
        {
            for (int i = 0; i < values.Length; ++i)
            {
                var key = variables.Keys.ToArray()[i];
                variables[key] = values[i];
            }
        }

        private void updateVariables(Dictionary<string, double> values)
        {
            try
            {
                foreach (var variable in values)
                {
                    variables[variable.Key] = variable.Value;
                }
            }
            catch
            {
                throw new ArgumentException("You try to set unregistered variable.");
            }
        }
    }
}
