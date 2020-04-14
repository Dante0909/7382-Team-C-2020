﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private float maxHp = 100;
    private float maxLives = 3;
    [SerializeField]private float currentHp;
    [SerializeField]private float currentLives;
    private float maxStamina = 5;
    public float currentStamina;
    [SerializeField] private float meleeAttack = 10;
    //private float meleeStrongAttack = 35;
    private float typeAdvantage = 1.5f;
    private float typeDisadvantage = 0.5f;
    //public HealthBar healthBar;
    [SerializeField] private float staminaRegen;
    [SerializeField] private float damageDelay;
    private bool damageReady = true;
    //public Image effect;

    private AsyncOperation async;
    private FixedVariables variables;
    private UiChaos ui;
    //private float rangeLightAttack = 15;
    //private float rangeStrongAttack = 20;
    //private float rangeShieldDamage = 10;
    //private float rangeStrongShieldDamage = 20;
    //[SerializeField] private Animator anim;


    public static GameManager instance = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        variables = FixedVariables.instance;
        maxHp = 100 * Mathf.Pow(1.25f, variables.HealthUpgrade);
        maxStamina = 5 + variables.StaminaUpgrade;
        ui = UiChaos.instance;
        currentHp = maxHp;
        ui.PlayerHp(currentHp);
        currentLives = maxLives;
        
        currentStamina = maxStamina;
        ui.Stamina(currentStamina);
        StartCoroutine(StaminaRegen());
    }
    [SerializeField]private int isAttacking = 0;

    public float MeleeAttack { get => meleeAttack; set => meleeAttack = value; }

    private IEnumerator StaminaRegen()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f + maxStamina / (5 + variables.StaminaUpgrade));
            if(isAttacking <= 0 && currentStamina < maxStamina)
            {
                ++currentStamina;
                ui.Stamina(currentStamina);
            }

        }
        
    }
    public void Attack()
    {
        if(Time.timeScale != 0)
        {
            --currentStamina;
            ++isAttacking;
            Invoke("ResetAttack", 1f);
        }
        ui.Stamina(currentStamina);
        
        
    }
    private void ResetAttack()
    {
        --isAttacking;
    }
    // Update is called once per frame
    void Update()
    {
        //Attack(); // this is not necessary with the input system now implemented
        HpCheck();
        //if(currentStamina <= maxStamina && !isAttacking)
        //{
        //    currentStamina += Time.deltaTime * staminaRegen;
        //}
        //ui.Stamina(currentStamina);
        //if (test == 0) effect.enabled = false;
        //else effect.enabled = true;
    }
    
    private void HpCheck()
    {
        if (currentHp <= 0)
        {
            if(async == null)
            {
                if(variables != null) variables.LastScene = SceneManager.GetActiveScene().name;
                PlayerPrefs.SetString("SceneToLoad", SceneManager.GetActiveScene().name);
                
                async = SceneManager.LoadSceneAsync("Loading");
                async.allowSceneActivation = true;
            }
        }
        if (currentLives <= 0)
        {
            //Restart();
        }
    }
    private void HpReset()
    {
        --currentLives;
        currentHp = maxHp;
    }
    private void Restart()
    {
        Debug.Log("Scene Restart");
    }
    private int test = 0;
    public void GetHit(float damage)
    {
        if(damage < 0)
        {
            currentHp = maxHp;
        }
        else if (damageReady)
        {
            currentHp -= damage;
            ui.PlayerHp(currentHp);
            damageReady = false;
            Invoke("DamageReady", damageDelay);
        }
        
        //++test;
        ////anim.SetInteger("takingDamage", test);
        //Invoke("DisableEffect", 1f);
    }
    private void DamageReady() { damageReady = true; }
    //private void DisableEffect()
    //{
    //    if (test > 0) --test;
    //    //anim.SetInteger("takingDamage", test);
    //}
}
