using System.Linq;
using UnityEngine;

public class CharacterSensor : MonoBehaviour
{
    private Character character;
    private void Awake()
    {
        character = GetComponent<Character>();
    }

    private void OnTriggerEnter(Collider other)
    {
        var trash = GetComponent<Trash>();
        var friend = other.GetComponent<Character>();

        var trashBehavior = character.StateMachine.TrashBehaviour;
        var friends = character.Blackboard.Friends;

        if (trash != null && trashBehavior == CityCharacterTrashBehaviour.PickUp)
        {
            character.Blackboard.LastSeenTrash = trash;
        }
        else if(friend != null && friends.Contains(friend))
        {
            character.Blackboard.LastSeenFriend = friend;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var trash = GetComponent<Trash>();
        var friend = other.GetComponent<Character>();

        if (character.Blackboard.LastSeenTrash == trash)
        {
            character.Blackboard.LastSeenTrash = null;
        }
        else if (character.Blackboard.LastSeenFriend == friend)
        {
            character.Blackboard.LastSeenFriend = null;
        }
    }
}
