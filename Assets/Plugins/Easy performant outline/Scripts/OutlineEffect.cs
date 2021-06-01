﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;

namespace EPOOutline
{
    public static class OutlineEffect
    {
        public static readonly int FillRefHash = Shader.PropertyToID("_FillRef");
        public static readonly int DilateShiftHash = Shader.PropertyToID("_DilateShift");
        public static readonly int ColorMaskHash = Shader.PropertyToID("_ColorMask");
        public static readonly int OutlineRefHash = Shader.PropertyToID("_OutlineRef");
        public static readonly int RefHash = Shader.PropertyToID("_Ref");
        public static readonly int ZWriteHash = Shader.PropertyToID("_ZWrite");
        public static readonly int EffectSizeHash = Shader.PropertyToID("_EffectSize");
        public static readonly int CullHash = Shader.PropertyToID("_Cull");
        public static readonly int ZTestHash = Shader.PropertyToID("_ZTest");
        public static readonly int ColorHash = Shader.PropertyToID("_EPOColor");
        public static readonly int ScaleHash = Shader.PropertyToID("_Scale");
        public static readonly int ShiftHash = Shader.PropertyToID("_Shift");
        public static readonly int InitialTexHash = Shader.PropertyToID("_InitialTex");
        public static readonly int InfoBufferHash = Shader.PropertyToID("_InfoBuffer");
        public static readonly int ComparisonHash = Shader.PropertyToID("_Comparison");
        public static readonly int ReadMaskHash = Shader.PropertyToID("_ReadMask");
        public static readonly int WriteMaskHash = Shader.PropertyToID("_WriteMask");
        public static readonly int OperationHash = Shader.PropertyToID("_Operation");
        public static readonly int CutoutThresholdHash = Shader.PropertyToID("_CutoutThreshold");
        public static readonly int TextureIndexHash = Shader.PropertyToID("_TextureIndex");
        public static readonly int CutoutTextureHash = Shader.PropertyToID("_CutoutTexture");
        public static readonly int CutoutTextureSTHash = Shader.PropertyToID("_CutoutTexture_ST");
        public static readonly int SrcBlendHash = Shader.PropertyToID("_SrcBlend");
        public static readonly int DstBlendHash = Shader.PropertyToID("_DstBlend");

        public static readonly int TargetHash = Shader.PropertyToID("ScreenRenderTargetTexture");
        public static readonly int InfoTargetHash = Shader.PropertyToID("ScreenInfoRenderTargetTexture");

        public static readonly int PrimaryBufferHash = Shader.PropertyToID("PrimaryBuffer");
        public static readonly int HelperBufferHash = Shader.PropertyToID("HelperBuffer");

        public static readonly int PrimaryInfoBufferHash = Shader.PropertyToID("PrimaryInfoBuffer");
        public static readonly int HelperInfoBufferHash = Shader.PropertyToID("HelperInfoBuffer");

        private static Material TransparentBlitMaterial;
        private static Material EmptyFillMaterial;
        private static Material OutlineMaterial;
        private static Material PartialBlitMaterial;
        private static Material ObstacleMaterial;
        private static Material FillMaskMaterial;
        private static Material ZPrepassMaterial;
        private static Material OutlineMaskMaterial;
        private static Material DilateMaterial;
        private static Material EdgeDilateMaterial;
        private static Material BlurMaterial;
        private static Material FinalBlitMaterial;
        private static Material BasicBlitMaterial;
        private static Material ClearStencilMaterial;

        private struct OutlineTargetGroup
        {
            public readonly Outlinable Outlinable;
            public readonly OutlineTarget Target;

            public OutlineTargetGroup(Outlinable outlinable, OutlineTarget target)
            {
                Outlinable = outlinable;
                Target = target;
            }
        }

        private static List<OutlineTargetGroup> targets = new List<OutlineTargetGroup>();

        private static List<string> keywords = new List<string>();

        public static Material LoadMaterial(string shaderName)
        {
            return new Material(Resources.Load<Shader>(string.Format("Easy performant outline/Shaders/{0}", shaderName)));
        }

