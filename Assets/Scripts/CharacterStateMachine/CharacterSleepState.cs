public class CharacterSleepState : ICharacterState
{   
    private readonly Character character;
    public CharacterSleepState(Character character)
    {
        this.character = character;
    }

    public void Enter()
    {
        
    }

    public void Update()
    {
        if (!character.IsCloseTo(character.Blackboard.House))
        {
            character.StateMachine.PushState(new CharacterTravelState(character.Blackboard.House, character));
            return;
        }

        else 
        {
            character.Vitals.LowerSleepiness();
            character.MakeInvisible();

            if (character.Vitals.IsSleepinessBellowTarget)
            {
                character.MakeVisible();
                character.StateMachine.PopState();
            }

            return;
        }
    }

    public void Exit()
    {
        character.CancelNavigate();
    }
}
