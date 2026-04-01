using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GerenciadorSelecaoNivel : MonoBehaviour
{
    [Header("Referências do Painel")]
    public GameObject painelSubniveis;
    public TextMeshProUGUI textoNomeDoTema;

    [Header("Indicadores de Conclusão")]
    // Arraste os 5 objetos de imagem (Icone_Concluido) para cá no Inspector, na ordem dos botões
    public GameObject[] iconesConcluido;

    private string temaSelecionado;

    void Start()
    {
        painelSubniveis.SetActive(false);
    }

    // --- MÉTODOS DOS BOTÕES PRINCIPAIS (TEMAS) ---

    // Chame este método no OnClick dos 4 botões de tema, passando o nome do tema como parâmetro (String)
    public void AbrirTema(string nomeTema)
    {
        temaSelecionado = nomeTema;
        if (temaSelecionado == "Saudacao")
        {
            textoNomeDoTema.text = "Tema: Saudação";
        }

        // Salva o tema escolhido globalmente para as cenas dos minigames saberem o que carregar
        PlayerPrefs.SetString("TemaAtual", temaSelecionado);

        AtualizarProgresso();
        painelSubniveis.SetActive(true);
    }

    public void FecharPainel()
    {
        painelSubniveis.SetActive(false);
    }

    // --- LÓGICA DE PROGRESSÃO ---

    private void AtualizarProgresso()
    {
        // Verifica cada um dos 5 níveis para ligar ou desligar o ícone de conclusão
        for (int i = 0; i < iconesConcluido.Length; i++)
        {
            // Cria uma chave única de save, ex: "Animais_Nivel_0", "Animais_Nivel_1"
            string chaveProgresso = temaSelecionado + "_Nivel_" + i;

            // O padrão é 0 (falso). Se o minigame salvou 1, significa que o nível foi concluído.
            bool concluido = PlayerPrefs.GetInt(chaveProgresso, 0) == 1;
            iconesConcluido[i].SetActive(concluido);
        }
    }

    // --- MÉTODOS DOS 5 BOTÕES DE NÍVEL (DENTRO DO PAINEL) ---

    // Coloque no OnClick dos botões de Quebra-cabeça e digite 1 ou 2 no parâmetro (Int)
    public void JogarQuebraCabeca(int numeroFase)
    {
        // Como cada tema tem uma cena separada, precisamos de um padrão de nomenclatura.
        // Exemplo de nome de cena: "QuebraCabeca_Animais_1"
        string nomeDaCena = "QuebraCabeca_" + temaSelecionado + "_" + numeroFase;
        SceneManager.LoadScene(nomeDaCena);
    }

    // Coloque no OnClick dos botões de Memória e digite 1 ou 2 no parâmetro (Int)
    public void JogarMemoria(int numeroFase)
    {
        // Salva qual é a fase para o jogo da memória saber se deve gerar um grid mais difícil (ex: 4x4 ou 6x6)
        PlayerPrefs.SetInt("FaseAtual", numeroFase);
        string nomeDaCena = "Memoria_" + temaSelecionado + "_" + numeroFase;
        SceneManager.LoadScene(nomeDaCena);
    }

    // Coloque no OnClick do botão de Bingo
    public void JogarBingo()
    {
        string nomeDaCena = "Bingo_" + temaSelecionado;
        SceneManager.LoadScene(nomeDaCena);
    }
}