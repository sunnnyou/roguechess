using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SpriteHolder : MonoBehaviour
{
    [Header("UI References")]
    public Image mainImage;
    public Image loadingIcon;
    public Image playerMoveIcon;
    public Image idleIcon;

    [Header("Animation Settings")]
    public float fadeTime = 1f;
    public float growthDuration = 0.1f;
    public float growthScale = 1.15f;
    public float idleDisplayDelay = 5f;

    [Header("Materials")]
    public Material mainImageMaterial;
    public Material playerMoveIconMaterial;
    public Material idleIconMaterial;
    public Material loadingIconMaterial;

    private float lastMoveTime;
    private Coroutine idleCheckCoroutine;
    private Vector3 originalPlayerMoveScale;

    private void Awake()
    {
        if (loadingIcon != null)
        {
            loadingIcon.gameObject.SetActive(false);
        }

        if (playerMoveIcon != null)
        {
            playerMoveIcon.gameObject.SetActive(false);
            originalPlayerMoveScale = playerMoveIcon.transform.localScale;
            playerMoveIcon.rectTransform.pivot = new Vector2(0.5f, 0);
        }

        if (idleIcon != null)
        {
            idleIcon.gameObject.SetActive(false);
        }

        // Apply materials if assigned
        ApplyMaterial(mainImage, mainImageMaterial);
        ApplyMaterial(playerMoveIcon, playerMoveIconMaterial);
        ApplyMaterial(idleIcon, idleIconMaterial);
        ApplyMaterial(loadingIcon, loadingIconMaterial);

        lastMoveTime = Time.time;
        StartIdleCheck();
    }

    private void ApplyMaterial(Image image, Material material)
    {
        if (image != null && material != null)
        {
            image.material = Instantiate(material); // Create instance to avoid shared material modifications
        }
    }

    public void SetSprite(Sprite sprite)
    {
        if (mainImage != null)
        {
            mainImage.sprite = sprite;
        }
    }

    public void ShowLoading(bool show)
    {
        if (loadingIcon != null)
        {
            loadingIcon.gameObject.SetActive(show);
        }
    }

    public void OnPlayerMove()
    {
        lastMoveTime = Time.time;
        StartIdleCheck();
        ShowPlayerMoveIcon();
    }

    private void ShowPlayerMoveIcon()
    {
        if (playerMoveIcon == null)
            return;

        // Reset state
        playerMoveIcon.gameObject.SetActive(true);
        playerMoveIcon.transform.localScale = originalPlayerMoveScale;
        playerMoveIcon.color = new Color(1, 1, 1, 1);

        // Create animation sequence
        Sequence sequence = DOTween.Sequence();

        // Growth animation over 1 second
        sequence.Append(
            playerMoveIcon
                .transform.DOScale(originalPlayerMoveScale * growthScale, 1f)
                .SetEase(Ease.OutQuad)
        );

        // Fade out
        sequence.Append(
            playerMoveIcon
                .DOFade(0, fadeTime)
                .SetDelay(0.5f) // Add delay before fade starts
                .OnComplete(() => playerMoveIcon.gameObject.SetActive(false))
        );
    }

    private void StartIdleCheck()
    {
        if (idleCheckCoroutine != null)
        {
            StopCoroutine(idleCheckCoroutine);
        }
        idleCheckCoroutine = StartCoroutine(CheckIdleTime());
    }

    // Coroutine to check idle time so that idle icon is shown every x seconds when its the players turn and no moves are made
    private IEnumerator CheckIdleTime()
    {
        while (true)
        {
            float timeSinceLastMove = Time.time - lastMoveTime;
            if (timeSinceLastMove >= idleDisplayDelay)
            {
                ShowIdleIcon();
                // Wait for fade animation to complete before showing again
                yield return new WaitForSeconds(fadeTime + idleDisplayDelay);
            }
            else
            {
                yield return new WaitForSeconds(0.5f); // Check every half second
            }
        }
    }

    private void ShowIdleIcon()
    {
        if (idleIcon == null)
            return;

        idleIcon.gameObject.SetActive(true);
        idleIcon.color = new Color(1, 1, 1, 1);

        // Show for specified duration before starting fade
        Sequence sequence = DOTween.Sequence();

        sequence.AppendInterval(2f); // Show for 2 seconds
        sequence.Append(
            idleIcon
                .DOFade(0, fadeTime)
                .OnComplete(() =>
                {
                    idleIcon.gameObject.SetActive(false);
                    idleIcon.color = new Color(1, 1, 1, 1);
                })
        );
    }

    public void SetMainImageMaterial(Material material)
    {
        ApplyMaterial(mainImage, material);
    }

    public void SetPlayerMoveIconMaterial(Material material)
    {
        ApplyMaterial(playerMoveIcon, material);
    }

    public void SetIdleIconMaterial(Material material)
    {
        ApplyMaterial(idleIcon, material);
    }

    public void SetLoadingIconMaterial(Material material)
    {
        ApplyMaterial(loadingIcon, material);
    }

    private void OnDestroy()
    {
        // Clean up DOTween animations
        DOTween.Kill(playerMoveIcon.transform);
        DOTween.Kill(playerMoveIcon);
        DOTween.Kill(idleIcon);
    }
}
