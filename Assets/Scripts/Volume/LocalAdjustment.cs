using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using StarterAssets;
using Unity.VisualScripting;
using UnityEngine.Rendering.HighDefinition;

public class LocalAdjustment : MonoBehaviour
{
    [Header("Reference")]
    public GameObject Player;
    public Volume Volume;
    [SerializeField] private float _normalView;
    [SerializeField] private float _matchView;
    [SerializeField] private float _flashlightView;
    [SerializeField] private float _fogChangeSpeed = 5f;

    private FirstPersonController _control;
    private Fog _fogVolume;
    private bool _isFogChanging = false;
    [SerializeField] private bool _isPassed;

    private void Start()
    {
        _control = Player.GetComponent<FirstPersonController>();
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {            
            if (!_isFogChanging && !_isPassed)
            {
                StartCoroutine(ChangeFogSmooth());
            }
        }
        _isPassed = !_isPassed;
    }

    private IEnumerator ChangeFogSmooth()
    {
        _isFogChanging = true;
        float timeElapsed = 0f;
        float startFogDensity = _control.NormalView;
        if (Volume.profile != null)
        {
            if (Volume.profile.TryGet(out _fogVolume))
            {           
                while (timeElapsed < _fogChangeSpeed)
                {
                    _fogVolume.meanFreePath.value = Mathf.Lerp(startFogDensity, _normalView, timeElapsed / _fogChangeSpeed);
                    timeElapsed += Time.deltaTime;
                    yield return null;
                }                                    
                _fogVolume.meanFreePath.value = _normalView;                
                _control.NormalView = _normalView;
                _control.MatchView = _matchView;
                _control.FlashlightView = _flashlightView;
                _isFogChanging = false;

            }
            else
            {
                Debug.LogWarning("Fog settings not found in the volume profile.");
            }
        }
        else
        {
            Debug.LogWarning("No Volume component found on this GameObject.");
        }
    }
}
