using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GraphicsSettingsManager : MonoBehaviour
{
    public UniversalRenderPipelineAsset renderPipelineAsset;
    public Camera mainCamera;
    public VolumeProfile volumeProfile;

    public Button resetButton;

    public Dropdown resolutionDropdown;
    public Toggle fullScreenToggle;
    public Toggle vsyncToggle;
    public Dropdown graphicsQualityDropdown;
    public Dropdown antiAliasingDropdown;
    public Dropdown textureQualityDropdown;
    public Dropdown refreshRateDropdown;
    public Slider shadowDistanceSlider;
    public Text shadowDistanceText;
    public Dropdown softShadowQualityDropdown;

    public Toggle bloomToggle;
    public Toggle motionBlurToggle;
    public Toggle depthOfFieldToggle;

    private void Start()
    {
        // Populate dropdowns with options
        resolutionDropdown.AddOptions(GetAvailableResolutions());
        graphicsQualityDropdown.AddOptions(GetAvailableQualities());
        antiAliasingDropdown.AddOptions(GetAvailableAntiAliasingOptions());
        textureQualityDropdown.AddOptions(GetAvailableTextureQualities());
        refreshRateDropdown.AddOptions(GetAvailableRefreshRatesDistinct());
        softShadowQualityDropdown.AddOptions(GetAvailableSoftShadowQualities());

        // Set initial dropdown values to current settings
        resolutionDropdown.value = GetCurrentResolutionIndex();
        fullScreenToggle.isOn = Screen.fullScreen;
        vsyncToggle.isOn = QualitySettings.vSyncCount != 0;
        graphicsQualityDropdown.value = QualitySettings.GetQualityLevel();
        antiAliasingDropdown.value = renderPipelineAsset != null ? renderPipelineAsset.msaaSampleCount : 0;
        textureQualityDropdown.value = QualitySettings.globalTextureMipmapLimit;
        refreshRateDropdown.value = GetCurrentRefreshRateIndex();
        shadowDistanceSlider.value = renderPipelineAsset != null ? renderPipelineAsset.shadowDistance : 0;
        softShadowQualityDropdown.value = (int)renderPipelineAsset.shadowCascadeOption;
        shadowDistanceText.text = (renderPipelineAsset != null ? renderPipelineAsset.shadowDistance.ToString() : "0");

        // Set up dropdown onChange events
        resolutionDropdown.onValueChanged.AddListener(index => SetResolution(index));
        fullScreenToggle.onValueChanged.AddListener(value => SetFullScreen(value));
        vsyncToggle.onValueChanged.AddListener(value => SetVSync(value));
        graphicsQualityDropdown.onValueChanged.AddListener(index => SetGraphicsQuality(index));
        antiAliasingDropdown.onValueChanged.AddListener(index => SetAntiAliasingQuality(index));
        textureQualityDropdown.onValueChanged.AddListener(index => SetTextureQuality(index));
        refreshRateDropdown.onValueChanged.AddListener(index => SetRefreshRate(index));
        shadowDistanceSlider.onValueChanged.AddListener(value => SetShadowDistance(value));
        softShadowQualityDropdown.onValueChanged.AddListener(index => SetSoftShadowQuality(index));
        resetButton.onClick.AddListener(ResetToDefaults);

        // Get current post-process settings
        var bloom = volumeProfile.components.FirstOrDefault(x => x is Bloom) as Bloom;
        var motionBlur = volumeProfile.components.FirstOrDefault(x => x is MotionBlur) as MotionBlur;
        var vignette = volumeProfile.components.FirstOrDefault(x => x is Vignette) as Vignette;
        var depthOfField = volumeProfile.components.FirstOrDefault(x => x is DepthOfField) as DepthOfField;

        // Set initial toggle values
        bloomToggle.isOn = bloom != null && bloom.active;
        motionBlurToggle.isOn = motionBlur != null && motionBlur.active;
        depthOfFieldToggle.isOn = depthOfField != null && depthOfField.active;

        // Set up toggle onChange events
        bloomToggle.onValueChanged.AddListener(value => SetBloom(value));
        motionBlurToggle.onValueChanged.AddListener(value => SetMotionBlur(value));
        depthOfFieldToggle.onValueChanged.AddListener(value => SetDepthOfField(value));
    }

    private int GetCurrentResolutionIndex()
    {
        string currentResolution = Screen.width + "x" + Screen.height;
        return resolutionDropdown.options.FindIndex(option => option.text == currentResolution);
    }

    private int GetCurrentRefreshRateIndex()
    {
        string currentRefreshRate = Screen.currentResolution.refreshRate.ToString() + "Hz";
        return refreshRateDropdown.options.FindIndex(option => option.text == currentRefreshRate);
    }

    private List<string> GetAvailableResolutions()
    {
        HashSet<string> uniqueResolutions = new HashSet<string>();
        foreach (var res in Screen.resolutions)
        {
            uniqueResolutions.Add(res.width + "x" + res.height);
        }
        return uniqueResolutions.ToList();
    }

    private List<string> GetAvailableQualities()
    {
        return new List<string>(QualitySettings.names);
    }

    private List<string> GetAvailableAntiAliasingOptions()
    {
        return new List<string>() { "Disabled", "FXAA", "SMAA", "TSAA" };
    }

    private List<string> GetAvailableTextureQualities()
    {
        return new List<string>() { "Low", "Medium", "High" };
    }

    private List<string> GetAvailableRefreshRatesDistinct()
    {
        HashSet<string> refreshRates = new HashSet<string>();
        foreach (var rate in Screen.resolutions)
        {
            string refreshRate = rate.refreshRate.ToString() + "Hz";
            refreshRates.Add(refreshRate);
        }
        return new List<string>(refreshRates);
    }

    private List<string> GetAvailableSoftShadowQualities()
    {
        return new List<string>() { "No Cascades", "Two Cascades", "Four Cascades" };
    }

    private void SetSoftShadowQuality(int optionIndex)
    {
        if (renderPipelineAsset != null)
        {
            switch (optionIndex)
            {
                case 0: 
                    renderPipelineAsset.shadowCascadeOption = ShadowCascadesOption.NoCascades;
                    break;
                case 1: 
                    renderPipelineAsset.shadowCascadeOption = ShadowCascadesOption.TwoCascades;
                    break;
                case 2: 
                    renderPipelineAsset.shadowCascadeOption = ShadowCascadesOption.FourCascades;
                    break;
                default:
                    Debug.LogError("Invalid Soft Shadow option index: " + optionIndex);
                    break;
            }
        }
        PlayerPrefs.SetInt("SoftShadowQuality", optionIndex);
    }


    private void SetResolution(int index)
    {
        var parts = resolutionDropdown.options[index].text.Split('x');
        int width = int.Parse(parts[0]);
        int height = int.Parse(parts[1]);
        Screen.SetResolution(width, height, Screen.fullScreen);

        PlayerPrefs.SetInt("ResolutionWidth", width);
        PlayerPrefs.SetInt("ResolutionHeight", height);
    }

    private void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
        PlayerPrefs.SetInt("FullScreen", isFullScreen ? 1 : 0);
    }

    private void SetVSync(bool isEnabled)
    {
        QualitySettings.vSyncCount = isEnabled ? 1 : 0;
        PlayerPrefs.SetInt("VSync", isEnabled ? 1 : 0);
    }

    private void SetGraphicsQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("GraphicsQuality", qualityIndex);
    }

    private void SetAntiAliasingQuality(int optionIndex)
    {
        if (renderPipelineAsset != null)
        {
            UniversalAdditionalCameraData additionalCameraData = mainCamera.GetComponent<UniversalAdditionalCameraData>();

            if (additionalCameraData != null)
            {
                switch (optionIndex)
                {
                    case 0: 
                        additionalCameraData.antialiasing = AntialiasingMode.None;
                        break;
                    case 1: 
                        additionalCameraData.antialiasing = AntialiasingMode.FastApproximateAntialiasing;
                        break;
                    case 2: 
                        additionalCameraData.antialiasing = AntialiasingMode.SubpixelMorphologicalAntiAliasing;
                        break;
                    case 3: 
                        additionalCameraData.antialiasing = AntialiasingMode.TemporalAntiAliasing;
                        break;
                    default:
                        Debug.LogError("Invalid Anti-Aliasing option index: " + optionIndex);
                        break;
                }
            }
        }

        PlayerPrefs.SetInt("AntiAliasingQuality", optionIndex);
    }

    private void SetRefreshRate(int index)
    {
        string refreshRateString = refreshRateDropdown.options[index].text;
        int refreshRate = int.Parse(refreshRateString.Substring(0, refreshRateString.Length - 2));

        PlayerPrefs.SetInt("RefreshRate", refreshRate);
    }

    private void SetTextureQuality(int qualityIndex)
    {
        QualitySettings.globalTextureMipmapLimit = qualityIndex;
        PlayerPrefs.SetInt("TextureQuality", qualityIndex);
    }

    private void SetShadowDistance(float value)
    {
        if (renderPipelineAsset != null)
        {
            renderPipelineAsset.shadowDistance = value;
        }
        shadowDistanceText.text = value.ToString();

        PlayerPrefs.SetFloat("ShadowDistance", value);
    }

    private void SetBloom(bool isEnabled)
    {
        var bloom = volumeProfile.components.FirstOrDefault(x => x is Bloom) as Bloom;

        if (bloom != null)
        {
            bloom.active = isEnabled;
        }

        PlayerPrefs.SetInt("Bloom", isEnabled ? 1 : 0);
    }

    private void SetMotionBlur(bool isEnabled)
    {
        var motionBlur = volumeProfile.components.FirstOrDefault(x => x is MotionBlur) as MotionBlur;

        if (motionBlur != null)
        {
            motionBlur.active = isEnabled;
        }

        PlayerPrefs.SetInt("MotionBlur", isEnabled ? 1 : 0);
    }

    private void SetDepthOfField(bool isEnabled)
    {
        var depthOfField = volumeProfile.components.FirstOrDefault(x => x is DepthOfField) as DepthOfField;

        if (depthOfField != null)
        {
            depthOfField.active = isEnabled;
        }

        PlayerPrefs.SetInt("DepthOfField", isEnabled ? 1 : 0);
    }

    private void ResetToDefaults()
    {
        Screen.SetResolution(1920, 1080, true);
        QualitySettings.vSyncCount = 1;
        QualitySettings.SetQualityLevel(2);
        QualitySettings.globalTextureMipmapLimit = 1;

        if (renderPipelineAsset != null)
        {
            renderPipelineAsset.shadowDistance = 150f; 
            renderPipelineAsset.shadowCascadeOption = ShadowCascadesOption.FourCascades; 
        }

        resolutionDropdown.value = GetCurrentResolutionIndex();
        fullScreenToggle.isOn = Screen.fullScreen;
        vsyncToggle.isOn = QualitySettings.vSyncCount != 0;
        graphicsQualityDropdown.value = QualitySettings.GetQualityLevel();
        textureQualityDropdown.value = QualitySettings.globalTextureMipmapLimit;
        shadowDistanceSlider.value = renderPipelineAsset != null ? renderPipelineAsset.shadowDistance : 0;
        shadowDistanceText.text = (renderPipelineAsset != null ? renderPipelineAsset.shadowDistance.ToString() : "0");
        softShadowQualityDropdown.value = (int)renderPipelineAsset.shadowCascadeOption;

        PlayerPrefs.SetInt("ResolutionWidth", 1920);
        PlayerPrefs.SetInt("ResolutionHeight", 1080);
        PlayerPrefs.SetInt("FullScreen", 1);
        PlayerPrefs.SetInt("VSync", 1);
        PlayerPrefs.SetInt("GraphicsQuality", 2);
        PlayerPrefs.SetInt("TextureQuality", 1);
        PlayerPrefs.SetFloat("ShadowDistance", 150f);
        PlayerPrefs.SetInt("SoftShadowQuality", 2);
    }
}
