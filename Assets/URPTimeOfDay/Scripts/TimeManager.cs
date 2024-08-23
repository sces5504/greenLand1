using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;
using System.Collections;

namespace TimeOfDayURP
{
    public class TimeManager : MonoBehaviour
    {
        public bool enableTimeOfDay = false;

        public Light directionalLight;
        public Color defaultSunColor = new Color(1f, 0.957f, 0.839f, 1f);
        public Color sunriseSunsetColor = new Color(152 / 255f, 107 / 255f, 24 / 255f, 1);
        public Color nightSunColor = new Color(0.839f, 1f, 0.976f, 1f);

        public Material proceduralSkyBox;
        public float dayStarIntensity = 0f;
        public float nightStarIntensity = 0.8f;
        [Range(-1f, 1f)]
        public float moonPhase = 0.05f;

        [ColorUsageAttribute(true, true)] public Color dayColorTop = new Color(2 / 255f, 10 / 255f, 36 / 255f, 1);
        [ColorUsageAttribute(true, true)] public Color dayColorMiddle = new Color(0 / 255f, 23 / 255f, 96 / 255f, 1);
        [ColorUsageAttribute(true, true)] public Color dayColorLow = new Color(114 / 255f, 116 / 255f, 154 / 255f, 1);
        [ColorUsageAttribute(true, true)] public Color nightColorTop = new Color(0 / 255f, 0 / 255f, 0 / 255f, 1);
        [ColorUsageAttribute(true, true)] public Color nightColorMiddle = new Color(2 / 255f, 3 / 255f, 5 / 255f, 1);
        [ColorUsageAttribute(true, true)] public Color nightColorLow = new Color(8 / 255f, 9 / 255f, 14 / 255f, 1);

        public bool enableSkyboxChange = false;
        public Material blendSkybox;

        public float dayLengthInMinutes = 24f;
        public AnimationCurve sunIntensityCurve = new AnimationCurve(
            new Keyframe(0, 0.1f),
            new Keyframe(0.2f, 0.1f),
            new Keyframe(0.25f, 1),
            new Keyframe(0.75f, 1),
            new Keyframe(0.8f, 0.1f),
            new Keyframe(1, 0.1f)
        );
        public int daysPerYear = 360; // Make sure this is divisible by 4 for simplicity
        public int startingDayOfYear = 1;
        public float startingTimeOfDay = 0;

        public bool enableWeather = true;
        public Material[] weatherMaterials;
        public Transform playerTransform;

        public bool enableCloudCoverage = false;
        public bool disableCloudCoverage = false;
        public float cloudCoverageTransitionTime = 1f;
        public float cloudCoverageStartValue = 0f;
        public float cloudCoverageEndValue = 1f;
        private float currentCloudCoverageTransition = 0f;
        private float currentCloudCoverage = 0f;
        private DecalProjector decalProjector;
        public bool useDecalProjector = true;
        public float shadowwMaxFadeFactor = 0.5f;

        public bool enableRain = false;
        public GameObject rainPrefab;
        private AudioSource rainAudio;
        public float audioTransitionTime = 2.0f;
        public float rainStrength = 0.4f;
        public float rainTransitionTime = 1f;
        public float rainStrengthTransitionTime = 1f;
        public float rainAmountTransitionTime = 1f;
        public float disableRainTransitionTime = 1f;
        public float disableRainStrengthTransitionTime = 1f;
        public float disableRainAmountTransitionTime = 1f;

        public bool enableSnow = false;
        public GameObject snowPrefab;
        public float snowTransitionTime = 1f;
        public float snowAmountTransitionTime = 1f;
        public float disableSnowTransitionTime = 1f;
        public float disableSnowAmountTransitionTime = 1f;

        public bool enableWind = false;
        public GameObject windPrefab;
        public bool enableThunder = false;
        public GameObject thunderPrefab;

        private GameObject instantiatedWindPrefab;
        private GameObject instantiatedThunderPrefab;
        private AudioSource windAudio;
        private AudioSource thunderAudio;
        private bool wasWindEnabled = false;
        private bool wasThunderEnabled = false;

        public float timeOfDay;
        private int day;
        private int season;
        private int daysPerSeason;
        public float CurrentRainAmount;
        public float CurrentSnowAmount;
        private float rainStrengthTargetValue = 0;
        public float rainAmountTargetValue = 0;
        private bool wasRainEnabled = false;
        private bool wasSnowEnabled = false;
        private GameObject instantiatedRainPrefab;
        private GameObject instantiatedSnowPrefab;
        public bool isRainTransitioning = false;
        private bool isSnowTransitioning = false;

