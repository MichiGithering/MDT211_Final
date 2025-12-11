using UnityEngine;

public class Player : Character
{
    private Vector2 input;

    public int level = 1;
    public int exp = 0;
    public int lvbar = 100;

    void Update()
    {
        // Handle movement input
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        // Read EXP & Level
        if (ScoreManager.Instance != null)
            exp = ScoreManager.Instance.Score;
        if (exp >= lvbar)
            LevelUp();
    }

    void FixedUpdate()
    {
        Move(input);
    }

    public override void AttackTarget(Character target)
    {
        throw new System.NotImplementedException();
    }

    public void LevelUp()
    {
        level++;

        ScoreManager.Instance.ResetScore();
        exp = 0;

        lvbar = 100 + (10 * (level - 1));
    }
}
