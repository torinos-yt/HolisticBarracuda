using UnityEngine;

namespace MediaPipe.Holistic {

[CreateAssetMenu(fileName = "Holistic",
                 menuName = "ScriptableObjects/MediaPipe/Holistic Resource Set")]
public sealed class ResourceSet : ScriptableObject
{
    public PoseDetect.ResourceSet pose_resource;
    public PoseLandmark.ResourceSet landmark_resource;
    public HandLandmark.ResourceSet hand_resource;
    public FaceLandmark.ResourceSet face_resource;
    public ComputeShader pipeline;
    public ComputeShader postprocess;
}

}
