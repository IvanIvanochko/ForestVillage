using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Simple UI Health Bar, sets the Image fillAmount based on the linked HealthSystem
/// Check the Demo scene for a usage example
/// </summary>
public class HealthBarUI : MonoBehaviour
{

    [Tooltip("Optional; Either assign a reference in the Editor (that implements IGetHealthSystem) or manually call SetHealthSystem()")]
    [SerializeField] private GameObject getHealthSystemGameObject;

    [Tooltip("Image to show the Health Bar, should be set as Fill, the script modifies fillAmount")]
    [SerializeField] private Slider slider;

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float duration;
    [SerializeField][Range(0f, 1f)] private float endBrightness;

    private Coroutine brightnessCoroutine;

    private HealthSystem healthSystem;


    private void Start()
    {
        if (HealthSystem.TryGetHealthSystem(getHealthSystemGameObject, out HealthSystem healthSystem))
        {
            SetHealthSystem(healthSystem);

            this.healthSystem.OnHealthChanged += BrightnessUI;
            canvasGroup.alpha = endBrightness;
        }
    }

    private void BrightnessUI(object sender, System.EventArgs e)
    {
        if(brightnessCoroutine != null)
        {
            StopCoroutine(brightnessCoroutine);
        }

        StartCoroutine(DecreaseBrightness());
    }

    IEnumerator DecreaseBrightness()
    {
        float initialIntensity = 1;
        float elapsedTime = 0f;

        while (elapsedTime < duration && canvasGroup.alpha >= endBrightness)
        {
            // Gradually decrease brightness from 1 to 0
            canvasGroup.alpha = Mathf.Lerp(initialIntensity, 0f, elapsedTime / duration);

            // Increase elapsed time
            elapsedTime += Time.deltaTime;

            // Wait until the next frame
            yield return null;
        }

        // Ensure the light is fully turned off at the end
        canvasGroup.alpha = endBrightness;
    }

    /// <summary>
    /// Set the Health System for this Health Bar
    /// </summary>
    public void SetHealthSystem(HealthSystem healthSystem)
    {
        if (this.healthSystem != null)
        {
            this.healthSystem.OnHealthChanged -= HealthSystem_OnHealthChanged;
        }
        this.healthSystem = healthSystem;

        UpdateHealthBar();

        healthSystem.OnHealthChanged += HealthSystem_OnHealthChanged;
    }

    /// <summary>
    /// Event fired from the Health System when Health Amount changes, update Health Bar
    /// </summary>
    private void HealthSystem_OnHealthChanged(object sender, System.EventArgs e)
    {
        UpdateHealthBar();
    }

    /// <summary>
    /// Update Health Bar using the Image fillAmount based on the current Health Amount
    /// </summary>
    private void UpdateHealthBar()
    {
        slider.value = healthSystem.GetHealthNormalized();
    }

    /// <summary>
    /// Clean up events when this Game Object is destroyed
    /// </summary>
    private void OnDestroy()
    {
        healthSystem.OnHealthChanged -= HealthSystem_OnHealthChanged;
    }

}
