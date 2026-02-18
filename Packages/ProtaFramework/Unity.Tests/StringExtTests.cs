using NUnit.Framework;
using Prota;
using System.Collections.Generic;

public class StringExtTests
{
    #region TryParseFunctionCallLike Tests
    
    [Test]
    public void TryParseFunctionCallLike_NullString_ReturnsFalse()
    {
        // Arrange
        string input = null;
        
        // Act
        bool result = input.TryParseFunctionCallLike(out string functionName, out string[] args);
        
        // Assert
        Assert.IsFalse(result);
        Assert.IsNull(functionName);
        Assert.IsNull(args);
    }
    
    [Test]
    public void TryParseFunctionCallLike_EmptyString_ReturnsFalse()
    {
        // Arrange
        string input = "";
        
        // Act
        bool result = input.TryParseFunctionCallLike(out string functionName, out string[] args);
        
        // Assert
        Assert.IsFalse(result);
        Assert.IsNull(functionName);
        Assert.IsNull(args);
    }
    
    [Test]
    public void TryParseFunctionCallLike_NoParentheses_ReturnsFalse()
    {
        // Arrange
        string input = "FunctionName";
        
        // Act
        bool result = input.TryParseFunctionCallLike(out string functionName, out string[] args);
        
        // Assert
        Assert.IsFalse(result);
    }
    
    [Test]
    public void TryParseFunctionCallLike_NoOpeningParenthesis_ReturnsFalse()
    {
        // Arrange
        string input = "FunctionName)";
        
        // Act
        bool result = input.TryParseFunctionCallLike(out string functionName, out string[] args);
        
        // Assert
        Assert.IsFalse(result);
    }
    
    [Test]
    public void TryParseFunctionCallLike_NoClosingParenthesis_ReturnsFalse()
    {
        // Arrange
        string input = "FunctionName(";
        
        // Act
        bool result = input.TryParseFunctionCallLike(out string functionName, out string[] args);
        
        // Assert
        Assert.IsFalse(result);
    }
    
    [Test]
    public void TryParseFunctionCallLike_EmptyParentheses_ReturnsTrue()
    {
        // Arrange
        string input = "FunctionName()";
        
        // Act
        bool result = input.TryParseFunctionCallLike(out string functionName, out string[] args);
        
        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual("FunctionName", functionName);
        Assert.IsNotNull(args);
        Assert.AreEqual(0, args.Length);
    }
    
    [Test]
    public void TryParseFunctionCallLike_SingleArgument_ReturnsTrue()
    {
        // Arrange
        string input = "FunctionName(arg1)";
        
        // Act
        bool result = input.TryParseFunctionCallLike(out string functionName, out string[] args);
        
        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual("FunctionName", functionName);
        Assert.IsNotNull(args);
        Assert.AreEqual(1, args.Length);
        Assert.AreEqual("arg1", args[0]);
    }
    
    [Test]
    public void TryParseFunctionCallLike_MultipleArguments_ReturnsTrue()
    {
        // Arrange
        string input = "FunctionName(arg1, arg2, arg3)";
        
        // Act
        bool result = input.TryParseFunctionCallLike(out string functionName, out string[] args);
        
        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual("FunctionName", functionName);
        Assert.IsNotNull(args);
        Assert.AreEqual(3, args.Length);
        Assert.AreEqual("arg1", args[0]);
        Assert.AreEqual("arg2", args[1]);
        Assert.AreEqual("arg3", args[2]);
    }
    
    [Test]
    public void TryParseFunctionCallLike_ArgumentsWithWhitespace_TrimsCorrectly()
    {
        // Arrange
        string input = "FunctionName( arg1 , arg2 , arg3 )";
        
        // Act
        bool result = input.TryParseFunctionCallLike(out string functionName, out string[] args);
        
        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual("FunctionName", functionName);
        Assert.IsNotNull(args);
        Assert.AreEqual(3, args.Length);
        Assert.AreEqual("arg1", args[0]);
        Assert.AreEqual("arg2", args[1]);
        Assert.AreEqual("arg3", args[2]);
    }
    
