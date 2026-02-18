using NUnit.Framework;
using Prota;

public class CircleListTests
{
    [Test]
    public void TestCircleArray()
    {
        CircleList<string> list = new CircleList<string>();
        list.PushBack("a");
        list.PushBack("b");
        list.PushBack("c");
        list.PushBack("d");
        list.PushBack("e");
        
        Assert.AreEqual(5, list.count);
        Assert.AreEqual("a", list[0]);
        Assert.AreEqual("b", list[1]);
        Assert.AreEqual("c", list[2]);
        Assert.AreEqual("d", list[3]);
        Assert.AreEqual("e", list[4]);
        
        list.PopBack();
        Assert.AreEqual(4, list.count);
        Assert.AreEqual("a", list[0]);
        Assert.AreEqual("b", list[1]);
        Assert.AreEqual("c", list[2]);
        Assert.AreEqual("d", list[3]);
        
        list.PopFront();
        Assert.AreEqual(3, list.count);
        Assert.AreEqual("b", list[0]);
        Assert.AreEqual("c", list[1]);
        Assert.AreEqual("d", list[2]);
        
        list[0] = "x";
        list[1] = "y";
        
        Assert.AreEqual(3, list.count);
        Assert.AreEqual("x", list[0]);
        Assert.AreEqual("y", list[1]);
        Assert.AreEqual("d", list[2]);
    }
}
