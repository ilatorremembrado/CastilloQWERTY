using UnityEngine;

public class Bonus : MonoBehaviour
{
    public int healAmount = 10;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.GainLife(healAmount);
                SoundManager.instance.PlaySound(SoundManager.instance.bonusSound);
                Destroy(gameObject); // Destruye el bonus tras recogerlo
            }
        }
    }
}
