using UnityEngine;
using UnityEngine.UI;

public class SeletorDeFases : MonoBehaviour
{
    public GameObject botaoPrefab; // Prefab de botão (com Image + Button)
    public Transform gridParent;   // Painel/grid onde os botões serão instanciados

    void Start()
    {
        CriarBotoes();
    }

    void CriarBotoes()
    {
        if (GameManager.instancia == null)
        {
            Debug.LogError("❌ GameManager não encontrado na cena!");
            return;
        }

        if (GameManager.instancia.fases == null || GameManager.instancia.fases.Count == 0)
        {
            Debug.LogWarning("⚠️ Nenhuma fase carregada para criar botões!");
            return;
        }

        foreach (var fase in GameManager.instancia.fases)
        {
            GameObject botaoObj = Instantiate(botaoPrefab, gridParent);
            Button botao = botaoObj.GetComponent<Button>();
            Image imagemBotao = botaoObj.GetComponent<Image>();

            if (imagemBotao == null) imagemBotao = botaoObj.GetComponentInChildren<Image>();

            // Tenta carregar a imagem do botão
            Sprite sprite = null;
            if (!string.IsNullOrEmpty(fase.imagem))
            {
                sprite = Resources.Load<Sprite>(fase.imagem);
            }

            if (sprite != null)
            {
                imagemBotao.sprite = sprite;
            }
            else
            {
                // log sem quebrar
                Debug.LogWarning($"⚠️ Imagem '{fase.imagem}' não encontrada em Resources para fase {fase.id}.");
            }

            // variável local para capturar corretamente no listener
            int id = fase.id;
            if (botao != null)
            {
                botao.onClick.AddListener(() =>
                {
                    Debug.Log($"🔘 Botão clicado -> pedindo seleção da fase {id}");
                    GameManager.instancia.SelecionarFase(id);
                });
            }
            else
            {
                Debug.LogWarning("⚠️ Prefab do botão não tem componente Button no root.");
            }
        }

        Debug.Log($"✅ {GameManager.instancia.fases.Count} botões de fase criados com sucesso!");
    }
}
