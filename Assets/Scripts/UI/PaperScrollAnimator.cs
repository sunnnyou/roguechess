using DG.Tweening;
using TMPro;
using UnityEngine;

public class PaperScrollAnimator : MonoBehaviour
{
    public RectTransform ScrollPanel;
    public AudioSource ScrollSoundOpen;
    public AudioSource ScrollSoundClose;
    public TMP_Text Title;
    public TMP_Text Description;
    public TMP_Text Cost;

    private bool isOpen;

    public void Start()
    {
        // hide initially
        this.ScrollPanel.localScale = new Vector3(1, 0, 1);
    }

    public void OpenScroll()
    {
        if (this.isOpen)
        {
            return;
        }

        this.isOpen = true;

        // Reset rotation first (important for consistency)
        this.ScrollPanel.rotation = Quaternion.identity;

        // Open scroll
        this.ScrollPanel.DOScaleY(1f, 0.2f).SetEase(Ease.OutBack);

        if (this.ScrollSoundOpen != null)
        {
            this.ScrollSoundOpen.Play();
        }
    }

    public void CloseScroll()
    {
        if (!this.isOpen)
        {
            return;
        }

        this.isOpen = false;

        // Reset rotation first (important for consistency)
        this.ScrollPanel.rotation = Quaternion.identity;

        // Shrink Y scale to simulate closing
        this.ScrollPanel.DOScaleY(0f, 0.15f).SetEase(Ease.InBack);

        // Play closing sound
        if (this.ScrollSoundClose != null)
        {
            this.ScrollSoundClose.Play();
        }
    }

    public void Reroll()
    {
        this.CloseScroll();

        this.OpenScroll();
    }

    public void Wiggle()
    {
        this.ScrollPanel.DORotate(new Vector3(0, 0, -1f), 0.1f)
            .SetLoops(2, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }
}
