using CamLib;
using UnityEngine;

public class ParallaxMaster : Singleton<ParallaxMaster>
{
    public Transform JourneyStart;
    public Transform JourneyEnd;
    
    protected override void Awake()
    {
        base.Awake();
        
        Parallax[] objs = FindObjectsByType<Parallax>(FindObjectsSortMode.None);

        foreach (Parallax parallax in objs)
        {
            parallax.SetCamera(transform);
        }
    }

    public void SetProgress(float progress)
    {
        transform.position = Vector3.Lerp(JourneyStart.position, JourneyEnd.position, progress);
    }
}