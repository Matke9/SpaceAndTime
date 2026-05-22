using UnityEngine;

public class SongSpeedUp : MonoBehaviour
{
    [SerializeField] private AudioSource audioSrc;
    [SerializeField] private AudioClip songSlow;
    [SerializeField] private AudioClip songMedium;
    [SerializeField] private AudioClip songFast;
    private int songCount;

    void Start()
    {
        songCount = 0;
        audioSrc.clip = songSlow;
        audioSrc.Play();
    }

    void Update()
    {
        if (GameTimeManager.GetTime() < 25 && songCount != 2)
        {
            songCount = 2;
            audioSrc.Stop();
            audioSrc.clip = songFast;
            audioSrc.Play();

        }
        else if (GameTimeManager.GetTime() < 50 && GameTimeManager.GetTime() > 25 && songCount !=1)
        {
            songCount = 1;
            audioSrc.Stop();
            audioSrc.clip = songMedium;
            audioSrc.Play();
        }
        else if (GameTimeManager.GetTime() > 50 && songCount !=0)
        { 
            songCount = 0;
            audioSrc.Stop();
            audioSrc.clip = songSlow;
            audioSrc.Play();
        }
    }

    public void PlaySongSlow()
    {
        audioSrc.Play();
    }

    public void PlaySongMedium()
    {
        audioSrc.Play();
    }

    public void PlaySongFast()
    {
        audioSrc.Play();
    }

}
