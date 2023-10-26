using System;
using UnityEngine;
using System.Collections;

public class Wheel : MonoBehaviour
{
    [Header("Suspension")]
    [SerializeField] private float _restLength;
    [SerializeField] protected float _wheelRadius;
    [SerializeField] private float _springTravel;
    [SerializeField] private float _springStiffness;
    [SerializeField] private float _damperStiffness;

    [Header("GroundCheck")]
    [SerializeField] private float _distanceRay;
    [SerializeField] private LayerMask _asphaltLayer;
    private int _waterLayer = 4;
    private RaycastHit _hit;

    [Header("Steering")]
    [SerializeField] private float _mass;
    [SerializeField] private float _gripFactor;
    [SerializeField] private float _steerTime;
    public float _currentMass;
    private float _startFrontWheelMass = 30f; 
    private float _currentGripFactor;
    private float _currentSteerAngle;

    [Header("Braking")]
    [SerializeField] private float _brakingForce = 70f;
    private float _currentBrakingForce;
    private bool _isBraking = false;

    [Header("Drifting")]
    private Coroutine _driftCoroutine;
    private bool _isDrifting = false;

    #region Acceleration
    private Vector3 _acceleration;
    private float _desiredAcceleration;
    #endregion

    #region Positions
    public bool FrontLeft;
    public bool FrontRight;
    public bool RearLeft;
    public bool RearRight;
    #endregion

    #region Suspension
    private Vector3 _suspensionForce;
    private float _maxLength;
    private float _minLength;
    private float _lastLength;
    private float _springVelocity;
    private float _springForce;
    private float _springLength;
    private float _damperForce;
    #endregion

    [SerializeField] private WheelView _wheelView;

    private CarMovement _carMovement;
    private Rigidbody _carRigidbody;

    public bool IsGrounded { get; private set; }

    public bool CanDrifting { get; private set; }

    public bool IsAsphalt { get; private set; }

    public float CurrentSteerAngle { get => _currentSteerAngle; }

    public float CarMovementSpeed { get => _carMovement.Speed; }

    public RaycastHit Hit { get => _hit; }

    public void Iniitialize(CarMovement carMovement)
    {
        _carMovement = carMovement;

        _carRigidbody = transform.root.GetComponent<Rigidbody>();
        _wheelView.Initialize(this, _carRigidbody, _wheelRadius);

        _carMovement.ChangeGear += ChangeGear;
    }

    private void Start()
    {
        _maxLength = _restLength + _springTravel;
        _minLength = _restLength - _springTravel;
        _currentGripFactor = _gripFactor;
        _currentMass = FrontLeft || FrontRight ? _startFrontWheelMass : _mass;
    }

    private void FixedUpdate()
    {
        GroudCheck();
        if (IsGrounded)
        {
            SuspensionForce();
            Friction();
        }
    }

    private void Update()
    {
        _wheelView.TireTrailEffect((_isBraking || _isDrifting) && IsGrounded && IsAsphalt);
    }

    private void SuspensionForce()
    {
        _lastLength = _springLength;
        _springLength = _hit.distance - _wheelRadius;
        _springForce = Mathf.Clamp(_springForce, _minLength, _maxLength);
        _springForce = _springStiffness * (_restLength - _springLength);

        _springVelocity = (_lastLength - _springLength) / Time.fixedDeltaTime;
        _damperForce = _damperStiffness * _springVelocity;

        _suspensionForce = (_springForce + _damperForce) * transform.up;

        _carRigidbody.AddForceAtPosition(_suspensionForce, transform.position);
    }

    private void Friction()
    {
        Vector3 frictionDirection = transform.right;
        Vector3 wheelWorldVelocity = _carRigidbody.GetPointVelocity(transform.position);
        float frictionForce = Vector3.Dot(frictionDirection, wheelWorldVelocity);
        float desiredVelocity = -frictionForce * _currentGripFactor;
        _desiredAcceleration = desiredVelocity / Time.fixedDeltaTime;

        _carRigidbody.AddForceAtPosition(_desiredAcceleration * frictionDirection * _currentMass, transform.position);
    }

    public void Acceleration(float velocity)
    {
        if (IsGrounded && !_isBraking)
        {
            Vector3 frictionDirection = -transform.forward;
            Vector3 wheelWorldVelocity = _carRigidbody.GetPointVelocity(transform.position);
            float frictionForce = Vector3.Dot(frictionDirection, wheelWorldVelocity);
            float desiredVelocity = frictionForce * _currentGripFactor * _currentBrakingForce;
            velocity += desiredVelocity;
            _acceleration = transform.forward * (velocity / Time.fixedDeltaTime);
            _carRigidbody.AddForceAtPosition(_acceleration, transform.position);
        }
    }

    public void Steering(float steerAngle)
    {
        _currentSteerAngle = Mathf.Lerp(_currentSteerAngle, steerAngle, _steerTime * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(Vector3.up * _currentSteerAngle);

        CanDrifting = (Mathf.Round(_currentSteerAngle) == Mathf.Round(steerAngle)) && steerAngle != 0 ? true : false;
    }

    public void Drifting(bool canDrifiting, CarAudio carAudio)
    {
        if (!_isDrifting && canDrifiting && IsGrounded)
        {
            _isDrifting = true;
            _driftCoroutine = StartCoroutine(DriftingCor());
        }
        else if ((_isDrifting && !canDrifiting) || !IsGrounded)
        {
            _isDrifting = false;
            carAudio.TireWhistling(false);
            if (_driftCoroutine != null)
                StopCoroutine(_driftCoroutine);
            _currentMass = _mass;
        }

        if(_isDrifting && IsGrounded && IsAsphalt)
            carAudio.TireWhistling(true);
        else if (!IsAsphalt || !IsGrounded)
            carAudio.TireWhistling(false);

    }

    private IEnumerator DriftingCor()
    {
        float driftTime = 5f;
        float elapsedTime = 0f;
        float percent = (1f / 15f);
            
        while (elapsedTime < driftTime)
        {
            _currentMass -= percent;
            _currentMass = _currentMass <= 0 ? 0 : _currentMass;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private void GroudCheck()
    {
        IsGrounded = Physics.Raycast(transform.position, transform.up * -1f, out _hit, _distanceRay, ~(1 << _waterLayer)) ? true : false;
        IsAsphalt = (_hit.collider != null && _hit.collider.gameObject.layer == 7) ? true : false;
    }

    public void HandleBrake(bool active)
    {
        _isBraking = active;
        _currentBrakingForce = _isBraking ? _brakingForce : 1f;
    }

    private void ChangeGear(int oldGear, int currentGear)
    {
        float frontMassPercent = 0.35f;

        float frontPercentSlip = 1.35f;
        float rearPercentSlip = 1.15f;

        if (FrontLeft || FrontRight)
        {
            _currentGripFactor = _gripFactor - (currentGear * frontPercentSlip);

            switch (currentGear)
            {
                case 1:
                    _currentMass = _startFrontWheelMass;
                    break;
                case 2:
                    _currentMass = _mass;
                    break;
                case 5:
                    _currentMass = _mass - 3 * (currentGear * frontMassPercent);
                    break;
                default:
                    _currentMass = _mass - (currentGear * frontMassPercent);
                    break;
            }
        }
        else
            _currentGripFactor = _gripFactor - (currentGear * rearPercentSlip);

        _currentGripFactor = _currentGripFactor < 0f ? 0f : _currentGripFactor;
    }

    private void OnDisable() => _carMovement.ChangeGear -= ChangeGear;
}
