public interface ICombatant
{
    // Called when "DRAW!" appears. Implement to kick off reaction.
    void OnSignalShown(float signalTime);

    // True once this side has "attacked" (pressed).
    bool HasActed { get; }

    // If acted, when did they act (Time.time)?
    float ActTime { get; }
}