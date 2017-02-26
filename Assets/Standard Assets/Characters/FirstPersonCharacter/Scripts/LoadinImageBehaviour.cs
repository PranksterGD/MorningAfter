using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LoadinImageBehaviour : MonoBehaviour
{
    private Image image;
    public float FadeInTime;
	// Use this for initialization
	void Start ()
    {
        image = GetComponent<Image>();

	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(FadeOutImage());
        }        
	}

    IEnumerator FadeOutImage()
    {
        while (image.canvasRenderer.GetAlpha() < 0.05f)
        {
            image.CrossFadeAlpha(0, FadeInTime, true);
            yield return null;
        }
        this.enabled = false;
    }
}
