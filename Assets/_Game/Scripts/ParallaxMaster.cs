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

    private Vector2 spot;
    private Vector2 vel;
    
    public void SetProgress(float progress)
    {
        spot = Vector3.Lerp(JourneyStart.position, JourneyEnd.position, progress);
    }

    private void Update()
    {
        transform.position = Vector2.SmoothDamp(transform.position, spot, ref vel, 300f * Time.deltaTime);
    }
}