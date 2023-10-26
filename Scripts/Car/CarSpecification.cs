using System;
using System.Collections.Generic;
using UnityEngine;

public class CarSpecification : MonoBehaviour
{
    [field: SerializeField] public float EnginePower { get; private set; }
    [field: SerializeField] public float WheelBase { get; private set; }
    [field: SerializeField] public float RearTrack { get; private set; }
    [field: SerializeField] public float TurnRadius { get; private set; }
    [field: SerializeField] public float TopSpeed { get; private set; }

    public List<FrontWheel> FrontWheels = new List<FrontWheel>();
    public List<RearWheel> RearWheels = new List<RearWheel>();

    public void Initialize(CarMovement carMovement)
    {
        foreach (FrontWheel frontWheel in FrontWheels)
            frontWheel.Iniitialize(carMovement);

        foreach (RearWheel rearWheel in RearWheels)
            rearWheel.Iniitialize(carMovement); 
    }
}
