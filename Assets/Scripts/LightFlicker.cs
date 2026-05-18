using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    private Light flickerLight;
    public float minIntensity = 0.5f;
    public float maxIntensity = 2f;
    public float flickerSpeed = 0.1f;

    void Start()
    {
        flickerLight = GetComponent<Light>();
    }

    void Update()
    {
        flickerLight.intensity = Mathf.Lerp(
            flickerLight.intensity,
            Random.Range(minIntensity, maxIntensity),
            flickerSpeed
        );
    }
}