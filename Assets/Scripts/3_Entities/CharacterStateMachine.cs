using System.Collections.Generic;
using UnityEngine;

//
// Il s'agit de la classe dans laquelle vous allez implémenter la machine à état. Vous aurez tout particulièrement à
// compléter la méthode "UpdateStateMachine" (et peut-être aussi ajouter des méthodes "PushState" et "PopState").
//
// Notez que c'est aussi ici que les personnages "malpropres" sont gérés. Pour les personnages "malpropre", cette classe
// contient une routine qui, de temps en temps, met le booléen "shouldThrowTrash" du CharacterBlackboard à "true". Dans
// votre machine à état, cela signifie qu'il est temps de changer d'état pour lancer un déchet.
//
// Au cas où ce n'était pas encore clair, vous avez à modifier cette classe pour le travail.
public class CharacterStateMachine : MonoBehaviour
{
    [Header("Behaviour")]
    [SerializeField] private CityCharacterTrashBehaviour trashBehaviour = CityCharacterTrashBehaviour.Ignore;
    [SerializeField, Range(0, 100)] private float throwTrashChances = 5f;
    [SerializeField, Min(0)] private float throwTrashCheckDelay = 1f;

    private Character character;
    private float throwTrashCheckTimer;
    private Stack<ICharacterState> stateStack;

    public string CurrentStateName { get; set; }
    public CityCharacterTrashBehaviour TrashBehaviour => trashBehaviour;

    private void Awake()
    {
        // Get dependencies.
        character = GetComponent<Character>();

        // Init timers.
        throwTrashCheckTimer = 0;
        
        // Init state machine.
        stateStack = new Stack<ICharacterState>();
        stateStack.Push(new CharacterWorkState(character));
    }
    private void Start()
    {
        stateStack.Peek().Enter();
    }

    private void Update()
    {
        UpdateTimers();
        UpdateStateMachine();
    }

    private void UpdateTimers()
    {
        if (trashBehaviour == CityCharacterTrashBehaviour.Throw)
        {
            throwTrashCheckTimer += Time.deltaTime;
            if (throwTrashCheckTimer >= throwTrashCheckDelay)
            {
                character.Blackboard.ShouldThrowTrash = RandomUtils.Chance(throwTrashChances);
                throwTrashCheckTimer = 0f;
            }
        }
        else
        {
            throwTrashCheckTimer = 0f;
        }
    }

    private void UpdateStateMachine()
    {
        stateStack.Peek().Update();

        if (stateStack.Count == 0) return;
    }

    public void PushState(ICharacterState state)
    {
        stateStack.Peek().Exit();
        stateStack.Push(state);
        stateStack.Peek().Enter();
    }

    public void PopState()
    {
        stateStack.Peek().Exit();
        stateStack.Pop();
        stateStack.Peek().Enter();
    }
}

public enum CityCharacterTrashBehaviour
{
    Ignore,
    PickUp,
    Throw
}