    [Test]
    public void TryParseFunctionCallLike_NestedParentheses_IgnoresCommasInside()
    {
        // Arrange
        string input = "Func(arg1(arg2, arg3), arg4)";
        
        // Act
        bool result = input.TryParseFunctionCallLike(out string functionName, out string[] args);
        
        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual("Func", functionName);
        Assert.IsNotNull(args);
        Assert.AreEqual(2, args.Length);
        Assert.AreEqual("arg1(arg2, arg3)", args[0]);
        Assert.AreEqual("arg4", args[1]);
    }
    
    [Test]
    public void TryParseFunctionCallLike_DeeplyNestedParentheses_WorksCorrectly()
    {
        // Arrange
        string input = "Func(arg1(arg2(arg3, arg4), arg5), arg6)";
        
        // Act
        bool result = input.TryParseFunctionCallLike(out string functionName, out string[] args);
        
        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual("Func", functionName);
        Assert.IsNotNull(args);
        Assert.AreEqual(2, args.Length);
        Assert.AreEqual("arg1(arg2(arg3, arg4), arg5)", args[0]);
        Assert.AreEqual("arg6", args[1]);
    }
    
    [Test]
    public void TryParseFunctionCallLike_MultipleNestedArguments_WorksCorrectly()
    {
        // Arrange
        string input = "Func(arg1(a, b), arg2(c, d), arg3)";
        
        // Act
        bool result = input.TryParseFunctionCallLike(out string functionName, out string[] args);
        
        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual("Func", functionName);
        Assert.IsNotNull(args);
        Assert.AreEqual(3, args.Length);
        Assert.AreEqual("arg1(a, b)", args[0]);
        Assert.AreEqual("arg2(c, d)", args[1]);
        Assert.AreEqual("arg3", args[2]);
    }
    
    [Test]
    public void TryParseFunctionCallLike_FunctionNameWithUnderscore_WorksCorrectly()
    {
        // Arrange
        string input = "Function_Name(arg1)";
        
        // Act
        bool result = input.TryParseFunctionCallLike(out string functionName, out string[] args);
        
        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual("Function_Name", functionName);
        Assert.IsNotNull(args);
        Assert.AreEqual(1, args.Length);
        Assert.AreEqual("arg1", args[0]);
    }
    
    [Test]
    public void TryParseFunctionCallLike_FunctionNameWithNumbers_WorksCorrectly()
    {
        // Arrange
        string input = "Function123(arg1)";
        
        // Act
        bool result = input.TryParseFunctionCallLike(out string functionName, out string[] args);
        
        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual("Function123", functionName);
        Assert.IsNotNull(args);
        Assert.AreEqual(1, args.Length);
        Assert.AreEqual("arg1", args[0]);
    }
    
    [Test]
    public void TryParseFunctionCallLike_FunctionNameWithWhitespace_TrimsCorrectly()
    {
        // Arrange
        string input = "  FunctionName  (arg1)";
        
        // Act
        bool result = input.TryParseFunctionCallLike(out string functionName, out string[] args);
        
        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual("FunctionName", functionName);
        Assert.IsNotNull(args);
        Assert.AreEqual(1, args.Length);
        Assert.AreEqual("arg1", args[0]);
    }
    
    [Test]
    public void TryParseFunctionCallLike_InvalidFunctionName_ReturnsFalse()
    {
        // Arrange
        string input = "Function-Name(arg1)";
        
        // Act
        bool result = input.TryParseFunctionCallLike(out string functionName, out string[] args);
        
        // Assert
        Assert.IsFalse(result);
    }
    
