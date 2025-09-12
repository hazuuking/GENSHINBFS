using UnityEngine;

public class BloomCoreController : MonoBehaviour
{
    public GameObject bloomExplosionVFXPrefab;
    public float coreLifetime = 6f; // Tempo de vida do Dendro Core

    private float timer;

    void OnEnable()
    {
        timer = coreLifetime;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            ExplodeCore();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Lógica simplificada: se colidir com algo que tenha um elemento Pyro ou Electro
        // Em um jogo real, você verificaria tags, camadas ou componentes específicos
        if (other.CompareTag("PyroAttack") || other.CompareTag("ElectroAttack"))
        {
            ExplodeCore();
        }
    }

    private void ExplodeCore()
    {
        // Instancia o VFX de explosão na posição do core
        if (bloomExplosionVFXPrefab != null)
        {
            GameObject explosionInstance = Instantiate(bloomExplosionVFXPrefab, transform.position, Quaternion.identity);
            ElementalReactionVFX vfxController = explosionInstance.GetComponent<ElementalReactionVFX>();
            if (vfxController != null)
            {
                vfxController.Activate(transform.position, Quaternion.identity);
            }
        }
        gameObject.SetActive(false); // Desativa o core
    }
}