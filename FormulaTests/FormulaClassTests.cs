// <copyright file="FormulaSyntaxTests.cs" company="UofU-CS3500">
//   Copyright 2024 UofU-CS3500. All rights reserved.
// </copyright>
// <authors> Jared Pratt</authors>
// <date> 09/20/2024 </date>

namespace FormulaTests;

using CS3500.Formula;

/// <summary>
///   <para>
///     The following class shows the basics of how to use the MSTest framework,
///     including:
///   </para>
///   <list type="number">
///     <item> How to catch exceptions. </item>
///     <item> How a test of valid code should look. </item>
///   </list>
/// </summary>
[TestClass]
public class FormulaSyntaxTests
{
    // --- Tests for One Token Rule ---

    /// <summary>
    ///   <para>
    ///     This test makes sure the right kind of exception is thrown
    ///     when trying to create a formula with no tokens.
    ///   </para>
    ///   <remarks>
    ///     <list type="bullet">
    ///       <item>
    ///         We use the _ (discard) notation because the formula object
    ///         is not used after that point in the method.  Note: you can also
    ///         use _ when a method must match an interface but does not use
    ///         some of the required arguments to that method.
    ///       </item>
    ///       <item>
    ///         string.Empty is often considered best practice (rather than using "") because it
    ///         is explicit in intent (e.g., perhaps the coder forgot to but something in "").
    ///       </item>
    ///       <item>
    ///         The name of a test method should follow the MS standard:
    ///         https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices
    ///       </item>
    ///       <item>
    ///         All methods should be documented, but perhaps not to the same extent
    ///         as this one.  The remarks here are for your educational
    ///         purposes (i.e., a developer would assume another developer would know these
    ///         items) and would be superfluous in your code.
    ///       </item>
    ///       <item>
    ///         Notice the use of the attribute tag [ExpectedException] which tells the test
    ///         that the code should throw an exception, and if it doesn't an error has occurred;
    ///         i.e., the correct implementation of the constructor should result
    ///         in this exception being thrown based on the given poorly formed formula.
    ///       </item>
    ///     </list>
    ///   </remarks>
    ///   <example>
    ///     <code>
    ///        // here is how we call the formula constructor with a string representing the formula
    ///        _ = new Formula( "5+5" );
    ///     </code>
    ///   </example>
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestNoTokens_Invalid()
    {
        _ = new Formula(string.Empty);  // note: it is arguable that you should replace "" with string.Empty for readability and clarity of intent (e.g., not a cut and paste error or a "I forgot to put something there" error).
    }

    /// <summary>
    ///   <para>
    ///     This is different than testing an empty String like the previous test. This tests that 'space' is
    ///     not included in the token list. The result should throw an exception as 'space' is not a valid token.
    ///   </para>
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestSpaceAsToken_Invalid()
    {
        _ = new Formula(" ");
    }

    [TestMethod]
    public void FormulaConstructor_TestOneNumberToken_Valid()
    {
        _ = new Formula("1");
    }

    [TestMethod]
    public void FormulaConstructor_TestOneTokenFloatNumber_Valid()
    {
        _ = new Formula("3.14159");
    }

    [TestMethod]
    public void FormulaConstructor_TestOneTokenExponentNumber_Valid()
    {
        _ = new Formula("3e-6");
    }

    [TestMethod]
    public void FormulaConstructor_TestShortVariableToken_Valid()
    {
        _ = new Formula("a1");
    }


    // --- Tests for Valid Token Rule ---

    /// <summary>
    ///   <para>
    ///     Even though this test satisfies the "at least one token" rule, the token is not an operator, variable, or number. 
    ///     The result should have an exception.
    ///   </para>
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestInvalidTokenAtSymbol_Invalid()
    {
        _ = new Formula("@");
    }