        [RuntimeInitializeOnLoadMethod]
        private static void InitMaterials()
        {
            if (PartialBlitMaterial == null)
                PartialBlitMaterial = LoadMaterial("PartialBlit");

            if (ObstacleMaterial == null)
                ObstacleMaterial = LoadMaterial("Obstacle");

            if (OutlineMaterial == null)
                OutlineMaterial = LoadMaterial("Outline");

            if (TransparentBlitMaterial == null)
                TransparentBlitMaterial = LoadMaterial("TransparentBlit");

            if (ZPrepassMaterial == null)
                ZPrepassMaterial = LoadMaterial("ZPrepass");

            if (OutlineMaskMaterial == null)
                OutlineMaskMaterial = LoadMaterial("OutlineMask");

            if (DilateMaterial == null)
                DilateMaterial = LoadMaterial("Dilate");

            if (BlurMaterial == null)
                BlurMaterial = LoadMaterial("Blur");

            if (FinalBlitMaterial == null)
                FinalBlitMaterial = LoadMaterial("FinalBlit");

            if (BasicBlitMaterial == null)
                BasicBlitMaterial = LoadMaterial("BasicBlit");

            if (EmptyFillMaterial == null)
                EmptyFillMaterial = LoadMaterial("Fills/EmptyFill");

            if (FillMaskMaterial == null)
                FillMaskMaterial = LoadMaterial("Fills/FillMask");

            if (ClearStencilMaterial == null)
                ClearStencilMaterial = LoadMaterial("ClearStencil");

            if (EdgeDilateMaterial == null)
                EdgeDilateMaterial = LoadMaterial("EdgeDilate");
        }

        private static void Postprocess(OutlineParameters parameters, float shiftScale, bool scaleIndependent, float bufferScale, int first, int second, Material material, int iterrations, bool additionalShift, float shiftValue, ref int stencil, Rect viewport)
        {
            if (iterrations <= 0)
                return;

            var scalingFactor = scaleIndependent ? bufferScale : 1.0f;

            parameters.Buffer.SetGlobalInt(ComparisonHash, (int)CompareFunction.Always);

            for (var index = 1; index <= iterrations; index++)
            {
                parameters.Buffer.SetGlobalInt(RefHash, stencil);

                var shift = (additionalShift ? (float)index : 1.0f) * scalingFactor;

                parameters.Buffer.SetGlobalVector(ShiftHash, new Vector4(shift * shiftScale, 0));
                Blit(parameters, RenderTargetUtility.ComposeTarget(parameters, first), RenderTargetUtility.ComposeTarget(parameters, second), RenderTargetUtility.ComposeTarget(parameters, second), material, shiftValue, null, -1, viewport);

                parameters.Buffer.SetGlobalVector(ShiftHash, new Vector4(0, shift * shiftScale));
                Blit(parameters, RenderTargetUtility.ComposeTarget(parameters, second), RenderTargetUtility.ComposeTarget(parameters, first), RenderTargetUtility.ComposeTarget(parameters, first), material, shiftValue, null, -1, viewport);

                stencil = (stencil + 1) & 0xFF;
            }
        }

        private static void Blit(OutlineParameters parameters, RenderTargetIdentifier source, RenderTargetIdentifier destination, RenderTargetIdentifier destinationDepth, Material material, float effectSize, CommandBuffer buffer, int pass = -1, Rect? viewport = null)
        {
            parameters.Buffer.SetGlobalFloat(EffectSizeHash, effectSize);
            BlitUtility.Blit(parameters, source, destination, destinationDepth, material, buffer, pass, viewport);
        }

        private static float GetBlurShift(BlurType blurType, int iterrationsCount)
        {
            switch (blurType)
            {
                case BlurType.Anisotropic:
                case BlurType.Box:
                    return (float)(iterrationsCount * 0.65f) + 1.0f;
                case BlurType.Gaussian5x5:
                    return 3.0f *  + iterrationsCount;
                case BlurType.Gaussian9x9:
                    return 5.0f + iterrationsCount;
                case BlurType.Gaussian13x13:
                    return 7.0f + iterrationsCount;
                default:
                    throw new ArgumentException("Unknown blur type");
            }
        }

        private static float GetMaskingValueForMode(OutlinableDrawingMode mode)
        {
            if ((mode & OutlinableDrawingMode.Mask) != 0)
                return 0.6f;
            else if ((mode & OutlinableDrawingMode.Obstacle) != 0)
                return 0.25f;
            else
                return 1.0f;
        }

        private static float ComputeEffectShift(OutlineParameters parameters)
        {
            var effectShift = GetBlurShift(parameters.BlurType, parameters.BlurIterations) * parameters.BlurShift + parameters.DilateIterations * 4.0f * parameters.DilateShift;
            if (!parameters.ScaleIndependent)
                effectShift /= parameters.PrimaryBufferScale;

            return effectShift * 2.0f;
        }
        
