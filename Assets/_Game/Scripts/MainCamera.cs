using CamLib;
using System.Collections;
using UnityEngine;

public class MainCamera : Singleton<MainCamera>
{
    public Camera Camera;
    public Vector2 MousePosition => Camera.ScreenToWorldPoint(Input.mousePosition);

    private Coroutine ActiveShakeRoutine;

    private void Start()
    {
        
    }

    public void ShakeCamera(float intensity = 0.15f, float duration = 0.5f)
    {
        if (intensity <= 0) return;

        if (ActiveShakeRoutine != null)
        {
            CancelShake();
        }

        ActiveShakeRoutine = StartCoroutine(ShakeCameraRoutine(intensity, duration));
    }

    public void CancelShake()
    {
        Camera.transform.localPosition = new Vector3(0, 0, Camera.transform.localPosition.z);

        if (ActiveShakeRoutine != null)
        {
            StopCoroutine(ActiveShakeRoutine);
        }

        ActiveShakeRoutine = null;
    }

    private IEnumerator ShakeCameraRoutine(float intensity, float duration)
    {
        WaitForFixedUpdate wait = new WaitForFixedUpdate();

        for (float time = duration; time > 0f; time -= Time.fixedDeltaTime)
        {
            float ratio = (time / duration);

            var random = Random.insideUnitCircle * (ratio * intensity);
            Camera.transform.localPosition = new Vector3(random.x, random.y, Camera.transform.localPosition.z);
            yield return wait;
        }

        CancelShake();
    }
}