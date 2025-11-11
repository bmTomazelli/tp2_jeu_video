public class CharacterSocializeState : ICharacterState
{
    private readonly Character character;
    private  Building bar;
    public CharacterSocializeState(Character character)
    {
        this.character = character;
        this.bar= ArrayExtensions.Random(character.Blackboard.SocialBuildings);
    }

    public void Enter()
    {

    }

    public void Update()
    {
        if (character.IsCloseTo(bar))
        {
            character.MakeInvisible();
            character.Vitals.LowerLoneliness();

            if (character.Vitals.IsLonelinessBellowTarget)
            {
                character.MakeVisible();
                character.StateMachine.PopState();
            }

            return;
        }

        else
        {
            character.StateMachine.PushState(new CharacterTravelState(bar, character));
            return;
        }
    }

    public void Exit()
    {

    }
}
