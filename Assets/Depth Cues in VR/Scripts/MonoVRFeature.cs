using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MonoVRFeature : ScriptableRendererFeature
{
    class MonoVRPass : ScriptableRenderPass
    {
        public Camera camera;

        [System.Obsolete]
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (camera == null || !camera.stereoEnabled)
                return;

            // Linkes Auge
            Matrix4x4 leftView = camera.GetStereoViewMatrix(Camera.StereoscopicEye.Left);
            Matrix4x4 leftProj = camera.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left);

            // Rechtes Auge exakt auf linkes Auge setzen
            camera.SetStereoViewMatrix(Camera.StereoscopicEye.Right, leftView);
            camera.SetStereoProjectionMatrix(Camera.StereoscopicEye.Right, leftProj);
        }
    }

    MonoVRPass pass;

    public override void Create()
    {
        pass = new MonoVRPass();
        // Vor dem Rendern ausführen, damit Matrizen korrekt sind
        pass.renderPassEvent = RenderPassEvent.BeforeRendering;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        pass.camera = renderingData.cameraData.camera;
        renderer.EnqueuePass(pass);
    }
}
