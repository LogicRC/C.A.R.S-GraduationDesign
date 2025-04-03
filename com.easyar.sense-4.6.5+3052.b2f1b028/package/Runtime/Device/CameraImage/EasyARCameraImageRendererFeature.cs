//================================================================================================================================
//
//  Copyright (c) 2015-2023 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
//  EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
//  and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
//
//================================================================================================================================

using UnityEngine;
using UnityEngine.Rendering;
#if EASYAR_URP_ENABLE
using UnityEngine.Rendering.Universal;
#if EASYAR_URP_17_OR_NEWER
using UnityEngine.Rendering.RenderGraphModule;
#endif
#elif EASYAR_LWRP_ENABLE
using UnityEngine.Rendering.LWRP;
#else
using ScriptableRendererFeature = UnityEngine.ScriptableObject;
#endif

namespace easyar
{
    /// <summary>
    /// <para xml:lang="en">A render feature for rendering the camera image for AR devies when URP in used. Add this render feature to the renderer feature list in forward renderer asset.</para>
    /// <para xml:lang="zh">使用URP时用来渲染AR设备相机图像的render feature。需要在forward renderer asset的renderer feature 列表中添加这个render feature。</para>
    /// </summary>
    public class EasyARCameraImageRendererFeature : ScriptableRendererFeature
    {
#if EASYAR_URP_ENABLE || EASYAR_LWRP_ENABLE
#if EASYAR_URP_17_OR_NEWER
        CameraImageRenderGraphPass renderPass;
        CameraImageRenderGraphPass renderPassUser;
#else
        CameraImageRenderPass renderPass;
        CameraImageRenderPass renderPassUser;
#endif
#if EASYAR_URP_13_1_OR_NEWER
        Optional<RTHandleSystem> rtHandleSystem;
#endif

        public override void Create()
        {
#if EASYAR_URP_17_OR_NEWER
            renderPass = new CameraImageRenderGraphPass();
            renderPassUser = new CameraImageRenderGraphPass(true);
#else
            renderPass = new CameraImageRenderPass();
            renderPassUser = new CameraImageRenderPass();
#endif
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            Camera camera = renderingData.cameraData.camera;
            if (!camera) { return; }

            var imageRenderer = CameraImageRenderer.TryGetRenderer(camera);
            if (!imageRenderer || !imageRenderer.Material) { return; }

            if (imageRenderer.enabled)
            {
                renderPass.Setup(imageRenderer.Material, imageRenderer.InvertCulling);
#if EASYAR_URP_17_OR_NEWER
                renderPass.SetupCameraImageRenderer(imageRenderer);
#endif
                renderer.EnqueuePass(renderPass);
            }
            if (imageRenderer.UserTexture.OnSome)
            {
#if EASYAR_URP_13_1_OR_NEWER
                if (rtHandleSystem.OnNone)
                {
                    rtHandleSystem = new RTHandleSystem();
                    rtHandleSystem.Value.Initialize(Screen.width, Screen.height);
                }
                renderPassUser.SetupRTHandleSystem(rtHandleSystem.Value);
#endif
                renderPassUser.Setup(imageRenderer.Material, imageRenderer.InvertCulling);
                renderPassUser.SetupTarget(imageRenderer.UserTexture.Value);
#if EASYAR_URP_17_OR_NEWER
                renderPassUser.SetupCameraImageRenderer(imageRenderer);
#endif
                renderer.EnqueuePass(renderPassUser);
            }
        }

#if EASYAR_URP_13_1_OR_NEWER
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                if (rtHandleSystem.OnSome)
                {
                    rtHandleSystem.Value.Dispose();
                    rtHandleSystem = null;
                }
            }
        }
