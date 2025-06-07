using CamLib;
using UnityEngine;

public class MusicPlayer : Singleton<MusicPlayer>
{
    [SerializeField] private AudioSource musicSource;

    private void Start()
    {
        
    }

    public void SetProgress(float progress)
    {
        float pitch = Mathf.Lerp(0.99f, 1.11f, progress);
        musicSource.pitch = pitch;
    }
}