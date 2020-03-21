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
		[SerializeField] Vector3 maxVelocity = new Vector3(10f, 10f, 10f);
		[SerializeField] float frictionInAir = 0.01f;
		[SerializeField] float frictionOnFloor = 0.95f;
		//
		State curState;
		public State CurState => curState;
		public Rigidbody Rbody { get; private set; }
		public bool OnFloor { get; private set; }
		public Vector2 MoveInput { get; set; }
		public int TargetStateIdx { get; set; }
		public float MoveSpeedFactor { get; set; } = 1f;
		//
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

				var depth = capsRadius + curState.legHeight + curState.legDepth;
				var bottomSphere = capsule.transform.TransformPoint(capsule.center - new Vector3(0f, height * 0.5f - capsRadius, 0f));
				var hitCount = Physics.SphereCastNonAlloc(bottomSphere, capsRadius * 0.99f, Vector3.down, results, depth, layerFloor.value, QueryTriggerInteraction.Ignore);
				if (hitCount > 0) {
					var dist = 1000f;
					for (int i = 0; i < hitCount; ++i) {
						var res = results[i];
						if (res.rigidbody != null && res.rigidbody.angularVelocity.magnitude > 1f) {
							// tumbling objects (e.g. crates) cannot not be used as platforms
							continue;
						}
						var y = Mathf.Abs(res.point.y - bottomSphere.y);
						if (y < dist) {
							var dir = res.point + transform.up * 0.002f - bottomSphere;
							if (res.collider.Raycast(new Ray(bottomSphere, dir), out RaycastHit hit, dir.magnitude + 0.5f)) {
								if (Vector3.Angle(hit.normal, Vector3.up) < curState.floorMaxSlope) {
									dist = y;
								}
							}
						}
					}
					var diff = dist - (capsRadius + curState.legHeight);
					if (diff < 0f) {
						OnFloor = true;
						Rbody.MovePosition(Rbody.position + Vector3.down * diff * 0.35f);
					}
					else if (diff > 0f && diff < curState.legDepth && wasOnFloor) {
						OnFloor = true;
						Rbody.MovePosition(Rbody.position + Vector3.down * diff * 0.50f);
					}
				}

				// movement
				if (MoveInput.sqrMagnitude > 1f) { MoveInput = MoveInput.normalized; }
				var moveVelocityXZ = (moveDummy.forward * MoveInput.y + moveDummy.right * MoveInput.x) * curState.moveSpeed;
				if (OnFloor) {
					Rbody.velocity *= (1f - frictionOnFloor);
					Rbody.AddForce(moveVelocityXZ * MoveSpeedFactor * 25f, ForceMode.Force);
					curVelocity = Rbody.velocity;
					curVelocity.y = 0f;
				}
				else {
					curVelocity.x = curVelocity.x * (1f - frictionInAir) + moveVelocityXZ.x * frictionInAir;
					curVelocity.z = curVelocity.z * (1f - frictionInAir) + moveVelocityXZ.z * frictionInAir;
				}

				Rbody.velocity = new Vector3(
					Mathf.Clamp(curVelocity.x, -maxVelocity.x, maxVelocity.x),
					Mathf.Clamp(curVelocity.y, -maxVelocity.y, maxVelocity.y),
					Mathf.Clamp(curVelocity.z, -maxVelocity.z, maxVelocity.z));
			}
			Rbody.angularVelocity = Vector3.zero;
			wasOnFloor = OnFloor;
		}

		//

		public void Jump() {
			if (!OnFloor || jumpTime > Time.time) { return; }
			Rbody.AddRelativeForce(new Vector3(0f, CurState.jumpForce, 0f), ForceMode.Impulse);
			jumpTime = Time.time + 0.1f;
		}

		//

#if UNITY_EDITOR
		void OnDrawGizmos() {
			if (capsule == null) { return; }
			var c = capsule;
			Gizmos.matrix = transform.localToWorldMatrix;
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireCube(c.center, new Vector3(c.radius * 2f, c.height, c.radius * 2f));
			var s = Application.isPlaying ? curState : states[0];
			Gizmos.color = Color.green;
			Gizmos.DrawWireCube(c.center + Vector3.down * (c.height + s.legHeight) * 0.5f, new Vector3(c.radius * 2f, s.legHeight, c.radius * 2f));
			Gizmos.color = Color.cyan;
			Gizmos.DrawWireCube(c.center + Vector3.down * (c.height * 0.5f + s.legHeight + s.legDepth * 0.5f), new Vector3(c.radius * 2f, s.legDepth, c.radius * 2f));
		}

#endif
	}

}