using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Builds the hand-drawn display page behind the existing Settings button.</summary>
public sealed class OfflineSettingsMenu : MonoBehaviour
{
    [SerializeField] private Button settingsButton;
    [SerializeField] private Image menuBackground;
    [SerializeField] private Sprite paperSprite;
    [SerializeField] private Sprite trackSprite;
    [SerializeField] private Sprite thumbSprite;
    [SerializeField] private Sprite buttonSprite;
    [SerializeField] private Sprite aboutNamesSprite;

    private GameObject page;
    private GameObject aboutPage;
    private Button aboutButton;
    private GameObject[] mainButtons;
    private Slider brightnessSlider;
    private Slider contrastSlider;
    private Slider gammaSlider;
    private TMP_Text brightnessValue;
    private TMP_Text contrastValue;
    private TMP_Text gammaValue;
    private Texture2D previewTexture;
    private TMP_FontAsset font;
    private DisplayColorSettings colorSettings;

    private static readonly Color Ink = new Color(0.10f, 0.10f, 0.11f);
    private static readonly Color Accent = new Color(0.02f, 0.42f, 0.92f);

    private void Awake()
    {
        if (settingsButton == null)
        {
            Debug.LogError("The offline Settings button is not assigned.", this);
            enabled = false;
            return;
        }

        colorSettings = DisplayColorSettings.Instance;
        if (colorSettings == null)
        {
            Debug.LogError("DisplayColorSettings did not initialize.", this);
            enabled = false;
            return;
        }

        font = settingsButton.GetComponentInChildren<TextMeshProUGUI>(true)?.font;
        if (font == null) font = TMP_Settings.defaultFontAsset;

        Transform root = settingsButton.transform.parent;
        mainButtons = new GameObject[4];
        for (int i = 0; i < 4; i++)
        {
            Transform child = root.Find("btn" + (i + 1));
            if (child != null) mainButtons[i] = child.gameObject;
        }
        if (mainButtons[1] != null) aboutButton = mainButtons[1].GetComponent<Button>();

        if (menuBackground == null) menuBackground = root.GetComponent<Image>();
        if (menuBackground != null && colorSettings.UiMaterial != null)
            menuBackground.material = colorSettings.UiMaterial;

        BuildPage();
        BuildAboutPage();
        settingsButton.onClick.AddListener(Open);
        if (aboutButton != null) aboutButton.onClick.AddListener(OpenAbout);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (page != null && page.activeSelf) Close();
            else if (aboutPage != null && aboutPage.activeSelf) CloseAbout();
        }
    }

    private void OnDestroy()
    {
        if (settingsButton != null) settingsButton.onClick.RemoveListener(Open);
        if (aboutButton != null) aboutButton.onClick.RemoveListener(OpenAbout);
        if (previewTexture != null) Destroy(previewTexture);
    }

    private void Open()
    {
        foreach (GameObject button in mainButtons)
            if (button != null) button.SetActive(false);
        SyncSliders();
        page.SetActive(true);
    }

    private void Close()
    {
        page.SetActive(false);
        foreach (GameObject button in mainButtons)
            if (button != null) button.SetActive(true);
        colorSettings.Save();
    }

    private void OpenAbout()
    {
        foreach (GameObject button in mainButtons)
            if (button != null) button.SetActive(false);
        aboutPage.SetActive(true);
    }

    private void CloseAbout()
    {
        aboutPage.SetActive(false);
        foreach (GameObject button in mainButtons)
            if (button != null) button.SetActive(true);
    }

    private void Reset()
    {
        colorSettings.ResetValues();
        SyncSliders();
        colorSettings.Save();
    }

    private void ValuesChanged()
    {
        colorSettings.SetValues(brightnessSlider.value, contrastSlider.value, gammaSlider.value);
        RefreshNumbers();
    }

    private void SyncSliders()
    {
        brightnessSlider.SetValueWithoutNotify(colorSettings.Brightness);
        contrastSlider.SetValueWithoutNotify(colorSettings.Contrast);
        gammaSlider.SetValueWithoutNotify(colorSettings.Gamma);
        RefreshNumbers();
    }

    private void RefreshNumbers()
    {
        brightnessValue.text = colorSettings.Brightness.ToString("+0.0;-0.0;0.0") + " EV";
        contrastValue.text = colorSettings.Contrast.ToString("+0;-0;0");
        gammaValue.text = colorSettings.Gamma.ToString("0.00");
    }

    private void BuildPage()
    {
        RectTransform canvas = GetComponent<RectTransform>();
        RectTransform overlay = Rect(canvas, "Settings Page", Vector2.zero, Vector2.one);
        page = overlay.gameObject;
        Image veil = overlay.gameObject.AddComponent<Image>();
        veil.color = new Color(0f, 0f, 0f, 0.34f);
        veil.raycastTarget = true;

        RectTransform paper = Rect(overlay, "Hand-drawn Paper", new Vector2(0.40f, 0.08f), new Vector2(0.95f, 0.92f));
        Image paperImage = paper.gameObject.AddComponent<Image>();
        paperImage.sprite = paperSprite;
        paperImage.color = Color.white;
        paperImage.raycastTarget = true;

        Label(paper, "DISPLAY / SETTINGS", 38, TextAlignmentOptions.Left,
            new Vector2(0.09f, 0.83f), new Vector2(0.91f, 0.95f), Ink, true);
        Label(paper, "Adjust the monochrome image", 20, TextAlignmentOptions.Left,
            new Vector2(0.09f, 0.77f), new Vector2(0.91f, 0.83f), new Color(0.35f, 0.35f, 0.35f));

        brightnessSlider = SliderRow(paper, "BRIGHTNESS", 0.68f, -2f, 2f, out brightnessValue);
        contrastSlider = SliderRow(paper, "CONTRAST", 0.52f, -50f, 50f, out contrastValue);
        gammaSlider = SliderRow(paper, "GAMMA", 0.36f, 0.5f, 2f, out gammaValue);
        brightnessSlider.onValueChanged.AddListener(_ => ValuesChanged());
        contrastSlider.onValueChanged.AddListener(_ => ValuesChanged());
        gammaSlider.onValueChanged.AddListener(_ => ValuesChanged());

        Label(paper, "BLACK                                      MID                                      WHITE", 15,
            TextAlignmentOptions.Center, new Vector2(0.10f, 0.225f), new Vector2(0.90f, 0.265f), Ink);
        RectTransform preview = Rect(paper, "Tone Preview", new Vector2(0.10f, 0.17f), new Vector2(0.90f, 0.225f));
        RawImage previewImage = preview.gameObject.AddComponent<RawImage>();
        previewImage.raycastTarget = false;
        previewTexture = new Texture2D(256, 1, TextureFormat.RGBA32, false);
        for (int x = 0; x < 256; x++)
            previewTexture.SetPixel(x, 0, new Color(x / 255f, x / 255f, x / 255f, 1f));
        previewTexture.Apply(false, true);
        previewImage.texture = previewTexture;
        previewImage.material = colorSettings.UiMaterial;

        ActionButton(paper, "BACK", new Vector2(0.10f, 0.045f), new Vector2(0.35f, 0.135f), Close);
        ActionButton(paper, "RESET", new Vector2(0.65f, 0.045f), new Vector2(0.90f, 0.135f), Reset);
        SyncSliders();
        page.SetActive(false);
    }

    private void BuildAboutPage()
    {
        RectTransform canvas = GetComponent<RectTransform>();
        RectTransform overlay = Rect(canvas, "About Us Page", Vector2.zero, Vector2.one);
        aboutPage = overlay.gameObject;
        Image veil = overlay.gameObject.AddComponent<Image>();
        veil.color = new Color(0f, 0f, 0f, 0.34f);
        veil.raycastTarget = true;

        RectTransform paper = Rect(overlay, "Hand-drawn Paper", new Vector2(0.40f, 0.08f), new Vector2(0.95f, 0.92f));
        Image paperImage = paper.gameObject.AddComponent<Image>();
        paperImage.sprite = paperSprite;
        paperImage.color = Color.white;
        paperImage.raycastTarget = true;

        Label(paper, "ABOUT US", 38, TextAlignmentOptions.Left,
            new Vector2(0.09f, 0.83f), new Vector2(0.91f, 0.95f), Ink, true);
        Label(paper, "THE PEOPLE BEHIND THE GAME", 19, TextAlignmentOptions.Left,
            new Vector2(0.09f, 0.77f), new Vector2(0.91f, 0.83f), new Color(0.35f, 0.35f, 0.35f));

        RectTransform names = Rect(paper, "Team Names", new Vector2(0.08f, 0.20f), new Vector2(0.92f, 0.75f));
        Image namesImage = names.gameObject.AddComponent<Image>();
        namesImage.sprite = aboutNamesSprite;
        namesImage.preserveAspect = true;
        namesImage.raycastTarget = false;

        ActionButton(paper, "BACK", new Vector2(0.10f, 0.045f), new Vector2(0.35f, 0.135f), CloseAbout);
        aboutPage.SetActive(false);
    }

    private Slider SliderRow(RectTransform parent, string title, float center, float min, float max, out TMP_Text value)
    {
        RectTransform row = Rect(parent, title + " Row", new Vector2(0.09f, center - 0.07f), new Vector2(0.91f, center + 0.065f));
        Label(row, title, 23, TextAlignmentOptions.Left, new Vector2(0f, 0.50f), new Vector2(0.73f, 1f), Ink, true);
        value = Label(row, "", 21, TextAlignmentOptions.Right, new Vector2(0.72f, 0.50f), new Vector2(1f, 1f), Ink);

        RectTransform track = Rect(row, title + " Slider", new Vector2(0f, 0.08f), new Vector2(1f, 0.46f));
        Image trackImage = track.gameObject.AddComponent<Image>();
        trackImage.sprite = trackSprite;
        trackImage.color = Color.white;
        trackImage.raycastTarget = true;

        RectTransform travel = Rect(track, "Handle Slide Area", Vector2.zero, Vector2.one);
        travel.offsetMin = new Vector2(14f, 0f);
        travel.offsetMax = new Vector2(-14f, 0f);
        RectTransform handle = Rect(travel, "Ink Handle", new Vector2(0f, 0.5f), new Vector2(0f, 0.5f));
        handle.sizeDelta = new Vector2(30f, 30f);
        Image handleImage = handle.gameObject.AddComponent<Image>();
        handleImage.sprite = thumbSprite;
        handleImage.color = Accent;

        Slider slider = track.gameObject.AddComponent<Slider>();
        slider.transition = Selectable.Transition.None;
        slider.targetGraphic = handleImage;
        slider.handleRect = handle;
        slider.direction = Slider.Direction.LeftToRight;
        slider.minValue = min;
        slider.maxValue = max;
        slider.wholeNumbers = false;
        return slider;
    }

    private void ActionButton(RectTransform parent, string title, Vector2 min, Vector2 max, UnityEngine.Events.UnityAction action)
    {
        RectTransform rect = Rect(parent, title, min, max);
        Image image = rect.gameObject.AddComponent<Image>();
        image.sprite = buttonSprite;
        image.color = Color.white;
        Button button = rect.gameObject.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(action);
        Label(rect, title, 21, TextAlignmentOptions.Center, Vector2.zero, Vector2.one, Ink, true);
    }

    private TMP_Text Label(RectTransform parent, string content, float size, TextAlignmentOptions alignment,
        Vector2 min, Vector2 max, Color color, bool bold = false)
    {
        RectTransform rect = Rect(parent, content + " Label", min, max);
        TextMeshProUGUI label = rect.gameObject.AddComponent<TextMeshProUGUI>();
        label.text = content;
        label.font = font;
        label.fontSize = size;
        label.enableAutoSizing = true;
        label.fontSizeMin = Mathf.Max(12f, size * 0.6f);
        label.fontSizeMax = size;
        label.alignment = alignment;
        label.color = color;
        label.fontStyle = bold ? FontStyles.Bold : FontStyles.Normal;
        label.raycastTarget = false;
        return label;
    }

    private static RectTransform Rect(Transform parent, string name, Vector2 min, Vector2 max)
    {
        GameObject child = new GameObject(name, typeof(RectTransform));
        RectTransform rect = child.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.anchorMin = min;
        rect.anchorMax = max;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        return rect;
    }
}
