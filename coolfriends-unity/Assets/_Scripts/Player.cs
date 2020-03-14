using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace COOLFRIENDS {

	[RequireComponent(typeof(Creature))]
	public class Player : MonoBehaviour {
		[SerializeField] Vector2 rotateSpeedMax = new Vector2(180f, 360f);
		[SerializeField] RatKing.Base.RangeFloat pitchRange = new RatKing.Base.RangeFloat(-60f, 60f);
		[SerializeField] Transform lookDummy = null;
		public Creature Creature { get; private set; }
		//
		float curPitch = 0f;
		bool mouseLook = false;

		//

		void Awake() {
			Creature = GetComponent<Creature>();
			if (lookDummy == null) { lookDummy = Creature.MoveDummy; }
		}

		void FixedUpdate() {
			Creature.MoveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

#if UNITY_EDITOR
			mouseLook = Input.GetMouseButton(1);
#else
			if (Input.GetMouseButtonDown(1)) { mouseLook = !mouseLook; }
#endif
			Cursor.lockState = mouseLook ? CursorLockMode.Locked : CursorLockMode.None;
			Cursor.visible = !mouseLook;

			if (mouseLook) {
				var yaw = Quaternion.Euler(0f, Mathf.Clamp(Input.GetAxis("Mouse X"), -1f, 1f) * Time.deltaTime * rotateSpeedMax.y, 0f);
				Creature.Rbody.rotation = yaw * Creature.Rbody.rotation;
				curPitch = pitchRange.Clamp(curPitch - Input.GetAxis("Mouse Y") * Time.deltaTime * rotateSpeedMax.x);
				lookDummy.localEulerAngles = new Vector3(curPitch, 0f, 0f);
			}
		}
		
#if UNITY_EDITOR
		void OnGUI() {
			GUI.Label(new Rect(10, 10, 500, 300), "Press RMB to activate mouse look!\nR - Restart\nF12 - Screenshot");
		}
#endif
	}

}