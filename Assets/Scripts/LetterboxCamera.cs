using UnityEngine;

public class LetterboxCamera : MonoBehaviour
{
    public float minTargetAspect = 4.0f / 3.0f;
    public float maxTargetAspect = 1.8f;

    private int lastScreenWidth;
    private int lastScreenHeight;

    private Camera backgroundCamera;

    void Start()
    {
        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;

        // Assicurati che la camera principale usi uno sfondo a colore solido
        Camera camera = GetComponent<Camera>();
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.678f, 0.847f, 0.902f, 1.0f); // Colore ADD8E6

        // Creiamo una camera di background
        CreateBackgroundCamera();

        UpdateCameraViewport();
    }

    void Update()
    {
        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
        {
            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;
            UpdateCameraViewport();
        }
    }

    void UpdateCameraViewport()
    {
        float windowAspect = (float)Screen.width / Screen.height;
        Camera camera = GetComponent<Camera>();

        if (windowAspect < minTargetAspect)
        {
            float scaleHeight = windowAspect / minTargetAspect;
            camera.rect = new Rect(0, (1.0f - scaleHeight) / 2.0f, 1.0f, scaleHeight);
        }
        else if (windowAspect > maxTargetAspect)
        {
            float scaleWidth = maxTargetAspect / windowAspect;
            camera.rect = new Rect((1.0f - scaleWidth) / 2.0f, 0, scaleWidth, 1.0f);
        }
        else
        {
            camera.rect = new Rect(0, 0, 1, 1);
        }
    }

    void CreateBackgroundCamera()
    {
        GameObject bgCameraObj = new GameObject("BackgroundCamera");
        backgroundCamera = bgCameraObj.AddComponent<Camera>();

        // Configura la camera di sfondo
        backgroundCamera.depth = -10; // Assicura che stia dietro alla main camera
        backgroundCamera.clearFlags = CameraClearFlags.SolidColor;
        backgroundCamera.backgroundColor = new Color(0.678f, 0.847f, 0.902f, 1.0f); // Colore ADD8E6
        backgroundCamera.cullingMask = 0; // Non deve renderizzare nulla
    }
}
