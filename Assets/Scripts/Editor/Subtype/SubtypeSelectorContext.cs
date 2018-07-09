using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEditor;

namespace SixteenFifty.Editor {
  using Reflection;
  
  /**
   * \brief
   * Maintains state for populating subtype selectors.
   */
  [Serializable]
  public class SubtypeSelectorContext<T> where T : class {
    /**
     * \brief
     * Maps selectable types to their editors.
     */
    Dictionary<Type, Type> editors;

    private static string GetSelectableSubtypeFriendlyName(Type type) =>
      type.FindCustomAttribute<SelectableSubtype>()
      ?.FindNamedArgument<string>("friendlyName")
      ?.Nullify();

    /**
     * \brief
     * A list of all `T` implementations with the SelectableSubtype attribute.
     */
    private Type[] selectableTypes;
    public Type[] SelectableTypes {
      get {
        if(null != selectableTypes)
          return selectableTypes;
        var types = GetSelectableTypes().ToList();
        types.Sort(
          (x, y) =>
          Comparer<string>.Default.Compare(
            GetSelectableSubtypeFriendlyName(x),
            GetSelectableSubtypeFriendlyName(y)));
        return selectableTypes = types.ToArray();
      }
    }

    /**
     * \brief
     * A list of the names of all supported events.
     *
     * This array is basically just used for reducing memory usage
     * when creating SelectorControl objects for choosing an event
     * type.
     */
    private string[] selectableTypeNames;
    public string[] SelectableTypeNames {
      get {
        if(null != selectableTypeNames)
          return selectableTypeNames;
        selectableTypeNames =
          SelectableTypes.Select(
            t =>
            GetSelectableSubtypeFriendlyName(t))
          .ToArray();
        return selectableTypeNames;
      }
    }

    public void EnsureEditorsAreReady() {
      if(null != editors)
        return;
      UpdateKnownEditors();
    }

    /**
    * \brief
    * Finds all implementations of `T` with the SelectableSubtype
    * attribute.
    *
    * This set of types maybe be larger that the set of *editable*
    * types, since it is possible to define a type decorated
    * with SelectableSubtype without also defining an editor for it
    * (namely, a subclass of ISubtypeEditor decorated with
    * `SubtypeEditorFor(target = ...)`).
    */
    private static IEnumerable<Type> GetSelectableTypes() =>
      SubtypeReflector
      .GetImplementations<T, T>()
      .WithAttribute<SelectableSubtype>();

    public static IEnumerable<Tuple<Type, Type>> GetEditorTypes() {
      var foo = 
        SubtypeReflector
        .GetImplementations<ISubtypeEditor<T>, ISubtypeEditor<T>>()
        .WithAttribute<SubtypeEditorFor>();

      Debug.LogFormat("l = {0}", foo.ToArray().Length);

      foreach(var t in foo) {
        var target = 
          t.FindCustomAttribute<SubtypeEditorFor>()
          ?.FindNamedArgument<Type>("target")
          .ToArray();
        if(target.Length > 0)
          yield return Tuple.Create(target[0], t);
      }
    }

    /**
     * \brief
     * Finds the editor for each event type, and associates that type
     * with its editor type.
     */
    public static Dictionary<Type, Type> GetEditorDictionary() =>
      // all implementations of ScriptedEventItemEditor with the
      // ScriptedEventItemEditor attribute.
      GetEditorTypes()
      .ToDictionary(
        tup => tup.Item1,
        tup => tup.Item2);

    private void UpdateKnownEditors() {
      editors = GetEditorDictionary();
      Debug.LogFormat(
        "Editor map updated.\n" +
        "{0}",
        String.Join(
          "\n",
          editors.Select(
            kvp =>
            String.Format("{0} -> {1}", kvp.Key, kvp.Value))));
    }

    /**
     * \brief
     * Creates a new editor according to an index into the
     * #SupportedEvents array.
     *
     * \returns
     * The new editor object.
     */
    public ISubtypeEditor<T> GetEditor(int i) =>
      GetEditor(SelectableTypes[i]);
    
    /**
     * \brief
     * Creates a new editor for the given type.
     *
     * \returns
     * The editor instance, or null if there is none for the given
     * type.
     */
    public ISubtypeEditor<S> GetEditor<S>() where S : T =>
      GetEditorClass<S>()
      ?.Construct<SubtypeSelectorContext<T>, ISubtypeEditor<S>>(this);

    /**
     * \brief
     * Creates a new editor for the given type.
     *
     * \returns
     * The editor instance, or null if there is none for the given
     * type.
     */
    public ISubtypeEditor<T> GetEditor(Type t) =>
      GetEditorClass(t)
      ?.Construct<SubtypeSelectorContext<T>, ISubtypeEditor<T>>(this);

    /**
     * \brief
     * Gets the editor class for the given type.
     *
     * \returns
     * The editor instance, or null if there isn't one.
     */
    public Type GetEditorClass<S>() where S : T =>
      GetEditorClass(typeof(S));

    /**
     * \brief
     * Gets the editor for the given type.
     *
     * It is preferable to call the generic version of this method.
     *
     * \returns
     * The editor instance, or null if there isn't one.
     */
    public Type GetEditorClass(Type t) {
      EnsureEditorsAreReady();

      Type e;
      return editors.TryGetValue(t, out e) ? e : null;
    }

  }
}