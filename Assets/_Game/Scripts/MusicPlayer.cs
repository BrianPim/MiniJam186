using CamLib;
using UnityEngine;

public class MusicPlayer : Singleton<MusicPlayer>
{
    [SerializeField] private AudioSource musicSource;

    public bool DeadMusic;
    
    private void Start()
    {
        
    }

    private float vel;
    private float target = 1f;

    public void SetProgress(float progress)
    {
        if (DeadMusic) return;
        
        target = Mathf.Lerp(0.99f, 1.11f, progress);
    }

    private void Update()
    {
        musicSource.pitch = Mathf.SmoothDamp(musicSource.pitch, target, ref vel, 100f * Time.deltaTime);
    }

    public void SetDead(bool dead)
    {
        DeadMusic = dead;
        if (!dead)
        {
            target = 1;
        }
        else
        {
            target = 0.86f;
        }
    }
}