using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Jogo : MonoBehaviour
{
    [Header("Configurações do Jogo")]
    [Range(0f, 100f)]
    public float velocidade = 10f;

    [Range(0f, 20f)]
    [SerializeField]
    private float modificadorAumentoVelocidade = 1f;

    [Range(0f, 200f)]
    [SerializeField]
    private float velocidadeMaxima = 50f;

    internal bool Rodando;

    [Range(0f, 3f)]
    [SerializeField]
    private float tempoLiberarReinicioJogo = 1f;

    private bool _reiniciarJogoLiberado;

    [Header("Elementos de Interface")]
    [SerializeField]
    private GameObject botaoReiniciar;

    [SerializeField]
    private Text textoPontuacao;

    [SerializeField]
    private Animator textoPontuacaoAnim;

    [SerializeField]
    private Text textoMelhorPontuacao;

    [Header("Pontuação")]
    [SerializeField]
    private float delayAtualizarTextoPontuacao = 1f;

    internal float Pontos;

    private bool _atualizarTextoPontuacao = true;

    private float _melhorPontuacao;

    private const string MelhorPontuacaoKey = "MELHOR_PONTUACAO";

    [Header("Audio Sources")]
    public AudioSource pularAudioSource;

    [SerializeField]
    private AudioSource pontuacaoAudioSource;

    [SerializeField]
    private AudioSource fimDeJogoAudioSource;

    private void Awake()
    {
        Rodando = true;

        _melhorPontuacao = PlayerPrefs.GetFloat(MelhorPontuacaoKey);
    }

    private void Update()
    {
        // Aumentar velocidade

        velocidade = Mathf.Clamp(
            velocidade + modificadorAumentoVelocidade * Time.deltaTime,
            0,
            velocidadeMaxima
        );

        // Pontuação

        Pontos += velocidade * Time.deltaTime;

        var pontuacaoArredondada = Mathf.FloorToInt(Pontos);

        if (_atualizarTextoPontuacao)
        {
            textoPontuacao.text = $"{pontuacaoArredondada:00000}";
        }

        if (pontuacaoArredondada > 0
            && pontuacaoArredondada % 100 == 0
            && _atualizarTextoPontuacao)
        {
            pontuacaoAudioSource.Play();

            _atualizarTextoPontuacao = false;

            Invoke(nameof(LiberarAtualizacaoTextoPontuacao), delayAtualizarTextoPontuacao);
        }

        textoPontuacaoAnim.SetBool("Piscar", !_atualizarTextoPontuacao);

        // Melhor pontuação

        ExibirMelhorPontuacao();

        // Reiniciar jogo

        if (!Rodando
            && _reiniciarJogoLiberado
            && (Input.GetKeyDown(KeyCode.UpArrow)
                || Input.GetKeyDown(KeyCode.DownArrow)))
        {
            Reiniciar();
        }
    }

    private void LiberarAtualizacaoTextoPontuacao()
    {
        _atualizarTextoPontuacao = true;
    }

    public void FimDeJogo()
    {
        Rodando = false;

        Time.timeScale = 0f;

        botaoReiniciar.SetActive(true);

        fimDeJogoAudioSource.Play();

        if (Pontos > _melhorPontuacao)
        {
            _melhorPontuacao = Pontos;

            PlayerPrefs.SetFloat(MelhorPontuacaoKey, _melhorPontuacao);

            ExibirMelhorPontuacao();
        }

        StartCoroutine(LiberarReiniciarJogo());
    }

    private void ExibirMelhorPontuacao()
    {
        var melhorPontuacaoArredondada = Mathf.FloorToInt(_melhorPontuacao);

        textoMelhorPontuacao.text = $"HI {melhorPontuacaoArredondada:00000}";

        textoMelhorPontuacao.enabled = melhorPontuacaoArredondada > 0;
    }

    public void Reiniciar()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        Time.timeScale = 1f;
    }

    private IEnumerator LiberarReiniciarJogo()
    {
        yield return new WaitForSecondsRealtime(tempoLiberarReinicioJogo);

        _reiniciarJogoLiberado = true;
    }
}
