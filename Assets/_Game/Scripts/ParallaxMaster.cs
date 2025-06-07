using UnityEngine;

public class ParallaxMaster : MonoBehaviour
{
    private void Awake()
    {
        Parallax[] objs = FindObjectsByType<Parallax>(FindObjectsSortMode.None);

        foreach (Parallax parallax in objs)
        {
            parallax.SetCamera(transform);
        }
    }
}