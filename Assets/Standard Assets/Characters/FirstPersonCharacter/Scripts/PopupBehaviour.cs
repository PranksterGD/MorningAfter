using UnityEngine;
using UnityEngine.UI;

public class PopupBehaviour : InteractableBehaviour
{
    [SerializeField]
    private Image overlayImage;   

    public override void startInteraction()
    {
        overlayImage.enabled = true;
    }

    public override void endInteraction()
    {
        print("");
        overlayImage.enabled = false;
    }
}
