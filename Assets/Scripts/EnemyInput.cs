using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInput : MonoBehaviour
{
    static string attackStringInputs = "PK";  
    static int numAttackStrings = 3;
    static int minAttackStringLength = 2;
    static int maxAttackString = 4;
    static float closeToPlayerRange = 3f; //Range at which player is considered "close"
    static float minBlockDuration = 0.5f;
    static float maxBlockDuration = 0.1f;
    static float minApproachTime = 0.2f;
    static float maxApproachTime = 0.5f;
    static float minRetreatTime = 0.75f;
    static float maxRetreatTime = 1.25f;
    static float minWaitTime = 0.5f;
    static float maxWaitTime = 1f;

    [Header("References")]
    public StateController controller;
    public Transform playerT;
    public List<AudioClip> comboStartLines;

    [Header("Status")]
    public List<string> attackStrings;
    public bool executingCombo;
    public int comboIdx; //How far along we are in current combo
    public int currCombo; //Combo we are executing
    public bool executingMiscAction;
    public int miscActionType; //0-High Block 1-Low Block 2-Walk Left 3-Walk Right 4-Wait
    public float actionTimer;
    public bool executingSimpleAction; 



    // Start is called before the first frame update

    void Start()
    {
        attackStrings = new List<string>();
        executingCombo = false;
        comboIdx = -1;
        currCombo = -1;
        executingMiscAction = false;
        miscActionType = -1;
        actionTimer = 0f;
        executingSimpleAction = false;

        if (controller == null) controller = GetComponent<StateController>();
        playerT = GameObject.Find("Player").transform;

        //Generate random attack strings
        for (int i = 0; i < numAttackStrings; i++) 
        {
            string attackString = "W";
            int stringLength = Random.Range(minAttackStringLength, maxAttackString + 1);
            for (int e = 0; e < stringLength; e++) 
            {
                int attackCharIdx = Random.Range(0, attackStringInputs.Length);
                char attack = attackStringInputs[attackCharIdx];
                attackString += attack;
                attackString += "W";
            }
            attackStrings.Add(attackString);

        }
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(controller.currState);
        if (executingCombo) //Series of Punch and Kick actions
        {
            if (controller.currState == CharacterState.Idle)
            {
                comboIdx++;
                if (comboIdx >= attackStrings[currCombo].Length)
                {
                    controller.SendCommand(InputCommand.None);
                    StartRetreat();
                    executingCombo = false;
                } 
                else
                {
                    char currAttack = attackStrings[currCombo][comboIdx];
                    if (currAttack == 'P') controller.SendCommand(InputCommand.Punch);
                    else if (currAttack == 'K') controller.SendCommand(InputCommand.Kick);
                    else controller.SendCommand(InputCommand.HighBlock); //Hack way of telling AI to wait
                }
            }
            else if (controller.currState == CharacterState.Stunned)
            {
                // Debug.Log("Combo over");
                executingCombo = false;
            }
            else if (controller.currState == CharacterState.HighBlock)
            {
                controller.SendCommand(InputCommand.None);
            }
        }
        else if (executingMiscAction) //Action with variable length execution (walking, blocking, waiting, etc.)
        {
            switch (miscActionType)
            {
                case 0:
                    controller.SendCommand(InputCommand.HighBlock);
                    break;
                case 1:
                    controller.SendCommand(InputCommand.LowBlock);
                    break;
                case 2:
                    controller.SendCommand(InputCommand.MoveLeft);
                    break;
                case 3:
                    controller.SendCommand(InputCommand.MoveRight);
                    break;
                case 4:
                    controller.SendCommand(InputCommand.None);
                    break;
            }
            actionTimer -= Time.deltaTime;
            if (actionTimer <= 0f || controller.currState == CharacterState.Stunned) 
            {
                executingMiscAction = false;
                controller.SendCommand(InputCommand.None);
            }
        }
        else if (executingSimpleAction) //Action with single input and fixed duration (fireball)
        {
            if (controller.currState == CharacterState.Stunned || controller.currState == CharacterState.Idle)
            {
                executingSimpleAction = false;
                controller.SendCommand(InputCommand.None);
            }
        }
        else //Currently idle, decide on an action.
        {
            float distFromPlayer = Mathf.Abs(playerT.transform.position.x - transform.position.x);
            if (distFromPlayer <= closeToPlayerRange) //Player is close!
            {
                // Debug.Log("player is close");
                //Either attack, block, or make a retreat.
                int decision = Random.Range(0, 10);
                if (decision <= 4)
                {
                    // Debug.Log("Starting combo");
                    StartCombo();
                }
                else if (decision <= 8)
                {
                    // Debug.Log("Starting block");
                    StartBlock();
                }
                else
                {
                    // Debug.Log("Starting retreat");
                    StartRetreat();
                }
            }
            else //Player is further away...
            {
                //Either approach, wait, or throw a fireball.
                int decision = Random.Range(0, 10);
                if (decision <= 5)
                {
                    // Debug.Log("Starting approach");
                    StartApproach();
                }
                else if (decision <= 8)
                {
                    // Debug.Log("Starting wait");
                    StartWait();
                }
                else
                {
                    // Debug.Log("Starting fireball");
                    StartFireball();
                }
            }
        }
    }

    void StartCombo() //Randomly pick combo, and raise flag for combo execution.
    {
        comboIdx = -1;
        currCombo = Random.Range(0, attackStrings.Count);
        SFXManager.PlayOpponent(comboStartLines[currCombo]);
        executingCombo = true;
    }

    void StartBlock() //Randomly pick high or low bloack, and randomly decide duration. Then raise flag for misc action.
    {
        int highOrLow = Random.Range(0, 2);
        if (highOrLow == 0)
        {
            miscActionType = 0;
        } 
        else 
        {
            miscActionType = 1;
        }
        executingMiscAction = true;
        actionTimer = Random.Range(minBlockDuration, maxBlockDuration);
    }

    void StartApproach() //Randomly decide duration, then raise flag for misc action.
    {
        executingMiscAction = true;
        actionTimer = Random.Range(minApproachTime, maxApproachTime);
        miscActionType = 2; 
    }

    void StartRetreat()
    {
        executingMiscAction = true;
        actionTimer = Random.Range(minRetreatTime, maxRetreatTime);
        miscActionType = 3;
    }

    void StartFireball()
    {
        executingSimpleAction = true;
        controller.SendCommand(InputCommand.FireBall);
    }

    void StartWait()
    {
        executingMiscAction = true;
        actionTimer = Random.Range(minWaitTime, maxWaitTime);
        miscActionType = 4;
    }
}
