using TMPro;
using UnityEngine;

public class FPS : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private float _hudRefreshRate = 1f;
    private float _timer;

    private void Awake()
    {
        SettingsData data = SettingsSaveLoadUtils.LoadSettingsData();
        _text.gameObject.SetActive(data.FPS);
        enabled = data.FPS;
    }

    private void Update()
    {
        if (Time.unscaledTime > _timer)
        {
            _text.text = "FPS: " + $"{1f / Time.unscaledDeltaTime:F0}";
            _timer = Time.unscaledTime + _hudRefreshRate;
        }
    }
}
