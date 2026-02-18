
namespace Prota
{
    public interface ISpatialQuery
    {
        void GetQueryParameters(out bool valid, out SpatialCoord min, out SpatialCoord max);
        
        // 主线程
        void OnSpatialQueryPrepare();
        
        // 副线程, 在查询开始前执行.
        void OnSpatialQueryStart();
        // 副线程, 在找到一个对应的查询对象后执行.
        void OnSpatialQueryFound(ISpatialNode node);
        // 副线程, 在找到所有对应的查询对象后执行.
        void OnSpatialQueryFinish();
        
        // 主线程
        void OnSpatialQueryComplete();
    }
}
