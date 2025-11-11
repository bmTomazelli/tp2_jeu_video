using Unity.VisualScripting;
using UnityEngine;

public class CharacterTravelState : ICharacterState
{
    private readonly IDestination destination;
    private readonly Character character;
    private float lastGreetTime= 0f;
    private float canGreetAfter = 10f;
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
        lastGreetTime += Time.deltaTime;
        var trash = character.Blackboard.LastSeenTrash;

        character.Vitals.RaiseHunger();
        character.Vitals.RaiseSleepiness();
        character.Vitals.RaiseLoneliness();

        if (!character.IsCloseTo(destination)) 
        { 
            character.NavigateTo(destination);

            if (character.StateMachine.TrashBehaviour == CityCharacterTrashBehaviour.PickUp && trash != null)
            {
                character.StateMachine.PushState(new CharacterPickupTrashState(trash, character));
            }

            if (character.Blackboard.LastSeenFriend != null)
            {
                if(lastGreetTime >= canGreetAfter)
                {
                    lastGreetTime = 0f;
                    character.StateMachine.PushState(new CharacterGreetingState(character));
                    return;
                }
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
