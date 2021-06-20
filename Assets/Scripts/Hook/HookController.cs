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
        private Camera _mainCamera;
        private Collider2D _myCollider;
        private Transform _myTransform;
        private Transform _hookedTransform;

        private int _length;
        private int _strength;
        private int _fishCount;

        private readonly List<FishController> _hookedFishes = new List<FishController>();

        private bool _canMove;

        private Tweener _cameraTween;

        private const float MinFishingDepth = -25f;
        private const float CameraFollowDepth = -11f;
        private const float HookStartingDepth = -6f;

        private IdleManager _idleManager;
        private ScreenManager _screenManager;

        [Inject]
        public void ConstructorHandling(IdleManager idleManager, ScreenManager screenManager)
        {
            _idleManager = idleManager;
            _screenManager = screenManager;
        }

        private void Awake()
        {
            _mainCamera = Camera.main;
            _myCollider = GetComponent<Collider2D>();
            _myTransform = transform;
            _hookedTransform = _myTransform.Find("HookedCatch").transform;
            _idleManager.CastHook = StartFishing;
        }

        private void Update()
        {
            if (_canMove && Input.GetMouseButton(0))
            {
                var vector = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
                var position = _myTransform.position;
                position.x = vector.x;
                _myTransform.position = position;
            }
        }

        private void StartFishing()
        {
            _length = -_idleManager.Length;
            _strength = _idleManager.Strength;
            _fishCount = 0;
            var time = -_length * .1f;

            _cameraTween = _mainCamera.transform.DOMoveY(_length, 1 + time * .25f);
            _cameraTween.onUpdate = AttachHookToCameraOnMoveDown;
            _cameraTween.onComplete = () => MoveHookUp(time);

            _screenManager.ChangeScreen(Screens.Game);
            
            _myCollider.enabled = false;
            _canMove = true;
            _hookedFishes.Clear();
        }

        private void AttachHookToCameraOnMoveDown()
        {
            if (_mainCamera.transform.position.y <= CameraFollowDepth)
                _myTransform.SetParent(_mainCamera.transform);
        }

        private void MoveHookUp(float time)
        {
            _myCollider.enabled = true;
            _cameraTween = _mainCamera.transform.DOMoveY(0f, time * 5f);
            _cameraTween.onUpdate = CheckDepthForStopFishing;
        }

        private void ResetHook()
        {
            _myCollider.enabled = false;
            var sumCatchPrice = 0;
            foreach (var fish in _hookedFishes)
            {
                fish.transform.SetParent(null);
                fish.ResetFish();
                sumCatchPrice += fish.FishData.price;
            }

            _idleManager.TotalGain = sumCatchPrice;
            _screenManager.ChangeScreen(Screens.End);
        }

        private void CheckDepthForStopFishing()
        {
            if (_mainCamera.transform.position.y >= MinFishingDepth)
                StopFishing();
        }

        private void StopFishing()
        {
            _canMove = false;
            _cameraTween.Kill();
            _cameraTween = _mainCamera.transform.DOMoveY(0, 2);
            _cameraTween.onUpdate = CheckForResetHook;
            _cameraTween.onComplete = ResetHook;
        }

        private void CheckForResetHook()
        {
            if (_mainCamera.transform.position.y >= CameraFollowDepth)
            {
                _myTransform.SetParent(null);
                _myTransform.position = new Vector2(transform.position.x, HookStartingDepth);
            }
        }

        private void OnTriggerEnter2D(Collider2D target)
        {
            if (target.CompareTag("Fish") && _strength > _fishCount)
            {
                _fishCount++;
                var fishController = target.GetComponent<FishController>();
                _hookedFishes.Add(fishController);
                fishController.OnHooked();
                var targetTransform = target.transform;
                targetTransform.SetParent(_hookedTransform);
                targetTransform.position = _hookedTransform.position;
                targetTransform.rotation = _hookedTransform.rotation;
                targetTransform.localScale = Vector3.one;
                var shakeTween = targetTransform.DOShakeRotation(5f, Vector3.forward * 45f).SetLoops(1, LoopType.Yoyo);
                shakeTween.onComplete = () => ResetRotation(targetTransform);
            }
            if (_strength == _fishCount) StopFishing();
        }

        private static void ResetRotation(Transform target)
        {
            target.rotation = Quaternion.identity;
        }
    }
}
