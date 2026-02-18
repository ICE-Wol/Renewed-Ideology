using NUnit.Framework;
using Prota;
using System;
using System.Collections.Generic;

public class DenseSetTests
{
    private class TestNode
    {
        public string Name { get; set; }
        public TestNode(string name) => Name = name;
        public override string ToString() => Name;
    }

    [Test]
    public void TestArrayCollectionT_RemoveBySwap()
    {
        var collection = new DenseSet<TestNode>(4);
        var a = new TestNode("A");
        var b = new TestNode("B");
        var c = new TestNode("C");
        var d = new TestNode("D");

        collection.Add(a); // index 0
        collection.Add(b); // index 1
        collection.Add(c); // index 2
        collection.Add(d); // index 3

        Assert.AreEqual(4, collection.Count);
        Assert.AreEqual(a, collection[0]);
        Assert.AreEqual(b, collection[1]);
        Assert.AreEqual(c, collection[2]);
        Assert.AreEqual(d, collection[3]);

        // Remove B (index 1)
        // RemoveBySwap mechanism: 
        // 1. last element (D, index 3) moves to index 1.
        // 2. index 3 becomes null.
        // 3. count becomes 3.
        collection.Remove(b);

        Assert.AreEqual(3, collection.Count);
        Assert.AreEqual(a, collection[0]);
        Assert.AreEqual(d, collection[1], "D should be swapped to index 1");
        Assert.AreEqual(c, collection[2]);
        
        Assert.IsFalse(collection.Contains(b));
        Assert.IsTrue(collection.Contains(d));
        
        int dIndex;
        Assert.IsTrue(collection.TryGetIndex(d, out dIndex));
        Assert.AreEqual(1, dIndex, "D's index should be updated to 1");
    }

    [Test]
    public void TestArrayCollectionT_Replace()
    {
        var collection = new DenseSet<TestNode>(4);
        var a = new TestNode("A");
        var b = new TestNode("B");
        var c = new TestNode("C");
        collection.Add(a);
        collection.Add(b);

        var original = collection.Replace(1, c);
        Assert.AreEqual(b, original);
        Assert.AreEqual(c, collection[1]);
        Assert.AreEqual(2, collection.Count);
        Assert.IsFalse(collection.Contains(b));
        Assert.IsTrue(collection.Contains(c));
        
        int cIndex;
        Assert.IsTrue(collection.TryGetIndex(c, out cIndex));
        Assert.AreEqual(1, cIndex);
    }

    [Test]
    public void TestArrayCollectionTK_RemoveBySwap()
    {
        // K=string, V=TestNode. getValue maps string name to a new TestNode.
        var collection = new DenseSet<string, TestNode>(4, name => new TestNode(name));
        var a = new TestNode("A");
        var b = new TestNode("B");
        var c = new TestNode("C");
        var d = new TestNode("D");

        collection.Add("A", a); // index 0
        collection.Add("B", b); // index 1
        collection.Add("C", c); // index 2
        collection.Add("D", d); // index 3

        Assert.AreEqual(4, collection.Count);
        
        // Remove B by key
        collection.RemoveByKey("B");

        Assert.AreEqual(3, collection.Count);
        Assert.AreEqual(a, collection[0]);
        Assert.AreEqual(d, collection[1], "D should be swapped to index 1");
        Assert.AreEqual(c, collection[2]);

        Assert.IsFalse(collection.ContainsKey("B"));
        Assert.IsTrue(collection.ContainsKey("D"));

        int dIndex;
        Assert.IsTrue(collection.TryGetIndexByKey("D", out dIndex));
        Assert.AreEqual(1, dIndex, "D's index should be updated to 1");
    }

