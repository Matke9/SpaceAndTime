using UnityEngine;
using static Unity.VisualScripting.Member;

public class SongSpeedUp : MonoBehaviour
{
    [SerializeField] private AudioSource audioSrc;
    [SerializeField] private AudioClip songSlow;
    [SerializeField] private AudioClip songMedium;
    [SerializeField] private AudioClip songFast;
    private int songCount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        songCount = 0;
        audioSrc.clip = songSlow;
        audioSrc.Play();
    }

    // Update is called once per frame
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

    public void playSongSlow()
    {
        
        audioSrc.Play();
    }

    public void playSongMedium()
    {

        audioSrc.Play();
    }

    public void playSongFast()
    {
        audioSrc.Play();
    }

}
