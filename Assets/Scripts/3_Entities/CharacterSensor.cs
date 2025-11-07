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
        var trashBehavior = character.StateMachine.TrashBehaviour;
        var friends = character.Blackboard.Friends;
        if (other.GetComponent<Trash>() != null && trashBehavior == CityCharacterTrashBehaviour.PickUp)
        {
            var trash = other.GetComponent<Trash>();
            character.Blackboard.LastSeenTrash = trash;
        }
        
        else if(other.GetComponent<Character>() != null && friends.Contains(other.GetComponent<Character>()))
        {
            var friend = other.GetComponent<Character>();
            character.Blackboard.LastSeenFriend = friend;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Trash>() != null && character.Blackboard.LastSeenTrash != null)
        {
            character.Blackboard.LastSeenTrash = null;
        }
        else if (other.GetComponent<Character>() != null && character.Blackboard.LastSeenFriend != null)
        {
            character.Blackboard.LastSeenFriend = null;
        }
    }
}
