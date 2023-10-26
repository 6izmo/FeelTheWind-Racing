using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _loadingText;
    [SerializeField] private Image _carIamge;
    private Animator _animator;
    private AsyncOperation _asyncOperation;

    private bool _shouldPlayOpenning = false;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        
        if(_shouldPlayOpenning)
            _animator.SetTrigger("Open");
    } 

    public void SceneSwitch(int sceneIndex)
    {
        if (_asyncOperation != null)
            return;

        _animator.SetTrigger("Close");
        _asyncOperation = SceneManager.LoadSceneAsync(sceneIndex);
        _asyncOperation.allowSceneActivation = false;
    }

    public void StartLoadAnimations() => StartCoroutine(LoadAnimations());

    private IEnumerator LoadAnimations()
    {
        _loadingText.Activate();
        _carIamge.Activate();
        while (!_asyncOperation.allowSceneActivation)
        {
            _loadingText.text = "Loading";
            yield return new WaitForSeconds(1);
            _loadingText.text = "Loading" + ".";
            yield return new WaitForSeconds(1);
            _loadingText.text = "Loading" + "..";
            yield return new WaitForSeconds(1);
            _loadingText.text = "Loading" + "...";
            yield return new WaitForSeconds(1);
        }
        _carIamge.Deactivate();
        _loadingText.Deactivate();
    }

   
    public void OnAnimationOver()
    {   
        _asyncOperation.allowSceneActivation = true;
        _shouldPlayOpenning = true;
    }
}

