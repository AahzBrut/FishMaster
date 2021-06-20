using System.Collections;
using UnityEngine;
using Zenject;

namespace JSAM
{
    public enum AudioPlaybackBehaviour
    {
        None,
        Play,
        Stop
    }

    [AddComponentMenu("AudioManager/Audio Player")]
    public class AudioPlayer : BaseAudioFeedback
    {
        [Tooltip("Behaviour to trigger when the object this is attached to is created")] [SerializeField]
        private AudioPlaybackBehaviour onStart = AudioPlaybackBehaviour.Play;

        [Tooltip("Behaviour to trigger when the object this is attached to is enabled or when the object is created")]
        [SerializeField]
        private AudioPlaybackBehaviour onEnable = AudioPlaybackBehaviour.None;

        [Tooltip("Behaviour to trigger when the object this is attached to is destroyed or set to in-active")]
        [SerializeField]
        private AudioPlaybackBehaviour onDisable = AudioPlaybackBehaviour.Stop;

        [Tooltip("Behaviour to trigger when the object this is attached to is destroyed")] [SerializeField]
        private AudioPlaybackBehaviour onDestroy = AudioPlaybackBehaviour.None;

        /// <summary>
        /// Boolean prevents the sound from being played multiple times when the Start and OnEnable callbacks intersect
        /// </summary>
        private bool _activated;

        private AudioManager _audioManager;

        [Inject]
        public void Construct(AudioManager audioManager)
        {
            _audioManager = audioManager;
        }

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();

            switch (onStart)
            {
                case AudioPlaybackBehaviour.Play:
                    if (!_activated)
                    {
                        _activated = true;
                        StartCoroutine(PlayOnEnable());
                    }

                    break;
                case AudioPlaybackBehaviour.Stop:
                    Stop();
                    break;
            }
        }

        public AudioSource Play()
        {
            var source = loopSound
                ? _audioManager.PlaySoundLoopInternal(sound, sTransform)
                : _audioManager.PlaySoundInternal(sound, sTransform);

            // Ready to play again later
            _activated = false;

            return source;
        }

        void PlayAtPosition()
        {
            if (loopSound)
            {
                var unused = sound.spatialize ? 
                    _audioManager.PlaySoundLoopInternal(sound, sTransform.position) : 
                    _audioManager.PlaySoundLoopInternal(sound, null);
            }
            else
            {
                var unused = sound.spatialize ? 
                    _audioManager.PlaySoundInternal(sound, sTransform.position) : 
                    _audioManager.PlaySoundInternal(sound, null);
            }

            // Ready to play again later
            _activated = false;
        }

        public void PlaySound()
        {
            Play();
        }

        /// <summary>
        /// Stops the sound instantly
        /// </summary>
        public void Stop()
        {
            if (!loopSound)
            {
                if (_audioManager.IsSoundPlayingInternal(sound, sTransform))
                {
                    _audioManager.StopSoundInternal(sound, sTransform);
                }
            }
            else
            {
                if (_audioManager.IsSoundLoopingInternal(sound))
                {
                    _audioManager.StopSoundLoopInternal(sound, true, sTransform);
                }
            }
        }

        private void OnEnable()
        {
            switch (onEnable)
            {
                case AudioPlaybackBehaviour.Play:
                    if (!_activated)
                    {
                        _activated = true;
                        StartCoroutine(PlayOnEnable());
                    }

                    break;
                case AudioPlaybackBehaviour.Stop:
                    Stop();
                    break;
            }
        }

        private IEnumerator PlayOnEnable()
        {
            while (!_audioManager.Initialized())
            {
                yield return new WaitForEndOfFrame();
            }

            Play();
        }

        private void OnDisable()
        {
            switch (onDisable)
            {
                case AudioPlaybackBehaviour.Play:
                    Play();
                    break;
                case AudioPlaybackBehaviour.Stop:
                    Stop();
                    break;
            }
        }

        private void OnDestroy()
        {
            switch (onDestroy)
            {
                case AudioPlaybackBehaviour.Play:
                    Play();
                    break;
                case AudioPlaybackBehaviour.Stop:
                    Stop();
                    break;
            }
        }
    }
}