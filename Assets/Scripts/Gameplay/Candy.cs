using UnityEngine;
using CandyCrush.Core;

namespace CandyCrush.Gameplay
{
    /// <summary>
    /// Represents a single candy on the game grid
    /// </summary>
    public class Candy : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Sprite[] candySprites = new Sprite[6]; // Red, Orange, Yellow, Green, Blue, Purple
        [SerializeField] private float fallSpeed = 5f;
        [SerializeField] private float swapAnimationDuration = 0.2f;

        private int gridX;
        private int gridY;
        private CandyType candyType;
        private SpecialCandyType specialType;
        private bool isAnimating;
        private Vector3 targetPosition;
        private Collider2D candyCollider;

        public int GridX => gridX;
        public int GridY => gridY;
        public CandyType CandyType => candyType;
        public SpecialCandyType SpecialType => specialType;
        public bool IsAnimating => isAnimating;

        private void Awake()
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
            if (candyCollider == null)
                candyCollider = GetComponent<Collider2D>();
        }

        /// <summary>
        /// Initialize candy with type and position
        /// </summary>
        public void Initialize(int x, int y, CandyType type)
        {
            gridX = x;
            gridY = y;
            candyType = type;
            specialType = SpecialCandyType.None;
            isAnimating = false;
            targetPosition = transform.position;

            UpdateSprite();
        }

        /// <summary>
        /// Update candy sprite based on type
        /// </summary>
        public void UpdateSprite()
        {
            if (candyType == CandyType.None)
            {
                spriteRenderer.sprite = null;
                candyCollider.enabled = false;
                return;
            }

            int spriteIndex = (int)candyType - 1; // Enum starts at 1
            if (spriteIndex >= 0 && spriteIndex < candySprites.Length)
            {
                spriteRenderer.sprite = candySprites[spriteIndex];
            }

            candyCollider.enabled = true;
        }

        /// <summary>
        /// Set special candy type and update visual
        /// </summary>
        public void SetSpecialType(SpecialCandyType type)
        {
            specialType = type;
            // TODO: Update visual based on special type (add glow, pattern, etc.)
        }

        /// <summary>
        /// Animate candy falling to target position
        /// </summary>
        public void Fall(Vector3 newTargetPosition)
        {
            targetPosition = newTargetPosition;
            isAnimating = true;
        }

        /// <summary>
        /// Animate candy swap with another candy
        /// </summary>
        public void AnimateSwap(Vector3 targetPos)
        {
            StartCoroutine(SwapCoroutine(targetPos));
        }

        private System.Collections.IEnumerator SwapCoroutine(Vector3 targetPos)
        {
            isAnimating = true;
            Vector3 startPos = transform.position;
            float elapsed = 0f;

            while (elapsed < swapAnimationDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / swapAnimationDuration;
                transform.position = Vector3.Lerp(startPos, targetPos, t);
                yield return null;
            }

            transform.position = targetPos;
            targetPosition = targetPos;
            isAnimating = false;
        }

        /// <summary>
        /// Animate candy match/clear
        /// </summary>
        public void AnimateClear()
        {
            StartCoroutine(ClearCoroutine());
        }

        private System.Collections.IEnumerator ClearCoroutine()
        {
            isAnimating = true;
            float elapsed = 0f;
            float duration = 0.3f;
            Vector3 startScale = transform.localScale;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
                yield return null;
            }

            transform.localScale = Vector3.zero;
            candyType = CandyType.None;
            specialType = SpecialCandyType.None;
            candyCollider.enabled = false;
            isAnimating = false;
        }

        /// <summary>
        /// Update physics each frame
        /// </summary>
        private void Update()
        {
            if (!isAnimating && candyType != CandyType.None)
            {
                // Gravity simulation
                if (transform.position != targetPosition)
                {
                    Vector3 direction = (targetPosition - transform.position).normalized;
                    transform.position += direction * fallSpeed * Time.deltaTime;

                    // Snap to target if close enough
                    if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
                    {
                        transform.position = targetPosition;
                    }
                }
            }
        }

        /// <summary>
        /// Update grid position
        /// </summary>
        public void SetGridPosition(int x, int y)
        {
            gridX = x;
            gridY = y;
        }

        /// <summary>
        /// Swap candy types with another candy
        /// </summary>
        public void SwapWith(Candy other)
        {
            // Swap types
            CandyType tempType = candyType;
            SpecialCandyType tempSpecial = specialType;

            candyType = other.candyType;
            specialType = other.specialType;

            other.candyType = tempType;
            other.specialType = tempSpecial;

            // Update sprites
            UpdateSprite();
            other.UpdateSprite();
        }
    }
}
