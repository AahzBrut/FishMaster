using System.Collections.Generic;
using DG.Tweening;
using Fish;
using Managers;
using UnityEngine;
using Zenject;

namespace Hook
{
    public class HookController : MonoBehaviour
    {
        private Camera mainCamera;
        private Collider2D myCollider;
        private Transform myTransform;
        private Transform hookedTransform;

        private int length;
        private int strength;
        private int fishCount;

        private readonly List<FishController> hookedFishes = new List<FishController>();

        private bool canMove;
        private bool canCastHook = true;

        private Tweener cameraTween;

        private const float MinFishingDepth = -25f;
        private const float CameraFollowDepth = -11f;
        private const float HookStartingDepth = -6f;

        private IdleManager idleManager;

        [Inject]
        public void ConstructorHandling(IdleManager idleManagerZ)
        {
            idleManager = idleManagerZ;
        }

        private void Awake()
        {
            mainCamera = Camera.main;
            myCollider = GetComponent<Collider2D>();
            myTransform = transform;
            hookedTransform = myTransform.Find("HookedCatch").transform;
            idleManager.Length = 30;
        }

        private void Update()
        {
            if (canMove && Input.GetMouseButton(0))
            {
                var vector = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                var position = myTransform.position;
                position.x = vector.x;
                myTransform.position = position;
            }
            if (canCastHook && Input.GetMouseButton(0))
                StartFishing();
        }

        private void StartFishing()
        {
            length = -50;
            strength = 3;
            fishCount = 0;
            var time = -length * .1f;

            cameraTween = mainCamera.transform.DOMoveY(length, 1 + time * .25f);
            cameraTween.onUpdate = AttachHookToCameraOnMoveDown;
            cameraTween.onComplete = () => MoveHookUp(time);

            myCollider.enabled = false;
            canMove = true;
            canCastHook = false;
            hookedFishes.Clear();
        }

        private void AttachHookToCameraOnMoveDown()
        {
            if (mainCamera.transform.position.y <= CameraFollowDepth)
                myTransform.SetParent(mainCamera.transform);
        }

        private void MoveHookUp(float time)
        {
            myCollider.enabled = true;
            cameraTween = mainCamera.transform.DOMoveY(0f, time * 5f);
            cameraTween.onUpdate = CheckDepthForStopFishing;
        }

        private void ResetHook()
        {
            myCollider.enabled = false;
            canCastHook = true;
            var sumCatchPrice = 0;
            foreach (var fish in hookedFishes)
            {
                fish.transform.SetParent(null);
                fish.ResetFish();
                sumCatchPrice += fish.FishData.price;
            }
        }

        private void CheckDepthForStopFishing()
        {
            if (mainCamera.transform.position.y >= MinFishingDepth)
                StopFishing();
        }

        private void StopFishing()
        {
            canMove = false;
            cameraTween.Kill();
            cameraTween = mainCamera.transform.DOMoveY(0, 2);
            cameraTween.onUpdate = CheckForResetHook;
            cameraTween.onComplete = ResetHook;
        }

        private void CheckForResetHook()
        {
            if (mainCamera.transform.position.y >= CameraFollowDepth)
            {
                myTransform.SetParent(null);
                myTransform.position = new Vector2(transform.position.x, HookStartingDepth);
            }
        }

        private void OnTriggerEnter2D(Collider2D target)
        {
            if (target.CompareTag("Fish") && strength > fishCount)
            {
                fishCount++;
                var fishController = target.GetComponent<FishController>();
                hookedFishes.Add(fishController);
                fishController.OnHooked();
                var targetTransform = target.transform;
                targetTransform.SetParent(hookedTransform);
                targetTransform.position = hookedTransform.position;
                targetTransform.rotation = hookedTransform.rotation;
                targetTransform.localScale = Vector3.one;
                var shakeTween = targetTransform.DOShakeRotation(5f, Vector3.forward * 45f).SetLoops(1, LoopType.Yoyo);
                shakeTween.onComplete = () => ResetRotation(targetTransform);
            }
            if (strength == fishCount) StopFishing();
        }

        private static void ResetRotation(Transform target)
        {
            target.rotation = Quaternion.identity;
        }
    }
}