        public UnityEvent onMorning;
        public UnityEvent onNoon;
        public UnityEvent onEvening;
        public UnityEvent onMidnight;
        public UnityEvent onTimeChange;

        public UnityEvent onSpring;
        public UnityEvent onSummer;
        public UnityEvent onAutumn;
        public UnityEvent onWinter;

        private int currentSeason = -1;

        public UnityEvent onSeasonChange;
        private float lastTimeTriggered = -1f;

        public float CurrentTime24Hour
        {
            get
            {
                return timeOfDay * 24.0f;
            }
        }

        private void Start()
        {
            timeOfDay = startingTimeOfDay / 24f;
            daysPerSeason = daysPerYear / 4; // Divide year into 4 seasons

            if (proceduralSkyBox != null)
            {
                proceduralSkyBox.SetFloat("_Moon_Offset", moonPhase);
            }

            day = startingDayOfYear % daysPerSeason;

            season = startingDayOfYear / daysPerSeason;
            if (day == 0)
            {
                day = daysPerSeason;
                season--;
            }

            directionalLight.transform.eulerAngles = new Vector3((timeOfDay * 360f) - 90f, -30f, 0);

            if (RenderSettings.skybox.HasProperty("_Day_Color_Low"))
            {
                proceduralSkyBox = RenderSettings.skybox;
            }

            if (RenderSettings.skybox.HasProperty("_Day_Color_Top"))
            {
                proceduralSkyBox = RenderSettings.skybox;
                proceduralSkyBox.SetColor("_Day_Color_Top", dayColorTop);
                proceduralSkyBox.SetColor("_Day_Color_Middle", dayColorMiddle);
                proceduralSkyBox.SetColor("_Day_Color_Low", dayColorLow);
                proceduralSkyBox.SetColor("_Night_Color_Top", nightColorTop);
                proceduralSkyBox.SetColor("_Night_Color_Middle", nightColorMiddle);
                proceduralSkyBox.SetColor("_Night_Color_Low", nightColorLow);
            }

            if (useDecalProjector)
            {
                decalProjector = GetComponent<DecalProjector>();
                if (decalProjector == null)
                {
                    Debug.LogWarning("Decal Projector not found but useDecalProjector is true.");
                }
            }
        }

