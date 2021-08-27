using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseStructure : MonoBehaviour
{
    public float HP = 100f;

    float currentHealth;
    bool _isDead;
    public bool isDead
    {
        get { return _isDead; }
        set
        {
            _isDead = value;
            enabled = !value;
        }
    }
    // GameObject modelObj;

    // Start is called before the first frame update
    void Start()
    {
        // Set HP
        currentHealth = HP;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeDamage(float damage)
    {
        if (isDead)
        {
            return;
        }

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        var model = transform.GetChild(0).gameObject;
        Destroy(model);
    }

}