    [Test]
    public void TryParseFunctionCallLike_ContentAfterClosingParenthesis_ReturnsFalse()
    {
        // Arrange
        string input = "FunctionName(arg1) extra";
        
        // Act
        bool result = input.TryParseFunctionCallLike(out string functionName, out string[] args);
        
        // Assert
        Assert.IsFalse(result);
    }
    
    [Test]
    public void TryParseFunctionCallLike_WhitespaceAfterClosingParenthesis_ReturnsTrue()
    {
        // Arrange
        string input = "FunctionName(arg1)  ";
        
        // Act
        bool result = input.TryParseFunctionCallLike(out string functionName, out string[] args);
        
        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual("FunctionName", functionName);
        Assert.IsNotNull(args);
        Assert.AreEqual(1, args.Length);
        Assert.AreEqual("arg1", args[0]);
    }
    
    [Test]
    public void TryParseFunctionCallLike_UnmatchedParentheses_ReturnsFalse()
    {
        // Arrange
        string input = "FunctionName(arg1(arg2)";
        
        // Act
        bool result = input.TryParseFunctionCallLike(out string functionName, out string[] args);
        
        // Assert
        Assert.IsFalse(result);
    }
    
    [Test]
    public void TryParseFunctionCallLike_EmptyArgument_WorksCorrectly()
    {
        // Arrange
        string input = "FunctionName(, arg2)";
        
        // Act
        bool result = input.TryParseFunctionCallLike(out string functionName, out string[] args);
        
        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual("FunctionName", functionName);
        Assert.IsNotNull(args);
        Assert.AreEqual(2, args.Length);
        Assert.AreEqual("", args[0]);
        Assert.AreEqual("arg2", args[1]);
    }
    
    [Test]
    public void TryParseFunctionCallLike_EmptyLastArgument_WorksCorrectly()
    {
        // Arrange
        string input = "FunctionName(arg1, )";
        
        // Act
        bool result = input.TryParseFunctionCallLike(out string functionName, out string[] args);
        
        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual("FunctionName", functionName);
        Assert.IsNotNull(args);
        Assert.AreEqual(2, args.Length);
        Assert.AreEqual("arg1", args[0]);
        Assert.AreEqual("", args[1]);
    }
    
    [Test]
    public void TryParseFunctionCallLike_MultipleEmptyArguments_WorksCorrectly()
    {
        // Arrange
        string input = "FunctionName(, , arg3)";
        
        // Act
        bool result = input.TryParseFunctionCallLike(out string functionName, out string[] args);
        
        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual("FunctionName", functionName);
        Assert.IsNotNull(args);
        Assert.AreEqual(3, args.Length);
        Assert.AreEqual("", args[0]);
        Assert.AreEqual("", args[1]);
        Assert.AreEqual("arg3", args[2]);
    }
    
    [Test]
    public void TryParseFunctionCallLike_ComplexNestedExample_WorksCorrectly()
    {
        // Arrange - Example from comment: Func(arg1(arg2, arg3), arg4)
        string input = "Func(arg1(arg2, arg3), arg4)";
        
        // Act
        bool result = input.TryParseFunctionCallLike(out string functionName, out string[] args);
        
        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual("Func", functionName);
        Assert.IsNotNull(args);
        Assert.AreEqual(2, args.Length);
        Assert.AreEqual("arg1(arg2, arg3)", args[0]);
        Assert.AreEqual("arg4", args[1]);
    }
    
    [Test]
    public void TryParseFunctionCallLike_ArgumentWithMultipleNestedCalls_WorksCorrectly()
    {
        // Arrange
        string input = "Func(arg1(a(b, c), d), arg2)";
        
        // Act
        bool result = input.TryParseFunctionCallLike(out string functionName, out string[] args);
        
        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual("Func", functionName);
        Assert.IsNotNull(args);
        Assert.AreEqual(2, args.Length);
        Assert.AreEqual("arg1(a(b, c), d)", args[0]);
        Assert.AreEqual("arg2", args[1]);
    }
    
    #endregion
    
