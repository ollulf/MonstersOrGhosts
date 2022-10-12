using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

namespace OccaSoftware.Buto.Editor
{
	[VolumeComponentEditor(typeof(VolumetricFog))]
    public class VolumetricFogEditor : VolumeComponentEditor
    {
        SerializedDataParameter mode;
        SerializedDataParameter sampleCount;
        SerializedDataParameter animateSamplePosition;
        SerializedDataParameter selfShadowingEnabled;
        SerializedDataParameter maximumSelfShadowingOctaves;
        SerializedDataParameter horizonShadowingEnabled;
        SerializedDataParameter maxDistanceVolumetric;
        SerializedDataParameter analyticFogEnabled;
        SerializedDataParameter maxDistanceAnalytic;
        SerializedDataParameter temporalAntiAliasingEnabled;
        SerializedDataParameter temporalAntiAliasingIntegrationRate;

        SerializedDataParameter fogDensity;
        SerializedDataParameter anisotropy;
        SerializedDataParameter lightIntensity;
        SerializedDataParameter shadowIntensity;

        SerializedDataParameter baseHeight;
        SerializedDataParameter attenuationBoundarySize;

        SerializedDataParameter colorRamp;
        SerializedDataParameter litColor;
        SerializedDataParameter shadowedColor;
        SerializedDataParameter emitColor;

        SerializedDataParameter colorInfluence;

        SerializedDataParameter octaves;
        SerializedDataParameter lacunarity;
        SerializedDataParameter gain;
        SerializedDataParameter noiseTiling;
        SerializedDataParameter noiseWindSpeed;
        SerializedDataParameter noiseMap;

        // Generated Noise
        SerializedProperty p_frequency;
        SerializedProperty p_octaves;
        SerializedProperty p_lacunarity;
        SerializedProperty p_gain;
        SerializedProperty p_seed;
        SerializedProperty p_noiseQuality;
        SerializedProperty p_noiseType;
        SerializedProperty p_userTexture;
        SerializedProperty p_invert;


        public override void OnEnable()
		{
            PropertyFetcher<VolumetricFog> o = new PropertyFetcher<VolumetricFog>(serializedObject);
            mode = Unpack(o.Find(x => x.mode));
            sampleCount = Unpack(o.Find(x => x.sampleCount));
            animateSamplePosition = Unpack(o.Find(x => x.animateSamplePosition));
            selfShadowingEnabled = Unpack(o.Find(x => x.selfShadowingEnabled));
            maximumSelfShadowingOctaves = Unpack(o.Find(x => x.maximumSelfShadowingOctaves));
            horizonShadowingEnabled = Unpack(o.Find(x => x.horizonShadowingEnabled));
            maxDistanceVolumetric = Unpack(o.Find(x => x.maxDistanceVolumetric));
            analyticFogEnabled = Unpack(o.Find(x => x.analyticFogEnabled));
            maxDistanceAnalytic= Unpack(o.Find(x => x.maxDistanceAnalytic));
            temporalAntiAliasingEnabled = Unpack(o.Find(x => x.temporalAntiAliasingEnabled));
            temporalAntiAliasingIntegrationRate = Unpack(o.Find(x => x.temporalAntiAliasingIntegrationRate));


            fogDensity = Unpack(o.Find(x => x.fogDensity));
            anisotropy = Unpack(o.Find(x => x.anisotropy));
            lightIntensity = Unpack(o.Find(x => x.lightIntensity));
            shadowIntensity = Unpack(o.Find(x => x.shadowIntensity));


            baseHeight= Unpack(o.Find(x => x.baseHeight));
            attenuationBoundarySize= Unpack(o.Find(x => x.attenuationBoundarySize));


            colorRamp = Unpack(o.Find(x => x.colorRamp));
            litColor = Unpack(o.Find(x => x.litColor));
            shadowedColor = Unpack(o.Find(x => x.shadowedColor));
            emitColor = Unpack(o.Find(x => x.emitColor));
            colorInfluence = Unpack(o.Find(x => x.colorInfluence));

            octaves = Unpack(o.Find(x => x.octaves));
            lacunarity= Unpack(o.Find(x => x.lacunarity));
            gain= Unpack(o.Find(x => x.gain));
            noiseTiling= Unpack(o.Find(x => x.noiseTiling));
            noiseWindSpeed= Unpack(o.Find(x => x.noiseWindSpeed));
            noiseMap = Unpack(o.Find(x => x.noiseMap));


            p_frequency = o.Find("volumeNoise.m_Value.frequency");
            p_octaves = o.Find("volumeNoise.m_Value.octaves");
            p_lacunarity = o.Find("volumeNoise.m_Value.lacunarity");
            p_gain = o.Find("volumeNoise.m_Value.gain");
            p_seed = o.Find("volumeNoise.m_Value.seed");
            p_noiseType = o.Find("volumeNoise.m_Value.noiseType");
            p_noiseQuality = o.Find("volumeNoise.m_Value.noiseQuality");
            p_userTexture = o.Find("volumeNoise.m_Value.userTexture");
            p_invert = o.Find("volumeNoise.m_Value.invert");
        }


