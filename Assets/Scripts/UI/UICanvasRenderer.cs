using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public class UICanvasRenderer : ScriptableRendererFeature
{
    public class UICanvasPass : ScriptableRenderPass, IDisposable
    {
        UICanvasSettings settings;
        Mesh mesh = new Mesh();

        public UICanvasPass(UICanvasSettings settings)
        {
            this.settings = settings;
            this.renderPassEvent = settings.renderPassEvent; 
        }

        public class PassData
        {
            public Material material;
            public Mesh mesh;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {

            using (var builder = renderGraph.AddRasterRenderPass<PassData>("Canvas", out var passData))
            {
                if (!UIManager.instance.canvas)
                    return;
                    
                UIManager.instance.canvas.Refresh();
                passData.mesh = UIManager.instance.canvas.GetMesh();
                passData.material = settings.material;
                if (!passData.material)
                    return;

                var resourceData = frameData.Get<UniversalResourceData>();
                builder.SetRenderAttachment(resourceData.activeColorTexture, 0);
                builder.SetRenderFunc((PassData data, RasterGraphContext ctx) =>
                {
                    var cmd = ctx.cmd;
                    Vector2 min = new Vector2(0, 0);
                    Vector2 max = new Vector2(Screen.width, Screen.height);

                    Matrix4x4 projection;
                    if (SystemInfo.graphicsUVStartsAtTop)
                        projection = Matrix4x4.Ortho(min.x, max.x, min.y, max.y, -1, 1);
                    else
                        projection = Matrix4x4.Ortho(min.x, max.x, max.y, min.y, -1, 1);
                    
                    cmd.SetViewProjectionMatrices(Matrix4x4.identity, projection);
                    cmd.DrawMesh(passData.mesh, Matrix4x4.identity, data.material);
                });
            }
        }

        public void Dispose()
        {
            settings = null;
        }
    }

    [Serializable]
    public class UICanvasSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
        public Material material;
    }

    public UICanvasSettings settings = new UICanvasSettings();
    UICanvasPass canvasPass;

    public override void Create()
    {
        canvasPass = new UICanvasPass(settings);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.camera.cameraType == CameraType.Game)
            renderer.EnqueuePass(canvasPass);
    }
}
