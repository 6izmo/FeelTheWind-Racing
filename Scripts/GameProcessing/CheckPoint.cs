using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [SerializeField] private ÑheckPointProcessing _checkPointProcessing;
    [SerializeField] private ParticleSystem[] _particleSystems;
    [SerializeField] private float _timeBonus;

    public float TimeBonus => _timeBonus;

    private void AddTimeBonus()
    {
        _checkPointProcessing.PassedCheckPoint(this);
        foreach (ParticleSystem particle in _particleSystems)       
            particle.Stop();      
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")      
            AddTimeBonus();      
    }
}
