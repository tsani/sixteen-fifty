﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace SixteenFifty.Behaviours {
  using EventItems;
  using UI;
  using TileMap;
  
  /**
  * \brief
  * Handler for scripted events.
  */
  public class EventManager : MonoBehaviour {

    private EventRunner currentEvent;

    public bool IsEventRunning => currentEvent != null;

    public RectTransform RectTransform {
      get;
      private set;
    }

    /**
    * \brief
    * The controller for the inventory.
    */
    public InventoryController inventory;

    /**
    * \brief
    * The current interaction menu present in the game.
    */
    public InteractionMenu interactionMenu;

    /**
     * \brief
     * Determines whether a UI is currently controlling the game.
     * In this case, inputs should not be consumed by the map.
     */
    public bool IsUI => IsEventRunning || interactionMenu.gameObject.activeInHierarchy;

    /**
    * \brief
    * The box that houses the dialogue text.
    */
    public Image dialogueTextBox;

    /**
    * \brief
    * The text object that holds dialogue text.
    */
    public Text dialogueText;

    /**
    * \brief
    * A panel used to cover the entire screen with black.
    */
    public Image fadeToBlackPanel;

    /**
    * \brief
    * Used to display full-screen images (e.g. title screen)
    */
    public Image fullscreenImage;

    /**
    * \brief
    * Houses all dialogue system related objects.
    * Controlling the alpha of this CanvasGroup is used to fade in and out the dialogue system.
    */
    public CanvasGroup DialogueSystemCanvasGroup;

    /**
    * \brief
    * Manages the inventory and its UI.
    */
    public PlayerMenuController playerMenuController;

    /**
    * \brief
    * A prefab used to create talking heads.
    */
    public GameObject speakerPrefab;

    /**
    * Used by the dialogue system.
    * The main blocking panel raises this event when it receives a
    * click event from the Unity eventsystem.
    * Event scripts can listen on this event to receive these click
    * events in turn.
    */
    public event Action<PointerEventData> MainPanelClicked;

    /**
    * Controls whether the invisible dialogue panel will intercept mouse clicks.
    * Dialogue needs to set this to true as pretty much the first thing
    * it does, and set it to false when the dialogue concludes.
    */
    public bool BlocksRaycasts {
      get {
        return DialogueSystemCanvasGroup.blocksRaycasts;
      }
      set {
        DialogueSystemCanvasGroup.blocksRaycasts = value;
      }
    }

    // Use this for initialization
    void Awake () {
      // we become the current event manager upon initialization;
      RectTransform = GetComponent<RectTransform>();
      Debug.Assert(null != RectTransform, "EventManager RectTransform is not null.");
    }

    void OnEnable() {
      StateManager.Instance.eventManager = this;
      BlocksRaycasts = false;
    }

    // Update is called once per frame
    void Update () {

    }

    public void BeginScript(HexGridManager gridManager, IScript e) {
      Debug.LogFormat(
        "Beginning event script {0}.",
        e?.GetType());
      // eventrunners null themselves out of the manager when they
      // finish executing script, so if currentEvent is not null,
      // there's already an event running.
      Debug.Assert(
        null == currentEvent,
        "Trying to run a new event while one is running.");

      currentEvent = new EventRunner(this, gridManager, e);
      currentEvent.EventComplete += OnEventComplete;
      StartCoroutine(currentEvent.Coroutine);
    }

    void OnEventComplete(IScript e) {
      Debug.LogFormat(
        "Event {0} complete.",
        e.GetType());
      currentEvent.EventComplete -= OnEventComplete;
      currentEvent = null;
    }

    public void RaiseMainPanelClicked(PointerEventData data) {
      if(null != MainPanelClicked) {
        MainPanelClicked(data);
      }
    }
  }
}
