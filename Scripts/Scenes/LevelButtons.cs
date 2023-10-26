using UnityEngine;
using UnityEngine.UI;

public class LevelButtons : MonoBehaviour
{
    [SerializeField] private SceneLoader _sceneLoader;
    [SerializeField] private Image _pausePanel;
    private bool _paused = false;
    private const int _levelIndex = 1;
    private const int _menuIndex = 0;

    public bool CanPaused { get; set; }

    public void PausePanel()
    {
        if (!CanPaused || MainGameEvents.Instance.LevelCompleted)
            return;

        _paused = !_paused;
        Time.timeScale = _paused ? 0.0f : 1.0f;
        _pausePanel.gameObject.SetActive(_paused);
    }

    public void OnRestartClickButton()
    {
        _pausePanel.Deactivate();
        Time.timeScale = 1.0f;
        _sceneLoader.SceneSwitch(_levelIndex);
    } 

    public void OnMenuClickButton()
    {
        _pausePanel.Deactivate();
        Time.timeScale = 1.0f;
        _sceneLoader.SceneSwitch(_menuIndex);
    }
}
