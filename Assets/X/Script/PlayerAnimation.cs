using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    Animator am;
    Player Pl;
    SpriteRenderer sr;
    string currentAnimation = " ";
    void Start()
    {
        am = GetComponent<Animator>();
        Pl = GetComponent<Player>();
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
      SpriteDirectionChecker();
    }
    void SpriteDirectionChecker()
    {
        if (Pl.motion.x < 0)
        {
            sr.flipX = true;
        }
        else
        {
            sr.flipX = false;
        }
    }
    public void ChangeAnimation(string animation)
    {
        if (currentAnimation != animation)
        {
            am.Play(currentAnimation);
        }

    }
}
