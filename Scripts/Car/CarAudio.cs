using UnityEngine;

public class CarAudio : MonoBehaviour
{
    [Header("AudioSources")]
    [SerializeField] AudioSource _audioSourceIdle;
    [SerializeField] AudioSource _audioSourceMove;
    [SerializeField] AudioSource _audioSourceBraking;
    [SerializeField] AudioSource _nitroSource;
    [SerializeField] AudioSource _collisionSource;
    private bool _canPlay = true;

    [Header("AudioClips")]
    [SerializeField] private AudioClip _startMedium;
    [SerializeField] private AudioClip _durationMedium;
    [SerializeField] private AudioClip _braking;
    [SerializeField] private AudioClip _nitroStart;
    [SerializeField] private AudioClip _nitro;

    [Header("PitchSettings")]
    [SerializeField] private float _pitchMultiplier;
    [SerializeField] private float _lowPitchMin;
    [SerializeField] private float _lowPitchMax;
    [SerializeField] private float _highPitchMultiplier;
    private float _pitch;


    #region Volume
    private float _decFade;
    private float _accFade;
    private float _highFade;
    private float _lowFade;
    #endregion

    private CarMovement _carMovement;
    private InputValue _inputValue;

    public void Initialize(CarMovement carMovement, InputValue inputValue)
    {
        _carMovement = carMovement;
        _inputValue = inputValue;

        _carMovement.NitroEffects += NitroSound;
        _carMovement.ChangeGear += ChangeGear;
        _carMovement.CollisionSound += CollisionSound;
    }

    private void Update()
    {
        UpdateVolume();
        UpdatePitch();
    }

    private void ChangeGear(int old, int current)
    {
        switch (current)
        {
            case 0:
                PlayClipIdle();
                break;
            case 1:
                PlayClipMove(_durationMedium);
                break;
        }

    }
    private void UpdateVolume()
    {
        _accFade = Mathf.Abs((_inputValue.AccelerationInput > 0f && _carMovement.Accelerated) ? _inputValue.AccelerationInput : 0);
        _decFade = 1 - _accFade;
        _highFade = Mathf.InverseLerp(0.2f, 0.8f, (3 * _carMovement.Speed) / _carMovement.MaxSpeed);
        _lowFade = 1 - _highFade;

        _highFade = 1 - ((1 - _highFade) * (1 - _highFade));
        _lowFade = 1 - ((1 - _lowFade) * (1 - _lowFade));
        _accFade = 1 - ((1 - _accFade) * (1 - _accFade));
        _decFade = 1 - ((1 - _decFade) * (1 - _decFade));


        _audioSourceIdle.volume = _lowFade * _decFade;
        _audioSourceBraking.volume = _highFade * _decFade;
        _audioSourceMove.volume = _highFade * _decFade;
    }

    private void UpdatePitch()
    {
        _pitch = Mathf.Lerp(_lowPitchMin, _lowPitchMax, _carMovement.Speed / _carMovement.MaxSpeed);
        _audioSourceMove.pitch = _pitch * _pitchMultiplier;
    }

    private void PlayClipIdle()
    {
        if (!_audioSourceIdle.isPlaying)
            _audioSourceIdle.Play();
    }

    private void PlayClipMove(AudioClip clip)
    {
        if (!_audioSourceMove.isPlaying && _canPlay)
        {
            _audioSourceMove.clip = clip;
            _audioSourceMove.Play();
        }
    }

    private void CollisionSound(int currentGear)
    {
        _collisionSource.volume =  currentGear < 3 ? 0.2f * currentGear : 1f;
        _collisionSource.Play();
    }
 
    private void NitroSound(bool active)
    {
        if (active)
        {
            _nitroSource.PlayOneShot(_nitroStart);
            _nitroSource.Play();
        }
        else
            _nitroSource.Stop();
    }

    public void TireWhistling(bool active)
    {
        if (active && !_audioSourceBraking.isPlaying)
        {
            _canPlay = false;
            _audioSourceBraking.clip = _braking;
            _audioSourceBraking.Play();
        }
        else if (!active && _audioSourceBraking.isPlaying)
        {
            _audioSourceBraking.Stop();
            _canPlay = true;
            PlayClipMove(_durationMedium);
        }
    }

    private void OnDisable()
    {
        _carMovement.NitroEffects -= NitroSound;
        _carMovement.ChangeGear -= ChangeGear;
        _carMovement.CollisionSound -= CollisionSound;
    }
}
