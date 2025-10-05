using UnityEngine;

public class SwipeAnimationSounds : MonoBehaviour
{
    public SFXManager sfxManager;
    public AudioClip swipeAudio;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sfxManager = SFXManager.Instance;
    }

    public void PlayAnimationSound() {
        sfxManager.PlaySFX(swipeAudio, 0.6f);
    }
}
