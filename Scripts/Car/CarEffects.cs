using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.PostProcessing;


public class CarEffects : MonoBehaviour
{
    [Header("Lights")]
    [SerializeField] private Light[] _rearLights;
    private bool _isActive;

    [Header("PostProcessing")]
    [SerializeField] private PostProcessVolume _postProcessVolume;
    private ChromaticAberration _chromaticAberration;
    private MotionBlur _motionBlur;
    private bool _activeMotionSetting;

    [Header("CameraEffects")]
    [SerializeField] private float _angleViewEffects;
    private float _currentAngleView;

    private Camera _camera;
    private CarMovement _carMovement;

    [Header("NitroEffects")]
    [SerializeField] private ParticleSystem _nitroParticle;
    private Coroutine _nitroEffects;
    private bool _isNitroEffects = false;

    public void Initialize(CarMovement carMovement)
    {
        _carMovement = carMovement;
        _postProcessVolume.profile.TryGetSettings(out _chromaticAberration);
        _postProcessVolume.profile.TryGetSettings(out _motionBlur);

        _camera = Camera.main;
        _currentAngleView = _camera.fieldOfView;

        _carMovement.InWater += WaterEffects;
        _carMovement.NitroEffects += NitroEffects;
        _carMovement.RearLightsActive += RearLights;

        _activeMotionSetting = SettingsSaveLoadUtils.LoadSettingsData().MotionBlur;
    }

    private void RearLights(bool active)
    {
        foreach (var light in _rearLights)
            light.gameObject.SetActive(active);
    }

    private void NitroEffects(bool active)
    {                      
        _isNitroEffects = active;

        _nitroParticle.gameObject.SetActive(active);
        _motionBlur.active = active && _activeMotionSetting;
        _chromaticAberration.active = active;

        float angleView = active ? _angleViewEffects : _currentAngleView;

        if (_nitroEffects != null)
            StopCoroutine(_nitroEffects);

        _nitroEffects = StartCoroutine(CameraFieldOfView(angleView));
    }

    private IEnumerator CameraFieldOfView(float angleView)
    {
        float speedCamera = 2f;
        while (Mathf.Round(_camera.fieldOfView) != angleView)
        {
            _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, angleView, speedCamera * Time.deltaTime);
            yield return null;
        }
        _camera.fieldOfView = Mathf.Round(_camera.fieldOfView);
    }

    private void WaterEffects() => _camera.GetComponent<CameraMovement>().enabled = false;
  
    private void OnDisable()
    {
        _carMovement.InWater -= WaterEffects;
        _carMovement.NitroEffects -= NitroEffects;
        _carMovement.RearLightsActive -= RearLights;
    }
}
