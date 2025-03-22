using System.Collections;
using StarterAssets;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Reference")]
    [Tooltip("Reference to Player Controller")]
    public GameObject Player;
    [Tooltip("UI Screen -> Tools")]
    public GameObject Tools;
    [Tooltip("UI Screen -> Maps")]
    public GameObject Maps;
    [Tooltip("UI Screen -> SanityBar")]
    public GameObject SanityBar;
    [Tooltip("UI Screen -> PlayerState")]
    public GameObject PlayerState;
    [Tooltip("Used for Scene Management Script")]
    public GameObject SceneManagement;

    [Header("Others")]
    public bool backpackFound = false;
    public bool flashlightFound = false;
    public bool matchFound = false;
    public bool sanityEnabled = false;
    public bool mapsEnabled = false;
    public GameObject Objective;
    public GameObject Collider;
    public GameObject Trigger;

    private StarterAssetsInputs _input;
    private CutsceneManager _scene;
    private Image _lighter;
    private Image _flashlight;
    private Image _bag;
    private FirstPersonController _control;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _input = Player.GetComponent<StarterAssetsInputs>();
        _scene = SceneManagement.GetComponent<CutsceneManager>();
        _lighter = Tools.transform.GetChild(0).transform.GetChild(0).GetComponent<Image>();
        _flashlight = Tools.transform.GetChild(1).transform.GetChild(0).GetComponent<Image>();
        _bag = Tools.transform.GetChild(2).transform.GetChild(0).GetComponent<Image>();
        _control = Player.GetComponent<FirstPersonController>();
    }

    // Update is called once per frame
    private void Update()
    {
        inputSementara();   
        Tools.SetActive(!_scene.isCutscenePlaying);
        Tools.transform.GetChild(0).gameObject.SetActive(matchFound);
        Tools.transform.GetChild(1).gameObject.SetActive(flashlightFound);
        Tools.transform.GetChild(2).gameObject.SetActive(backpackFound);
        PlayerState.SetActive(sanityEnabled);
        SanityBar.SetActive(sanityEnabled);
        Maps.SetActive(mapsEnabled);

        if (_scene.isCutscenePlaying)
        {
            _input.match = _input.flashlight = _input.match = false;
        }
        if (matchFound)
        {
            SetAlpha(_lighter, _input.match);
        }
        if (flashlightFound)
        {
            SetAlpha(_flashlight, _input.flashlight && _control.thereIsBattery());
        }
        
    }

    private void SetAlpha(Image image, bool condition)
    {
        Color newColor = image.color;
        newColor.a = condition ? 1f : 50f / 255f;
        image.color = newColor;
    }

    void inputSementara()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            backpackFound = true;   
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            flashlightFound = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            matchFound = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            sanityEnabled = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            mapsEnabled = true;
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            _control.addBattery(0.25f);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            _control.FoundFirstBattery();
            Collider.transform.GetChild(0).gameObject.SetActive(false);
            Trigger.transform.GetChild(0).gameObject.SetActive(true);
            Objective.transform.GetChild(1).gameObject.SetActive(true);
        }
    }

}
