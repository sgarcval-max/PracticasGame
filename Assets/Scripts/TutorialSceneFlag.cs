using UnityEngine;

public class TutorialSceneFlag : MonoBehaviour
{
    public static bool IsTutorial = false;

    void Awake()
    {
        IsTutorial = true;
    }

    void OnDestroy()
    {
        IsTutorial = false;
    }
}
