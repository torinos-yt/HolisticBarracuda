using UnityEngine;

namespace MediaPipe.Holistic {

//
// Image processing part of the holistic pipeline class
//

partial class HolisticPipeline
{
    void RunPipeline(Texture input, bool face, bool hand)
    {
        var cs_pipeline = _resources.pipeline;
        var cs_poseprocess = _resources.postprocess;

        // Letterboxing scale factor
        var scale = new Vector2
          (Mathf.Max((float)input.height / input.width, 1),
           Mathf.Max(1, (float)input.width / input.height));

        // Image scaling and padding
        cs_pipeline.SetInt("_spad_width", InputWidth);
        cs_pipeline.SetVector("_spad_scale", scale);
        cs_pipeline.SetTexture(0, "_spad_input", input);
        cs_pipeline.SetBuffer(0, "_spad_output", _buffer.input);
        cs_pipeline.Dispatch(0, InputWidth / 8, InputWidth / 8, 1);

        // pose detection
        _detector.pose.ProcessImage(_buffer.input);

        // Pose region bounding box update
        cs_pipeline.SetFloat("_bbox_dt", Time.deltaTime);
        cs_pipeline.SetInt("_UpperBody", _upperbody ? 1 : 0);
        cs_pipeline.SetBuffer(1, "_bbox_count", _detector.pose.CountBuffer);
        cs_pipeline.SetBuffer(1, "_bbox_pose", _detector.pose.DetectionBuffer);
        cs_pipeline.SetBuffer(1, "_bbox_region", _buffer.region);
        cs_pipeline.Dispatch(1, 1, 1, 1);

        // Pose region cropping
        cs_pipeline.SetTexture(2, "_crop_input", input);
        cs_pipeline.SetBuffer(2, "_crop_region", _buffer.region);
        cs_pipeline.SetBuffer(2, "_crop_output", _buffer.crop);
        cs_pipeline.Dispatch(2, CropSize / 8, CropSize / 8, 1);

        // Pose landmark detection
        _detector.landmark.ProcessImage(_buffer.crop);

        // Key point postprocess
        cs_poseprocess.SetFloat("_post_dt", Time.deltaTime);
        cs_poseprocess.SetFloat("_post_scale", scale.y);
        cs_poseprocess.SetBuffer(0, "_post_input", _detector.landmark.OutputBuffer);
        cs_poseprocess.SetBuffer(0, "_post_pose_region", _buffer.region);
        cs_poseprocess.SetBuffer(0, "_post_output", _buffer.filter);
        cs_poseprocess.Dispatch(0, 1, 1, 1);

        // Hand region bounding box update
        cs_pipeline.SetFloat("_handscale", 1 / scale.y);
        cs_pipeline.SetBuffer(3, "_bbox_count", _detector.pose.CountBuffer);
        cs_pipeline.SetBuffer(3, "_filtered_point", _buffer.filter);
        cs_pipeline.SetBuffer(3, "_left_hand_input", _detector.lefthand.OutputBuffer);
        cs_pipeline.SetBuffer(3, "_right_hand_input", _detector.righthand.OutputBuffer);
        cs_pipeline.SetBuffer(3, "_filtered_hand_input", _buffer.handfilter);
        cs_pipeline.SetBuffer(3, "_hand_bbox_region", _buffer.handRegion);
        cs_pipeline.Dispatch(3, 1, 1, 1);

        // Hand region cropping
        cs_pipeline.SetTexture(4, "_hand_crop_input", input);
        cs_pipeline.SetBuffer(4, "_hand_crop_region", _buffer.handRegion);
        cs_pipeline.SetBuffer(4, "_lefthand_crop_output", _buffer.lefthandcrop);
        cs_pipeline.SetBuffer(4, "_righthand_crop_output", _buffer.righthandcrop);
        cs_pipeline.Dispatch(4, CropSize / 8, CropSize / 8, 1);

        // Hand landmark detection
        _detector.lefthand.ProcessImage(_buffer.lefthandcrop);
        _detector.righthand.ProcessImage(_buffer.righthandcrop);

        // Hand key point postprocess
        cs_poseprocess.SetBuffer(1, "_post_left_hand_input", _detector.lefthand.OutputBuffer);
        cs_poseprocess.SetBuffer(1, "_post_right_hand_input", _detector.righthand.OutputBuffer);
        cs_poseprocess.SetBuffer(1, "_post_hand_region", _buffer.handRegion);
        cs_poseprocess.SetBuffer(1, "_post_hand_output", _buffer.handfilter);
        cs_poseprocess.Dispatch(1, 1, 1, 1);

        // Read cache invalidation
        InvalidateReadCache();
    }
}

} // namespace MediaPipe.Holistic