        void Update()
        {
            if (!enableTimeOfDay) return;

            timeOfDay += Time.deltaTime / (dayLengthInMinutes * 60f);

            if (proceduralSkyBox != null)
            {
                proceduralSkyBox.SetVector("_Sun_Direction", -directionalLight.transform.forward);
                proceduralSkyBox.SetFloat("_Moon_Offset", moonPhase);
            }

            if (timeOfDay >= 1)
            {
                timeOfDay -= 1;
                day += 1;
            }

            if (day >= daysPerSeason)
            {
                day -= daysPerSeason;
                season = (season + 1) % 4;
            }

            float rotationDegreesPerMinute = 360f / (dayLengthInMinutes * 60f);
            directionalLight.transform.Rotate(Vector3.right, rotationDegreesPerMinute * Time.deltaTime);
            directionalLight.intensity = sunIntensityCurve.Evaluate(timeOfDay);

            AdjustSunColor();
            AdjustStarIntensity();

            if (timeOfDay >= 0f && timeOfDay < 0.0001f)
            {
                onMidnight?.Invoke();
            }
            else if (timeOfDay >= 0.25f && timeOfDay < 0.2501f)
            {
                onMorning?.Invoke();
            }
            else if (timeOfDay >= 0.5f && timeOfDay < 0.5001f)
            {
                onNoon?.Invoke();
            }
            else if (timeOfDay >= 0.75f && timeOfDay < 0.7501f)
            {
                onEvening?.Invoke();
            }

            if (enableWeather)
            {
                if (enableRain)
                {
                    if (!wasRainEnabled)
                    {
                        instantiatedRainPrefab = Instantiate(rainPrefab, playerTransform.position, Quaternion.identity, playerTransform);
                        rainAudio = instantiatedRainPrefab.GetComponent<AudioSource>();
                        float targetVolume = rainAudio.volume;
                        rainAudio.volume = 0;
                        StartCoroutine(AdjustVolume(rainAudio, 0, targetVolume, audioTransitionTime));
                        wasRainEnabled = true;
                        isRainTransitioning = true;
                        rainStrengthTargetValue = rainStrength;
                        rainAmountTargetValue = 1f;
                    }
                }
                else if (wasRainEnabled)
                {
                    wasRainEnabled = false;
                    if (rainAudio)
                    {
                        StartCoroutine(FadeOutAndDestroy(rainAudio, instantiatedRainPrefab, audioTransitionTime));
                    }
                    else
                    {
                        Destroy(instantiatedRainPrefab);
                    }
                    instantiatedRainPrefab = null;
                    isRainTransitioning = true;
                    rainStrengthTargetValue = 0;
                    rainAmountTargetValue = 0;
                }

                if (isRainTransitioning)
                {
                    foreach (Material mat in weatherMaterials)
                    {
                        float currentValue = mat.GetFloat("_General_Rain");
                        float newValue = Mathf.Lerp(currentValue, rainAmountTargetValue, Time.deltaTime / (enableRain ? rainTransitionTime : disableRainTransitionTime));
                        mat.SetFloat("_General_Rain", newValue);

                        float currentRainStrength = mat.GetFloat("_Rain_Strength");
                        float newRainStrength = Mathf.Lerp(currentRainStrength, rainStrengthTargetValue, Time.deltaTime / (enableRain ? rainStrengthTransitionTime : disableRainStrengthTransitionTime));
                        mat.SetFloat("_Rain_Strength", newRainStrength);

                        CurrentRainAmount = mat.GetFloat("_Rain_Amount");
                        float newRainAmount = Mathf.Lerp(CurrentRainAmount, rainAmountTargetValue, Time.deltaTime / (enableRain ? rainAmountTransitionTime : disableRainAmountTransitionTime));
                        mat.SetFloat("_Rain_Amount", newRainAmount);

                        float currentRainDropStrength = mat.GetFloat("_Rain_Drop_Strength");
                        float newRainDropStrength = Mathf.Lerp(currentRainDropStrength, rainAmountTargetValue, Time.deltaTime / (enableRain ? rainAmountTransitionTime : disableRainAmountTransitionTime));
                        mat.SetFloat("_Rain_Drop_Strength", newRainDropStrength);

                        if (Mathf.Approximately(newValue, rainStrengthTargetValue) && Mathf.Approximately(newRainStrength, rainStrengthTargetValue) && Mathf.Approximately(newRainAmount, rainAmountTargetValue) && Mathf.Approximately(newRainDropStrength, rainAmountTargetValue)) isRainTransitioning = false;
                    }
                }

                if (enableSnow)
                {
                    if (!wasSnowEnabled)
                    {
                        instantiatedSnowPrefab = Instantiate(snowPrefab, playerTransform.position, Quaternion.identity, playerTransform);
                        wasSnowEnabled = true;
                        isSnowTransitioning = true;
                    }
                }
                else if (wasSnowEnabled)
                {
                    wasSnowEnabled = false;
                    Destroy(instantiatedSnowPrefab);
                    instantiatedSnowPrefab = null;
                    isSnowTransitioning = true;
                }

                if (isSnowTransitioning)
                {
                    foreach (Material mat in weatherMaterials)
                    {
                        float currentGeneralSnow = mat.GetFloat("_General_Snow");
                        float newGeneralSnow = Mathf.Lerp(currentGeneralSnow, enableSnow ? 1 : 0, Time.deltaTime / (enableSnow ? snowTransitionTime : disableSnowTransitionTime));
                        mat.SetFloat("_General_Snow", newGeneralSnow);

                        CurrentSnowAmount = mat.GetFloat("_Snow_Amount");
                        float newSnowAmount = Mathf.Lerp(CurrentSnowAmount, enableSnow ? 1 : 0, Time.deltaTime / (enableSnow ? snowAmountTransitionTime : disableSnowAmountTransitionTime));
                        mat.SetFloat("_Snow_Amount", newSnowAmount);

                        if (Mathf.Approximately(newGeneralSnow, enableSnow ? 1 : 0) && Mathf.Approximately(newSnowAmount, enableSnow ? 1 : 0)) isSnowTransitioning = false;
                    }
                }

                AdjustCloudCoverage();

                if (season != currentSeason) // Only execute when the season changes
                {
                    currentSeason = season; // Update the current season

                    switch (currentSeason)
                    {
                        case 0: // Spring
                            onSpring?.Invoke();
                            break;
                        case 1: // Summer
                            onSummer?.Invoke();
                            break;
                        case 2: // Autumn
                            onAutumn?.Invoke();
                            break;
                        case 3: // Winter
                            onWinter?.Invoke();
                            break;
                    }
                }

                if (enableWeather && enableWind)
                {
                    if (!wasWindEnabled)
                    {
                        instantiatedWindPrefab = Instantiate(windPrefab, playerTransform.position, Quaternion.identity, playerTransform);
                        windAudio = instantiatedWindPrefab.GetComponent<AudioSource>();
                        float targetVolume = windAudio.volume;
                        windAudio.volume = 0;
                        StartCoroutine(AdjustVolume(windAudio, 0, targetVolume, audioTransitionTime));
                        wasWindEnabled = true;
                    }
                }
                else if (wasWindEnabled)
                {
                    wasWindEnabled = false;
                    if (windAudio)
                    {
                        StartCoroutine(FadeOutAndDestroy(windAudio, instantiatedWindPrefab, audioTransitionTime));
                    }
                    else
                    {
                        Destroy(instantiatedWindPrefab);
                    }
                    instantiatedWindPrefab = null;
                }

                if (enableWeather && enableThunder)
                {
                    if (!wasThunderEnabled)
                    {
                        instantiatedThunderPrefab = Instantiate(thunderPrefab, playerTransform.position, Quaternion.identity, playerTransform);
                        thunderAudio = instantiatedThunderPrefab.GetComponent<AudioSource>();
                        float targetVolume = thunderAudio.volume;
                        thunderAudio.volume = 0;
                        StartCoroutine(AdjustVolume(thunderAudio, 0, targetVolume, audioTransitionTime));
                        wasThunderEnabled = true;
                    }
                }
                else if (wasThunderEnabled)
                {
                    wasThunderEnabled = false;
                    if (thunderAudio)
                    {
                        StartCoroutine(FadeOutAndDestroy(thunderAudio, instantiatedThunderPrefab, audioTransitionTime));
                    }
                    else
                    {
                        Destroy(instantiatedThunderPrefab);
                    }
                    instantiatedThunderPrefab = null;
                }
            }

            if (enableSkyboxChange)
            {
                float lerpValue;

                if (timeOfDay >= 0.25f && timeOfDay <= 0.75f)
                {
                    lerpValue = 1;
                }
                else if (timeOfDay > 0.2f && timeOfDay < 0.25f)  // 0.2 to 0.25 is sunrise
                {
                    lerpValue = Mathf.InverseLerp(0.2f, 0.25f, timeOfDay);
                }
                else if (timeOfDay > 0.75f && timeOfDay < 0.8f)  // 0.75 to 0.8 is sunset
                {
                    lerpValue = Mathf.InverseLerp(0.8f, 0.75f, timeOfDay);
                }
                else  // night time
                {
                    lerpValue = 0;
                }

                blendSkybox.SetFloat("_LerpValue", lerpValue);
                RenderSettings.skybox = blendSkybox;  // Ensure blendSkybox is always set.
            }
        }

