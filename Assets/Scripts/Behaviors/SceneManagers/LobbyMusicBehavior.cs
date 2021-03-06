using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.Tempest.Nightmare {

    public class LobbyMusicBehavior : Photon.PunBehaviour {

        private static LobbyMusicBehavior instance;

        private bool isPlaying = false;

        public AudioSource source;

        public void Awake() {
            if (instance == null) {
                instance = this;
                DontDestroyOnLoad(gameObject);
            } else if (instance != this) {
                Destroy(gameObject);
            }
        }

        public void StartMusic() {
            if (!isPlaying) {
                source.Play();
                isPlaying = true;
                LoadVolume();
            }
        }

        public void StopMusic() {
            source.Stop();
            isPlaying = false;
        }

        public void LoadVolume() {
            if (isPlaying) {
                source.volume = ControlBindingContainer.GetInstance().musicVolume;
            }
        }
    }
}
