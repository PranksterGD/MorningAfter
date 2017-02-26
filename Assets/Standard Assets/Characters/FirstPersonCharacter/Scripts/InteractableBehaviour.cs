using UnityEngine;

public abstract class InteractableBehaviour: MonoBehaviour
{
    [SerializeField]
    private bool allowZoomIn = false;

    public bool canZoomIn()
    {
        return allowZoomIn;
    }

    public abstract void startInteraction();

    public abstract void endInteraction();
}