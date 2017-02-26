using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Characters.FirstPerson;

public class GameManager : MonoBehaviour
{
    private static readonly string INTERACTABLE_TAG = "Interactable";
    private static readonly string DOOR_SFX = "SlidingDoorSFX";    

    [SerializeField]
    private Image image;

    [SerializeField]
    private float FadeInTime;

    [SerializeField]
    private EndingControl finalPhone;

    [SerializeField]
    private AudioSource voiceMailPhone;

    private GameObject[] interactables;
    private FirstPersonController firstPersonController;
    private MouseLook mouseLook;
    private AudioSource audioSource;
    private List<GameObject> objectsToBeInteracted;
    private bool gameStarted = false;
    private bool debugEnabled = false;

    static public int numberOfInteractableObjects;

    void Start()
    {
        interactables = GameObject.FindGameObjectsWithTag(INTERACTABLE_TAG);
        objectsToBeInteracted = new List<GameObject>(interactables);
        numberOfInteractableObjects = objectsToBeInteracted.Count;
        firstPersonController = GetComponent<FirstPersonController>();
        mouseLook = GetComponent<MouseLook>();
        audioSource = GetComponent<AudioSource>();
        firstPersonController.ToggleHasControl(false);
        mouseLook.setMovementLocked(true);
    }
    
    public void interacted(GameObject gameObject)
    {
        if (objectsToBeInteracted.Contains(gameObject))
        {
            objectsToBeInteracted.Remove(gameObject);
            numberOfInteractableObjects--;
        }

        if (numberOfInteractableObjects == 0)
        {
            StartCoroutine(triggerFinalSequence(gameObject.GetComponent<AudioSource>().clip.length));
        }
    }

    IEnumerator triggerFinalSequence(float waitSeconds)
    {
        yield return new WaitForSeconds(waitSeconds);
        finalPhone.MakeInteractable();
        finalPhone.PlayEnding();
    }

    void Update()
    {
        if (!gameStarted && Input.GetKeyDown(KeyCode.E))
        {
            firstPersonController.ToggleHasControl(true);
            mouseLook.setMovementLocked(false);
            StartCoroutine(FadeOutImage());
            gameStarted = true;
            AudioClip doorSound = Resources.Load<AudioClip>(DOOR_SFX);
            audioSource.PlayOneShot(doorSound);
        }
        
        if(Input.GetKeyDown(KeyCode.F11)&& !debugEnabled)
        {
            numberOfInteractableObjects = 0;
            debugEnabled = true;
            finalPhone.MakeInteractable();
            finalPhone.PlayEnding();
        }
    }

    IEnumerator FadeOutImage()
    {        
        while (image.canvasRenderer.GetAlpha() > 0.05f)
        {
            image.CrossFadeAlpha(0, FadeInTime, true);
            yield return null;
        }
        image.enabled = false;
        voiceMailPhone.Play();
    }
}
