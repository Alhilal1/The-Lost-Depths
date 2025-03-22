using UnityEngine;
using UnityEngine.Video;

public class PlayerManager : MonoBehaviour
{
    public GameObject player;
    private VideoPlayer _video;

    private void Awake()
    {
        _video = GetComponent<VideoPlayer>();

        _video.loopPointReached += OnVideoFinished;

        _video.Play();
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        player.SetActive(false);  // Disables the player GameObject (which is assumed to have the VideoPlayer)
    }

    private void OnDestroy()
    {
        // Remove the listener when the object is destroyed to prevent memory leaks
        _video.loopPointReached -= OnVideoFinished;
    }
}
