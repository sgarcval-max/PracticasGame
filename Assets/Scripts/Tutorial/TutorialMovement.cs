using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class TutorialMovement : MonoBehaviour
{
    private bool triggered = false;
    private bool ready = false;

    IEnumerator Start()
    {
        yield return new WaitUntil(() => TutorialManager.Instance != null);
        ready = true;
    }

    void Update()
    {
        if (!ready || triggered) return;

        if (Keyboard.current.wKey.isPressed ||
            Keyboard.current.aKey.isPressed ||
            Keyboard.current.sKey.isPressed ||
            Keyboard.current.dKey.isPressed ||
            Keyboard.current.upArrowKey.isPressed ||
            Keyboard.current.downArrowKey.isPressed ||
            Keyboard.current.leftArrowKey.isPressed ||
            Keyboard.current.rightArrowKey.isPressed)
        {
            triggered = true;
            TutorialManager.Instance.TriggerAction(
                "movement",
                "Movimiento",
                "Usa WASD para moverte por el mar.\nEl buzo tiene inercia, como si estuvieras bajo el agua."
            );
        }
    }
}
