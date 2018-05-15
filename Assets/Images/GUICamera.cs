using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class GUICamera : MonoBehaviour {

    public float intensity = 0.375f;
    public float blur = 0.1f;
    public float blurSpread = 1.5f;
    public float chromaticAberration = 0.2f;

    public Shader chromAberrationShader;
    private Material chromAberrationMaterial;

    public Shader separableBlurShader;
    private Material separableBlurMaterial;

    public Shader vignetteShader;
    private Material vignetteMaterial;
    void Awake() {
        cam = GetComponent<Camera>();
        chromAberrationMaterial = new Material(chromAberrationShader);
        chromAberrationMaterial.hideFlags = HideFlags.DontSave;
        separableBlurMaterial = new Material(separableBlurShader);
        separableBlurMaterial.hideFlags = HideFlags.DontSave;
        vignetteMaterial = new Material(vignetteShader);
        vignetteMaterial.hideFlags = HideFlags.DontSave;
    }

    private RenderTexture display;
    public Camera cam;
    public Material material;

    void OnPreRender() {

        CleanRenderTextures();
        // display = RenderTexture.GetTemporary((int)camera.pixelWidth,
        //           (int)camera.pixelHeight, 16, RenderTextureFormat.ARGB32);
        // cam.targetTexture = display;
        cam.backgroundColor = new Color(0, 0, 0, 0);
        cam.clearFlags = CameraClearFlags.SolidColor;
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination) {
        // Graphics.Blit (source, destination);


        float widthOverHeight = (1.0f * source.width) / (1.0f * source.height);
        float oneOverBaseSize = 1.0f / 512.0f;

        RenderTexture color = RenderTexture.GetTemporary(source.width, source.height, 0);
        RenderTexture color2 = RenderTexture.GetTemporary(source.width, source.height, 0);

        RenderTexture halfRezColor = RenderTexture.GetTemporary((int)(source.width / 2.0), (int)(source.height / 2.0), 0);
        RenderTexture quarterRezColor = RenderTexture.GetTemporary((int)(source.width / 4.0), (int)(source.height / 4.0), 0);
        RenderTexture secondQuarterRezColor = RenderTexture.GetTemporary((int)(source.width / 4.0), (int)(source.height / 4.0), 0);

        Graphics.Blit(source, halfRezColor, chromAberrationMaterial, 0);
        Graphics.Blit(halfRezColor, quarterRezColor);

        for (int it = 0; it < 2; it++) {
            separableBlurMaterial.SetVector("offsets", new Vector4(0.0f, blurSpread * oneOverBaseSize, 0.0f, 0.0f));
            Graphics.Blit(quarterRezColor, secondQuarterRezColor, separableBlurMaterial);
            separableBlurMaterial.SetVector("offsets", new Vector4(blurSpread * oneOverBaseSize / widthOverHeight, 0.0f, 0.0f, 0.0f));
            Graphics.Blit(secondQuarterRezColor, quarterRezColor, separableBlurMaterial);
        }

        vignetteMaterial.SetFloat("_Intensity", intensity);
        vignetteMaterial.SetFloat("_Blur", blur);
        vignetteMaterial.SetTexture("_VignetteTex", quarterRezColor);
        Graphics.Blit(source, color, vignetteMaterial);
        // Graphics.Blit (source, halfRezColor, chromAberrationMaterial, 0);
        chromAberrationMaterial.SetFloat("_ChromaticAberration", chromaticAberration);

        Graphics.Blit(color, color2, chromAberrationMaterial, 1);
        Graphics.Blit(color2, destination, material);
        // RenderTexture.active = destination;

        // material.SetTexture("_DepthNormal", display);
        // ImageEffects.BlitWithMaterial(material, source, destination);

        // get shaders to do their shit here
        RenderTexture.ReleaseTemporary(quarterRezColor);
        RenderTexture.ReleaseTemporary(secondQuarterRezColor);
        RenderTexture.ReleaseTemporary(halfRezColor);
        RenderTexture.ReleaseTemporary(color);
        RenderTexture.ReleaseTemporary(color2);
        CleanRenderTextures();
    }

    void CleanRenderTextures() {
        if (display != null) {
            RenderTexture.ReleaseTemporary(display);
            display = null;
        }

    }

}