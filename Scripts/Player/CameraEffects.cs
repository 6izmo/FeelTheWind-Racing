using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class CameraEffects : MonoBehaviour
{
    private PostProcessLayer _postProcessLayer;
    private void Awake()
    {
        _postProcessLayer = GetComponent<PostProcessLayer>();
        _postProcessLayer.antialiasingMode = SettingsSaveLoadUtils.LoadSettingsData().AntiAliasing.Mode;
    }
}
