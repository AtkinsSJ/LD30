using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Route {
	public struct Stop {
		public readonly Planet planet;
		public readonly GoodType goodType;
		public readonly StopType stopType;

		public Stop(Planet planet, GoodType goodType, StopType stopType) {
			this.planet = planet;
			this.goodType = goodType;
			this.stopType = stopType;
		}
	}

	public enum StopType {
		Buy,
		Sell
	}

	private List<Stop> stops = new List<Stop>();
	public Stop[] Stops {
		get { return stops.ToArray(); }
	}

	public void AddStop(Stop stop) {
		stops.Add(stop);
	}

	public void RemoveStop(int i) {
		stops.RemoveAt(i);
	}

	/**
	 * Returns the next stop index, when given the current stop index
	 */
	public int NextStop(int currentStop) {
		if (currentStop + 1 >= stops.Count) {
			return 0;
		} else {
			return currentStop + 1;
		}
	}

	public Stop this[int index] {
		get {
			if (index < 0 || index >= stops.Count) {
				throw new IndexOutOfRangeException();
			}
			return stops[index];
		}
	}

	public int Length {
		get { return stops.Count; }
	}
}
