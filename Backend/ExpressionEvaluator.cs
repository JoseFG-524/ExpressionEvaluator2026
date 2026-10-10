using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection.Metadata;
//Backend
namespace Backend;

public static class ExpressionEvaluator
{
    public static double Evalute(string infix) => EvalutePostfix(ToPostfix(infix));

    // 1. Tokenizer to separate numbers (integers and decimals) and operators
    private static List<string> Tokenize(string infix)
    {
        var tokens = new List<string>();
        string currentNumber = string.Empty;

        foreach (var c in infix)
        {
            if (char.IsWhiteSpace(c)) continue;

            if (char.IsDigit(c) || c == '.')
            {
                currentNumber += c;
            }
            else
            {
                if (!string.IsNullOrEmpty(currentNumber))
                {
                    tokens.Add(currentNumber);
                    currentNumber = string.Empty;
                }
                tokens.Add(c.ToString());
            }
        }

        if (!string.IsNullOrEmpty(currentNumber))
        {
            tokens.Add(currentNumber);
        }

        return tokens;
    }

    private static List<string> ToPostfix(string infix)
    {
        var tokens = Tokenize(infix);
        var postfix = new List<string>();
        var stack = new Stack<string>();

        foreach (var token in tokens)
        {
            if (IsOperator(token))
            {
                if (token == ")")
                {
                    var ope = stack.Pop();
                    while (ope != "(")
                    {
                        postfix.Add(ope);
                        ope = stack.Pop();
                    }
                }
                else
                {
                    if (stack.Count == 0)
                    {
                        stack.Push(token);
                    }
                    else
                    {
                        if (PriorityInfix(token) > PriorityStack(stack.Peek()))
                        {
                            stack.Push(token);
                        }
                        else
                        {
                            postfix.Add(stack.Pop());
                            stack.Push(token);
                        }
                    }
                }
            }
            else
            {
                postfix.Add(token);
            }
        }

        while (stack.Count > 0)
        {
            postfix.Add(stack.Pop());
        }

        return postfix;
    }

    private static int PriorityStack(string op) => op switch
    {
        "^" => 3,
        "*" => 2,
        "/" => 2,
        "+" => 1,
        "-" => 1,
        "(" => 0,
        _ => throw new Exception("Invalid expression."),
    };

    private static int PriorityInfix(string op) => op switch
    {
        "^" => 4,
        "*" => 2,
        "/" => 2,
        "+" => 1,
        "-" => 1,
        "(" => 5,
        _ => throw new Exception("Invalid expression."),
    };

    private static bool IsOperator(string token) =>
        token == "^" || token == "*" || token == "/" || token == "+" || token == "-" || token == "(" || token == ")";

    private static double EvalutePostfix(List<string> postfix)
    {
        var stack = new Stack<double>();
        foreach (var token in postfix)
        {
            if (IsOperator(token))
            {
                var ope2 = stack.Pop();
                var ope1 = stack.Pop();
                stack.Push(Calculate(ope1, ope2, token));
            }
            else
            {
                // We use InvariantCulture to ensure that the decimal point is interpreted correctly
                stack.Push(double.Parse(token, CultureInfo.InvariantCulture));
            }
        }
        return stack.Pop();
    }

    private static double Calculate(double ope1, double ope2, string item) => item switch
    {
        "*" => ope1 * ope2,
        "/" => ope1 / ope2,
        "+" => ope1 + ope2,
        "-" => ope1 - ope2,
        "^" => Math.Pow(ope1, ope2),
        _ => throw new Exception("Invalid expression."),
    };
}