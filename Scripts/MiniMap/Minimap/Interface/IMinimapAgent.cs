using UnityEngine;

public interface IMinimapAgent  
{
    Pose Pose { get; }
    string Name { get; }
    Color IconColor { get; }
}
