using UnityEngine;
using System.Collections;

public class ElementalReactionVFX : MonoBehaviour
{
    [Tooltip("Duração do efeito visual em segundos. Se 0, o efeito será contínuo ou gerenciado externamente.")]
    public float effectDuration = 0f;

    private ParticleSystem[] particleSystems;

    void Awake()
    {
        // Obtém todos os sistemas de partículas filhos deste GameObject
        particleSystems = GetComponentsInChildren<ParticleSystem>();
    }

    void OnEnable()
    {
        // Garante que os sistemas de partículas sejam reproduzidos ao serem ativados
        PlayVFX();

        // Se a duração for maior que 0, agenda a desativação/destruição do efeito
        if (effectDuration > 0f)
        {
            StartCoroutine(DeactivateAfterDuration(effectDuration));
        }
    }

    public void PlayVFX()
    {
        foreach (ParticleSystem ps in particleSystems)
        {
            if (ps != null)
            {
                ps.Play();
            }
        }
    }

    public void StopVFX()
    {
        foreach (ParticleSystem ps in particleSystems)
        {
            if (ps != null)
            {
                ps.Stop();
            }
        }
    }

    private IEnumerator DeactivateAfterDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        // Para efeitos que serão reutilizados (pooling), desative o GameObject
        // Para efeitos que não serão reutilizados, destrua o GameObject
        // gameObject.SetActive(false); // Ou Destroy(gameObject);

        // Se estiver usando ObjectPoolManager, retorne o objeto para o pool
        if (ObjectPoolManager.Instance != null)
        {
            ObjectPoolManager.Instance.ReturnToPool(gameObject.name, gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    // Método para ser chamado quando a reação é ativada
    public void Activate(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
        gameObject.SetActive(true);
    }

    // Método para ser chamado quando a reação é desativada (para efeitos contínuos)
    public void Deactivate()
    {
        gameObject.SetActive(false);
        if (ObjectPoolManager.Instance != null)
        {
            ObjectPoolManager.Instance.ReturnToPool(gameObject.name, gameObject);
        }
    }
}


