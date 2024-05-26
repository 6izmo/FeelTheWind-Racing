using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class MainGameEvents : MonoBehaviour
{
    public static MainGameEvents Instance;

    private ÑheckPointProcessing _processing;
    [SerializeField] private UnityEvent LevelCompleteEvent;
    [SerializeField] private UnityEvent GameOverEvent;
    private bool _gameOver = false;
    private bool _levelCompleted = false;

    public bool IsGameOver => _gameOver;

    public bool LevelCompleted => _levelCompleted;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        Time.timeScale = 1f;
        _processing = GetComponent<ÑheckPointProcessing>();
        _processing.LevelComplete += () => StartCoroutine(LevelComplete());
    }

    public void GameOver()
    {
        if (LevelCompleted)
            return;

        _gameOver = true;
        GameOverEvent?.Invoke();
        CursorRenderer.Visable(true);
    }

    private IEnumerator LevelComplete()
    {
        LevelCompleteEvent?.Invoke();
        _levelCompleted = true;
        float elapsedTime = 0f;
        float freezeTime = 5f;

        while (elapsedTime < freezeTime)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Time.timeScale = 0f;
        CursorRenderer.Visable(true);
    }

    public void DeactivateCar(Component carComponent) => StartCoroutine(DeactivateByTime(carComponent));

    private IEnumerator DeactivateByTime(Component component)
    {
        float time = 0.65f;
        while (time > 0)
        {
            time -= Time.deltaTime;
            yield return null;
        }
        component.Deactivate();
    }
}
