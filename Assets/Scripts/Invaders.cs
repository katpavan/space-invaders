using UnityEngine;
using UnityEngine.SceneManagement;
public class Invaders : MonoBehaviour
{
    public Invader[] prefabs;
    public int rows = 5;
    public int cols = 11;
    private Vector3 _direction = Vector2.right;
 
    public AnimationCurve speedCurve; //it's basically an x y graph
    public Projectile misslePrefab;
    public int invadersKilled { get; private set; } //public getter, private setter. meaning any method can get this but only this script can set it
    public int totalInvaders => this.rows * this.cols; //this is how we set a calculated variable
    public float percentKilled => (float)this.invadersKilled / (float)this.totalInvaders;
    public int amountAlive => totalInvaders - invadersKilled;
    public float missleAttackRate = 1.0f;
    private void Awake()
    {   
        //make a grid of invaders
        //I need to play around with this to fully understand what's going on
        for (int row = 0; row < this.rows; row++)
        {
            float width = 2.0f * (this.cols - 1);
            float height = 2.0f * (this.rows - 1);
            Vector2 centering = new Vector2(-width / 2, -height / 2);
            Vector3 rowPosition = new Vector3(centering.x, centering.y + row * 2.0f, 0.0f);

            for (int col = 0; col < this.cols; col++)
            {
                Invader invader = Instantiate(this.prefabs[row], this.transform); //this.transform refers to this game object's transform (the grid of invaders)
                invader.killedInvader += IncrementInvadersKilled; //we call IncrementInvadersKilled when the killedInvader delegate action gets invoked from Invader.cs
                Vector3 position = rowPosition;
                position.x += col * 2.0f; //2 as a guess for the spacing between
                invader.transform.localPosition = position; //we do invader.transform.localPosition instead of invader.transform.position because we want this position to be relative to the Invaders container game object (the parent). Otherwise any adjustments we make to the Invaders container game object's transform (moving it up on the screen), won't happen.
            }
        }
    }

    private void Start()
    {
        InvokeRepeating(nameof(MissleAttack), this.missleAttackRate, this.missleAttackRate);
    }

    private void Update()
    {   
        this.transform.position += _direction * this.speedCurve.Evaluate(percentKilled) * Time.deltaTime;

        Vector3 leftEdge = Camera.main.ViewportToWorldPoint(Vector3.zero);
        Vector3 rightEdge = Camera.main.ViewportToWorldPoint(Vector3.right);

        //looping through the invaders' Transforms inside the grid
        foreach (Transform invader in this.transform)
        {
            if (!invader.gameObject.activeInHierarchy) 
            {
                continue;
            }

            if (_direction == Vector3.right && invader.position.x >= (rightEdge.x - 1.0f))
            {
                AdvanceRow();
            } else if (_direction == Vector3.left && invader.position.x <= (leftEdge.x + 1.0f))
            {
                AdvanceRow();
            }

        }
    }

    private void AdvanceRow()
    {
        _direction.x *= -1.0f;

        Vector3 position = this.transform.position;
        position.y -= 1.0f;
        this.transform.position = position;
    }

    private void IncrementInvadersKilled()
    {
        this.invadersKilled += 1;
        
        //restart game if all invaders are killed
        if (this.invadersKilled >= totalInvaders)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void MissleAttack()
    {
        foreach (Transform invader in this.transform)
        {
            if (!invader.gameObject.activeInHierarchy) 
            {
                continue;
            }

            //Random.value spawns random value between 0 and 1
            //in the beginning there will be a lot alive, so 1/55 is a small number
            //near the end 1/10 is a bigger number and the chance that Random.value is less than that is greater
            if (Random.value < (1.0f / (float)this.amountAlive)) 
            {
                //Quaternion.identity means no rotation
                Instantiate(misslePrefab, invader.position, Quaternion.identity);
                break; //as soon as 1 missle happens, it won't spawn anymore missles
            }
            
        }
    }
}
