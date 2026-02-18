using System;
using System.Collections;
using System.Collections.Generic;
using Prota.Unity;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Profiling;

namespace Prota
{
    
    public class SpatialWorld
    {
        // 这个最好在 InitializeOnLoad 的时候初始化.
        static SpatialWorld _instance;
        public static SpatialWorld instance
        {
            get => _instance;
            set
            {
                _instance = value;
                context = new QueryContext(1024, value);
            }
        }
        static QueryContext context;
        
        
        public readonly SpatialGrid[] grids;
        public readonly int[] validLayers;
        public Dictionary<ISpatialNode, SpatialCoord> coords = new();
        
        public readonly float cellSize;
        
        public SpatialWorld(float cellSize, int allLayerMask)
        {
            this.cellSize = cellSize;
            var g = new List<SpatialGrid>();
            var l = new List<int>();
            for(int i = 0; i < 32; i++)
            {
                g.Add(default);
                if((allLayerMask & (1 << i)) == 0) continue;
                g[i] = new SpatialGrid();
                l.Add(i);
            }
            grids = g.ToArray();
            validLayers = l.ToArray();
        }
        
        public SpatialCoord GetCoord(Vector2 position)
            => new SpatialCoord(
                (position.x / cellSize).FloorToInt(),
                (position.y / cellSize).FloorToInt()
            );
        
        public Vector2 CoordCenter(SpatialCoord coord)
            => new Vector2(
                coord.x * cellSize + cellSize / 2,
                coord.y * cellSize + cellSize / 2
            );
        
        public Vector2 CoordMin(SpatialCoord coord)
            => new Vector2(
                coord.x * cellSize,
                coord.y * cellSize
            );
        
        public Vector2 CoordMax(SpatialCoord coord)
            => new Vector2(
                coord.x * cellSize + cellSize,
                coord.y * cellSize + cellSize
            );
        
        public Rect CoordRect(SpatialCoord coord)
            => new Rect(
                CoordMin(coord),
                new Vector2(cellSize, cellSize)
            );
        
        // ====================================================================================================
        // ====================================================================================================
        
        #region API
        
        struct QueryEntry
        {
            public ISpatialQuery query;
            public int layerMask;
        }
        
        struct NodeEntry
        {
            public ISpatialNode node;
            public int layerMask;
        }
        
        readonly DenseSet<ISpatialNode, NodeEntry> nodeEntries
            = new(1024, entry => default);
        
        readonly DenseSet<ISpatialQuery, QueryEntry> queryEntries
            = new(1024, entry => default);
        
        public int pendingQueries => queryEntries.Count;
        
        public bool waitingForCompletion { get; private set; }
        
        public void Add(ISpatialNode node, int layerMask)
        {
            if (nodeEntries.ContainsKey(node))
            {
                Debug.LogError("Node already added to spatial world");
                return;
            }
            
            nodeEntries.Add(node, new NodeEntry
			{
                node = node,
                layerMask = layerMask,
			});
            InsertNode(node, layerMask);
        }
        
        public void Remove(ISpatialNode node)
        {
            nodeEntries.RemoveByKey(node);
            RemoveNode(node);
        }
        
        public void Add(ISpatialQuery query, int layerMask)
        {
            if (queryEntries.ContainsKey(query))
            {
                Debug.LogError("Query already added to spatial world");
                return;
            }
            
            queryEntries.Add(query, new QueryEntry {
                query = query,
                layerMask = layerMask,
			});
        }
        
        public void Remove(ISpatialQuery query)
        {
            queryEntries.RemoveByKey(query);
        }
        
        
        
        public void StartQuery() => StartQueryInternal();
        
        public void CompleteQuery() => CompleteQueryInternal();
        
        #endregion
        
        // ====================================================================================================
        // ====================================================================================================
        
        #region insert/remove management
        
        void InsertNode(ISpatialNode node, int layerMask)
        {
            node.GetNodeParameters(out var coord);
            coords[node] = coord;
            foreach(var v in validLayers)
            {
                if((layerMask & (1 << v)) == 0) continue;
                grids[v].Add(coord, node);
            }
        }
        
        void RemoveNode(ISpatialNode node)
        {
            var coord = coords[node];
            foreach(var v in validLayers)
            {
                grids[v].Remove(coord, node);
            }
            coords.Remove(node);
        }
        
        #endregion
        
        // ====================================================================================================
        // ====================================================================================================
        #region query operation
        
        struct QueryContext
        {
            public readonly SpatialWorld world;
            public readonly List<ISpatialNode> nodesToBeUpdated;
            public readonly List<int> nodesToBeUpdatedLayers;
            public readonly List<SpatialCoord> nodesNewCoords;
            public readonly List<SpatialCoord> queryMins;
            public readonly List<SpatialCoord> queryMaxs;
            public SpatialGrid[] grids;
            public int[] validLayers;
            public JobHandle? job;
            public QueryContext(int capacity, SpatialWorld world)
            {
                this.world = world;
                nodesToBeUpdated = new List<ISpatialNode>(capacity);
                nodesToBeUpdatedLayers = new List<int>(capacity);
                nodesNewCoords = new List<SpatialCoord>(capacity);
                queryMins = new List<SpatialCoord>(capacity);
                queryMaxs = new List<SpatialCoord>(capacity);
                this.grids = null;
                this.validLayers = null;
                job = null;
            }
            public void Clear()
            {
                job?.Complete();
                job = null;
                nodesToBeUpdated.Clear();
                nodesToBeUpdatedLayers.Clear();
                nodesNewCoords.Clear();
                queryMins.Clear();
                queryMaxs.Clear();
                grids = null;
                validLayers = null;
            }
        }
        