#endif

        class CameraImageRenderPass : ScriptableRenderPass
        {
            static readonly Matrix4x4 projection = Matrix4x4.Ortho(0f, 1f, 0f, 1f, -0.1f, 9.9f);
            readonly Mesh mesh;
            Material material;
            bool invertCulling;
#if EASYAR_URP_13_1_OR_NEWER
            Optional<RTHandle> colorTarget;
            Optional<RTHandleSystem> rtHandleSystem;
#else
            Optional<RenderTargetIdentifier> colorTarget;
#endif

            public CameraImageRenderPass()
            {
                renderPassEvent = RenderPassEvent.BeforeRenderingOpaques;
                mesh = new Mesh
                {
                    vertices = new Vector3[]
                    {
                        new Vector3(0f, 0f, 0.1f),
                        new Vector3(0f, 1f, 0.1f),
                        new Vector3(1f, 1f, 0.1f),
                        new Vector3(1f, 0f, 0.1f),
                    },
                    uv = new Vector2[]
                    {
                        new Vector2(0f, 0f),
                        new Vector2(0f, 1f),
                        new Vector2(1f, 1f),
                        new Vector2(1f, 0f),
                    },
                    triangles = new int[] { 0, 1, 2, 0, 2, 3 }
                };
            }

            public void Setup(Material mat, bool iCulling)
            {
                material = mat;
                invertCulling = iCulling;
            }

#if EASYAR_URP_13_1_OR_NEWER
            public void SetupRTHandleSystem(RTHandleSystem system)=> rtHandleSystem = system;
            public void SetupTarget(RenderTexture color) => colorTarget = rtHandleSystem.Value.Alloc(color);
#else
            public void SetupTarget(RenderTexture color) => colorTarget = (RenderTargetIdentifier)color;
#endif

            public override void Configure(CommandBuffer commandBuffer, RenderTextureDescriptor renderTextureDescriptor)
            {
                if (colorTarget.OnSome)
                {
                    ConfigureTarget(colorTarget.Value);
                }
                ConfigureClear(ClearFlag.Depth, Color.clear);
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
#if EASYAR_URP_13_1_OR_NEWER
                if (rtHandleSystem.OnSome)
                {
                    rtHandleSystem.Value.SetReferenceSize(Screen.width, Screen.height);
                }
#endif
                var cmd = CommandBufferPool.Get();
                cmd.SetInvertCulling(invertCulling);
                cmd.SetViewProjectionMatrices(Matrix4x4.identity, projection);
                cmd.DrawMesh(mesh, Matrix4x4.identity, material);
                cmd.SetViewProjectionMatrices(renderingData.cameraData.camera.worldToCameraMatrix, renderingData.cameraData.camera.projectionMatrix);
                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }
        }
#endif

