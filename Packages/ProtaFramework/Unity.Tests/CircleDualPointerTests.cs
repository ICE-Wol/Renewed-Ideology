using NUnit.Framework;
using Prota;

public class CircleDualPointerTests
{
    [Test]
    public void TestCircleDualPointer()
    {
        CircleDualPointer g = new CircleDualPointer(6);
        try { g.HeadMoveAhead(); Assert.Fail("Should throw exception"); } catch { }
        try { g.TailMoveBack(); Assert.Fail("Should throw exception"); } catch { }
        
        Assert.IsTrue(g.isEmpty);
        Assert.IsFalse(g.isFull);
        
        g.HeadMoveBack();
        g.TailMoveAhead();
        Assert.AreEqual(5, g.head);
        Assert.AreEqual(1, g.tail);
        Assert.AreEqual(2, g.count);
        Assert.AreEqual(5, g[0]);
        Assert.AreEqual(0, g[1]);
        
        g.TailMoveAhead();
        g.TailMoveAhead();
        g.TailMoveAhead();
        g.TailMoveAhead();
        Assert.IsTrue(g.isFull);
        Assert.IsFalse(g.isEmpty);
        
        Assert.AreEqual(g.max, g.count);
        g.HeadMoveAhead();
        
        Assert.AreEqual(g.max - 1, g.count);
        Assert.AreEqual(5, g.count);
        
        g.Reset();
        Assert.AreEqual(0, g.count);
        Assert.AreEqual(0, g.head);
        Assert.AreEqual(0, g.tail);
    }
}
