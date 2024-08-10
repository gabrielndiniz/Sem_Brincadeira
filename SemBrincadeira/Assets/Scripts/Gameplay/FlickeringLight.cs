using System.Collections;
using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
    public Light PickupLight;
    public float MinIntensity = 0f;
    public float MaxIntensity = 2f;
    public float FlickerSpeed = 0.1f;

    private void Start()
    {
        if (PickupLight == null)
        {
            PickupLight = GetComponent<Light>();
        }
        StartCoroutine(FlickerLight());
    }

    private IEnumerator FlickerLight()
    {
        while (true)
        {
            PickupLight.intensity = Random.Range(MinIntensity, MaxIntensity);
            yield return new WaitForSeconds(Random.Range(FlickerSpeed, FlickerSpeed * 2));
        }
    }
}