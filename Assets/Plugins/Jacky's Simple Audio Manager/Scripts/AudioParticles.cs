using UnityEngine;
using Zenject;

namespace JSAM 
{
    /// <summary>
    /// Plays sounds when a particle system emits particles and when particles die
    /// https://answers.unity.com/questions/693044/play-sound-on-particle-emit-sub-emitter.html
    /// With help from these lovely gents
    /// </summary>

    [AddComponentMenu("AudioManager/Audio Particles")]
    [RequireComponent(typeof(ParticleSystem))]
    public class AudioParticles : BaseAudioFeedback
    {
        enum ParticleEvent
        {
            ParticleEmitted,
            ParticleDeath
        }

        [Header("Particle Settings")]

        [SerializeField]
        private ParticleEvent playSoundOn = ParticleEvent.ParticleEmitted;

        private ParticleSystem _partSys;
        private ParticleSystem.Particle[] _particles;
        private float _lowestLifetime = 99f;
        private AudioManager _audioManager;

        [Inject]
        public void Construct(AudioManager audioManager)
        {
            _audioManager = audioManager;
        }

        private void Awake()
        {
            _partSys = GetComponent<ParticleSystem>();
            _particles = new ParticleSystem.Particle[_partSys.main.maxParticles];
        }

        public void PlaySound()
        {
            switch (playSoundOn)
            {
                case ParticleEvent.ParticleEmitted:
                    _audioManager.PlaySoundInternal(sound, sTransform);
                    break;
                case ParticleEvent.ParticleDeath:
                    _audioManager.PlaySoundInternal(sound, sTransform);
                    break;
            }
        }

        private void LateUpdate()
        {
            if (_partSys.particleCount == 0)
                return;

            var numParticlesAlive = _partSys.GetParticles(_particles);

            GetYoungestParticle(numParticlesAlive, _particles, out var youngestParticleLifetime);
            if (_lowestLifetime > youngestParticleLifetime)
            {
                PlaySound();
            }

            _lowestLifetime = youngestParticleLifetime;
        }

        private static int GetYoungestParticle(int numPartAlive, ParticleSystem.Particle[] particles, out float lifetime)
        {
            var youngest = 0;

            // Change only the particles that are alive
            for (var i = 0; i < numPartAlive; i++)
            {

                if (i == 0)
                {
                    youngest = 0;
                    continue;
                }

                if (particles[i].remainingLifetime > particles[youngest].remainingLifetime)
                    youngest = i;
            }

            lifetime = particles[youngest].startLifetime - particles[youngest].remainingLifetime;

            return youngest;

        }
    }
}