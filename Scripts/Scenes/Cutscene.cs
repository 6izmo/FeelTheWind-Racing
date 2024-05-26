using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Playables;

public class Cutscene : MonoBehaviour
{
    [SerializeField] private Image _canvasMessage;
    private PlayableDirector _playableDirector;
    private Controls _controls;

    private void Awake()
    {
        _playableDirector = GetComponent<PlayableDirector>();
        _canvasMessage.Activate();

        _controls = new Controls();
        _controls.Main.SkipCutscene.performed += context => Skip();
    }

    private void Start() => StartCoroutine(HideMessage());

    private void Skip()
    {
        _playableDirector.time = _playableDirector.playableAsset.duration;
        _canvasMessage.Deactivate();
    }

    private IEnumerator HideMessage()
    {
        float elapcedTime = 0f;
        float showTime = 3f;
        while (elapcedTime < showTime)
        {
            elapcedTime += Time.deltaTime;
            yield return null;
        }
        _canvasMessage.Deactivate();
    }

    private void OnEnable() => _controls.Enable();

    private void OnDisable() => _controls.Disable();
}
