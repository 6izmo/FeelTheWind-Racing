using System.Collections;
using UnityEngine;
using UnityEngine.Events;


public class MainGameEvents : MonoBehaviour
{
    public static MainGameEvents Instance;

    [SerializeField] private UnityEvent LevelCompleteEvent;
    [SerializeField] private UnityEvent GameOverEvent;

    private ÑheckPointProcessing _processing;

    public bool IsGameOver { get; private set; }

    public bool LevelCompleted { get; private set; }

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

        IsGameOver = true;
        GameOverEvent?.Invoke();
    }

    private IEnumerator LevelComplete()
    {
        LevelCompleteEvent?.Invoke();
        LevelCompleted = true;
        float elapsedTime = 0f;
        float freezeTime = 5f;

        while (elapsedTime < freezeTime)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Time.timeScale = 0f;
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