    [Test]
    public void TestArrayCollectionTK_Interfaces()
    {
        var collection = new DenseSet<string, TestNode>(4, name => new TestNode(name));
        var a = new TestNode("A");
        var b = new TestNode("B");
        collection.Add("A", a);
        collection.Add("B", b);

        // IReadOnlyDictionary
        IReadOnlyDictionary<string, TestNode> dict = collection;
        Assert.AreEqual(2, dict.Count);
        Assert.IsTrue(dict.ContainsKey("A"));
        Assert.AreEqual(a, dict["A"]);
        
        TestNode val;
        Assert.IsTrue(dict.TryGetValue("B", out val));
        Assert.AreEqual(b, val);

        // Keys and Values
        var keys = new List<string>(dict.Keys);
        Assert.AreEqual(2, keys.Count);
        Assert.Contains("A", keys);
        Assert.Contains("B", keys);

        var values = new List<TestNode>(dict.Values);
        Assert.AreEqual(2, values.Count);
        Assert.Contains(a, values);
        Assert.Contains(b, values);

        // Foreach KeyValuePair
        int count = 0;
        foreach (var kvp in collection)
        {
            if (count == 0) { Assert.AreEqual("A", kvp.Key); Assert.AreEqual(a, kvp.Value); }
            if (count == 1) { Assert.AreEqual("B", kvp.Key); Assert.AreEqual(b, kvp.Value); }
            count++;
        }
        Assert.AreEqual(2, count);
    }

    [Test]
    public void TestArrayCollection_Iteration()
    {
        var collection = new DenseSet<TestNode>(4);
        var a = new TestNode("A");
        var b = new TestNode("B");
        collection.Add(a);
        collection.Add(b);

        int count = 0;
        foreach (var item in collection)
        {
            if (count == 0) Assert.AreEqual(a, item);
            if (count == 1) Assert.AreEqual(b, item);
            count++;
        }
        Assert.AreEqual(2, count);
    }

    [Test]
    public void TestCompareWithHashSet()
    {
        var denseSet = new DenseSet<TestNode>(10);
        var hashSet = new HashSet<TestNode>();
        var nodes = new List<TestNode>();
        for (int i = 0; i < 10; i++) nodes.Add(new TestNode(i.ToString()));

        // Add
        foreach (var node in nodes)
        {
            denseSet.Add(node);
            hashSet.Add(node);
        }
        Assert.AreEqual(hashSet.Count, denseSet.Count);
        foreach (var node in nodes)
        {
            Assert.IsTrue(denseSet.Contains(node));
            Assert.IsTrue(hashSet.Contains(node));
        }

        // Remove
        denseSet.Remove(nodes[3]);
        hashSet.Remove(nodes[3]);
        denseSet.Remove(nodes[7]);
        hashSet.Remove(nodes[7]);

        Assert.AreEqual(hashSet.Count, denseSet.Count);
        Assert.IsFalse(denseSet.Contains(nodes[3]));
        Assert.IsFalse(hashSet.Contains(nodes[3]));
        Assert.IsFalse(denseSet.Contains(nodes[7]));
        Assert.IsFalse(hashSet.Contains(nodes[7]));

        // Iteration content
        var denseList = new List<TestNode>(denseSet);
        var hashList = new List<TestNode>(hashSet);
        Assert.AreEqual(hashList.Count, denseList.Count);
        foreach (var item in denseList) Assert.IsTrue(hashSet.Contains(item));
    }

    [Test]
    public void TestCompareWithDictionary()
    {
        var denseDict = new DenseSet<string, TestNode>(10, k => new TestNode(k));
        var dictionary = new Dictionary<string, TestNode>();
        var keys = new[] { "A", "B", "C", "D", "E" };

        // Add
        foreach (var key in keys)
        {
            var node = new TestNode(key);
            denseDict.Add(key, node);
            dictionary.Add(key, node);
        }
        Assert.AreEqual(dictionary.Count, denseDict.Count);

        // ContainsKey and Value
        foreach (var key in keys)
        {
            Assert.IsTrue(denseDict.ContainsKey(key));
            Assert.IsTrue(dictionary.ContainsKey(key));
            Assert.AreEqual(dictionary[key], ((IReadOnlyDictionary<string, TestNode>)denseDict)[key]);
        }

        // Remove
        denseDict.RemoveByKey("B");
        dictionary.Remove("B");
        Assert.AreEqual(dictionary.Count, denseDict.Count);
        Assert.IsFalse(denseDict.ContainsKey("B"));
        Assert.IsFalse(dictionary.ContainsKey("B"));

        // Iteration
        int count = 0;
        foreach (var kvp in denseDict)
        {
            Assert.IsTrue(dictionary.ContainsKey(kvp.Key));
            Assert.AreEqual(dictionary[kvp.Key], kvp.Value);
            count++;
        }
        Assert.AreEqual(dictionary.Count, count);
    }

