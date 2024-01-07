using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using FMOD.Studio;
using FMODUnity;
using poetools.Console.Commands;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class DayNightSystem : MonoBehaviour, IConsoleDebugInfo
{
    private static readonly int SkyTint = Shader.PropertyToID("_SkyTint");
    private static readonly int AtmosphereThickness = Shader.PropertyToID("_AtmosphereThickness");
    
    [SerializeField]
    private Light sun;

    [SerializeField] 
    private TimeSetting[] timeSettings;

    [SerializeField]
    private float transitionDuration = 10;

    [SerializeField] 
    private float advanceTime = 5;

    [SerializeField] 
    private Material skyMaterial;

    private TimeSetting _currentSetting;
    private EventInstance _currentInstance;
    private CancellationTokenSource _animationCts;
    private float _elapsedTime;

    private void Start()
    {
        _currentSetting = GetSetting(Game.Save.TimeOfDay);
        _currentInstance = RuntimeManager.CreateInstance(_currentSetting.ambienceOptions[Random.Range(0, _currentSetting.ambienceOptions.Length)]);
        _currentInstance.start();
        LerpSettings(default, _currentSetting, 1);
    }

    private void OnDestroy()
    {
        _animationCts?.Cancel();
    }

    private void Update()
    {
        _elapsedTime += Time.deltaTime;

        if (_elapsedTime >= advanceTime + transitionDuration)
        {
            _elapsedTime = 0;
            AdvanceTime();
        }
    }

    public void ResetTime()
    {
        Game.Save.TimeOfDay = TimeOfDay.Morning;
        ApplyCurrentTimeSettings();
    }

    public void AdvanceTime()
    {
        TimeOfDay timeOfDay = Game.Save.TimeOfDay;

        Game.Save.TimeOfDay = timeOfDay switch {
            TimeOfDay.Morning => TimeOfDay.Noon,
            TimeOfDay.Noon => TimeOfDay.Evening,
            TimeOfDay.Evening => TimeOfDay.Night,
            _ => timeOfDay,
        };
        ApplyCurrentTimeSettings();
    }

    private void ApplyCurrentTimeSettings()
    {
        if (Game.Save.TimeOfDay == _currentSetting.timeOfDay)
            return;

        TimeSetting setting = GetSetting(Game.Save.TimeOfDay);
        _animationCts?.Cancel();
        _animationCts = new CancellationTokenSource();
        PlayTransition(_currentSetting, setting, _animationCts.Token).Forget();
    }

    private TimeSetting GetSetting(TimeOfDay timeOfDay)
    {
        foreach (TimeSetting setting in timeSettings)
        {
            if (setting.timeOfDay == timeOfDay)
                return setting;
        }

        throw new Exception("Tried to get time setting that hasn't been defined!");
    }
    
    private async UniTask PlayTransition(TimeSetting previous, TimeSetting current, CancellationToken token = default)
    {
        float elapsed = 0;

        // initialize new audio
        EventInstance oldInstance = _currentInstance;
        EventInstance newInstance = RuntimeManager.CreateInstance(current.ambienceOptions[Random.Range(0, current.ambienceOptions.Length)]);
        newInstance.setVolume(0);
        newInstance.start();
        
        _currentInstance = newInstance;
        _currentSetting = current;

        while (elapsed < transitionDuration && !token.IsCancellationRequested)
        {
            elapsed += UnityEngine.Time.deltaTime;
            float t = elapsed / transitionDuration;
            
            LerpSettings(previous, current, t);
            oldInstance.setVolume(Mathf.Lerp(1, 0, t));
            newInstance.setVolume(Mathf.Lerp(0, 1, t));
            
            await Task.Yield();
        }
        
        LerpSettings(previous, current, 1);
        oldInstance.setVolume(Mathf.Lerp(1, 0, 1));
        newInstance.setVolume(Mathf.Lerp(0, 1, 1));
    }

    private void LerpSettings(TimeSetting from, TimeSetting to, float t)
    {
        // Update sun
        float curSunAngle = Mathf.Lerp(from.sunAngle, to.sunAngle, t);
        sun.transform.rotation = Quaternion.Euler(new Vector3(curSunAngle, 0, 0));
            
        // Update graphics
        RenderSettings.ambientLight = Color.Lerp(from.ambientColor, to.ambientColor, t);
        RenderSettings.fogDensity = Mathf.Lerp(from.fogDensity, to.fogDensity, t);
        RenderSettings.fogColor = Color.Lerp(from.fogColor, to.fogColor, t);
            
        // Update sky
        skyMaterial.SetFloat(AtmosphereThickness, Mathf.Lerp(from.skyThickness, to.skyThickness, t));
        skyMaterial.SetColor(SkyTint, Color.Lerp(from.skyColor, to.skyColor, t));
    }
    
    public string DebugName => "time";
    
    public void DrawDebugInfo()
    {
        if (GUILayout.Button("Advance Time"))
            AdvanceTime();
        
        if (GUILayout.Button("Reset Time"))
            ResetTime();
        
        GUILayout.Label(Game.Save.TimeOfDay.ToString());
        GUILayout.Label(_currentSetting.timeOfDay.ToString());
        GUILayout.Label(_currentSetting.sunAngle.ToString(CultureInfo.InvariantCulture));
    }

    [Serializable]
    public struct TimeSetting
    {
        [FormerlySerializedAs("time")]
        public TimeOfDay timeOfDay;
        
        [Header("Audio")]
        public EventReference[] ambienceOptions;
        
        [Header("Sky")]
        public float sunAngle;
        public Color ambientColor;
        public float skyThickness;
        public Color skyColor;
        
        [Header("Fog")]
        public float fogDensity;
        public Color fogColor;
        
    }
}

public enum TimeOfDay
{
    Morning,
    Noon,
    Evening,
    Night,
}