    #region TryParseNameValueInt Tests
    
    [Test]
    public void TryParseNameValueInt_NullString_ReturnsFalse()
    {
        // Arrange
        string input = null;
        
        // Act
        bool result = input.TryParseNameValue(out string name, out int value);
        
        // Assert
        Assert.IsFalse(result);
        Assert.IsNull(name);
        Assert.AreEqual(0, value);
    }
    
    [Test]
    public void TryParseNameValueInt_EmptyString_ReturnsFalse()
    {
        // Arrange
        string input = "";
        
        // Act
        bool result = input.TryParseNameValue(out string name, out int value);
        
        // Assert
        Assert.IsFalse(result);
        Assert.IsNull(name);
        Assert.AreEqual(0, value);
    }
    
    [Test]
    public void TryParseNameValueInt_ValidFormat_DefaultSeparator_ReturnsTrue()
    {
        // Arrange
        string input = "abc:123";
        
        // Act
        bool result = input.TryParseNameValue(out string name, out int value);
        
        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual("abc", name);
        Assert.AreEqual(123, value);
    }
    
    [Test]
    public void TryParseNameValueInt_ValidFormat_CustomSeparator_ReturnsTrue()
    {
        // Arrange
        string input = "abc=123";
        
        // Act
        bool result = input.TryParseNameValue(out string name, out int value, '=');
        
        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual("abc", name);
        Assert.AreEqual(123, value);
    }
    
    [Test]
    public void TryParseNameValueInt_WithWhitespace_TrimsCorrectly()
    {
        // Arrange
        string input = "  abc  :  123  ";
        
        // Act
        bool result = input.TryParseNameValue(out string name, out int value);
        
        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual("abc", name);
        Assert.AreEqual(123, value);
    }
    
    [Test]
    public void TryParseNameValueInt_NegativeValue_ReturnsTrue()
    {
        // Arrange
        string input = "abc:-123";
        
        // Act
        bool result = input.TryParseNameValue(out string name, out int value);
        
        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual("abc", name);
        Assert.AreEqual(-123, value);
    }
    
    [Test]
    public void TryParseNameValueInt_NoSeparator_ReturnsFalse()
    {
        // Arrange
        string input = "abc123";
        
        // Act
        bool result = input.TryParseNameValue(out string name, out int value);
        
        // Assert
        Assert.IsFalse(result);
    }
    
    [Test]
    public void TryParseNameValueInt_EmptyName_ReturnsFalse()
    {
        // Arrange
        string input = ":123";
        
        // Act
        bool result = input.TryParseNameValue(out string name, out int value);
        
        // Assert
        Assert.IsFalse(result);
    }
    
    [Test]
    public void TryParseNameValueInt_EmptyValue_ReturnsFalse()
    {
        // Arrange
        string input = "abc:";
        
        // Act
        bool result = input.TryParseNameValue(out string name, out int value);
        
        // Assert
        Assert.IsFalse(result);
    }
    
    [Test]
    public void TryParseNameValueInt_InvalidValue_ReturnsFalse()
    {
        // Arrange
        string input = "abc:xyz";
        
        // Act
        bool result = input.TryParseNameValue(out string name, out int value);
        
        // Assert
        Assert.IsFalse(result);
    }
    
    [Test]
    public void TryParseNameValueInt_MultipleSeparators_UsesLast()
    {
        // Arrange
        string input = "abc:123:456";
        
        // Act
        bool result = input.TryParseNameValue(out string name, out int value);
        
        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual("abc:123", name);
        Assert.AreEqual(456, value);
    }
    
    [Test]
    public void TryParseNameValueInt_ZeroValue_ReturnsTrue()
    {
        // Arrange
        string input = "abc:0";
        
        // Act
        bool result = input.TryParseNameValue(out string name, out int value);
        
        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual("abc", name);
        Assert.AreEqual(0, value);
    }
    
    #endregion
    
