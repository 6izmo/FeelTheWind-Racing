using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class CameraTextureUtils : MonoBehaviour    
{
    private const float _farPlaneTreshold = 1f;

    public static Texture2D CreateScreen(Color backgroundColor, float cameraHeight, Rect rect)
    {
        if (rect == default)
            return null;

        Camera camera = new GameObject("ScreenshotsCamera").AddComponent<Camera>();
        camera.orthographic = true;
        camera.clearFlags = CameraClearFlags.Skybox;
        camera.backgroundColor = backgroundColor;
        camera.useOcclusionCulling = false;
        camera.transform.position = new Vector3(rect.center.x, cameraHeight, rect.center.y);
        camera.farClipPlane = cameraHeight + _farPlaneTreshold;
        camera.transform.rotation = Quaternion.Euler(90f,0f,0f);
     
        camera.orthographicSize = Mathf.Max(rect.size.x, rect.size.y) / 2;

        Vector3 bottomLeft = rect.center.ToXZVector() - rect.size.ToXZVector() / 2;
        Vector3 topRight = rect.center.ToXZVector() + rect.size.ToXZVector() / 2;

        Vector3 screenBottomLeft = camera.WorldToScreenPoint(bottomLeft);
        Vector3 screenTopRight = camera.WorldToScreenPoint(topRight);

        float width = screenTopRight.x - screenBottomLeft.x;
        float height = screenTopRight.y - screenBottomLeft.y;

        return CreateTextureFromCamera(camera, new Rect(screenBottomLeft, new Vector2(width, height)));
    }

    private static Texture2D CreateTextureFromCamera(Camera camera, Rect pixelRect)
    {
        int width = Mathf.CeilToInt(pixelRect.width);
        int height = Mathf.CeilToInt(pixelRect.height);

        RenderTexture rt = new RenderTexture(
            width,
            height,
            GraphicsFormat.R8G8B8A8_SRGB,
            GraphicsFormat.None,
            1
            );
        camera.ResetAspect();
       
        Texture2D cameraTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        camera.targetTexture = rt;
        RenderTexture.active = rt;

        camera.Render();

        cameraTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0, false);
        cameraTexture.Apply();

        rt.Release();
        Object.DestroyImmediate(camera.gameObject);

        return cameraTexture;
    }

}