        IEnumerator AdjustVolume(AudioSource audioSource, float startVolume, float endVolume, float duration)
        {
            float elapsed = 0;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                audioSource.volume = Mathf.Lerp(startVolume, endVolume, elapsed / duration);
                yield return null;
            }
        }

        IEnumerator FadeOutAndDestroy(AudioSource audioSource, GameObject targetObject, float duration)
        {
            yield return AdjustVolume(audioSource, audioSource.volume, 0, duration);
            Destroy(targetObject);
        }

        private void AdjustCloudCoverage()
        {
            if (enableCloudCoverage)
            {
                if (currentCloudCoverageTransition < cloudCoverageTransitionTime)
                {
                    currentCloudCoverageTransition += Time.deltaTime;
                    currentCloudCoverage = Mathf.Lerp(0, shadowwMaxFadeFactor, currentCloudCoverageTransition / cloudCoverageTransitionTime);

                    if (useDecalProjector && decalProjector != null)
                    {
                        decalProjector.fadeFactor = currentCloudCoverage;
                    }

                    if (proceduralSkyBox != null)
                    {
                        proceduralSkyBox.SetFloat("_Cloud_Opacity", currentCloudCoverage);
                    }
                }
                else
                {
                    enableCloudCoverage = false;
                    currentCloudCoverageTransition = 0f;
                }
            }
            else if (disableCloudCoverage)
            {
                if (currentCloudCoverageTransition < cloudCoverageTransitionTime)
                {
                    currentCloudCoverageTransition += Time.deltaTime;
                    currentCloudCoverage = Mathf.Lerp(shadowwMaxFadeFactor, 0, currentCloudCoverageTransition / cloudCoverageTransitionTime);

                    if (useDecalProjector && decalProjector != null)
                    {
                        decalProjector.fadeFactor = currentCloudCoverage;
                    }

                    if (proceduralSkyBox != null)
                    {
                        proceduralSkyBox.SetFloat("_Cloud_Opacity", currentCloudCoverage);
                    }
                }
                else
                {
                    disableCloudCoverage = false;
                    currentCloudCoverageTransition = 0f;
                }
            }
        }

