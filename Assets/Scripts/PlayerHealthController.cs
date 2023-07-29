using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerHealthController : MonoBehaviour
{
    // change sprite based on hp
    public Sprite[] currentSprite;

    public static PlayerHealthController instance;

    public int currentHealth;
    public int maxHealth;

    // invincible after being hit
    public float damageInvincLength = 1f;
    private float invincCount;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;

        UIController.instance.healthSlider.maxValue = maxHealth;
        UIController.instance.healthSlider.value = currentHealth;
        UIController.instance.healthText.text = currentHealth.ToString() + " / " + maxHealth.ToString();

    }

    // Update is called once per frame
    void Update()
    {
        if(invincCount > 0)
        {
            invincCount -= Time.deltaTime;
            if(invincCount <= 0)
            {
                PlayerController.instance.bodySR.color = new Color(PlayerController.instance.bodySR.color.r, PlayerController.instance.bodySR.color.g, PlayerController.instance.bodySR.color.b, 1f);
            }
        }

        

    }

    public void DamagePlayer()
    {

        // no damage if invinc
        if(invincCount <= 0)
        {
            AudioManager.instance.PlaySFX(11);
            currentHealth--;
            

            // start invinc countdown and show visual feedback
            invincCount = damageInvincLength;
            // color(r,g,b,a)
            PlayerController.instance.bodySR.color = new Color(PlayerController.instance.bodySR.color.r, PlayerController.instance.bodySR.color.g, PlayerController.instance.bodySR.color.b, .5f);
            if (currentHealth <= 0)
            {
                PlayerController.instance.gameObject.SetActive(false);

                UIController.instance.deathScreen.SetActive(true);

                AudioManager.instance.PlayGameOver();
                AudioManager.instance.PlaySFX(24);
            }

            UIController.instance.healthSlider.value = currentHealth;
            UIController.instance.healthText.text = currentHealth.ToString() + " / " + maxHealth.ToString();

            // balding----------------------------------------------------
            if (currentHealth <= 5)
            {
                PlayerController.instance.bodySR.sprite = currentSprite[currentHealth];
            }
            
        }
    }

    public void MakeInvincible(float length)
    {
        invincCount = length;
        PlayerController.instance.bodySR.color = new Color(PlayerController.instance.bodySR.color.r, PlayerController.instance.bodySR.color.g, PlayerController.instance.bodySR.color.b, .7f);
    }

    public void HealPlayer(int healAmount)
    {
        currentHealth += healAmount;
        if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        UIController.instance.healthSlider.value = currentHealth;
        UIController.instance.healthText.text = currentHealth.ToString() + " / " + maxHealth.ToString();

        //balding------------------------------------------------------------------------------------------------------
        if (currentHealth <= 5)
        {
            PlayerController.instance.bodySR.sprite = currentSprite[currentHealth];
        }
        else
        {
            PlayerController.instance.bodySR.sprite = currentSprite[currentSprite.Length - 1];
        }
    }


    public void IncreaseMaxHealth(int amount)
    {
        maxHealth += amount;
        //currentHealth = maxHealth;

        // update ui healthbar
        UIController.instance.healthSlider.maxValue = maxHealth;

        UIController.instance.healthSlider.value = currentHealth;
        UIController.instance.healthText.text = currentHealth.ToString() + " / " + maxHealth.ToString();
    }


}
