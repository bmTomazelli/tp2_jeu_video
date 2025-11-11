class CharacterThrowTrashState : ICharacterState
{
    Character character;
    public CharacterThrowTrashState (Character character)
    {
        this.character = character;
    }

    public void Enter()
    {
        
    }

    public void Update()
    {
        if (!character.IsThrowingTrash())
        {
            character.ThrowTrash();
            return;
        }
        else
        {
            character.StateMachine.PopState();
            return;
        }
    }

    public void Exit()
    {

    }
}

