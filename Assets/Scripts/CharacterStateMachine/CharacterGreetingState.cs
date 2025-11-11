
using UnityEngine;

class CharacterGreetingState : ICharacterState
{
    private readonly Character character;
    private readonly Character friend;
    private bool started;
    public CharacterGreetingState(Character character, Character friend)
    {
        this.character = character;
        this.friend = friend;
    }
    public void Enter()
    {
        started = false;
        character.StateMachine.CurrentStateName = "Etat de saluer un ami";
    }
    public void Update()
    {
        if (friend == null || !friend.IsAvailable)
        {
            character.StateMachine.PopState();
            return;
        }

        

        if (!started)
        {
            started = true;
            character.GreetCharacter(friend);
            return;
        }

        
        if (!character.IsGreetingCharacter())
        {
            Debug.Log("Finished greeting");
            character.StateMachine.PopState();
        }
    }

    public void Exit()
    {
        
    }

    
}