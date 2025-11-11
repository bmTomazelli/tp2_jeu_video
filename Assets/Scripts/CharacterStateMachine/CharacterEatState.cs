using UnityEngine;

public class CharacterEatState : ICharacterState
{
    private readonly Character character;
    private Building restaurant;
    public CharacterEatState(Character character)
    {
        this.character = character;
    }

    public void Enter()
    {
        restaurant = ArrayExtensions.Random(character.Blackboard.FoodBuildings);
    }

    public void Update()
    {
        if(character.IsCloseTo(restaurant))
        {
            character.MakeInvisible();
            character.Vitals.LowerHunger();

            if (character.Vitals.IsHungerBellowTarget)
            {
                character.MakeVisible();
                character.StateMachine.PopState();
            }

            return;
        }

        else
        {
            character.StateMachine.PushState(new CharacterTravelState(restaurant, character));
            return;
        }
    }

    public void Exit()
    {
        character.CancelNavigate();
    }
}
