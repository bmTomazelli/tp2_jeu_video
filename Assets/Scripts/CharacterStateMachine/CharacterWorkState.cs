
using UnityEngine;

public class CharacterWorkState : ICharacterState
{
    private readonly Character character;
    private Building workplace;
    public CharacterWorkState(Character character)
    {
        this.character = character;
    }

    public void Enter()
    {
        workplace = character.Blackboard.Workplace;
        character.StateMachine.CurrentStateName = "Travaille";
    }

    public void Exit()
    {

    }

    public void Update()
    {
        if (!character.IsCloseTo(workplace))
        {
            character.StateMachine.PushState(new CharacterTravelState(workplace, character));
            return;
        }

        character.Vitals.RaiseHunger();
        character.Vitals.RaiseSleepiness();
        character.Vitals.RaiseLoneliness();

        character.MakeInvisible();

        if (character.Vitals.IsHungerAboveThreshold)
        {
            character.StateMachine.PushState(new CharacterEatState(character));
            return;
        }
        if (character.Vitals.IsSleepinessAboveThreshold)
        {
            character.StateMachine.PushState(new CharacterSleepState(character));
            return;
        }
        if(character.Vitals.IsLonelinessAboveThreshold)
        {
            var friend = character.Blackboard.LastSeenFriend;
            if (friend != null)
            {
                character.StateMachine.PushState(new CharacterSocializeState(character));
                return;
            }
        }
    }
}
