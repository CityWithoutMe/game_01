using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

[RequireComponent(typeof(Camera))]
public sealed class CgVideoPlayback : MonoBehaviour
{
    [SerializeField] private VideoClip clip;

    private VideoPlayer player;
    private RenderTexture videoTexture;

    private void Awake()
    {
        Camera videoCamera = GetComponent<Camera>();
        videoCamera.clearFlags = CameraClearFlags.SolidColor;
        videoCamera.backgroundColor = Color.black;
        videoCamera.cullingMask = 0;

        if (clip == null) return;

        int width = Mathf.Max(1, (int)clip.width);
        int height = Mathf.Max(1, (int)clip.height);
        videoTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
        videoTexture.Create();

        GameObject canvasObject = new GameObject("Video Canvas", typeof(RectTransform), typeof(Canvas));
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = short.MaxValue;

        GameObject imageObject = new GameObject("Full Screen Video", typeof(RectTransform), typeof(CanvasRenderer), typeof(RawImage), typeof(AspectRatioFitter));
        imageObject.transform.SetParent(canvasObject.transform, false);
        RectTransform imageRect = imageObject.GetComponent<RectTransform>();
        imageRect.anchorMin = Vector2.zero;
        imageRect.anchorMax = Vector2.one;
        imageRect.offsetMin = Vector2.zero;
        imageRect.offsetMax = Vector2.zero;
        RawImage image = imageObject.GetComponent<RawImage>();
        image.texture = videoTexture;
        image.raycastTarget = false;
        if (DisplayColorSettings.Instance != null)
            image.material = DisplayColorSettings.Instance.UiMaterial;
        AspectRatioFitter fitter = imageObject.GetComponent<AspectRatioFitter>();
        fitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
        fitter.aspectRatio = (float)width / height;

        player = gameObject.AddComponent<VideoPlayer>();
        player.playOnAwake = false;
        player.isLooping = false;
        player.waitForFirstFrame = true;
        player.source = VideoSource.VideoClip;
        player.clip = clip;
        player.renderMode = VideoRenderMode.RenderTexture;
        player.targetTexture = videoTexture;
        player.audioOutputMode = VideoAudioOutputMode.Direct;
        player.prepareCompleted += OnVideoPrepared;
        player.loopPointReached += OnVideoFinished;
        player.errorReceived += OnVideoError;
    }

    private void Start()
    {
        if (clip == null)
        {
            Debug.LogError("CG video clip is not assigned.", this);
            return;
        }

        player.Prepare();
    }

    private void OnVideoPrepared(VideoPlayer preparedPlayer)
    {
        preparedPlayer.Play();
    }

    private void OnVideoFinished(VideoPlayer finishedPlayer)
    {
        SceneManager.LoadScene("main");
    }

    private void OnVideoError(VideoPlayer failedPlayer, string message)
    {
        Debug.LogError("CG video playback failed: " + message, this);
    }

    private void OnDestroy()
    {
        if (player != null)
        {
            player.prepareCompleted -= OnVideoPrepared;
            player.loopPointReached -= OnVideoFinished;
            player.errorReceived -= OnVideoError;
        }
        if (videoTexture != null)
        {
            videoTexture.Release();
            Destroy(videoTexture);
        }
    }
}
