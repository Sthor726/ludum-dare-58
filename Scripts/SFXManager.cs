using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;
    public AudioSource sfxSource;
    public AudioClip winSound;
    public AudioClip loseSound;

    
    private void Awake() { Instance = this; }

    public void PlaySFX(AudioClip clip, float volume)
    {
        Debug.Log("Playing sound");
        sfxSource.PlayOneShot(clip, volume);
    }

    public void PlayWinSound() {
        sfxSource.PlayOneShot(winSound, 1f);
    }

    public void PlayLoseSound() {
        sfxSource.PlayOneShot(loseSound, 1f);
    }

}
