using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace COOLFRIENDS {

	[RequireComponent(typeof(Rigidbody))]
	public class Creature : MonoBehaviour {

		[System.Serializable]
		public struct State {
			public float heightFactor; //= 1f;
			public float moveSpeed; //= 4f;
			public float legHeight; // = 0.3f;
			public float legDepth; // = 0.5f;
			public float floorMaxSlope; // = 60f;
			public float jumpForce; // = 5f;
			//
			public void MoveTowards(State other, float delta) {
				heightFactor = Mathf.MoveTowards(heightFactor, other.heightFactor, 2f * delta);
				moveSpeed = Mathf.MoveTowards(moveSpeed, other.moveSpeed, 5f * delta);
				legHeight = Mathf.MoveTowards(legHeight, other.legHeight, 1f * delta);
				legDepth = Mathf.MoveTowards(legDepth, other.legDepth, 1f * delta);
				floorMaxSlope = Mathf.MoveTowards(floorMaxSlope, other.floorMaxSlope, 180f * delta);
				jumpForce = Mathf.MoveTowards(jumpForce, other.jumpForce, 5f * delta);
			}
		}

		static RaycastHit[] results = new RaycastHit[32];

		[SerializeField] Transform moveDummy = null;
		public Transform MoveDummy => moveDummy;
		[SerializeField] CapsuleCollider capsule = null;
		[SerializeField] LayerMask layerFloor = new LayerMask();
		[SerializeField] State[] states = new[] { new State() };
		//[SerializeField] float moveSpeed = 4f;
		//[SerializeField] float legHeight = 0.35f;
		//[SerializeField] float legDepth = 0.85f;
		//[SerializeField] float floorMaxSlope = 60f;
		//[SerializeField] float jumpForce = 10f;
		//
		public Rigidbody Rbody { get; private set; }
		public bool OnFloor { get; private set; }
		public Vector2 MoveInput { get; set; }
		public int TargetStateIdx { get; set; }
		//
		State curState;
		float capsRadius, capsStdHeight;
		bool wasOnFloor;
		float jumpTime;

		//

		void Awake() {
			Rbody = GetComponent<Rigidbody>();
			if (moveDummy == null) { moveDummy = transform; }
			capsRadius = capsule.radius;
			capsStdHeight = capsule.height;
			curState = states[0];
		}

		void Update() {
			curState.MoveTowards(states[TargetStateIdx], Time.deltaTime * 2f);
		}
		void FixedUpdate() {
			OnFloor = false;

			var height = capsule.height = curState.heightFactor * capsStdHeight;

			// floor detection
			if (jumpTime < Time.time) {
				var curVelocity = Rbody.velocity;

				var depth = curState.legHeight + curState.legDepth;
				var bottomSphere = capsule.transform.TransformPoint(capsule.center - new Vector3(0f, height * 0.5f - capsRadius, 0f));
				var hitCount = Physics.SphereCastNonAlloc(bottomSphere, capsRadius * 0.99f, Vector3.down, results, depth, layerFloor.value, QueryTriggerInteraction.Ignore);
				if (hitCount > 0) {
					var dist = depth;
					for (int i = 0; i < hitCount; ++i) {
						if (results[i].distance < dist) {
							var dir = results[i].point + transform.up * 0.002f - bottomSphere;
							if (results[i].collider.Raycast(new Ray(bottomSphere, dir), out RaycastHit hit, dir.magnitude + 0.5f)) {
								if (Vector3.Angle(hit.normal, Vector3.up) < curState.floorMaxSlope) {
									dist = results[i].distance;
								}
							}
						}
					}
					var diff = dist - curState.legHeight;
					if (diff < 0f) {
						OnFloor = true;
					}
					if (diff < 0f) {
						Rbody.MovePosition(Rbody.position + Vector3.down * diff * 0.5f);
					}
					else if (diff > 0f && diff < curState.legDepth && wasOnFloor) {
						Rbody.MovePosition(Rbody.position + Vector3.down * diff * 0.8f);
						OnFloor = true;
					}
				}

				// movement
				if (OnFloor) {
					var moveVelocityXZ = (moveDummy.forward * MoveInput.y + moveDummy.right * MoveInput.x) * curState.moveSpeed;
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
			Rbody.AddRelativeForce(new Vector3(0f, curState.jumpForce, 0f), ForceMode.Impulse);
			jumpTime = Time.time + 0.1f;
		}
	}

}