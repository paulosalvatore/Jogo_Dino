using UnityEngine;

public class Dinossauro : MonoBehaviour
{
    [Header("Pulo")]
    [Range(0f, 180f)]
    [SerializeField]
    private float forcaPuloFraco = 60f;

    [Range(0f, 220f)]
    [SerializeField]
    private float forcaPuloForte = 90f;

    [Range(0f, 1f)]
    [SerializeField]
    private float tempoNecessarioPuloForte = 0.2f;

    private float _deltaTeclaSetaCima;

    private float _deltaTeclaBarraEspaco;

    [SerializeField]
    private float distanciaNecessariaDoChao = 1.5f;

    private bool _estaNoChao;

    private bool _puloLiberado = true;

    private bool _puloForteLiberado;

    private bool _puloTravado;

    [Header("Queda")]
    [Range(0f, 30f)]
    [SerializeField]
    private float velocidadeQuedaNormal = 5f;

    [Range(0f, 60f)]
    [SerializeField]
    private float velocidadeQuedaAcelerada = 20f;

    // Movimentação

    private bool _andando = true;

    // Posicionamento

    private float _posicaoYInicial;

    [Header("Componentes")]
    [SerializeField]
    private Rigidbody2D rb;

    [SerializeField]
    private Animator anim;

    [SerializeField]
    private Jogo jogo;

    private void Awake()
    {
        _posicaoYInicial = transform.position.y;
    }

    private void Update()
    {
        // Travar pulo
        TravarPulo();

        // Calcular tempo da tecla pressionada
        CalcularDeltaTeclasPulo();

        // Pulo
        Pular();

        // Queda Acelerada
        AcelerarQueda();

        // Atualizar animação
        AtualizarAnimacoes();

        // Clamp Posição Y
        ForcaPosicaoYMinima();
    }

    private void ForcaPosicaoYMinima()
    {
        var posicao = transform.position;

        if (posicao.y < _posicaoYInicial)
        {
            transform.position = new Vector3(
                posicao.x,
                _posicaoYInicial,
                posicao.z
            );
        }
    }

    private void AtualizarAnimacoes()
    {
        anim.SetBool("Andando", _andando);

        var abaixado = _estaNoChao && Input.GetKey(KeyCode.DownArrow);
        anim.SetBool("Abaixado", abaixado);
    }

    private void AcelerarQueda()
    {
        if (!_estaNoChao
            && Input.GetKeyDown(KeyCode.DownArrow))
        {
            rb.gravityScale = velocidadeQuedaAcelerada;
        }
        else if (_estaNoChao)
        {
            rb.gravityScale = velocidadeQuedaNormal;
        }
    }

    private void Pular()
    {
        if (_estaNoChao
            && _puloLiberado
            && !_puloTravado
            && !Input.GetKey(KeyCode.DownArrow)
            && (Input.GetKeyDown(KeyCode.UpArrow)
                || Input.GetKeyDown(KeyCode.Space)))
        {
            _puloLiberado = false;

            rb.velocity = Vector2.zero;
            rb.AddForce(forcaPuloFraco * Vector2.up);

            _puloForteLiberado = true;

            jogo.pularAudioSource.Play();
        }
        else if (_puloForteLiberado
                 && (_deltaTeclaBarraEspaco > tempoNecessarioPuloForte ||
                     _deltaTeclaSetaCima > tempoNecessarioPuloForte))
        {
            _puloForteLiberado = false;

            rb.AddForce(forcaPuloForte * Vector2.up);
        }
        else if (!_estaNoChao && !_puloLiberado)
        {
            _puloLiberado = true;
        }
    }

    private void TravarPulo()
    {
        if (!_puloTravado && Input.GetKeyDown(KeyCode.DownArrow))
        {
            _puloTravado = true;
        }
        else if (_puloTravado
                 && Input.GetKeyDown(KeyCode.UpArrow)
                 && !Input.GetKey(KeyCode.DownArrow))
        {
            _puloTravado = false;
        }
    }

    private void CalcularDeltaTeclasPulo()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            _puloForteLiberado = false;
            _deltaTeclaSetaCima = 0;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _puloForteLiberado = false;
            _deltaTeclaBarraEspaco = 0;
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            _deltaTeclaSetaCima += Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            _deltaTeclaBarraEspaco += Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        _estaNoChao = Physics2D.Raycast(
            transform.position,
            Vector2.down,
            distanciaNecessariaDoChao,
            1 << LayerMask.NameToLayer("Chão")
        );
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Obstáculo"))
        {
            anim.SetBool("Morto", true);

            jogo.FimDeJogo();
        }
    }
}
