using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace COOLFRIENDS {

	[RequireComponent(typeof(Rigidbody))]
	public class Creature : MonoBehaviour {

		static RaycastHit[] results = new RaycastHit[32];

		[SerializeField] float moveSpeed = 4f;
		[SerializeField] Transform moveDummy = null;
		public Transform MoveDummy => moveDummy;
		[SerializeField] CapsuleCollider capsule = null;
		[SerializeField] float legHeight = 0.35f;
		[SerializeField] float legDepth = 0.85f;
		[SerializeField] LayerMask layerFloor = new LayerMask();
		[SerializeField] float floorMaxSlope = 60f;
		[SerializeField] float jumpForce = 10f;
		//
		public Rigidbody Rbody { get; private set; }
		public bool OnFloor { get; private set; }
		public Vector2 MoveInput { get; set; }
		//
		float radius;
		float height;
		bool wasOnFloor;
		float jumpTime;

		//

		void Awake() {
			Rbody = GetComponent<Rigidbody>();
			if (moveDummy == null) { moveDummy = transform; }
			radius = capsule.radius;
			height = capsule.height;
		}

		void FixedUpdate() {
			OnFloor = false;

			// floor detection
			if (jumpTime < Time.time) {
				var curVelocity = Rbody.velocity;

				var depth = legHeight + legDepth;
				var bottomSphere = capsule.transform.TransformPoint(capsule.center - new Vector3(0f, height * 0.5f - radius, 0f));
				var hitCount = Physics.SphereCastNonAlloc(bottomSphere, radius * 0.99f, Vector3.down, results, depth, layerFloor.value, QueryTriggerInteraction.Ignore);
				if (hitCount > 0) {
					var dist = depth;
					for (int i = 0; i < hitCount; ++i) {
						if (results[i].distance < dist) {
							var dir = results[i].point + transform.up * 0.002f - bottomSphere;
							if (results[i].collider.Raycast(new Ray(bottomSphere, dir), out RaycastHit hit, dir.magnitude + 0.5f)) {
								if (Vector3.Angle(hit.normal, Vector3.up) < floorMaxSlope) {
									dist = results[i].distance;
								}
							}
						}
					}
					var diff = dist - legHeight;
					if (diff < 0f) {
						OnFloor = true;
					}
					if (diff < 0f) {
						Rbody.MovePosition(Rbody.position + Vector3.down * diff * 0.5f);
					}
					else if (diff > 0f && diff < legDepth && wasOnFloor) {
						Rbody.MovePosition(Rbody.position + Vector3.down * diff * 0.8f);
						OnFloor = true;
					}
				}

				// movement
				if (OnFloor) {
					var moveVelocityXZ = (moveDummy.forward * MoveInput.y + moveDummy.right * MoveInput.x) * moveSpeed;
					curVelocity.x = moveVelocityXZ.x;
					curVelocity.y = 0f;
					curVelocity.z = moveVelocityXZ.z;
				}

				Rbody.velocity = curVelocity; // TODO make force based
			}
			Rbody.angularVelocity = Vector3.zero;
			wasOnFloor = OnFloor;
		}

		//

		public void Jump() {
			if (!OnFloor || jumpTime > Time.time) { return; }
			Rbody.AddRelativeForce(new Vector3(0f, jumpForce, 0f), ForceMode.Impulse);
			jumpTime = Time.time + 0.1f;
		}
	}

}