        public override void OnInspectorGUI()
		{
            PropertyField(mode);
			if (mode.value.enumValueIndex == ((int)VolumetricFogMode.On))
            {
                DrawQualitySettings();
                DrawCharacteristics();
                DrawGeometry();
                DrawVolumeNoiseSource();
                DrawVolumeNoiseRendering();
                DrawCustomColorSettings();
                DrawDistantFog();
                DrawTemporalAntiAliasing();
                DrawIntensityOverrides();
                DrawAdvancedSettings();


                void DrawIntensityOverrides()
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField(new GUIContent("Intensity Overrides", "These properties can override the default fog density for lit and shadowed regions. Use with care."), EditorStyles.boldLabel);

                    PropertyField(lightIntensity);
                    PropertyField(shadowIntensity);
                }
                void DrawCharacteristics()
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Characteristics", EditorStyles.boldLabel);
                    PropertyField(fogDensity);
                    PropertyField(anisotropy);
                }
                void DrawGeometry()
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Geometry", EditorStyles.boldLabel);
                    PropertyField(baseHeight);
                    PropertyField(attenuationBoundarySize);
                }
                void DrawCustomColorSettings()
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField(new GUIContent("Custom Color Settings", "You can override the default fog color settings using the properties in this section. The color ramp is combined multiplicatively with the individual color settings. The resulting value is lerped to according to the influence slider."), EditorStyles.boldLabel);

