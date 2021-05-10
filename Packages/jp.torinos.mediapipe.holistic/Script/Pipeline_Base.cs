using MediaPipe.PoseDetect;
using MediaPipe.PoseLandmark;
using MediaPipe.FaceLandmark;
using MediaPipe.HandLandmark;
using UnityEngine;

namespace MediaPipe.Holistic {

//
// Basic implementation of the holistic pipeline class
//

public sealed partial class HolisticPipeline : System.IDisposable
{
    const int CropSize = PoseLandmarkDetector.ImageSize;
    const int HandCropSize = HandLandmarkDetector.ImageSize;

    int InputWidth => _detector.pose.ImageSize;

    ResourceSet _resources;

    (PoseDetector pose, PoseLandmarkDetector landmark, FaceLandmarkDetector face,
     HandLandmarkDetector lefthand, HandLandmarkDetector righthand) _detector;

    (ComputeBuffer input, ComputeBuffer crop,
     ComputeBuffer region, ComputeBuffer filter, ComputeBuffer handRegion,
     ComputeBuffer lefthandcrop, ComputeBuffer righthandcrop,
     ComputeBuffer handfilter) _buffer;

     bool _upperbody;

    void AllocateObjects()
    {
        _detector = (new PoseDetector(_resources.pose_resource),
                     new PoseLandmarkDetector(_resources.landmark_resource, _upperbody),
                     new FaceLandmarkDetector(_resources.face_resource),
                     new HandLandmarkDetector(_resources.hand_resource),
                     new HandLandmarkDetector(_resources.hand_resource));

        if(!_upperbody) _resources.postprocess.EnableKeyword("FULL_BODY");
        else            _resources.postprocess.DisableKeyword("FULL_BODY");

        var inputBufferLength = 3 * InputWidth * InputWidth;
        var handInputBufferLength = 3 * HandCropSize * HandCropSize;
        var cropBufferLength = 3 * CropSize * CropSize;
        var regionStructSize = sizeof(float) * 24;
        var filterBufferLength = _detector.landmark.VertexCount * 2;
        var handFilterBufferLength = HandLandmarkDetector.VertexCount * 2 * 2;

        _buffer = (new ComputeBuffer(inputBufferLength, sizeof(float)),
                   new ComputeBuffer(cropBufferLength, sizeof(float)),
                   new ComputeBuffer(1, regionStructSize),
                   new ComputeBuffer(filterBufferLength, sizeof(float) * 4),
                   new ComputeBuffer(2, regionStructSize),
                   new ComputeBuffer(handInputBufferLength, sizeof(float)),
                   new ComputeBuffer(handInputBufferLength, sizeof(float)),
                   new ComputeBuffer(handFilterBufferLength, sizeof(float) * 4));

        _readCache = new Vector4[PoseKeyPointCount];
    }

    void DeallocateObjects()
    {
        _detector.pose.Dispose();
        _detector.landmark.Dispose();
        _detector.face.Dispose();
        _detector.lefthand.Dispose();
        _detector.righthand.Dispose();

        _buffer.input.Dispose();
        _buffer.crop.Dispose();
        _buffer.region.Dispose();
        _buffer.filter.Dispose();
        _buffer.handRegion.Dispose();
        _buffer.lefthandcrop.Dispose();
        _buffer.righthandcrop.Dispose();
        _buffer.handfilter.Dispose();
    }
}

} // namespace MediaPipe.Holistic
