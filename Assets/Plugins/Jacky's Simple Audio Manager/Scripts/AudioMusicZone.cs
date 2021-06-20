using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace JSAM
{
    [AddComponentMenu("AudioManager/Audio Music Zone")]
    public class AudioMusicZone : BaseAudioMusicFeedback
    {
        public List<Vector3> positions = new List<Vector3>();
        public List<float> maxDistance = new List<float>();
        public List<float> minDistance = new List<float>();

        AudioListener listener;

        AudioSource source;

        private AudioManager _audioManager;

        [Inject]
        public void Construct(AudioManager audioManager)
        {
            _audioManager = audioManager;
        }

        // Start is called before the first frame update
        new void Start()
        {
            base.Start();
            listener = _audioManager.GetListenerInternal();

            source = _audioManager.GetMusicSource();
        }

        // Update is called once per frame
        private void Update()
        {
            float loudest = 0;
            for (var i = 0; i < positions.Count; i++)
            {
                var dist = Vector3.Distance(listener.transform.position, positions[i]);
                if (dist <= maxDistance[i])
                {
                    if (!_audioManager.IsMusicPlayingInternal(music))
                    {
                        source = _audioManager.PlayMusicInternal(music);
                    }

                    if (dist <= minDistance[i])
                    {
                        // Set to the max volume
                        source.volume = AudioManager.GetTrueMusicVolume() * music.relativeVolume;
                        return; // Can't be beat
                    }
                    else
                    {
                        var distanceFactor = Mathf.InverseLerp(maxDistance[i], minDistance[i], dist);
                        var newVol = AudioManager.GetTrueMusicVolume() * music.relativeVolume * distanceFactor;
                        if (newVol > loudest) loudest = newVol;
                    }
                }
            }
            if (_audioManager.IsMusicPlayingInternal(music)) source.volume = loudest;
        }
    }
}