using task05;
using Xunit;

public class TestClass
{
    public int PublicField;
    private string _privateField;
    public int Property { get; set; }

    public void Method() { }
}

[Serializable]
public class AttributedClass { }

public class ClassAnalyzerTests
{
    [Fact]
    public void GetPublicMethods_ReturnsCorrectMethods()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var methods = analyzer.GetPublicMethods();

        Assert.Contains("Method", methods);
    }

    [Fact]
    public void GetAllFields_IncludesPrivateFields()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var fields = analyzer.GetAllFields();

        Assert.Contains("_privateField", fields);
    }

    [Fact]
    public void GetMethodParams_ReturnsCorrectSignature()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));

        var simpleMethodInfo = analyzer.GetMethodParams("Method");

        Assert.Contains("Void Method()", simpleMethodInfo);

    }

    [Fact]
    public void HasAttribute_ReturnsTrueForExistingAttribute()
    {
        var analyzerWithAttr = new ClassAnalyzer(typeof(AttributedClass));
        var analyzerWithoutAttr = new ClassAnalyzer(typeof(TestClass));

        Assert.True(analyzerWithAttr.HasAttribute<SerializableAttribute>());
        Assert.False(analyzerWithoutAttr.HasAttribute<SerializableAttribute>());
    }

    [Fact]
    public void GetProperties_ReturnsCorrectProperties()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var properties = analyzer.GetProperties();
        Assert.Contains("Property", properties);
        Assert.Single(properties);
    }


}
