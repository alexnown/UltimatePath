using UnityEngine;
using Unity.Entities;
using alexnown.SegmentedTween;

namespace alexnown.path
{
    public class PathComponent : MonoBehaviour, IDrawablePath, IConvertGameObjectToEntity
    {
        [SerializeField]
        private bool _isNonUniformPath;
        [SerializeField]
        private NonUniformPath _nonUniformPath;
        [SerializeField]
        private bool _isUniformPath;
        [SerializeField]
        private UniformPath _uniformPath;

        public IPath Path
        {
            get
            {
                if (_isNonUniformPath) return _nonUniformPath;
                else if (_isUniformPath) return _uniformPath;
                return null;
            }
        }

        public Vector3[] Waypoints => Path?.Points;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            if (_isUniformPath)
            {

            }
            else if (_isNonUniformPath)
            {
                var distances = _nonUniformPath.Distances;
                dstManager.AddComponentData(entity, new SegmentedTranslations { Reference = BlobAllocatorHelper.AllocateFloat3Array(_nonUniformPath.Points) });
                dstManager.AddComponentData(entity, new SegmentingBounds { Reference = BlobAllocatorHelper.AllocateFloatArray(distances) });
                dstManager.AddComponentData(entity, new SegmentingParameters { SegmentsCount = distances.Length, TotalLength = distances[distances.Length - 1] });
                dstManager.AddComponentData(entity, new CachedSegmentBounds { End = distances[0] });
            }
            //using (var builder = new BlobBuilder(Allocator.Temp))
            //{
            //    ref var root = ref builder.ConstructRoot<StaticPathData>();
            //    root.IsCyclic = _path.IsCyclic;
            //    var pathPoints = _path.Points;
            //    var points = builder.Allocate(ref root.Points, pathPoints.Length);
            //    for (int i = 0; i < pathPoints.Length; i++) points[i] = pathPoints[i];
            //    var pathDistances = _path.Distances;
            //    var distances = builder.Allocate(ref root.Distances, pathDistances.Length);
            //    for (int i = 0; i < pathDistances.Length; i++) distances[i] = pathDistances[i];
            //    var allocatedData = builder.CreateBlobAssetReference<StaticPathData>(Allocator.Persistent);
            //    dstManager.AddComponentData(entity, new StaticPathReference { Value = allocatedData });
            //}
        }

        public void StorePath(IPath path)
        {
            _isNonUniformPath = path is NonUniformPath;
            _nonUniformPath = path as NonUniformPath;
            _isUniformPath = path is UniformPath;
            _uniformPath = path as UniformPath;
        }
    }
}