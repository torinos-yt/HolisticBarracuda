using UnityEngine;
using UnityEngine.UI;

namespace MediaPipe.Holistic {

public sealed class Visualize : MonoBehaviour
{
    [SerializeField] SourceInput _webcam;
    [Space]
    [SerializeField] ResourceSet _resources;
    [SerializeField] Shader _keyPointShader;
    [SerializeField] Shader _poseRegionShader;
    [SerializeField] Shader _handRegionShader;
    [Space]
    [SerializeField] RawImage _mainUI;
    [SerializeField] RawImage _cropUI;
    [SerializeField] RawImage _cropLeftHandUI;
    [SerializeField] RawImage _cropRightHandUI;
    [Space]
    [SerializeField] bool _upperBodyOnly = true;

    HolisticPipeline _pipeline;
    (Material keys, Material handkeys, Material region,
     Material leftHandRegion, Material rightHandRegion) _material;


    void Start()
    {
        _pipeline = new HolisticPipeline(_resources, _upperBodyOnly);
        _material = (new Material(_keyPointShader),
                     new Material(_keyPointShader),
                     new Material(_poseRegionShader),
                     new Material(_handRegionShader),
                     new Material(_handRegionShader));

        // Material initial setup
        _material.keys.SetBuffer("_KeyPoints", _pipeline.KeyPointBuffer);
        _material.handkeys.SetBuffer("_KeyPoints", _pipeline.HandKeyPointBuffer);
        _material.region.SetBuffer("_Image", _pipeline.PoseRegionCropBuffer);
        _material.leftHandRegion.SetBuffer("_Image", _pipeline.LeftHandRegionCropBuffer);
        _material.rightHandRegion.SetBuffer("_Image", _pipeline.RighthandRegionCropBuffer);

        // UI setup
        _cropUI.material = _material.region;
        _cropLeftHandUI.material = _material.leftHandRegion;
        _cropRightHandUI.material = _material.rightHandRegion;
    }

    void OnDestroy()
    {
        _pipeline.Dispose();
        Destroy(_material.keys);
        Destroy(_material.handkeys);
        Destroy(_material.region);
        Destroy(_material.leftHandRegion);
        Destroy(_material.rightHandRegion);
    }

    void LateUpdate()
    {
        // Feed the input image to the Pose pipeline.
        _pipeline.ProcessImage(_webcam.SourceTexture);

        // UI update
        _mainUI.texture = _webcam.SourceTexture;
        _cropUI.texture = _webcam.SourceTexture;
        _cropLeftHandUI.texture = _webcam.SourceTexture;
        _cropRightHandUI.texture = _webcam.SourceTexture;
    }

    void OnRenderObject()
    {
        // Key point circles
        _material.keys.SetPass(0);
        Graphics.DrawProceduralNow(MeshTopology.Triangles, 96, _pipeline.PoseKeyPointCount);

        _material.handkeys.SetPass(0);
        Graphics.DrawProceduralNow(MeshTopology.Triangles, 96, _pipeline.HandKeyPointCount * 2);

        // Skeleton lines
        // _material.keys.SetPass(1);
        // Graphics.DrawProceduralNow(MeshTopology.Lines, 2, 4 * 5 + 1);
    }
}

} // namespace MediaPipe.BlazePose
