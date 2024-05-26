using UnityEngine;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    [SerializeField] private SceneLoader _sceneloader;
    [SerializeField] private Image _interface;
    [SerializeField] private Image _startPanel;
    private const int _levelIndex = 1;

    private void Start() => CursorRenderer.Visable(true);
   
    public void OnPlayButtonClick()
    {
        SettingsData data = SettingsSaveLoadUtils.LoadSettingsData();

        if (data.FirstStartGame)
        {
            _interface.Deactivate();
            _startPanel.Activate();
            data.FirstStartGame = false;
            SettingsSaveLoadUtils.SaveSettingsData(data);
            return;
        }
        else
            _sceneloader.SceneSwitch(_levelIndex);
    } 

    public void OnExitButtonClick() => Application.Quit();
}
