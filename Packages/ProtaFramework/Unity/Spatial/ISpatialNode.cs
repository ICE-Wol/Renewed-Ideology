
namespace Prota
{
    // build up a linked list for each grid cell.
    public interface ISpatialNode
    {
        void GetNodeParameters(out SpatialCoord coord);
        void OnSpatialUpdatePrepare();      // 主线程
        void OnSpatialUpdate();             // 副线程
        void OnSpatialUpdateComplete();     // 主线程
    }
}
