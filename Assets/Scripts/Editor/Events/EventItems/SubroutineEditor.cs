using System;

using UnityEngine;
using UnityEditor;

namespace SixteenFifty.Editor {
  using EventItems;
  
  [SubtypeEditorFor(target = typeof(Subroutine))]
  public class SubroutineEditor : ISubtypeEditor<IScript> {
    Subroutine target;

    public SubroutineEditor(SubtypeSelectorContext<IScript> context) {
    }
    
    public bool CanEdit(Type type) =>
      type == typeof(Subroutine);

    public bool Draw(IScript _target) {
      target = _target as Subroutine;
      Debug.Assert(
        null != target,
        "Target of SubroutineEditor is of type Subroutine.");
      var old = target.target;
      return
        old !=
        (target.target =
         EditorGUILayout.ObjectField(
           "Target",
           old,
           typeof(BasicScriptedEvent),
           false)
         as BasicScriptedEvent);
    }
  }
}
