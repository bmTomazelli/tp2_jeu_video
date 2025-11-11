class CharacterGreetingState : ICharacterState
{
    private readonly Character character;
    private readonly Character friend;
    public CharacterGreetingState(Character character)
    {
        this.character = character;
        this.friend = character.Blackboard.LastSeenFriend;
    }
    public void Enter()
    {
        
    }
    public void Update()
    {
        if(friend != null)
        {
            if (!character.IsGreetingCharacter())
            character.GreetCharacter(friend);

            else
            {
                character.StateMachine.PopState();
            }
        }
    }

    public void Exit()
    {
        
    }

    
}