    #region TryParseNameValueFloat Tests
    
    [Test]
    public void TryParseNameValueFloat_NullString_ReturnsFalse()
    {
        // Arrange
        string input = null;
        
        // Act
        bool result = input.TryParseNameValue(out string name, out float value);
        
        // Assert
        Assert.IsFalse(result);
        Assert.IsNull(name);
        Assert.AreEqual(0f, value);
    }
    
    [Test]
    public void TryParseNameValueFloat_EmptyString_ReturnsFalse()
    {
        // Arrange
        string input = "";
        
        // Act
        bool result = input.TryParseNameValue(out string name, out float value);
        
        // Assert
        Assert.IsFalse(result);
        Assert.IsNull(name);
        Assert.AreEqual(0f, value);
    }
    
    [Test]
    public void TryParseNameValueFloat_ValidFormat_DefaultSeparator_ReturnsTrue()
    {
        // Arrange
        string input = "abc:123.5";
        
        // Act
        bool result = input.TryParseNameValue(out string name, out float value);
        
        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual("abc", name);
        Assert.AreEqual(123.5f, value);
    }
    
    [Test]
    public void TryParseNameValueFloat_ValidFormat_CustomSeparator_ReturnsTrue()
    {
        // Arrange
        string input = "abc=123.5";
        
        // Act
        bool result = input.TryParseNameValue(out string name, out float value, '=');
        
        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual("abc", name);
        Assert.AreEqual(123.5f, value);
    }
    
    [Test]
    public void TryParseNameValueFloat_WithWhitespace_TrimsCorrectly()
    {
        // Arrange
        string input = "  abc  :  123.5  ";
        
        // Act
        bool result = input.TryParseNameValue(out string name, out float value);
        
        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual("abc", name);
        Assert.AreEqual(123.5f, value);
    }
    
    [Test]
    public void TryParseNameValueFloat_NegativeValue_ReturnsTrue()
    {
        // Arrange
        string input = "abc:-123.5";
        
        // Act
        bool result = input.TryParseNameValue(out string name, out float value);
        
        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual("abc", name);
        Assert.AreEqual(-123.5f, value);
    }
    
    [Test]
    public void TryParseNameValueFloat_IntegerValue_ReturnsTrue()
    {
        // Arrange
        string input = "abc:123";
        
        // Act
        bool result = input.TryParseNameValue(out string name, out float value);
        
        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual("abc", name);
        Assert.AreEqual(123f, value);
    }
    
    [Test]
    public void TryParseNameValueFloat_ScientificNotation_ReturnsTrue()
    {
        // Arrange
        string input = "abc:1.5e2";
        
        // Act
        bool result = input.TryParseNameValue(out string name, out float value);
        
        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual("abc", name);
        Assert.AreEqual(150f, value, 0.001f);
    }
    
    [Test]
    public void TryParseNameValueFloat_NoSeparator_ReturnsFalse()
    {
        // Arrange
        string input = "abc123.5";
        
        // Act
        bool result = input.TryParseNameValue(out string name, out float value);
        
        // Assert
        Assert.IsFalse(result);
    }
    
    [Test]
    public void TryParseNameValueFloat_EmptyName_ReturnsFalse()
    {
        // Arrange
        string input = ":123.5";
        
        // Act
        bool result = input.TryParseNameValue(out string name, out float value);
        
        // Assert
        Assert.IsFalse(result);
    }
    
    [Test]
    public void TryParseNameValueFloat_EmptyValue_ReturnsFalse()
    {
        // Arrange
        string input = "abc:";
        
        // Act
        bool result = input.TryParseNameValue(out string name, out float value);
        
        // Assert
        Assert.IsFalse(result);
    }
    
    [Test]
    public void TryParseNameValueFloat_InvalidValue_ReturnsFalse()
    {
        // Arrange
        string input = "abc:xyz";
        
        // Act
        bool result = input.TryParseNameValue(out string name, out float value);
        
        // Assert
        Assert.IsFalse(result);
    }
    
