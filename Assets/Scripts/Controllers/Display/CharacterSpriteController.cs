using System;
using UnityEngine;
using System.Collections.Generic;

public class CharacterSpriteController : MonoBehaviour {

	string ID;
	Dictionary<Character, GameObject> characterGameObjectMap;
	//TODO support mutiple sprites
	Sprite characterSprite;

	void Awake() {
		characterGameObjectMap = new Dictionary<Character, GameObject> ();
		ID = IDGenerator.CreateUniqueId (typeof(CharacterSpriteController));
		PreloadSprites ();
	}

	public void OnCharacterManagerInitialized() {
		CharacterManager.Instance.RegisterOnCharacterCreated (OnCharacterCreated);	
	}

	public void OnCharacterCreated(Character c) {
		GameObject char_go = new GameObject ();

		characterGameObjectMap.Add (c, char_go);

		char_go.name = "Char_1";
		char_go.transform.position = new Vector3 (c.X, c.Y, 0f);
		char_go.transform.SetParent (this.transform, true);

		SpriteRenderer sr = char_go.AddComponent<SpriteRenderer> ();
		sr.sprite = characterSprite;
		sr.sortingLayerName = "Characters";

		c.RegisterOnChanged (OnCharacterChanged);
	}

	void OnCharacterChanged(Character c) {
		GameObject char_go = characterGameObjectMap [c];

		char_go.transform.position = new Vector3 (c.X, c.Y, 0f);
	}

	void PreloadSprites() {
		characterSprite = Resources.Load<Sprite> ("Sprites/Characters/char_1");

		Debug.Log (ID + ":: finished pre-loading sprites");
	}
}
