using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace JSAM
{
    public class AudioEvents : MonoBehaviour
    {
        private AudioManager _audioManager;

        [Inject]
        public void Construct(AudioManager audioManager)
        {
            _audioManager = audioManager;
        }
        
        public void PlayAudioPlayer(AudioPlayer player)
        {
            player.Play();
        }

        private int SoundNameToIndex(string soundName)
        {
            var enumName = soundName;
            if (soundName.Contains("."))
            {
                enumName = soundName.Remove(0, soundName.LastIndexOf('.') + 1);
            }

            var enums = new List<string>();
            var enumType = _audioManager.GetSceneSoundEnum();
            enums.AddRange(Enum.GetNames(enumType));
            return enums.IndexOf(enumName);
        }

        /// <summary>
        /// Takes the name of the Audio enum sound to be played as a string and plays it without spatializing.
        /// </summary>
        /// <param name="enumName">Either specify the name by it's Audio File name or use the entire enum</param>
        public void PlaySoundByEnum(string enumName)
        {
            var index = SoundNameToIndex(enumName);

            if (index > -1)
            {
                _audioManager.PlaySoundInternal(index, transform);
            }
        }

        public void PlayLoopingSoundByEnum(string enumName)
        {
            var index = SoundNameToIndex(enumName);

            if (index > -1)
            {
                _audioManager.PlaySoundLoopInternal(index, transform);
            }
        }

        public void StopLoopingSoundByEnum(string enumName)
        {
            var index = SoundNameToIndex(enumName);

            if (index > -1)
            {
                if (_audioManager.IsSoundLoopingInternal(index))
                {
                    _audioManager.StopSoundLoopInternal(index, false, transform);
                }
            }
        }

        public void SetMasterVolume(float newVal)
        {
            _audioManager.SetMasterVolumeInternal(newVal);
        }

        public void SetMusicVolume(float newVal)
        {
            _audioManager.SetMusicVolumeInternal(newVal);
        }

        public void SetSoundVolume(float newVal)
        {
            _audioManager.SetSoundVolumeInternal(newVal);
        }
    }
}