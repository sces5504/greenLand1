using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using TimeOfDayURP;

[CustomEditor(typeof(TimeManager))]
public class TODMyScriptEditor : Editor
{
    Texture2D headerImage;

    public override VisualElement CreateInspectorGUI()
    {
        VisualElement root = new VisualElement();
        StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/URPTimeOfDay/Scripts/Editor/MyEditorStyle.uss");
        root.styleSheets.Add(styleSheet);

        var unitySpacer = new VisualElement();
        unitySpacer.AddToClassList("unity-spacer");
        root.Add(unitySpacer);

        var header = new Image() { image = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/URPTimeOfDay/Scripts/Editor/headerImage.png") };
        header.style.maxHeight = 60;
        header.style.maxWidth = 240;
        header.style.marginLeft = 0;
        header.style.marginBottom = 5;
        header.style.marginTop = 5;

        root.Add(header);

        Foldout basicSystemsFoldout = new Foldout { value = true };
        basicSystemsFoldout.AddToClassList("foldout");
        basicSystemsFoldout.text = "Basics";
        basicSystemsFoldout.Add(unitySpacer);
        basicSystemsFoldout.Add(new PropertyField(serializedObject.FindProperty("directionalLight")));
        basicSystemsFoldout.Add(new PropertyField(serializedObject.FindProperty("sunIntensityCurve")));
        basicSystemsFoldout.Add(new PropertyField(serializedObject.FindProperty("defaultSunColor")));
        basicSystemsFoldout.Add(new PropertyField(serializedObject.FindProperty("sunriseSunsetColor")));
        basicSystemsFoldout.Add(new PropertyField(serializedObject.FindProperty("nightSunColor")));
        basicSystemsFoldout.Add(new PropertyField(serializedObject.FindProperty("CurrentTime24Hour")));
        root.Add(unitySpacer);
        basicSystemsFoldout.Add(new PropertyField(serializedObject.FindProperty("proceduralSkyBox")));
        basicSystemsFoldout.Add(new PropertyField(serializedObject.FindProperty("dayStarIntensity")));
        basicSystemsFoldout.Add(new PropertyField(serializedObject.FindProperty("nightStarIntensity")));
        basicSystemsFoldout.Add(new PropertyField(serializedObject.FindProperty("moonPhase")));
        basicSystemsFoldout.Add(new PropertyField(serializedObject.FindProperty("dayColorTop")));
        basicSystemsFoldout.Add(new PropertyField(serializedObject.FindProperty("dayColorMiddle")));
        basicSystemsFoldout.Add(new PropertyField(serializedObject.FindProperty("dayColorLow")));
        basicSystemsFoldout.Add(new PropertyField(serializedObject.FindProperty("nightColorTop")));
        basicSystemsFoldout.Add(new PropertyField(serializedObject.FindProperty("nightColorMiddle")));
        basicSystemsFoldout.Add(new PropertyField(serializedObject.FindProperty("nightColorLow")));
        basicSystemsFoldout.Add(unitySpacer);
        root.Add(basicSystemsFoldout);

        Foldout timeSystemsFoldout = new Foldout { value = false };
        timeSystemsFoldout.AddToClassList("foldout");
        timeSystemsFoldout.text = "Time System";
        timeSystemsFoldout.Add(unitySpacer);
        timeSystemsFoldout.Add(new PropertyField(serializedObject.FindProperty("enableTimeOfDay")));
        timeSystemsFoldout.Add(new PropertyField(serializedObject.FindProperty("dayLengthInMinutes")));
        timeSystemsFoldout.Add(new PropertyField(serializedObject.FindProperty("daysPerYear")));
        timeSystemsFoldout.Add(new PropertyField(serializedObject.FindProperty("startingDayOfYear")));
        timeSystemsFoldout.Add(new PropertyField(serializedObject.FindProperty("startingTimeOfDay")));
        timeSystemsFoldout.Add(unitySpacer);
        root.Add(timeSystemsFoldout);

        Foldout customSystemFoldout = new Foldout { value = false };
        customSystemFoldout.AddToClassList("foldout");
        customSystemFoldout.text = "Alternate Skybox";
        customSystemFoldout.Add(unitySpacer);
        customSystemFoldout.Add(new PropertyField(serializedObject.FindProperty("enableSkyboxChange")));
        customSystemFoldout.Add(new PropertyField(serializedObject.FindProperty("daySkybox")));
        customSystemFoldout.Add(new PropertyField(serializedObject.FindProperty("nightSkybox")));
        customSystemFoldout.Add(new PropertyField(serializedObject.FindProperty("blendSkybox")));
        customSystemFoldout.Add(unitySpacer);
        root.Add(customSystemFoldout);

        Foldout weatherSystemFoldout = new Foldout { value = false };
        weatherSystemFoldout.AddToClassList("foldout");
        weatherSystemFoldout.text = "Weather System";
        weatherSystemFoldout.Add(unitySpacer);
        weatherSystemFoldout.Add(new PropertyField(serializedObject.FindProperty("enableWeather")));
        weatherSystemFoldout.Add(new PropertyField(serializedObject.FindProperty("weatherMaterials")));
        weatherSystemFoldout.Add(new PropertyField(serializedObject.FindProperty("playerTransform")));
        weatherSystemFoldout.Add(new PropertyField(serializedObject.FindProperty("audioTransitionTime")));
        weatherSystemFoldout.Add(unitySpacer);
        weatherSystemFoldout.Add(new Label("Clouds"));
        weatherSystemFoldout.Add(new PropertyField(serializedObject.FindProperty("enableCloudCoverage")));
        weatherSystemFoldout.Add(new PropertyField(serializedObject.FindProperty("disableCloudCoverage")));
        weatherSystemFoldout.Add(new PropertyField(serializedObject.FindProperty("cloudCoverageTransitionTime")));
        weatherSystemFoldout.Add(new PropertyField(serializedObject.FindProperty("cloudCoverageStartValue")));
        weatherSystemFoldout.Add(new PropertyField(serializedObject.FindProperty("cloudCoverageEndValue")));
        weatherSystemFoldout.Add(new PropertyField(serializedObject.FindProperty("useDecalProjector")));
        weatherSystemFoldout.Add(new PropertyField(serializedObject.FindProperty("shadowwMaxFadeFactor")));
        weatherSystemFoldout.Add(unitySpacer);
        weatherSystemFoldout.Add(new Label("Rain"));
        weatherSystemFoldout.Add(new PropertyField(serializedObject.FindProperty("enableRain")));
        weatherSystemFoldout.Add(new PropertyField(serializedObject.FindProperty("rainPrefab")));
        weatherSystemFoldout.Add(new PropertyField(serializedObject.FindProperty("rainStrength")));
        weatherSystemFoldout.Add(new PropertyField(serializedObject.FindProperty("rainTransitionTime")));
        weatherSystemFoldout.Add(new PropertyField(serializedObject.FindProperty("rainStrengthTransitionTime")));
        weatherSystemFoldout.Add(new PropertyField(serializedObject.FindProperty("rainAmountTransitionTime")));
        weatherSystemFoldout.Add(new PropertyField(serializedObject.FindProperty("disableRainTransitionTime")));
        weatherSystemFoldout.Add(new PropertyField(serializedObject.FindProperty("disableRainStrengthTransitionTime")));
        weatherSystemFoldout.Add(new PropertyField(serializedObject.FindProperty("disableRainAmountTransitionTime")));
        weatherSystemFoldout.Add(unitySpacer);
        weatherSystemFoldout.Add(new Label("Snow"));
        weatherSystemFoldout.Add(new PropertyField(serializedObject.FindProperty("enableSnow")));
        weatherSystemFoldout.Add(new PropertyField(serializedObject.FindProperty("snowPrefab")));
        weatherSystemFoldout.Add(new PropertyField(serializedObject.FindProperty("snowTransitionTime")));
        weatherSystemFoldout.Add(new PropertyField(serializedObject.FindProperty("snowAmountTransitionTime")));
        weatherSystemFoldout.Add(new PropertyField(serializedObject.FindProperty("disableSnowTransitionTime")));
        weatherSystemFoldout.Add(new PropertyField(serializedObject.FindProperty("disableSnowAmountTransitionTim")));
        weatherSystemFoldout.Add(new Label("Other"));
        weatherSystemFoldout.Add(new PropertyField(serializedObject.FindProperty("enableWind")));
        weatherSystemFoldout.Add(new PropertyField(serializedObject.FindProperty("windPrefab")));
        weatherSystemFoldout.Add(new PropertyField(serializedObject.FindProperty("enableThunder")));
        weatherSystemFoldout.Add(new PropertyField(serializedObject.FindProperty("thunderPrefab")));
        root.Add(weatherSystemFoldout);

        Foldout eventsSystemFoldout = new Foldout { value = false };
        eventsSystemFoldout.AddToClassList("foldout");
        eventsSystemFoldout.text = "Events System";
        eventsSystemFoldout.Add(unitySpacer);
        eventsSystemFoldout.Add(new Label("Time of Day"));
        eventsSystemFoldout.Add(new PropertyField(serializedObject.FindProperty("onMorning")));
        eventsSystemFoldout.Add(new PropertyField(serializedObject.FindProperty("onNoon")));
        eventsSystemFoldout.Add(new PropertyField(serializedObject.FindProperty("onEvening")));
        eventsSystemFoldout.Add(new PropertyField(serializedObject.FindProperty("onMidnight")));
        eventsSystemFoldout.Add(unitySpacer);
        eventsSystemFoldout.Add(new Label("Seasons"));
        eventsSystemFoldout.Add(new PropertyField(serializedObject.FindProperty("onSpring")));
        eventsSystemFoldout.Add(new PropertyField(serializedObject.FindProperty("onSummer")));
        eventsSystemFoldout.Add(new PropertyField(serializedObject.FindProperty("onAutumn")));
        eventsSystemFoldout.Add(new PropertyField(serializedObject.FindProperty("onWinter")));
        eventsSystemFoldout.Add(unitySpacer);
        root.Add(eventsSystemFoldout);

        return root;
    }
}