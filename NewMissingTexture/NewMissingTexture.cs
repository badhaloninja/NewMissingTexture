using CodeX;
using FrooxEngine;
using HarmonyLib;
using NeosModLoader;
using System;
using BaseX;

namespace NewMissingTexture
{
    public class NewMissingTexture : NeosMod
    {
        public override string Name => "NewMissingTexture";
        public override string Author => "badhaloninja";
        public override string Version => "1.0.0";
        public override string Link => "https://github.com/badhaloninja/NewMissingTexture";
        public override void OnEngineInit()
        {
            Harmony harmony = new Harmony("me.badhaloninja.NewMissingTexture");
            harmony.PatchAll();
        }

        [HarmonyPatch(typeof(AssetManager), "Initialize")]
        class AssetManager_Initialize_Patch
        {
            public static void Postfix(AssetManager __instance)
            {
                __instance.Engine.RunPostInit(() =>
                {
                    var DarkCheckerTexture = Traverse.Create(__instance).Property<Texture2D>("DarkCheckerTexture");

                    DarkCheckerTexture.Value = CreateDefaultTexture(__instance, 8, 8, bmp => // 128x128 (default)
                    {
                        for (int y = 0; y < bmp.Size.y; ++y)
                        {
                            for (int x = 0; x < bmp.Size.x; ++x)
                            {
                                //bool flag = (index2 >> 2 & 1) == 1 ^ (index1 >> 2 & 1) == 0; // binary modulo 4 (default)
                                bool flag = (x & 1) == 1 ^ (y & 1) == 0; // binary modulo 2

                                color color = flag ? new color(1f, 0f, 1f, 0.5f) : new color(0f, 0.5f);
                                bmp.SetPixel(x, y, in color); // Write pixel
                            }
                        }
                    });
                    DarkCheckerTexture.Value.Connector.SetTexture2DProperties(TextureFilterMode.Point, 8, TextureWrapMode.Repeat, TextureWrapMode.Repeat, 0, null); // Make point filtered
                });
            }

            [HarmonyReversePatch]
            [HarmonyPatch(typeof(AssetManager), "CreateDefaultTexture", new Type[] { typeof(int), typeof(int), typeof(Action<Bitmap2D>) })]
            public static Texture2D CreateDefaultTexture(AssetManager instance, int width, int height, Action<Bitmap2D> init)
            {
                // its a stub so it has no initial content
                throw new NotImplementedException("It's a stub");
            }
        }
    }
}