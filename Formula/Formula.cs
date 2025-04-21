// <copyright file="Formula_PS2.cs" company="UofU-CS3500">
// Copyright (c) 2024 UofU-CS3500. All rights reserved.
// </copyright>
// <authors> Jared Pratt, Prof Joe, Prof Danny, and Prof Jim</authors>
// <date> 09/20/2024 </date>


namespace CS3500.Formula;

using System.ComponentModel.Design;
using System.Text;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Threading.Tasks;
using System.Security.AccessControl;

/// <summary>
///   <para>
///     This class represents formulas written in standard infix notation using standard precedence
///     rules.  The allowed symbols are non-negative numbers written using double-precision
///     floating-point syntax; variables that consist of one ore more letters followed by
///     one or more numbers; parentheses; and the four operator symbols +, -, *, and /.
///   </para>
///   <para>
///     Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
///     a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable;
///     and "x 23" consists of a variable "x" and a number "23".  Otherwise, spaces are to be removed.
///   </para>
///   <para>
///     For Assignment Two, you are to implement the following functionality:
///   </para>
///   <list type="bullet">
///     <item>
///        Formula Constructor which checks the syntax of a formula.
///     </item>
///     <item>
///        Get Variables
///     </item>
///     <item>
///        ToString
///     </item>
///   </list>
/// </summary>
public class Formula
{
    /// <summary>
    ///   All variables are letters followed by numbers.  This pattern
    ///   represents valid variable name strings.
    /// </summary>
    private const string VariableRegExPattern = @"[a-zA-Z]+\d+";

    /// <summary>
    /// Simplified form of elements in the formula that
    /// will be called in the ToString method.
    /// </summary>
    private string canonicalForm;

    /// <summary>
    /// A list of all of the valid elements 
    /// inputted into the formula constructor.
    /// </summary>
    private List<string> tokens;

    /// <summary>
    ///   Initializes a new instance of the <see cref="Formula"/> class.
    ///   <para>
    ///     Creates a Formula from a string that consists of an infix expression written as
    ///     described in the class comment.  If the expression is syntactically incorrect,
    ///     throws a FormulaFormatException with an explanatory Message.  See the assignment
    ///     specifications for the syntax rules you are to implement.
    ///   </para>
    ///   <para>
    ///     Non Exhaustive Example Errors:
    ///   </para>
    ///   <list type="bullet">
    ///     <item>
    ///        Invalid variable name, e.g., x, x1x  (Note: x1 is valid, but would be normalized to X1)
    ///     </item>
    ///     <item>
    ///        Empty formula, e.g., string.Empty
    ///     </item>
    ///     <item>
    ///        Mismatched Parentheses, e.g., "(("
    ///     </item>
    ///     <item>
    ///        Invalid Following Rule, e.g., "2x+5"
    ///     </item>
    ///   </list>
    /// </summary>
    /// <param name="formula"> The string representation of the formula to be created.</param>
    public Formula(string formula)
    {
        this.canonicalForm = "";
        string previousToken = "";
        tokens = new List<string>();
        List<string> temp = GetTokens(formula);
        int openParenthCount = 0;
        int closedParenthCount = 0;

        if (temp.Count == 0) // Checks that at least something is inputted to the formula
        {
            throw new FormulaFormatException("The string must contain at least one valid token.");
        }

        for (int i = 0; i < temp.Count; i++)
        {
            if (i > 0)
            {
                previousToken = temp[i - 1]; //Ensures that the pervious token is not null
            }

            string token = temp[i];
            if (token == ("("))
            {
                openParenthCount++;
            }
            else if (token == (")"))
            {
                closedParenthCount++;
            }
            // Check if there are more closing parenthesis than opening parenthisis (rule 4)
            if (openParenthCount < closedParenthCount)
            {
                throw new FormulaFormatException("Unbalanced parenthesis. Too many closing parenthesis in the formula.");
            }

            //Check if the current token is valid and makes sure first token is number, variable, or opening parenthesis
            if (!IsValidToken(token) || IsOperator(temp[0]))
            {
                throw new FormulaFormatException("Invalid token. Token must be a number, variable, operators, or parenthesis.");
            }
            if (double.TryParse(token, out double value))
            {
                token = value.ToString();
            }

            //Check if the token following another token is valid according to rules 7 and 8
            if (previousToken != "" && !IsValidFollowingRule(previousToken, token))
            {
                throw new FormulaFormatException("Invalid token sequence.");
            }
            if (IsVar(token))
            {
                canonicalForm += token.ToUpper();
            }
            else
            {
                canonicalForm += token;
            }
            tokens.Add(token); //adds the modified canonical token to a list of simplified tokens

        }
        if (closedParenthCount != openParenthCount) // checks if there are more opening parenthesis than closing
        {
            throw new FormulaFormatException("Unbalanced parenthesis. Too many opening parenthesis.");
        }

        // Check the last token rule
        string lastToken = temp[temp.Count - 1];
        if (!double.TryParse(lastToken, out _) && !IsVar(lastToken) && lastToken != ")")
        {
            throw new FormulaFormatException("Last token must be a number, variable, or closing parenthesis.");
        }
    }