        #region MoveNodeJob
        // 每一个grid分配一个job. 每个grid互不干扰.
        struct MoveNodeJob : IJobParallelFor
        {
            public void Execute(int index)
            {
                var grid = context.grids[context.validLayers[index]];
                for(int i = 0; i < context.nodesToBeUpdated.Count; i++)
                {
                    var layer = context.nodesToBeUpdatedLayers[i];
                    if((layer & (1 << index)) == 0) continue;
                    
                    var node = context.nodesToBeUpdated[i];
                    var oldCoord = context.world.coords[node];      // 这里只读不写.
                    var newCoord = context.nodesNewCoords[i];
                    if(oldCoord == newCoord) continue;
                    grid.Move(node, oldCoord, newCoord);
                }
            }
        }
        #endregion
        
        #region QueryJob
        // 每一个query分配一个job. 会去遍历它所属的grid.
        struct QueryJob : IJobParallelFor
        {
            public void Execute(int index)
            {
                var queryEntry = context.world.queryEntries[index];
                var layerMask = queryEntry.layerMask;
                var query = queryEntry.query;
                var min = context.queryMins[index];
                var max = context.queryMaxs[index];
                query.OnSpatialQueryStart();
                foreach(var v in context.validLayers)
                {
                    if((layerMask & (1 << v)) == 0) continue;
                    context.grids[v].Query(min, max, query);
                }
                query.OnSpatialQueryFinish();
            }
        }
        #endregion
        
        #region Start/Complete Query
        void StartQueryInternal()
        {
            context.Clear();
            context.grids = this.grids;
            context.validLayers = this.validLayers;
            
            Profiler.BeginSample("SpatialWorld: filter nodes");
            // 筛选 node, 复制数据.
            for(int i = 0; i < nodeEntries.Count; i++)
            {
                var e = nodeEntries[i];
                if(e.node == null) continue;
                e.node.GetNodeParameters(out var newCoord);
                context.nodesToBeUpdated.Add(e.node);
                context.nodesToBeUpdatedLayers.Add(e.layerMask);
                context.nodesNewCoords.Add(newCoord);
            }
            Profiler.EndSample();
            
            // node更新准备.
            Profiler.BeginSample("SpatialWorld: prepare nodes");
            foreach(var node in context.nodesToBeUpdated)
            {
                node.OnSpatialUpdatePrepare();
            }
            Profiler.EndSample();
            
            Debug.Assert(context.nodesToBeUpdated.Count == context.nodesNewCoords.Count);
            
            Profiler.BeginSample("SpatialWorld: filter queries");
            // 筛选 query, 复制数据.
            for(int i = 0; i < queryEntries.Count; i++)
            {
                var e = queryEntries[i];
                e.query.GetQueryParameters(out var valid, out var min, out var max);
                if(!valid)
                {
                    queryEntries.RemoveByKey(e.query);  // remove by swap.
                    i--;
                    continue;
                }
                context.queryMins.Add(min);
                context.queryMaxs.Add(max);
            }
            Profiler.EndSample();
            
            Profiler.BeginSample("SpatialWorld: prepare queries");
            foreach(var (query, _) in context.world.queryEntries)
            {
                query.OnSpatialQueryPrepare();
            }
            Profiler.EndSample();
            
            // 并行: 更新node移动.
            var moveNodeJob = new MoveNodeJob()
                .Schedule(context.validLayers.Length, 1);
            
            // 并行: 更新查询.
            var queryJob = new QueryJob()
                .Schedule(context.world.queryEntries.Count, 1, moveNodeJob);
            
            context.job = queryJob;
            
            // 单线程查询测试.
            // var job = new MoveNodeJob();
            // for(int i = 0; i < context.validLayers.Length; i++) job.Execute(i);
            // var queryJob = new QueryJob();
            // for(int i = 0; i < context.queriesToBeUpdated.Count; i++) queryJob.Execute(i);
            // context.job = new JobHandle();
        }
        
        void CompleteQueryInternal()
        {
            // 还没有请求查询.
            if(context.job == null) return;
            
            context.job.Value.Complete();
            context.job = null;
            
            // 把节点更新到 new coord.
            for(int i = 0; i < context.nodesToBeUpdated.Count; i++)
            {
                var node = context.nodesToBeUpdated[i];
                coords[node] = context.nodesNewCoords[i];
            }
            
            // 完成查询. 这里要缓存是因为在回调中可能有新的查询加进来, 所以要先清理查询队列再执行回调.
            using var _ = context.world.queryEntries.ToTempList(out var queries);
            context.world.queryEntries.Clear();
            
            // 执行回调.
            foreach(var (query, _) in queries)
            {
                query.OnSpatialQueryComplete();
            }
            
            context.Clear();
        }
        
        #endregion
        
        #endregion
    }
}
