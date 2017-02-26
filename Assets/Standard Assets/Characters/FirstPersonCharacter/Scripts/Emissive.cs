using UnityEngine;

public class Emissive : MonoBehaviour
{
    [SerializeField]
    private float emissiveTimePeriod = 1.0f;
    [SerializeField]
    private Color baseColor = Color.yellow;
    [SerializeField]
    private float intensityFloor = 0.0f;
    [SerializeField]
    private float intensityCeiling = 1.0f;
    [SerializeField]
    private Renderer[] emissiveRenderers;

    private float intensityRange;
    private float currentIntensityFloor;
    private bool permanent = false;

    // Use this for initialization
    void Start()
    {
        emissiveTimePeriod /= (intensityCeiling - intensityFloor);
        intensityRange = 0;
    }

    // Update is called once per frame
    void Update()
    {
        float emission = intensityFloor + Mathf.PingPong(Time.time, intensityRange * emissiveTimePeriod);
        emission /= emissiveTimePeriod;
        emission = emission * emission * emission * (emission * (6 * emission - 15) + 10);
        Color finalColor = baseColor * Mathf.LinearToGammaSpace(emission);

        foreach (Renderer emissiveRenderer in emissiveRenderers)
        {
            foreach (Material mat in emissiveRenderer.materials)
            {
                mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
                mat.SetColor("_EmissionColor", finalColor);
            }
        }

        // set intensity range back to 0. Only constant look will keep it at a positive value
        if (!permanent)
        {
            intensityRange = 0;
        }        
    }

    public void lookAt(bool permanent = false)
    {
        if (permanent)
        {
            this.permanent = true;
        }        
        intensityRange = intensityCeiling - intensityFloor;
    }
}
