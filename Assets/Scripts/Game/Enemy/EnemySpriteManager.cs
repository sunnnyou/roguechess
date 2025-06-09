namespace Assets.Scripts.Game.Enemy
{
    using System.Collections;
    using DG.Tweening;
    using UnityEngine;

    public class EnemySpriteManager : MonoBehaviour
    {
        [Header("Sprite References")]
        public SpriteRenderer MainSprite;
        public SpriteRenderer LoadingSprite;
        public SpriteRenderer PlayerMoveSprite;
        public SpriteRenderer IdleSprite;

        [Header("Animation Settings")]
        public float FadeTime = 1f;
        public float GrowthDuration = 0.1f;
        public float GrowthScale = 1.075f;
        public float IdleDisplayDelay = 5f;

        [Header("Materials")]
        public Material MainSpriteMaterial;
        public Material PlayerMoveSpriteMaterial;
        public Material IdleSpriteMaterial;
        public Material LoadingSpriteMaterial;

        private float lastMoveTime;
        private Coroutine idleCheckCoroutine;
        private Vector3 originalPlayerMoveScale;

        private void Awake()
        {
            if (this.LoadingSprite != null)
            {
                this.LoadingSprite.gameObject.SetActive(false);
            }

            if (this.PlayerMoveSprite != null)
            {
                this.PlayerMoveSprite.gameObject.SetActive(false);
                this.originalPlayerMoveScale = this.PlayerMoveSprite.transform.localScale;
            }

            if (this.IdleSprite != null)
            {
                this.IdleSprite.gameObject.SetActive(false);
            }

            // Apply materials if assigned
            ApplyMaterial(this.MainSprite, this.MainSpriteMaterial);
            ApplyMaterial(this.PlayerMoveSprite, this.PlayerMoveSpriteMaterial);
            ApplyMaterial(this.IdleSprite, this.IdleSpriteMaterial);
            ApplyMaterial(this.LoadingSprite, this.LoadingSpriteMaterial);

            this.lastMoveTime = Time.time;
            this.StartIdleCheck();
        }

        private static void ApplyMaterial(SpriteRenderer sprite, Material material)
        {
            if (sprite != null && material != null)
            {
                sprite.material = Instantiate(material); // Create instance to avoid shared material modifications
            }
        }

        public void SetSprite(Sprite sprite)
        {
            if (this.MainSprite != null)
            {
                this.MainSprite.sprite = sprite;
            }
        }

        public void ShowLoading(bool show)
        {
            if (this.LoadingSprite != null)
            {
                if (show)
                {
                    this.PlayerMoveSprite.gameObject.SetActive(false);
                    this.IdleSprite.gameObject.SetActive(false);
                    this.OnDestroy();
                }

                this.LoadingSprite.gameObject.SetActive(show);
            }
        }

        public void OnPlayerMove()
        {
            this.lastMoveTime = Time.time;
            this.StartIdleCheck();
            this.ShowPlayerMoveSprite();
        }

        private void ShowPlayerMoveSprite()
        {
            if (this.PlayerMoveSprite == null)
            {
                return;
            }

            this.LoadingSprite.gameObject.SetActive(false);
            this.IdleSprite.gameObject.SetActive(false);
            this.OnDestroy();

            // Reset state
            this.PlayerMoveSprite.gameObject.SetActive(true);
            this.PlayerMoveSprite.transform.localScale = this.originalPlayerMoveScale;
            this.PlayerMoveSprite.color = new Color(1, 1, 1, 1);

            // Create animation sequence
            Sequence sequence = DOTween.Sequence(this.PlayerMoveSprite);

            // Growth animation
            sequence.Append(
                this.PlayerMoveSprite.transform.DOScale(
                        this.originalPlayerMoveScale * this.GrowthScale,
                        0.1f
                    )
                    .SetEase(Ease.OutQuad)
            );

            // Fade out
            sequence.Append(
                this.PlayerMoveSprite.DOFade(0, this.FadeTime)
                    .OnComplete(() => this.PlayerMoveSprite.gameObject.SetActive(false))
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
                    this.ShowIdleSprite();

                    // Wait for fade animation to complete before showing again
                    yield return new WaitForSeconds(this.FadeTime + this.IdleDisplayDelay);
                }
                else
                {
                    yield return new WaitForSeconds(0.5f);
                }
            }
        }

        private void ShowIdleSprite()
        {
            if (this.IdleSprite == null)
            {
                return;
            }

            this.LoadingSprite.gameObject.SetActive(false);
            this.PlayerMoveSprite.gameObject.SetActive(false);
            this.OnDestroy();

            this.IdleSprite.gameObject.SetActive(true);
            this.IdleSprite.color = new Color(1, 1, 1, 1);

            // Show for specified duration before starting fade
            Sequence sequence = DOTween.Sequence(this.IdleSprite);

            sequence.AppendInterval(2f); // Show for 2 seconds
            sequence.Append(
                this.IdleSprite.DOFade(0, this.FadeTime)
                    .OnComplete(() =>
                    {
                        this.IdleSprite.gameObject.SetActive(false);
                        this.IdleSprite.color = new Color(1, 1, 1, 1);
                    })
            );
        }

        public void SetMainSpriteMaterial(Material material)
        {
            ApplyMaterial(this.MainSprite, material);
        }

        public void SetPlayerMoveSpriteMaterial(Material material)
        {
            ApplyMaterial(this.PlayerMoveSprite, material);
        }

        public void SetIdleSpriteMaterial(Material material)
        {
            ApplyMaterial(this.IdleSprite, material);
        }

        public void SetLoadingSpriteMaterial(Material material)
        {
            ApplyMaterial(this.LoadingSprite, material);
        }

        private void OnDestroy()
        {
            // Clean up DOTween animations
            if (this.PlayerMoveSprite != null)
            {
                DOTween.Kill(this.PlayerMoveSprite.transform);
                DOTween.Kill(this.PlayerMoveSprite);
            }

            if (this.IdleSprite != null)
            {
                DOTween.Kill(this.IdleSprite);
            }
        }
    }
}
