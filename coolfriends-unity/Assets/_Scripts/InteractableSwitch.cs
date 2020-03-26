using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace COOLFRIENDS {

	public class InteractableSwitch : MonoBehaviour {
		[SerializeField] UnityEngine.Events.UnityEvent[] onActivate = null;
		[SerializeField] int state = 0;

		Collider coll;
		
		//

		void Awake() {
			coll = GetComponentInChildren<Collider>();
			Signals.PlayerThrobs.Register(coll, OnPlayerThrob);
		}

		void OnDestroy() {
			Signals.PlayerThrobs.Unregister(coll, OnPlayerThrob);
		}

		//

		void OnPlayerThrob(Vector3 position) {
			if (onActivate == null || onActivate.Length == 0) { return; }
			state = (state + 1) % onActivate.Length;
			onActivate[state].Invoke();
		}
	}

}