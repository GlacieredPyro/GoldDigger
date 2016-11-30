using UnityEngine;
using System.Collections.Generic;
using System;

public enum FixtureEvent {
	FIXTURE_CREATED, FIXTURE_DESTROYED
}

public class FixtureManager {

	private static FixtureManager _instance;
	private string ID;
	public static FixtureManager Instance {
		get{ 
			if (_instance == null) {
				FixtureManager i = new FixtureManager ();
				i.ID = IDGenerator.CreateUniqueId (typeof(FixtureManager));
				_instance = i;
			}
			return _instance;
		} 
		protected set { 
			_instance = value;
		}
	}

	Dictionary<string, Action<FixtureEvent, Fixture>> listeners;

	Dictionary<string, Fixture> prototypes;
	List<Fixture> fixtures;

	public Fixture PlaceFixture(string type, Tile target) {
		Fixture f = CreateFixtureInstance(type);
		if (f.IsValidPlacementTile (target)) {
			f.RegisterFixtureCreated (OnFixtureCreated);
			f.RegisterFixtureDestroyed (OnFixtureDestroyed);
			f.PlaceAtTile (target);
			fixtures.Add (f);
			//We have to invalidate the tilegraph because this changes the walkability.
			WorldController.Instance.World.InvalidateTileGraph();
			return f;
		}
		return null;
	}

	public void RegisterForAllFixtureEvents(string id, Action<FixtureEvent, Fixture> listener) {
		if (listeners.ContainsKey (id)) {
			Debug.Log (id + " is already registered for fixture events.");
			return;
		}
		listeners.Add (id, listener);
	}

	public void InitializeFixtures() {
		listeners = new Dictionary<string, Action<FixtureEvent, Fixture>> ();
		fixtures = new List<Fixture> ();
		CreateFixtureMap ();
	}

	#region Private Methods

	void OnFixtureCreated(Fixture fixture) {
		Debug.Log (ID + ":: Fixture Created : " + fixture.FixtureType);
		foreach (string key in listeners.Keys) {
			listeners [key] (FixtureEvent.FIXTURE_CREATED, fixture);
		}
	}

	void OnFixtureDestroyed(Fixture fixture) {
		Debug.Log (ID + ":: Fixture Destroyed : " + fixture.FixtureType);
		foreach (string key in listeners.Keys) {
			listeners [key] (FixtureEvent.FIXTURE_DESTROYED, fixture);
		}
	}

	private Fixture CreateFixtureInstance(string type) {
		return prototypes [type].Clone ();
	}

	private void CreateFixtureMap() {
		if (prototypes != null) {
			Debug.LogError (ID + ":: Cannot create fixture prototype map more than once");
		}
		prototypes = new Dictionary<string, Fixture> ();
		prototypes.Add ("Wall", new Fixture ("Wall", 0f, true, true));
	}

	#endregion
}
