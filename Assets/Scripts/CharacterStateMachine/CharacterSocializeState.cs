public class CharacterSocializeState : ICharacterState
{
    private readonly Character character;
    private readonly Character friend;
    public CharacterSocializeState(Character friend, Character character)
    {
        this.character = character;
        this.friend = friend;
    }

    public void Enter()
    {

    }

    public void Update()
    {
        if (!character.IsCloseTo(friend))
        {
            character.StateMachine.PushState(new CharacterTravelState(friend, character));
            return;
        }
        else
        {
            character.Vitals.LowerLoneliness();
            if (!character.IsGreetingCharacter())
            {
                character.GreetCharacter(friend);
                character.StateMachine.PopState();
            }
            return;
        }
    }

    public void Exit()
    {

    }
}