    /// <summary>
    ///   <para>
    ///     Returns a set of all the variables in the formula.
    ///   </para>
    ///   <remarks>
    ///     Important: no variable may appear more than once in the returned set, even
    ///     if it is used more than once in the Formula.
    ///     Variables should be returned in canonical form, having all letters converted
    ///     to uppercase.
    ///   </remarks>
    ///   <list type="bullet">
    ///     <item>new("x1+y1*z1").GetVariables() should return a set containing "X1", "Y1", and "Z1".</item>
    ///     <item>new("x1+X1"   ).GetVariables() should return a set containing "X1".</item>
    ///   </list>
    /// </summary>
    /// <returns> the set of variables (string names) representing the variables referenced by the formula. </returns>
    public ISet<string> GetVariables()
    {
        HashSet<string> variables = new HashSet<string>();

        foreach (string token in tokens)
        {
            if (IsVar(token))
            {
                variables.Add(token.ToUpper());
            }
        }
        return variables;
    }

    /// <summary>
    ///   <para>
    ///     Returns a string representation of a canonical form of the formula.
    ///   </para>
    ///   <para>
    ///     The string will contain no spaces.
    ///   </para>
    ///   <para>
    ///     If the string is passed to the Formula constructor, the new Formula f 
    ///     will be such that this.ToString() == f.ToString().
    ///   </para>
    ///   <para>
    ///     All of the variables in the string will be normalized.  This
    ///     means capital letters.
    ///   </para>
    ///   <para>
    ///       For example:
    ///   </para>
    ///   <code>
    ///       new("x1 + y1").ToString() should return "X1+Y1"
    ///       new("X1 + 5.0000").ToString() should return "X1+5".
    ///   </code>
    ///   <para>
    ///     This code should execute in O(1) time.
    ///   <para>
    /// </summary>
    /// <returns>
    ///   A canonical version (string) of the formula. All "equal" formulas
    ///   should have the same value here.
    /// </returns>
    public override string ToString()
    {
        return canonicalForm;
    }

    /// <summary>
    ///   Reports whether "token" is a variable.  It must be one or more letters
    ///   followed by one or more numbers.
    /// </summary>
    /// <param name="token"> A token that may be a variable. </param>
    /// <returns> true if the string matches the requirements, e.g., A1 or a1. </returns>
    private static bool IsVar(string token)
    {
        // notice the use of ^ and $ to denote that the entire string being matched is just the variable
        string standaloneVarPattern = $"^{VariableRegExPattern}$";
        return Regex.IsMatch(token, standaloneVarPattern);
    }

