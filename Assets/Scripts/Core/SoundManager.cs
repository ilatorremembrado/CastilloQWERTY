using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public AudioClip buttonClick;
    public AudioClip deathEnemySound;
    public AudioClip deathPlayerSound;
    public AudioClip hurtPlayerSound;
    public AudioClip letterCorrectSound;
    public AudioClip bonusSound;
    public AudioClip wordCorrectSound;
    public AudioClip passLevelSound;

    private AudioSource audioSource;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySound(AudioClip clip, float volume = 1f)
    {
        audioSource.PlayOneShot(clip, volume);
    }
}