    /// <summary>
    ///   <para>
    ///     Even though this test satisfies the "at least one token" rule, the token is not an operator, variable, or number. 
    ///     The result should have an exception.
    ///   </para>
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestInvalidTokenLetter_Invalid()
    {
        _ = new Formula("A");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestInvalidTokenWord_Invalid()
    {
        _ = new Formula("thing");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestInvalidCarrotToken_Invalid()
    {
        _ = new Formula("6^3");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestInvalidEqualsToken_Invalid()
    {
        _ = new Formula("6 = (3 + 3)");
    }

    [TestMethod]
    public void FormulaConstructor_TestValidTokenDivideOperator_Valid()
    {
        _ = new Formula("3 / 2");
    }

    [TestMethod]
    public void FormulaConstructor_TestValidTokenMultiplyOperator_Valid()
    {
        _ = new Formula("4 * 5");
    }

    [TestMethod]
    public void FormulaConstructor_TestValidTokenSubtractOperator_Valid()
    {
        _ = new Formula("1 - 3");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestValidTokenRandomPoundSymbol_Invalid()
    {
        _ = new Formula("1 #");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestOneTokenRandomAndSymbol_Invalid()
    {
        _ = new Formula("1 & 2");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestValidTokenRandomDollarSymbol_Invalid()
    {
        _ = new Formula("$");
    }

    // --- Tests for Closing Parenthesis Rule

    /// <summary>
    ///   <para>
    ///    Tests the closing parenthesis rule, which cannot have more closing parenthesis seen before opening parenthesis.
    ///   </para>
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestClosingParenthesis_Invalid()
    {
        _ = new Formula(")");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestClosingParenthesisAfterOperation_Invalid()
    {
        _ = new Formula("1 + 1 )");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestClosingParenthesisBeforeOperation_Invalid()
    {
        _ = new Formula(" ) 1 + 0.7");
    }

    [TestMethod]
    public void FormulaConstructor_TestVariableAndNumberInParenthesis_Valid()
    {
        _ = new Formula("(x1) / (4)");
    }

    // --- Tests for Balanced Parentheses Rule

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestUnbalancedParenthesisWithNumbers_Invalid()
    {
        _ = new Formula("((1 * 3) + 2");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestBalancedOnlyOpendingParenthesis_Invalid()
    {
        _ = new Formula("(");
    }

    [TestMethod]
    public void FormulaConstructor_TestMultipleBalancedParenthesis_Valid()
    {
        _ = new Formula("(x1 * (( 7 )))");
    }

    /// <summary>
    ///   <para>
    ///    Testing balanced parenthesis, should not through an exception because no rules are broken. Also, satisfies rules 3, 5, and 6.
    ///   </para>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestBalancedParenthesisNoNumbers_Valid()
    {
        _ = new Formula("(a1)");
    }
    // --- Tests for First Token Rule

    /// <summary>
    ///   <para>
    ///     Make sure a simple well formed formula is accepted by the constructor (the constructor
    ///     should not throw an exception).
    ///   </para>
    ///   <remarks>
    ///     This is an example of a test that is not expected to throw an exception, i.e., it succeeds.
    ///     In other words, the formula "1+1" is a valid formula which should not cause any errors.
    ///   </remarks>
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestFirstTokenNumber_Valid()
    {
        _ = new Formula("1+1");
    }

    [TestMethod]
    public void FormulaConstructor_TestFirstTokenVariable_Valid()
    {
        _ = new Formula("a1 - 37");
    }

    [TestMethod]
    public void FormulaConstructor_TestFirstTokenFloat_Valid()
    {
        _ = new Formula("9.99E10 + 3");
    }

    [TestMethod]
    public void FormulaConstructor_TestFirstTokenOpeningParenthesis_Valid()
    {
        _ = new Formula("(69)");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestFirstTokenLetter_Invalid()
    {
        _ = new Formula("a + 13");
    }

    // --- Tests for  Last Token Rule ---
    [TestMethod]
    public void FormulaConstructor_TestLastTokenNumber_Valid()
    {
        _ = new Formula("u123 + 3.8");
    }

    [TestMethod]
    public void FormulaConstructor_TestLastTokenVariable_Valid()
    {
        _ = new Formula("21 + c69");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestLastTokenRandomSymbol_Invalid()
    {
        _ = new Formula("8 + %");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestLastTokenLetter_Invalid()
    {
        _ = new Formula("8 + B");
    }

    // --- Tests for Parentheses/Operator Following Rule ---
    [TestMethod]
    public void FormulaConstructor_TestFloatFollowingOperator_Valid()
    {
        _ = new Formula("4 * 3.7");
    }

    [TestMethod]
    public void FormulaConstructor_TestVariableFollowingOperator_Valid()
    {
        _ = new Formula("4.2 * p73");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestNumberFollowingParenthesis_InValid()
    {
        _ = new Formula("(4 + 1) 5");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestOperatorFollowOperator_Invalid()
    {
        _ = new Formula("1 + + 2");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestOperatorFollowParenthesis_Invalid()
    {
        _ = new Formula("1 ( + 2)");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestOpenFollowClosedParenthesis_Invalid()
    {
        _ = new Formula("(4 * 5) (2)");
    }

    // --- Tests for Extra Following Rule ---

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestNumberFollowsNumber_Invalid()
    {
        _ = new Formula("4 5 6 7");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestNumberFollowsVariable_Invalid()
    {
        _ = new Formula("a40 6");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestVariableFollowsVariable_Invalid()
    {
        _ = new Formula("b23 a79");
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestNumberFollowsCLosingParenthesis_Invalid()
    {
        _ = new Formula("(6) 3");
    }

    [TestMethod]
    public void FormulaConstructor_TestOperatorFollowsClosingParenthesis_Valid()
    {
        _ = new Formula("(99) + 2");
    }

    // ----------------------Additional tests for the formula constructor--------------------------------- 

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestOnlyOperators_Invalid()
    {
        _ = new Formula("+ / *");
    }

    // --- Multiple Operators Between Numbers ---

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestMultipleOperatorsBetweenNumbers_Invalid()
    {
        _ = new Formula("1 + + 2");
    }


    // --- Handling Special Characters in Variables ---

    /// <summary>
    ///   Test to ensure that special characters in variable names are handled correctly.
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestVariableWithSpecialCharacters_Invalid()
    {
        _ = new Formula("a1$ + 3");
    }

    [TestMethod]
    public void FormulaConstructor_ComplexEquationTest_Valid()
    {
        Formula f = new("ga22*a1 + 4E3- 22.0 / Ga22");
        Assert.AreEqual("GA22*A1+4000-22/GA22", f.ToString());
    }
    /// <summary>
    ///   Test to ensure that numbers with leading zeros are valid and parsed correctly.
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestNumberWithLeadingZeros_Valid()
    {
        Formula f = new Formula("0001 + 0023");
        Assert.AreEqual("1+23", f.ToString());
    }

    // --- Edge Case: Unconventional Scientific Notation ---

    /// <summary>
    ///   Test to validate unconventional scientific notation (e.g., missing leading digits).
    /// </summary>
    [TestMethod]
    public void FormulaConstructor_TestUnconventionalScientificNotation_Valid()
    {
        Formula f = new Formula("0.1E1 + 2");
        Assert.AreEqual("1+2", f.ToString());
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestInvalidOperatorAfterParentheses_Invalid()
    {
        _ = new Formula("((1+1)) + * 2");
    }

    // --- Test for Invalid Start with Operator ---

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestStartsWithOperator_Invalid()
    {
        _ = new Formula("+3 * 5");
    }

    //-------------------------------------Testing ToString()-----------------------------------------
    // --- Complex Variable Names ---

    [TestMethod]
    public void FormulaConstructor_TestComplexVariableNames_Valid()
    {
        Formula f = new Formula("longVariableName1 + 3");
        Assert.AreEqual("LONGVARIABLENAME1+3", f.ToString());
    }

    // --- Decimal Precision Handling ---

    [TestMethod]
    public void FormulaConstructor_TestHighPrecisionFloat_Valid()
    {
        Formula f = new Formula("3.14159265358979 + 1");
        Assert.AreEqual("3.14159265358979+1", f.ToString());
    }

    // --- Edge Case: Leading/Trailing Whitespace ---

    [TestMethod]
    public void FormulaConstructor_TestFormulaWithLeadingAndTrailingWhitespace_Valid()
    {
        Formula f = new Formula("  3 + 2  ");
        Assert.AreEqual("3+2", f.ToString());
    }

    // --- Handling Mixed Case Variables ---

    [TestMethod]
    public void FormulaConstructor_TestMixedCaseVariableNames_Valid()
    {
        Formula f = new Formula("aA1 + Bb2");
        Assert.AreEqual("AA1+BB2", f.ToString());
    }



    // --- Test for Valid Operator Between Parentheses and Numbers ---

    [TestMethod]
    public void FormulaConstructor_TestValidOperatorBetweenParenthesesAndNumber_Valid()
    {
        Formula f = new Formula("(1) + 2");
        Assert.AreEqual("(1)+2", f.ToString());
    }

    // --- Mixed Variables and Numbers Edge Case ---

    [TestMethod]
    public void FormulaConstructor_TestMixedVariablesAndNumbers_Valid()
    {
        Formula f = new Formula("var1 * 2 + 3 / var2");
        Assert.AreEqual("VAR1*2+3/VAR2", f.ToString());
    }

    // --- Formula Containing Only Operators ---



    // ------------------------------Testing GetVariables()-------------------------------------
    [TestMethod]
    public void GetVariables_SingleVariable_ReturnsOneVariable()
    {
        Formula formula = new Formula("x1");
        var variables = formula.GetVariables();
        HashSet<string> expected = ["X1"];
        Assert.IsTrue(expected.SetEquals(variables));
    }

    [TestMethod]
    public void GetVariables_NoVariables_ReturnsEmptyList()
    {
        Formula formula = new Formula("5 + 4 - 3");
        var variables = formula.GetVariables();

        Assert.AreEqual(0, variables.Count());
    }

    [TestMethod]
    public void GetVariables_MultipleVariables_ReturnsAllVariables()
    {
        Formula formula = new Formula("x1 + y2 - z3");
        var variables = formula.GetVariables();
        HashSet<string> expected = ["X1", "Y2", "Z3"];
        Assert.IsTrue(expected.SetEquals(variables));
    }

    [TestMethod]
    public void GetVariables_RepeatedVariables_ReturnsUniqueVariables()
    {
        Formula formula = new Formula("x1 + x1 + y2");
        var variables = formula.GetVariables();
        HashSet<string> expected = ["X1", "Y2"];
        Assert.IsTrue(expected.SetEquals(variables));
    }

    [TestMethod]
    public void GetVariables_VariablesInParentheses_ReturnsCorrectVariables()
    {
        Formula formula = new Formula("(x1 + y2) * z3");
        var variables = formula.GetVariables();
        HashSet<string> expected = ["X1", "Y2", "Z3"];
        Assert.IsTrue(expected.SetEquals(variables));
    }

    [TestMethod]
    public void GetVariables_VariablesAndNumbers_ReturnsVariablesOnly()
    {
        Formula formula = new Formula("x1 + 5 - y2 * 3.14");
        var variables = formula.GetVariables();
        HashSet<string> expected = ["X1", "Y2"];
        Assert.IsTrue(expected.SetEquals(variables));
    }

    [TestMethod]
    public void GetVariables_ComplexVariables_ReturnsCorrectVariables()
    {
        Formula formula = new Formula("foo1 + bar2 * baz3");
        var variables = formula.GetVariables();
        HashSet<string> expected = ["FOO1", "BAR2", "BAZ3"];
        Assert.IsTrue(expected.SetEquals(variables));
    }

    [TestMethod]
    public void GetVariables_VariablesAndExponents_ReturnsVariablesOnly()
    {
        Formula formula = new Formula("x1 * 2e3 - y2");
        var variables = formula.GetVariables();
        HashSet<string> expected = ["X1", "Y2"];
        Assert.IsTrue(expected.SetEquals(variables));
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void GetVariables_EmptyFormula_ReturnsEmptyList()
    {
        Formula formula = new Formula("");
        var variables = formula.GetVariables();
        Assert.AreEqual(0, variables.Count());
    }

    [TestMethod]
    public void GetVariables_NestedParentheses_ReturnsAllVariables()
    {
        Formula formula = new Formula("(x1 * (y2 + z3))");
        var variables = formula.GetVariables();
        HashSet<string> expected = ["X1", "Y2", "Z3"];
        Assert.IsTrue(expected.SetEquals(variables));
    }

    [TestMethod]
    public void FormulaConstructor_ComplexEquationCountTest_Valid()
    {
        Formula f = new("ga22*a1 + 4E3- 22.0 / Ga22");
        Assert.AreEqual(2, f.GetVariables().Count);
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void GetVariables_SingleLetterVariables_ReturnsAllVariables()
    {
        Formula formula = new Formula("a + b - c");
        var variables = formula.GetVariables();
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void GetVariables_OnlyOperators_ReturnsEmptyList()
    {
        Formula formula = new Formula("+ - * /");
        var variables = formula.GetVariables();
    }

    [TestMethod]
    [ExpectedException(typeof(FormulaFormatException))]
    public void GetVariables_VariableFollowedByNumber_ReturnsCorrectVariable()
    {
        Formula formula = new Formula("x1 123 + y2");
        var variables = formula.GetVariables();
        HashSet<string> expected = ["X1", "Y2"];
        Assert.IsTrue(expected.SetEquals(variables));
    }

    [TestMethod]
    public void GetVariables_ComplexExpression_ReturnsCorrectVariables()
    {
        Formula formula = new Formula("(a1 + b2) * (c3 - d4) / e5 + f6");
        var variables = formula.GetVariables();
        HashSet<string> expected = ["A1", "B2", "C3", "D4", "E5", "F6"];
        Assert.IsTrue(expected.SetEquals(variables));
    }
    [TestMethod]
    public void TestEquality_NullComparisons_ReturnFalse()
    {
        Formula f1 = new Formula("2 + 2");
        Formula f2 = null;
        Assert.IsFalse(f1 == f2);
    }

    [TestMethod]
    public void TestEquality_IdenticalFormulas_ReturnTrue()
    {
        Formula f1 = new Formula("x1 + 2");
        Formula f2 = new Formula("x1 + 2");
        Assert.IsTrue(f1 == f2);
    }

    [TestMethod]
    public void TestInequality_DifferentFormulas_ReturnTrue()
    {
        Formula f1 = new Formula("x1 + 2");
        Formula f2 = new Formula("x2 + 2");
        Assert.IsTrue(f1 != f2);
    }

    [TestMethod]
    public void TestEquality_SameReference_ReturnTrue()
    {
        Formula f1 = new Formula("2 + 3 * x1");
        Formula f2 = f1;
        Assert.IsTrue(f1 == f2);
    }

    // Testing Equals method
    [TestMethod]
    public void Equals_SameFormulas_ReturnTrue()
    {
        Formula f1 = new Formula("1 + 2");
        Formula f2 = new Formula("1 + 2");
        Assert.IsTrue(f1.Equals(f2));
    }

    [TestMethod]
    public void Equals_DifferentFormulas_ReturnFalse()
    {
        Formula f1 = new Formula("1 + 2");
        Formula f2 = new Formula("2 + 1");
        Assert.IsFalse(f1.Equals(f2));
    }

    [TestMethod]
    public void Equals_NullParameter_ReturnFalse()
    {
        Formula f1 = new Formula("3 + 4");
        Assert.IsFalse(f1.Equals(null));
    }

    // Testing Evaluate method
    [TestMethod]
    public void Evaluate_SimpleAddition_ReturnsCorrectValue()
    {
        Formula f = new Formula("2 + 3");
        Assert.AreEqual(5.0, f.Evaluate(s => 0));
    }

    [TestMethod]
    public void Evaluate_WithUndefinedVariable_ReturnsFormulaError()
    {
        Formula f = new Formula("x1 + 3");
        var result = f.Evaluate(s => throw new ArgumentException("Variable x1 is not defined"));
        Assert.IsTrue(result is FormulaError);
    }

    [TestMethod]
    public void Evaluate_WithDivisionByZero_ReturnsFormulaError()
    {
        Formula f = new Formula("10 / (5 - 5)");
        var result = f.Evaluate(s => 0);
        Assert.IsTrue(result is FormulaError);
    }

    // Testing GetHashCode method
    [TestMethod]
    public void GetHashCode_EqualFormulas_SameHashCode()
    {
        Formula f1 = new Formula("x1 + y1");
        Formula f2 = new Formula("x1 + y1");
        Assert.AreEqual(f1.GetHashCode(), f2.GetHashCode());
    }

    [TestMethod]
    public void GetHashCode_DifferentFormulas_DifferentHashCode()
    {
        Formula f1 = new Formula("x1 + y1");
        Formula f2 = new Formula("y1 + x1");
        Assert.AreNotEqual(f1.GetHashCode(), f2.GetHashCode());
    }

    // --- Test Last Token Rule ---
    /// <summary>
    /// Make sure the last token is valid, in this case, not an operator (plus).
    /// </summary>
    [TestMethod]
    [TestCategory("18")]
    [ExpectedException(typeof(FormulaFormatException))]
    public void FormulaConstructor_TestInvalidLastTokenPlus_Fails()
    {
        _ = new Formula("5 +");
    }
    // Some more general syntax errors detected by the constructor
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("34")]
    [ExpectedException(typeof(FormulaFormatException))]
    public void TestSingleOperator()
    {
        new Formula("+");
    }
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("35")]
    [ExpectedException(typeof(FormulaFormatException))]
    public void TestExtraOperator()
    {
        new Formula("2+5+");
    }
    // Test some longish valid formulas.
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("55")]
    public void FormulaConstructor_LongComplexFormula_IsAValidFormula()
    {
        _ = new Formula("(((((2+3*X1)/(7e-5+X2-X4))*X5+.0005e+92)-8.2)*3.14159) * ((x2 + 3.1) - .00000000008)" );
    }
    [TestMethod]
    [Timeout(2000)]
    [TestCategory("56")]
    public void FormulaConstructor_LongComplexFormula2_IsAValidFormula()
    {
        _ = new Formula("5 + (1-2) * 3.14 / 1e6 + 0.2E-9 - A1 + bb22");
    }

    // Test Equality and Inequality Operators
    [TestMethod]
    public void TestEquality_Operators()
    {
        var f1 = new Formula("x1 + y1");
        var f2 = new Formula("x1 + y1");
        var f3 = new Formula("x1 + y2");

        Assert.IsTrue(f1 == f2, "Identical formulas should be equal.");
        Assert.IsFalse(f1 == f3, "Different formulas should not be equal.");
        Assert.IsTrue(f1 != f3, "Different formulas should not be equal.");
        Assert.IsFalse(f1 != f2, "Identical formulas should be equal.");
    }

    // Test Equals Method
    [TestMethod]
    public void TestEquals()
    {
        var f1 = new Formula("x1 + y1");
        var f2 = new Formula("X1 + Y1");
        var f3 = new Formula("x1 + y2");
        var notFormula = "x1 + y1";

        Assert.IsTrue(f1.Equals(f2), "Formulas that normalize to the same value should be equal.");
        Assert.IsFalse(f1.Equals(notFormula), "A formula and a non-formula object should not be equal.");
        Assert.IsFalse(f1.Equals(f3), "Different formulas should not be considered equal.");
    }

    // Test Evaluate Method with simple expressions and variable lookup
    [TestMethod]
    public void TestEvaluate_SimpleExpressions()
    {
        var f1 = new Formula("2 + 2");
        var f2 = new Formula("2 + a1");
        var f3 = new Formula("10 / (5 - 5)");
        var f4 = new Formula("(2+3)*5");

        var result1 = f1.Evaluate(s => throw new ArgumentException("No variables expected."));
        var result2 = f2.Evaluate(s => 3.0);
        var result3 = f3.Evaluate(s => 0);
        var result4 = f4.Evaluate(s => throw new ArgumentException("No variables expected."));

        Assert.AreEqual(4.0, result1, "Simple addition should evaluate correctly.");
        Assert.AreEqual(5.0, result2, "Expression using lookup should evaluate correctly.");
        Assert.AreEqual(25.0, result4, "Parentheses and multiplication should evaluate correctly.");
    }

    // Test GetVariables Method
    [TestMethod]
    public void TestGetVariables()
    {
        var f1 = new Formula("x1 + y1 + x1 + z1");

        var expectedVariables = new HashSet<string> { "X1", "Y1", "Z1" };
        var variables = f1.GetVariables();

        Assert.IsTrue(expectedVariables.SetEquals(variables), "Should return unique set of variables in canonical form.");
    }

    // Test GetHashCode Method
    [TestMethod]
    public void TestGetHashCode()
    {
        var f1 = new Formula("x1 + y1");
        var f2 = new Formula("x1 + y1");
        var f3 = new Formula("x1 + y2");

        Assert.AreEqual(f1.GetHashCode(), f2.GetHashCode(), "Identical formulas should have the same hash code.");
        Assert.AreNotEqual(f1.GetHashCode(), f3.GetHashCode(), "Different formulas should have different hash codes.");
    }

    // Test ToString Method
    [TestMethod]
    public void TestToString()
    {
        var f1 = new Formula("x1 + 2");
        var expectedString = "X1+2";

        Assert.AreEqual(expectedString, f1.ToString(), "ToString should return the formula in canonical form without spaces.");
    }

    // Additional tests to cover operator precedence and complex expressions
    [TestMethod]
    public void TestComplexExpressions()
    {
        var f1 = new Formula("2 + 3 * 4 - 6 / 2");
        var f2 = new Formula("2 * (3 + 4) - 6");

        var result1 = f1.Evaluate(s => throw new ArgumentException("No variables expected."));
        var result2 = f2.Evaluate(s => throw new ArgumentException("No variables expected."));

        Assert.AreEqual(11.0, result1, "Expression should respect operator precedence.");
        Assert.AreEqual(8.0, result2, "Expression with parentheses should evaluate correctly.");
    }

    [TestMethod]
    public void Evaluate_Number_NoOperatorOnStack()
    {
        Formula f = new Formula("3");
        object result = f.Evaluate(s => 0);
        Assert.AreEqual(3.0, result);
    }

    [TestMethod]
    public void Evaluate_Number_WithMultiplicationOperator()
    {
        Formula f = new Formula("2 * 3");
        object result = f.Evaluate(s => 0);
        Assert.AreEqual(6.0, result);
    }

    [TestMethod]
    public void Evaluate_Number_WithDivisionOperator()
    {
        Formula f = new Formula("10 / 2");
        object result = f.Evaluate(s => 0);
        Assert.AreEqual(5.0, result);
    }

    [TestMethod]
    public void Evaluate_Number_AfterAddition()
    {
        Formula f = new Formula("2 + 3 * 4");
        object result = f.Evaluate(s => 0);
        Assert.AreEqual(14.0, result); // 2 + (3*4)
    }

    // Tests for Variable Tokens with Multiplication/Division

    [TestMethod]
    public void Evaluate_Variable_NoOperatorOnStack()
    {
        Formula f = new Formula("x1");
        object result = f.Evaluate(s => s == "X1" ? 5.0 : 0);
        Assert.AreEqual(5.0, result);
    }

    [TestMethod]
    public void Evaluate_Variable_WithMultiplicationOperator()
    {
        Formula f = new Formula("2 * x1");
        object result = f.Evaluate(s => s == "X1" ? 4.0 : 0);
        Assert.AreEqual(8.0, result);
    }

    [TestMethod]
    public void Evaluate_Variable_WithDivisionOperator()
    {
        Formula f = new Formula("20 / x1");
        object result = f.Evaluate(s => s == "X1" ? 4.0 : 0);
        Assert.AreEqual(5.0, result);
    }

    [TestMethod]
    public void Evaluate_Variable_AfterAddition()
    {
        Formula f = new Formula("2 + x1 * 4");
        object result = f.Evaluate(s => s == "X1" ? 3.0 : 0);
        Assert.AreEqual(14.0, result); // 2 + (3*4)
    }

    // Tests for Addition/Subtraction Operators

    [TestMethod]
    public void Evaluate_Addition_PendingAddition()
    {
        Formula f = new Formula("2 + 3 + 4");
        object result = f.Evaluate(s => 0);
        Assert.AreEqual(9.0, result); // (2+3)+4
    }

    [TestMethod]
    public void Evaluate_Subtraction_PendingSubtraction()
    {
        Formula f = new Formula("10 - 2 - 3");
        object result = f.Evaluate(s => 0);
        Assert.AreEqual(5.0, result); // (10-2)-3
    }

    [TestMethod]
    public void Evaluate_AdditionAndSubtraction()
    {
        Formula f = new Formula("10 - 2 + 3");
        object result = f.Evaluate(s => 0);
        Assert.AreEqual(11.0, result); // (10-2)+3
    }

    [TestMethod]
    public void Evaluate_Addition_WithPendingMultiplication()
    {
        Formula f = new Formula("2 * 3 + 4");
        object result = f.Evaluate(s => 0);
        Assert.AreEqual(10.0, result); // (2*3)+4
    }

    [TestMethod]
    public void Evaluate_Subtraction_WithPendingDivision()
    {
        Formula f = new Formula("8 / 4 - 2");
        object result = f.Evaluate(s => 0);
        Assert.AreEqual(0.0, result); // (8/4)-2
    }

    // Tests for Handling Parentheses

    [TestMethod]
    public void Evaluate_SimpleParentheses()
    {
        Formula f = new Formula("(5)");
        object result = f.Evaluate(s => 0);
        Assert.AreEqual(5.0, result);
    }

    [TestMethod]
    public void Evaluate_Parentheses_WithAddition()
    {
        Formula f = new Formula("(2 + 3)");
        object result = f.Evaluate(s => 0);
        Assert.AreEqual(5.0, result);
    }

    [TestMethod]
    public void Evaluate_Parentheses_WithMultiplication()
    {
        Formula f = new Formula("(2 + 3) * 4");
        object result = f.Evaluate(s => 0);
        Assert.AreEqual(20.0, result); // (2+3)*4
    }

    [TestMethod]
    public void Evaluate_NestedParentheses()
    {
        Formula f = new Formula("((2 + 3) * (4 - 2))");
        object result = f.Evaluate(s => 0);
        Assert.AreEqual(10.0, result); // (2+3)*(4-2)
    }

    // Tests for Edge Cases and Additional Conditions

    [TestMethod]
    public void Evaluate_MultipleAdditions()
    {
        Formula f = new Formula("2 + 3 + 4 + 5");
        object result = f.Evaluate(s => 0);
        Assert.AreEqual(14.0, result);
    }

    [TestMethod]
    public void Evaluate_MixedOperators()
    {
        Formula f = new Formula("8 * 2 / 4");
        object result = f.Evaluate(s => 0);
        Assert.AreEqual(4.0, result); // (8*2)/4
    }

    [TestMethod]
    public void Evaluate_OperatorPrecedence()
    {
        Formula f = new Formula("2 + 3 * 4");
        object result = f.Evaluate(s => 0);
        Assert.AreEqual(14.0, result); // 2 + (3*4)
    }

    [TestMethod]
    public void Evaluate_Variable_CaseInsensitive()
    {
        Formula f = new Formula("X1 + 2");
        object result = f.Evaluate(s => s == "X1" ? 3.0 : throw new ArgumentException());
        Assert.AreEqual(5.0, result); // 3 + 2
    }

    // Additional Tests to Cover All Code Paths

    [TestMethod]
    public void Evaluate_Multiplication_AfterParentheses()
    {
        Formula f = new Formula("(2 + 3) * 4");
        object result = f.Evaluate(s => 0);
        Assert.AreEqual(20.0, result);
    }

    [TestMethod]
    public void Evaluate_Addition_AfterParentheses()
    {
        Formula f = new Formula("(2 + 3) + (4 - 1)");
        object result = f.Evaluate(s => 0);
        Assert.AreEqual(8.0, result);
    }

    [TestMethod]
    public void Evaluate_MultipleParentheses()
    {
        Formula f = new Formula("(2 + (3 * 4)) - 5");
        object result = f.Evaluate(s => 0);
        Assert.AreEqual(9.0, result); // (2+(3*4))-5
    }

    [TestMethod]
    public void Evaluate_Multiplication_WithPendingAddition()
    {
        Formula f = new Formula("2 + 3 * 4 + 5");
        object result = f.Evaluate(s => 0);
        Assert.AreEqual(19.0, result); // 2 + (3*4) + 5
    }

    [TestMethod]
    public void Evaluate_Division_WithPendingSubtraction()
    {
        Formula f = new Formula("20 - 4 / 2");
        object result = f.Evaluate(s => 0);
        Assert.AreEqual(18.0, result); // 20 - (4/2)
    }

    [TestMethod]
    public void Evaluate_ComplexExpression_WithVariables()
    {
        Formula f = new Formula("(x1 + x2) * x3 - x4 / x5");
        object result = f.Evaluate(s =>
        {
            switch (s)
            {
                case "X1": return 2.0;
                case "X2": return 3.0;
                case "X3": return 4.0;
                case "X4": return 10.0;
                case "X5": return 2.0;
                default: throw new ArgumentException();
            }
        });
        Assert.AreEqual(15.0, result); // ((2+3)*4) - (10/2)
    }

    [TestMethod]
    public void Evaluate_Variables_WithParentheses()
    {
        Formula f = new Formula("(x1 + x2) * (x3 - x4)");
        object result = f.Evaluate(s => 2.0);
        Assert.AreEqual(0.0, result); // (2+2)*(2-2)
    }

    [TestMethod]
    public void Evaluate_OperatorStack_EmptyAtEnd()
    {
        Formula f = new Formula("2 + 3 * 4 - 5");
        object result = f.Evaluate(s => 0);
        Assert.AreEqual(9.0, result);
    }

    // Tests for Edge Cases with Operators and Parentheses

    [TestMethod]
    public void Evaluate_MultipleOperators_NoPendingOperators()
    {
        Formula f = new Formula("2 + 3 + 4");
        object result = f.Evaluate(s => 0);
        Assert.AreEqual(9.0, result);
    }

    [TestMethod]
    public void Evaluate_OperatorStack_EmptyAfterComplexEvaluation()
    {
        Formula f = new Formula("(2 + 3) * (4 + 5)");
        object result = f.Evaluate(s => 0);
        Assert.AreEqual(45.0, result); // (2+3)*(4+5)
    }

    /// <summary>
    /// Test when the operator stack contains a '+' operator before a '+' token is processed.
    /// This should cause the pending addition to be resolved.
    /// </summary>
    [TestMethod]
    public void Evaluate_PendingAddition_WithNextAdditionOperator()
    {
        Formula f = new Formula("2 + 3 + 4");
        object result = f.Evaluate(s => 0);
        Assert.AreEqual(9.0, result); // (2+3)+4
    }

    /// <summary>
    /// Test when the operator stack contains a '-' operator before a '+' token is processed.
    /// This should cause the pending subtraction to be resolved.
    /// </summary>
    [TestMethod]
    public void Evaluate_PendingSubtraction_WithNextAdditionOperator()
    {
        Formula f = new Formula("10 - 2 + 3");
        object result = f.Evaluate(s => 0);
        Assert.AreEqual(11.0, result); // (10-2)+3
    }

    /// <summary>
    /// Test when the operator stack contains a '+' operator before a '-' token is processed.
    /// This should cause the pending addition to be resolved.
    /// </summary>
    [TestMethod]
    public void Evaluate_PendingAddition_WithNextSubtractionOperator()
    {
        Formula f = new Formula("5 + 3 - 2");
        object result = f.Evaluate(s => 0);
        Assert.AreEqual(6.0, result); // (5+3)-2
    }

    /// <summary>
    /// Test when the operator stack does not contain a '+' or '-' operator before a '+' token is processed.
    /// The condition should be false, and no pending operation should be resolved.
    /// </summary>
    [TestMethod]
    public void Evaluate_NoPendingAdditionOrSubtraction_WithNextAdditionOperator()
    {
        Formula f = new Formula("2 * 3 + 4");
        object result = f.Evaluate(s => 0);
        Assert.AreEqual(10.0, result); // (2*3)+4
    }

    /// <summary>
    /// Test when processing a closing parenthesis and there is a pending '+' operator on the stack.
    /// This should cause the pending addition to be resolved before the '(' is popped.
    /// </summary>
    [TestMethod]
    public void Evaluate_PendingAddition_BeforeClosingParenthesis()
    {
        Formula f = new Formula("(2 + 3 + 4)");
        object result = f.Evaluate(s => 0);
        Assert.AreEqual(9.0, result); // (2+3+4)
    }

    /// <summary>
    /// Test when processing a closing parenthesis and there is no pending '+' or '-' operator.
    /// The condition should be false, and no pending operation should be resolved.
    /// </summary>
    [TestMethod]
    public void Evaluate_NoPendingAdditionOrSubtraction_BeforeClosingParenthesis()
    {
        Formula f = new Formula("(2 * 3)");
        object result = f.Evaluate(s => 0);
        Assert.AreEqual(6.0, result); // (2*3)
    }

    /// <summary>
    /// Test when multiple '+' and '-' operators are pending in the operator stack.
    /// Ensures that all pending additions and subtractions are resolved correctly.
    /// </summary>
    [TestMethod]
    public void Evaluate_MultiplePendingAdditionsAndSubtractions()
    {
        Formula f = new Formula("10 - 2 + 3 - 4 + 5");
        object result = f.Evaluate(s => 0);
        Assert.AreEqual(12.0, result); // (((10-2)+3)-4)+5
    }

    /// <summary>
    /// Test when the operator stack contains a '*' operator before a '+' token is processed.
    /// The condition should be false, and no pending operation should be resolved.
    /// </summary>
    [TestMethod]
    public void Evaluate_PendingMultiplication_WithNextAdditionOperator()
    {
        Formula f = new Formula("2 * 3 + 4");
        object result = f.Evaluate(s => 0);
        Assert.AreEqual(10.0, result); // (2*3)+4
    }

    /// <summary>
    /// Test when the operator stack contains both '*' and '+' operators.
    /// Ensures that only the '+' or '-' operator is considered in the condition.
    /// </summary>
    [TestMethod]
    public void Evaluate_PendingAdditionAndMultiplication_WithNextAdditionOperator()
    {
        Formula f = new Formula("2 + 3 * 4 + 5");
        object result = f.Evaluate(s => 0);
        Assert.AreEqual(19.0, result); // 2 + (3*4) + 5
    }

    /// <summary>
    /// Test when processing a closing parenthesis and multiple pending '+' and '-' operators exist.
    /// Ensures that the correct pending operation is resolved.
    /// </summary>
    [TestMethod]
    public void Evaluate_MultiplePendingOperators_BeforeClosingParenthesis()
    {
        Formula f = new Formula("(2 + 3 - 1)");
        object result = f.Evaluate(s => 0);
        Assert.AreEqual(4.0, result); // (2+3-1)
    }


    [TestMethod]
    public void TestEquality_SameReference_ReturnTrue2()
    {
        Formula f1 = new Formula("2 + 3 * x1");
        Formula f2 = f1;
        Assert.IsTrue(f1 == f2, "Same reference formulas should be equal.");
    }

    //[TestMethod]
    //public void TestEquality_BothNull_ReturnTrue()
    //{
    //    Formula f1 = null;
    //    Formula f2 = null;
    //    Assert.IsTrue(f1 == f2, "Both null formulas should be equal.");
    //}

    //[TestMethod]
    //public void TestEquality_NullFirstOperand_ReturnFalse()
    //{
    //    Formula f1 = null;
    //    Formula f2 = new Formula("2 + 2");
    //    Assert.IsFalse(f1 == f2, "Null formula should not be equal to a non-null formula.");
    //}

    [TestMethod]
    public void TestEquality_NullSecondOperand_ReturnFalse()
    {
        Formula f1 = new Formula("2 + 2");
        Formula f2 = null;
        Assert.IsFalse(f1 == f2, "Non-null formula should not be equal to a null formula.");
    }

    [TestMethod]
    public void TestEquality_DifferentInstancesButEqual_ReturnTrue()
    {
        Formula f1 = new Formula("x1 + y1");
        Formula f2 = new Formula("x1 + y1");
        Assert.IsTrue(f1 == f2, "Formulas with the same content should be equal.");
    }

    [TestMethod]
    public void TestEquality_DifferentInstancesNotEqual_ReturnFalse()
    {
        Formula f1 = new Formula("x1 + y1");
        Formula f2 = new Formula("x1 + y2");
        Assert.IsFalse(f1 == f2, "Formulas with different content should not be equal.");
    }
}

