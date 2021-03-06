﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SixteenFifty {
  namespace Commands {
    /**
    * \brief
    * Waits for a specified duration.
    */
    public class Delay : Command<object> {
      float seconds;

      public Delay(float seconds) {
        this.seconds = seconds;
      }

      public override IEnumerator GetCoroutine() {
        var t = 0f;
        while(t < seconds) {
          t += Time.deltaTime;
          yield return null;
        }
        yield break;
      }
    }
  }
}
