using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingPlayer : MonoBehaviour
{
    public float speed = 10.0f;
    public float maxVelocity = 50.0f;
    public float gravityScale = 1.0f;

    public float fuel = 100.0f;
    public float maxFuel = 100.0f;
    public float fuelDepletionRate = 10.0f;

    public float powerUpFuelIncrease = 25.0f;
    public float powerUpDuration = 5.0f;

    private Rigidbody2D rb;
    private bool isFlying = false;
    private float powerUpTimeLeft = 0.0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0.0f;
        rb.freezeRotation = true;
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump") && fuel > 0.0f)
        {
            isFlying = true;
        }

        if (Input.GetButtonUp("Jump"))
        {
            isFlying = false;
        }

        if (isFlying)
        {
            rb.AddForce(transform.up * speed, ForceMode2D.Force);
            fuel = Mathf.Max(0.0f, fuel - fuelDepletionRate * Time.deltaTime);
        }

        if (powerUpTimeLeft > 0.0f)
        {
            powerUpTimeLeft -= Time.deltaTime;
            fuel = Mathf.Min(fuel + powerUpFuelIncrease * Time.deltaTime, maxFuel);
        }
    }

    void FixedUpdate()
    {
        Vector2 moveDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        moveDirection *= speed;

        rb.AddForce(moveDirection, ForceMode2D.Force);

        if (rb.velocity.magnitude > maxVelocity)
        {
            rb.velocity = rb.velocity.normalized * maxVelocity;
        }

        rb.AddForce(Vector2.down * gravityScale, ForceMode2D.Force);
    }

    public void PowerUpCollected()
    {
        powerUpTimeLeft = powerUpDuration;
    }

    public void Refuel(float amount)
    {
        fuel = Mathf.Min(fuel + amount, maxFuel);
    }

    public bool IsFlying()
    {
        return isFlying;
    }

    public float GetFuelPercentage()
    {
        return fuel / maxFuel;
    }
}

