using UnityEngine;

public class SineWaveLocalPosition : MonoBehaviour
{
    public Vector2 Amplitude;
    public Vector2 Oscillation;
    public Vector2 OscillationOffset;

    public float Time;

    private void LateUpdate()
    {
        Time += UnityEngine.Time.deltaTime;
            
        float x = Mathf.Sin(Time * Oscillation.x + OscillationOffset.x) * Amplitude.x;
        float y = Mathf.Sin(Time * Oscillation.y + OscillationOffset.y) * Amplitude.y;
        transform.localPosition = new Vector2(x, y);
    }

    public void DoReset()
    {
        Time = 0;
    }
}