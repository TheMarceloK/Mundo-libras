using UnityEngine;
using UnityEngine.UI;

public class SeletorDeFases : MonoBehaviour
{
    public GameObject botaoPrefab; // prefab de botão (com Image + Button)
    public Transform gridParent;   // painel/grid para instanciar os botões

    void Start()
    {
        CriarBotoes();
    }

    void CriarBotoes()
    {
        foreach (var fase in GameManager.instancia.fases)
        {
            GameObject botaoObj = Instantiate(botaoPrefab, gridParent);
            Button botao = botaoObj.GetComponent<Button>();
            Image imagemBotao = botaoObj.GetComponent<Image>();

            // 1. Colocar a imagem da fase no botão
            Sprite sprite = Resources.Load<Sprite>(fase.imagem);
            if (sprite != null)
                imagemBotao.sprite = sprite;

            // 2. Adicionar o clique do botão
            int id = fase.id; // precisa criar variável local para fechar o loop corretamente
            botao.onClick.AddListener(() => GameManager.instancia.SelecionarFase(id));
        }
    }
}
