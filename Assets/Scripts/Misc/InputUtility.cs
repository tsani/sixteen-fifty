using UnityEngine;

namespace SixteenFifty {
  /**
   * \brief
   * Gets the mouse position in world coordinates.
   */
  public static class InputUtility {
    public static Vector2 PointerPosition =>
      Camera.main.ScreenPointToRay(Input.mousePosition).origin.Downgrade();

    public static Vector2 PrimaryAxis =>
      new Vector2(
        Input.GetAxis("Horizontal"),
        Input.GetAxis("Vertical"));
  }
}
