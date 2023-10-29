namespace WaitCommands {

using System.Collections;
using UnityEngine;

public class WaitCommands {

    private IEnumerator waitForKeyPress(KeyCode key) {

            bool done = false;

            while (!done) {

                if (Input.GetKeyDown(key)) {

                    done = true;

                }
                
                yield return null;

            }

        }
}
}