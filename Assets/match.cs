using UnityEngine;

public class match : MonoBehaviour
{
    public GameObject UIManager;
    private UIManager _ui;
    public GameObject mesh;
    public GameObject lampu;
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
            lampu.GetComponent<Light>().enabled = false;
            fake = true;
            _ui.sigma();
        }
    }
}
