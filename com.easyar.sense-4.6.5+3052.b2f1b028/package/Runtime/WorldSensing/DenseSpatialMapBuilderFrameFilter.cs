//================================================================================================================================
//
//  Copyright (c) 2015-2023 VisionStar Information Technology (Shanghai) Co., Ltd. All Rights Reserved.
//  EasyAR is the registered trademark or trademark of VisionStar Information Technology (Shanghai) Co., Ltd in China
//  and other countries for the augmented reality technology developed by VisionStar Information Technology (Shanghai) Co., Ltd.
//
//================================================================================================================================

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace easyar
{
    /// <summary>
    /// <para xml:lang="en"><see cref="MonoBehaviour"/> which controls <see cref="DenseSpatialMap"/> in the scene, providing a few extensions in the Unity environment. Use <see cref="Builder"/> directly when necessary.</para>
    /// <para xml:lang="zh">在场景中控制<see cref="DenseSpatialMap"/>的<see cref="MonoBehaviour"/>，在Unity环境下提供功能扩展。如有需要可以直接使用<see cref="Builder"/>。</para>
    /// </summary>
    [RequireComponent(typeof(DenseSpatialMapDepthRenderer))]
    public class DenseSpatialMapBuilderFrameFilter : FrameFilter, FrameFilter.IInputFrameSink
    {
        /// <summary>
        /// <para xml:lang="en">EasyAR Sense API. Accessible after Awake if available.</para>
        /// <para xml:lang="zh">EasyAR Sense API，如果功能可以使用，可以在Awake之后访问。</para>
        /// </summary>
        /// <senseapi/>
        public DenseSpatialMap Builder { get; private set; }

        /// <summary>
        /// <para xml:lang="en"><see cref="Material"/> for map mesh render. Mesh transparency is not enabled in URP by now when using default material.</para>
        /// <para xml:lang="zh">用于渲染Map网格的<see cref="Material"/>。在当前版本中，使用URP时默认材质的透明显示未开启。</para>
        /// </summary>
        public Material MapMeshMaterial;

        /// <summary>
        /// <para xml:lang="en">The target maximum update time per frame in milliseconds. The real time used each frame may differ from this value and a minimum amount fo data is ensured to be updated no matter what the value is. No extra time will be used if data does need to update. Decrease this value if the mesh update slows rendering.</para>
        /// <para xml:lang="zh">目标的每帧最长更新时间（毫秒）。实际每帧使用的时间可能与这个数值有所差异，无论数值设置成多少，每帧都会至少更新一部分数据。如果数据不需要更新则不会耗费额外时间。如果网格更新使渲染变慢可以降低这个数值。</para>
        /// </summary>
        public int TargetMaxUpdateTimePerFrame = 10;
        /// <summary>
        /// <para xml:lang="en">Whether to create mesh collider on the mesh created. Set before <see cref="ARSession.Start"/>.</para>
        /// <para xml:lang="zh">是否在生成的mesh上创建mesh collider，可以在<see cref="ARSession.Start"/>之前设置。</para>
        /// </summary>
        public bool EnableMeshCollider;

        private Dictionary<Vector3, DenseSpatialMapBlockController> blocksDict = new Dictionary<Vector3, DenseSpatialMapBlockController>();
        private GameObject mapRoot;
        private bool isStarted;
        [SerializeField, HideInInspector]
        private bool renderMesh = true;
        private Material mapMaterial;
        private DenseSpatialMapDepthRenderer depthRenderer;
        private bool enableMeshCollider;
        private PendingData pendingData;
        private System.Diagnostics.Stopwatch updateTimer = new System.Diagnostics.Stopwatch();

        /// <summary>
        /// <para xml:lang="en">Event when a new mesh block created.</para>
        /// <para xml:lang="zh">新网格块创建的事件。</para>
        /// </summary>
        public event Action<DenseSpatialMapBlockController> MapCreate;
        /// <summary>
        /// <para xml:lang="en">Event when mesh block updates.</para>
        /// <para xml:lang="zh">网格块更新的事件。</para>
        /// </summary>
        public event Action<List<DenseSpatialMapBlockController>> MapUpdate;

        public override int BufferRequirement
        {
            get { return Builder.bufferRequirement(); }
        }

        /// <summary>
        /// <para xml:lang="en">Mesh render on/off.</para>
        /// <para xml:lang="zh">是否渲染网格。</para>
        /// </summary>
        public bool RenderMesh
        {
            get { return renderMesh; }
            set
            {
                renderMesh = value;
                foreach (var block in blocksDict)
                {
                    block.Value.GetComponent<MeshRenderer>().enabled = renderMesh;
                    if (depthRenderer)
                    {
                        depthRenderer.enabled = renderMesh;
                    }
                }
            }
        }

        /// <summary>
        /// <para xml:lang="en">Mesh color.</para>
        /// <para xml:lang="zh">网格颜色。</para>
        /// </summary>
        public Color MeshColor
        {
            get
            {
                if (mapMaterial)
                {
                    return mapMaterial.color;
                }
                return Color.black;
            }
            set
            {
                if (mapMaterial)
                {
                    mapMaterial.color = value;
                }
            }
        }

        /// <summary>
        /// <para xml:lang="en">All mesh blocks.</para>
        /// <para xml:lang="zh">当前所有网格块。</para>
        /// </summary>
        public List<DenseSpatialMapBlockController> MeshBlocks
        {
            get
            {
                var list = new List<DenseSpatialMapBlockController>();
                foreach (var item in blocksDict)
                {
                    list.Add(item.Value);
                }
                return list;
            }
        }

        protected virtual void Awake()
        {
            if (!EasyARController.Initialized)
            {
                return;
            }
            if (!DenseSpatialMap.isAvailable())
            {
                throw new UIPopupException(typeof(DenseSpatialMap) + " not available");
            }
            mapRoot = new GameObject("DenseSpatialMapRoot");
            Builder = DenseSpatialMap.create();
            depthRenderer = GetComponent<DenseSpatialMapDepthRenderer>();
            mapMaterial = Instantiate(MapMeshMaterial);

            if (GraphicsSettings.currentRenderPipeline == null)
            {
                mapMaterial.SetShaderPassEnabled("UniversalForward", false);
                mapMaterial.SetShaderPassEnabled("ForwardBase", true);
            }
#if EASYAR_URP_ENABLE
            else if (GraphicsSettings.currentRenderPipeline is UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset)
            {
                mapMaterial.SetShaderPassEnabled("UniversalForward", true);
                mapMaterial.SetShaderPassEnabled("ForwardBase", false);
                if (mapMaterial.shader == Shader.Find("EasyAR/DenseSpatialMapMesh"))
                {
                    var c = mapMaterial.color;
                    mapMaterial.color = new Color(c.r, c.g, c.b);
                    Debug.LogWarning("EasyAR dense mesh transparency is default disabled in URP");
                }
            }
#endif

            if (depthRenderer && depthRenderer.enabled)
            {
                depthRenderer.MapMeshMaterial = mapMaterial;
            }
        }

        protected virtual void OnEnable()
        {
            if (Builder != null && isStarted)
            {
                Builder.start();
            }
        }

        protected virtual void Update()
        {
            if (pendingData == null)
            {
                if (Builder != null && Builder.updateSceneMesh(false))
                {
                    var sceneMesh = Builder.getMesh();
                    pendingData = new PendingData
                    {
                        Stage = PendingData.UpdateStage.UpdateInfo,
                        SceneMesh = sceneMesh,
                        PendingInfo = sceneMesh.getBlocksInfoIncremental(),
                        PendingController = new List<DenseSpatialMapBlockController>()
                    };
                }
                if (pendingData == null) { return; }
            }

            updateTimer.Restart();
            if (pendingData.Stage == PendingData.UpdateStage.UpdateInfo)
            {
                if (pendingData.PendingInfo != null && pendingData.PendingInfo.Count > 0)
                {
                    var usedInfo = new List<BlockInfo>();
                    try
                    {
                        foreach (var blockInfo in pendingData.PendingInfo)
                        {
                            if (updateTimer.ElapsedMilliseconds > TargetMaxUpdateTimePerFrame && usedInfo.Count >= 3) { break; }

                            usedInfo.Add(blockInfo);
                            DenseSpatialMapBlockController oldBlock;
                            blocksDict.TryGetValue(new Vector3(blockInfo.x, blockInfo.y, blockInfo.z), out oldBlock);

                            if (blockInfo.numOfVertex == 0 || blockInfo.numOfIndex == 0)
                            {
                                if (oldBlock)
                                {
                                    blocksDict.Remove(new Vector3(blockInfo.x, blockInfo.y, blockInfo.z));
                                    Destroy(oldBlock.gameObject);
                                }
                                continue;
                            }

                            if (oldBlock == null)
                            {
                                var go = new GameObject("MeshBlock");
                                go.AddComponent<MeshCollider>();
                                go.AddComponent<MeshFilter>();
                                var renderer = go.AddComponent<MeshRenderer>();
                                renderer.material = mapMaterial;
                                renderer.enabled = RenderMesh;
                                var block = go.AddComponent<DenseSpatialMapBlockController>();
                                block.UpdateData(blockInfo, pendingData.SceneMesh);
                                go.transform.SetParent(mapRoot.transform, false);
                                blocksDict.Add(new Vector3(blockInfo.x, blockInfo.y, blockInfo.z), block);
                                pendingData.PendingController.Add(block);
                                if (MapCreate != null)
                                {
                                    MapCreate(block);
                                }
                            }
                            else if (oldBlock.Info.version != blockInfo.version)
                            {
                                oldBlock.UpdateData(blockInfo, pendingData.SceneMesh);
                                pendingData.PendingController.Add(oldBlock);
                            }
                        }
                    }
                    finally
                    {
                        foreach (var info in usedInfo)
                        {
                            pendingData.PendingInfo.Remove(info);
                        }
                    }
                }

                if (pendingData.PendingInfo != null && pendingData.PendingInfo.Count > 0)
                {
                    return;
                }
                else
                {
                    pendingData.Stage = PendingData.UpdateStage.UpdateMeshFilter;
                }
            }

            if (pendingData.Stage == PendingData.UpdateStage.UpdateMeshFilter)
            {
                if (pendingData.PendingController != null)
                {
                    foreach (var block in pendingData.PendingController)
                    {
                        block.UpdateMeshFilter();
                    }
                }

                if (enableMeshCollider)
                {
                    pendingData.Stage = PendingData.UpdateStage.UpdateMeshCollider;
                    return;
                }
                else
                {
                    if (pendingData.PendingController != null && pendingData.PendingController.Count > 0)
                    {
                        MapUpdate?.Invoke(pendingData.PendingController);
                    }
                    pendingData.Stage = PendingData.UpdateStage.Finish;
                }
            }


            if (pendingData.Stage == PendingData.UpdateStage.UpdateMeshCollider)
            {
                if (pendingData.PendingController != null && pendingData.PendingController.Count > 0)
                {
                    var usedController = new List<DenseSpatialMapBlockController>();
                    try
                    {
                        foreach (var block in pendingData.PendingController)
                        {
                            if (updateTimer.ElapsedMilliseconds > TargetMaxUpdateTimePerFrame && usedController.Count >= 3) { break; }

                            usedController.Add(block);
                            block.UpdateMeshCollider();
                        }
                    }
                    finally
                    {
                        foreach (var info in usedController)
                        {
                            pendingData.PendingController.Remove(info);
                        }

                        MapUpdate?.Invoke(usedController);
                    }
                }

                if (pendingData.PendingController != null && pendingData.PendingController.Count > 0)
                {
                    return;
                }
                else
                {
                    pendingData.Stage = PendingData.UpdateStage.Finish;
                }
            }

            if (pendingData.Stage == PendingData.UpdateStage.Finish)
            {
                pendingData.Dispose();
                pendingData = null;
            }
        }

        protected virtual void OnDisable()
        {
            if (Builder != null)
            {
                Builder.stop();
            }
        }

        protected virtual void OnDestroy()
        {
            if (Builder != null)
            {
                Builder.Dispose();
            }
            if (mapRoot)
            {
                Destroy(mapRoot);
            }
            if (mapMaterial)
            {
                Destroy(mapMaterial);
            }
            pendingData?.Dispose();
        }

        public InputFrameSink InputFrameSink()
        {
            if (Builder != null)
            {
                return Builder.inputFrameSink();
            }
            return null;
        }

        public override void OnAssemble(ARSession session)
        {
            base.OnAssemble(session);
            if (depthRenderer)
            {
                depthRenderer.RenderDepthCamera = session.Assembly.Camera;
            }
            if (session.Assembly != null && session.Assembly.FrameSource is FramePlayer)
            {
                (session.Assembly.FrameSource as FramePlayer).RequireSpatial();
            }
            if (session.Origin)
            {
                mapRoot.transform.SetParent(session.Origin.transform, false);
            }

            if (session.Assembly.FrameSource.IsHMD && mapMaterial && mapMaterial.shader == Shader.Find("EasyAR/DenseSpatialMapMesh"))
            {
                var c = mapMaterial.color;
                mapMaterial.color = new Color(c.r, c.g, c.b);
                mapMaterial.SetInt("_UseDepthTexture", 0);
            }
            enableMeshCollider = EnableMeshCollider;

            isStarted = true;
            if (enabled)
            {
                OnEnable();
            }
        }

        private class PendingData : IDisposable
        {
            public UpdateStage Stage;
            public List<BlockInfo> PendingInfo;
            public List<DenseSpatialMapBlockController> PendingController;
            public SceneMesh SceneMesh;

            public enum UpdateStage
            {
                UpdateInfo,
                UpdateMeshFilter,
                UpdateMeshCollider,
                Finish,
            }

            ~PendingData()
            {
                SceneMesh?.Dispose();
            }

            public void Dispose()
            {
                SceneMesh?.Dispose();
                GC.SuppressFinalize(this);
            }
        }
    }
}