        private static void PrepareTargets(OutlineParameters parameters)
        {
            targets.Clear();

            foreach (var outlinable in parameters.OutlinablesToRender)
            {
                foreach (var target in outlinable.OutlineTargets)
                {
                    target.UpdateReadyToRender();
                    target.MakeIgnoreOcclusion();
                    target.UpdateRendererType();

                    var renderer = target.Renderer;
                    if (!target.ReadyToRender)
                    {
                        if ((outlinable.DrawingMode & OutlinableDrawingMode.GenericMask) == 0 || renderer == null)
                            continue;
                    }

                    targets.Add(new OutlineTargetGroup(outlinable, target));
                }
            }
        }

        public static void SetupOutline(OutlineParameters parameters)
        {
            parameters.Buffer.SetGlobalVector(ScaleHash, parameters.Scale);

            PrepareTargets(parameters);

            Profiler.BeginSample("Setup outline");

            Profiler.BeginSample("Check materials");
            InitMaterials();
            Profiler.EndSample();

            var effectShift = ComputeEffectShift(parameters);
            var targetWidth = parameters.TargetWidth;
            var targetHeight = parameters.TargetHeight;

            parameters.Camera.forceIntoRenderTexture = parameters.EyeMask == StereoTargetEyeMask.None || !UnityEngine.XR.XRSettings.enabled || parameters.IsEditorCamera;

            parameters.Buffer.SetGlobalInt(SrcBlendHash, (int)BlendMode.One);
            parameters.Buffer.SetGlobalInt(DstBlendHash, (int)BlendMode.Zero);

            var outlineRef = 1;
            parameters.Buffer.SetGlobalInt(OutlineRefHash, outlineRef);

            SetupDilateKeyword(parameters);

            RenderTargetUtility.GetTemporaryRT(parameters, TargetHash, targetWidth, targetHeight, 24, true, false, false);
            var scaledWidth = (int)(targetWidth * parameters.PrimaryBufferScale);
            var scaledHeight = (int)(targetHeight * parameters.PrimaryBufferScale);

            if (parameters.EyeMask != StereoTargetEyeMask.None)
            {
                if (scaledWidth % 2 != 0)
                    scaledWidth++;

                if (scaledHeight % 2 != 0)
                    scaledHeight++;
            }

            var scaledViewVector = parameters.MakeScaledVector(scaledWidth, scaledHeight);

            RenderTargetUtility.GetTemporaryRT(parameters, PrimaryBufferHash, scaledWidth, scaledHeight, 24, true, false, false);
            RenderTargetUtility.GetTemporaryRT(parameters, HelperBufferHash, scaledWidth, scaledHeight, 24, true, false, false);

            if (parameters.UseInfoBuffer)
            {
                var scaledInfoWidth = (int)(targetWidth * Mathf.Min(parameters.PrimaryBufferScale, parameters.InfoBufferScale));
                if (scaledInfoWidth % 2 != 0)
                    scaledInfoWidth++;

                var scaledInfoHeight = (int)(targetHeight * Mathf.Min(parameters.PrimaryBufferScale, parameters.InfoBufferScale));
                if (scaledInfoHeight % 2 != 0)
                    scaledInfoHeight++;

                RenderTargetUtility.GetTemporaryRT(parameters, InfoTargetHash, targetWidth, targetHeight, 0, false, false, false);

                RenderTargetUtility.GetTemporaryRT(parameters, PrimaryInfoBufferHash, scaledInfoWidth, scaledInfoHeight, 0, true, true, false);
                RenderTargetUtility.GetTemporaryRT(parameters, HelperInfoBufferHash, scaledInfoWidth, scaledInfoHeight, 0, true, true, false);
            }

            Profiler.BeginSample("Updating mesh");
            BlitUtility.SetupMesh(parameters, effectShift);
            Profiler.EndSample();

            parameters.Buffer.SetRenderTarget(RenderTargetUtility.ComposeTarget(parameters, TargetHash), parameters.DepthTarget);
            if (parameters.CustomViewport.HasValue)
                parameters.Buffer.SetViewport(parameters.CustomViewport.Value);

            DrawOutlineables(parameters, CompareFunction.LessEqual, false, 0.0f, x => true, x => Color.clear, x => ZPrepassMaterial, RenderStyle.FrontBack | RenderStyle.Single, OutlinableDrawingMode.ZOnly);

            parameters.Buffer.DisableShaderKeyword(KeywordsUtility.GetEnabledInfoBufferKeyword());

            if (parameters.UseInfoBuffer)
            {
                parameters.Buffer.EnableShaderKeyword(KeywordsUtility.GetInfoBufferStageKeyword());

                parameters.Buffer.SetRenderTarget(RenderTargetUtility.ComposeTarget(parameters, InfoTargetHash), parameters.DepthTarget);
                parameters.Buffer.ClearRenderTarget(false, true, Color.clear);
                if (parameters.CustomViewport.HasValue)
                    parameters.Buffer.SetViewport(parameters.CustomViewport.Value);

                DrawOutlineables(parameters, CompareFunction.Always,    false, 0.0f, x => x.OutlineParameters.Enabled,   x => new Color(x.OutlineParameters.DilateShift * parameters.DilateShift, x.OutlineParameters.BlurShift * parameters.BlurShift, 0, 1),
                    x => OutlineMaterial,
                    RenderStyle.Single, 
                    OutlinableDrawingMode.Normal);

                DrawOutlineables(parameters, CompareFunction.NotEqual,  false, 0.0f, x => x.BackParameters.Enabled,      x => new Color(x.BackParameters.DilateShift * parameters.DilateShift, x.BackParameters.BlurShift * parameters.BlurShift, 0, 1),
                    x => OutlineMaterial,
                    RenderStyle.FrontBack);

                DrawOutlineables(parameters, CompareFunction.LessEqual, false, 0.0f, x => x.FrontParameters.Enabled,     x => new Color(x.FrontParameters.DilateShift * parameters.DilateShift, x.FrontParameters.BlurShift * parameters.BlurShift, 0, 1),
                    x => OutlineMaterial,
                    RenderStyle.FrontBack, 
                    OutlinableDrawingMode.Normal);

                DrawOutlineables(parameters, CompareFunction.LessEqual, false, 0.0f, x => true, x => new Color(0, 0, GetMaskingValueForMode(x.DrawingMode), 1),
                    x => ObstacleMaterial,
                    RenderStyle.Single | RenderStyle.FrontBack,
                    OutlinableDrawingMode.Obstacle | OutlinableDrawingMode.Mask);

                parameters.Buffer.SetGlobalInt(ComparisonHash, (int)CompareFunction.Always);
                parameters.Buffer.SetGlobalInt(RefHash, 0);
                Blit(parameters, 
                    RenderTargetUtility.ComposeTarget(parameters, InfoTargetHash), 
                    RenderTargetUtility.ComposeTarget(parameters, PrimaryInfoBufferHash),
                    RenderTargetUtility.ComposeTarget(parameters, PrimaryInfoBufferHash), 
                    BasicBlitMaterial, effectShift, null,
                    -1);

                var infoRef = 0;
                var bufferScale = Mathf.Min(parameters.PrimaryBufferScale, parameters.InfoBufferScale);
                Postprocess(parameters, 1.0f, false, bufferScale, PrimaryInfoBufferHash, HelperInfoBufferHash, DilateMaterial,
                    (parameters.DilateQuality == DilateQuality.Base ? parameters.DilateIterations : parameters.DilateIterations * 2) + parameters.BlurIterations, false,
                    effectShift, ref infoRef,
                    new Rect(0, 0, scaledViewVector.x, scaledViewVector.y));
                
                parameters.Buffer.SetRenderTarget(RenderTargetUtility.ComposeTarget(parameters, InfoTargetHash), parameters.DepthTarget);
                if (parameters.CustomViewport.HasValue)
                    parameters.Buffer.SetViewport(parameters.CustomViewport.Value);

                DrawOutlineables(parameters, CompareFunction.Always,    true, effectShift,  x => x.OutlineParameters.Enabled,   x => new Color(x.OutlineParameters.DilateShift * 0.5f * parameters.DilateShift, x.OutlineParameters.BlurShift * 0.5f * parameters.BlurShift, 0, 1.0f), x => EdgeDilateMaterial, RenderStyle.Single, OutlinableDrawingMode.Normal);
                DrawOutlineables(parameters, CompareFunction.NotEqual,  true, effectShift, x => x.BackParameters.Enabled,      x => new Color(x.BackParameters.DilateShift * 0.5f * parameters.DilateShift, x.BackParameters.BlurShift * 0.5f * parameters.BlurShift, 0, x.ComplexMaskingEnabled ? 0.0f : 0.2f), x => EdgeDilateMaterial, RenderStyle.FrontBack);
                DrawOutlineables(parameters, CompareFunction.LessEqual, true, effectShift, x => x.FrontParameters.Enabled,     x => new Color(x.FrontParameters.DilateShift * 0.5f * parameters.DilateShift, x.FrontParameters.BlurShift * 0.5f * parameters.BlurShift, 0, 1.0f), x => EdgeDilateMaterial, RenderStyle.FrontBack, OutlinableDrawingMode.Normal);

                parameters.Buffer.SetGlobalTexture(InfoBufferHash, PrimaryInfoBufferHash);

                parameters.Buffer.DisableShaderKeyword(KeywordsUtility.GetInfoBufferStageKeyword());
            }

            if (parameters.UseInfoBuffer)
                parameters.Buffer.EnableShaderKeyword(KeywordsUtility.GetEnabledInfoBufferKeyword());

            parameters.Buffer.SetRenderTarget(RenderTargetUtility.ComposeTarget(parameters, TargetHash), parameters.DepthTarget);
            parameters.Buffer.ClearRenderTarget(false, true, Color.clear);
            if (parameters.CustomViewport.HasValue)
                parameters.Buffer.SetViewport(parameters.CustomViewport.Value);

            var drawnOutlinablesCount = 0;
            drawnOutlinablesCount += DrawOutlineables(parameters, CompareFunction.Always, false, 0.0f, x => x.OutlineParameters.Enabled, x => x.OutlineParameters.Color,
                x => OutlineMaterial,
                RenderStyle.Single, OutlinableDrawingMode.Normal);

            drawnOutlinablesCount += DrawOutlineables(parameters, CompareFunction.NotEqual, false, 0.0f, x => x.BackParameters.Enabled, x => x.BackParameters.Color,
                x => OutlineMaterial,
                RenderStyle.FrontBack, OutlinableDrawingMode.Normal);

            drawnOutlinablesCount += DrawOutlineables(parameters, CompareFunction.LessEqual, false, 0.0f, x => x.FrontParameters.Enabled, x => x.FrontParameters.Color,
                x => OutlineMaterial,
                RenderStyle.FrontBack, OutlinableDrawingMode.Normal);

            parameters.Buffer.SetGlobalInt(ComparisonHash, (int)CompareFunction.Always);

            var postProcessingRef = 0;
            if (drawnOutlinablesCount > 0)
            {
                Blit(parameters, RenderTargetUtility.ComposeTarget(parameters, TargetHash),
                    RenderTargetUtility.ComposeTarget(parameters, PrimaryBufferHash),
                    RenderTargetUtility.ComposeTarget(parameters, PrimaryBufferHash),
                    BasicBlitMaterial, effectShift, null, -1, 
                    new Rect(0, 0, scaledViewVector.x, scaledViewVector.y));

                Postprocess(parameters, parameters.DilateShift, parameters.ScaleIndependent, parameters.PrimaryBufferScale, PrimaryBufferHash, HelperBufferHash, DilateMaterial, parameters.DilateIterations, false, effectShift, ref postProcessingRef,
                    new Rect(0, 0, scaledViewVector.x, scaledViewVector.y));
            }

            parameters.Buffer.SetRenderTarget(RenderTargetUtility.ComposeTarget(parameters, TargetHash), parameters.DepthTarget);
            if (drawnOutlinablesCount > 0)
                parameters.Buffer.ClearRenderTarget(false, true, Color.clear);

            if (parameters.CustomViewport.HasValue)
                parameters.Buffer.SetViewport(parameters.CustomViewport.Value);

            var drawnSimpleEdgeDilateOutlinesCount = DrawOutlineables(parameters, CompareFunction.Always,   true, -1, x => x.OutlineParameters.Enabled, x => x.OutlineParameters.Color, x => EdgeDilateMaterial, RenderStyle.Single);
            drawnSimpleEdgeDilateOutlinesCount += DrawOutlineables(parameters, CompareFunction.NotEqual,    true, -1, x => x.BackParameters.Enabled,    x => x.BackParameters.Color, x => EdgeDilateMaterial, RenderStyle.FrontBack);
            drawnSimpleEdgeDilateOutlinesCount += DrawOutlineables(parameters, CompareFunction.LessEqual,   true, -1, x => x.FrontParameters.Enabled,   x => x.FrontParameters.Color, x => EdgeDilateMaterial, RenderStyle.FrontBack);

            if (drawnSimpleEdgeDilateOutlinesCount > 0)
            {
                Blit(parameters, 
                    RenderTargetUtility.ComposeTarget(parameters, TargetHash), 
                    RenderTargetUtility.ComposeTarget(parameters, PrimaryBufferHash), 
                    RenderTargetUtility.ComposeTarget(parameters, PrimaryBufferHash), 
                    PartialBlitMaterial, effectShift, null, -1,
                    new Rect(0, 0, scaledViewVector.x, scaledViewVector.y));
            }

            if (parameters.BlurIterations > 0)
            {
                SetupBlurKeyword(parameters);
                Postprocess(parameters, parameters.BlurShift, parameters.ScaleIndependent, parameters.PrimaryBufferScale, PrimaryBufferHash, HelperBufferHash, BlurMaterial, parameters.BlurIterations, false, 
                    effectShift, 
                    ref postProcessingRef,
                    new Rect(0, 0, scaledViewVector.x, scaledViewVector.y));
            }

            DrawOutlineables(parameters, CompareFunction.LessEqual, false, 0.0f, x => true, x => Color.clear, x => OutlineMaskMaterial, RenderStyle.FrontBack | RenderStyle.Single, OutlinableDrawingMode.GenericMask);

            parameters.Buffer.SetGlobalInt(ComparisonHash, (int)CompareFunction.NotEqual);
            parameters.Buffer.SetGlobalInt(ReadMaskHash, 255);
            parameters.Buffer.SetGlobalInt(OperationHash, (int)StencilOp.Replace);

            Blit(parameters, 
                RenderTargetUtility.ComposeTarget(parameters, PrimaryBufferHash), 
                parameters.Target, 
                parameters.DepthTarget, 
                FinalBlitMaterial, 
                effectShift, null, -1,
                parameters.CustomViewport);

            DrawFill(parameters, parameters.Target);

            parameters.Buffer.SetGlobalFloat(EffectSizeHash, effectShift);
            BlitUtility.Draw(parameters, parameters.Target, parameters.DepthTarget, ClearStencilMaterial, parameters.CustomViewport);

            parameters.Buffer.ReleaseTemporaryRT(PrimaryBufferHash);

            parameters.Buffer.ReleaseTemporaryRT(HelperBufferHash);
            parameters.Buffer.ReleaseTemporaryRT(TargetHash);

            if (parameters.UseInfoBuffer)
            {
                parameters.Buffer.ReleaseTemporaryRT(InfoBufferHash);
                parameters.Buffer.ReleaseTemporaryRT(PrimaryInfoBufferHash);
                parameters.Buffer.ReleaseTemporaryRT(HelperInfoBufferHash);
            }

            Profiler.EndSample();
        }

