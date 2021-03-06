using System;

using UnityEngine;

namespace SixteenFifty.Behaviours {
  using Variables;

  public class IsoPositioner : MonoBehaviour, IPositioner {
    public Vector2Variable destination;

    public event Action Positioned;

    void OnEnable() {
      destination.RepositionRequested += Reposition;
    }

    void OnDisable() {
      destination.RepositionRequested -= Reposition;
    }

    public void Reposition() {
      if(null == destination)
        return;
      
      transform.position = destination.Value;

      Positioned?.Invoke();
    }

    void Start() {
      Reposition();
    }
  }
}