        private void AdjustSunColor()
        {
            if (timeOfDay < 0.33333f)
            {
                directionalLight.color = Color.Lerp(sunriseSunsetColor, defaultSunColor, (timeOfDay - 0.20833f) / 0.125f);
                if (proceduralSkyBox != null)
                {
                    proceduralSkyBox.SetColor("_Day_Color_Low", Color.Lerp(sunriseSunsetColor, dayColorLow, (timeOfDay - 0.20833f) / 0.125f));
                }
            }
            else if (timeOfDay < 0.6666667f)
            {
                directionalLight.color = defaultSunColor;
                if (proceduralSkyBox != null)
                {
                    proceduralSkyBox.SetColor("_Day_Color_Low", dayColorLow);
                }
            }
            else if (timeOfDay < 0.8125f)
            {
                directionalLight.color = Color.Lerp(defaultSunColor, sunriseSunsetColor, (timeOfDay - 0.6666667f) / 0.1458333f);
                if (proceduralSkyBox != null)
                {
                    proceduralSkyBox.SetColor("_Day_Color_Low", Color.Lerp(dayColorLow, sunriseSunsetColor, (timeOfDay - 0.6666667f) / 0.1458333f));
                }
            }
            else
            {
                directionalLight.color = Color.Lerp(sunriseSunsetColor, nightSunColor, (timeOfDay - 0.8125f) / (1 - 0.8125f));
                if (proceduralSkyBox != null)
                {
                    proceduralSkyBox.SetColor("_Day_Color_Low", Color.Lerp(sunriseSunsetColor, nightSunColor, (timeOfDay - 0.8125f) / (1 - 0.8125f)));
                }
            }
        }


        private void AdjustStarIntensity()
        {
            if (proceduralSkyBox != null)
            {
                if (timeOfDay < 0.20833f || timeOfDay > 0.83333f)
                {
                    proceduralSkyBox.SetFloat("_Stars_Intensity", nightStarIntensity);
                }
                else if (timeOfDay < 0.33333f)
                {
                    proceduralSkyBox.SetFloat("_Stars_Intensity", Mathf.Lerp(nightStarIntensity, dayStarIntensity, (timeOfDay - 0.20833f) / 0.125f));
                }
                else if (timeOfDay < 0.70833f)
                {
                    proceduralSkyBox.SetFloat("_Stars_Intensity", dayStarIntensity);
                }
                else if (timeOfDay < 0.83333f)
                {
                    proceduralSkyBox.SetFloat("_Stars_Intensity", Mathf.Lerp(dayStarIntensity, nightStarIntensity, (timeOfDay - 0.70833f) / 0.125f));
                }
            }
        }

        public int GetSeason()
        {
            return season + 1;
        }

        public float GetTime()
        {
            float timeDifference = Mathf.Abs(timeOfDay - lastTimeTriggered);
            if (timeDifference >= 0.01f)
            {
                lastTimeTriggered = timeOfDay;
                onTimeChange.Invoke();
            }
            return timeOfDay;
        }

        public int GetCurrentDayOfYear()
        {
            return (season * daysPerSeason) + day;
        }

        public string GetCurrentSeason()
        {
            switch (GetSeason())
            {
                case 1:
                    return "Spring";
                case 2:
                    return "Summer";
                case 3:
                    return "Autumn";
                case 4:
                    return "Winter";
                default:
                    return "Unknown Season";
            }
        }

        public void ToggleRain(bool status)
        {
            enableRain = status;
        }

        public void ToggleSnow(bool status)
        {
            enableSnow = status;
        }

        public void ToggleEnableCloud(bool status)
        {
            enableCloudCoverage = status;
        }

        public void ToggleDisableCloud(bool status)
        {
            disableCloudCoverage = status;
        }

        public void UpdateSeason(int newSeason)
        {
            if (season != newSeason)
            {
                season = newSeason;
                onSeasonChange.Invoke();
            }
        }
    }
}
