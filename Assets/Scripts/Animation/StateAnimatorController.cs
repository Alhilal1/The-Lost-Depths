using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

public class StateAnimatorController : MonoBehaviour
{
    private Animator _animation;
    private StarterAssetsInputs _input;
    private FirstPersonController _player;
    private int _speedHash, _crouchHash, _proneHash, _matchHash;

    void Start()
    {
        _player = GetComponent<FirstPersonController>();
        _animation = GetComponent<Animator>();
        _input = GetComponent<StarterAssetsInputs>();
        _speedHash = Animator.StringToHash("Speed");
        _crouchHash = Animator.StringToHash("Crouch");
        _proneHash = Animator.StringToHash("Prone");
        _matchHash = Animator.StringToHash("Match");
    }

    void Update()
    {
        _animation.SetFloat(_speedHash, _player._speed);
        _animation.SetBool(_crouchHash, _input.crouch);
        _animation.SetBool(_proneHash, _input.prone);
        _animation.SetBool(_matchHash, _input.match);        
    }
}
