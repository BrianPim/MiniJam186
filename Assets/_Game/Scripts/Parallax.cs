using JetBrains.Annotations;
using UnityEngine;

public sealed class Parallax : MonoBehaviour
{
	public bool useParallax = true;
        
	[Tooltip("Whether to use FixedUpdate; otherwise LateUpdate")]
	public bool useFixedUpdate;
	public Vector2 parallaxFactor;
	public Vector2 halfLevelSize;
	public bool scaled;
        
	private Transform _cameraTransform;
	private Vector3 _prevPos;
	private bool _prevUseParallax;
	private Vector3 _delta;

	private void Start()
	{
		//If already cached from external code, don't fight with it
		if (_cameraTransform)
		{
			return;
		}
	        
		Camera cam = Camera.main;
		if (cam)
		{
			SetCamera(cam.transform);
			Center();
		}
	}
        
	private void Center()
	{
		Vector2 scaledOffset = halfLevelSize;
		if (scaled)
		{
			scaledOffset *= (Vector2.one - parallaxFactor);
		}
		_prevPos = (Vector2)transform.position + scaledOffset;
		
		_prevUseParallax = true;
	}

	/// <summary>
	/// Custom-controlled set.
	/// </summary>
	[PublicAPI]
	public void SetCamera(Transform cam)
	{
		_cameraTransform = cam;
	}

	private void FixedUpdate()
	{
		if (useFixedUpdate)
		{
			UpdateParallax();
		}
	}
	private void LateUpdate()
	{
		if (!useFixedUpdate)
		{
			UpdateParallax();
		}
	}

	private void UpdateParallax()
	{
		if (!_cameraTransform)
		{
			return;
		}

		if (parallaxFactor == Vector2.zero)
		{
			return;
		}

		Vector3 cameraPos = _cameraTransform.position;
	        
		//only realize a difference if the parallax is enabled
		if (useParallax && !_prevUseParallax)
		{
			_prevPos = cameraPos;
		}
		_prevUseParallax = useParallax;

		//Delta of the camera from last update. If there was movement, move this object with the camera but of a certain multiplier. 
		_delta = cameraPos - _prevPos;
		transform.position += Vector3.Scale(_delta, parallaxFactor);

		_prevPos = cameraPos;
	}
}