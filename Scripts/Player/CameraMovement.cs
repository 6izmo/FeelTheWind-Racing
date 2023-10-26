using System.Collections;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Transform _cameraPoint;
    [SerializeField] private float _speed;
    [SerializeField] private Vector3 _offset;

    [SerializeField] private Vector3 _gameOverPosition;
    private Rigidbody _playerRigidbody;

    private void Awake() => _playerRigidbody = _playerTransform.gameObject.GetComponent<Rigidbody>();
  
    private void FixedUpdate()
    {
        Vector3 playerForward = (_playerTransform.forward + _playerRigidbody.velocity).normalized;
        transform.position = Vector3.Lerp(transform.position,
            _playerTransform.position + _playerTransform.TransformVector(_offset) + playerForward * (-5f),
            _speed * Time.deltaTime);
        transform.LookAt(_cameraPoint);
    }

    public void CameraRemove() => StartCoroutine(Remove());

    private IEnumerator Remove()
    {
        float movingTime = 5f;
        float yDirection= 100f;
        float percent = (1f / movingTime);

        while (_offset.y < yDirection)
        {
            _offset.y = Mathf.Lerp(_offset.y, yDirection, percent * Time.deltaTime);
            yield return null;
        }
    }

    public void GameOverPosition() => StartCoroutine(SetPosition());

    private IEnumerator SetPosition()
    {
        while (_offset != _gameOverPosition)
        {
            _offset = Vector3.Lerp(_offset, _gameOverPosition, Time.deltaTime);
            yield return null;
        }
    }
}
