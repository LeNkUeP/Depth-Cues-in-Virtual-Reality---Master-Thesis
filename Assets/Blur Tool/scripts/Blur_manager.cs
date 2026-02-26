using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace Haipeng.blur
{
    public class Blur_manager : MonoBehaviour
    {
        public static Blur_manager instance;

        [Header("Is initialization successful?")]
        public bool is_initialization_successful;

        [Header("Canvas")]
        public Canvas canvas;

        [Header("Canvas scaler")]
        public CanvasScaler canvas_scaler;

        [Header("camera")]
        public Camera camera_blur;

        [Header("render_texture")]
        private RenderTexture render_texture;

        [Header("glicth material")]
        private Material material_glitch;

        [Header("material attribute")]
        [Range(0, 0.01f)]
        public float blur_strength = 0.003f;
        [Range(0, 1f)]
        public float brightness = 0.125f;


        void Awake()
        {
            if (Blur_manager.instance == null)
                Blur_manager.instance = this;

            int layer_outline_object = LayerMask.NameToLayer("blur_layer");
            if (layer_outline_object == -1)
            {
                this.is_initialization_successful = false;
                Debug.LogError("Please perform initialization settings first. Just click 'Tools/WuHaipeng / Initialization settings / add layer' in the menu bar.");
            }
            else
            {
                this.is_initialization_successful = true;

                //ser camera priority
                this.camera_blur.cullingMask = 1 << layer_outline_object;
                this.camera_blur.depth = Camera.main.depth - 1;

                //set render texture size
                this.render_texture = new RenderTexture(Screen.width, Screen.height, 24);
                this.camera_blur.targetTexture = this.render_texture;

                //set canvas
                this.canvas.renderMode = RenderMode.ScreenSpaceCamera;
                this.canvas.worldCamera = Camera.main;
                this.canvas_scaler.referenceResolution = new Vector2(Screen.width, Screen.height);

                //get the material, set the main texture
                this.material_glitch = this.canvas.transform.GetChild(0).GetComponent<RawImage>().material;
                this.material_glitch.SetTexture("_Main_texture", this.render_texture);
            }
        }

        void LateUpdate()
        {
            if (this.is_initialization_successful == false)
                return;

            //The camera position is the same as the main camera
            if (this.transform.position != Camera.main.transform.position)
                this.transform.position = Camera.main.transform.position;
            //The camera ratation is the same as the main camera

            if (this.transform.rotation != Camera.main.transform.rotation)
                this.transform.rotation = Camera.main.transform.rotation;

            //The camera field of view is the same as the main camera
            if (this.camera_blur.fieldOfView != Camera.main.fieldOfView)
                this.camera_blur.fieldOfView = Camera.main.fieldOfView;

            //set material attribute
            #region 
            if (this.material_glitch.GetFloat("_blur_strength") != this.blur_strength)
                this.material_glitch.SetFloat("_blur_strength", this.blur_strength);
            if (this.material_glitch.GetFloat("_brightness") != this.brightness)
                this.material_glitch.SetFloat("_brightness", this.brightness);
            #endregion
        }
    }
}
