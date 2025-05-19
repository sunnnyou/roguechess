using DG.Tweening;
using UnityEngine;

public class ScrollAnimator : MonoBehaviour
{
    public RectTransform scrollPanel;
    public AudioSource scrollSoundOpen;
    public AudioSource scrollSoundClose;

    private bool isOpen = false;

    void Start()
    {
        // Optional: hide initially
        scrollPanel.localScale = new Vector3(1, 0, 1);
    }

    public void OpenScroll()
    {
        if (isOpen)
        {
            return;
        }
        isOpen = true;

        // Reset rotation first (important for consistency)
        scrollPanel.rotation = Quaternion.identity;

        // Open scoll
        scrollPanel.DOScaleY(1f, 0.2f).SetEase(Ease.OutBack);

        if (scrollSoundOpen != null)
            scrollSoundOpen.Play();
    }

    public void CloseScroll()
    {
        if (!isOpen)
        {
            return;
        }
        isOpen = false;

        // Reset rotation first (important for consistency)
        scrollPanel.rotation = Quaternion.identity;

        // Shrink Y scale to simulate closing
        scrollPanel.DOScaleY(0f, 0.15f).SetEase(Ease.InBack);

        // Play closing sound
        if (scrollSoundClose != null)
            scrollSoundClose.Play();
    }

    public void wiggle()
    {
        scrollPanel
            .DORotate(new Vector3(0, 0, -1f), 0.1f)
            .SetLoops(2, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }
}
