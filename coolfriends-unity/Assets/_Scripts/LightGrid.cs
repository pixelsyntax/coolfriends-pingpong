using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace COOLFRIENDS {

	public class LightList {
		public List<Light> lights = new List<Light>();
	}

	public class LightGrid {
		public static readonly int gridSize = 10;
		public static Vector3Int GetKey(Vector3 pos) {
			return Vector3Int.FloorToInt(pos / (float)gridSize);
		}

		//

		Light directionalLight;
		Dictionary<Vector3Int, LightList> cells = new Dictionary<Vector3Int, LightList>();
		
		//

		void AddToCell(Vector3Int key, Light l) {
			if (!cells.TryGetValue(key, out LightList list)) { list = cells[key] = new LightList(); }
			if (!list.lights.Contains(l)) { list.lights.Add(l); }
		}

		void RemoveFromCell(Vector3Int key, Light l) {
			if (!cells.TryGetValue(key, out LightList list)) { return; }
			list.lights.Remove(l);
		}

		//

		public void SetDirectionalLight(Light l) {
			directionalLight = l;
		}

		public void RemoveDirectionalLight() {
			directionalLight = null;
		}

		public void Add(GameObject go) {
			var lights = go.GetComponentsInChildren<Light>(true);
			if (lights.Length == 0) { return; }

			foreach (var l in lights) {
				if (l.type == LightType.Point || l.type == LightType.Spot) {
					// point and spot lights:
					var start = GetKey(l.transform.position - Vector3.one * l.range);
					var end = GetKey(l.transform.position + Vector3.one * l.range);
					for (int z = start.z; z <= end.z; ++z) {
						for (int y = start.y; y <= end.y; ++y) {
							for (int x = start.x; x <= end.x; ++x) {
								// TODO: calculate radius (box/sphere test) for point lights
								// TODO: calculate angle (box/cone test) for spot lights
								// TODO: put lights with shadow at end, other at beginning
								AddToCell(new Vector3Int(x, y, z), l);
							}
						}
					}
				}
				else if (l.type == LightType.Directional) {
					SetDirectionalLight(l);
				}
			}
		}

		public void Remove(GameObject go) {
			var lights = go.GetComponentsInChildren<Light>(true);
			if (lights.Length == 0) { return; }

			foreach (var l in lights) {
				// point and spot lights:
				var start = GetKey(l.transform.position - Vector3.one * l.range);
				var end = GetKey(l.transform.position + Vector3.one * l.range);
				for (int z = start.z; z <= end.z; ++z) {
					for (int y = start.y; y <= end.y; ++y) {
						for (int x = start.x; x <= end.x; ++x) {
							RemoveFromCell(new Vector3Int(x, y, z), l);
						}
					}
				}
			}
		}

		public float GetBrightnessAt(Vector3 pos, int layer = ~0, float max = 1f, Vector3? normal = null) {
			var bright = RenderSettings.ambientLight.grayscale;
			if (bright >= max) { return max; }

			// directional light, if there is one
			var dl = directionalLight;
			if (dl != null && dl.isActiveAndEnabled) {
				if (dl.shadows == LightShadows.None || !Physics.Raycast(pos, -dl.transform.forward, 1000f, layer, QueryTriggerInteraction.Ignore)) {
					var b = dl.color.grayscale * dl.intensity;
					if (normal != null) { b *= Mathf.Clamp01(Vector3.Dot(dl.transform.forward, normal.Value)); }
					bright += b;
					if (bright >= max) { return max; }
				}
			}

			var key = GetKey(pos);
			LightList list = null;
			if (cells.TryGetValue(key, out list)) {
				foreach (var l in list.lights) {
					var lpos = l.transform.position;
					var dir = pos - lpos;
					var distSqr = dir.sqrMagnitude;
					if (l.intensity <= 0f || distSqr >= l.range * l.range) { continue; }
					var lineDist = l.type == LightType.Spot
						? Vector3.Angle(l.transform.forward, dir) / (l.spotAngle * 0.5f) // normalized!
						: 0f;
					if (lineDist > 1f) { continue; }
					var dist = Mathf.Sqrt(distSqr);
					if (l.shadows != LightShadows.None && Physics.Raycast(lpos, dir, dist, layer, QueryTriggerInteraction.Ignore)) { continue; }
					dist /= l.range; // now normalized!
					var b = l.color.grayscale * l.intensity; // TODO: correct formula for distance?
					if (normal != null) { b *= Mathf.Clamp01(Vector3.Dot(dir, normal.Value)); if (b == 0f) { continue; } }
					if (l.type == LightType.Point) {
						// https://forum.unity.com/threads/light-distance-in-shader.509306/#post-3326818
						b *= Mathf.Clamp01(1f / (1f + 25f*dist*dist) * Mathf.Clamp01((1f - dist) * 5f));
					}
					else if (l.type == LightType.Spot) {
						/* TODO: ANGLE!! */
						b *= Mathf.Clamp01(1f / (1f + 25f*lineDist*lineDist) * Mathf.Clamp01((1f - lineDist) * 5f));
						b *= 1f - dist;
					}
					bright += b;
					if (bright >= max) { return max; }
				}
			}

			return Mathf.Min(bright, max);
		}
	}

}