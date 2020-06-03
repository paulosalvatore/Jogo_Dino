using System.Collections;
using UnityEngine;

public class GerarObstaculos : MonoBehaviour
{
    [Header("Cactos")]
    [SerializeField]
    private GameObject[] cactosDisponiveis;

    [SerializeField]
    private Range delayInicial = new Range(1f, 1f);

    [SerializeField]
    private Range distanciaEntreCactos = new Range(1f, 1f);

    [Header("Voadores")]
    [SerializeField]
    private GameObject voadorPrefab;

    [SerializeField]
    private Range distanciaEntreVoadores = new Range(10f, 10f);

    public Range chanceVoador = new Range(0f, 30f);

    [Tooltip("Os valores mínimos e máximos serão somados à posição de Y atual do objeto.")]
    [SerializeField]
    private Range posicaoVoadorY = new Range(0, 0);

    [Range(0, 99999)]
    [Tooltip("Só irá gerar os voadores caso o jogador atinja a pontuação mínima informada aqui.")]
    [SerializeField]
    private int pontuacaoMinimaVoadores;

    [Header("Componentes")]
    [SerializeField]
    private Jogo jogo;

    private void Start()
    {
        // Assim que jogo iniciar, chamamos a Coroutine que irá gerar os objetos
        StartCoroutine(GerarObstaculo());
    }

    private IEnumerator GerarObstaculo()
    {
        // Esperamos o tempo inicial
        yield return new WaitForSeconds(delayInicial.ValorAleatorio);

        GameObject ultimoVoador = null;
        GameObject ultimoCacto = null;

        // Iniciamos o loop da geração dos objetos
        do
        {
            // Voador
            var random = Random.Range(0, 100);
            var gerarVoador = jogo.Pontos > pontuacaoMinimaVoadores
                              && random > chanceVoador.min && random < chanceVoador.max;

            var distanciaVoadorNecessaria = distanciaEntreVoadores.ValorAleatorio;
            var distanciaVoadorValidada = ultimoVoador == null
                                          || Mathf.Abs(transform.position.x - ultimoVoador.transform.position.x) >
                                          distanciaVoadorNecessaria;
            // Cacto
            var distanciaCactoNecessaria = distanciaEntreCactos.ValorAleatorio;
            var distanciaCactoValidada = ultimoCacto == null
                                         || Mathf.Abs(transform.position.x - ultimoCacto.transform.position.x) >
                                         distanciaCactoNecessaria;

            if (gerarVoador && distanciaVoadorValidada)
            {
                while (ultimoCacto != null
                       && Mathf.Abs(transform.position.x - ultimoCacto.transform.position.x) <
                       distanciaCactoNecessaria)
                {
                    yield return null;
                }

                // Atualizamos a posição de Y com o valor aleatório baseado no que foi informado
                var posicaoVoador = transform.position;
                posicaoVoador.y += posicaoVoadorY.ValorAleatorio;

                ultimoVoador = Instantiate(voadorPrefab, posicaoVoador, voadorPrefab.transform.rotation);
            }
            else if (distanciaCactoValidada)
            {
                while (ultimoVoador != null
                       && Mathf.Abs(transform.position.x - ultimoVoador.transform.position.x) <
                       distanciaCactoNecessaria)
                {
                    yield return null;
                }

                var gerarPrefab = cactosDisponiveis.RandomElement();
                ultimoCacto = Instantiate(gerarPrefab, transform.position, gerarPrefab.transform.rotation);
            }

            yield return null;
        } while (jogo.Rodando);
    }
}
