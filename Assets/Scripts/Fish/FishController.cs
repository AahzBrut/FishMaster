using DG.Tweening;
using ScriptableObjects;
using UnityEngine;

namespace Fish
{
    public class FishController : MonoBehaviour
    {
        public FishData FishData
        {
            get => fishData;
            set
            {
                fishData = value;
                myCollider.radius = fishData.size;
                spriteRenderer.sprite = fishData.sprite;
            }
        }

        private CircleCollider2D myCollider;
        private Transform myTransform;

        private SpriteRenderer spriteRenderer;

        private Tweener tween;
        private FishData fishData;

        private float leftScreenCoord;
        
        private const float MovementDepthSpread = 1f;

        private void Awake()
        {
            myTransform = transform;
            myCollider = GetComponent<CircleCollider2D>();
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            leftScreenCoord = Camera.main != null ? Camera.main.ScreenToWorldPoint(Vector3.zero).x : 0f;
        }

        public void ResetFish()
        {
            tween?.Kill();
            var depth = Random.Range(fishData.minDepth, fishData.maxDepth);
            myCollider.enabled = true;
            myTransform.position = new Vector2(leftScreenCoord, depth);

            var moveDepth = Random.Range(depth - MovementDepthSpread, depth + MovementDepthSpread);
            var speed = Random.Range(fishData.minSpeed, fishData.maxSpeed);
            tween = myTransform
                .DOMove(new Vector3(-myTransform.position.x, moveDepth), speed)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.Linear)
                .SetDelay(Random.Range(0f, .5f));
            tween.onStepComplete = ReverseDirection;
        }

        public void OnHooked()
        {
            myCollider.enabled = false;
            tween.Kill();
        }

        private void ReverseDirection()
        {
            var localScale = myTransform.localScale;
            localScale.x = -localScale.x;
            myTransform.localScale = localScale;
        }
    }
}