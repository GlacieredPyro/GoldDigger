using System;
using UnityEngine;
using System.Collections;

/// <summary>
/// Effectively dirt
/// </summary>
public class Material {

	public Tile Tile { get; protected set; }
	public Character Character { get; protected set; }

	public string Type { get; protected set; }

	int currentAmount;

	int maxAmount;

	/// <summary>
	/// The yield per unit of material.
	/// Affects quality of dirt effectively.
	/// </summary>
	public float yieldPerUnit { get; protected set; }

	Action<Material> OnMaterialChanged;

	public Material(int currentAmount, int maxAmount, float yieldPerUnit) {
		this.currentAmount = currentAmount;
		this.maxAmount = maxAmount;
		this.yieldPerUnit = yieldPerUnit;
	}

	public Boolean IsValidPlacementTile(Tile tile) {
		if (tile.Fixture == null) {
			return true;
		}
		return false;
	}

	public void PlaceAtTile(Tile tile) {
		if (tile.PlaceMaterial (this)) {
			this.Tile = tile;
			OnMaterialChanged (this);
		}
	}

	public void RegisterOnChanged(Action<Material> listener) {
		OnMaterialChanged += listener;
	}

	public void UnRegisterOnChanged(Action<Material> listener) {
		OnMaterialChanged -= listener;
	}

}
