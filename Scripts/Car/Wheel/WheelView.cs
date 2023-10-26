using UnityEngine;

public class WheelView : MonoBehaviour
{
    [Header("Effects")]
    [SerializeField] private ParticleSystem _dustParticle;
    [SerializeField] private TrailRenderer _asphaltTrailRenderer;
    private bool _isDustEffectPlaying = false;

    private float _rotationZ;
    private float _rotation;
    private float _wheelRadius;

    private Wheel _wheelModel;
    private CarEffects _carEffects;
    private Rigidbody _carRigidbody;

    public void Initialize(Wheel wheelModel, Rigidbody carRigidbody, float wheelRadius)
    {
        _wheelModel = wheelModel;
        _carRigidbody = carRigidbody;
        _wheelRadius = wheelRadius;
    }

    private void Start()
    {
        _rotationZ = transform.localEulerAngles.z;
        _carEffects = transform.root.GetComponent<CarEffects>();
    }

    private void Update()
    {
        UpdateEffects();
        UpdateRotation();
        UpdateTransform();
    }
    private void UpdateRotation()
    {
        float delta = Time.fixedDeltaTime;
        float carSpeed = _carRigidbody.velocity.magnitude;
        float rmp = (carSpeed / (Mathf.PI * _wheelRadius * 2)) * 60;
        _rotation = Mathf.Repeat(_rotation + delta * rmp * 360 / 60, 360);
        transform.localRotation = Quaternion.Euler(_rotation, _wheelModel.CurrentSteerAngle, _rotationZ);
    }

    private void UpdateTransform()
    {
        transform.localPosition =
            new Vector3(transform.localPosition.x,
            Mathf.Clamp(_wheelModel.Hit.point.y + _wheelRadius, _wheelRadius, _wheelRadius + 0.05f),
            transform.localPosition.z);
    }
    private void UpdateEffects()
    {
        _dustParticle.startSize = _wheelModel.CarMovementSpeed > 150f ? 1f : 0.6f;

        if (_wheelModel.IsGrounded && !_wheelModel.IsAsphalt && !_dustParticle.isPlaying && _wheelModel.CarMovementSpeed > 75f)       
            _dustParticle.Play();      
        else if ((!_wheelModel.IsGrounded) || _wheelModel.IsAsphalt || _wheelModel.CarMovementSpeed < 75f)       
            _dustParticle.Stop();       
    }

    public void TireTrailEffect(bool active)
    {
        if (!_asphaltTrailRenderer.emitting && active)
            _asphaltTrailRenderer.emitting = true;
        else if (_asphaltTrailRenderer.emitting && !active)
            _asphaltTrailRenderer.emitting = false;
    }
}
