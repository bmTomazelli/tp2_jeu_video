internal class CharacterPickupTrashState : ICharacterState
{
    private readonly Trash trash;
    private readonly Character character;
    public CharacterPickupTrashState(Trash trash, Character character)
    {
        this.trash = trash;
        this.character = character;
    }

    public void Enter()
    {

    }

    public void Exit()
    {

    }

    public void Update()
    {

    }
}
