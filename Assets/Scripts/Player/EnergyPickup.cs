using UnityEngine;

public class EnergyPickup : MonoBehaviour
{
    public int energyAmount = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerEnergy playerEnergy = other.GetComponent<PlayerEnergy>();
            if (playerEnergy != null)
            {
                playerEnergy.AddEnergy(energyAmount);
                Destroy(gameObject);
            }
        }
    }
}
