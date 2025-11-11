using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

internal class CharacterPickupTrashState : ICharacterState
{
    private readonly Trash trash;
    private readonly Character character;

    private bool started;
    public CharacterPickupTrashState(Trash trash, Character character)
    {
        this.trash = trash;
        this.character = character;
    }

    public void Enter()
    {
        started = false;
        if (trash != null && !character.IsCloseTo(trash))
        {
            character.StateMachine.PushState(new CharacterTravelState(trash, character));
        }
        character.StateMachine.CurrentStateName = "Etat de ramasser des déchets";
    }

    public void Update()
    {
        if (trash == null || !trash.gameObject.activeInHierarchy)
        {
            character.StateMachine.PopState();
            return;
        }

        if (!character.IsCloseTo(trash))
            return; 

        if (!started)
        {
            started = true;
            character.PickUpTrash(trash);
            return;
        }

        if (!character.IsPickingTrash())
        {
            character.StateMachine.PopState();
        }
    }


    public void Exit()
    {

    }


}
