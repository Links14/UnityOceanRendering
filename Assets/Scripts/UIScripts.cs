using UnityEngine;
using UnityEngine.UI;

public class UIScripts : MonoBehaviour
{
    [Header("Signals")]
    [SerializeField]
    private Signal trigger_RegenerateWaves;

    [Space]
    [Header("Buttons")]
    // Buttons
    [SerializeField]
    private Button RegenerateWavesButton;

    private void OnEnable()
    {
        if (RegenerateWavesButton == null)
            throw new MissingReferenceException(
                $"{nameof(RegenerateWavesButton)} is not assigned on {name}.");
        if (trigger_RegenerateWaves == null)
            throw new MissingReferenceException(
                $"{nameof(trigger_RegenerateWaves)} is not assigned on {name}.");

        RegenerateWavesButton.onClick.AddListener(RegenerateWavesOnClick);
    }

    private void OnDisable()
    {
        
        RegenerateWavesButton.onClick.RemoveListener(RegenerateWavesOnClick);
    }

    private void RegenerateWavesOnClick()
    {
        if (trigger_RegenerateWaves != null) trigger_RegenerateWaves.Post();
    }
}
