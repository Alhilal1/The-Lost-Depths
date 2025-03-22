using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class AudioManager : MonoBehaviour
{
    [Header("Reference")]
    public AudioSource AudioSource;    

    [Header("Audio Reference")]
    public List<AudioClip> AudioClipList;

    private AudioSource _sibling;

    private void Start()
    {
        _sibling = AudioSource.transform.parent.GetChild(4).GetComponent<AudioSource>();
    }

    public void PlaySound(int soundID)
    {
        if (soundID < AudioClipList.Count && soundID >= 0)
        {
            AudioSource.Stop();
            _sibling.Stop();
            AudioSource.PlayOneShot(AudioClipList[soundID]);
        }
    }

    public void PlaySound(int soundA, int soundB)
    {
        if (soundA < AudioClipList.Count && soundA >= 0 && soundB < AudioClipList.Count && soundB >= 0)
        {
            StartCoroutine(PlaySequentallyTwo(soundA, soundB));
        }
    }

    private IEnumerator<WaitForSeconds> PlaySequentallyTwo(int soundA, int soundB)
    {
        _sibling.Stop();
        AudioSource.Stop();
        AudioSource.PlayOneShot(AudioClipList[soundA]);
        yield return new WaitForSeconds(AudioClipList[soundA].length);

        AudioSource.PlayOneShot(AudioClipList[soundB]);
        yield return new WaitForSeconds(AudioClipList[soundB].length);

    }

    public void PlaySound(int soundA, int soundB, int soundC)
    {
        if (soundA < AudioClipList.Count && soundA >= 0 && soundB < AudioClipList.Count && soundB >= 0 && soundC < AudioClipList.Count && soundC >= 0)
        {
            StartCoroutine(PlaySequentallyThree(soundA, soundB, soundC));
        }
    }

    private IEnumerator<WaitForSeconds> PlaySequentallyThree(int soundA, int soundB, int soundC)
    {
        _sibling.Stop();
        AudioSource.Stop();
        AudioSource.PlayOneShot(AudioClipList[soundA]);
        yield return new WaitForSeconds(AudioClipList[soundA].length);

        AudioSource.PlayOneShot(AudioClipList[soundB]);
        yield return new WaitForSeconds(AudioClipList[soundB].length);

        AudioSource.PlayOneShot(AudioClipList[soundC]);
        yield return new WaitForSeconds(AudioClipList[soundC].length);
    }


}
