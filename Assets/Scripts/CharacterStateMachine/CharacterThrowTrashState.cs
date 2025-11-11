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
        var trashBehavior = character.StateMachine.TrashBehaviour;
        if (!character.IsThrowingTrash() && trashBehavior == CityCharacterTrashBehaviour.Throw)
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

