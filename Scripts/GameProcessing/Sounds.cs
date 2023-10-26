using System.Collections;
using UnityEngine;

public class Sounds : MonoBehaviour
{
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _soundSource;

    private Coroutine _volumeCor;
    private float _speed = 5f;
    private float _defaultMusicVolume;
    private float _quietMusicVolume = 0.2f;

    private void Start() => _defaultMusicVolume = _musicSource.volume;
   
    public void MuteMusic(bool mute) 
    {
        float volume = mute ? _quietMusicVolume : _defaultMusicVolume;

        if (_volumeCor != null)
            StopCoroutine(_volumeCor);

        _volumeCor = StartCoroutine(SetVolume(volume));
    }

    public IEnumerator SetVolume(float volume)
    {
        float elapsedTime = 0f;
        float volumeTime = 2f;
        while (elapsedTime < volumeTime)
        {
            _musicSource.volume = Mathf.Lerp(_musicSource.volume, volume, Time.deltaTime * _speed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

   
}
