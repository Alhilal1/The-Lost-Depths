using System.Collections;
using UnityEngine;

public class TriggerManager : MonoBehaviour
{
    public GameObject AudioManager;
    public int SoundNumberOne;
    public int SoundNumberTwo;
    public int SoundNumberThree;
    public bool canRepeat = false;
    public bool Two;
    public bool Three;

    private AudioManager _audio;
    private bool _isHold = false;   

    private void Start()
    {
        _audio = AudioManager.GetComponent<AudioManager>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!_isHold)
        {
            if (!Two && !Three)
            {
                _isHold = true;
                _audio.PlaySound(SoundNumberOne);
                if (canRepeat)
                {
                    StartCoroutine(DelayEntry(10f));
                }
            }
            else if (Two && !Three)
            {
                _isHold = true;
                _audio.PlaySound(SoundNumberOne, SoundNumberTwo);
                if (canRepeat)
                {
                    StartCoroutine(DelayEntry(20f));
                }
            }
            else if (Three && !Two)
            {
                _isHold = true;
                _audio.PlaySound(SoundNumberOne, SoundNumberTwo, SoundNumberThree);
                if (canRepeat)
                {
                    StartCoroutine(DelayEntry(30f));
                }
            }

            
        }        
    }

    private IEnumerator DelayEntry(float length)
    {
        yield return new WaitForSeconds(length);
        _isHold = false;
    }
}
