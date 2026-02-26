using Oculus.Interaction.Locomotion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ToggleDepthCuePanel : MonoBehaviour
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

    [Header("Rating Sliders")]
    public GameObject shadowCastRatingSlider;
    public GameObject shapeFromShadingRatingSlider;
    public GameObject occlusionRatingSlider;
    public GameObject disparityRatingSlider;
    public GameObject motionParallaxRatingSlider;
    public GameObject atmosphericPerspectiveRatingSlider;
    public GameObject relativeSizeRatingSlider;
    public GameObject knownSizeRatingSlider;
    public GameObject heightInFieldOfViewRatingSlider;
    public GameObject accommodationRatingSlider;
    public GameObject convergenceRatingSlider;
    public GameObject imageBlurRatingSlider;
    public GameObject textureGradientRatingSlider;
    public GameObject linearPerspectiveRatingSlider;
    public GameObject accretionRatingSlider;
    private GameObject currentRatingSlider;

    private bool shadowCastWasToggled = false;
    private bool shapeFromShadingWasToggled = false;
    private bool occlusionWasToggled = false;
    private bool disparityWasToggled = false;
    private bool motionParallaxWasToggled = false;
    private bool atmosphericPerspectiveWasToggled = false;
    private bool relativeSizeWasToggled = false;
    private bool knownSizeWasToggled = false;
    private bool heightInFieldOfViewWasToggled = false;
    private bool accommodationWasToggled = false;
    private bool convergenceWasToggled = false;
    private bool imageBlurWasToggled = false;
    private bool textureGradientWasToggled = false;
    private bool linearPerspectiveWasToggled = false;
    private bool accretionWasToggled = false;

    [Header("General")]
    public GameObject sceneEnvironment;
    public GameObject ratingPanel;
    public GameObject openedEyesIconUI;
    public GameObject closedEyesIconUI;
    public GameObject nextButtonUI;
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

    [Header("Convergence - Settings")]
    public GameObject[] objectsAffectedByConvergence;
    public List<GameObject> blurClones = new List<GameObject>();
    public float convergenceMaxDistance = .5f;
    public Material blurMaterial;

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
        allRenderers = sceneEnvironment.GetComponentsInChildren<Renderer>(true);
        cam = Camera.main;

        // motion parallax
        cameraRig = FindFirstObjectByType<OVRCameraRig>();
        locomotor = FindFirstObjectByType<FirstPersonLocomotor>();

        // convergence
        foreach (GameObject gb in objectsAffectedByConvergence)
        {
            CreateBlurClone(gb);
        }
    }

    public void ToggleVisibility()
    {
        openedEyesIconUI.SetActive(!openedEyesIconUI.activeSelf);
        closedEyesIconUI.SetActive(!closedEyesIconUI.activeSelf);
        gameObject.SetActive(!gameObject.activeSelf);
    }

    public void UpdateRatingSlider(GameObject slider)
    {
        if (currentRatingSlider != slider)
        {
            // Disable current slider
            if (currentRatingSlider != null)
            {
                currentRatingSlider.SetActive(false);
            }
            // Enable new slider
            currentRatingSlider = slider;
            currentRatingSlider.SetActive(true);
        }
    }

    public void HideCurrentRatingSlider()
    {
        if (currentRatingSlider != null)
        {
            currentRatingSlider.SetActive(false);
        }
    }

    public void ResetWasToggledState()
    {
        shadowCastWasToggled = false;
        shapeFromShadingWasToggled = false;
        occlusionWasToggled = false;
        disparityWasToggled = false;
        motionParallaxWasToggled = false;
        atmosphericPerspectiveWasToggled = false;
        relativeSizeWasToggled = false;
        knownSizeWasToggled = false;
        heightInFieldOfViewWasToggled = false;
        accommodationWasToggled = false;
        convergenceWasToggled = false;
        imageBlurWasToggled = false;
        textureGradientWasToggled = false;
        linearPerspectiveWasToggled = false;
        accretionWasToggled = false;
    }

    public bool CheckIfCompleted()
    {
        if (shadowCastWasToggled && shapeFromShadingWasToggled && occlusionWasToggled && disparityWasToggled &&
            motionParallaxWasToggled && atmosphericPerspectiveWasToggled && relativeSizeWasToggled &&
            knownSizeWasToggled && heightInFieldOfViewWasToggled && accommodationWasToggled && convergenceWasToggled &&
            imageBlurWasToggled && textureGradientWasToggled && linearPerspectiveWasToggled && accretionWasToggled)
        {
            StartCoroutine(ShowUI(nextButtonUI));
            return true;
        }
        return false;
    }

    private IEnumerator ShowUI(GameObject objectToShow)
    {
        objectToShow.SetActive(true);
        objectToShow.GetComponent<Animator>().SetTrigger("show");
        yield return null;
    }

    private void Update()
    {
        if (!convergenceEnabled)
        {
            UpdateBlurClones();
        }
    }

    // ********************************************************************************************************
    // ********************************************************************************************************
    //                                      DEPTH CUE IMPLEMENTATION
    // ********************************************************************************************************
    // ********************************************************************************************************



    // ********************************************************************************************************
    // ********************************************************************************************************
    // OCCLUSION
    // ********************************************************************************************************
    // ********************************************************************************************************

    public void ToggleOcclusion()
    {
        occlusionWasToggled = true;
        occlusionEnabled = !occlusionEnabled;

        CheckIfCompleted();
        UpdateRatingSlider(occlusionRatingSlider);

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

        Debug.Log("Occlusion " + (occlusionEnabled ? "enabled" : "disabled"));
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

    // ********************************************************************************************************
    // ********************************************************************************************************
    // DISPARITY
    // ********************************************************************************************************
    // ********************************************************************************************************

    public void ToggleDisparity()
    {
        disparityWasToggled = true;
        disparityEnabled = !disparityEnabled;

        CheckIfCompleted();
        UpdateRatingSlider(disparityRatingSlider);

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

    // ********************************************************************************************************
    // ********************************************************************************************************
    // CONVERGENCE
    // ********************************************************************************************************
    // ********************************************************************************************************

    public void ToggleConvergence()
    {
        convergenceWasToggled = true;
        convergenceEnabled = !convergenceEnabled;

        CheckIfCompleted();
        UpdateRatingSlider(convergenceRatingSlider);

        if (!convergenceEnabled)
        {
            foreach (GameObject clone in blurClones)
            {
                clone.SetActive(true);
            }
        }
        else
        {
            foreach (GameObject clone in blurClones)
            {
                clone.SetActive(false);
            }
        }

        Debug.Log("Convergence: " + (convergenceEnabled ? "ENABLED" : "DISABLED"));
    }

    public void CreateBlurClone(GameObject originalObject)
    {
        if (blurMaterial == null)
        {
            Debug.LogError("BlurMaterial is null!");
            return;
        }

        // Alle MeshRenderer inklusive inaktiver Objekte holen
        MeshRenderer renderer = originalObject.GetComponentInChildren<MeshRenderer>();
        MeshFilter originalFilter = originalObject.GetComponentInChildren<MeshFilter>();

        if (originalFilter == null || originalFilter.sharedMesh == null)
            return;

        // ----- Neuen visuellen Klon erzeugen -----
        GameObject clone = new GameObject(originalObject.name + "_BlurClone");

        // Parent setzen (wichtig für parallele Bewegung)
        clone.transform.SetParent(originalObject.transform, false);

        // Lokale Transformwerte kopieren
        clone.transform.localPosition = Vector3.zero;
        clone.transform.localRotation = Quaternion.identity;
        clone.transform.localScale = Vector3.one;

        // Nur notwendige Komponenten hinzufügen
        MeshFilter newFilter = clone.AddComponent<MeshFilter>();
        MeshRenderer newRenderer = clone.AddComponent<MeshRenderer>();

        // Mesh referenzieren (NICHT duplizieren → Speicher sparen)
        newFilter.sharedMesh = originalFilter.sharedMesh;

        // Blur-Material zuweisen
        Material blurmaterial_clone = new Material(blurMaterial);
        newRenderer.sharedMaterial = blurmaterial_clone;


        // Optional: gleiche Shadow Settings übernehmen
        //newRenderer.shadowCastingMode = originalRenderer.shadowCastingMode;
        //newRenderer.receiveShadows = originalRenderer.receiveShadows;
        newRenderer.shadowCastingMode = ShadowCastingMode.Off;
        newRenderer.receiveShadows = false;

        // Optional: Layer übernehmen (falls Blur Shader Layer nutzt)
        //clone.layer = originalGO.layer;

        blurClones.Add(clone);
        clone.GetComponent<MeshRenderer>().enabled = false;
    }

    public void UpdateBlurClones()
    {
        foreach (GameObject clone in blurClones)
        {
            float distance = Vector3.Distance(Camera.main.transform.position, this.transform.position);
            if (distance > convergenceMaxDistance)
            {
                clone.GetComponent<MeshRenderer>().enabled = false;
            }
            else
            {
                clone.GetComponent<MeshRenderer>().enabled = true;
                float powerMin = 0f;
                float powerMax = 0.007f;

                // Normalisieren und auf Shaderbereich mappen
                float normalized = Mathf.InverseLerp(convergenceMaxDistance, 0.2f, distance); // 2m -> 0, 0m -> 1
                float power = Mathf.Lerp(powerMin, powerMax, normalized);

                // Shader setzen
                Renderer rend = clone.GetComponent<Renderer>();
                if (rend != null)
                {
                    rend.material.SetFloat("_Power", power);
                    Debug.Log(clone.name + " wird gesetzt auf " + power);
                }
            }
        }
    }

    // ********************************************************************************************************
    // ********************************************************************************************************
    // ACCOMMODATION
    // ********************************************************************************************************
    // ********************************************************************************************************

    public void ToggleAccommodation()
    {
        accommodationWasToggled = true;
        accommodationEnabled = !accommodationEnabled;

        CheckIfCompleted();
        UpdateRatingSlider(accommodationRatingSlider);

        if (!accommodationEnabled)
        {

        }
        else
        {

        }

        Debug.Log("Accommodation: " + (accommodationEnabled ? "ENABLED" : "DISABLED"));
    }

    // ********************************************************************************************************
    // ********************************************************************************************************
    // IMAGE BLUR
    // ********************************************************************************************************
    // ********************************************************************************************************

    public void ToggleImageBlur()
    {
        imageBlurWasToggled = true;
        imageBlurEnabled = !imageBlurEnabled;

        CheckIfCompleted();
        UpdateRatingSlider(imageBlurRatingSlider);

        if (!imageBlurEnabled)
        {

        }
        else
        {

        }

        Debug.Log("Image Blur: " + (imageBlurEnabled ? "ENABLED" : "DISABLED"));
    }

    // ********************************************************************************************************
    // ********************************************************************************************************
    // LINEAR PERSPECTIVE
    // ********************************************************************************************************
    // ********************************************************************************************************

    public void ToggleLinearPerspective()
    {
        linearPerspectiveWasToggled = true;
        linearPerspectiveEnabled = !linearPerspectiveEnabled;

        CheckIfCompleted();
        UpdateRatingSlider(linearPerspectiveRatingSlider);

        if (!linearPerspectiveEnabled)
        {

        }
        else
        {

        }

        Debug.Log("Linear perspektive: " + (linearPerspectiveEnabled ? "ENABLED" : "DISABLED"));
    }

    // ********************************************************************************************************
    // ********************************************************************************************************
    // TEXTURE GRADIENT
    // ********************************************************************************************************
    // ********************************************************************************************************

    public void ToggleTextureGradient()
    {
        textureGradientWasToggled = true;
        textureGradientEnabled = !textureGradientEnabled;

        CheckIfCompleted();
        UpdateRatingSlider(textureGradientRatingSlider);

        if (!textureGradientEnabled)
        {

        }
        else
        {

        }

        Debug.Log("Texture gradient: " + (textureGradientEnabled ? "ENABLED" : "DISABLED"));
    }

    // ********************************************************************************************************
    // ********************************************************************************************************
    // RELATIVE SIZE
    // ********************************************************************************************************
    // ********************************************************************************************************

    public void ToggleRelativeSize()
    {
        relativeSizeWasToggled = true;
        relativeSizeEnabled = !relativeSizeEnabled;

        CheckIfCompleted();
        UpdateRatingSlider(relativeSizeRatingSlider);

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
    // KNOWN SIZE
    // ********************************************************************************************************
    // ********************************************************************************************************

    public void ToggleKnownSize()
    {
        knownSizeWasToggled = true;
        knownSizeEnabled = !knownSizeEnabled;

        CheckIfCompleted();
        UpdateRatingSlider(knownSizeRatingSlider);

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
    // HEIGHT IN FIELD OF VIEW
    // ********************************************************************************************************
    // ********************************************************************************************************

    public void ToggleHeightInFieldOfView()
    {
        heightInFieldOfViewWasToggled = true;
        heightInFieldOfViewEnabled = !heightInFieldOfViewEnabled;

        CheckIfCompleted();
        UpdateRatingSlider(heightInFieldOfViewRatingSlider);

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
    // ATMOSPHERIC PERSPECTIVE
    // ********************************************************************************************************
    // ********************************************************************************************************

    public void ToggleAtmosphericPerspective()
    {
        atmosphericPerspectiveWasToggled = true;
        atmosphericPerspectiveEnabled = !atmosphericPerspectiveEnabled;

        CheckIfCompleted();
        UpdateRatingSlider(atmosphericPerspectiveRatingSlider);

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
    // SHAPE FROM SHADING
    // ********************************************************************************************************
    // ********************************************************************************************************

    public void ToggleShapeFromShading()
    {
        shapeFromShadingWasToggled = true;
        shapeFromShadingEnabled = !shapeFromShadingEnabled;

        CheckIfCompleted();
        UpdateRatingSlider(shapeFromShadingRatingSlider);

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
                        if (original.HasProperty("Texture2D_E5864E9"))
                            unlit.mainTexture = original.mainTexture;

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
    // SHADOW CAST
    // ********************************************************************************************************
    // ********************************************************************************************************

    public void ToggleShadowCast()
    {
        shadowCastWasToggled = true;
        shadowCastEnabled = !shadowCastEnabled;

        CheckIfCompleted();
        UpdateRatingSlider(shadowCastRatingSlider);

        foreach (Renderer rend in allRenderers)
        {
            rend.shadowCastingMode = shadowCastEnabled ? ShadowCastingMode.On : ShadowCastingMode.Off;
            rend.receiveShadows = shadowCastEnabled;
        }

        Debug.Log("Shadow Cast " + (shadowCastEnabled ? "enabled" : "disabled"));
    }

    // ********************************************************************************************************
    // ********************************************************************************************************
    // MOTION PARALLAX
    // ********************************************************************************************************
    // ********************************************************************************************************

    public void ToggleMotionParallax()
    {
        motionParallaxWasToggled = true;
        motionParallaxEnabled = !motionParallaxEnabled;

        CheckIfCompleted();
        UpdateRatingSlider(motionParallaxRatingSlider);

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
    // ACCRETION
    // ********************************************************************************************************
    // ********************************************************************************************************

    public void ToggleAccretion()
    {
        accretionWasToggled = true;
        accretionEnabled = !accretionEnabled;

        CheckIfCompleted();
        UpdateRatingSlider(accretionRatingSlider);

        if (!accretionEnabled)
        {

        }
        else
        {

        }

        Debug.Log("Accretion: " + (accretionEnabled ? "ENABLED" : "DISABLED"));
    }
}
