using UnityEngine;
using UnityEngine.UI;

public class CartaMemoria : MonoBehaviour
{
    public Image imagemCarta;
    public Button botao;

    [HideInInspector] public string imagemNome;
    [HideInInspector] public string videoNome;
    [HideInInspector] public bool revelada;
     public int idPar;

    private MinigameMemoria gerenciador;

    private Sprite spriteFrente;
    private Sprite spriteCostas;

    public void Configurar(string imagemNome, string videoNome, int idPar, MinigameMemoria gerenciador)
    {
        this.imagemNome = imagemNome;
        this.videoNome = videoNome;
        this.idPar = idPar;
        this.gerenciador = gerenciador;

        // Carregar sprites
        spriteFrente = Resources.Load<Sprite>(imagemNome);
        if (spriteFrente == null)
            Debug.LogWarning($"Imagem '{imagemNome}' não encontrada em Resources!");

        spriteCostas = Resources.Load<Sprite>("memoria_costas");
        if (spriteCostas == null)
            Debug.LogWarning("Imagem 'memoria_costas' não encontrada em Resources!");

        if (imagemCarta == null)
            imagemCarta = GetComponent<Image>();
        if (botao == null)
            botao = GetComponent<Button>();

        if (imagemCarta != null)
            imagemCarta.sprite = spriteCostas;
        revelada = false;

        if (botao != null)
        {
            botao.onClick.RemoveAllListeners();
            botao.onClick.AddListener(() => gerenciador.SelecionarCarta(this));
        }
        else
        {
            Debug.LogError("❌ Botão não encontrado em CartaMemoria!");
        }
    }

    public void VirarParaCima()
    {
        if (!revelada && spriteFrente != null)
            imagemCarta.sprite = spriteFrente;
        revelada = true;
    }

    public void VirarParaBaixo()
    {
        if (spriteCostas != null)
            imagemCarta.sprite = spriteCostas;
        revelada = false;
    }

    public void Desativar()
    {
        if (botao != null)
            botao.interactable = false;
        Color cor = imagemCarta.color;
        cor.a = 0.4f;
        imagemCarta.color = cor;
    }
}
