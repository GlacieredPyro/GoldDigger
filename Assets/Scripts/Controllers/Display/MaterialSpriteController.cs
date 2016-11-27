using System;
using UnityEngine;
using System.Collections.Generic;

public class MaterialSpriteController : MonoBehaviour {

	string ID;
	Dictionary<Material, GameObject> materialGameObjectMap;
	//TODO support mutiple sprites
	Sprite materialSprite;

	void Awake() {
		materialGameObjectMap = new Dictionary<Material, GameObject> ();
		ID = IDGenerator.CreateUniqueId (typeof(MaterialSpriteController));
		PreloadSprites ();
	}

	public void OnMaterialManagerInitialized() {
		MaterialManager.Instance.RegisterForAllMaterialsChanged(OnMaterialChanged);	
	}

	public void OnMaterialChanged(Material m) {
		//get the game object for this material
		GameObject go = GetGameObjectFor (m);

		//Should it show?
		if (ShouldRenderMaterial (m)) {
			//Material doesnt have a go yet?
			if (go == null) {
				//Make one
				go = CreateGameObjectFor (m);
				//add it to the list
				materialGameObjectMap.Add (m, go);
			}

			go.SetActive (true);
		} else {
			//if it doesnt have a game object
			if (go != null) {
				//do nothing
				//we create one when it actually has to render.
			} else {
				go.transform.position = new Vector3 (m.Tile.X, m.Tile.Y, 0);
				go.SetActive (false);
			}
		}

	}

	private bool ShouldRenderMaterial(Material m) {
		return m.Tile != null;
	}

	//TODO Improve this with a Pool
	private GameObject CreateGameObjectFor(Material m) {
		GameObject mat_go = new GameObject ();
		mat_go.name = "Dirt";
		mat_go.transform.position = new Vector3 (m.Tile.X, m.Tile.Y, 0f);
		mat_go.transform.SetParent (this.transform, true);
		
		SpriteRenderer sr = mat_go.AddComponent<SpriteRenderer> ();
		sr.sprite = materialSprite;
		sr.sortingLayerName = "Materials";

		return mat_go;
	}

	private GameObject GetGameObjectFor(Material m) {
		if (materialGameObjectMap.ContainsKey (m)) {
			return materialGameObjectMap [m];
		}
		return null;
	}

	void PreloadSprites() {
		materialSprite = Resources.Load<Sprite> ("Sprites/Materials/Dirt");

		Debug.Log (ID + ":: finished pre-loading sprites");
	}
}
