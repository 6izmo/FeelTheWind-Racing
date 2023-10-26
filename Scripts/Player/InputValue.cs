using UnityEngine;

public class InputValue : MonoBehaviour 
{
    [SerializeField] private LevelButtons _levelButtons;
    private Controls _controls;

    public bool IsHandleBrakeInput { get; private set; }
    public float SteeringInput { get; private set; }
    public float AccelerationInput { get; set; }
    public bool IsNitroInput { get; private set; }


    private void Awake()
    {
        _controls = new Controls();
        _controls.Main.Esc.performed += context => _levelButtons.PausePanel();
    } 

    private void OnEnable() => _controls.Enable();

    private void Update()
    {
        AccelerationInput = _controls.Main.Acceleration.ReadValue<float>();
        SteeringInput = _controls.Main.Steering.ReadValue<float>();
        IsHandleBrakeInput = _controls.Main.HandleBrake.ReadValue<float>() > 0f ? true : false;
        IsNitroInput = _controls.Main.Nitro.ReadValue<float>() > 0f ? true : false;
    }

    private void OnDisable() => _controls.Disable();
}
