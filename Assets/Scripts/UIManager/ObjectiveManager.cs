using NUnit.Framework;
using StarterAssets;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class ObjectiveManager : MonoBehaviour
{
    public GameObject Player;
    public GameObject UI;
    public GameObject Scene;

    private FirstPersonController _control;
    private StarterAssetsInputs _input;
    private UIManager _ui;
    private CutsceneManager _scene;
    public List<GameObject> _objective;
    private List<bool> _passed;
    [SerializeField] private int _index = 0;

    private void Start()
    {
        _control = Player.GetComponent<FirstPersonController>();
        _input = Player.GetComponent<StarterAssetsInputs>();
        _ui = UI.GetComponent<UIManager>();
        _scene = Scene.GetComponent<CutsceneManager>();

        for (int i = 0; i < transform.childCount; i++)
        {
            _objective.Add(transform.GetChild(i).gameObject);
        }

        _passed = new List<bool>(_objective.Count);
        for (int i = 0; i < _objective.Count; i++)
        {
            _passed.Add(false);
        }
    }

    private void Update()
    {
        indexZeroHandling();
    }

    private void indexZeroHandling()
    {
        if (_index < _passed.Count && !_passed[_index])
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                _objective[_index].gameObject.SetActive(false);
                _passed[_index] = true;
                _index++;
            }

            else if (_scene.isTutorial() && _objective.Count > 0)
            {
                _objective[_index].gameObject.SetActive(true);
            }
        
        }
       
    }
}
