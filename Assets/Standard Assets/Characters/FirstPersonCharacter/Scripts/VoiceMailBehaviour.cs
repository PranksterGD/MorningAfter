using UnityEngine;
using System.Collections;

public class VoiceMailBehaviour : InteractableBehaviour
{
    private static readonly string MESSAGE_CLIP = "EditedVoiceMailPassFilters";

    private GameObject player;
    private bool firstTimeInteraction = true;
    void Start()
    {
        player = GameObject.Find("Player");
    }

    override public void startInteraction()
    {
        AudioClip message = Resources.Load<AudioClip>(MESSAGE_CLIP);
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.loop = false;
        audioSource.clip = message;
        audioSource.Play();
        if(firstTimeInteraction)
        {
            StartCoroutine(playGameMusic(audioSource.clip.length));
            firstTimeInteraction = false;
        }
      
    }    

    override public void endInteraction()
    {
    }

    IEnumerator playGameMusic(float waitForSeconds)
    {
        yield return new WaitForSeconds(waitForSeconds);
        player.GetComponent<AudioSource>().Play();
    }
    
}