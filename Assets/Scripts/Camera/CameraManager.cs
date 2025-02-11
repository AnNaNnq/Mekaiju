using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineInputAxisController _cinemachineInput;

    public event System.Action<bool> onCameraDisabledChanged;
    private bool _isCameraDisabled;
    public bool IsCameraDisabled
    {
        get => _isCameraDisabled;
        set
        {
            if (_isCameraDisabled != value)
            {
                _isCameraDisabled = value;
                onCameraDisabledChanged?.Invoke(_isCameraDisabled); // D�clencher l'�v�nement
            }
        }
    }

    private void OnEnable()
    {
        onCameraDisabledChanged += HandleCameraInput;
    }
    private void OnDisable()
    {
        onCameraDisabledChanged -= HandleCameraInput;
    }

    private void HandleCameraInput(bool isDisabled)
    {
        // M�thode 1 : D�sactiver CinemachineInputProvider
        if (_cinemachineInput != null)
        {
            _cinemachineInput.enabled = !isDisabled;
        }
    }
}
