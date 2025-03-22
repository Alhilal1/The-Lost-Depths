using System.Collections;
using UnityEngine;

public class CrouchTutorial : MonoBehaviour
{
    public GameObject Tutorial;
    private bool _shown = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!_shown)
        {
            _shown = true;
            Tutorial.SetActive(true);
            StartCoroutine(DelayHidden());
        }
    }

    private IEnumerator DelayHidden()
    {
        yield return new WaitForSeconds(5f);
        Tutorial.SetActive(false);
    }
}