    [Test]
    public void TryParseNameValueFloat_MultipleSeparators_UsesLast()
    {
        // Arrange
        string input = "abc:123.5:456.7";
        
        // Act
        bool result = input.TryParseNameValue(out string name, out float value);
        
        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual("abc:123.5", name);
        Assert.AreEqual(456.7f, value);
    }
    
    [Test]
    public void TryParseNameValueFloat_ZeroValue_ReturnsTrue()
    {
        // Arrange
        string input = "abc:0";
        
        // Act
        bool result = input.TryParseNameValue(out string name, out float value);
        
        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual("abc", name);
        Assert.AreEqual(0f, value);
    }
    
    [Test]
    public void TryParseNameValueFloat_DecimalPointOnly_ReturnsTrue()
    {
        // Arrange
        string input = "abc:.5";
        
        // Act
        bool result = input.TryParseNameValue(out string name, out float value);
        
        // Assert
        Assert.IsTrue(result);
        Assert.AreEqual("abc", name);
        Assert.AreEqual(0.5f, value);
    }
    
    #endregion

    #region PatternReplace Tests

    static string[] BuildContents(int count)
    {
        var contents = new string[count];
        for(int i = 0; i < count; i++)
            contents[i] = (i + 1).ToString();
        return contents;
    }

    [Test]
    public void PatternReplace_EmptyString_ReturnsEmpty()
    {
        var result = MethodExtensions.PatternReplace("", new[] { "x" });
        Assert.AreEqual("", result);
    }

    [Test]
    public void PatternReplace_ReplaceSinglePlaceholder_Works()
    {
        var result = MethodExtensions.PatternReplace("Hello $1", new[] { "World" });
        Assert.AreEqual("Hello World", result);
    }

    [Test]
    public void PatternReplace_ReplaceMultiplePlaceholders_Works()
    {
        var result = MethodExtensions.PatternReplace("$1-$2-$1", new[] { "A", "B" });
        Assert.AreEqual("A-B-A", result);
    }

    [Test]
    public void PatternReplace_MultiDigitIndex_Works()
    {
        var contents = BuildContents(12);
        var result = MethodExtensions.PatternReplace("v$12", contents);
        Assert.AreEqual("v12", result);
    }

    [Test]
    public void PatternReplace_OutOfRangeIndex_RemovesPlaceholder()
    {
        var result = MethodExtensions.PatternReplace("a$2b", new[] { "x" });
        Assert.AreEqual("ab", result);
    }

    [Test]
    public void PatternReplace_ZeroIndex_RemovesPlaceholder()
    {
        var result = MethodExtensions.PatternReplace("a$0b", new[] { "x" });
        Assert.AreEqual("ab", result);
    }

    [Test]
    public void PatternReplace_DoubleDollar_EscapesToSingleDollar()
    {
        var result = MethodExtensions.PatternReplace("a$$b", new[] { "x" });
        Assert.AreEqual("a$b", result);
    }

    [Test]
    public void PatternReplace_DoubleDollarBeforeDigits_ProducesLiteralDollarThenDigits()
    {
        var result = MethodExtensions.PatternReplace("$$1", new[] { "x" });
        Assert.AreEqual("$1", result);
    }

    [Test]
    public void PatternReplace_DollarNotFollowedByDigitOrDollar_KeepsDollar()
    {
        var result = MethodExtensions.PatternReplace("a$b", new[] { "x" });
        Assert.AreEqual("a$b", result);
    }

    [Test]
    public void PatternReplace_DollarAtEnd_KeepsDollar()
    {
        var result = MethodExtensions.PatternReplace("a$", new[] { "x" });
        Assert.AreEqual("a$", result);
    }

    #endregion

    #region ParseNameValues Tests

