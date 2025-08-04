using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SaveCompatibility;

namespace F
{
    public class AudioManager : MonoBehaviour
    {

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
				if (transform.parent != null)
                {
                    transform.SetParent(null); 
                }
                DontDestroyOnLoad(gameObject);
                CleanUpAudioSources(); 
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }

            _musicVolume = LocalPlayerPrefs.GetFloat("MusicVolume", 1f);
            _sfxVolume = LocalPlayerPrefs.GetFloat("SFXVolume", 1f);
        }


        private void CleanUpAudioSources()
        {

            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            audioSourcePools.Clear();
        }

        private void CreatePoolForTag(string tag)
        {
            if (!audioSourcePools.ContainsKey(tag))
            {
                audioSourcePools[tag] = new Queue<AudioSource>();
                ExpandAudioSourcePool(tag, initialPoolSize);
            }
        }

        private void ExpandAudioSourcePool(string tag, int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                AudioSource source = CreateNewAudioSource(tag);
                audioSourcePools[tag].Enqueue(source);
            }
        }

        private AudioSource CreateNewAudioSource(string tag)
        {
            GameObject newAudioObj = new GameObject($"{tag}_PooledAudioSource");
            newAudioObj.transform.SetParent(transform); 
            AudioSource source = newAudioObj.AddComponent<AudioSource>();
            newAudioObj.SetActive(false);
            return source;
        }

        public AudioSource GetPooledAudioSource(string tag = "default")
        {
            if (!audioSourcePools.ContainsKey(tag))
            {
                CreatePoolForTag(tag);
            }

            Queue<AudioSource> pool = audioSourcePools[tag];
            if (pool.Count > 0)
            {
                AudioSource source = pool.Dequeue();
                source.gameObject.SetActive(true);
                return source;
            }

            ExpandAudioSourcePool(tag, 5);
            return GetPooledAudioSource(tag);
        }

        public void ReturnAudioSourceToPool(AudioSource source, float delay)
        {
            StartCoroutine(ReturnToPoolAfterDelay(source, delay));
        }

        private IEnumerator ReturnToPoolAfterDelay(AudioSource source, float delay)
        {
            yield return new WaitForSeconds(delay);
            source.Stop();
            source.gameObject.SetActive(false);
            string tag = source.gameObject.name.Split('_')[0]; 
            if (!audioSourcePools.ContainsKey(tag))
            {
                audioSourcePools[tag] = new Queue<AudioSource>();
            }
            audioSourcePools[tag].Enqueue(source);
        }

        public float MusicVolume
        {
            get { return _musicVolume; }
            set
            {
                _musicVolume = value;
                musicSource.volume = _musicVolume;
                LocalPlayerPrefs.SetFloat("MusicVolume", _musicVolume);
            }
        }

        public float SFXVolume
        {
            get { return _sfxVolume; }
            set
            {
                _sfxVolume = value;
                LocalPlayerPrefs.SetFloat("SFXVolume", _sfxVolume);
            }
        }

        public void PlayMusic(AudioClip clip)
        {
            musicSource.clip = clip;
            musicSource.Play();
        }

        public void SetLowPassFilter(bool isOn)
        {
            musicLowPassFilter.enabled = isOn;
        }

        public void FadeInMusic(float fadeDuration)
		{
			this.StopMusicFade();
			this._musicFadeCR = this.FadeInMusicCR(fadeDuration);
			base.StartCoroutine(this._musicFadeCR);
		}

		public void FadeOutMusic(float fadeDuration)
		{
			this.StopMusicFade();
			this._musicFadeCR = this.FadeOutMusicCR(fadeDuration);
			base.StartCoroutine(this._musicFadeCR);
		}

        private void StopMusicFade()
		{
			if (this._musicFadeCR != null)
			{
				base.StopCoroutine(this._musicFadeCR);
				this._musicFadeCR = null;
				this.musicSource.volume = this._musicVolume;
			}
		}

        private IEnumerator FadeInMusicCR(float fadeDuration)
		{
			this.musicSource.volume = 0f;
			while (this.musicSource.volume < this._musicVolume)
			{
				this.musicSource.volume += this._musicVolume * Time.unscaledDeltaTime / fadeDuration;
				yield return null;
			}
			this.musicSource.volume = this._musicVolume;
			this._musicFadeCR = null;
			yield break;
		}

		private IEnumerator FadeOutMusicCR(float fadeDuration)
		{
			while (this.musicSource.volume > 0f)
			{
				this.musicSource.volume -= this._musicVolume * Time.unscaledDeltaTime / fadeDuration;
				yield return null;
			}
			this.musicSource.Stop();
			this.musicSource.volume = this._musicVolume;
			this._musicFadeCR = null;
			yield break;
		}

		public static AudioManager Instance;

        [SerializeField] 
		private AudioSource musicSource;

        [SerializeField] 
		private AudioLowPassFilter musicLowPassFilter;

        private float _musicVolume;
        private float _sfxVolume;
        private IEnumerator _musicFadeCR;

        // Pool de AudioSource
        private Dictionary<string, Queue<AudioSource>> audioSourcePools = new Dictionary<string, Queue<AudioSource>>();
        private int initialPoolSize = 10;
    }
}
