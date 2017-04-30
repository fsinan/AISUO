using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {

    public GameObject gameManager;
	public Image armorBar;
	public Text armorText;
	public Image healthBar;
	public Text healthText;
	public int armor;
	public int maxArmor = 100;
	public int health;
	public int maxHealth = 100;


	// Use this for initialization
	void Start () 
	{
		health = maxHealth;
        armor = 0;
	}
	
	// Update is called once per frame
	void Update () 
	{
		SetUI ();
	}

	void SetUI()
	{
		armorBar.fillAmount = (float) Mathf.Clamp (armor, 0, maxArmor) / maxArmor;
		armorText.text = Mathf.Clamp (armor, 0, maxArmor).ToString();
		healthBar.fillAmount = (float) Mathf.Clamp (health, 0, maxHealth) / maxHealth;
		healthText.text = Mathf.Clamp (health, 0, maxHealth).ToString();
	}

    public void TakeDamage(int amount)
    {
        if (armor > 0)
        {
            armor -= amount;

            if (armor < 0)
                health += armor;
        }
        else
        {
            health -= amount;
        }

        if(health <= 0)
        {
            GameOver();
        }
    }

    public void AddArmor(int amount)
    {
        armor = Mathf.Clamp(armor + amount, 0, maxArmor);
    }

    void GameOver()
    {
        gameManager.GetComponent<GameManager>().GameOver();
    }
}
