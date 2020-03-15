using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace COOLFRIENDS {

	public class Main : MonoBehaviour {
		public static Main Inst { get; private set; }
		//
		[SerializeField] GameObject levelLights = null;
		//
		public LightGrid LightGrid { get; private set; }

		//

		void Awake(){
			Inst = this;
			LightGrid = new LightGrid();
			LightGrid.Add(levelLights);
		}

	}

}