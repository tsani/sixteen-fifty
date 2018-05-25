﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * \brief
 * Renders a HexMapEntity in the correct orientation.
 * This component connects a HexMapEntity to a MapEntity.
 */
[RequireComponent(typeof(SpriteRenderer))]
public class MapOrientation : MonoBehaviour {
  private HexDirection orientation;

  public HexMapEntity hexMapEntity;

  /**
   * \brief
   * The current orientation of the MapEntity.
   *
   * Setting this property changes which Sprite the SpriteRenderer
   * will draw.
   */
  public HexDirection Orientation {
    get { return orientation; }
    set {
      orientation = value;
      if(null != renderer)
        renderer.sprite = hexMapEntity[orientation];
    }
  }

  /**
   * \brief
   * The renderer to use to draw the HexMapEntity's Sprites.
   *
   * Initialized in ::Awake.
   */
  [SerializeField]
  new private SpriteRenderer renderer;

  /**
   * \brief
   * The MapEntity to monitor for orientation changes.
   * 
   * Initialized in ::OnEnable.
   */
  [SerializeField]
  private MapEntity mapEntity;

  void Awake() {
    renderer = this.GetComponentNotNull<SpriteRenderer>();
    // to set the sprite
    Orientation = Orientation;
  }

  void OnEnable() {
    mapEntity = GetComponent<MapEntity>();
    if(null != mapEntity) {
      mapEntity.ChangeDirection += OnChangeDirection;
    }
  }

  void OnDisable() {
    if(null != mapEntity)
      return;
    mapEntity.ChangeDirection -= OnChangeDirection;
  }

  /**
   * \brief
   * Handles ::mapEntity::ChangeDirection.
   */
  void OnChangeDirection(MapEntity me, HexDirection d) {
    Orientation = d;
  }
}