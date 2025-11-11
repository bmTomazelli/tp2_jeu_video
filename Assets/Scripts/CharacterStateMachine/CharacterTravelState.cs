using Unity.VisualScripting;
using UnityEngine;

public class CharacterTravelState : ICharacterState
{
    private readonly IDestination destination;
    private readonly Character character;
    private float lastGreetTime = 0f;
    private float greetCoolDown = 5f;
    private bool startedNav;
    public CharacterTravelState(IDestination destination, Character character)
    {
        this.destination = destination;
        this.character = character;
    }

    public void Enter()
    {
        character.MakeVisible();
        startedNav = false;
        lastGreetTime = greetCoolDown;
        character.StateMachine.CurrentStateName = "Se déplace vers " + destination.ToString();
    }

    public void Update()
    {
        lastGreetTime += Time.deltaTime;
        var trash = character.Blackboard.LastSeenTrash;

        character.Vitals.RaiseHunger();
        character.Vitals.RaiseSleepiness();
        character.Vitals.RaiseLoneliness();

        if(!startedNav)
        {
            character.NavigateTo(destination);
            startedNav = true;
        }

        if (!character.IsCloseTo(destination)) 
        { 
            character.NavigateTo(destination);

            if (character.StateMachine.TrashBehaviour == CityCharacterTrashBehaviour.PickUp && trash != null)
            {
                character.Blackboard.LastSeenTrash = null;
                character.StateMachine.PushState(new CharacterPickupTrashState(trash, character));
            }

            var friend = character.Blackboard.LastSeenFriend;
            if (lastGreetTime >= greetCoolDown && friend != null)
            {
                    lastGreetTime = 0f;
                    character.Blackboard.LastSeenFriend = null;
                character.StateMachine.PushState(new CharacterGreetingState(character, friend));
                    return;
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
        lastGreetTime = 0f;
        character.CancelNavigate();
    }
}
