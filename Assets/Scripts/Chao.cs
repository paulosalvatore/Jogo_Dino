using UnityEngine;

public class Chao : MonoBehaviour
{
    [SerializeField]
    private Jogo jogo;

    [SerializeField]
    public float minX;

    [SerializeField]
    public float reiniciarX;

    private void Update()
    {
        transform.Translate(jogo.velocidade * Time.deltaTime * Vector2.left);

        if (transform.position.x < minX)
        {
            var posicao = transform.position;
            posicao.x += reiniciarX;

            transform.position = posicao;
        }
    }
}
