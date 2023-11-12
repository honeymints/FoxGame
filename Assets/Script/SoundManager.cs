using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{ 
        public static SoundManager instance { get; private set; }
        private AudioSource source; private GameObject obj;
        private Scene currentScene;
        private void Awake()
        { 
                source = GetComponent<AudioSource>();
                instance = this;
        }
        
        public void PlaySound(AudioClip _sound)
        {
                float volumeScale=0.41f;
                source.PlayOneShot(_sound, volumeScale);
        } 
        public void PlaySoundSource(AudioSource audioSource)
        {
                audioSource.Play();
        }
}
