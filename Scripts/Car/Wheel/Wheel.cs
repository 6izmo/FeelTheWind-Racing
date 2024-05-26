using UnityEngine;

public class Wheel : MonoBehaviour
{
    public enum WheelPos
    {
        FrontLeft,
        FrontRight,
        RearLeft,
        RearRight,
    }

    [Header("Suspension")]
    [SerializeField] private float _restLength;
    [SerializeField] protected float _wheelRadius;
    [SerializeField] private float _springTravel;
    [SerializeField] private float _springStiffness;
    [SerializeField] private float _damperStiffness;

    [Header("GroundCheck")]
    [SerializeField] private float _distanceRay;
    [SerializeField] private LayerMask _asphaltLayer;
    private bool _isGrounded = false;
    private bool _isAsphalt = false;
    private int _waterLayer = 4;
    private RaycastHit _hit;

    [Header("Steering")]
    [SerializeField] protected float _mass;
    [SerializeField] protected float _gripFactor;
    protected float _currentMass;
    protected float _currentGripFactor;

    [Header("Braking")]
    [SerializeField] private float _brakingForce = 70f;
    protected float _currentBrakingForce;
    protected bool _isBraking = false;

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
    [SerializeField] protected WheelPos _position;
    protected CarMovement _carMovement;
    protected Rigidbody _carRigidbody;

    private float _startFrontWheelMass = 30f;
    private float _desiredAcceleration;
    protected float _currentSteerAngle;
    protected bool _isDrifting = false;

    public RaycastHit Hit => _hit;

    public WheelPos Position => _position;

    public bool IsGrounded => _isGrounded;

    public bool IsAsphalt => _isAsphalt;

    public bool IsBraking => _isBraking;

    public bool IsDrifting => _isDrifting;

    public float CarMovementSpeed => _carMovement.Speed;

    public float CurrentSteerAngle => _currentSteerAngle;

    public void Iniitialize(CarMovement carMovement)
    {
        _carMovement = carMovement;

        _carRigidbody = transform.root.GetComponent<Rigidbody>();
        _wheelView.Initialize(this, _carRigidbody, _wheelRadius);
        _currentMass = _position == WheelPos.FrontLeft || _position == WheelPos.FrontRight ? _startFrontWheelMass : _mass;

        _carMovement.ChangeGear += ChangeGear;
    }

    private void Start()
    {
        _maxLength = _restLength + _springTravel;
        _minLength = _restLength - _springTravel;
        _currentGripFactor = _gripFactor;
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

    private void GroudCheck()
    {
        _isGrounded = Physics.Raycast(transform.position, transform.up * -1f, out _hit, _distanceRay, ~(1 << _waterLayer)) ? true : false;
        _isAsphalt = (_hit.collider != null && _hit.collider.gameObject.layer == 7) ? true : false;
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

        if (_position == WheelPos.FrontLeft || _position == WheelPos.FrontRight)
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
