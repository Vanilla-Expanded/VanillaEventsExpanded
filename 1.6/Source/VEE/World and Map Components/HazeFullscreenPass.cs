using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace VEE.World_and_Map_Components
{
    [StaticConstructorOnStartup]
    public static class HazeFullscreenPass
    {
        public static bool Enabled = true;

        private const int RenderQueue = 3599;
        //private const string ShaderPath = "Map/HazeFullscreen";
        //private const string ShaderName = "Haze/HazeFullscreen";
        private const string NoiseTexAPath = "Weather/HazeNoiseA";
        private const string NoiseTexBPath = "Weather/HazeNoiseB";
        private const string MaskTexPath = "Weather/HazeMask";

        private static Material _material;
        private static bool _triedInit;
        //private static bool _warnedMissingShader;
        private static readonly int NoiseTexA = Shader.PropertyToID("_NoiseTexA");
        private static readonly int NoiseTexB = Shader.PropertyToID("_NoiseTexB");
        private static readonly int MaskTex = Shader.PropertyToID("_MaskTex");
        private static readonly int ColorProperty = Shader.PropertyToID("_Color");
        private static readonly int NoiseScaleA = Shader.PropertyToID("_NoiseScaleA");
        private static readonly int NoiseScaleB = Shader.PropertyToID("_NoiseScaleB");
        private static readonly int NoiseSpeedA = Shader.PropertyToID("_NoiseSpeedA");
        private static readonly int NoiseSpeedB = Shader.PropertyToID("_NoiseSpeedB");
        private static readonly int MaskScale = Shader.PropertyToID("_MaskScale");
        private static readonly int MaskSpeed = Shader.PropertyToID("_MaskSpeed");
        private static readonly int DistortionIntensity = Shader.PropertyToID("_DistortionIntensity");
        private static readonly int RippleIntensity = Shader.PropertyToID("_RippleIntensity");
        private static readonly int HazeIntensity = Shader.PropertyToID("_HazeIntensity");
        private static readonly int BrightnessMultiplier = Shader.PropertyToID("_BrightnessMultiplier");
        private static readonly int IntensityProperty = Shader.PropertyToID("_Intensity");

        public static void Draw(Map map, float intensity)
        {
            if (!Enabled || intensity <= 0f || map == null || map != Find.CurrentMap || !WorldRendererUtility.DrawingMap || LongEventHandler.ShouldWaitForEvent)
                return;

            Material passMaterial = PassMaterial;
            if (passMaterial == null)
                return;

            Camera camera = Find.Camera;
            if (camera == null)
                return;

            passMaterial.SetFloat(IntensityProperty, intensity);
            SkyOverlay.DrawScreenOverlay(passMaterial, AltitudeLayer.MoteOverheadLow.AltitudeFor() - Altitudes.AltInc, 0, camera);
        }

        private static Material PassMaterial
        {
            get
            {
                if (!_triedInit)
                    Init();

                return _material;
            }
        }

        private static void Init()
        {
            _triedInit = true;
            /*Shader shader = Resources.Load<Shader>("Materials/" + ShaderPath) ?? ContentFinder<Shader>.TryFindAssetInModBundles(ShaderPath) ?? Shader.Find(ShaderName);
            if (shader == null)
            {
                if (!warnedMissingShader)
                {
                    warnedMissingShader = true;
                    Log.Warning("Haze could not find fullscreen shader " + ShaderName + ". Build the mod asset bundle or use the native Assets/Resources copy.");
                }
                return;
            }*/

            Shader shader = VEE_DefOf.VEE_HazeFullscreen.Shader;

            if (shader == null)
            {
                // just a quick n dirty log message, have better handling later lol
                Log.Warning("Haze shader NOT found. Ruh roh raggy!");
            }

            _material = new Material(shader)
            {
                name = "HazeFullscreenPass",
                renderQueue = RenderQueue
            };
            _material.SetTexture(NoiseTexA, ContentFinder<Texture2D>.Get(NoiseTexAPath, false) ?? Texture2D.blackTexture);
            _material.SetTexture(NoiseTexB, ContentFinder<Texture2D>.Get(NoiseTexBPath, false) ?? Texture2D.grayTexture);
            _material.SetTexture(MaskTex, ContentFinder<Texture2D>.Get(MaskTexPath, false) ?? Texture2D.grayTexture);
            _material.SetColor(ColorProperty, Color.white);
            _material.SetFloat(NoiseScaleA, 0.171f);
            _material.SetFloat(NoiseScaleB, 0.075f);
            _material.SetVector(NoiseSpeedA, new Vector4(0.08f, 0.03f, 0f, 0f));
            _material.SetVector(NoiseSpeedB, new Vector4(-0.02f, 0.03f, 0f, 0f));
            _material.SetVector(NoiseSpeedB, new Vector4(-0.02f, 0.03f, 0f, 0f));
            _material.SetFloat(MaskScale, 0.075f);
            _material.SetVector(MaskSpeed, new Vector4(0.02f, -0.03f, 0f, 0f));
            _material.SetFloat(DistortionIntensity, 0.035f);
            _material.SetFloat(RippleIntensity, 1f);
            _material.SetFloat(HazeIntensity, 0.4f);
            _material.SetFloat(BrightnessMultiplier, 1f);
            _material.SetFloat(IntensityProperty, 0f);
        }
    }

}
