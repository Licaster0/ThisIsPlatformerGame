public abstract class State
{
    protected CharacterController2D character;

    public State(CharacterController2D character)
    {
        this.character = character;
    }

    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
}