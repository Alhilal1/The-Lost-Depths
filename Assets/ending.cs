using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class ending : MonoBehaviour
{
    public GameObject player;
    public GameObject UI;
    public GameObject Objective;
    private VideoPlayer _video;

    private void Start()
    {
        _video = player.GetComponent<VideoPlayer>();        
    }

    private void OnTriggerEnter(Collider other)
    {
        _video.loopPointReached += OnVideoFinished;
        UI.SetActive(false);
        Objective.SetActive(false);
        _video.Play();
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        player.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
     
        SceneManager.LoadScene("Main Menu"); 
    }

    private void OnDestroy()
    {       
        _video.loopPointReached -= OnVideoFinished;
    }
}
