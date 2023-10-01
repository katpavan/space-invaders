using UnityEngine;

public class Invader : MonoBehaviour
{
    public Sprite[] animationSprites;
    public float animationTime;

    private SpriteRenderer _spriteRenderer;
    private int _animationFrame;
    public System.Action killedInvader;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        InvokeRepeating(nameof(AnimateSprite), this.animationTime, this.animationTime);
    }

    private void AnimateSprite()
    {
        _animationFrame++;

        if(_animationFrame >= this.animationSprites.Length)
        {
            _animationFrame = 0;
        }

        _spriteRenderer.sprite = this.animationSprites[_animationFrame];
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Laser")) {
            //we need to inform that the laser hit an invader from the Invader.cs script to the Invaders.cs script, because as invaders are killed, we want to speed up the invaders. Also when all the invaders are killed, we want the invaders script to trigger a game reset
            this.killedInvader.Invoke();
            this.gameObject.SetActive(false);
        }
    }

}
