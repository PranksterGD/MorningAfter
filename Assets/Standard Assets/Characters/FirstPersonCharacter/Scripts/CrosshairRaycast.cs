using UnityEngine;

public class CrosshairRaycast : MonoBehaviour
{
    private static readonly string INTERACTABLE_TAG = "Interactable";
    private static readonly string DOOR_TAG = "Door";

    [SerializeField]
    public float interactionDistance = 3f;
    static public bool canInteract;
    static public GameObject interactingObject;

    void Update()
    {
        canInteract = false;
        RaycastHit hit;
        Debug.DrawRay(transform.position, transform.forward * interactionDistance);

        if (Physics.Raycast(transform.position, transform.forward, out hit, interactionDistance))
        {
            interactingObject = hit.transform.gameObject;
            if(hit.collider.tag == INTERACTABLE_TAG || hit.collider.tag == DOOR_TAG)
            {
                canInteract = true;
            }
        }
    }
}
