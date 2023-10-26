using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class MapAgentGraphics : MonoBehaviour
{
    [SerializeField] private Image _colorIcon;
    public RectTransform RectTransform { get; private set; }

    private void Awake() => RectTransform = (RectTransform)transform;

    public void Initialize(Color color, string agentName = null)
    {
        if (!string.IsNullOrEmpty(agentName))
            gameObject.name = agentName;

        _colorIcon.color = color;
    }
}
