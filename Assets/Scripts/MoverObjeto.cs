using UnityEngine;

public class MoverObjeto : MonoBehaviour
{
    private Jogo _jogo;

    [Range(0f, 60f)]
    [Tooltip("Caso o valor seja maior que zero, destrói o objeto após esse tempo")]
    [SerializeField]
    private float delayDestruir;

    [SerializeField]
    private Range rangeDestruirX = new Range(0f, 0f);

    [Tooltip("Esse modificador será aplicado em cima da velocidade do jogo.")]
    [SerializeField]
    private Range modificadorVelocidade = new Range(1f, 1f);

    private void Start()
    {
        _jogo = GameObject.Find("Jogo").GetComponent<Jogo>();

        if (delayDestruir > 0)
        {
            Destroy(gameObject, delayDestruir);
        }
    }

    private void Update()
    {
        transform.Translate(
            _jogo.velocidade * modificadorVelocidade.ValorAleatorio * Time.deltaTime * Vector2.left
        );

        if (rangeDestruirX.min != 0f && rangeDestruirX.max != 0f)
        {
            var posicao = transform.position;

            if (posicao.x < rangeDestruirX.min || posicao.x > rangeDestruirX.max)
            {
                Destroy(gameObject);
            }
        }
    }
}