                    PropertyField(colorRamp);
                    PropertyField(litColor);
                    PropertyField(shadowedColor);
                    PropertyField(emitColor);
                    PropertyField(colorInfluence);
                }
                void DrawQualitySettings()
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField(new GUIContent("Quality", "Configure settings that affect the baseline quality of the Volumetric Fog."), EditorStyles.boldLabel);
                    PropertyField(maxDistanceVolumetric, new GUIContent("Maximum Distance"));
                    PropertyField(sampleCount);
                }
                void DrawVolumeNoiseSource()
				{
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField(new GUIContent("Volume Noise Source", "Configure the noise texture that will be used for rendering. You can load your own 3D Texture or configure the parameters used to generate a texture."), EditorStyles.boldLabel);

                    EditorGUI.BeginChangeCheck();
                    EditorGUI.indentLevel++;

                    EditorGUILayout.PropertyField(p_noiseType, new GUIContent("Type"));
                    switch (p_noiseType.enumValueIndex)
                    {
                        case (int)VolumeNoise.NoiseType.None:
                            break;
                        case (int)VolumeNoise.NoiseType.Texture:
                            DrawTextureControls();
                            break;
                        default:
                            DrawNoiseControls();
                            break;
                    }

                    void DrawTextureControls()
                    {
                        EditorGUILayout.PropertyField(p_userTexture, new GUIContent("Texture"));
                    }

                    void DrawNoiseControls()
                    {
                        EditorGUI.indentLevel++;
                        EditorGUILayout.PropertyField(p_noiseQuality, new GUIContent("Quality"));
                        EditorGUILayout.IntSlider(p_frequency, 1, 32, new GUIContent("Frequency"));
                        EditorGUILayout.IntSlider(p_octaves, 1, 10);
                        if (p_octaves.intValue > 1)
                        {
                            EditorGUILayout.IntSlider(p_lacunarity, 1, 10);
                            EditorGUILayout.Slider(p_gain, 0f, 1f);
                        }
                        EditorGUILayout.PropertyField(p_invert);
                        EditorGUILayout.PropertyField(p_seed);
                        EditorGUI.indentLevel--;
                    }

                    if (EditorGUI.EndChangeCheck())
                    {
                        var t = target as VolumetricFog;

                        if (t == null)
                            return;

                        t.volumeNoise.value.Release();
                        t.volumeNoise.value.SetDirty();
                    }

                    EditorGUI.indentLevel--;
                }
                void DrawVolumeNoiseRendering()
				{
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField(new GUIContent("Volume Noise Rendering", "Configure noise rendering parameters. Describes how the Noise Texture from the Volume Noise Source section will be sampled."), EditorStyles.boldLabel);
                    PropertyField(noiseTiling, new GUIContent("Tiling Rate"));
                    PropertyField(noiseMap, new GUIContent("Remapping"));
                    PropertyField(noiseWindSpeed, new GUIContent("Wind Speed"));

                    PropertyField(octaves);
                    if (octaves.value.intValue > 1)
                    {
                        PropertyField(lacunarity);
                        PropertyField(gain);
                    }
                }
                void DrawDistantFog()
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField(new GUIContent("Distant Fog", "Set the analytic (distant) fog properties."), EditorStyles.boldLabel);
                    PropertyField(analyticFogEnabled, new GUIContent("Distant Fog Enabled"));
                    bool distantFogEnabled = analyticFogEnabled.value.boolValue && analyticFogEnabled.overrideState.boolValue ? true : false;
                    if (distantFogEnabled)
                    {
                        using (new IndentLevelScope(15))
                        {
                            PropertyField(maxDistanceAnalytic, new GUIContent("Max Distance"));
                        }
                    }
                }
                void DrawTemporalAntiAliasing()
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField(new GUIContent("Temporal Anti-Aliasing", "Set and configure Temporal Anti-Aliasing. Can cause artifacts when the camera or objects in scene are in motion."), EditorStyles.boldLabel);
                    PropertyField(temporalAntiAliasingEnabled, new GUIContent("TAA Enabled"));
                    bool taaState = temporalAntiAliasingEnabled.value.boolValue && temporalAntiAliasingEnabled.overrideState.boolValue ? true : false;
                    if (taaState)
                    {
                        using (new IndentLevelScope(15))
                        {
                            PropertyField(temporalAntiAliasingIntegrationRate, new GUIContent("Integration Rate"));
                        }
                    }
                }
                void DrawAdvancedSettings()
				{
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Advanced Settings", EditorStyles.boldLabel);
                    PropertyField(animateSamplePosition);
                    PropertyField(selfShadowingEnabled);

                    bool selfShadowingState = selfShadowingEnabled.value.boolValue || !selfShadowingEnabled.overrideState.boolValue ? true : false;
                    if (selfShadowingState)
                    {
                        using (new IndentLevelScope(15))
                        {
                            PropertyField(maximumSelfShadowingOctaves, new GUIContent("Octave Limit"));
                        }
                    }
                    PropertyField(horizonShadowingEnabled);
                }

            }
		}
	}
}
