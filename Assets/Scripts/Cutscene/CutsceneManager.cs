using System.Collections;
using StarterAssets;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class CutsceneManager : MonoBehaviour
{
    public PlayableDirector playableDirector;  // Reference to the Playable Director (Timeline)
    public GameObject player;  // Reference to the player game object (the character)
    public GameObject firstPersonCamera;  // Reference to the first-person camera (same for cutscene and gameplay)
    public AudioClip AudioClip;
    public AudioClip InterfaceSound;
    public GameObject Tutorial;
    public GameObject Objective;

    public bool isCutscenePlaying = false;
    [SerializeField] private bool _isShowing = false;
    private StarterAssetsInputs _input;

    public GameObject UIManager;
    private UIManager _ui;

    private void Start()
    {
        // Automatically start the cutscene (or you can trigger this based on your game logic)
        _input = player.GetComponent<StarterAssetsInputs>();
        _ui = UIManager.GetComponent<UIManager>();
        PlayCutscene();

        // Subscribe to the 'stopped' event to detect when the timeline finishes
        playableDirector.stopped += OnCutsceneEnd;
    }

    // Call this method to start the cutscene
    public void PlayCutscene()
    {
        isCutscenePlaying = true;
        playableDirector.Play();  // Play the timeline animation (e.g., waking up, etc.)

        // Disable player controls and input during the cutscene
        player.GetComponent<FirstPersonController>().canMove = false;
    }

    // This method is called when the cutscene ends (via the 'stopped' event)
    public void OnCutsceneEnd(PlayableDirector director)
    {
        isCutscenePlaying = false;

        // Re-enable player controls and input after the cutscene
        player.GetComponent<FirstPersonController>().canMove = true;

        // Optionally, reset the player's input if needed
        _input.move = _input.look = new Vector2(0, 0);
        _input.jump = _input.sprint = _input.crouch = _input.prone = _input.match = _input.flashlight = false;

        // Start audio sequence after cutscene ends
        StartCoroutine(AudioSequentally());
    }

    private void OnDestroy()
    {
        // Unsubscribe from the 'stopped' event when this object is destroyed
        playableDirector.stopped -= OnCutsceneEnd;
    }

    private void ShowTutorial()
    {
        Tutorial.SetActive(true);
        _ui.flashlightFound = true;
        if (!_isShowing)
        {
            _isShowing = true;
            StartCoroutine(Hidden());
        }
    }

    private IEnumerator Hidden()
    {
        yield return new WaitForSeconds(5f);
        Tutorial.SetActive(false);
    }

    private IEnumerator AudioSequentally()
    {
        AudioSource audioSource = player.transform.GetChild(3).gameObject.GetComponent<AudioSource>();
        audioSource.volume = 0.4f;

        // Play the first sound
        audioSource.PlayOneShot(AudioClip);
        yield return new WaitForSeconds(AudioClip.length);

        // Play the second sound
        audioSource.volume = 1f;
        audioSource.PlayOneShot(InterfaceSound);

        // Show the tutorial
        ShowTutorial();
    }

    public bool isTutorial()
    {
        return _isShowing;
    }
}
