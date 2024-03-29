using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.UI;

public class HPHandler : NetworkBehaviour
{
    [Networked(OnChanged = nameof(OnHPChanged))]

    byte HP { get; set; }

    [Networked(OnChanged = nameof(OnStateChanged))]

    public bool isDead { get; set; }

    bool isInitialized = false;

    const byte startingHP = 5;

    public Color uiOnHitColor;

    //Image red to represent damage to player
    public Image uiOnHitImage;

    //Represent damage to clients
    public MeshRenderer bodyMeshRenderer;
    Color defaultMeshBodyColor;

    public GameObject playerModel;
    public GameObject deathGameObjectPrefab;

    HitboxRoot hitboxRoot;

    CharacterMovementHandler characterMovementHandler;

    private void Awake()
    {
        characterMovementHandler = GetComponent<CharacterMovementHandler>();
        hitboxRoot = GetComponentInChildren<HitboxRoot>();
    }

    void Start()
    {
        HP = startingHP;
        isDead = false;

        defaultMeshBodyColor = bodyMeshRenderer.material.color;

        isInitialized = true;
    }

    IEnumerator OnHitCO()
    {
        bodyMeshRenderer.material.color = Color.white;

        if (Object.HasInputAuthority)
            uiOnHitImage.color = uiOnHitColor;

        yield return new WaitForSeconds(0.2f);

        bodyMeshRenderer.material.color = defaultMeshBodyColor;

        if (Object.HasInputAuthority && !isDead)
            uiOnHitImage.color = new Color(0, 0, 0, 0);
    }

    IEnumerator ServerReviveCO()
    {
        yield return new WaitForSeconds(2.0f);

        characterMovementHandler.RequestRespawn();
    }

    public void OnTakeDamage()
    {
        if (isDead)
            return;

        HP -= 1;

        Debug.Log($"{Time.time} {transform.name} took damage got {HP} left");

        if (HP <= 0)
        {
            Debug.Log($"{Time.time} {transform.name} died");

            StartCoroutine(ServerReviveCO());

            isDead = true;
        }
    }

    static void OnHPChanged(Changed<HPHandler> changed)
    {
        Debug.Log($"{Time.time} OnHPChanged value {changed.Behaviour.HP}");

        byte newHP = changed.Behaviour.HP;

        // load the old value
        changed.LoadOld();

        byte oldHP  = changed.Behaviour.HP;

        if (newHP < oldHP)
            changed.Behaviour.OnHPReduced();
    }

    

    private void OnHPReduced()
    {
        if (!isInitialized)
            return;

        StartCoroutine(OnHitCO());
    }
    static void OnStateChanged(Changed<HPHandler> changed)
    {
        Debug.Log($"{Time.time} OnStateChanged value {changed.Behaviour.isDead}");

        bool isDeadCurrent = changed.Behaviour.isDead;

        changed.LoadOld();

        bool isDeadOld = changed.Behaviour.isDead;

        if (isDeadCurrent)
            changed.Behaviour.OnDeath();
        else if (!isDeadCurrent && isDeadOld)
            changed.Behaviour.OnRevive();
    }

    private void OnDeath()
    {
        Debug.Log($"{Time.time} OnDeath");

        playerModel.gameObject.SetActive(false);
        hitboxRoot.HitboxRootActive = false;
        characterMovementHandler.SetCharacterControllerEnabled(false);

        Instantiate(deathGameObjectPrefab, transform.position, Quaternion.identity);
    }

    private void OnRevive()
    {
        Debug.Log($"{Time.time} OnRevive");

        if (Object.HasInputAuthority)
            uiOnHitImage.color = new Color(0, 0, 0, 0);

        playerModel.gameObject.SetActive(true);
        hitboxRoot.HitboxRootActive = true;
        characterMovementHandler.SetCharacterControllerEnabled(true);
    }

    public void OnRespawned()
    {
        //Reset variables
        HP = startingHP;
        isDead = false;
    }
}
