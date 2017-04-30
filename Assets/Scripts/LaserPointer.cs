using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPointer : MonoBehaviour
{

	public string Options;

	#region Variables Teleport

	public Transform cameraRigTransform;
	public GameObject teleportReticlePrefab;
	private GameObject reticle;
	private Transform teleportReticleTransform;
	public Transform headTransform;
	public Vector3 teleportReticleOffset;
	public LayerMask teleportMask;
	private bool shouldTeleport;

	#endregion

	#region Variables Laser

	private SteamVR_TrackedObject trackedObj;
	public GameObject laserPrefab;
	private GameObject laser;
	private Transform laserTransform;
	private Vector3 hitPoint;

	#endregion

	private SteamVR_Controller.Device Controller {
		get { return SteamVR_Controller.Input ((int)trackedObj.index); }
	}

	void Awake ()
	{
		trackedObj = GetComponent<SteamVR_TrackedObject> ();
	}

	void Start ()
	{
		laser = Instantiate (laserPrefab);
		laserTransform = laser.transform;
		reticle = Instantiate (teleportReticlePrefab);
		teleportReticleTransform = reticle.transform;
	}

	// Update is called once per frame
	void Update ()
	{
		if (Controller.GetPress (SteamVR_Controller.ButtonMask.Touchpad)) {
			RaycastHit hit;
			if (Physics.Raycast (trackedObj.transform.position, transform.forward, out hit, 100, teleportMask)) {
				hitPoint = hit.point;
				ShowLaser (hit);
				reticle.SetActive (true);
				teleportReticleTransform.position = hitPoint + teleportReticleOffset;
				shouldTeleport = true;
			}
		} else {
			laser.SetActive (false);
			reticle.SetActive (false);
		}

		if (Controller.GetPress (SteamVR_Controller.ButtonMask.Touchpad) && shouldTeleport) {
			switch (Options) {
			case "Teleport":
				Teleport ();
				break;
			case "MoveTo":
				MoveTo ();
				break;
			}
		}
	}

	private void ShowLaser (RaycastHit hit)
	{
		laser.SetActive (true);
		laserTransform.position = Vector3.Lerp (trackedObj.transform.position, hitPoint, 0.5f);
		laserTransform.LookAt (hitPoint);
		laserTransform.localScale = new Vector3 (laserTransform.localScale.x, laserTransform.localScale.y, hit.distance);
	}

	#region Funciones movimiento

	private void Teleport ()
	{
		shouldTeleport = false;
		reticle.SetActive (false);
		Vector3 difference = cameraRigTransform.position - headTransform.position;
		difference.y = 0;
		cameraRigTransform.position = hitPoint + difference;
	}

	private void MoveTo ()
	{
		shouldTeleport = false;
		reticle.SetActive (false);
		Vector3 difference = cameraRigTransform.position - headTransform.position;
		difference.y = 0;
		StartCoroutine (MoveTo (difference));
	}

	private IEnumerator MoveTo (Vector3 difference)
	{
		Vector3 origin = cameraRigTransform.position;
		while (cameraRigTransform.position != hitPoint + difference) {
			cameraRigTransform.position = Vector3.Lerp (origin, hitPoint + difference, Time.deltaTime);
			yield return new WaitForSeconds (0.01f);
		}
	}

	#endregion
}
