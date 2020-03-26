using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace COOLFRIENDS {

	[RequireComponent(typeof(Creature))]
	public class Player : MonoBehaviour {
		[SerializeField] float runSpeedFactor = 1.4f;
		[SerializeField] float rotateSpeed = 180f;
		[SerializeField] float rotatePerSecondMax = 30f;
		[SerializeField] RatKing.Base.RangeFloat pitchRange = new RatKing.Base.RangeFloat(-60f, 60f);
		[SerializeField] Transform lookDummy = null;
		//
		Creature creature = null;
		public Creature Creature => creature != null ? creature : creature = GetComponent<Creature>();
		BrightnessChecker brightnessChecker = null;
		public BrightnessChecker BrightnessChecker => brightnessChecker != null ? brightnessChecker : brightnessChecker = GetComponent<BrightnessChecker>();
		//
		float curPitch = 0f;
		bool mouseLook = false;

		//

		void Awake() {
			if (lookDummy == null) { lookDummy = Creature.MoveDummy; }
		}

		void Update() {
			Creature.MoveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

#if UNITY_EDITOR
			mouseLook = Input.GetMouseButton(1);
#else
			if (Input.GetMouseButtonDown(1)) { mouseLook = !mouseLook; }
#endif
			Cursor.lockState = mouseLook ? CursorLockMode.Locked : CursorLockMode.None;
			Cursor.visible = !mouseLook;

			if (mouseLook) {
				var rotMax = rotatePerSecondMax * Time.deltaTime;
				var yaw = Quaternion.Euler(0f, Mathf.Clamp(Input.GetAxis("Mouse X") * Time.deltaTime * rotateSpeed, -rotatePerSecondMax, rotatePerSecondMax), 0f);
				Creature.Rbody.rotation = yaw * Creature.Rbody.rotation;
				curPitch = pitchRange.Clamp(curPitch - Mathf.Clamp(Input.GetAxis("Mouse Y") * Time.deltaTime * rotateSpeed, -rotatePerSecondMax, rotatePerSecondMax));
				lookDummy.localEulerAngles = new Vector3(curPitch, 0f, 0f);
			}

			if (Input.GetButtonDown("Jump")) {
				Creature.Jump();
			}

			if (Input.GetButtonDown("Interact")) {
				TryToInteract();
			}

			Creature.TargetStateIdx = Input.GetButton("Crouch") ? 1 : 0;
			Creature.MoveSpeedFactor = Input.GetButton("Run") ? runSpeedFactor : 1f;

			BrightnessChecker.HeightFactor = Creature.CurState.heightFactor;
		}
		
#if UNITY_EDITOR
		void OnGUI() {
			GUI.Label(new Rect(10, 10, 500, 300), "OnFloor: " + Creature.OnFloor + "\nPress RMB to activate mouse look!\nLMB - Interact\nR - Restart\nF12 - Screenshot");
		}
#endif

		//

		void TryToInteract() {
			var ray = new Ray(lookDummy.position, lookDummy.forward);
			if (Physics.Raycast(ray, out var hit, 1.5f, ~0, QueryTriggerInteraction.Ignore)) {
				Signals.PlayerThrobs.Broadcast(hit.collider, hit.point);
			}
		}
	}

}