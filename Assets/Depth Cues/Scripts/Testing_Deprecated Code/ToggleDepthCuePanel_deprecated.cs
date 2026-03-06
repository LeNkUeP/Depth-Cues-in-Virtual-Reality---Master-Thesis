using Oculus.Interaction.Locomotion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class ToggleDepthCuePanel_dedprecated : MonoBehaviour
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
    public OVRCameraRig ovrCameraRig;
    private Camera cam;
    private Renderer[] allRenderers;

    [Header("Binocular disparity - Settings")]
    private Vector3 cameraLocalScale;

    [Header("Motion parallax - Settings")]
    private OVRCameraRig cameraRig;
    private FirstPersonLocomotor locomotor;

    [Header("Accretion - Settings")]
    public AccretionSpawner[] accretionObjects;

    [Header("Occlusion - Settings")]
    public Material occlusionMaterialLit;
    public Material occlusionMaterialUnlit;
    [Range(0, 1)]
    public float occlusionAlpha = 1f;

    [Header("Accommodation - Settings")]
    public LayerMask focusLayer;
    public float rayDownwardAngle = 10f;
    [Range(1f, 30f)]
    public float coneAngle = 10f;
    public float maxFocusDistance = 1000f;
    public float nearSoftDistance = 2f;      // bis hierhin abnehmende Unschaerfe
    [Range(1, 32)]
    public float maxAperture = 32f;
    [Range(1, 32)]
    public float minAperture = 8f;
    [Range(1, 300)]
    public float focalLength = 50f;
    [Range(3, 9)]
    public int bladeCount = 6;
    public float focusSmoothTime = 0.1f;

    private Volume dofVolume;
    private DepthOfField dof;
    private float targetFocusDistance;
    private float currentFocusDistance;
    private float focusVelocity = 0f;
    private LineRenderer rayVisualizer;

    [Header("Convergence - Settings")]
    public Vector3 nearAreaCollisionCubeScale = new Vector3(.5f,.5f, 2);
    public float maxOffset = 0.03f;   // volle Verschiebung
    public float minDistance = 0.2f;  // Abstand, ab dem der Effekt beginnt
    
    private Transform leftEyeObj;
    private Transform rightEyeObj;

    private Transform leftEye;
    private Transform rightEye;

    private GameObject nearAreaCollisionCube;
    //public Material blurMaterial;
    //public float convergenceMaxDistance = .5f;
    //private List<GameObject> blurClones = new List<GameObject>();

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
    private List<GameObject> shadowClones = new List<GameObject>();

    [Header("Shape from shading - Settings")]
    private Dictionary<Renderer, Material> originalMaterials = new Dictionary<Renderer, Material>();
    private Dictionary<Renderer, Material> unlitCache = new Dictionary<Renderer, Material>();

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

        // create object clones for different depth cues
        foreach (Renderer rend in allRenderers)
        {
            CreateNoShapeFromShadingMaterial(rend);
            CreateShadowClone(rend.gameObject);
        }

        InitAccommodation();
        //InitConvergence();
    }


    // ********************************************************************************************************
    // ********************************************************************************************************
    //                                              GENERAL/HELPER
    // ********************************************************************************************************
    // ********************************************************************************************************

    private void Update()
    {
        // accommodation
        if (accommodationEnabled)
        {
            if (dofVolume.gameObject.activeSelf)
            {
                UpdateFocusTarget();
                SmoothFocus();
                UpdateDOFValues();
            }
        }

        // convergence
        if (!convergenceEnabled)
        {
            UpdateDiplopiaEffect();
        }
    }

    public void RestoreNormalView()
    {
        if (!disparityEnabled)
            ToggleDisparity();
        if (!convergenceEnabled)
            ToggleConvergence();
        if (accommodationEnabled)
            ToggleAccommodation();
        if (!motionParallaxEnabled)
            ToggleMotionParallax();
        if (accretionEnabled)
            ToggleAccretion();
        if (knownSizeEnabled)
            ToggleKnownSize();
        if (atmosphericPerspectiveEnabled)
            ToggleAtmosphericPerspective();
    }

    public void ToggleVisibility()
    {
        // normal eye = visible, closed/crossed eye = invisible
        SwitchHideUIEyeIcons();

        // only hide ddo not dedactivate this gameobject, update needs to run
        if (openedEyesIconUI.activeSelf)
        {
            StartCoroutine(ShowUI(gameObject));
            if (CheckIfCompleted())
            {
                StartCoroutine(ShowUI(nextButtonUI));
            }
        }
        else
        {
            StartCoroutine(HideUIButKeepActive(gameObject));
            if (CheckIfCompleted())
            {
                StartCoroutine(HideUI(nextButtonUI));
            }
        }
    }

    public void SwitchHideUIEyeIcons()
    {
        openedEyesIconUI.SetActive(!openedEyesIconUI.activeSelf);
        closedEyesIconUI.SetActive(!closedEyesIconUI.activeSelf);
    }

    private IEnumerator ShowUI(GameObject objectToShow)
    {
        objectToShow.SetActive(true);
        objectToShow.GetComponent<Animator>().SetTrigger("show");
        yield return null;
    }

    private IEnumerator HideUIButKeepActive(GameObject objectToHide)
    {
        objectToHide.GetComponent<Animator>().SetTrigger("hide");
        yield return null;
    }

    private IEnumerator HideUI(GameObject objectToHide)
    {
        objectToHide.GetComponent<Animator>().SetTrigger("hide");
        yield return new WaitForSeconds(.5f);
        objectToHide.SetActive(false);
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
        shadowCastWasToggled = true;
        shapeFromShadingWasToggled = true;
        occlusionWasToggled = true;
        disparityWasToggled = false;
        motionParallaxWasToggled = true;
        atmosphericPerspectiveWasToggled = true;
        relativeSizeWasToggled = true;
        knownSizeWasToggled = true;
        heightInFieldOfViewWasToggled = true;
        accommodationWasToggled = true;
        convergenceWasToggled = true;
        imageBlurWasToggled = true;
        textureGradientWasToggled = true;
        linearPerspectiveWasToggled = true;
        accretionWasToggled = true;
    }

    /// <summary>
    /// Whether or not all depth cues were toggled each at least once.
    /// </summary>
    public bool CheckIfCompleted()
    {
        if (shadowCastWasToggled && shapeFromShadingWasToggled && occlusionWasToggled && disparityWasToggled &&
            motionParallaxWasToggled && atmosphericPerspectiveWasToggled && relativeSizeWasToggled &&
            knownSizeWasToggled && heightInFieldOfViewWasToggled && accommodationWasToggled && convergenceWasToggled &&
            imageBlurWasToggled && textureGradientWasToggled && linearPerspectiveWasToggled && accretionWasToggled)
        {
            if (!nextButtonUI.activeSelf)
            {
                StartCoroutine(ShowUI(nextButtonUI));
            }
            return true;
        }
        return false;
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

        if (!occlusionEnabled)
        {
            foreach (Renderer rend in allRenderers)
            {
                RemoveOcclusionFromRenderer(rend);
            }
        }
        else
        {
            if (shapeFromShadingEnabled)
            {
                foreach (Renderer rend in originalMaterials.Keys)
                {
                    rend.material = new Material(originalMaterials[rend]);
                    if (!textureGradientEnabled)
                    {
                        rend.material.mainTexture = null;
                    }
                }
            }
            else
            {
                foreach (Renderer rend in originalMaterials.Keys)
                {
                    rend.material = new Material(unlitCache[rend]);
                    if (!textureGradientEnabled)
                    {
                        rend.material.mainTexture = null;
                    }
                }
            }
        }

        Debug.Log("Occlusion " + (occlusionEnabled ? "enabled" : "disabled"));
    }

    public void RemoveOcclusionFromRenderer(Renderer rend)
    {
        Material originalMaterial = rend.material;
        Material transparentMaterial;
        if (shapeFromShadingEnabled)
        {
            transparentMaterial = new Material(occlusionMaterialLit);
        }
        else
        {
            transparentMaterial = new Material(occlusionMaterialUnlit);
        }

        Color c = originalMaterial.color;
        c.a = occlusionAlpha;
        transparentMaterial.color = c;

        // texture
        if (textureGradientEnabled && originalMaterial.mainTexture != null)
        {
            transparentMaterial.mainTexture = originalMaterial.mainTexture;
            transparentMaterial.mainTextureScale = originalMaterial.mainTextureScale;
            transparentMaterial.mainTextureOffset = originalMaterial.mainTextureOffset;
        }

        rend.material = transparentMaterial;
    }

    // Old attempt: RenderFeature, Quick switching of layers to create flicker/always visible effect
    public IEnumerator ChangeRenderLayer()
    {
        List<Renderer> occlusionRenderers = new List<Renderer>();
        while (!occlusionEnabled)
        {
            for (int i = 0; i < occlusionRenderers.Count(); i++)
            {
                occlusionRenderers[i].gameObject.layer = LayerMask.NameToLayer("onTop");
                yield return new WaitForSeconds(0.0005f);
                occlusionRenderers[i].gameObject.layer = LayerMask.NameToLayer("Default");
            }
        }
    }

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
            
        }
        else
        {
            
        }

        Debug.Log("Convergence: " + (convergenceEnabled ? "ENABLED" : "DISABLED"));
    }

    public void UpdateDiplopiaEffect()
    {
        Vector3 center = (leftEye.position + rightEye.position) / 2f;
        Vector3 objPos = transform.position;

        float distanceToCenter = Vector3.Distance(objPos, center);

        // 0 wenn weiter als minDistance, 1 wenn direkt am Kopf
        float proximityFactor = Mathf.Clamp01(1f - (distanceToCenter / minDistance));

        // Smooth Übergang
        proximityFactor = Mathf.SmoothStep(0f, 1f, proximityFactor);

        Vector3 eyeAxis = (rightEye.position - leftEye.position).normalized;
        float horizontalDistance = Vector3.Dot(objPos - center, eyeAxis);

        // Maximale Stärke wenn exakt mittig
        float centerFactor = 1f - Mathf.Clamp01(Mathf.Abs(horizontalDistance) / minDistance);

        float diplopiaAmount = maxOffset * proximityFactor * centerFactor;

        Vector3 eyeOffset = eyeAxis * diplopiaAmount;

        leftEyeObj.position = transform.position - eyeOffset;
        rightEyeObj.position = transform.position + eyeOffset;
    }

    public void InitConvergence()
    {
        if (ovrCameraRig == null) { Debug.LogError("OVRCameraRig fehlt!"); enabled = false; return; }
        leftEye = ovrCameraRig.leftEyeAnchor;
        rightEye = ovrCameraRig.rightEyeAnchor;
        if (leftEyeObj == null || rightEyeObj == null) { Debug.LogError("Child-Objekte fehlen!"); enabled = false; return; }


        //nearAreaCollisionCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //nearAreaCollisionCube.GetComponent<Renderer>().enabled = true;
        //nearAreaCollisionCube.GetComponent<Collider>().isTrigger = true;

        //nearAreaCollisionCube.transform.parent = cam.gameObject.transform;

        //nearAreaCollisionCube.transform.localScale = nearAreaCollisionCubeScale;
        //nearAreaCollisionCube.transform.localPosition = new Vector3(0, 0, nearAreaCollisionCubeScale.z/2);
    }

    //public void CreateBlurClone(GameObject originalObject)
    //{
    //    if (blurMaterial == null)
    //    {
    //        Debug.LogError("BlurMaterial is null!");
    //        return;
    //    }

    //    // Alle MeshRenderer inklusive inaktiver Objekte holen
    //    MeshRenderer renderer = originalObject.GetComponentInChildren<MeshRenderer>();
    //    MeshFilter originalFilter = originalObject.GetComponentInChildren<MeshFilter>();

    //    if (originalFilter == null || originalFilter.sharedMesh == null)
    //        return;

    //    // ----- Neuen visuellen Klon erzeugen -----
    //    GameObject clone = new GameObject(originalObject.name + "_BlurClone");
    //    clone.tag = "BlurClone";

    //    // Parent setzen (wichtig für parallele Bewegung)
    //    clone.transform.SetParent(originalObject.transform, false);

    //    // Lokale Transformwerte kopieren
    //    clone.transform.localPosition = Vector3.zero;
    //    clone.transform.localRotation = Quaternion.identity;
    //    clone.transform.localScale = Vector3.one;

    //    // Nur notwendige Komponenten hinzufügen
    //    MeshFilter newFilter = clone.AddComponent<MeshFilter>();
    //    MeshRenderer newRenderer = clone.AddComponent<MeshRenderer>();

    //    // Mesh referenzieren (NICHT duplizieren → Speicher sparen)
    //    newFilter.sharedMesh = originalFilter.sharedMesh;

    //    // Blur-Material zuweisen
    //    Material blurmaterial_clone = new Material(blurMaterial);
    //    newRenderer.sharedMaterial = blurmaterial_clone;


    //    // Optional: gleiche Shadow Settings übernehmen
    //    //newRenderer.shadowCastingMode = originalRenderer.shadowCastingMode;
    //    //newRenderer.receiveShadows = originalRenderer.receiveShadows;
    //    newRenderer.shadowCastingMode = ShadowCastingMode.Off;
    //    newRenderer.receiveShadows = false;

    //    // Optional: Layer übernehmen (falls Blur Shader Layer nutzt)
    //    //clone.layer = originalGO.layer;

    //    blurClones.Add(clone);
    //    clone.GetComponent<MeshRenderer>().enabled = false;
    //}

    //public void UpdateBlurClones()
    //{
    //    foreach (GameObject clone in blurClones)
    //    {
    //        float distance = Vector3.Distance(Camera.main.transform.position, this.transform.position);
    //        if (distance > convergenceMaxDistance)
    //        {
    //            clone.GetComponent<MeshRenderer>().enabled = false;
    //        }
    //        else
    //        {
    //            clone.GetComponent<MeshRenderer>().enabled = true;
    //            float powerMin = 0f;
    //            float powerMax = 0.007f;

    //            // Normalisieren und auf Shaderbereich mappen
    //            float normalized = Mathf.InverseLerp(convergenceMaxDistance, 0.2f, distance); // 2m -> 0, 0m -> 1
    //            float power = Mathf.Lerp(powerMin, powerMax, normalized);

    //            // Shader setzen
    //            Renderer rend = clone.GetComponent<Renderer>();
    //            if (rend != null)
    //            {
    //                rend.material.SetFloat("_Power", power);
    //                Debug.Log(clone.name + " wird gesetzt auf " + power);
    //            }
    //        }
    //    }
    //}

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
            SetFarPointState();
        }
        else
        {
            // setting the enabled variable is enough, rest in Update
        }

        Debug.Log("Accommodation: " + (accommodationEnabled ? "ENABLED" : "DISABLED"));
    }

    public void InitAccommodation()
    {
        GameObject volumeObj = new GameObject("DOFVolume");
        dofVolume = volumeObj.AddComponent<Volume>();
        dofVolume.isGlobal = true;

        VolumeProfile profile = ScriptableObject.CreateInstance<VolumeProfile>();
        dof = profile.Add<DepthOfField>(true);
        dof.mode.value = DepthOfFieldMode.Bokeh;
        SetFarPointState();

        dofVolume.profile = profile;

        dof.mode.value = DepthOfFieldMode.Bokeh;
        dof.focusDistance.value = maxFocusDistance;

        currentFocusDistance = maxFocusDistance;
        targetFocusDistance = maxFocusDistance;

        dofVolume.gameObject.SetActive(false);
    }

    public void SetFarPointState()
    {
        // 6m or infinity is farpoint for people with normal eye vision
        dof.focusDistance.value = 6;
        dof.aperture.value = 16;
        dof.focalLength.value = 90;
        dof.bladeCount.value = bladeCount;
    }

    void UpdateFocusTarget()
    {
        Vector3 origin = cam.transform.position;
        Vector3 forward = Quaternion.Euler(rayDownwardAngle, 0f, 0f) * cam.transform.forward;
        float maxDist = maxFocusDistance;

        // 1️⃣ Primärer Raycast
        Ray ray = new Ray(origin, forward);
        //RenderRaycast(ray.origin, ray.direction, maxDist);

        if (Physics.Raycast(ray, out RaycastHit hit, maxDist, focusLayer))
        {
            targetFocusDistance = hit.distance;
            return;
        }

        // 2️⃣ Cone-Toleranz: Backup Fokus
        Collider[] candidates = Physics.OverlapSphere(origin + forward * maxDist * 0.5f, maxDist * 0.5f, focusLayer);
        float closestAngle = coneAngle;
        float closestDistance = maxFocusDistance;
        bool found = false;

        foreach (var col in candidates)
        {
            Vector3 toTarget = (col.bounds.center - origin).normalized;
            float angle = Vector3.Angle(forward, toTarget);
            if (angle < closestAngle)
            {
                float distance = Vector3.Distance(origin, col.bounds.center);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    targetFocusDistance = distance;
                    found = true;
                }
            }
        }

        if (!found)
            targetFocusDistance = maxFocusDistance;
    }

    void SmoothFocus()
    {
        currentFocusDistance = Mathf.SmoothDamp(currentFocusDistance, targetFocusDistance, ref focusVelocity, focusSmoothTime);
    }

    void UpdateDOFValues()
    {
        float aperture;

        if (currentFocusDistance <= nearSoftDistance)
        {
            float t = currentFocusDistance / nearSoftDistance; // 0 = nah, 1 = 2m
            aperture = Mathf.Lerp(maxAperture, minAperture, t * t);
        }
        else
        {
            aperture = minAperture;
        }

        float focusDist = (currentFocusDistance > nearSoftDistance) ? maxFocusDistance : currentFocusDistance;

        dof.focusDistance.value = focusDist;
        dof.aperture.value = aperture;
        dof.focalLength.value = focalLength;
        dof.bladeCount.value = bladeCount;
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
            dofVolume.gameObject.SetActive(false);
        }
        else
        {
            dofVolume.gameObject.SetActive(true);
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

        foreach (Renderer rend in originalMaterials.Keys)
        {
            if (!rend) 
                continue;

            Material textureGradientMaterial = new Material(rend.material);
            Material originalMat = originalMaterials[rend];

            if (textureGradientEnabled)
            {
                if (originalMat.mainTexture != null)
                {
                    textureGradientMaterial.mainTexture = originalMat.mainTexture;
                    textureGradientMaterial.mainTextureScale = originalMat.mainTextureScale;
                    textureGradientMaterial.mainTextureOffset = originalMat.mainTextureOffset;
                }
            }
            else
            {
                textureGradientMaterial.mainTexture = null;
            }
            rend.material = textureGradientMaterial;
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
                //relSizeObject.SetActive(false);
            }
            relativeSizeObjects[1].transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
            relativeSizeObjects[2].transform.localScale = new Vector3(2f, 2f, 2f);
        }
        else
        {
            foreach (GameObject relSizeObject in relativeSizeObjects)
            {
                //relSizeObject.SetActive(true);
            }
            relativeSizeObjects[1].transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            relativeSizeObjects[2].transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
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
                //heightInFieldOfViewObject.SetActive(false);
            }

            heightInFieldOfViewObjects[0].transform.position = new Vector3 (
                heightInFieldOfViewObjects[0].transform.position.x,
                2.16f,
                heightInFieldOfViewObjects[0].transform.position.z);

            heightInFieldOfViewObjects[1].transform.position = new Vector3(
                heightInFieldOfViewObjects[1].transform.position.x,
                3.16f,
                heightInFieldOfViewObjects[1].transform.position.z);

            heightInFieldOfViewObjects[2].transform.position = new Vector3(
                heightInFieldOfViewObjects[2].transform.position.x,
                20,
                heightInFieldOfViewObjects[2].transform.position.z);

            heightInFieldOfViewObjects[3].transform.position = new Vector3(
                heightInFieldOfViewObjects[3].transform.position.x,
                30,
                heightInFieldOfViewObjects[3].transform.position.z);
        }
        else
        {
            foreach (GameObject heightInFieldOfViewObject in heightInFieldOfViewObjects)
            {
                //heightInFieldOfViewObject.SetActive(true);
            }

            heightInFieldOfViewObjects[0].transform.position = new Vector3(
                heightInFieldOfViewObjects[0].transform.position.x,
                0.169f,
                heightInFieldOfViewObjects[0].transform.position.z);

            heightInFieldOfViewObjects[1].transform.position = new Vector3(
                heightInFieldOfViewObjects[1].transform.position.x,
                0.56f,
                heightInFieldOfViewObjects[1].transform.position.z);

            heightInFieldOfViewObjects[2].transform.position = new Vector3(
                heightInFieldOfViewObjects[2].transform.position.x,
                4.1f,
                heightInFieldOfViewObjects[2].transform.position.z);

            heightInFieldOfViewObjects[3].transform.position = new Vector3(
                heightInFieldOfViewObjects[3].transform.position.x,
                55,
                heightInFieldOfViewObjects[3].transform.position.z);
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

        if (shapeFromShadingEnabled)
        {
            foreach (Renderer rend in originalMaterials.Keys)
            {
                rend.material = new Material(originalMaterials[rend]);
                if (!occlusionEnabled)
                {
                    RemoveOcclusionFromRenderer(rend);
                }
                if (!textureGradientEnabled)
                {
                    rend.material.mainTexture = null;
                }
            }
        }
        else
        {
            foreach (Renderer rend in originalMaterials.Keys)
            {
                rend.material = new Material(unlitCache[rend]);
                if (!occlusionEnabled)
                {
                    RemoveOcclusionFromRenderer(rend);
                }
                if (!textureGradientEnabled)
                {
                    rend.material.mainTexture = null;
                }
            }
        }

        Debug.Log("Shape From Shading " + (shapeFromShadingEnabled ? "enabled (Lit)" : "disabled (Unlit)"));
    }

    public Material CreateNoShapeFromShadingMaterial(Renderer original)
    {
        if (original.gameObject.CompareTag("Ground") || original.gameObject.CompareTag("ShadowClone")
            || original.gameObject.CompareTag("BlurClone"))
            return null;

        // save original
        if (!originalMaterials.ContainsKey(original))
            originalMaterials[original] = original.material;

        Material unlit = new Material(Shader.Find("Universal Render Pipeline/Unlit"));

        if (textureGradientEnabled && original.material.mainTexture != null)
        {
            unlit.mainTexture = original.material.mainTexture;
            unlit.mainTextureScale = original.material.mainTextureScale;
            unlit.mainTextureOffset = original.material.mainTextureOffset;
        }

        Color c = original.material.color;
        unlit.color = c;

        // save unlit material
        if (!unlitCache.ContainsKey(original))
            unlitCache[original] = unlit;

        return unlit;
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

        if (!shadowCastEnabled)
        {
            foreach (GameObject clone in shadowClones)
            {
                clone.SetActive(false);
            }
        }
        else
        {
            foreach (GameObject clone in shadowClones)
            {
                clone.SetActive(true);
            }
        }

        Debug.Log("Shadow Cast " + (shadowCastEnabled ? "enabled" : "disabled"));
    }

    public void CreateShadowClone(GameObject originalObject)
    {
        // Alle MeshRenderer inklusive inaktiver Objekte holen
        MeshRenderer renderer = originalObject.GetComponentInChildren<MeshRenderer>();
        MeshFilter originalFilter = originalObject.GetComponentInChildren<MeshFilter>();

        if (originalFilter == null || originalFilter.sharedMesh == null)
            return;

        // ----- Neuen visuellen Klon erzeugen -----
        GameObject clone = new GameObject(originalObject.name + "_ShadowClone");
        clone.tag = "ShadowClone";

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

        newRenderer.sharedMaterial = new Material(renderer.material);

        newRenderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
        newRenderer.receiveShadows = false;

        shadowClones.Add(clone);
        renderer.shadowCastingMode = ShadowCastingMode.Off;
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
            foreach (AccretionSpawner accretionObject in accretionObjects)
            {
                accretionObject.enabled = false;
            }
        }
        else
        {
            foreach (AccretionSpawner accretionObject in accretionObjects)
            {
                accretionObject.enabled = true;
            }
        }

        Debug.Log("Accretion: " + (accretionEnabled ? "ENABLED" : "DISABLED"));
    }
}
