using UnityEngine;
using System.Collections;

public class RearWheel : Wheel
{
    private Coroutine _driftCoroutine;
    private Vector3 _acceleration;

    private void Start() => _currentMass = _mass;
   
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

        if (_isDrifting && IsGrounded && IsAsphalt)
            carAudio.TireWhistling(true);
        else if (!IsAsphalt || !IsGrounded)
            carAudio.TireWhistling(false);

    }

    private IEnumerator DriftingCor()
    {
        float driftTime = 5f;
        float elapsedTime = 0f;
        float percent = 5.5f;

        while (elapsedTime < driftTime)
        {
            _currentMass -= percent * Time.deltaTime;
            _currentMass = _currentMass <= 0 ? 0 : _currentMass;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}
