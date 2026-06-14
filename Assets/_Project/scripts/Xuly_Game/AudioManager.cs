using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource backgroundAudioSource;
    [SerializeField] private AudioSource effectAudioSource;

    [Header("Background Music")]
    [SerializeField] private AudioClip backGroundClip;

    [Header("Player Sounds")]
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip attackClip;
    [SerializeField] private AudioClip takeHitClip;
    [SerializeField] private AudioClip dieClip;
    [SerializeField] private AudioClip pickupClip;

    [Header("Game Sounds")]
    [SerializeField] private AudioClip gameOverClip;
    [SerializeField] private AudioClip victoryClip;

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
        PlayBackGroundMusic();
    }

    // ================= BGM =================

    public void PlayBackGroundMusic()
    {
        if (backgroundAudioSource == null) return;
        if (backGroundClip == null) return;

        backgroundAudioSource.clip = backGroundClip;
        backgroundAudioSource.loop = true;
        backgroundAudioSource.Play();
    }

    // ================= PLAYER =================

    public void PlayJumpSound()
    {
        if (jumpClip != null)
            effectAudioSource.PlayOneShot(jumpClip);
    }

    public void PlayAttackSound()
    {
        if (attackClip != null)
            effectAudioSource.PlayOneShot(attackClip);
    }

    public void PlayTakeHitSound()
    {
        if (takeHitClip != null)
            effectAudioSource.PlayOneShot(takeHitClip);
    }

    public void PlayDieSound()
    {
        if (dieClip != null)
            effectAudioSource.PlayOneShot(dieClip);
    }

    public void PlayPickupSound()
    {
        if (pickupClip != null)
            effectAudioSource.PlayOneShot(pickupClip);
    }

    // ================= GAME =================

    public void PlayGameOverSound()
    {
        if (gameOverClip != null)
            effectAudioSource.PlayOneShot(gameOverClip);
    }

    public void PlayVictorySound()
    {
        if (victoryClip != null)
            effectAudioSource.PlayOneShot(victoryClip);
    }
}