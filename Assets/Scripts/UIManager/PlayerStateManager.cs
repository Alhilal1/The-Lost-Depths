using System.Collections;
using StarterAssets;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerStateManager : MonoBehaviour
{
    [Header("Player Reference")]
    public GameObject Player;

    private StarterAssetsInputs _input;
    private GameObject _standing;
    private GameObject _crouch;
    private GameObject _prone;
    private bool _isStanding = false;
    private bool _newStand = false;


    private void Start()
    {
        _input = Player.GetComponent<StarterAssetsInputs>();
        _standing = transform.GetChild(0).gameObject;
        _crouch = transform.GetChild(1).gameObject;
        _prone = transform.GetChild(2).gameObject;
    }

    // Update is called once per frame
    private void Update()
    {
        if (_input.crouch)
        {
            SetAppear(false, true, false);
            _newStand = true;
        }
        else if (_input.prone)
        {
            SetAppear(false, false, true);
            _newStand = true;
        }
        else if (!_input.crouch && !_input.prone && _newStand)
        {
            if (!_isStanding)
            {
                SetAppear(true, false, false);
                _isStanding = true;
                _newStand = false;
                StartCoroutine(StandDissapear());
            }
        }
    }

    private IEnumerator StandDissapear()
    {
        yield return new WaitForSeconds(3f);

        _standing.SetActive(false);
        _isStanding = false;
    }

    private void SetAppear(bool standing, bool crouch, bool prone)
    {
        _standing.SetActive(standing);
        _crouch.SetActive(crouch);
        _prone.SetActive(prone);
    }

}
