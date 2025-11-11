using Unity.VisualScripting;

public class CharacterTravelState : ICharacterState
{
    private readonly IDestination destination;
    private readonly Character character;
    public CharacterTravelState(IDestination destination, Character character)
    {
        this.destination = destination;
        this.character = character;
    }

    public void Enter()
    {
        character.MakeVisible();
    }

    public void Update()
    {
        var trash = character.Blackboard.LastSeenTrash;

        character.Vitals.RaiseHunger();
        character.Vitals.RaiseSleepiness();
        character.Vitals.RaiseLoneliness();

        if (!character.IsCloseTo(destination)) 
        { 
            character.NavigateTo(destination);

            if (character.StateMachine.TrashBehaviour == CityCharacterTrashBehaviour.Throw && trash != null)
            {
                character.StateMachine.PushState(new CharacterPickupTrashState(trash, character));
            }
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
        character.CancelNavigate();
    }
}