    [Test]
    public void ParseNameValuesFloat_MultipleItems_ParsesCorrectly()
    {
        string input = "id1:1.5,id2:2.0";
        var results = new List<(string, float)>();
        
        input.ParseNameValues(results);
        
        Assert.AreEqual(2, results.Count);
        Assert.AreEqual("id1", results[0].Item1);
        Assert.AreEqual(1.5f, results[0].Item2);
        Assert.AreEqual("id2", results[1].Item1);
        Assert.AreEqual(2.0f, results[1].Item2);
    }

    [Test]
    public void ParseNameValuesFloat_WithWhitespace_ParsesCorrectly()
    {
        string input = " id1 : 1.5 , id2 : 2.0 ";
        var results = new List<(string, float)>();
        
        input.ParseNameValues(results);
        
        Assert.AreEqual(2, results.Count);
        Assert.AreEqual("id1", results[0].Item1);
        Assert.AreEqual(1.5f, results[0].Item2);
        Assert.AreEqual("id2", results[1].Item1);
        Assert.AreEqual(2.0f, results[1].Item2);
    }

    [Test]
    public void ParseNameValuesFloat_InvalidItem_Skipped()
    {
        string input = "id1:1.5,invalid,id2:2.0";
        var results = new List<(string, float)>();
        
        input.ParseNameValues(results);
        
        Assert.AreEqual(2, results.Count);
        Assert.AreEqual("id1", results[0].Item1);
        Assert.AreEqual("id2", results[1].Item1);
    }

    [Test]
    public void ParseNameValuesFloat_CustomSeparators_ParsesCorrectly()
    {
        string input = "id1=1.5;id2=2.0";
        var results = new List<(string, float)>();
        
        input.ParseNameValues(results, ';', '=');
        
        Assert.AreEqual(2, results.Count);
        Assert.AreEqual("id1", results[0].Item1);
        Assert.AreEqual(1.5f, results[0].Item2);
        Assert.AreEqual("id2", results[1].Item1);
        Assert.AreEqual(2.0f, results[1].Item2);
    }

    [Test]
    public void TryParseNameValuesFloat_OutArray_ParsesCorrectly()
    {
        string input = "id1:1.5,id2:2.0";
        
        bool success = input.TryParseNameValues(out (string name, float value)[] results);
        
        Assert.IsTrue(success);
        Assert.AreEqual(2, results.Length);
        Assert.AreEqual("id1", results[0].name);
        Assert.AreEqual(1.5f, results[0].value);
        Assert.AreEqual("id2", results[1].name);
        Assert.AreEqual(2.0f, results[1].value);
    }

    [Test]
    public void ParseNameValuesInt_MultipleItems_ParsesCorrectly()
    {
        string input = "id1:1,id2:2";
        var results = new List<(string, int)>();
        
        input.ParseNameValues(results);
        
        Assert.AreEqual(2, results.Count);
        Assert.AreEqual("id1", results[0].Item1);
        Assert.AreEqual(1, results[0].Item2);
        Assert.AreEqual("id2", results[1].Item1);
        Assert.AreEqual(2, results[1].Item2);
    }

    [Test]
    public void TryParseNameValuesInt_OutArray_ParsesCorrectly()
    {
        string input = "id1:1,id2:2";
        
        bool success = input.TryParseNameValues(out (string name, int value)[] results);
        
        Assert.IsTrue(success);
        Assert.AreEqual(2, results.Length);
        Assert.AreEqual("id1", results[0].name);
        Assert.AreEqual(1, results[0].value);
        Assert.AreEqual("id2", results[1].name);
        Assert.AreEqual(2, results[1].value);
    }

    #endregion

    [Test]
    public void PatternReplace_CalledTwice_DoesNotLeakPreviousOutput()
    {
        Assert.AreEqual("A", MethodExtensions.PatternReplace("$1", new[] { "A" }));
        Assert.AreEqual("BB", MethodExtensions.PatternReplace("$1$1", new[] { "B" }));
    }
}
