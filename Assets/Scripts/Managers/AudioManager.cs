using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public class ManagableSource : MonoBehaviour
    {
        public void Initialize(AudioSource source)
        {
            _source = source;

            CurrentID = GLOBAL.UnassignedString;
            IsPlaying = false;
        }

        public AudioSource Source { get => _source; }
        public string CurrentID { get; private set; }
        public bool IsPlaying { get; private set; }

        AudioSource _source;
        IEnumerator EndClip_Handle = null;

        public void Play(string id, AudioClip clip, float volume = 1, float pitch = 1, bool playLooping = false, float forSeconds = -1)
        {
            if (EndClip_Handle != null) StopImmediately();
            StopCoroutine(nameof(StopSoundIEnum));

            CurrentID = id;

            Source.clip = clip;
            Source.loop = playLooping;
            Source.volume = Mathf.Clamp(volume, 0, 1);
            Source.pitch = Mathf.Clamp(pitch, -3, 3);
            Source.Play();
            IsPlaying = true;

            float duration = playLooping ? forSeconds : clip.length;

            if (duration != -1)
            {
                EndClip_Handle = EndClip(duration);
                StartCoroutine(EndClip_Handle);
            }
        }

        IEnumerator EndClip(float duration)
        {
            duration += .1f;
            yield return new WaitForSeconds(duration);

            EndClip_Handle = null;
            StopImmediately();
        }

        public void Stop(bool afterTheClip = false)
        {
            if (afterTheClip) StopAfterClipIsOver();
            else StopImmediately();
        }

        void StopImmediately()
        {
            if (IsPlaying == false) return;

            if(EndClip_Handle != null)
            {
                StopCoroutine(EndClip_Handle);
                EndClip_Handle = null;
            }
            StopCoroutine(nameof(StopSoundIEnum));

            Source.Stop();
            Source.volume = 1;
            Source.pitch = 1;
            Source.clip = null;
            CurrentID = GLOBAL.UnassignedString;
            IsPlaying = false;
        }
        void StopAfterClipIsOver()
        {
            if(IsPlaying ==false) return;

            StopCoroutine(nameof(StopSoundIEnum));
            StartCoroutine(nameof(StopSoundIEnum));
        }
        IEnumerator StopSoundIEnum()
        {
            float remaining = (Source.clip.length - Source.time) - .05f;
            yield return new WaitForSeconds(remaining);
            StopImmediately();
        }
    }

    public static AudioManager Instance
    {
        get
        {
            if(AUTO_instance == null)
            {
                new GameObject("Audio Manager", new System.Type[] { typeof(AudioManager) });
            }

            return AUTO_instance;
        }
        set
        {
            AUTO_instance = value;
        }
    }
    static AudioManager AUTO_instance = null;

    [SerializeField] int AudioSourceCount = 50;

    List<ManagableSource> _audioSources = new List<ManagableSource>();
    bool _isInitialized = false;

    private void Awake()
    {
        if (AUTO_instance == null)
        {
            Instance = this;
        }
        else if (AUTO_instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Initialize();
    }

    public void Initialize()
    {
        if (_isInitialized) return;

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < AudioSourceCount; i++)
        {
            GameObject go = new GameObject("AudioSource" + i);
            go.transform.parent = transform;

            AudioSource aso = go.AddComponent<AudioSource>();
            ManagableSource ms = go.AddComponent<ManagableSource>();
            ms.Initialize(aso);

            _audioSources.Add(ms);
        }

        _isInitialized = true;
        return;
    }

    public void PlayClip(string id, AudioClip clip, float volume = 1, float pitch = 1, bool playLooping = false, float forSeconds = -1, bool @override = false)
    {
        if (clip == null) return;

        ManagableSource ms = null;

        if (@override)
        {
            ManagableSource[] mSources = FindAllById(id);
            if (mSources.Length > 0)
            {
                ms = mSources[0];
                ms.Stop();
            }
        }

        if (ms == null)  ms = GetFreeSource();

        if (ms == null) return;

        ms.Play(id, clip, volume, pitch, playLooping, forSeconds);
    }

    public void StopClip(string id, bool afterTheClip = false)
    {
        ManagableSource[] mSources = FindAllById(id);

        foreach (var ms in mSources)
        {
            ms.Stop(afterTheClip);
        }
    }

    ManagableSource GetFreeSource() => _audioSources.Find(x => x.IsPlaying == false);
    ManagableSource[] FindAllById(string id) => _audioSources.FindAll(x => x.CurrentID == id).ToArray();
}
