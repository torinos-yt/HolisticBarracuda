using UnityEngine;
using UnityEngine.Rendering;

namespace MediaPipe.Holistic {

//
// GPU to CPU readback implementation of the pose pipeline class
//

sealed partial class HolisticPipeline
{

    Vector4[] _readCache;
    bool _readFlag;

    Vector4[] ReadCache
      => (_readFlag || UseAsyncReadback) ? _readCache : UpdateReadCache();

    Vector4[] UpdateReadCache()
    {
        _buffer.filter.GetData(_readCache, 0, 0, PoseKeyPointCount);
        _buffer.handfilter.GetData(_readCache, PoseKeyPointCount, 0, HandKeyPointCount*2);
        _readFlag = true;
        return _readCache;
    }

    void InvalidateReadCache()
    {
        if (UseAsyncReadback)
            AsyncGPUReadback.Request
              (_buffer.filter, ReadbackBytes, 0, ReadbackCompleteAction);
        else
            _readFlag = false;
    }

    readonly int ReadbackBytes;

    System.Action<AsyncGPUReadbackRequest> ReadbackCompleteAction
      => OnReadbackComplete;

    void OnReadbackComplete(AsyncGPUReadbackRequest req)
      => req.GetData<Vector4>().CopyTo(_readCache);
}

} // namespace MediaPipe.BlazePose
