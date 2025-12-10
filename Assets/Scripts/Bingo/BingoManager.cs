using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro; // Usando TextMeshPro para textos melhores (padrão atual do Unity)

public class BingoManager : MonoBehaviour
{
    // --- ESTRUTURA DE DADOS ---
    [System.Serializable]
    public class ItemLibras
    {
        public string nome;         // Nome para identificação (ex: "Casa")
        public VideoClip video;     // O arquivo de vídeo
        public Sprite imagem;       // A imagem correta
    }

    [Header("Configurações do Jogo")]
    public List<ItemLibras> listaDeConteudo; // Arraste seus 10+ itens aqui no Inspector
    public int totalRodadas = 10; // Limite de pontuação/rodadas

    [Header("Referências da UI")]
    public VideoPlayer videoPlayer;      // Componente que toca o vídeo
    public RawImage telaDoVideo;         // Onde o vídeo é renderizado na UI
    public Image[] botoesOpcoes;         // Os 4 botões de resposta (Imagens)
    public TextMeshProUGUI textoPontuacao; // Texto para mostrar "Acertos: X"
    public GameObject painelFimDeJogo;   // Painel para ativar quando acabar

    // --- VARIÁVEIS DE CONTROLE ---
    private List<ItemLibras> listaEmbaralhada;
    private ItemLibras itemAtual;
    private int rodadaAtual = 0;
    private int pontuacao = 0;
    private bool podeResponder = true;

    void Start()
    {
        // 1. Validar se temos itens suficientes
        if (listaDeConteudo.Count < 4)
        {
            Debug.LogError("Você precisa de pelo menos 4 itens na lista para criar as opções falsas!");
            return;
        }

        // 2. Preparar o jogo
        pontuacao = 0;
        rodadaAtual = 0;
        painelFimDeJogo.SetActive(false);
        AtualizarTextoPontuacao();

        // 3. Criar uma cópia da lista e embaralhar (Shuffle)
        // Isso garante ordem aleatória a cada jogo sem estragar a lista original
        listaEmbaralhada = new List<ItemLibras>(listaDeConteudo);
        EmbaralharLista(listaEmbaralhada);

        // 4. Começar a primeira rodada
        CarregarRodada();
    }

    void CarregarRodada()
    {
        // Verifica se atingiu o limite de rodadas ou se acabaram os itens
        if (rodadaAtual >= totalRodadas || rodadaAtual >= listaEmbaralhada.Count)
        {
            FimDeJogo();
            return;
        }

        podeResponder = true;
        itemAtual = listaEmbaralhada[rodadaAtual];

        // --- Tocar o Vídeo ---
        videoPlayer.clip = itemAtual.video;
        videoPlayer.Play();

        // --- Configurar as Opções (Botões) ---
        List<ItemLibras> opcoesDaRodada = new List<ItemLibras>();

        // Adiciona a resposta CORRETA
        opcoesDaRodada.Add(itemAtual);

        // Adiciona 3 respostas INCORRETAS (Aleatórias da lista geral)
        while (opcoesDaRodada.Count < 4)
        {
            ItemLibras itemAleatorio = listaDeConteudo[Random.Range(0, listaDeConteudo.Count)];

            // Garante que não repete itens na mesma rodada (nem a certa, nem duplicatas erradas)
            if (!opcoesDaRodada.Contains(itemAleatorio))
            {
                opcoesDaRodada.Add(itemAleatorio);
            }
        }

        // Embaralha as 4 opções para a correta não ficar sempre no mesmo botão
        EmbaralharLista(opcoesDaRodada);

        // Atribui as imagens aos botões e configura os cliques
        for (int i = 0; i < botoesOpcoes.Length; i++)
        {
            botoesOpcoes[i].sprite = opcoesDaRodada[i].imagem;

            // Remove listeners antigos para não acumular cliques
            botoesOpcoes[i].GetComponent<Button>().onClick.RemoveAllListeners();

            // Identifica qual item está neste botão para checar a resposta
            ItemLibras itemDoBotao = opcoesDaRodada[i];
            botoesOpcoes[i].GetComponent<Button>().onClick.AddListener(() => VerificarResposta(itemDoBotao));
        }
    }

    void VerificarResposta(ItemLibras itemEscolhido)
    {
        if (!podeResponder) return; // Evita clique duplo
        podeResponder = false;

        if (itemEscolhido == itemAtual)
        {
            // Acertou!
            Debug.Log("Acertou!");
            pontuacao++;
            AtualizarTextoPontuacao();
            // Aqui você pode tocar um som de acerto ou efeito visual
        }
        else
        {
            // Errou!
            Debug.Log("Errou! Era: " + itemAtual.nome);
            // Aqui você pode tocar um som de erro
        }

        // Avança para a próxima rodada após um pequeno delay (opcional, aqui vou direto)
        rodadaAtual++;
        Invoke("CarregarRodada", 1.0f); // Espera 1 segundo para o jogador ver o resultado
    }

    void AtualizarTextoPontuacao()
    {
        if (textoPontuacao != null)
            textoPontuacao.text = "Pontos: " + pontuacao + " / " + totalRodadas;
    }

    void FimDeJogo()
    {
        Debug.Log("Jogo Finalizado! Pontuação Final: " + pontuacao);
        videoPlayer.Stop();
        painelFimDeJogo.SetActive(true);
        // Aqui você pode mostrar estrelas, botão de reiniciar, etc.
    }

    // Algoritmo de Fisher-Yates para embaralhar listas (Eficiente)
    void EmbaralharLista<T>(List<T> lista)
    {
        int n = lista.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = lista[k];
            lista[k] = lista[n];
            lista[n] = value;
        }
    }
}