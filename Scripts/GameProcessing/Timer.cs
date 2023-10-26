using System.Collections;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [Header("Timers")]
    [SerializeField] private float _startTime;
    [SerializeField] private float _startTimeCheckPoint;
    [SerializeField] private TextMeshProUGUI _timerStartText;
    [SerializeField] private TextMeshProUGUI _timerCheckPointText;

    [Header("Sounds Effect")]
    [SerializeField] private Sounds _sounds;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip[] _audioClips; // i = 0,1,2,3 => theree, two, one, go
    [SerializeField] private AudioClip _kick;

    private float _currentTime;

    private void Start() => _currentTime = _startTime;

    private IEnumerator TickTime()
    {
        while(_currentTime > 0f)
        {
            _currentTime -= Time.deltaTime;
            float remainder = (int)_currentTime % 60;
            float division = (int)_currentTime / 60;

            if (_currentTime > 60f)
                _timerCheckPointText.text = remainder >= 10f ? $"{division}" + "." + $"{remainder}" : $"{division}" + "." + "0" + $"{remainder}";
            else
                _timerCheckPointText.text = $"{_currentTime:F1}";
            yield return null;
        }
        MainGameEvents.Instance.GameOver();
        this.enabled = false;
    }


    public void StartCoroutineTime(CarMovement carMovement) => StartCoroutine(TimeStartTick(carMovement));

    private IEnumerator TimeStartTick(CarMovement carMovement)
    {
        _timerStartText.gameObject.SetActive(true);
        while (_currentTime > 0)
        {
            _sounds.MuteMusic(true);
            _timerStartText.text = $"{_currentTime:F0}";
            switch (_currentTime) 
            {
                case 3:
                    _audioSource.PlayOneShot(_audioClips[0]);
                    break;
                case 2:
                    _audioSource.PlayOneShot(_audioClips[1]);
                    break;
                case 1:
                    _audioSource.PlayOneShot(_audioClips[2]);
                    break;
            }
            _audioSource.PlayOneShot(_kick);
            yield return new WaitForSeconds(1);
            _currentTime -= 1;
        }
        carMovement.CanMove = true;
        _timerStartText.text = "GO!";
        _audioSource.PlayOneShot(_audioClips[3]);
        _audioSource.PlayOneShot(_kick);
        _currentTime = _startTimeCheckPoint;
        _timerCheckPointText.gameObject.SetActive(true);
        StartCoroutine(TickTime());
        yield return new WaitForSeconds(1.2f);
        _sounds.MuteMusic(false);
        _timerStartText.gameObject.SetActive(false);
    }

    public void AddTime(float time) => _currentTime += time;
}
