namespace Assets.Scripts.Game.AI
{
    using System.Collections;
    using DG.Tweening;
    using UnityEngine;
    using UnityEngine.UI;

    public class SpriteHolder : MonoBehaviour
    {
        [Header("UI References")]
        public Image MainImage;
        public Image LoadingIcon;
        public Image PlayerMoveIcon;
        public Image IdleIcon;

        [Header("Animation Settings")]
        public float FadeTime = 1f;
        public float GrowthDuration = 0.1f;
        public float GrowthScale = 1.15f;
        public float IdleDisplayDelay = 5f;

        [Header("Materials")]
        public Material MainImageMaterial;
        public Material PlayerMoveIconMaterial;
        public Material IdleIconMaterial;
        public Material LoadingIconMaterial;

        private float lastMoveTime;
        private Coroutine idleCheckCoroutine;
        private Vector3 originalPlayerMoveScale;

        private void Awake()
        {
            if (this.LoadingIcon != null)
            {
                this.LoadingIcon.gameObject.SetActive(false);
            }

            if (this.PlayerMoveIcon != null)
            {
                this.PlayerMoveIcon.gameObject.SetActive(false);
                this.originalPlayerMoveScale = this.PlayerMoveIcon.transform.localScale;
                this.PlayerMoveIcon.rectTransform.pivot = new Vector2(0.5f, 0);
            }

            if (this.IdleIcon != null)
            {
                this.IdleIcon.gameObject.SetActive(false);
            }

            // Apply materials if assigned
            ApplyMaterial(this.MainImage, this.MainImageMaterial);
            ApplyMaterial(this.PlayerMoveIcon, this.PlayerMoveIconMaterial);
            ApplyMaterial(this.IdleIcon, this.IdleIconMaterial);
            ApplyMaterial(this.LoadingIcon, this.LoadingIconMaterial);

            this.lastMoveTime = Time.time;
            this.StartIdleCheck();
        }

        private static void ApplyMaterial(Image image, Material material)
        {
            if (image != null && material != null)
            {
                image.material = Instantiate(material); // Create instance to avoid shared material modifications
            }
        }

        public void SetSprite(Sprite sprite)
        {
            if (this.MainImage != null)
            {
                this.MainImage.sprite = sprite;
            }
        }

        public void ShowLoading(bool show)
        {
            if (this.LoadingIcon != null)
            {
                this.LoadingIcon.gameObject.SetActive(show);
            }
        }

        public void OnPlayerMove()
        {
            this.lastMoveTime = Time.time;
            this.StartIdleCheck();
            this.ShowPlayerMoveIcon();
        }

        private void ShowPlayerMoveIcon()
        {
            if (this.PlayerMoveIcon == null)
            {
                return;
            }

            // Reset state
            this.PlayerMoveIcon.gameObject.SetActive(true);
            this.PlayerMoveIcon.transform.localScale = this.originalPlayerMoveScale;
            this.PlayerMoveIcon.color = new Color(1, 1, 1, 1);

            // Create animation sequence
            Sequence sequence = DOTween.Sequence();

            // Growth animation over 1 second
            sequence.Append(
                this.PlayerMoveIcon.transform.DOScale(
                        this.originalPlayerMoveScale * this.GrowthScale,
                        1f
                    )
                    .SetEase(Ease.OutQuad)
            );

            // Fade out
            sequence.Append(
                this.PlayerMoveIcon.DOFade(0, this.FadeTime)
                    .SetDelay(0.5f) // Add delay before fade starts
                    .OnComplete(() => this.PlayerMoveIcon.gameObject.SetActive(false))
            );
        }

        private void StartIdleCheck()
        {
            if (this.idleCheckCoroutine != null)
            {
                this.StopCoroutine(this.idleCheckCoroutine);
            }

            this.idleCheckCoroutine = this.StartCoroutine(this.CheckIdleTime());
        }

        // Coroutine to check idle time so that idle icon is shown every x seconds when its the players turn and no moves are made
        private IEnumerator CheckIdleTime()
        {
            while (true)
            {
                float timeSinceLastMove = Time.time - this.lastMoveTime;
                if (timeSinceLastMove >= this.IdleDisplayDelay)
                {
                    this.ShowIdleIcon();

                    // Wait for fade animation to complete before showing again
                    yield return new WaitForSeconds(this.FadeTime + this.IdleDisplayDelay);
                }
                else
                {
                    yield return new WaitForSeconds(0.5f); // Check every half second
                }
            }
        }

        private void ShowIdleIcon()
        {
            if (this.IdleIcon == null)
            {
                return;
            }

            this.IdleIcon.gameObject.SetActive(true);
            this.IdleIcon.color = new Color(1, 1, 1, 1);

            // Show for specified duration before starting fade
            Sequence sequence = DOTween.Sequence();

            sequence.AppendInterval(2f); // Show for 2 seconds
            sequence.Append(
                this.IdleIcon.DOFade(0, this.FadeTime)
                    .OnComplete(() =>
                    {
                        this.IdleIcon.gameObject.SetActive(false);
                        this.IdleIcon.color = new Color(1, 1, 1, 1);
                    })
            );
        }

        public void SetMainImageMaterial(Material material)
        {
            ApplyMaterial(this.MainImage, material);
        }

        public void SetPlayerMoveIconMaterial(Material material)
        {
            ApplyMaterial(this.PlayerMoveIcon, material);
        }

        public void SetIdleIconMaterial(Material material)
        {
            ApplyMaterial(this.IdleIcon, material);
        }

        public void SetLoadingIconMaterial(Material material)
        {
            ApplyMaterial(this.LoadingIcon, material);
        }

        private void OnDestroy()
        {
            // Clean up DOTween animations
            if (this.PlayerMoveIcon != null)
            {
                DOTween.Kill(this.PlayerMoveIcon.transform);
                DOTween.Kill(this.PlayerMoveIcon);
            }
            if (this.IdleIcon != null)
            {
                DOTween.Kill(this.IdleIcon);
            }
        }
    }
}
