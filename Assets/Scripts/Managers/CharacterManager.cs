using System;
using UnityEngine;
using System.Collections.Generic;

public class CharacterManager {

	private static CharacterManager _instance;
	private string ID;
	public static CharacterManager Instance {
		get{ 
			if (_instance == null) {
				CharacterManager i = new CharacterManager ();
				i.ID = IDGenerator.CreateUniqueId (typeof(CharacterManager));
				_instance = i;
			}
			return _instance;
		} 
		protected set { 
			_instance = value;
		}
	}

	public List<Character> characters { get; protected set; }
	private Action<Character> OnCharacterCreated;

	public void InitializeCharacterManager() {
		characters = new List<Character> ();
	}

	public Character CreateCharacter(Tile tile) {
		Character c = new Character (tile);
		if (OnCharacterCreated != null) {
			OnCharacterCreated (c);
		}
		Debug.Log (ID + ":: created char.");
		characters.Add (c);
		return c;
	}

	public void RegisterOnCharacterCreated(Action<Character> listener) {
		OnCharacterCreated += listener;
	}

	public void UnRegisterOnCharacterCreated(Action<Character> listener) {
		OnCharacterCreated -= listener;
	}

	public void Update(float deltaTime) {
		foreach (Character c in characters) {
			c.Update (deltaTime);
		}
	}

	public Job GetJobFor(Character c) {
		if (JobManager.AvailableJobCount (JobType.BUILD) == 0) {
			if (c.Material == null || c.Material.IsFull() == false) {
				return JobManager.DequeueJob (JobType.MINE);
			}
		} else {
			return JobManager.DequeueJob (JobType.BUILD);
		}
		return null;
	}

	private CharacterManager () {
		
	}
}