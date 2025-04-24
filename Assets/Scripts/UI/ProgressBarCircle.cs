using UnityEngine;
using UnityEngine.UI;

public class ProgressBarCircle : MonoBehaviour
{
    [SerializeField] Slider _slider;

    public void SetSliderValue(float value)
    {
        _slider.value = Mathf.Clamp01(value);
    }
}
