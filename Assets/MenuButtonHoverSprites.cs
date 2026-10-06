using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>Shows the alternate menu sprite while the pointer is over a button.</summary>
public sealed class MenuButtonHoverSprites : MonoBehaviour
{
    [System.Serializable]
    private struct ButtonSprites
    {
        public string name;
        public Sprite normal;
        public Sprite hover;
    }

    [SerializeField] private ButtonSprites[] sprites;

    private Button[] buttons;
    private Image[] images;
    private readonly List<RaycastResult> hits = new List<RaycastResult>();
    private int hoveredIndex = -1;

    private void Awake()
    {
        buttons = new Button[sprites.Length];
        images = new Image[sprites.Length];

        for (int i = 0; i < sprites.Length; i++)
        {
            Transform child = transform.Find(sprites[i].name);
            if (child == null) continue;

            buttons[i] = child.GetComponent<Button>();
            images[i] = child.GetComponent<Image>();
            if (buttons[i] != null) buttons[i].transition = Selectable.Transition.None;
            if (images[i] != null) images[i].sprite = sprites[i].normal;
        }
    }

    private void Update()
    {
        int pointerIndex = ButtonUnderPointer();
        if (pointerIndex == hoveredIndex) return;

        if (hoveredIndex >= 0)
        {
            images[hoveredIndex].sprite = sprites[hoveredIndex].normal;
        }

        hoveredIndex = pointerIndex;
        if (hoveredIndex >= 0)
            images[hoveredIndex].sprite = sprites[hoveredIndex].hover;
    }

    private int ButtonUnderPointer()
    {
        if (EventSystem.current == null) return -1;

        hits.Clear();
        var pointer = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
        EventSystem.current.RaycastAll(pointer, hits);
        foreach (RaycastResult hit in hits)
        {
            Button button = hit.gameObject.GetComponentInParent<Button>();
            if (button == null) return -1;
            for (int i = 0; i < buttons.Length; i++)
                if (button == buttons[i] && button.IsActive() && button.interactable && images[i] != null)
                    return i;
            return -1;
        }
        return -1;
    }

    private void OnDisable()
    {
        if (hoveredIndex >= 0 && images[hoveredIndex] != null)
            images[hoveredIndex].sprite = sprites[hoveredIndex].normal;
        hoveredIndex = -1;
    }
}
