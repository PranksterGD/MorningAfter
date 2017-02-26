using UnityEngine;

public class EndingControl : MonoBehaviour
{
    private static readonly string INTERACTABLE_TAG = "Interactable";
    private static readonly string PHONE_VIBRATOR_CLIP = "Phone Vibrate";

    private AudioSource audioSource;
    private AudioClip audioClip;
    private Emissive emissive;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioClip = Resources.Load<AudioClip>(PHONE_VIBRATOR_CLIP);
        emissive = GetComponent<Emissive>();
    }

    public void MakeInteractable()
    {
        gameObject.tag = INTERACTABLE_TAG;
    }

    public void PlayEnding()
    {
        audioSource.clip = audioClip;
        audioSource.loop = true;
        audioSource.Play();
        emissive.lookAt(true);
    }
}
