using UnityEngine;
using System.Collections.Generic;
using System;

public class MaterialManager {

	private static MaterialManager _instance;
	private string ID;
	public static MaterialManager Instance {
		get{ 
			if (_instance == null) {
				MaterialManager i = new MaterialManager ();
				i.ID = IDGenerator.CreateUniqueId (typeof(MaterialManager));
				_instance = i;
			}
			return _instance;
		} 
		protected set { 
			_instance = value;
		}
	}
		
	List<Material> materials;
	Action<Material> OnMaterialChangedListeners;

	public Material PlaceMaterial(Tile target, Material material) {
		if (material.IsValidPlacementTile (target)) {
			material.RegisterOnChanged (OnMaterialChanged);
			material.PlaceAtTile (target);
			materials.Add (material);
			Debug.Log (ID + ":: New material placed");
			return material;
		}
		return null;
	}

	public void RegisterForAllMaterialsChanged(Action<Material> listener) {
		OnMaterialChangedListeners += listener;
	}

	public void InitializeMaterialManager() {
		materials = new List<Material> ();
	}

	void OnMaterialChanged(Material material) {
		if (OnMaterialChangedListeners != null) {
			OnMaterialChangedListeners (material);
		}
	}

	#region Private Methods



	#endregion
}
