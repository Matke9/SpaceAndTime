using UnityEngine;

public class ShootSound : MonoBehaviour
{

    public AudioSource source;
    public AudioClip clip;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            source.PlayOneShot(clip);
        }
    }
}
