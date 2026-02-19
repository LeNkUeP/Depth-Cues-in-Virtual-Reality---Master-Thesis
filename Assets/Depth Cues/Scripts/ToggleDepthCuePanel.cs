using Oculus.Interaction.Locomotion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DepthCuePanel : MonoBehaviour
{

    // ********************************************************************************************************
    // ********************************************************************************************************
    //                                          SETTINGS & ATTRIBUTES
    // ********************************************************************************************************
    // ********************************************************************************************************

    [Header("De-/Activation of Depth Cues")]
    public bool shadowCastEnabled = true;
    public bool shapeFromShadingEnabled = true;
    public bool occlusionEnabled = true;
    public bool disparityEnabled = true;
    public bool motionParallaxEnabled = true;
    public bool atmosphericPerspectiveEnabled = true;
    public bool relativeSizeEnabled = true;
    public bool knownSizeEnabled = true;
    public bool heightInFieldOfViewEnabled = true;
    public bool accommodationEnabled = true;
    public bool convergenceEnabled = true;
    public bool imageBlurEnabled = true;
    public bool textureGradientEnabled = true;
    public bool linearPerspectiveEnabled = true;
    public bool accretionEnabled = true;

    [Header("General")]
    private Camera cam;
    private Renderer[] allRenderers;

    [Header("Binocular disparity - Settings")]
    private Vector3 cameraLocalScale;

    [Header("Motion parallax - Settings")]
    private OVRCameraRig cameraRig;
    private FirstPersonLocomotor locomotor;

    [Header("Accretion - Settings")]
    // nothing

    [Header("Occlusion - Settings")]
    // nothing

    [Header("Accommodation - Settings")]
    [Range(1, 300)]
    public float focalLength = 300;
    [Range(1, 32)]
    public float aperture = 32;
    [Range(0, 1)]
    public float focusSpeed = 0.2f;
    private GameObject volumeGameobject;
    private Volume volume;
    private VolumeProfile profile;
    private DepthOfField depthOfField;
    private RaycastHit hit;
    private Coroutine dofCoroutine;
    private float currentFocus;

    [Header("Convergence - Settings")]
    // nothing

    [Header("Image Blur - Settings")]
    // nothing

    [Header("Atmospheric Perspective - Settings")]
    [Range(0, 1)]
    public float fogDensity = 0.03f;
    public float fogFadeSpeed = 1.0f;
    public Color fogColor = new Color(76,185,200);

    [Header("Texture gradient - Settings")]
    // nothing

    [Header("Linear perspective - Settings")]
    // nothing

    [Header("Shadow cast - Settings")]
    // nothing

    [Header("Shape from shading - Settings")]
    private Dictionary<Renderer, Material[]> originalMaterials = new Dictionary<Renderer, Material[]>();
    private Dictionary<Material, Material> unlitCache = new Dictionary<Material, Material>();

    [Header("Relative size - Settings")]
    public GameObject[] relativeSizeObjects;

    [Header("Known size - Settings")]
    public GameObject[] knownSizeObjects;

    [Header("Height in field of view - Settings")]
    public GameObject[] heightInFieldOfViewObjects;



    // ********************************************************************************************************
    // ********************************************************************************************************
    //                                          INITIALIZATION
    // ********************************************************************************************************
    // ********************************************************************************************************

    public void Start()
    {
        // general
        allRenderers = FindObjectsByType<Renderer>(FindObjectsSortMode.None);
        cam = Camera.main;

        // motion parallax
        cameraRig = FindFirstObjectByType<OVRCameraRig>();
        locomotor = FindFirstObjectByType<FirstPersonLocomotor>();

        // accommodation
        //InitDof();
        //StartCoroutine(UpdateDOFFocus());
    }

    // ********************************************************************************************************
    // ********************************************************************************************************
    //                                      DEPTH CUE IMPLEMENTATION
    // ********************************************************************************************************
    // ********************************************************************************************************



    // ********************************************************************************************************
    // ********************************************************************************************************
    // ACCOMMODATION
    // ********************************************************************************************************
    // ********************************************************************************************************

    public void ToggleAccommodation()
    {
        accommodationEnabled = !accommodationEnabled;

        if (!accommodationEnabled)
        {
            if (dofCoroutine != null)
            {
                StopCoroutine(dofCoroutine);
            }
            volume.gameObject.SetActive(false);
        }
        else
        {
            volume.gameObject.SetActive(true);
            
            dofCoroutine = StartCoroutine(UpdateDOFFocus());
        }

        Debug.Log("Accommodation: " + (accommodationEnabled ? "ENABLED" : "DISABLED"));
    }

    private void InitDof()
    {
        volumeGameobject = new GameObject("DofVolume");
        volumeGameobject.transform.parent = transform;

        volume = volumeGameobject.AddComponent<Volume>();
        volume.isGlobal = true;

        profile = ScriptableObject.CreateInstance<VolumeProfile>();
        depthOfField = profile.Add<DepthOfField>(true);

        depthOfField.mode.overrideState = true;
        depthOfField.mode.value = DepthOfFieldMode.Bokeh;

        depthOfField.focalLength.overrideState = true;
        depthOfField.focalLength.value = 300f;

        depthOfField.aperture.overrideState = true;
        depthOfField.aperture.value = 32f;

        volume.profile = profile;
    }

    private IEnumerator UpdateDOFFocus()
    {
        //WaitForSeconds wait = new WaitForSeconds(0.05f); // 20x pro Sekunde

        while (true)
        {
            float targetFocus = currentFocus;

            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit))
            {
                targetFocus = hit.distance;
            }

            currentFocus = Mathf.Lerp(currentFocus, targetFocus, focusSpeed);
            depthOfField.focusDistance.value = currentFocus;

            yield return null;
        }
    }

    // ********************************************************************************************************
    // ********************************************************************************************************
    // HEIGHT IN FIELD OF VIEW
    // ********************************************************************************************************
    // ********************************************************************************************************

    public void ToggleHeightInFieldOfView()
    {
        heightInFieldOfViewEnabled = !heightInFieldOfViewEnabled;

        if (!heightInFieldOfViewEnabled)
        {
            foreach (GameObject heightInFieldOfViewObject in heightInFieldOfViewObjects)
            {
                heightInFieldOfViewObject.SetActive(false);
            }
        }
        else
        {
            foreach (GameObject heightInFieldOfViewObject in heightInFieldOfViewObjects)
            {
                heightInFieldOfViewObject.SetActive(true);
            }
        }

        Debug.Log("Height in Field of View: " + (heightInFieldOfViewEnabled ? "ENABLED" : "DISABLED"));
    }

    // ********************************************************************************************************
    // ********************************************************************************************************
    // KNOWN SIZE
    // ********************************************************************************************************
    // ********************************************************************************************************

    public void ToggleKnownSize()
    {
        knownSizeEnabled = !knownSizeEnabled;

        if (!knownSizeEnabled)
        {
            foreach (GameObject knowSizeObject in knownSizeObjects)
            {
                knowSizeObject.SetActive(false);
            }
        }
        else
        {
            foreach (GameObject knowSizeObject in knownSizeObjects)
            {
                knowSizeObject.SetActive(true);
            }
        }

        Debug.Log("Known Size: " + (knownSizeEnabled ? "ENABLED" : "DISABLED"));
    }

    // ********************************************************************************************************
    // ********************************************************************************************************
    // RELATIVE SIZE
    // ********************************************************************************************************
    // ********************************************************************************************************

    public void ToggleRelativeSize()
    {
        relativeSizeEnabled = !relativeSizeEnabled;

        if (!relativeSizeEnabled)
        {
            foreach (GameObject relSizeObject in relativeSizeObjects)
            {
                relSizeObject.SetActive(false);
            }
        }
        else
        {
            foreach (GameObject relSizeObject in relativeSizeObjects)
            {
                relSizeObject.SetActive(true);
            }
        }

        Debug.Log("Relative Size: " + (relativeSizeEnabled ? "ENABLED" : "DISABLED"));
    }

    // ********************************************************************************************************
    // ********************************************************************************************************
    // ATMOSPHERIC PERSPECTIVE
    // ********************************************************************************************************
    // ********************************************************************************************************

    public void ToggleAtmosphericPerspective()
    {
        atmosphericPerspectiveEnabled = !atmosphericPerspectiveEnabled;

        if (!atmosphericPerspectiveEnabled)
        {
            RenderSettings.fog = false;
        }
        else
        {
            RenderSettings.fogColor = fogColor;
            RenderSettings.fogDensity = fogDensity;
            RenderSettings.fog = true;
        }

        Debug.Log("Atmospheric Perspective: " + (atmosphericPerspectiveEnabled ? "ENABLED" : "DISABLED"));
    }

    // ********************************************************************************************************
    // ********************************************************************************************************
    // MOTION PARALLAX
    // ********************************************************************************************************
    // ********************************************************************************************************

    public void ToggleMotionParallax()
    {
        motionParallaxEnabled = !motionParallaxEnabled;

        if (!motionParallaxEnabled)
        {
            cameraRig.rotationOnlyTracking = true;
            locomotor.DisableMovement();
        }else
        {
            cameraRig.rotationOnlyTracking = false;
            locomotor.EnableMovement();
        }

        Debug.Log("Motion Parallax: " + (motionParallaxEnabled ? "ENABLED" : "DISABLED"));
    }

    // ********************************************************************************************************
    // ********************************************************************************************************
    // DISPARITY
    // ********************************************************************************************************
    // ********************************************************************************************************

    public void ToggleDisparity()
    {
        disparityEnabled = !disparityEnabled;

        if (!disparityEnabled)
        {
            cameraLocalScale = cam.transform.localScale;
            cam.transform.localScale = Vector3.zero;
        }
        else
        {
            cam.transform.localScale = cameraLocalScale;
        }

        Debug.Log("Disparity: " + (disparityEnabled ? "ENABLED" : "DISABLED"));
    }

    //void OnPreCull()
    //{
    //    if (disparityEnabled && cam != null && XRSettings.enabled)
    //    {
    //        Matrix4x4 leftView = cam.GetStereoViewMatrix(Camera.StereoscopicEye.Left);
    //        Matrix4x4 leftProj = cam.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left);

    //        cam.SetStereoViewMatrix(Camera.StereoscopicEye.Right, leftView);
    //        cam.SetStereoProjectionMatrix(Camera.StereoscopicEye.Right, leftProj);
    //    }
    //    else if (!disparityEnabled && cam != null)
    //    {
    //        cam.ResetStereoViewMatrices();
    //        cam.ResetStereoProjectionMatrices();
    //    }
    //}

    // ********************************************************************************************************
    // ********************************************************************************************************
    // SHADOW CAST
    // ********************************************************************************************************
    // ********************************************************************************************************

    public void ToggleShadowCast()
    {
        shadowCastEnabled = !shadowCastEnabled;

        foreach (Renderer rend in allRenderers)
        {
            rend.shadowCastingMode = shadowCastEnabled ? ShadowCastingMode.On : ShadowCastingMode.Off;
            rend.receiveShadows = shadowCastEnabled;
        }

        Debug.Log("Shadow Cast " + (shadowCastEnabled ? "enabled" : "disabled"));
    }

    // ********************************************************************************************************
    // ********************************************************************************************************
    // SHAPE FROM SHADING
    // ********************************************************************************************************
    // ********************************************************************************************************

    public void ToggleShapeFromShading()
    {
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

    // ********************************************************************************************************
    // ********************************************************************************************************
    // OCCLUSION
    // ********************************************************************************************************
    // ********************************************************************************************************

    public void ToggleOcclusion()
    {
        //occlusionEnabled = !occlusionEnabled;

        //foreach (Renderer rend in allRenderers)
        //{
        //    foreach (Material mat in rend.materials)
        //    {
        //        if (occlusionEnabled)
        //        {
        //            // Depth Test und Write wieder normal
        //            mat.SetInt("_ZTest", (int)CompareFunction.LessEqual);
        //            mat.SetInt("_ZWrite", 1);
        //            //mat.renderQueue = -1;
        //        }
        //        else
        //        {
        //            // Depth Test auf Always und ZWrite aus
        //            mat.SetInt("_ZTest", (int)CompareFunction.Greater);
        //            mat.SetInt("_ZWrite", 0);
        //            //mat.renderQueue = (int)RenderQueue.Transparent;
        //            //mat.renderQueue = 5000;
        //        }
        //    }
        //}

        //Debug.Log("Occlusion " + (occlusionEnabled ? "enabled" : "disabled"));
    }

    //public void ToggleOcclusion()
    //{
    //    StartCoroutine(ChangeRenderQueueLoop());
    //    Debug.Log("Occlusion " + (occlusionEnabled ? "enabled" : "disabled"));
    //}

    //public int baseQueue = 3000;

    //public IEnumerator ChangeRenderQueueLoop()
    //{
    //    List<Renderer> renderers = allRenderers.ToList();
    //    occlusionEnabled = !occlusionEnabled;

    //    while (true)
    //    {
    //        if (renderers == null || renderers.Count == 0)
    //            yield break;

    //        // Rotiert die Liste
    //        Renderer last = renderers[renderers.Count - 1];
    //        renderers.RemoveAt(renderers.Count - 1);
    //        renderers.Insert(0, last);

    //        // Neue RenderQueue-Werte setzen
    //        for (int i = 0; i < renderers.Count; i++)
    //        {
    //            if (renderers[i] != null)
    //            {
    //                Material mat = renderers[i].material;
    //                mat.renderQueue = baseQueue + i;
    //            }
    //        }

    //        yield return new WaitForSeconds(0f);
    //    }
    //}
}
