using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public enum EnemyStates{Attack, Chase, Patrol, Dying};

public class EnemyBehavior : MonoBehaviour {

	NavMeshAgent agent;

    private float shootTimer;
	private GameObject player;
	private Animator animator;
    private LineRenderer lineRenderer;
	private SkinnedMeshRenderer[] skinnedMeshRenderers;
    private Transform lineShooter;
    private Light shootLight;

    public Transform target;

	[Header("Health")]
	public Image healthBar;
	public int health;
	public int maxHealth = 100;

	[Header("States")]
	public bool isDying;
	public bool isRunning;
	public float minVelocity = 0.001f;
	public EnemyStates state;

	[Header("Sight")]
	public float seeingDistance = 50f;
	public float attackDistance = 3f;

    [Header("Attack")]
    public float attackInterval = 0.5f;
    public int damageDeals = 10;
    public float effectTimeProportion = 0.2f;

    [Header("Misc")]
    public int worthPoints = 10;
    public float walkRadius = 10f;

    [Range(0f, 1f)]
    public float chanceToDropArmor = 0.1f;
    public GameObject armorDropPrefab;

    // Use this for initialization
    void Start () 
	{
		agent = GetComponent<NavMeshAgent> ();
		animator = GetComponent<Animator> ();
		skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer> ();
        lineRenderer = GetComponent<LineRenderer>();
        player = GameObject.FindGameObjectWithTag ("Player");
        lineShooter = transform.FindChild("LineShooter");
        shootLight = transform.FindChild("ShootLight").gameObject.GetComponent<Light>();

        shootTimer = 0f;
		state = EnemyStates.Patrol;
		health = 100;
		isDying = false;
		isRunning = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
        if (!GameManager.isGamePaused && !isDying)
        {
            shootTimer += Time.deltaTime;

            // If we can see the player, then chase
            // If we are close enough, attack
            // Else, activate patrol mode
            if (CanSeePlayer())
            {
                target = player.transform;

                // if can see but not in the attack range
                if (Vector3.Distance(target.position, transform.position) > attackDistance)
                {
                    state = EnemyStates.Chase;
                    SetDestination(target.position);
                }
                else
                {
                    state = EnemyStates.Attack;
                    SetDestination(target.position);
                }
            }
            else
            {
                state = EnemyStates.Patrol;

                // Go to a random point on the NavMesh

                Vector3 randomDirection = Random.insideUnitSphere * walkRadius;

                randomDirection += transform.position;

                NavMeshHit hit;
                NavMesh.SamplePosition(randomDirection, out hit, walkRadius, NavMesh.AllAreas);
                
                if(agent.velocity.magnitude < minVelocity)
                    SetDestination(hit.position);
            }

            // If it is running with a reasonable speed, transition to Run animation
            // or stop
            if (agent.velocity.magnitude < minVelocity)
            {
                animator.SetBool("isRunning", false);
                isRunning = false;
            }
            else
            {
                animator.SetBool("isRunning", true);
                isRunning = true;
            }

            // Set animation speed using the velocity
            animator.SetFloat("velocity", agent.velocity.magnitude);

            if (state == EnemyStates.Attack && shootTimer >= attackInterval)
            {
                Attack();
                shootTimer = 0f;
            }

            if (shootTimer >= attackInterval * effectTimeProportion)
            {
                DisableEffects();
            }
        }
        else
        {
            shootTimer += Time.deltaTime;
            // if it is dying then we should slowly make it disappear
            Disappear();

            if (shootTimer >= attackInterval * effectTimeProportion)
            {
                DisableEffects();
            }
        }
    }

	void SetDestination(Vector3 destPosition)
	{
		agent.destination = destPosition;
		isRunning = true;
	}

	public void TakeDamage(int amount)
	{
		health -= amount;

		// Update the floating health bar
		healthBar.fillAmount = (float) Mathf.Clamp (health, 0, maxHealth) / maxHealth;

		// If no health left
		if (health <= 0) 
		{
			Die ();
            GameManager.IncreaseScore(worthPoints);
		}
	}

	void Die()
	{
		// Set the bool
		isDying = true;
		state = EnemyStates.Dying;
		// Set the animator parameter for transitioning
		animator.SetBool ("isDying", true);
		// Stop moving
		agent.Stop();
		// Dead enemies shouldn't have physical existence
		GetComponent<CapsuleCollider>().enabled = false;
        GetComponent<NavMeshAgent>().enabled = false;

        GameManager.enemiesAlive--;
        GameManager.SetEnemiesText();

        if(Random.Range(0f, 1f) <= chanceToDropArmor)
        {
            Instantiate(armorDropPrefab, transform.position + Vector3.up * 0.7f, armorDropPrefab.transform.rotation);
        }
	}

	void Disappear()
	{
		// Iterate through all the meshes (children) and make them transparent
		foreach(var rend in skinnedMeshRenderers)
		{
			// Need to enable alpha transparency before disappearing
			if (!rend.material.IsKeywordEnabled ("_ALPHAPREMULTIPLY_ON")) 
			{
				rend.material.EnableKeyword ("_ALPHAPREMULTIPLY_ON");
				rend.material.SetFloat("_Mode", 3);
				rend.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
				rend.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
				rend.material.SetInt("_ZWrite", 0);
				rend.material.DisableKeyword("_ALPHATEST_ON");
				rend.material.DisableKeyword("_ALPHABLEND_ON");
				rend.material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
				rend.material.renderQueue = 3000;
			}

			// Now decrease the alpha
			Color color = rend.material.color;

			if (color.a > 0f) 
			{
				color.a -= 0.5f * Time.deltaTime;
				rend.material.color = color;
			} 
			else 
			{
				// This means it is dead enough
				// Now destroy the object
				Destroy (gameObject);
			}
		}
	}

	bool CanSeePlayer()
	{
        Ray ray = new Ray(lineShooter.position, player.transform.position + Vector3.up - lineShooter.position);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo, seeingDistance))
        {
            if (hitInfo.transform.tag == "Player")
                return true;
        }

        return false;
    }

    void Attack()
    {
        SetRayLine(lineShooter.position, player.transform.position + Vector3.up);
        player.GetComponent<PlayerHealth>().TakeDamage(damageDeals);
    }

    void SetRayLine(Vector3 start, Vector3 end)
    {
        Vector3[] positions = new Vector3[2];

        positions[0] = start;
        positions[1] = end;

        lineRenderer.SetPositions(positions);
        lineRenderer.enabled = true;
        shootLight.enabled = true;
    }

    void DisableEffects()
    {
        lineRenderer.enabled = false;
        shootLight.enabled = false;
    }
}
