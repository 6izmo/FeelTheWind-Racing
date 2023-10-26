using TMPro;
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ÑheckPointProcessing : MonoBehaviour
{
    [Header("AudioEffects")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _checkPointAudio;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI _checkPointsCount;
    [SerializeField] private TextMeshProUGUI _addTimeText;

    [SerializeField] private List<CheckPoint> _checkPoints;
    private CheckPoint _currentPoint;
    private int _passedCheckPoints = 0;
    private float _addTimeShow = 1.2f;

    private Timer _timer;

    public Action LevelComplete;

    private void Awake()
    {
        _timer = GetComponent<Timer>();

        _currentPoint = _checkPoints[0];
        _checkPointsCount.text = $"{_passedCheckPoints}" + "/" + _checkPoints.Count;
    }

    public void PassedCheckPoint(CheckPoint checkPoint)
    {
        if(_currentPoint == checkPoint && !MainGameEvents.Instance.IsGameOver)
        {
            int index = _checkPoints.IndexOf(checkPoint);

            if (index < _checkPoints.Count - 1)
            {
                _timer.AddTime(checkPoint.TimeBonus);
                StartCoroutine(AddTimeShow(checkPoint.TimeBonus));

                _currentPoint = _checkPoints[index + 1];

                _passedCheckPoints++;
                _checkPointsCount.text = $"{_passedCheckPoints}" + "/" + _checkPoints.Count;
                _audioSource.PlayOneShot(_checkPointAudio);
            }
            else
                LevelComplete?.Invoke();
        }
    }

    private IEnumerator AddTimeShow(float time)
    {
        _addTimeText.Activate();
        _addTimeText.text = "+" + $"{time:F0}" + "s";
        yield return new WaitForSeconds(_addTimeShow);
        _addTimeText.Deactivate();
    }
}