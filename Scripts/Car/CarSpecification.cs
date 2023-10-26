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

    public List<Wheel> FrontWheels = new List<Wheel>();
    public List<Wheel> RearWheels = new List<Wheel>();

    public void Initialize(CarMovement carMovement)
    {
        foreach (Wheel frontWheel in FrontWheels)
            frontWheel.Iniitialize(carMovement);

        foreach (Wheel rearWheel in RearWheels)
            rearWheel.Iniitialize(carMovement); 
    }
}
