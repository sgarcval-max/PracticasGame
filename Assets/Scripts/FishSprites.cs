using UnityEngine;

public class FishSprites : MonoBehaviour
{
    public static FishSprites Instance;

    [Header("Sprites de los peces")]
    public Sprite pufferfishSprite;
    public Sprite sharkSprite;
    public Sprite clownfishSprite;
    public Sprite squidSprite;
    public Sprite swordfishSprite;
    public Sprite cirujanoSprite;

    void Awake()
    {
        Instance = this;
    }

    public Sprite GetSprite(FishType type)
    {
        switch (type)
        {
            case FishType.Pufferfish: return pufferfishSprite;
            case FishType.Shark: return sharkSprite;
            case FishType.Clownfish: return clownfishSprite;
            case FishType.Squid: return squidSprite;
            case FishType.Swordfish: return swordfishSprite;
            case FishType.Cirujano: return cirujanoSprite;
            default: return null;
        }
    }
}