        private static void SetupDilateKeyword(OutlineParameters parameters)
        {
            KeywordsUtility.GetAllDilateKeywords(keywords);
            foreach (var keyword in keywords)
                parameters.Buffer.DisableShaderKeyword(keyword);

            parameters.Buffer.EnableShaderKeyword(KeywordsUtility.GetDilateQualityKeyword(parameters.DilateQuality));
        }

        private static void SetupBlurKeyword(OutlineParameters parameters)
        {
            KeywordsUtility.GetAllBlurKeywords(keywords);
            foreach (var keyword in keywords)
                parameters.Buffer.DisableShaderKeyword(keyword);

            parameters.Buffer.EnableShaderKeyword(KeywordsUtility.GetBlurKeyword(parameters.BlurType));
        }

        private static int DrawOutlineables(OutlineParameters parameters, CompareFunction function, bool edgeShiftOnly, float shift, Func<Outlinable, bool> shouldRender, Func<Outlinable, Color> colorProvider, Func<Outlinable, Material> materialProvider, RenderStyle styleMask, OutlinableDrawingMode modeMask = OutlinableDrawingMode.Normal)
        {
            var drawnCount = 0;
            parameters.Buffer.SetGlobalInt(ZTestHash, (int)function);

            foreach (var targetGroup in targets)
            {
                var outlinable = targetGroup.Outlinable;
                if ((int)(outlinable.RenderStyle & styleMask) == 0)
                    continue;

                if ((int)(outlinable.DrawingMode & modeMask) == 0)
                    continue;

                parameters.Buffer.DisableShaderKeyword(KeywordsUtility.GetBackKeyword(ComplexMaskingMode.MaskingMode));
                parameters.Buffer.DisableShaderKeyword(KeywordsUtility.GetBackKeyword(ComplexMaskingMode.ObstaclesMode));
                if (function == CompareFunction.NotEqual && outlinable.ComplexMaskingEnabled)
                    parameters.Buffer.EnableShaderKeyword(KeywordsUtility.GetBackKeyword(outlinable.ComplexMaskingMode));

                var color = shouldRender(outlinable) ? colorProvider(outlinable) : Color.clear;

                parameters.Buffer.SetGlobalColor(ColorHash, color);
                var target = targetGroup.Target;

                var postProcessing = !target.CanUseEdgeDilateShift || target.DilateRenderingMode == DilateRenderMode.PostProcessing;
                if (edgeShiftOnly && postProcessing)
                    continue;

                if (!postProcessing)
                {
                    var dilateShift = 0.0f;
                    switch (function)
                    {
                        case CompareFunction.Always:
                            dilateShift = target.EdgeDilateAmount;
                            break;
                        case CompareFunction.NotEqual:
                            dilateShift = target.BackEdgeDilateAmount;
                            break;
                        case CompareFunction.LessEqual:
                            dilateShift = target.FrontEdgeDilateAmount;
                            break;
                    }

                    parameters.Buffer.SetGlobalFloat(DilateShiftHash, shift < 0.0f ? dilateShift : shift);
                }

                parameters.Buffer.SetGlobalInt(ColorMaskHash, postProcessing ? 255 : 0);

                SetupCutout(parameters, target);
                SetupCull(parameters, target);

                if (postProcessing || edgeShiftOnly)
                    drawnCount++;

                var materialToUse = materialProvider(outlinable);
                parameters.Buffer.DrawRenderer(target.Renderer, materialToUse, target.ShiftedSubmeshIndex);
            }

            return drawnCount;
        }

