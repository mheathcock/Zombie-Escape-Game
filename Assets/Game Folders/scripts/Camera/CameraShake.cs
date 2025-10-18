using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    //variables
    public Transform cameraTransform;
    public float shakeDuration = 0.2f;
    public float shakeMagnitude = 0.5f;
    private Vector3 originalPosition;

    void Start()
    {
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }

        originalPosition = cameraTransform.localPosition;//keep the original position of the camera
    }

    public void Shake()
    {
        StartCoroutine(PerformShake());
    }

/*
Co routine to give camera shake when called
*/
    IEnumerator PerformShake()
    {
        float elapsed = 0.0f;

        while (elapsed < shakeDuration)
        {
            //set x and y positions * the magnitude of the shake
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            cameraTransform.localPosition = originalPosition + new Vector3(x, y, 0);//shake the camera

            elapsed += Time.deltaTime;

            yield return null;
        }

        cameraTransform.localPosition = originalPosition;//set position back to normal
    }
}