#if EASYAR_URP_17_OR_NEWER
        class CameraImageRenderGraphPass : ScriptableRenderPass
        {
            static readonly Matrix4x4 projection = Matrix4x4.Ortho(0f, 1f, 0f, 1f, -0.1f, 9.9f);
            readonly Mesh mesh;
            Material material;
            bool invertCulling;
            bool textureTargetFlag = false;
            Optional<RTHandle> colorTarget;
            Optional<RTHandleSystem> rtHandleSystem;

            class PassData
            {
                internal Matrix4x4 worldToCameraMatrix;
                internal Matrix4x4 projectionMatrix;
                internal bool invertCulling;
                internal Mesh mesh;
                internal Material material;
            }
            PassData renderPassData = new PassData();
            const string kRenderGraphDisablePassName = "EasyAR Camera Image Render Pass (Render Graph Disabled)";
            const string kRenderGraphEnablePassName = "EasyAR Camera Image Render Pass (Render Graph Enabled)";
            const string kRenderGraphEnableTexturePassName = "EasyAR Camera Image Texture Render Pass (Render Graph Enabled)";
            CameraImageRenderer imageRenderer;
            public CameraImageRenderGraphPass(bool toTextureTarget = false)
            {
                renderPassEvent = RenderPassEvent.BeforeRenderingOpaques;
                bool workaround = !EasyARSettings.Instance || EasyARSettings.Instance.WorkaroundForUnity.URP17RG_DX11_RuinedScene;
                if (workaround && SystemInfo.graphicsDeviceType == GraphicsDeviceType.Direct3D11)
                {
                    renderPassEvent = RenderPassEvent.BeforeRendering;
                }
                mesh = new Mesh
                {
                    vertices = new Vector3[]
                    {
                        new Vector3(0f, 0f, 0.1f),
                        new Vector3(0f, 1f, 0.1f),
                        new Vector3(1f, 1f, 0.1f),
                        new Vector3(1f, 0f, 0.1f),
                    },
                    uv = new Vector2[]
                    {
                        new Vector2(0f, 0f),
                        new Vector2(0f, 1f),
                        new Vector2(1f, 1f),
                        new Vector2(1f, 0f),
                    },
                    triangles = new int[] { 0, 1, 2, 0, 2, 3 }
                };
                textureTargetFlag = toTextureTarget;
            }
            //For RenderGraph Compability Mode Only.
            public void Setup(Material mat, bool iCulling)
            {
                material = mat;
                invertCulling = iCulling;
            }
            //For RenderGraph Compability Mode Only.
            public void SetupTarget(RenderTexture color) => colorTarget = rtHandleSystem.Value.Alloc(color);
            public void SetupRTHandleSystem(RTHandleSystem system)=> rtHandleSystem = system;
            public void SetupCameraImageRenderer(CameraImageRenderer renderer) => imageRenderer = renderer;
            //For RenderGraph Compability Mode Only.
            public override void Configure(CommandBuffer commandBuffer, RenderTextureDescriptor renderTextureDescriptor)
            {
                if (colorTarget.OnSome)
                {
                    ConfigureTarget(colorTarget.Value);
                }
                ConfigureClear(ClearFlag.Depth, Color.clear);
            }
            //For RenderGraph Compability Mode Only.
            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                if (rtHandleSystem.OnSome)
                {
                    rtHandleSystem.Value.SetReferenceSize(Screen.width, Screen.height);
                }
                renderPassData.worldToCameraMatrix = renderingData.cameraData.camera.worldToCameraMatrix;
                renderPassData.projectionMatrix = renderingData.cameraData.camera.projectionMatrix;
                renderPassData.invertCulling = invertCulling;
                renderPassData.mesh = mesh;
                renderPassData.material = material;
                var cmd = CommandBufferPool.Get(kRenderGraphDisablePassName);
                ExecuteRenderPass(CommandBufferHelpers.GetRasterCommandBuffer(cmd), renderPassData);
                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }
            static void ExecuteRenderPass(RasterCommandBuffer rasterCommandBuffer, PassData passData)
            {
                rasterCommandBuffer.SetInvertCulling(passData.invertCulling);
                rasterCommandBuffer.SetViewProjectionMatrices(Matrix4x4.identity, projection);
                rasterCommandBuffer.DrawMesh(
                    passData.mesh,
                    Matrix4x4.identity,
                    passData.material);
                rasterCommandBuffer.SetViewProjectionMatrices(
                    passData.worldToCameraMatrix,
                    passData.projectionMatrix);
            }

            static void ExecuteRasterRenderGraphPass(PassData passData, RasterGraphContext rasterContext)
            {
                ExecuteRenderPass(rasterContext.cmd, passData);
            }

            public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
            {
                string renderGraphName = textureTargetFlag ? kRenderGraphEnableTexturePassName : kRenderGraphEnablePassName;
                using (var builder = renderGraph.AddRasterRenderPass<PassData>(
                    renderGraphName,
                    out renderPassData,
                    profilingSampler))
                {
                    UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
                    UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();

                    renderPassData.worldToCameraMatrix = cameraData.camera.worldToCameraMatrix;
                    renderPassData.projectionMatrix = cameraData.camera.projectionMatrix;
                    renderPassData.invertCulling = imageRenderer.InvertCulling;
                    renderPassData.mesh = mesh;
                    renderPassData.material = imageRenderer.Material;

                    builder.AllowGlobalStateModification(true);
                    builder.AllowPassCulling(false);

                    if (textureTargetFlag)
                    {
                        if (imageRenderer.UserTexture.OnSome)
                        {
                            RTHandle destinationRtHandle = rtHandleSystem.Value.Alloc(imageRenderer.UserTexture.Value);
                            TextureHandle destinationTextureHandle = renderGraph.ImportTexture(destinationRtHandle);
                            builder.SetRenderAttachment(destinationTextureHandle, 0);
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        bool workaround = !EasyARSettings.Instance || EasyARSettings.Instance.WorkaroundForUnity.URP17RG_DX11_RuinedScene;
                        if (!(workaround && SystemInfo.graphicsDeviceType == GraphicsDeviceType.Direct3D11))
                        {
                            builder.SetRenderAttachmentDepth(resourceData.activeDepthTexture, 0);
                        }
                        builder.SetRenderAttachment(resourceData.activeColorTexture, 0);
                    }
                    builder.SetRenderFunc<PassData>(ExecuteRasterRenderGraphPass);
                }
            }
        }
#endif
    }
}
