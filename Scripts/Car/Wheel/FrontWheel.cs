using UnityEngine;

public class FrontWheel : Wheel
{
    [SerializeField] private float _steerTime;
    private bool _canDrifting = false;

    public bool CanDrifting => _canDrifting;

    public void Steering(float steerAngle)
    {
        _currentSteerAngle = Mathf.Lerp(_currentSteerAngle, steerAngle, _steerTime * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(Vector3.up * _currentSteerAngle);

        _canDrifting = (Mathf.Round(_currentSteerAngle) == Mathf.Round(steerAngle)) && steerAngle != 0 ? true : false;
    }
}
