using System;
using System.Collections;
using System.Collections.Generic;
using Prota.Unity;
using UnityEngine;

namespace Prota
{
    public class SpatialGrid
    {
        public const int COARSE_GRID_SIZE = 4; // Each coarse grid is 4x4 fine grids
        
        public HashMapSet<SpatialCoord, ISpatialNode> fineGridCells = new();
        public HashMapSet<SpatialCoord, ISpatialNode> coarseGridCells = new();
        
        private SpatialCoord ToCoarseCoord(SpatialCoord fineCoord)
        {
            return new SpatialCoord(
                Mathf.FloorToInt(1.0f * fineCoord.x / COARSE_GRID_SIZE),
                Mathf.FloorToInt(1.0f * fineCoord.y / COARSE_GRID_SIZE)
            );
        }
        
        public void Add(SpatialCoord coord, ISpatialNode node)
        {
            fineGridCells.AddElement(coord, node);
            coarseGridCells.AddElement(ToCoarseCoord(coord), node);
        }
        
        public void Remove(SpatialCoord coord, ISpatialNode node)
        {
            fineGridCells.RemoveElement(coord, node);
            coarseGridCells.RemoveElement(ToCoarseCoord(coord), node);
        }
        
        public void Move(ISpatialNode node, SpatialCoord oldCoord, SpatialCoord newCoord)
        {
            Debug.Assert(oldCoord != newCoord);
            Debug.Assert(fineGridCells.ContainsElement(oldCoord, node));
            
            var oldCoarse = ToCoarseCoord(oldCoord);
            var newCoarse = ToCoarseCoord(newCoord);
            
            fineGridCells.RemoveElement(oldCoord, node);
            fineGridCells.AddElement(newCoord, node);
            
            if (oldCoarse != newCoarse)
            {
                coarseGridCells.RemoveElement(oldCoarse, node);
                coarseGridCells.AddElement(newCoarse, node);
            }
            
            node.OnSpatialUpdate();
        }
        
        public void Query(SpatialCoord min, SpatialCoord max, ISpatialQuery query)
        {
            var coarseMin = ToCoarseCoord(min);
            var coarseMax = ToCoarseCoord(max);
            
            // First check coarse grid
            for (int cx = coarseMin.x; cx <= coarseMax.x; cx++)
            for (int cy = coarseMin.y; cy <= coarseMax.y; cy++)
            {
                var coarseCoord = new SpatialCoord(cx, cy);
                if (!coarseGridCells.TryGetValue(coarseCoord, out _)) continue;
                
                // If coarse cell has any objects, check fine cells within it
                int startX = Math.Max(min.x, cx * COARSE_GRID_SIZE);
                int startY = Math.Max(min.y, cy * COARSE_GRID_SIZE);
                int endX = Math.Min(max.x, (cx + 1) * COARSE_GRID_SIZE - 1);
                int endY = Math.Min(max.y, (cy + 1) * COARSE_GRID_SIZE - 1);
                
                for (int x = startX; x <= endX; x++)
                for (int y = startY; y <= endY; y++)
                {
                    var coord = new SpatialCoord(x, y);
                    if(!fineGridCells.TryGetValue(coord, out var nodes)) continue;
                    foreach (var node in nodes)
                    {
                        query.OnSpatialQueryFound(node);
                    }
                }
            }
        }
    }
}
