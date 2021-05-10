using UnityEngine;

namespace MediaPipe.Holistic {

//
// Public part of the Holistic pipeline class
//

partial class HolisticPipeline
{
    const int FullKeyPointCount = 33;
    const int UpperKeyPointCount = 25;
    public int PoseKeyPointCount
        => _upperbody ? UpperKeyPointCount : FullKeyPointCount;

    public int HandKeyPointCount = 21;

    public enum PoseKeyPoint
    {
        Nose,
        LeftEye1, LeftEye2, LeftEye3,
        RightEye1, RightEye2, RightEye3,
        LeftEar, RightEar,
        MouseLeft, MouseRight,
        LeftShoulder, RightShoulder,
        LeftElbow, RightElbow,
        LeftWrist, RightWrist,
        LeftPinky, RightPinky,
        LeftIndex, RightIndex,
        LeftThumb, RightTumb,
        LeftHip, RightHip,
        LeftKnee, RightKnee,
        LeftAnkle, RightAnkle,
        LeftHeel, RightHeel,
        LeftFootIndex, RightFootIndex
    }

    public enum HandKeyPoint
    {
        Wrist,
        Thumb1,  Thumb2,  Thumb3,  Thumb4,
        Index1,  Index2,  Index3,  Index4,
        Middle1, Middle2, Middle3, Middle4,
        Ring1,   Ring2,   Ring3,   Ring4,
        Pinky1,  Pinky2,  Pinky3,  Pinky4
    }

    public Vector3 GetKeyPoint(PoseKeyPoint point)
      => ReadCache[(int)point];

    public Vector3 GetKeyPoint(HandKeyPoint hand)
      => ReadCache[(int)(PoseKeyPointCount + hand)];

    public Vector3 GetKeyPoint(int index)
      => ReadCache[index];

    public ComputeBuffer InputBuffer
      => _buffer.input;

    public ComputeBuffer KeyPointBuffer
      => _buffer.filter;

    public ComputeBuffer HandKeyPointBuffer
      => _buffer.handfilter;

    public ComputeBuffer PoseRegionBuffer
      => _buffer.region;

    public ComputeBuffer HandRegionBuffer
      => _buffer.handRegion;

    public ComputeBuffer PoseRegionCropBuffer
      => _buffer.crop;

    public ComputeBuffer LeftHandRegionCropBuffer
      => _buffer.lefthandcrop;

    public ComputeBuffer RighthandRegionCropBuffer
      => _buffer.righthandcrop;

    public RenderTexture SegmentationBuffer
      => _detector.landmark.SegmentationBuffer;

    public ComputeBuffer PoseBuffer
      => _detector.pose.DetectionBuffer;

    public bool UseAsyncReadback { get; set; } = true;

    public HolisticPipeline(ResourceSet resources, bool upperbody)
    {
      _resources = resources;
      _upperbody = upperbody;
      ReadbackBytes = (PoseKeyPointCount) * sizeof(float) * 4;

      AllocateObjects();
    }

    public void Dispose()
      => DeallocateObjects();

    public void ProcessImage(Texture image, bool face = true, bool hand = true)
      => RunPipeline(image, face, hand);

}

} // namespace MediaPipe.Holistic
