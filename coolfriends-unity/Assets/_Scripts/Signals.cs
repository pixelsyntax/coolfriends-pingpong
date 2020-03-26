using System.Collections.Generic;
using UnityEngine;

namespace COOLFRIENDS {

	public static class Signals {
		public static Signal<Collider, Vector3> PlayerThrobs = new Signal<Collider, Vector3>("player_throbs");
	}

	//
	
	public interface ISignal {
	}

	[System.Flags]
	public enum SignalCallee {
		TargetOnly = 1,
		GeneralOnly = 2,
		Both = 3
	}

	public class Signal<TTarget> : ISignal, System.IEquatable<Signal<TTarget>> {
		readonly string name;
		readonly int hash;
		Dictionary<TTarget, List<System.Action>> targetedActions = new Dictionary<TTarget, List<System.Action>>();
		List<System.Action<TTarget>> generalActions = new List<System.Action<TTarget>>();

		//

		public Signal(string name) {
			this.name = name;
			hash = name.GetHashCode();
		}

		public void Register(TTarget target, System.Action action) {
			if (target == null || action == null) { return; }
			if (!targetedActions.TryGetValue(target, out var actions)) {
				actions = targetedActions[target] = new List<System.Action>();
			}
			actions.Add(action);
		}

		public void Register(System.Action<TTarget> action) {
			if (action == null) { return; }
			generalActions.Add(action);
		}

		public void Unregister(TTarget target, System.Action action) {
			if (target == null || action == null) { return; }
			if (targetedActions.TryGetValue(target, out var actions)) {
				actions.Remove(action);
			}
		}

		public void Unregister(System.Action<TTarget> action) {
			if (action == null) { return; }
			generalActions.Remove(action);
		}

		public void Broadcast(TTarget target, SignalCallee callee = SignalCallee.Both) {
			if (target == null) { return; }
			if ((callee & SignalCallee.TargetOnly) != 0 && targetedActions.TryGetValue(target, out var actions)) {
				foreach (var a in actions) { a(); }
			}
			if ((callee & SignalCallee.GeneralOnly) != 0 && generalActions.Count > 0) {
				foreach (var ga in generalActions) { ga(target); }
			}
		}

		//

		public bool Equals(Signal<TTarget> other) { return hash == other.hash; }
		public override int GetHashCode() { return hash; }
		public override string ToString() { return name; }
	}

	public class Signal<TTarget, T1> : ISignal, System.IEquatable<Signal<TTarget, T1>> {
		readonly string name;
		readonly int hash;
		Dictionary<TTarget, List<System.Action<T1>>> targetedActions = new Dictionary<TTarget, List<System.Action<T1>>>();
		List<System.Action<TTarget, T1>> generalActions = new List<System.Action<TTarget, T1>>();

		//

		public Signal(string name) {
			this.name = name;
			hash = name.GetHashCode();
		}

		public void Register(TTarget target, System.Action<T1> action) {
			if (target == null || action == null) { return; }
			if (!targetedActions.TryGetValue(target, out var actions)) {
				actions = targetedActions[target] = new List<System.Action<T1>>();
			}
			actions.Add(action);
		}

		public void Register(System.Action<TTarget, T1> action) {
			if (action == null) { return; }
			generalActions.Add(action);
		}

		public void Unregister(TTarget target, System.Action<T1> action) {
			if (target == null || action == null) { return; }
			if (targetedActions.TryGetValue(target, out var actions)) {
				actions.Remove(action);
			}
		}

		public void Unregister(System.Action<TTarget, T1> action) {
			if (action == null) { return; }
			generalActions.Remove(action);
		}

		public void Broadcast(TTarget target, T1 param1, SignalCallee callee = SignalCallee.Both) {
			if (target == null) { return; }
			if ((callee & SignalCallee.TargetOnly) != 0 && targetedActions.TryGetValue(target, out var actions)) {
				foreach (var a in actions) { a(param1); }
			}
			if ((callee & SignalCallee.GeneralOnly) != 0 && generalActions.Count > 0) {
				foreach (var ga in generalActions) { ga(target, param1); }
			}
		}

		//

		public bool Equals(Signal<TTarget, T1> other) { return hash == other.hash; }
		public override int GetHashCode() { return hash; }
		public override string ToString() { return name; }
	}

	public class Signal<TTarget, T1, T2> : ISignal, System.IEquatable<Signal<TTarget, T1, T2>> {
		readonly string name;
		readonly int hash;
		Dictionary<TTarget, List<System.Action<T1, T2>>> targetedActions = new Dictionary<TTarget, List<System.Action<T1, T2>>>();
		List<System.Action<TTarget, T1, T2>> generalActions = new List<System.Action<TTarget, T1, T2>>();

		//

		public Signal(string name) {
			this.name = name;
			hash = name.GetHashCode();
		}

		public void Register(TTarget target, System.Action<T1, T2> action) {
			if (target == null || action == null) { return; }
			if (!targetedActions.TryGetValue(target, out var actions)) {
				actions = targetedActions[target] = new List<System.Action<T1, T2>>();
			}
			actions.Add(action);
		}

		public void Register(System.Action<TTarget, T1, T2> action) {
			if (action == null) { return; }
			generalActions.Add(action);
		}

		public void Unregister(TTarget target, System.Action<T1, T2> action) {
			if (target == null || action == null) { return; }
			if (targetedActions.TryGetValue(target, out var actions)) {
				actions.Remove(action);
			}
		}

		public void Unregister(System.Action<TTarget, T1, T2> action) {
			if (action == null) { return; }
			generalActions.Remove(action);
		}

		public void Broadcast(TTarget target, T1 param1, T2 param2, SignalCallee callee = SignalCallee.Both) {
			if (target == null) { return; }
			if ((callee & SignalCallee.TargetOnly) != 0 && targetedActions.TryGetValue(target, out var actions)) {
				foreach (var a in actions) { a(param1, param2); }
			}
			if ((callee & SignalCallee.GeneralOnly) != 0 && generalActions.Count > 0) {
				foreach (var ga in generalActions) { ga(target, param1, param2); }
			}
		}

		//

		public bool Equals(Signal<TTarget, T1, T2> other) { return hash == other.hash; }
		public override int GetHashCode() { return hash; }
		public override string ToString() { return name; }
	}

}