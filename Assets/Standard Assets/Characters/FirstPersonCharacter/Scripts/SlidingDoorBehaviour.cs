using UnityEngine;
using System.Collections;

public class SlidingDoorBehaviour : InteractableBehaviour
{
    [SerializeField]
    private Vector3 translation = Vector3.zero;
    [SerializeField]
    private float slideTime = 2;

    private bool isClosed;
    Coroutine interaction;
    private Transform transformComponent;
    private float currentTime;

    void Start()
    {
        isClosed = true;
        transformComponent = GetComponent<Transform>();
    }

    override public void startInteraction()
    {
        // interaction disabled while an interaction is in progress
        if (currentTime != 0)
        {
            return;
        }

        if (interaction != null)
        {
            StopCoroutine(interaction);
        }
        interaction = StartCoroutine(slideDoor());
    }

    IEnumerator slideDoor()
    {
        currentTime = 0;
        Vector3 slidePosition = transformComponent.position + translation;
        while (currentTime / slideTime < 0.9)
        {
            currentTime += Time.deltaTime;
            transformComponent.position = Vector3.Lerp(transformComponent.position, slidePosition, (currentTime / slideTime) * (currentTime / slideTime));
            yield return null;
        }
        //transform.position = slidePosition;
        isClosed = !isClosed;
        translation = -translation;
        currentTime = 0;
        yield return null;
    }

    override public void endInteraction()
    {
    }
}