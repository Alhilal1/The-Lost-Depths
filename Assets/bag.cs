using StarterAssets;
using UnityEngine;

public class bag : MonoBehaviour
{
    public GameObject UIManager;
    public GameObject Player;
    public GameObject mesh;
    public GameObject trigger;
    public GameObject lampu;
    private UIManager _ui;
    private bool fake = false;

    private void Start()
    {
        _ui = UIManager.GetComponent<UIManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!fake)
        {
            mesh.SetActive(false);
            trigger.SetActive(false);
            lampu.GetComponent<Light>().enabled = false;
            fake = true;
            _ui.GaadaWaktu();
            Player.GetComponent<FirstPersonController>().addBattery(0.25f);
        }
    }
}
