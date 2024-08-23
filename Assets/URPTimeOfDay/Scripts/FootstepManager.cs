using UnityEngine;
using TimeOfDayURP;

public class FootstepManager : MonoBehaviour
{
    public TimeManager timeManager;
    private AudioSource audioSource;
    public Transform Player;
    public Transform FootLeft;
    public Transform FootRight;
    public Transform RotationSource;
    [Space]
    public GameObject WaterSplashPrefab;
    public AudioClip WaterSplashSound;
    [Space]
    public GameObject SnowPrefab;
    public AudioClip SnowFootstepSound;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }
    public void LeftFootStep()
    {
        if (timeManager.enableRain && timeManager.CurrentRainAmount >= 0.6f)
        {
            Instantiate(WaterSplashPrefab, FootLeft.position, Quaternion.identity);
            audioSource.PlayOneShot(WaterSplashSound);
        }
        else if (timeManager.enableSnow && timeManager.CurrentSnowAmount >= 0.6f)
        {
            Quaternion rotation = Quaternion.Euler(0, RotationSource.rotation.eulerAngles.y, 0);
            Instantiate(SnowPrefab, FootLeft.position, rotation);
            audioSource.PlayOneShot(SnowFootstepSound);
        }
    }

    public void RightFootStep()
    {
        if (timeManager.enableRain && timeManager.CurrentRainAmount >= 0.6f)
        {
            Instantiate(WaterSplashPrefab, FootRight.position, Quaternion.identity);
            audioSource.PlayOneShot(WaterSplashSound);
        }
        else if (timeManager.enableSnow && timeManager.CurrentSnowAmount >= 0.6f)
        {
            Quaternion rotation = Quaternion.Euler(0, RotationSource.rotation.eulerAngles.y, 0);
            Instantiate(SnowPrefab, FootRight.position, rotation);
            audioSource.PlayOneShot(SnowFootstepSound);
        }
    }
}
