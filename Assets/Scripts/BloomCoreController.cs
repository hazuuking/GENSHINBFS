using UnityEngine;
using System.Collections;

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
        // ou se o objeto que colidiu tem um ElementalAuraManager e qual elemento ele está aplicando.
        // Exemplo: ProjectileElement projectile = other.GetComponent<ProjectileElement>();
        // if (projectile != null && (projectile.element == ElementType.Pyro || projectile.element == ElementType.Electro))
        // {
        //     ExplodeCore();
        // }

        // Usando tags como no script original, mas idealmente seria mais robusto
        if (other.CompareTag("PyroAttack") || other.CompareTag("ElectroAttack"))
        {
            ExplodeCore();
        }
    }

    private void ExplodeCore()
    {
        // Instancia o VFX de explosão na posição do core usando ObjectPoolManager
        if (bloomExplosionVFXPrefab != null)
        {
            GameObject explosionInstance = null;
            if (ObjectPoolManager.Instance != null)
            {
                // Assumindo que o nome do prefab de explosão é a tag do pool
                explosionInstance = ObjectPoolManager.Instance.SpawnFromPool(bloomExplosionVFXPrefab.name, transform.position, Quaternion.identity);
            }
            else
            {
                explosionInstance = Instantiate(bloomExplosionVFXPrefab, transform.position, Quaternion.identity);
            }
            
            if (explosionInstance != null)
            {
                ElementalReactionVFX vfxController = explosionInstance.GetComponent<ElementalReactionVFX>();
                if (vfxController != null)
                {
                    // O Activate já é chamado pelo SpawnFromPool que ativa o GameObject
                    // vfxController.Activate(transform.position, Quaternion.identity); // Não precisa chamar novamente
                }
            }
        }
        
        // Retorna o próprio core para o pool, se estiver usando pooling para o core
        if (ObjectPoolManager.Instance != null)
        {
            ObjectPoolManager.Instance.ReturnToPool(gameObject.name, gameObject);
        }
        else
        {
            gameObject.SetActive(false); // Desativa o core
        }
    }
}