﻿using UnityEngine;
using System.Collections;

public class WorldController : MonoBehaviour {

	private string ID;
	public static WorldController Instance {get; protected set;}

	public World World { get; protected set; }

	// Use this for initialization
	void Awake () {
		Instance = this;
		ID = IDGenerator.CreateUniqueId (typeof(WorldController));
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Initialize () {
		CreateEmptyWorld ();
	}

	private void CreateEmptyWorld() {
		World = new World (50, 50);
	}
}