    /// <summary>
    ///   <para>
    ///     Given an expression, enumerates the tokens that compose it.
    ///   </para>
    ///   <para>
    ///     Tokens returned are:
    ///   </para>
    ///   <list type="bullet">
    ///     <item>left paren</item>
    ///     <item>right paren</item>
    ///     <item>one of the four operator symbols</item>
    ///     <item>a string consisting of one or more letters followed by one or more numbers</item>
    ///     <item>a double literal</item>
    ///     <item>and anything that doesn't match one of the above patterns</item>
    ///   </list>
    ///   <para>
    ///     There are no empty tokens; white space is ignored (except to separate other tokens).
    ///   </para>
    /// </summary>
    /// <param name="formula"> A string representing an infix formula such as 1*B1/3.0. </param>
    /// <returns> The ordered list of tokens in the formula. </returns>
    private static List<string> GetTokens(string formula)
    {
        List<string> results = [];

        string lpPattern = @"\(";
        string rpPattern = @"\)";
        string opPattern = @"[\+\-*/]";
        string doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
        string spacePattern = @"\s+";

        // Overall pattern
        string pattern = string.Format(
                                        "({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                        lpPattern,
                                        rpPattern,
                                        opPattern,
                                        VariableRegExPattern,
                                        doublePattern,
                                        spacePattern);

        // Enumerate matching tokens that don't consist solely of white space.
        foreach (string s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
        {
            if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
            {
                results.Add(s);
            }
        }

        return results;
    }
    /// <summary>
    /// Used to determine whether the token being processed is valid for operations. Unrecognized tokens 
    /// cannot be used in the formula, thus they will not be added to the final formula string. 
    /// </summary>
    /// 
    /// <param name="token">The current element from the inputted string that is being processed.</param>
    /// 
    /// <returns>
    /// A boolean. Returns true if the current element being examined is a valid token according 
    /// to assignment specifications; returns false otherwise.
    /// </returns>
    private static bool IsValidToken(string token)
    {
        return (Double.TryParse(token, out double d) || IsVar(token) || token == "+" || token == "-" ||
            token == "*" || token == "/" || token == "(" || token == ")");

    }

    /// <summary>
    /// Used to detemine whether or not the ordering of two valid tokens is correct.
    /// The current element is examined against the previous element, and certain rules 
    /// determine whether the order is correct. I.e - two operators cannot be next to 
    /// eachother.
    /// </summary>
    /// 
    /// <param name="previousToken">The element that precedes the current element being processed.</param>
    /// 
    /// <param name="token">The current element beign processed.</param>
    /// 
    /// <returns>
    /// A boolean value that determines whether the order is correct or not. Returns true if
    /// at least one token following another token is correct; returns false otherwise.
    /// </returns>
    private static bool IsValidFollowingRule(string previousToken, string token)
    {
        if (double.TryParse(previousToken, out _) || IsVar(previousToken) || previousToken == ")")
        {
            return token == "+" || token == "-" || token == "*" || token == "/" || token == ")";
        }
        else //(previousToken == "+" || previousToken == "-" || previousToken == "*" || previousToken == "/" || previousToken == "(")
        {
            return double.TryParse(token, out _) || IsVar(token) || token == "(";
        }
    }

    /// <summary>
    /// Determines whether a specific token is an arithmetic operator, or not. The significance of this method 
    /// is to separate the operators from other valid tokens so we are able to check the first item in a string.
    /// </summary>
    /// 
    /// <param name="token">A string representing a token from the formula.</param>
    /// 
    /// <returns> 
    /// True if the token is one of the four basic arithmetic operators: addition ("+"), 
    /// subtraction ("-"), multiplication ("*"), or division ("/"); otherwise, false.
    /// </returns>
    private bool IsOperator(string token)
    {
        return token == "+" || token == "-" || token == "*" || token == "/";
    }

    /// <summary>
    ///   <para>
    ///     Reports whether f1 == f2, using the notion of equality from the <see cref="Equals"/> method.
    ///   </para>
    /// </summary>
    /// <param name="f1"> The first of two formula objects. </param>
    /// <param name="f2"> The second of two formula objects. </param>
    /// <returns> true if the two formulas are the same.</returns>
    public static bool operator == (Formula f1, Formula f2)
    {
        // Use the Equals method to determine equality
        return f1.Equals(f2);
    }

    /// <summary>
    ///   <para>
    ///     Reports whether f1 != f2, using the notion of equality from the <see cref="Equals"/> method.
    ///   </para>
    /// </summary>
    /// <param name="f1"> The first of two formula objects. </param>
    /// <param name="f2"> The second of two formula objects. </param>
    /// <returns> true if the two formulas are not equal to each other.</returns>
    public static bool operator !=(Formula f1, Formula f2)
    {
        return !(f1 == f2);
    }

    /// <summary>
    ///   <para>
    ///     Determines if two formula objects represent the same formula.
    ///   </para>
    ///   <para>
    ///     By definition, if the parameter is null or does not reference 
    ///     a Formula Object then return false.
    ///   </para>
    ///   <para>
    ///     Two Formulas are considered equal if their canonical string representations
    ///     (as defined by ToString) are equal.  
    ///   </para>
    /// </summary>
    /// <param name="obj"> The other object.</param>
    /// <returns>
    ///   True if the two objects represent the same formula.
    /// </returns>
    public override bool Equals(object? obj)
    {
        if (obj is not Formula otherFormula)
        {
            return false;
        }
        return this.ToString() == otherFormula.ToString();
    }

    public object Evaluate(Lookup lookup)
    {
        // Stacks for storing values and operators
        Stack<double> valStack = new Stack<double>();
        Stack<string> opStack = new Stack<string>();

        foreach (string token in tokens)
        {
            if (Double.TryParse(token, out double d) || IsVar(token))
            {
                double value;

                // Get the value of token if it is a double
                if (Double.TryParse(token, out double parseVal))
                {
                    value = parseVal;
                }
                else
                {
                    // Get the variable's value from the lookup
                    // If the variable is undefined, return a FormulaError
                    try
                    {
                        value = lookup(token.ToUpper());
                    }
                    catch (ArgumentException)
                    {
                        return new FormulaError("Undefined variable");
                    }
                }
                if (opStack.IsOnTop("*") || opStack.IsOnTop("/"))
                {
                    // Evaluate the topmost value and current value and push onto valueStack
                    // Catch divide by zero if it occurs and return FormulaError
                    try
                    {
                        valStack.Push(EvaluateSimple(valStack.Pop(), opStack.Pop(), value));
                    }
                    catch (ArgumentException)
                    {
                        return new FormulaError("Division by 0");
                    }
                }
                else
                {
                    valStack.Push(value);
                }
            }
            else if (token.Equals("+") || token.Equals("-"))
            {
                IfAddOrSubOnStackEvaluate();
                opStack.Push(token);
            }
            else if (token.Equals("*") || token.Equals("/") || token.Equals("("))
            {
                opStack.Push(token);
            }
            else
            {
                IfAddOrSubOnStackEvaluate();

                opStack.Pop();

                if (opStack.IsOnTop("*") || opStack.IsOnTop("/"))
                {
                    double rightVal = valStack.Pop();
                    double leftVal = valStack.Pop();

                    // Evaluate the values and push onto valueStack
                    // Catch divide by zero if it occurs and return FormulaError
                    try
                    {
                        valStack.Push(EvaluateSimple(leftVal, opStack.Pop(), rightVal));
                    }
                    catch (ArgumentException)
                    {
                        return new FormulaError("Division by 0");
                    }
                }
            }
        }

        if (opStack.Count == 0)
        {
            // Return last item in valueStack
            return valStack.Pop();
        }
        else
        {
            double rightVal = valStack.Pop();
            double leftVal = valStack.Pop();

            // Evaluate last two items in valueStack and return
            return EvaluateSimple(leftVal, opStack.Pop(), rightVal);
        }

        /// <summary>
        ///   Local function for checking if + or - operators are on the top of the stack.
        ///   Evaluates the top two values on the value stack if they are and pushes it back on top.
        /// </summary>
        void IfAddOrSubOnStackEvaluate()
        {
            if (opStack.IsOnTop("+") || opStack.IsOnTop("-"))
            {
                double rightVal = valStack.Pop();
                double leftVal = valStack.Pop();
                valStack.Push(EvaluateSimple(leftVal, opStack.Pop(), rightVal));
            }
        }

        /// <summary>
        ///   Local function for making a simple evaluation given two values and an operator.
        ///   Throws an ArgumentException if division by zero occurs.
        /// </summary>
        /// <param name="leftVal"> Value on the left of the operator according to infix. </param>
        /// <param name="op"> Operator to be applied to the values. </param>
        /// <param name="rightVal"> Value on the right of the operator according to infix. </param>
        /// <returns> Returns a double representing the value of the expression given. </returns>
        double EvaluateSimple(double leftVal, string op, double rightVal)
        {
            switch (op)
            {
                default:
                case "+":
                    return leftVal + rightVal;
                case "-":
                    return leftVal - rightVal;
                case "*":
                    return leftVal * rightVal;
                case "/":
                    if (rightVal == 0) throw new ArgumentException("Division by zero");
                    return leftVal / rightVal;

            }
        }
    }

    /// <summary>
    ///   <para>
    ///     Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
    ///     case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two
    ///     randomly-generated unequal Formulas have the same hash code should be extremely small.
    ///   </para>
    /// </summary>
    /// <returns> The hashcode for the object. </returns>
    public override int GetHashCode()
    {
        return canonicalForm.GetHashCode();
    }

    /// <summary>
    /// Performs the specified arithmetic operation on two numbers depending on
    /// the specific operator from the operatorStack in the evaluate method.
    /// </summary>
    /// <param name="firstNum"> The first operand in the operation.</param>
    /// <param name="secondNum">The second operand in the operation </param>
    /// <param name="op">The operator that was popped off of the operatorStack</param>
    /// <returns>The computed value from the operation between two doubles and an operator.</returns>
    private double Operate(double firstNum, double secondNum, string op)
    {
        if (op == "+")
        {
            return firstNum + secondNum;
        }
        else if (op == "-")
        {
            return firstNum - secondNum;
        }
        else if (op == "*")
        {
            return firstNum * secondNum;
        }
            return firstNum / secondNum; //else (op == "/")
    }
}

/// <summary>
///   Used to report syntax errors in the argument to the Formula constructor.
/// </summary>
public class FormulaFormatException : Exception
{
    /// <summary>
    ///   Initializes a new instance of the <see cref="FormulaFormatException"/> class.
    ///   <para>
    ///      Constructs a FormulaFormatException containing the explanatory message.
    ///   </para>
    /// </summary>
    /// <param name="message"> A developer defined message describing why the exception occured.</param>
    public FormulaFormatException(string message)
        : base(message)
    {
       
    }
}

/// <summary>
/// Used as a possible return value of the Formula.Evaluate method.
/// </summary>
public class FormulaError
{
    /// <summary>
    ///   Initializes a new instance of the <see cref="FormulaError"/> class.
    ///   <para>
    ///     Constructs a FormulaError containing the explanatory reason.
    ///   </para>
    /// </summary>
    /// <param name="message"> Contains a message for why the error occurred.</param>
    public FormulaError(string message)
    {
        Reason = message;
    }

    /// <summary>
    ///  Gets the reason why this FormulaError was created.
    /// </summary>
    public string Reason { get; private set; }
}

/// <summary>
///   Any method meeting this type signature can be used for
///   looking up the value of a variable.
/// </summary>
/// <exception cref="ArgumentException">
///   If a variable name is provided that is not recognized by the implementing method,
///   then the method should throw an ArgumentException.
/// </exception>
/// <param name="variableName">
///   The name of the variable (e.g., "A1") to lookup.
/// </param>
/// <returns> The value of the given variable (if one exists). </returns>
public delegate double Lookup(string variableName);

/// <summary>
/// Class for extension to Stack
/// </summary>
public static class StackExtensions
{
    /// <summary>
    /// Extension for Stack to check if a given token is on top.
    /// </summary>
    /// <param name="stack"> Stack to look at. </param>
    /// <param name="token"> The token we want to check the top of the stack for. </param>
    /// <returns> True if the token is on the top of the stack, false otherwise. </returns>
    public static bool IsOnTop(this Stack<string> stack, string token)
    {
        return stack.Count > 0 && stack.Peek() == token;
    }
}