        private static void DrawFill(OutlineParameters parameters, RenderTargetIdentifier targetSurfance)
        {
            parameters.Buffer.SetRenderTarget(targetSurfance, parameters.DepthTarget);
            if (parameters.CustomViewport.HasValue)
                parameters.Buffer.SetViewport(parameters.CustomViewport.Value);

            var singleMask = 1;
            var frontMask = 2;
            var backMask = 3;

            foreach (var outlinable in parameters.OutlinablesToRender)
            {
                if ((outlinable.DrawingMode & OutlinableDrawingMode.Normal) == 0)
                    continue;

                if (outlinable.ComplexMaskingEnabled)
                    parameters.Buffer.EnableShaderKeyword(KeywordsUtility.GetBackKeyword(outlinable.ComplexMaskingMode));

                parameters.Buffer.SetGlobalInt(ZTestHash, (int)CompareFunction.Greater);
                foreach (var target in outlinable.OutlineTargets)
                {
                    if (target.Renderer == null)
                        continue;

                    var renderer = target.Renderer;
                    if (!renderer.gameObject.activeInHierarchy || !renderer.enabled)
                        continue;

                    SetupCutout(parameters, target);
                    SetupCull(parameters, target);

                    parameters.Buffer.SetGlobalInt(FillRefHash, backMask);
                    parameters.Buffer.DrawRenderer(renderer, FillMaskMaterial, target.ShiftedSubmeshIndex);
                }

                if (outlinable.ComplexMaskingEnabled)
                    parameters.Buffer.DisableShaderKeyword(KeywordsUtility.GetBackKeyword(outlinable.ComplexMaskingMode));
            }

            foreach (var outlinable in parameters.OutlinablesToRender)
            {
                if ((outlinable.DrawingMode & OutlinableDrawingMode.Normal) == 0)
                    continue;

                parameters.Buffer.SetGlobalInt(ZTestHash, (int)CompareFunction.LessEqual);
                foreach (var target in outlinable.OutlineTargets)
                {
                    if (target.Renderer == null)
                        continue;

                    var renderer = target.Renderer;
                    if (!renderer.gameObject.activeInHierarchy || !renderer.enabled)
                        continue;

                    SetupCutout(parameters, target);
                    SetupCull(parameters, target);

                    parameters.Buffer.SetGlobalInt(FillRefHash, frontMask);
                    parameters.Buffer.DrawRenderer(renderer, FillMaskMaterial, target.ShiftedSubmeshIndex);
                }
            }

            foreach (var outlinable in parameters.OutlinablesToRender)
            {
                if ((outlinable.DrawingMode & OutlinableDrawingMode.Normal) == 0)
                    continue;

                if (outlinable.RenderStyle == RenderStyle.FrontBack)
                {
                    if ((outlinable.BackParameters.FillPass.Material == null || !outlinable.BackParameters.Enabled) &&
                        (outlinable.FrontParameters.FillPass.Material == null || !outlinable.FrontParameters.Enabled))
                        continue;

                    var frontMaterial = outlinable.FrontParameters.FillPass.Material;
                    parameters.Buffer.SetGlobalInt(FillRefHash, frontMask);
                    if (frontMaterial != null && outlinable.FrontParameters.Enabled)
                    {
                        foreach (var target in outlinable.OutlineTargets)
                        {
                            if (target.Renderer == null)
                                continue;

                            var renderer = target.Renderer;
                            if (!renderer.gameObject.activeInHierarchy || !renderer.enabled)
                                continue;

                            SetupCutout(parameters, target);
                            SetupCull(parameters, target);

                            parameters.Buffer.DrawRenderer(renderer, frontMaterial, target.ShiftedSubmeshIndex);
                        }
                    }

                    var backMaterial = outlinable.BackParameters.FillPass.Material;
                    parameters.Buffer.SetGlobalInt(FillRefHash, backMask);
                    if (backMaterial != null && outlinable.BackParameters.Enabled)
                    {
                        foreach (var target in outlinable.OutlineTargets)
                        {
                            if (target.Renderer == null)
                                continue;

                            var renderer = target.Renderer;
                            if (!renderer.gameObject.activeInHierarchy || !renderer.enabled)
                                continue;

                            SetupCutout(parameters, target);
                            SetupCull(parameters, target);

                            parameters.Buffer.DrawRenderer(renderer, backMaterial, target.ShiftedSubmeshIndex);
                        }
                    }
                }
                else
                {
                    if (outlinable.OutlineParameters.FillPass.Material == null)
                        continue;

                    if (!outlinable.OutlineParameters.Enabled)
                        continue;

                    parameters.Buffer.SetGlobalInt(FillRefHash, singleMask);
                    parameters.Buffer.SetGlobalInt(ZTestHash, (int)CompareFunction.Always);

                    foreach (var target in outlinable.OutlineTargets)
                    {
                        if (target.Renderer == null)
                            continue;

                        var renderer = target.Renderer;
                        if (!renderer.gameObject.activeInHierarchy || !renderer.enabled)
                            continue;

                        SetupCutout(parameters, target);
                        SetupCull(parameters, target);

                        parameters.Buffer.DrawRenderer(renderer, FillMaskMaterial, target.ShiftedSubmeshIndex);
                    }

                    parameters.Buffer.SetGlobalInt(FillRefHash, singleMask);
                    var fillMaterial = outlinable.OutlineParameters.FillPass.Material;
                    if (FillMaskMaterial != null)
                    {
                        foreach (var target in outlinable.OutlineTargets)
                        {
                            if (target.Renderer == null)
                                continue;

                            var renderer = target.Renderer;
                            if (!renderer.gameObject.activeInHierarchy || !renderer.enabled)
                                continue;

                            SetupCutout(parameters, target);
                            SetupCull(parameters, target);

                            parameters.Buffer.DrawRenderer(renderer, fillMaterial, target.ShiftedSubmeshIndex);
                        }
                    }
                }
            }
        }

