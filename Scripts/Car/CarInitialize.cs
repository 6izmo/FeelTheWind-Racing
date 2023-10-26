using UnityEngine;

public class CarInitialize : MonoBehaviour
{
    [SerializeField] private InputValue _inputValue;
    private CarSpecification _carSpecification;
    private CarMovement _carMovement;
    private CarEffects _carEffects;
    private CarAudio _carAudio;

    private void Start()
    {
        _carMovement = GetComponent<CarMovement>();
        _carSpecification = GetComponent<CarSpecification>();
        _carEffects = GetComponent<CarEffects>();
        _carAudio = GetComponent<CarAudio>();

        _carSpecification.Initialize(_carMovement);
        _carMovement.Initialize(_carSpecification, _carAudio, _inputValue);
        _carEffects.Initialize(_carMovement);
        _carAudio.Initialize(_carMovement, _inputValue);
    }
}

