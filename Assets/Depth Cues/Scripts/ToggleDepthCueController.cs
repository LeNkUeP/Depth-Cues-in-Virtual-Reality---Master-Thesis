using Oculus.Interaction.Locomotion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ToggleDepthCueController : MonoBehaviour
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
    public GameObject sceneEnvironment;
    private Camera cam;
    private Renderer[] allRenderers;

    [Header("Disparity - Settings")]
    // nothing, just sets camera scale to 0

    [Header("Motion parallax - Settings")]
    private OVRCameraRig cameraRig;
    private FirstPersonLocomotor locomotor;

    [Header("Accretion - Settings")]
    public AccretionSpawner[] accretionSpawners;

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

    [Header("Convergence - Settings")]
    public Vector3 convergenceCameraScale = new Vector3(2.5f, 2.5f, 2.5f);
    public float convergenceFocusTreshold = .5f;

    [Header("Image Blur - Settings")]
    private Volume dofVolumeGaussian;
    private DepthOfField dofGaussian;

    [Header("Atmospheric Perspective - Settings")]
    [Range(0, 1)]
    public float fogDensity = 0.003f;
    public Color fogColor = new Color(76, 185, 200);

    [Header("Texture gradient - Settings")]
    // nothing, only needs to save original textures and rebuild them

    [Header("Linear perspective - Settings")]
    public GameObject orthographicCamera;

    [Header("Shadow cast - Settings")]
    private List<GameObject> shadowClones = new List<GameObject>();

    [Header("Shape from shading - Settings")]
    private Dictionary<Renderer, Material> originalMaterials = new Dictionary<Renderer, Material>();
    private Dictionary<Renderer, Material> unlitCache = new Dictionary<Renderer, Material>();

    [Header("Relative size - Settings")]
    private Dictionary<Transform, Vector3> originalRelativeScales = new Dictionary<Transform, Vector3>();

    private Dictionary<string, List<Transform>> relativeSizeGroups = new Dictionary<string, List<Transform>>();

    [Header("Known size - Settings")]
    public GameObject[] knownSizeObjects;

    [Header("Height in field of view - Settings")]
    public float visualHeight = 0f; // exact eye level
    public bool useSmoothTransition = false;
    public float smoothSpeed = 5f;

    private Dictionary<Transform, Vector3> originalHeightPositions = new Dictionary<Transform, Vector3>();





    // ********************************************************************************************************
    // ********************************************************************************************************
    //                                          INITIALIZATION
    // ********************************************************************************************************
    // ********************************************************************************************************





    public void Start()
    {
        // general
        cam = Camera.main;

        // motion parallax
        cameraRig = FindFirstObjectByType<OVRCameraRig>();
        locomotor = FindFirstObjectByType<FirstPersonLocomotor>();

        // accommodadtion
        InitAccommodation();

        // init environment related stuff
        InitEnvironment();
    }

    public void InitEnvironment()
    {
        allRenderers = sceneEnvironment.GetComponentsInChildren<Renderer>(true);

        foreach (Renderer rend in allRenderers)
        {
            CreateNoShapeFromShadingMaterial(rend);
            CreateShadowClone(rend.gameObject);
        }

        // relative size, groups "equal" objects to manipulate relative scale
        BuildRelativeSizeGroups();
    }





    // ********************************************************************************************************
    // ********************************************************************************************************
    //                                              GENERAL
    // ********************************************************************************************************
    // ********************************************************************************************************





    private void Update()
    {
        // check current focus target, update dof volume
        if (accommodationEnabled)
        {
            if (dofVolume.gameObject.activeSelf)
            {
                UpdateFocusTarget();
                SmoothFocus();
                UpdateDOFValues();
            }
        }

        // keep all objects at visual height until detoggled
        if (!heightInFieldOfViewEnabled)
        {
            AlignObjectsAtVisualHeight();
        }

        // scale all "equal" objects relative to distance until detoggled, also needed for linear perspective
        if (!relativeSizeEnabled || !linearPerspectiveEnabled)
        {
            ScaleRelativeSizeGroups();
        }
    }

    public void DisableAllCues()
    {
        if (occlusionEnabled)
            ToggleOcclusion();
        if (disparityEnabled)
            ToggleDisparity();
        if (convergenceEnabled)
            ToggleConvergence();
        if (accommodationEnabled)
            ToggleAccommodation();
        if (imageBlurEnabled)
            ToggleImageBlur();
        if (linearPerspectiveEnabled)
            ToggleLinearPerspective();
        if (textureGradientEnabled)
            ToggleTextureGradient();
        if (relativeSizeEnabled)
            ToggleRelativeSize();
        if (knownSizeEnabled)
            ToggleKnownSize();
        if (heightInFieldOfViewEnabled)
            ToggleHeightInFieldOfView();
        if (atmosphericPerspectiveEnabled)
            ToggleAtmosphericPerspective();
        if (shapeFromShadingEnabled)
            ToggleShapeFromShading();
        if (shadowCastEnabled)
            ToggleShadowCast();
        if (motionParallaxEnabled)
            ToggleMotionParallax();
        if (accretionEnabled)
            ToggleAccretion();
    }

    public void RestoreNormalView()
    {
        if (!occlusionEnabled)
            ToggleOcclusion();
        if (!disparityEnabled)
            ToggleDisparity();
        if (!convergenceEnabled)
            ToggleConvergence();
        if (accommodationEnabled)
            ToggleAccommodation();
        if (imageBlurEnabled)
            ToggleImageBlur();
        if (!linearPerspectiveEnabled)
            ToggleLinearPerspective();
        if (!textureGradientEnabled)
            ToggleTextureGradient();
        if (!relativeSizeEnabled)
            ToggleRelativeSize();
        if (knownSizeEnabled)
            ToggleKnownSize();
        if (!heightInFieldOfViewEnabled)
            ToggleHeightInFieldOfView();
        if (atmosphericPerspectiveEnabled)
            ToggleAtmosphericPerspective();
        if (!shapeFromShadingEnabled)
            ToggleShapeFromShading();
        if (!shadowCastEnabled)
            ToggleShadowCast();
        if (!motionParallaxEnabled)
            ToggleMotionParallax();
        if (accretionEnabled)
            ToggleAccretion();
    }

    public void SetStartingCueState()
    {
        if (!occlusionEnabled)
        {
            occlusionEnabled = true;
            ToggleOcclusion();
        }
        if (!disparityEnabled)
        {
            disparityEnabled = true;
            ToggleDisparity();
        }
        if (!convergenceEnabled)
        {
            convergenceEnabled = true;
            ToggleConvergence();
        }
        if (accommodationEnabled)
        {
            accommodationEnabled = false;
            ToggleAccommodation();
        }
        if (imageBlurEnabled)
        {
            imageBlurEnabled = false;
            ToggleImageBlur();
        }
        if (!linearPerspectiveEnabled)
        {
            linearPerspectiveEnabled = true;
            ToggleLinearPerspective();
        }
        if (!textureGradientEnabled)
        {
            textureGradientEnabled = true;
            ToggleTextureGradient();
        }
        if (!relativeSizeEnabled)
        {
            relativeSizeEnabled = true;
            ToggleRelativeSize();
        }
        if (!knownSizeEnabled)
        {
            knownSizeEnabled = true;
            ToggleKnownSize();
        }
        if (!heightInFieldOfViewEnabled)
        {
            heightInFieldOfViewEnabled = true;
            ToggleHeightInFieldOfView();
        }
        if (!atmosphericPerspectiveEnabled)
        {
            atmosphericPerspectiveEnabled = true;
            ToggleAtmosphericPerspective();
        }
        if (!shapeFromShadingEnabled)
        {
            shapeFromShadingEnabled = true;
            ToggleShapeFromShading();
        }
        if (!shadowCastEnabled)
        {
            shadowCastEnabled = true;
            ToggleShadowCast();
        }
        if (!motionParallaxEnabled)
        {
            motionParallaxEnabled = true;
            ToggleMotionParallax();
        }
        if (!accretionEnabled)
        {
            accretionEnabled = true;
            ToggleAccretion();
        }
        else
        {
            // Wait for scene animation end (see EnvironmentAnimationController)
            StartCoroutine(StartAccretionAfterDelay());
        }
    }

    public IEnumerator StartAccretionAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        accretionEnabled = false;
        ToggleAccretion();
    }





    // ********************************************************************************************************
    // ********************************************************************************************************
    // ********************************************************************************************************
    //                                   ---DEPTH CUE IMPLEMENTATION---
    // ********************************************************************************************************
    // ********************************************************************************************************
    // ********************************************************************************************************







    // ********************************************************************************************************
    // ********************************************************************************************************
    // OCCLUSION
    // ********************************************************************************************************
    // ********************************************************************************************************

    public void ToggleOcclusion()
    {
        occlusionEnabled = !occlusionEnabled;

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
            cam.transform.localScale = Vector3.zero;
        }
        else
        {
            if (!convergenceEnabled)
            {
                cam.transform.localScale = convergenceCameraScale;
            }
            else
            {
                cam.transform.localScale = Vector3.one;
            }
        }
    }


    // ********************************************************************************************************
    // ********************************************************************************************************
    // CONVERGENCE
    // ********************************************************************************************************
    // ********************************************************************************************************

    public void ToggleConvergence()
    {
        convergenceEnabled = !convergenceEnabled;

        // increase camera scale for increased diplopia effect
        if (!convergenceEnabled)
        {
            if (disparityEnabled)
            {
                cam.transform.localScale = convergenceCameraScale;
            }
        }
        else
        {
            if (disparityEnabled)
            {
                cam.transform.localScale = Vector3.one;
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
        accommodationEnabled = !accommodationEnabled;

        if (!accommodationEnabled)
        {
            SetFarPointState();
        }
        else
        {
            // setting the enabled variable is enough, rest in Update
        }
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

            // convergence disabled -> restrict focus area (near focus not possible)
            if (disparityEnabled && !convergenceEnabled && targetFocusDistance < convergenceFocusTreshold)
                targetFocusDistance = maxFocusDistance;

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

        // nothing found, or convergence disabled -> restrict focus area (near focus not possible)
        if (!found || (disparityEnabled && !convergenceEnabled && targetFocusDistance < convergenceFocusTreshold))
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
        imageBlurEnabled = !imageBlurEnabled;

        if (!imageBlurEnabled)
        {
            dofVolume.gameObject.SetActive(false);
        }
        else
        {
            dofVolume.gameObject.SetActive(true);
        }
    }


    // ********************************************************************************************************
    // ********************************************************************************************************
    // LINEAR PERSPECTIVE
    // ********************************************************************************************************
    // ********************************************************************************************************

    public void ToggleLinearPerspective()
    {
        linearPerspectiveEnabled = !linearPerspectiveEnabled;

        if (!linearPerspectiveEnabled)
        {
            // old 2d cam plane attempt -> really ugly and confusing
            //orthographicCamera.gameObject.SetActive(true);
            //cam.transform.localScale = Vector3.one;
            //cam.cullingMask = 1 << LayerMask.NameToLayer("OrthographicView");

            // deactivate visual for all ground(flat and wide) objects -> important cue to linear perspective
            // "IgnorePlayer" Layer are interactive items (pills, chess figure)
            foreach (Renderer rend in allRenderers)
            {
                if (rend.gameObject.CompareTag("NotAffectedByScaleOrPositionEffects") &&
                    rend.gameObject.layer != LayerMask.NameToLayer("Ignore Player"))
                {
                    rend.enabled = false;
                }
            }

            // objects should have equal size regardless of distance during absence of linear perspective
            if (relativeSizeEnabled)
            {
                linearPerspectiveEnabled = true;
                ToggleRelativeSize();
                linearPerspectiveEnabled = false;
                relativeSizeEnabled = true;
            }
        }
        else
        {
            //orthographicCamera.gameObject.SetActive(false);
            //cam.cullingMask = ~0;
            //if (!disparityEnabled)
            //{
            //    cam.transform.localScale = Vector3.zero;
            //}else if (!convergenceEnabled)
            //{
            //    cam.transform.localScale = convergenceCameraScale;
            //}

            // "IgnorePlayer" Layer are interactive items (pills, chess figure)
            foreach (Renderer rend in allRenderers)
            {
                if (rend.gameObject.CompareTag("NotAffectedByScaleOrPositionEffects") &&
                    rend.gameObject.layer != LayerMask.NameToLayer("Ignore Player"))
                {
                    rend.enabled = true;
                }
            }

            // reset relative size if not toggled otherwise during absence of linear perspective
            if (relativeSizeEnabled)
            {
                relativeSizeEnabled = false;
                ToggleRelativeSize();
            }
        }
    }


    // ********************************************************************************************************
    // ********************************************************************************************************
    // TEXTURE GRADIENT
    // ********************************************************************************************************
    // ********************************************************************************************************

    public void ToggleTextureGradient()
    {
        textureGradientEnabled = !textureGradientEnabled;

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
    }


    // ********************************************************************************************************
    // ********************************************************************************************************
    // RELATIVE SIZE
    // ********************************************************************************************************
    // ********************************************************************************************************

    public void ToggleRelativeSize()
    {
        relativeSizeEnabled = !relativeSizeEnabled;

        // if linear perspective is disabled -> relativeSize should be disabled all the time
        if (!linearPerspectiveEnabled)
        {
            return;
        }

        if (!relativeSizeEnabled)
        {
            StoreOriginalRelativeScales();
        }
        else
        {
            RestoreOriginalRelativeScales();
        }
    }

    private void BuildRelativeSizeGroups()
    {
        relativeSizeGroups.Clear();

        foreach (Transform tr in sceneEnvironment.transform)
        {
            GameObject go = tr.gameObject;
            if (go.CompareTag("NotAffectedByScaleOrPositionEffects"))
            {
                continue;
            }

            Renderer rend = go.GetComponentInChildren<Renderer>();
            SkinnedMeshRenderer smr = rend as SkinnedMeshRenderer;
            if (rend == null) continue;

            UnityEngine.Mesh mesh;
            MeshFilter mf = rend.GetComponent<MeshFilter>();

            if (mf != null && mf.sharedMesh != null)
            {
                mesh = mf.sharedMesh;
            }
            else if (smr != null && smr.sharedMesh != null)
            {
                mesh = smr.sharedMesh;
            }
            else
            {
                continue;
            }

            string meshName = mesh.name;
            string materialName = rend.sharedMaterial != null ? rend.sharedMaterial.name : "NoMat";
            Vector3 scale = rend.transform.localScale;

            bool isPrimitive = IsPrimitiveMesh(meshName);

            string groupKey;

            if (isPrimitive)
            {
                // Primitive: Mesh + Scale
                groupKey = meshName + "_" + scale.ToString();
            }
            else
            {
                // Normal Meshes: Mesh
                groupKey = meshName;
            }

            if (!relativeSizeGroups.ContainsKey(groupKey))
            {
                relativeSizeGroups[groupKey] = new List<Transform>();
            }

            relativeSizeGroups[groupKey].Add(go.transform);
        }
    }

    private bool IsPrimitiveMesh(string meshName)
    {
        meshName = meshName.ToLower();

        return meshName.Contains("cube") ||
               meshName.Contains("sphere") ||
               meshName.Contains("capsule") ||
               meshName.Contains("cylinder") ||
               meshName.Contains("plane") ||
               meshName.Contains("quad");
    }

    private void StoreOriginalRelativeScales()
    {
        originalRelativeScales.Clear();

        foreach (Transform tr in sceneEnvironment.transform)
        {
            GameObject go = tr.gameObject;
            if (go != null)
            {
                originalRelativeScales[go.gameObject.transform] = go.transform.localScale;
            }
        }
    }

    private void RestoreOriginalRelativeScales()
    {
        foreach (Transform tr in sceneEnvironment.transform)
        {
            GameObject go = tr.gameObject;
            if (go != null && originalRelativeScales.ContainsKey(go.transform))
            {
                go.transform.localScale = originalRelativeScales[go.transform];
            }
        }
    }

    private void ScaleRelativeSizeGroups()
    {
        Vector3 camPos = cam.transform.position;

        foreach (var group in relativeSizeGroups.Values)
        {
            if (group.Count < 2) continue;

            Transform reference = null;
            float minDistance = float.MaxValue;

            // use nearest object as reference
            foreach (Transform t in group)
            {
                float dist = Vector3.Distance(camPos, t.position);

                if (dist < minDistance)
                {
                    minDistance = dist;
                    reference = t;
                }
            }

            if (reference == null) continue;
            if (!originalRelativeScales.ContainsKey(reference)) continue;

            Vector3 referenceOriginalScale = originalRelativeScales[reference];
            float referenceDistance = minDistance;

            // scale according to nearest 
            foreach (Transform t in group)
            {
                float distance = Vector3.Distance(camPos, t.position);

                float scaleFactor = distance / referenceDistance;

                t.localScale = referenceOriginalScale * scaleFactor;
            }
        }
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
            StoreOriginalHeightPositions();
        }
        else
        {
            RestoreOriginalHeightPositions();
        }
    }

    private void StoreOriginalHeightPositions()
    {
        originalHeightPositions.Clear();

        foreach (Transform tr in sceneEnvironment.transform)
        {
            GameObject go = tr.gameObject;
            originalHeightPositions[go.transform] = go.transform.position;
        }
    }

    private void RestoreOriginalHeightPositions()
    {
        foreach (Transform tr in sceneEnvironment.transform)
        {
            GameObject go = tr.gameObject;
            if (originalHeightPositions.ContainsKey(go.transform))
            {
                // only y-value needed
                go.transform.position = new Vector3(go.transform.position.x,
                    originalHeightPositions[go.transform].y, go.transform.position.z);
            }
        }
    }

    public void AlignObjectsAtVisualHeight()
    {
        // Winkel zur gewünschten Höhe aus Sicht des Spielers
        float playerY = cam.transform.position.y;
        float targetAngle = Mathf.Atan2(visualHeight, 1f);
        // 1f = Referenzdistanz, der Winkel wird proportional skaliert. Alternativ: kann direkt in Meter umgesetzt werden.

        foreach (Transform tr in sceneEnvironment.transform)
        {
            GameObject go = tr.gameObject;
            Vector3 dir = tr.position - cam.transform.position;

            if (go.CompareTag("NotAffectedByScaleOrPositionEffects"))
            {
                continue;
            }

            // Horizontale Entfernung
            float horizontalDistance = new Vector3(dir.x, 0, dir.z).magnitude;

            // Ziel-Y berechnen anhand des Winkels
            float targetY = playerY + Mathf.Tan(targetAngle) * horizontalDistance;

            Vector3 newPos = new Vector3(tr.position.x, targetY, tr.position.z);

            if (useSmoothTransition)
            {
                tr.position = Vector3.Lerp(tr.position, newPos, Time.deltaTime * smoothSpeed);
            }
            else
            {
                tr.position = newPos;
            }
        }
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
    }


    // ********************************************************************************************************
    // ********************************************************************************************************
    // SHAPE FROM SHADING
    // ********************************************************************************************************
    // ********************************************************************************************************

    public void ToggleShapeFromShading()
    {
        shapeFromShadingEnabled = !shapeFromShadingEnabled;

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
    }

    public Material CreateNoShapeFromShadingMaterial(Renderer original)
    {
        if (original.gameObject.CompareTag("Ground") || original.gameObject.CompareTag("ShadowClone"))
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
        shadowCastEnabled = !shadowCastEnabled;

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
    }

    public void CreateShadowClone(GameObject originalObject)
    {
        if (originalObject.CompareTag("NotAffectedByScaleOrPositionEffects"))
        {
            return;
        }

        Renderer renderer = originalObject.GetComponentInChildren<Renderer>();
        SkinnedMeshRenderer smr = renderer as SkinnedMeshRenderer;

        UnityEngine.Mesh mesh;
        MeshFilter mf = renderer.GetComponent<MeshFilter>();
        if (mf != null && mf.sharedMesh != null)
        {
            mesh = mf.sharedMesh;
        }
        else if (smr != null && smr.sharedMesh != null)
        {
            mesh = smr.sharedMesh;
        }
        else
        {
            return;
        }

        GameObject clone = new GameObject(originalObject.name + "_ShadowClone");
        clone.tag = "ShadowClone";

        clone.transform.SetParent(originalObject.transform, false);

        clone.transform.localPosition = Vector3.zero;
        clone.transform.localRotation = Quaternion.identity;
        clone.transform.localScale = Vector3.one;

        // only necessary components
        MeshFilter newFilter = clone.AddComponent<MeshFilter>();
        MeshRenderer newRenderer = clone.AddComponent<MeshRenderer>();

        // reference mesh -> save memory
        newFilter.sharedMesh = mesh;

        newRenderer.sharedMaterial = new Material(renderer.material);

        newRenderer.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
        newRenderer.receiveShadows = false;

        shadowClones.Add(clone);

        // original shouldnt cast shadow
        renderer.shadowCastingMode = ShadowCastingMode.Off;
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
        }
        else
        {
            cameraRig.rotationOnlyTracking = false;
            locomotor.EnableMovement();
        }
    }


    // ********************************************************************************************************
    // ********************************************************************************************************
    // ACCRETION
    // ********************************************************************************************************
    // ********************************************************************************************************

    public void ToggleAccretion()
    {
        accretionEnabled = !accretionEnabled;

        if (!accretionEnabled)
        {
            foreach (AccretionSpawner accretionSpawner in accretionSpawners)
            {
                accretionSpawner.enabled = false;
            }
        }
        else
        {
            foreach (AccretionSpawner accretionSpawner in accretionSpawners)
            {
                accretionSpawner.enabled = true;
            }
        }
    }
}
