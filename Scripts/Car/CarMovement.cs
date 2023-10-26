using TMPro;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CarMovement : MonoBehaviour
{
    [Header("Speed")]
    [SerializeField] private Transform _speedometrArrow;
    private float _speed;
    private const float _speedConst = 2.4f;

    [Header("Gear")]
    [SerializeField] private int _gearCount = 5;
    [SerializeField] private TextMeshProUGUI _gearText;
    private List<int> _speedValue = new List<int>(6) { 5, 62, 112, 173, 229, 301 };
    private bool _canNextGear = true;
    private int _currentGear = 0;

    [Header("Acceleration")]
    [SerializeField] private float _startingForce = 9000f;
    private float _lastSpeed;
    private float _acceleration;
    private bool _accelerated;

    [Header("Steering")]
    private float _ackermannAngleLeft;
    private float _ackermannAngleRight;

    [Header("Nitro")]
    [SerializeField] private Image _nitroBar;
    [SerializeField] private float _accelerationTime = 5f;
    private float _nitroPercent;
    private bool _canHandleNitro = true;
    private bool _isNitroAcceleration = false;
    private Coroutine _nitroCoroutine;

    private InputValue _inputValue;
    private CarAudio _carAudio;
    private Rigidbody _rigidbody;
    private CarSpecification _carSpec;
    private List<Wheel> _allWheels;

    private bool _canDrifting;
    private bool _isBrakingSound;

    public event Action InWater;
    public event Action<int> CollisionSound;
    public event Action<int, int> ChangeGear;
    public event Action<bool> NitroEffects;
    public event Action<bool> RearLightsActive;

    public bool CanMove { get; set; }

    public float Speed =>  _speed;

    public float MaxSpeed => _carSpec.TopSpeed;

    public bool Accelerated => _accelerated;

    public void Initialize(CarSpecification carSpecification, CarAudio carAudio, InputValue inputValue)
    {
        _rigidbody = GetComponent<Rigidbody>();

        _carSpec = carSpecification;
        _inputValue = inputValue;
        _carAudio = carAudio;
        _nitroPercent = (1 / _accelerationTime);

        _allWheels = new List<Wheel>();
        _allWheels.AddRange(from FrontWheel frontWheel in _carSpec.FrontWheels select frontWheel);
        _allWheels.AddRange(from RearWheel rearWheel in _carSpec.RearWheels select rearWheel);
    }

    private void Update()
    {
        Steering();
        UpdateGear();
        HandleBrake();
        HandleNitro();
        SpeedUpdate();
    }

    private void FixedUpdate() => Acceleration();

    private void Acceleration()
    {      
        _lastSpeed = _acceleration;
        _acceleration = ((_startingForce * _inputValue.AccelerationInput) + (_currentGear * _carSpec.EnginePower * Physics.gravity.y)) * Time.deltaTime;
        _accelerated = _lastSpeed < _acceleration ? true : false;

        if (!CanMove)
            return;

        foreach (RearWheel rearWheel in _carSpec.RearWheels)
            rearWheel.Acceleration(_acceleration);
    }

    private void Steering()
    { 
        if (_inputValue.SteeringInput > 0f)
        {
            _ackermannAngleLeft = Mathf.Rad2Deg * Mathf.Atan(_carSpec.WheelBase / (_carSpec.TurnRadius + (_carSpec.RearTrack / 2))) * _inputValue.SteeringInput;
            _ackermannAngleRight = Mathf.Rad2Deg * Mathf.Atan(_carSpec.WheelBase / (_carSpec.TurnRadius - (_carSpec.RearTrack / 2))) * _inputValue.SteeringInput;
        }
        else if (_inputValue.SteeringInput < 0f)
        {
            _ackermannAngleLeft = Mathf.Rad2Deg * Mathf.Atan(_carSpec.WheelBase / (_carSpec.TurnRadius - (_carSpec.RearTrack / 2))) * _inputValue.SteeringInput;
            _ackermannAngleRight = Mathf.Rad2Deg * Mathf.Atan(_carSpec.WheelBase / (_carSpec.TurnRadius + (_carSpec.RearTrack / 2))) * _inputValue.SteeringInput;
        }
        else
        {
            _ackermannAngleLeft = 0f;
            _ackermannAngleRight = 0f;
        }

        foreach (FrontWheel frontWheels in _carSpec.FrontWheels)
        {
            if(frontWheels.Position == Wheel.WheelPos.FrontLeft)
                frontWheels.Steering(_ackermannAngleLeft);
            else
                frontWheels.Steering(_ackermannAngleRight);
 
            _canDrifting = frontWheels.CanDrifting;
        }

        foreach (RearWheel rearWheels in _carSpec.RearWheels)       
            rearWheels.Drifting(_canDrifting, _carAudio);       
    }

    private void SpeedUpdate()
    {       
        _speed = _rigidbody.velocity.magnitude * _speedConst;
        _speedometrArrow.localRotation = Quaternion.Euler(0, 0, -_speed);

        if (_inputValue.AccelerationInput > 0)
            RearLightsActive?.Invoke(false);
        else
            RearLightsActive?.Invoke(true);
    }

    private void UpdateGear()
    {
        if (_currentGear < _gearCount && _speed > _speedValue[_currentGear] && _canNextGear)
            StartCoroutine(NextGear());
        else if (_currentGear >= 1 && _speed < _speedValue[_currentGear - 1] && _canNextGear)
            StartCoroutine(BackGear());
    }

    private IEnumerator NextGear()
    {
        _canNextGear = false;
        ChangeGear?.Invoke(_currentGear, _currentGear + 1);
        _currentGear++;
        _gearText.text = $"{_currentGear:F0}";
        yield return new WaitForEndOfFrame();
        _canNextGear = true;
    }

    private IEnumerator BackGear()
    {
        _canNextGear = false;
        ChangeGear?.Invoke(_currentGear, _currentGear - 1);
        _currentGear--;
        _gearText.text = _currentGear == 0 ? $"{1:F0}" : $"{_currentGear:F0}";
        yield return new WaitForEndOfFrame();
        _canNextGear = true;
    }

    private void HandleBrake()
    {       
        float controlSumm = 0;
        foreach (Wheel wheel in _allWheels)
        {
            wheel.HandleBrake(_inputValue.IsHandleBrakeInput);
            controlSumm += wheel.IsAsphalt ? 1 : 0;
        }
        if (controlSumm > 0 && !_isBrakingSound && _inputValue.IsHandleBrakeInput)
        {
            _isBrakingSound = true;
            _carAudio.TireWhistling(_isBrakingSound);
        }
        else if ((controlSumm == 0 || !_inputValue.IsHandleBrakeInput) && _isBrakingSound)
        {
            _isBrakingSound = false;
            _carAudio.TireWhistling(_isBrakingSound);
        }
    }

    private void HandleNitro()
    {
        if (_inputValue.IsNitroInput && !_isNitroAcceleration && _canHandleNitro)
        {
            NitroActive(true);
            _nitroCoroutine = StartCoroutine(NitroConsumption());
        }            
        else if (!_inputValue.IsNitroInput && _isNitroAcceleration)
        {
            NitroActive(false);
            if (_nitroCoroutine != null)
                StopCoroutine(_nitroCoroutine);
        }
    }

    private void NitroActive(bool active)
    {
        _isNitroAcceleration = active;
        NitroEffects?.Invoke(active);

        _startingForce = active ? 1000000f : 750000f;
    }
        
    private IEnumerator NitroConsumption()
    {
        while (_accelerationTime >= 0)
        {
            _inputValue.AccelerationInput = 1f;
            _nitroBar.fillAmount -= (_nitroPercent * Time.deltaTime);
            _accelerationTime -= Time.deltaTime;
            yield return null;
        }
        NitroActive(false);
        _canHandleNitro = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision != null && (collision.gameObject.layer == 8 || collision.gameObject.layer == 9))
        {
            —alculate—ollisions.Direction direction = —alculate—ollisions.DirectionCollsion(collision.contacts[0].normal, transform.forward);
            if(direction == —alculate—ollisions.Direction.Forward)
            {
                CollisionSound?.Invoke(_currentGear);
                _rigidbody.velocity = Vector3.zero;
            }           
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other != null && other.gameObject.layer == 4)
        {
            InWater?.Invoke();
            MainGameEvents.Instance.GameOver();
            MainGameEvents.Instance.DeactivateCar(this);
        }   
    }
}

