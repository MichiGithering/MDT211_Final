using UnityEngine;

public class Player : Character
{
    private Vector2 input;


    void Update()
    {
        // Get 2D input (WASD / Arrow Keys)
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    void FixedUpdate()
    {
        // Call Character.Move using input
        Move(input);
    }

    public override void AttackTarget(Character target)
    {
        throw new System.NotImplementedException();
    }
}

