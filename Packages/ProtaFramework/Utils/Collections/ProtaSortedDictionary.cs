using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Prota
{
    public class ProtaSortedDictoianry<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
        where TKey : IComparable<TKey>
    {
        private struct Entry
        {
            public TKey Key;
            public TValue Value;

            public Entry(TKey key, TValue value)
            {
                Key = key;
                Value = value;
            }
        }

        // 一个 cache line 64B, 能存 16个 int. 给它翻倍的大小.
        // 大小需要更大一点以避免太频繁的内存分配和block表遍历.
        private const int MAX_BLOCK_SIZE = 64;
        private const int MIN_BLOCK_SIZE = MAX_BLOCK_SIZE / 2;

        private class Block
        {
            public TKey startKey;
            public Entry[] elements;
            public int Count;

            public Block(TKey startKey, int capacity = MAX_BLOCK_SIZE)
            {
                this.startKey = startKey;
                elements = new Entry[capacity];
                Count = 0;
            }
        }

        private class BlockPool
        {
            private Stack<Block> pool = new Stack<Block>();

            public Block Rent(TKey startKey)
            {
                if (pool.Count > 0)
                {
                    var block = pool.Pop();
                    block.startKey = startKey;
                    block.Count = 0;
                    return block;
                }
                return new Block(startKey);
            }

            public void Return(Block block)
            {
                Array.Clear(block.elements, 0, block.elements.Length);
                pool.Push(block);
            }
        }

        private readonly BlockPool blockPool = new BlockPool();
        private List<Block> blocks = new List<Block>();
        private int count;

        public int Count => count;

        public void Add(TKey key, TValue value)
        {
            if (TryGet(key, out _))
                throw new ArgumentException($"Key {key} already exists in ProtaSortedArray.");
            AddInternal(key, value);
        }

        protected void AddInternal(TKey key, TValue value)
        {
            if (blocks.Count == 0)
            {
                Block newBlock = blockPool.Rent(key);
                blocks.Add(newBlock);
                blocks[0].elements[0] = new Entry(key, value);
                blocks[0].Count = 1;
                count = 1;
                return;
            }

            int blockIndex = FindBlockIndex(key);
            Block block = blocks[blockIndex];
            
            int insertPos = FindInsertPosition(block, key);
            
            if (block.Count >= MAX_BLOCK_SIZE)
            {
                SplitBlock(blockIndex);
                AddInternal(key, value); // Recursively add after split
                return;
            }

            for (int i = block.Count; i > insertPos; i--)
            {
                block.elements[i] = block.elements[i - 1];
            }

            block.elements[insertPos] = new Entry(key, value);
            block.Count++;
            count++;
        }

        public TValue this[TKey key]
        {
            get
            {
                if (TryGet(key, out TValue value))
                {
                    return value;
                }
                throw new KeyNotFoundException($"Key {key} not found in ProtaSortedArray.");
            }
            set
            {
                AddInternal(key, value);
            }
        }

        private int FindBlockIndex(TKey key)
        {
            int left = 0;
            int right = blocks.Count - 1;

            while (left < right)
            {
                int mid = (left + right + 1) / 2;
                if (blocks[mid].startKey.CompareTo(key) <= 0)
                    left = mid;
                else
                    right = mid - 1;
            }
            return left;
        }

        private int FindInsertPosition(Block block, TKey key)
        {
            int left = 0;
            int right = block.Count - 1;

            while (left <= right)
            {
                int mid = (left + right) / 2;
                int comparison = block.elements[mid].Key.CompareTo(key);

                if (comparison == 0)
                    return mid;
                if (comparison < 0)
                    left = mid + 1;
                else
                    right = mid - 1;
            }
            return left;
        }

        private void SplitBlock(int blockIndex)
        {
            Block oldBlock = blocks[blockIndex];
            Block newBlock = blockPool.Rent(oldBlock.elements[MIN_BLOCK_SIZE].Key);

            for (int i = 0; i < MIN_BLOCK_SIZE; i++)
            {
                newBlock.elements[i] = oldBlock.elements[i + MIN_BLOCK_SIZE];
            }

            oldBlock.Count = MIN_BLOCK_SIZE;
            newBlock.Count = MIN_BLOCK_SIZE;

            blocks.Insert(blockIndex + 1, newBlock);
        }

        public bool TryGet(TKey key, out TValue value)
        {
            if (blocks.Count == 0)
            {
                value = default;
                return false;
            }

            int blockIndex = FindBlockIndex(key);
            Block block = blocks[blockIndex];
            int pos = FindInsertPosition(block, key);

            if (pos < block.Count && block.elements[pos].Key.CompareTo(key) == 0)
            {
                value = block.elements[pos].Value;
                return true;
            }

            value = default;
            return false;
        }

        public bool ContainsKey(TKey key)
        {
            return TryGet(key, out _);
        }

        public bool Remove(TKey key)
        {
            if (blocks.Count == 0)
                return false;

            int blockIndex = FindBlockIndex(key);
            Block block = blocks[blockIndex];
            int pos = FindInsertPosition(block, key);

            if (pos >= block.Count || block.elements[pos].Key.CompareTo(key) != 0)
                return false;

            for (int i = pos; i < block.Count - 1; i++)
            {
                block.elements[i] = block.elements[i + 1];
            }

            block.Count--;
            count--;

            if (block.Count == 0 && blocks.Count > 1)
            {
                blocks.RemoveAt(blockIndex);
                blockPool.Return(block);
                return true;
            }

            if (block.Count < MIN_BLOCK_SIZE && blocks.Count > 1)
            {
                if (blockIndex > 0)
                {
                    Block prevBlock = blocks[blockIndex - 1];
                    if (prevBlock.Count + block.Count <= MIN_BLOCK_SIZE)
                    {
                        for (int i = 0; i < block.Count; i++)
                        {
                            prevBlock.elements[prevBlock.Count + i] = block.elements[i];
                        }
                        prevBlock.Count += block.Count;
                        blocks.RemoveAt(blockIndex);
                        blockPool.Return(block);
                        return true;
                    }
                }

                if (blockIndex < blocks.Count - 1)
                {
                    Block nextBlock = blocks[blockIndex + 1];
                    if (nextBlock.Count + block.Count <= MIN_BLOCK_SIZE)
                    {
                        for (int i = 0; i < nextBlock.Count; i++)
                        {
                            block.elements[block.Count + i] = nextBlock.elements[i];
                        }
                        block.Count += nextBlock.Count;
                        blocks.RemoveAt(blockIndex + 1);
                        blockPool.Return(nextBlock);
                    }
                }
            }

            return true;
        }

        public void Clear()
        {
            foreach (var block in blocks)
            {
                blockPool.Return(block);
            }
            blocks.Clear();
            count = 0;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach (var block in blocks)
            {
                for (int i = 0; i < block.Count; i++)
                {
                    yield return new KeyValuePair<TKey, TValue>(
                        block.elements[i].Key,
                        block.elements[i].Value);
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool FindFirst(out KeyValuePair<TKey, TValue> result)
        {
            if (blocks.Count == 0 || blocks[0].Count == 0)
            {
                result = default;
                return false;
            }

            var firstBlock = blocks[0];
            result = new KeyValuePair<TKey, TValue>(
                firstBlock.elements[0].Key,
                firstBlock.elements[0].Value);
            return true;
        }

        public bool FindLast(out KeyValuePair<TKey, TValue> result)
        {
            if (blocks.Count == 0)
            {
                result = default;
                return false;
            }

            var lastBlock = blocks[^1];
            if (lastBlock.Count == 0)
            {
                result = default;
                return false;
            }

            result = new KeyValuePair<TKey, TValue>(
                lastBlock.elements[lastBlock.Count - 1].Key,
                lastBlock.elements[lastBlock.Count - 1].Value);
            return true;
        }
        
        public static void UnitTest()
        {
            static void TestEmptyArray()
            {
                var array = new ProtaSortedDictoianry<int, string>();
                Debug.Assert(array.Count == 0);
                Debug.Assert(!array.TryGet(1, out _));
            }

            static void TestSingleElement()
            {
                var array = new ProtaSortedDictoianry<int, string>();
                array.Add(1, "one");
                Debug.Assert(array.TryGet(1, out string value));
                Debug.Assert(value == "one");
            }

            static void TestMultipleElementsInOrder()
            {
                var array = new ProtaSortedDictoianry<int, string>();
                for (int i = 0; i < 5; i++)
                {
                    array.Add(i, $"value{i}");
                }

                Debug.Assert(array.Count == 5);
                for (int i = 0; i < 5; i++)
                {
                    Debug.Assert(array.TryGet(i, out string value));
                    Debug.Assert(value == $"value{i}");
                }
            }

            static void TestMultipleElementsReverseOrder()
            {
                var array = new ProtaSortedDictoianry<int, string>();
                for (int i = 4; i >= 0; i--)
                {
                    array.Add(i, $"value{i}");
                }

                Debug.Assert(array.Count == 5);
                for (int i = 0; i < 5; i++)
                {
                    Debug.Assert(array.TryGet(i, out string value));
                    Debug.Assert(value == $"value{i}");
                }
            }

            static void TestBlockSplit()
            {
                var array = new ProtaSortedDictoianry<int, string>();
                for (int i = 0; i < 40; i++)
                {
                    array.Add(i, $"value{i}");
                }

                Debug.Assert(array.Count == 40);
                for (int i = 0; i < 40; i++)
                {
                    Debug.Assert(array.TryGet(i, out string value));
                    Debug.Assert(value == $"value{i}");
                }
            }

            static void TestRandomOrder()
            {
                var array = new ProtaSortedDictoianry<int, string>();
                int[] values = { 5, 2, 8, 1, 9, 3, 7, 4, 6 };
                foreach (int i in values)
                {
                    array.Add(i, $"value{i}");
                }

                Debug.Assert(array.Count == values.Length);
                foreach (int i in values.OrderBy(x => x))
                {
                    Debug.Assert(array.TryGet(i, out string value));
                    Debug.Assert(value == $"value{i}");
                }
            }

            static void TestNonExistentKey()
            {
                var array = new ProtaSortedDictoianry<int, string>();
                array.Add(1, "one");
                array.Add(3, "three");
                
                Debug.Assert(!array.TryGet(2, out _));
            }

            static void TestPairedWithSortedDictionary()
            {
                var a = new ProtaSortedDictoianry<int, string>();
                var b = new SortedDictionary<int, string>();
                var rd = new Random();
                for(int i = 0; i < 100000; i++)
                {
                    var op = rd.Next(1, 100);
                    var type = rd.Next(1, 4);
                    switch(type)
                    {
                        case 1: 
                            a[op] = $"value{op}";
                            b[op] = $"value{op}";
                            break;
                            
                        case 2:
                            a.Remove(op);
                            b.Remove(op);
                            break;
                        
                        case 3:
                            var t2 = rd.Next(0, 3);
                            switch(t2)
                            {
                                case 0:
                                    var successa = a.TryGet(op, out string valueA);
                                    var successb = b.TryGetValue(op, out string valueB);
                                    Debug.Assert(successa == successb, $"Mismatch at {op}: {successa} != {successb}");
                                    if(!successa) break;
                                    Debug.Assert(valueA == valueB, $"Mismatch at {op}: {valueA} != {valueB}");
                                    break;
                                case 1:
                                    successa = a.FindFirst(out var kva);
                                    successb = b.Count != 0;
                                    Debug.Assert(successa == successb, $"Mismatch at {op}: {successa} != {successb}");
                                    if(!successa) break;
                                    var kvb = b.First();
                                    Debug.Assert(kva.Key == kvb.Key, $"Mismatch at {op}: {kva} != {kvb}");
                                    Debug.Assert(kva.Value == kvb.Value, $"Mismatch at {op}: {kva} != {kvb}");
                                    break;
                                case 2:
                                    successa = a.FindLast(out var kva2);
                                    successb = b.Count != 0;
                                    Debug.Assert(successa == successb, $"Mismatch at {op}: {successa} != {successb}");
                                    if(!successa) break;
                                    var kvb2 = b.Last();
                                    Debug.Assert(kva2.Key == kvb2.Key, $"Mismatch at {op}: {kva2} != {kvb2}");
                                    Debug.Assert(kva2.Value == kvb2.Value, $"Mismatch at {op}: {kva2} != {kvb2}");
                                    break;
                                default: break;
                            }
                            break;
                        
                        default: break;
                    }
                }
            }

            TestEmptyArray();
            TestSingleElement();
            TestMultipleElementsInOrder();
            TestMultipleElementsReverseOrder();
            TestBlockSplit();
            TestRandomOrder();
            TestNonExistentKey();
            TestPairedWithSortedDictionary();
        }
    }
}
