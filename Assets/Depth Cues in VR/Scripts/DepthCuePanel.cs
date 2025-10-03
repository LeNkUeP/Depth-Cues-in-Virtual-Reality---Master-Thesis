using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class DepthCuePanel : MonoBehaviour
{
    private static bool shadowCastEnabled = true;
    private static bool shapeFromShadingEnabled = true;
    private static bool occlusionEnabled = true;

    private static Dictionary<Renderer, Material[]> originalMaterials = new Dictionary<Renderer, Material[]>();
    private static Dictionary<Material, Material> unlitCache = new Dictionary<Material, Material>();

    public static void ToggleShadowCast()
    {
        Renderer[] allRenderers = FindObjectsByType<Renderer>(FindObjectsSortMode.None);
        shadowCastEnabled = !shadowCastEnabled;

        foreach (Renderer rend in allRenderers)
        {
            rend.shadowCastingMode = shadowCastEnabled ? ShadowCastingMode.On : ShadowCastingMode.Off;
            rend.receiveShadows = shadowCastEnabled;
        }

        Debug.Log("Shadow Cast " + (shadowCastEnabled ? "enabled" : "disabled"));
    }

    public static void ToggleShapeFromShading()
    {
        Renderer[] allRenderers = FindObjectsByType<Renderer>(FindObjectsSortMode.None);
        shapeFromShadingEnabled = !shapeFromShadingEnabled;

        foreach (Renderer rend in allRenderers)
        {
            if (rend.gameObject.CompareTag("Ground"))
                continue;

            if (shapeFromShadingEnabled)
            {
                // Ursprüngliche Materialien zurücksetzen
                if (originalMaterials.ContainsKey(rend))
                    rend.materials = originalMaterials[rend];
            }
            else
            {
                // Originalmaterialien merken
                if (!originalMaterials.ContainsKey(rend))
                    originalMaterials[rend] = rend.materials;

                // Ersetze jedes Material durch Unlit-Version
                Material[] mats = new Material[rend.materials.Length];
                for (int i = 0; i < rend.materials.Length; i++)
                {
                    Material original = rend.materials[i];

                    if (unlitCache.ContainsKey(original))
                    {
                        mats[i] = unlitCache[original];
                    }
                    else
                    {
                        Material unlit = new Material(Shader.Find("Custom/UnlitWithShadowReceiver"));

                        if (original.HasProperty("_MainTex") && original.mainTexture != null)
                            unlit.mainTexture = original.mainTexture;
                        if (original.HasProperty("_Color"))
                            unlit.color = original.color;

                        mats[i] = unlit;
                        unlitCache[original] = unlit;
                    }
                }
                rend.materials = mats;
            }
        }

        Debug.Log("Shape From Shading " + (shapeFromShadingEnabled ? "enabled (Lit)" : "disabled (Unlit)"));
    }

    public static void ToggleOcclusion()
    {
        Renderer[] allRenderers = FindObjectsByType<Renderer>(FindObjectsSortMode.None);
        occlusionEnabled = !occlusionEnabled;

        foreach (Renderer rend in allRenderers)
        {
            foreach (Material mat in rend.materials)
            {
                if (occlusionEnabled)
                {
                    // Depth Test und Write wieder normal
                    mat.SetInt("_ZTest", (int)CompareFunction.LessEqual);
                    mat.SetInt("_ZWrite", 1);
                }
                else
                {
                    // Depth Test auf Always und ZWrite aus
                    mat.SetInt("_ZTest", (int)CompareFunction.Always);
                    mat.SetInt("_ZWrite", 0);
                    mat.renderQueue = (int)RenderQueue.Transparent;
                }
            }
        }

        Debug.Log("Occlusion " + (occlusionEnabled ? "enabled" : "disabled"));
    }
}
