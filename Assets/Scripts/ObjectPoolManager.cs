using UnityEngine;
using System.Collections.Generic;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance { get; private set; }

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools;
    private Dictionary<string, Queue<GameObject>> poolDictionary;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            if (poolDictionary.ContainsKey(pool.tag))
            {
                Debug.LogWarning($"Uma Pool com a tag '{pool.tag}' já existe. Por favor, use tags únicas para cada pool. Esta pool será ignorada.");
                continue; // Pula esta pool duplicada
            }

            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                obj.transform.SetParent(this.transform); // Opcional: manter objetos instanciados como filhos do ObjectPoolManager
                objectPool.Enqueue(obj);
            }
            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool com a tag {tag} não existe.");
            return null;
        }

        if (poolDictionary[tag].Count == 0)
        {
            Debug.LogWarning($"Pool \'{tag}\' está vazia. Instanciando novo objeto. Considere aumentar o tamanho do pool.");
            // Opcional: instanciar um novo objeto se o pool estiver vazio
            Pool targetPool = pools.Find(p => p.tag == tag);
            if (targetPool != null)
            {
                GameObject newObj = Instantiate(targetPool.prefab);
                newObj.transform.SetParent(this.transform);
                newObj.transform.position = position;
                newObj.transform.rotation = rotation;
                newObj.SetActive(true);
                return newObj;
            }
            return null;
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        objectToSpawn.SetActive(true);

        // Ativa o script ElementalReactionVFX se presente
        ElementalReactionVFX vfxController = objectToSpawn.GetComponent<ElementalReactionVFX>();
        if (vfxController != null)
        {
            vfxController.Activate(position, rotation); // Garante que o VFX seja ativado corretamente
        }

        return objectToSpawn;
    }

    public void ReturnToPool(string tag, GameObject objectToReturn)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool com a tag {tag} não existe. Destruindo objeto {objectToReturn.name}.");
            Destroy(objectToReturn);
            return;
        }

        objectToReturn.SetActive(false);
        objectToReturn.transform.SetParent(this.transform); // Garante que o objeto retorne como filho do pool manager
        poolDictionary[tag].Enqueue(objectToReturn);
    }
}


