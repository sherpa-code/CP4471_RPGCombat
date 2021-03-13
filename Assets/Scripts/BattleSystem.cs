﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState
{
    START,
    PLAYERTURN,
    ENEMYTURN,
    WON,
    LOST
}

public class BattleSystem : MonoBehaviour
{

    public GameObject allyPrefab;
    public GameObject enemyPrefab;

    public Transform allySpawnTransform;
    public Transform enemySpawnTransform;

    Monster allymonster;
    Monster enemymonster;

    public Image playerActions;

    public Image combatReadout;
    public Text dialogueText;

    public BattleState battleState;

    public BattleHUDScript allyHUD;
    public BattleHUDScript enemyHUD;



    // Start is called before the first frame update
    void Start()
    {
        battleState = BattleState.START;
        //SetupBattle()

        //StartCoroutine(SetupBattle());
    }



    //void SetupBattle()
    IEnumerator SetupBattle()
    {
        //GameObject allyGameObject = Instantiate(allyPrefab, allySpawnTransform);
        GameObject allyGameObject = Instantiate(allyPrefab, allySpawnTransform.position, allySpawnTransform.rotation);
        allymonster = allyGameObject.GetComponent<Monster>();

        //GameObject enemyGameObject = Instantiate(enemyPrefab, enemySpawnTransform);
        GameObject enemyGameObject = Instantiate(enemyPrefab, enemySpawnTransform.position, enemySpawnTransform.rotation);
        enemymonster = enemyGameObject.GetComponent<Monster>();

        dialogueText.text = "A wild " + enemymonster.monsterName + " approaches!";

        allyHUD.SetHUD(allymonster);
        enemyHUD.SetHUD(enemymonster);

        //yield return new WaitForSeconds(2f);
        yield return new WaitForSeconds(0f); // debug setting for instant state change

        battleState = BattleState.PLAYERTURN;
        PlayerTurn();
    }

    void PlayerTurn()
    {

        //dialogueText.text = "Choose an action:";
        combatReadout.gameObject.SetActive(false); // debug setting
        playerActions.gameObject.SetActive(true); // debug setting
    }

    //IEnumerator PlayerTurn()
    //{
    //    yield return new WaitForSeconds(0f);
    //    //dialogueText.text = "Choose an action:";
    //    combatReadout.gameObject.SetActive(false); // debug setting
    //    playerActions.gameObject.SetActive(true); // debug setting
    //}



    public void OnMeleeButton()
    {
        Debug.Log("Melee Button clicked");
        if (battleState != BattleState.PLAYERTURN) return;

        StartCoroutine(PlayerAttack());
    }

    public void OnSpecialButton()
    {
        Debug.Log("Special Button clicked");
        if (battleState != BattleState.PLAYERTURN) return;

        StartCoroutine(PlayerSpecialAbility());
    }

    public void OnDefendButton()
    {
        Debug.Log("Defend Button clicked");
        if (battleState != BattleState.PLAYERTURN) return;

        StartCoroutine(PlayerDefend());
    }

    public void OnItemButton()
    {
        Debug.Log("Items Button clicked");
        if (battleState != BattleState.PLAYERTURN) return;

        StartCoroutine(PlayerItems());
    }


    IEnumerator PlayerAttack()
    {
        playerActions.gameObject.SetActive(false); // debug setting
        combatReadout.gameObject.SetActive(true); // debug setting
        dialogueText.text = allymonster.monsterName+" tried to melee attack...";
        yield return new WaitForSeconds(2f);

        // Damage enemy and check if dead
        bool isDead = enemymonster.TakeDamage(allymonster.attack);

        enemyHUD.SetHP(enemymonster.currentHP);
        dialogueText.text = "The attack was successful!";

        yield return new WaitForSeconds(2f);

        if (isDead)
        {
            // check for remaining monsters
            // if monsters remaining: (if team length > 1)
            //      send out another (TODO: replace with a way to select from remaining)
            //      proceed to enemy turn
            //      state = BattleState.ENEMYTURN;
            // else:
            //      end battle
            //      state = BattleState.WON;
            
            battleState = BattleState.WON;
            EndBattle();

        }
        else
        {
            //      proceed to enemy turn
            //      state = BattleState.ENEMYTURN;
            battleState = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }

    }

    void EndBattle()
    {
        playerActions.gameObject.SetActive(false);
        combatReadout.gameObject.SetActive(true);
        if (battleState == BattleState.WON)
        {
            dialogueText.text = "You have defeated all opponents. You win!";
        } else if (battleState == BattleState.LOST)
        {
            dialogueText.text = "You have no remaining monsters. You lose!";
        }
    }

    IEnumerator EnemyTurn()
    {
        playerActions.gameObject.SetActive(false);
        combatReadout.gameObject.SetActive(true);
        dialogueText.text = enemymonster.monsterName+" attacks...";

        yield return new WaitForSeconds(1f);

        bool isDead = allymonster.TakeDamage(enemymonster.attack);

        allyHUD.SetHP(allymonster.currentHP);

        yield return new WaitForSeconds(1f);

        if (isDead)
        {
            // check for remaining monsters
            // if ally monsters remaining: (if team length > 1)
            //      send out another (TODO: replace with a way to select from remaining)
            //      proceed to player turn
            //      state = BattleState.ENEMYTURN;
            // else:
            //      end battle
            //      state = BattleState.WON;

            battleState = BattleState.WON;
            EndBattle();

            battleState = BattleState.LOST;
            EndBattle();
        } else
        {
            //      proceed to enemy turn
            //      state = BattleState.ENEMYTURN;
            battleState = BattleState.PLAYERTURN;
            //StartCoroutine(PlayerTurn());
            PlayerTurn();
        }
    }

    IEnumerator PlayerSpecialAbility()
    {
        playerActions.gameObject.SetActive(false); // debug setting
        combatReadout.gameObject.SetActive(true); // debug setting

        dialogueText.text = allymonster.monsterName + " tried to heal...";
        yield return new WaitForSeconds(2f);

        allymonster.Heal(1);
        allyHUD.SetHP(allymonster.currentHP);
        dialogueText.text = "The heal was successful!";

        yield return new WaitForSeconds(2f);
        
        StartCoroutine(EnemyTurn());

    }

    

    IEnumerator PlayerDefend()
    {
        // Damage enemy
        yield return new WaitForSeconds(2f);

        // Set Defense state
        // Change state based on above result
    }

    IEnumerator PlayerItems()
    {
        // Damage enemy
        yield return new WaitForSeconds(2f);

        // Hide player actions window
        // Show items list window
        
        // Wait for user to click an item or cancel
        // if clicked an item, show confirm window
            // if clicked Yes, hide Confirm window and use item
            // if clicked No, hide Confirm window
        // if clicked cancel hide items list window and show player actions window
    }
}
