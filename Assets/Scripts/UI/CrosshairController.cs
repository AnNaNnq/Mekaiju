using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairController : MonoBehaviour
{
    [Header("Références")]
    public Image crosshairImage;
    public Sprite defaultCrosshair;
    public Sprite lockOnCrosshair;

    public float rotationDuration = 0.1f;
    public float rotationAngle = 40f;
    public float swayAngle = 30f;
    public float swayDuration = 0.1f;

    public void ChangeCrosshairAfterDelay(bool isLockedOn)
    {
        if (isLockedOn)
        {
            StartCoroutine(_ChangeCrosshairWithDelay());
        }
        else
        {
            StartCoroutine(_ReverseCrosshairWithDelay());
        }
    }

    public void SwayCrosshairOnTargetChange()
    {
        StartCoroutine(_SwayCrosshair());
    }

    private IEnumerator _ChangeCrosshairWithDelay()
    {
        yield return new WaitForSeconds(0.2f);

        float elapsedTime = 0f;
        float startRotation = crosshairImage.rectTransform.localEulerAngles.z;
        float targetRotation = startRotation + rotationAngle;

        while (elapsedTime < rotationDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / rotationDuration;

            float currentRotation = Mathf.LerpAngle(startRotation, targetRotation, progress);
            crosshairImage.rectTransform.localRotation = Quaternion.Euler(0f, 0f, currentRotation);

            yield return null;
        }

        crosshairImage.sprite = lockOnCrosshair;
    }

    private IEnumerator _ReverseCrosshairWithDelay()
    {
        float elapsedTime = 0f;
        float startRotation = crosshairImage.rectTransform.localEulerAngles.z;
        float targetRotation = startRotation - rotationAngle;

        while (elapsedTime < rotationDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / rotationDuration;

            float currentRotation = Mathf.LerpAngle(startRotation, targetRotation, progress);
            crosshairImage.rectTransform.localRotation = Quaternion.Euler(0f, 0f, currentRotation);

            yield return null;
        }

        crosshairImage.sprite = defaultCrosshair;
    }

    private IEnumerator _SwayCrosshair()
    {
        // Rotation à gauche
        float elapsedTime = 0f;
        float startRotation = crosshairImage.rectTransform.localEulerAngles.z;
        float leftTargetRotation = startRotation - swayAngle;

        while (elapsedTime < swayDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / swayDuration;

            float currentRotation = Mathf.LerpAngle(startRotation, leftTargetRotation, progress);
            crosshairImage.rectTransform.localRotation = Quaternion.Euler(0f, 0f, currentRotation);

            yield return null;
        }

        // Retour au centre
        elapsedTime = 0f;
        while (elapsedTime < swayDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / swayDuration;

            float currentRotation = Mathf.LerpAngle(leftTargetRotation, startRotation, progress);
            crosshairImage.rectTransform.localRotation = Quaternion.Euler(0f, 0f, currentRotation);

            yield return null;
        }
    }
}
