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
            case FishType.Cirujano: return "Cirujano";
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
            case FishType.Cirujano:
                return "💙 Curación\nRecupera 1 punto de vida.";
           
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
            case FishType.Cirujano: return new Color(0.1f, 0.4f, 0.9f);
            default: return Color.white;
        }
    }
}