        private static void SetupCutout(OutlineParameters parameters, OutlineTarget target)
        {
            if (target.Renderer == null)
                return;

            if (target.Renderer is SpriteRenderer)
            {
                var spriteRenderer = target.Renderer as SpriteRenderer;
                var sprite = spriteRenderer.sprite;
                if (sprite == null)
                {
                    parameters.Buffer.DisableShaderKeyword(KeywordsUtility.GetCutoutKeyword());
                    return;
                }

                parameters.Buffer.EnableShaderKeyword(KeywordsUtility.GetCutoutKeyword());
                parameters.Buffer.SetGlobalFloat(CutoutThresholdHash, target.CutoutThreshold);
                parameters.Buffer.SetGlobalTexture(CutoutTextureHash, spriteRenderer.sprite.texture);

                return;
            }

            var materialToGetTextureFrom = target.Renderer.sharedMaterial;

            if (target.CutoutDescriptionType != CutoutDescriptionType.None &&
                materialToGetTextureFrom != null &&
                materialToGetTextureFrom.HasProperty(target.CutoutTextureId))
            {
                parameters.Buffer.EnableShaderKeyword(KeywordsUtility.GetCutoutKeyword());
                parameters.Buffer.SetGlobalFloat(CutoutThresholdHash, target.CutoutThreshold);

                var offset = materialToGetTextureFrom.GetTextureOffset(target.CutoutTextureId);
                var scale = materialToGetTextureFrom.GetTextureScale(target.CutoutTextureId);

                parameters.Buffer.SetGlobalVector(CutoutTextureSTHash, new Vector4(scale.x, scale.y, offset.x, offset.y));

                var texture = materialToGetTextureFrom.GetTexture(target.CutoutTextureId);
                if (texture == null || texture.dimension != TextureDimension.Tex2DArray)
                    parameters.Buffer.DisableShaderKeyword(KeywordsUtility.GetTextureArrayCutoutKeyword());
                else
                {
                    parameters.Buffer.SetGlobalFloat(TextureIndexHash, target.CutoutTextureIndex);
                    parameters.Buffer.EnableShaderKeyword(KeywordsUtility.GetTextureArrayCutoutKeyword());
                }

                parameters.Buffer.SetGlobalTexture(CutoutTextureHash, texture);
            }
            else
                parameters.Buffer.DisableShaderKeyword(KeywordsUtility.GetCutoutKeyword());

        }

        private static void SetupCull(OutlineParameters parameters, OutlineTarget target)
        {
            parameters.Buffer.SetGlobalInt(CullHash, (int)target.CullMode);
        }
    }
}