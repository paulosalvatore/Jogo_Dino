using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GerarObjetos : MonoBehaviour
{
    [Header("Prefabs a serem gerados")]
    [SerializeField]
    private GameObject[] prefabsDisponiveis;

    [Header("Configurações")]
    [SerializeField]
    private Range delayInicial = new Range(1f, 1f);

    [SerializeField]
    private Range distanciaEntreObjetos = new Range(1f, 1f);

    [Tooltip("Os valores mínimos e máximos serão somados à posição de Y atual do objeto.")]
    [SerializeField]
    private Range posicaoY = new Range(0, 0);

    [Range(0, 99999)]
    [Tooltip("Só irá gerar os objetos caso o jogador atinja a pontuação mínima informada aqui.")]
    [SerializeField]
    private int pontuacaoMinima;

    [Tooltip("Marque se esse objeto for um obstáculo, para que o script evite a criação de obstáculos sobrepostos.")]
    [SerializeField]
    private bool obstaculo;

    [Header("Componentes")]
    [SerializeField]
    private Jogo jogo;

    private static GameObject ultimoObstaculo;

    private void Start()
    {
        // Assim que jogo iniciar, chamamos a Coroutine que irá gerar os objetos
        StartCoroutine(GerarObjeto());
    }

    private IEnumerator GerarObjeto()
    {
        // Esperamos o tempo inicial
        yield return new WaitForSeconds(delayInicial.ValorAleatorio);

        // Aguardamos até que o jogador atinja a pontuação mínima informada
        while (jogo.Pontos < pontuacaoMinima)
        {
            yield return null;
        }

        GameObject ultimoObjetoGerado = null;

        // Iniciamos o loop da geração dos objetos
        do
        {
            var ultimoObjeto = obstaculo ? ultimoObstaculo : ultimoObjetoGerado;

            // Calculamos a distância necessária que esse objeto precisa percorrer
            // para gerar um novo objeto
            var distanciaNecessaria = distanciaEntreObjetos.ValorAleatorio;

            // print($"{gameObject.name}, {ultimoObjeto?.name}, {distanciaNecessaria}");

            // Caso o objeto que tenhamos gerado por último não tenha sido destruído
            // e a distância atual desse objeto for menor que a 'distanciaNecessaria', aguardamos
            // até o próximo frame.
            // Caso contrário, encerramos esse 'while' (pois a condição mudará para false) e voltamos para
            // o início do loop anterior, para que um novo objeto seja gerado.
            while (ultimoObjeto != null
                   && Mathf.Abs(transform.position.x - ultimoObjeto.transform.position.x) < distanciaNecessaria)
            {
                ultimoObjeto = obstaculo ? ultimoObstaculo : ultimoObjetoGerado;

                // print($"{gameObject.name}, {ultimoObjeto?.name}, {ultimoObstaculo?.name}, {distanciaNecessaria}, {Mathf.Abs(transform.position.x - ultimoObjeto.transform.position.x)}");
                yield return null;
            }

            // Atualizamos a posição de Y com o valor aleatório baseado no que foi informado
            var posicao = transform.position;
            posicao.y += posicaoY.ValorAleatorio;

            // Pegamos um item aleatório dos prefabs disponíveis
            var gerarPrefab = prefabsDisponiveis.RandomElement();

            if (obstaculo)
            {
                if (ultimoObjeto != null)
                {
                    print($"{gameObject.name}, {ultimoObjeto?.name}, {ultimoObstaculo?.name}, {distanciaNecessaria}, {Mathf.Abs(transform.position.x - (ultimoObjeto?.transform.position.x ?? 0))}");
                }
                else
                {
                    print($"{gameObject.name}, {ultimoObstaculo?.name}, {distanciaNecessaria}");
                }
            }

            // print($"{gameObject.name}, {ultimoObjeto?.name}, {distanciaNecessaria}");

            // Geramos um novo objeto e guardamos na variavel 'ultimoObjeto'
            ultimoObjetoGerado = Instantiate(gerarPrefab, posicao, gerarPrefab.transform.rotation);

            if (obstaculo)
            {
                ultimoObstaculo = ultimoObjetoGerado;
            }

            yield return null;
        } while (jogo.Rodando);
    }
}