    [Test]
    public void TestRandomOperations_T()
    {
        var denseSet = new DenseSet<TestNode>(10);
        var hashSet = new HashSet<TestNode>();
        var allNodes = new List<TestNode>();
        for (int i = 0; i < 2000; i++) allNodes.Add(new TestNode(i.ToString()));
        
        var random = new Random(42);
        var addedNodes = new List<TestNode>();

        for (int i = 0; i < 1000; i++)
        {
            int op = random.Next(3);
            if (op == 0 || addedNodes.Count == 0) // Add
            {
                var node = allNodes[random.Next(allNodes.Count)];
                if (!hashSet.Contains(node))
                {
                    denseSet.Add(node);
                    hashSet.Add(node);
                    addedNodes.Add(node);
                }
            }
            else if (op == 1) // Remove
            {
                int index = random.Next(addedNodes.Count);
                var node = addedNodes[index];
                denseSet.Remove(node);
                hashSet.Remove(node);
                addedNodes.RemoveAt(index);
            }
            else // Contains
            {
                var node = allNodes[random.Next(allNodes.Count)];
                Assert.AreEqual(hashSet.Contains(node), denseSet.Contains(node));
            }

            Assert.AreEqual(hashSet.Count, denseSet.Count);
        }

        // Final check
        Assert.AreEqual(hashSet.Count, denseSet.Count);
        foreach (var node in addedNodes)
        {
            Assert.IsTrue(denseSet.Contains(node));
        }
    }

    [Test]
    public void TestRandomOperations_KV()
    {
        var denseDict = new DenseSet<string, TestNode>(10, k => new TestNode(k));
        var dictionary = new Dictionary<string, TestNode>();
        
        var random = new Random(43);
        var addedKeys = new List<string>();

        for (int i = 0; i < 1000; i++)
        {
            int op = random.Next(3);
            if (op == 0 || addedKeys.Count == 0) // Add
            {
                string key = random.Next(2000).ToString();
                if (!dictionary.ContainsKey(key))
                {
                    var node = new TestNode(key);
                    denseDict.Add(key, node);
                    dictionary.Add(key, node);
                    addedKeys.Add(key);
                }
            }
            else if (op == 1) // Remove
            {
                int index = random.Next(addedKeys.Count);
                string key = addedKeys[index];
                denseDict.RemoveByKey(key);
                dictionary.Remove(key);
                addedKeys.RemoveAt(index);
            }
            else // ContainsKey & Value
            {
                string key = random.Next(2000).ToString();
                Assert.AreEqual(dictionary.ContainsKey(key), denseDict.ContainsKey(key));
                if (dictionary.ContainsKey(key))
                {
                    Assert.AreEqual(dictionary[key], ((IReadOnlyDictionary<string, TestNode>)denseDict)[key]);
                }
            }

            Assert.AreEqual(dictionary.Count, denseDict.Count);
        }

        // Final iteration check
        int count = 0;
        foreach (var kvp in denseDict)
        {
            Assert.IsTrue(dictionary.ContainsKey(kvp.Key));
            Assert.AreEqual(dictionary[kvp.Key], kvp.Value);
            count++;
        }
        Assert.AreEqual(dictionary.Count, count);
    }
}
