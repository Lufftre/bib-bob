 using UnityEngine;
 
 public class CameraFollow : MonoBehaviour
 {
     public Transform playerTransform;
     public int depth = -20;
 
	public float smoothSpeed;
	public Vector3 offset;

	void FixedUpdate ()
	{
		if (playerTransform == null) return;

		Vector3 desiredPosition = playerTransform.position + offset;
		Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
		transform.position = smoothedPosition;

		transform.LookAt(playerTransform);
	}
 
     public void setTarget(Transform target)
     {
         playerTransform = target;
     }
 }