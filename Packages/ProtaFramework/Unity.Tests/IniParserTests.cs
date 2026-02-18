using NUnit.Framework;
using Prota;

public class IniParserTests
{
    [Test]
    public void UnitTest()
    {
        var ini = @"
; This is a comment
[header1]
key1 = value1
key2 = value2
key3 = value3    ; this is another comment
[header2]   ;; comment!
key1 = 1
key2 = 1.5
key3 = value3
key4 = ""12-34""   
";
        var parser = new IniParser(ini);
        
        Assert.AreEqual("value1", parser.entries["header1"]["key1"].value);
        Assert.AreEqual("value2", parser.entries["header1"]["key2"].value);
        Assert.AreEqual("value3", parser.entries["header1"]["key3"].value);
        Assert.AreEqual("1", parser.entries["header2"]["key1"].value);
        Assert.AreEqual("1.5", parser.entries["header2"]["key2"].value);
        Assert.AreEqual("value3", parser.entries["header2"]["key3"].value);
        Assert.AreEqual("12-34", parser.entries["header2"]["key4"].value);
        Assert.AreEqual(2, parser.entries.Count);
        Assert.AreEqual(3, parser.entries["header1"].Count);
        Assert.AreEqual(4, parser.entries["header2"].Count);
        Assert.AreEqual(1, parser.entries["header2"]["key1"].intValue);
        Assert.AreEqual(1.5f, parser.entries["header2"]["key2"].floatValue);
    }
}
