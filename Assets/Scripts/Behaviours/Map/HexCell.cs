﻿using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(SpriteRenderer))]
public class HexCell : MonoBehaviour {
  public HexCoordinates coordinates;
  public HexTile tile;

  public HexGrid grid;

  private ISet<MapEntity> entitiesHere;
  public ISet<MapEntity> EntitiesHere => entitiesHere;

  /**
   * \brief
   * Are there any MapEntities here?
   */
  public bool IsNonEmpty => 0 < EntitiesHere.Count;

  public bool IsEmpty => 0 == EntitiesHere.Count;

  public event Action<MapEntity> EntityAdded;
  public event Action<MapEntity> EntityRemoved;

  public IEnumerable<HexCell> Neighbours =>
    coordinates
    .Neighbours
    .Select(i => grid[i])
    .Where(o => null != o);

  public void AddEntity(MapEntity e) {
    entitiesHere.Add(e);
    if(null != EntityAdded)
      EntityAdded(e);
  }

  public void RemoveEntity(MapEntity e) {
    entitiesHere.Remove(e);
    if(null != EntityRemoved)
      EntityRemoved(e);
  }

  public int SortingOrder {
    get {
      return GetComponent<SpriteRenderer>().sortingOrder;
    }
    set {
      GetComponent<SpriteRenderer>().sortingOrder = value;
    }
  }

  public static HexCell Construct(GameObject prefab, HexTile tile) {
    var self = prefab.GetComponent<HexCell>();
    self.tile = tile;
    var instance = Instantiate(prefab).GetComponent<HexCell>();
    self.tile = null;
    return instance;
  }

  void Awake () {
    Debug.Assert(null != tile);
    entitiesHere = new HashSet<MapEntity>();
    SetupRenderer();
  }

  void Start () {
    grid = GetComponentInParent<HexGrid>();
    Debug.Assert(null != grid, "owning grid of cell is not null");
  }

  void SetupRenderer() {
    var renderer = GetComponent<SpriteRenderer>();
    renderer.sprite = tile.sprite;
  }

  public override string ToString() {
    return "(Cell " + coordinates.ToString() + ")";
  }
}
