[System.Serializable]
public class ActiveEffect
{
    public EffectData Data;
    public float RemainingTime;
    public float TickTimer; // Это нужно для отсчета времени между ударами яда

    public ActiveEffect(EffectData data)
    {
        Data = data;
        RemainingTime = data.duration;
        TickTimer = 0; // Сработает сразу при наложении
    }
}