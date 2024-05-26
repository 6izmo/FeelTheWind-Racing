using UnityEngine;

public class CursorRenderer : MonoBehaviour
{
    private void Start() => Visable(false);
  
    public static void Visable(bool show) => Cursor.visible = show;

    private void OnApplicationFocus(bool focus) => Cursor.visible = !focus;
}
