﻿using UnityEngine;

public class ControllerGrabObject : MonoBehaviour
{

	private SteamVR_TrackedObject trackedObj;
	private GameObject collidingObject;
	private GameObject objectInHand;

	private SteamVR_Controller.Device Controller {
		get{ return SteamVR_Controller.Input ((int)trackedObj.index); }
	}

	void Awake ()
	{
		trackedObj = GetComponent<SteamVR_TrackedObject> ();
	}

	private void SetCollidingObject (Collider col)
	{
		if (collidingObject || !col.GetComponent<Rigidbody> ()) {
			return;
		}
		collidingObject = col.gameObject;
	}

	public void OnTriggerEnter (Collider col)
	{
		SetCollidingObject (col);
	}

	public void OnTriggerStay (Collider col)
	{
		SetCollidingObject (col);
	}

	public void OnTriggerExit (Collider col)
	{
		if (!collidingObject) {
			return;
		}

		collidingObject = null;
	}

	private void GrabObject ()
	{
		objectInHand = collidingObject;
		collidingObject = null;

		FixedJoint joint = AddFixedJoint ();
		joint.connectedBody = objectInHand.GetComponent<Rigidbody> ();
	}

	private FixedJoint AddFixedJoint ()
	{
		FixedJoint joint = gameObject.AddComponent<FixedJoint> ();
		joint.breakForce = 20000;
		joint.breakTorque = 20000;
		return joint;
	}

	private void ReleaseObject ()
	{
		if (GetComponent<FixedJoint> ()) {
			GetComponent<FixedJoint> ().connectedBody = null;
			Destroy (GetComponent<FixedJoint> ());
			objectInHand.GetComponent<Rigidbody> ().velocity = Controller.velocity;
			objectInHand.GetComponent<Rigidbody> ().angularVelocity = Controller.angularVelocity;
		}
		objectInHand = null;
	}

	// Update is called once per frame
	void Update ()
	{
		if (Controller.GetHairTriggerDown ()) {
			if (collidingObject) {
				GrabObject ();
			}
		}

		if (Controller.GetHairTriggerUp ()) {
			if (objectInHand) {
				ReleaseObject ();
			}
		}
	}
}
