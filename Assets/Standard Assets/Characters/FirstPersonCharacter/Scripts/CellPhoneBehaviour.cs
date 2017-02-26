using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CellPhoneBehaviour : InteractableBehaviour
{
    private static readonly string MESSAGE_CLIP = "EditedVoiceMailPassFilters";
    private static readonly string NEGATIVE_CLIP = "PhoneResolutionNegativeEnding";
    private static readonly string POSITIVE_CLIP = "PhoneResolutionPositiveEnding";
    private static readonly string DECISION_CLIP = "ResolutionPredecision";

    [SerializeField]
    private Image callingImage;

    [SerializeField]
    private Image optionsImage;

    [SerializeField]
    private Image creditsImage;

    [SerializeField]
    private float creditsFadeInTime;

    private AudioSource audioSource;

    private bool acceptOptions=false;
    private bool readyToExit = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        creditsImage.enabled = false;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E)&& acceptOptions)
        {
            audioSource.loop = false;
            audioSource.clip = Resources.Load<AudioClip>(POSITIVE_CLIP);
            audioSource.Play();
            acceptOptions = false;
            StartCoroutine(loadCredits(audioSource.clip.length));
        }

        if (Input.GetKeyDown(KeyCode.X) && acceptOptions)
        {
            audioSource.loop = false;
            audioSource.clip = Resources.Load<AudioClip>(NEGATIVE_CLIP);
            audioSource.Play();
            acceptOptions = false;
            StartCoroutine(loadCredits(audioSource.clip.length));
        }

        if (Input.GetKeyDown(KeyCode.E) && readyToExit)
        {
            Application.Quit();
        }
    }

    override public void startInteraction()
    {
        this.gameObject.tag="Untagged";
        GameManager.numberOfInteractableObjects++;
        callingImage.enabled = true;
        audioSource.loop = false;
        audioSource.Stop();
        audioSource.clip = Resources.Load<AudioClip>(DECISION_CLIP);
        audioSource.Play();
        StartCoroutine(displayOptionsScreen(audioSource.clip.length));
    }

    override public void endInteraction()
    {
    }

    IEnumerator displayOptionsScreen(float waitSeconds)
    {        
        yield return new WaitForSeconds(waitSeconds);
        audioSource.Stop();
        callingImage.enabled = false;
        optionsImage.enabled = true;
        acceptOptions = true;
    }

    IEnumerator loadCredits(float waitSeconds)
    {
        yield return new WaitForSeconds(waitSeconds);
        creditsImage.enabled = true;
        optionsImage.enabled = false;
        creditsImage.canvasRenderer.SetAlpha(0.01f);

        while (creditsImage.canvasRenderer.GetAlpha() < 0.95f)
        {
            creditsImage.CrossFadeAlpha(1, creditsFadeInTime, true);
            yield return null;
        }
        creditsImage.canvasRenderer.SetAlpha(1);
        readyToExit = true;
    }
}