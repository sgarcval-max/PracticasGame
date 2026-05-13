using UnityEngine;

// Información de cada tipo de pez
public static class FishData
{
    public static string GetName(FishType type)
    {
        switch (type)
        {
            case FishType.Pufferfish: return "Pez Globo";
            case FishType.Shark: return "Tiburón";
            case FishType.Clownfish: return "Pez Payaso";
            case FishType.Squid: return "Calamar";
            case FishType.Swordfish: return "Pez Espada";
            default: return "Desconocido";
        }
    }

    public static string GetDescription(FishType type)
    {
        switch (type)
        {
            case FishType.Pufferfish:
                return "💥 Explosión\nDaña y empuja a todos los enemigos cercanos.";
            case FishType.Shark:
                return "🛡️ Escudo\nTe vuelve invencible durante unos segundos.";
            case FishType.Clownfish:
                return "🎭 Señuelo\nCrea un señuelo que atrae a los enemigos.";
            case FishType.Squid:
                return "🌑 Tinta\nRalentiza a todos los enemigos cercanos.";
            case FishType.Swordfish:
                return "⚡ Dash\nSales disparado hacia el cursor dañando enemigos.";
            default:
                return "Habilidad desconocida.";
        }
    }

    public static Color GetColor(FishType type)
    {
        switch (type)
        {
            case FishType.Pufferfish: return new Color(1f, 0.4f, 0f);
            case FishType.Shark: return new Color(0.6f, 0.6f, 0.6f);
            case FishType.Clownfish: return new Color(1f, 0.55f, 0f);
            case FishType.Squid: return new Color(0.6f, 0f, 0.8f);
            case FishType.Swordfish: return new Color(0f, 0.7f, 1f);
            default: return Color.white;
        }
    }
}
