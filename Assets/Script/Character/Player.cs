using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Player : Character
{
    private Vector2 input;

    public int exp = 0;
    public int lvbar = 100;

    void Update()
    {
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

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
        level++; // This works because 'level' is in the parent script

        if (ScoreManager.Instance != null)
            ScoreManager.Instance.ResetScore();

        exp = 0;
        lvbar = 100 + (10 * (level - 1));
    }
}