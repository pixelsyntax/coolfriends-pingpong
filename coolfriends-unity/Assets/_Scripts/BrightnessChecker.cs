using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace COOLFRIENDS {

	public class BrightnessChecker : MonoBehaviour {

		public enum CalculateMethod { Max, Average }
		
		[SerializeField] CalculateMethod calcMethod = CalculateMethod.Max;
		[SerializeField] Vector3[] checkPoints = new[] { Vector3.zero };
		//
		public Vector3[] CheckPoints => checkPoints;
		[SerializeField] UnityEngine.UI.Image uiLightGem = null;
		[SerializeField] LayerMask layers = ~0;
		float lightGemChangeVel;
		//
		public float HeightFactor { get; set; } = 1f;

		//

		void Update() {
			var b = CheckBrightness();
			if (uiLightGem != null) {
				var col = Color.white * Mathf.SmoothDamp(uiLightGem.color.r, b, ref lightGemChangeVel, 0.05f);
				col.a = 1f;
				uiLightGem.color = col;
			}
		}

		//

		float CheckBrightness() {
			switch (calcMethod) {
				case CalculateMethod.Max: return CheckBrightness_Max();
				case CalculateMethod.Average: return CheckBrightness_Average();
			}
			return 0f;
		}

		float CheckBrightness_Max() {
			var brightness = 0f;
			foreach (var cp in checkPoints) {
				var p = transform.TransformPoint(cp);
				p.y *= HeightFactor;
				brightness = Mathf.Max(brightness, Main.Inst.LightGrid.GetBrightnessAt(p, layers.value));
			}
			return brightness;
		}

		float CheckBrightness_Average() {
			var brightnessAcc = 0f;
			foreach (var cp in checkPoints) {
				var p = transform.TransformPoint(cp);
				brightnessAcc += Main.Inst.LightGrid.GetBrightnessAt(p, layers.value);
			}
			return brightnessAcc / checkPoints.Length;
		}

		//

#if UNITY_EDITOR
		void OnDrawGizmos() {
			foreach (var cp in checkPoints) {
				var p = transform.TransformPoint(cp);
				p.y *= HeightFactor;
				Gizmos.color = Color.cyan;
				Gizmos.DrawLine(transform.position, p);
				Gizmos.DrawSphere(p, 0.125f);
			}
		}
#endif
	}

}