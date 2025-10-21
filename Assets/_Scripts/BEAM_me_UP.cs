using UnityEngine;


public class BEAM_me_UP : MonoBehaviour
{

    [SerializeField] GameObject player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.GetComponent<SpriteRenderer>().enabled = false;
        this.GetComponent<Animator>().enabled = false;
    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player.SetActive(false);
            var animator = GetComponent<Animator>();
            var spriteRenderer = GetComponent<SpriteRenderer>();
            
            spriteRenderer.enabled = true;
            animator.enabled = true;
            animator.Play("Goal_BEAM_Animation");
            
        }
    }
    void Update()
    {

    }
}
