using UnityEngine;

public class RaycastDebugger : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Desenha uma linha na Scene View do ponto da câmera até o ponto de colisão.
                Debug.DrawLine(ray.origin, hit.point, Color.red, 2.0f);

                // Imprime no console qual objeto foi atingido.
                Debug.Log("Raycast atingiu: " + hit.collider.gameObject.name);
            }
            else
            {
                // Desenha uma linha longa para mostrar para onde o raio foi se não atingiu nada.
                Debug.DrawRay(ray.origin, ray.direction * 100, Color.yellow, 2.0f);
                Debug.Log("Raycast não atingiu nenhum collider.");
            }
        }
    }
}


