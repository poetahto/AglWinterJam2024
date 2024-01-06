using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using FMOD.Studio;
using FMODUnity;
using poetools.Console.Commands;
using UnityEngine;
using Random = UnityEngine.Random;

public class TimeSystem : MonoBehaviour, IConsoleDebugInfo
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
    private Material skyMaterial;

    private TimeSetting _currentSetting;
    private EventInstance _currentInstance;
    private CancellationTokenSource _animationCts;

    private void Start()
    {
        _currentSetting.time = Time.Invalid;
        ApplyCurrentTimeSettings();
    }

    private void OnDestroy()
    {
        _animationCts?.Cancel();
    }

    public void ResetTime()
    {
        Game.Save.Time = Time.Morning;
        ApplyCurrentTimeSettings();
    }

    public void AdvanceTime()
    {
        Time time = Game.Save.Time;

        Game.Save.Time = time switch {
            Time.Morning => Time.Noon,
            Time.Noon => Time.Evening,
            Time.Evening => Time.Night,
            _ => time,
        };
        ApplyCurrentTimeSettings();
    }

    private void ApplyCurrentTimeSettings()
    {
        // pretty much, find the relevant settings and start animating them
        foreach (TimeSetting timeSetting in timeSettings)
        {
            if (timeSetting.time == Game.Save.Time && timeSetting.time != _currentSetting.time)
            {
                _animationCts?.Cancel();
                _animationCts = new CancellationTokenSource();
                PlayTransition(_currentSetting, timeSetting, _animationCts.Token).Forget();
                break;
            }
        }
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
            
            // Update sun
            float curSunAngle = Mathf.Lerp(previous.sunAngle, current.sunAngle, t);
            sun.transform.rotation = Quaternion.Euler(new Vector3(curSunAngle, 0, 0));
            
            // Update graphics
            RenderSettings.ambientLight = Color.Lerp(previous.ambientColor, current.ambientColor, t);
            RenderSettings.fogDensity = Mathf.Lerp(previous.fogDensity, current.fogDensity, t);
            RenderSettings.fogColor = Color.Lerp(previous.fogColor, current.fogColor, t);
            
            // Update sky
            skyMaterial.SetFloat(AtmosphereThickness, Mathf.Lerp(previous.skyThickness, current.skyThickness, t));
            skyMaterial.SetColor(SkyTint, Color.Lerp(previous.skyColor, current.skyColor, t));
            
            // Update audio
            oldInstance.setVolume(Mathf.Lerp(1, 0, t));
            newInstance.setVolume(Mathf.Lerp(0, 1, t));
            
            await Task.Yield();
        }
    }
    
    public string DebugName => "time";
    
    public void DrawDebugInfo()
    {
        if (GUILayout.Button("Advance Time"))
            AdvanceTime();
        
        if (GUILayout.Button("Reset Time"))
            ResetTime();
        
        GUILayout.Label(Game.Save.Time.ToString());
        GUILayout.Label(_currentSetting.time.ToString());
        GUILayout.Label(_currentSetting.sunAngle.ToString(CultureInfo.InvariantCulture));
    }

    [Serializable]
    public struct TimeSetting
    {
        public Time time;
        
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

public enum Time
{
    Invalid = -1,
    Morning,
    Noon,
    Evening,
    Night,
}
