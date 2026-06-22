using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Components")]
    [SerializeField] private AudioSource musicSource;

    [Header("Music Tracks")]
    public AudioClip menuMusic;
    public AudioClip gameMusic;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayMusic(menuMusic);
    }

    public void PlayMusic(AudioClip newClip)
    {
        if (newClip == null || musicSource.clip == newClip) return;

        musicSource.clip = newClip;
        musicSource.Play();
    }
}