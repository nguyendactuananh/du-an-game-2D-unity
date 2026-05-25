using UnityEngine;
using UnityEngine.UI;

public class BossHPBarUI : MonoBehaviour
{
    public Transform boss;
    public Vector3 offset = new Vector3(0, 1.8f, 0);

    public int maxHP;
    private int currentHP;

    private Camera cam;
    private GameObject root;
    private Image fill;

    void Start()
    {
        cam = Camera.main;
        CreateUI();
    }

    void CreateUI()
    {
        root = new GameObject("BossHP_UI");
        Canvas c = root.AddComponent<Canvas>();
        c.renderMode = RenderMode.ScreenSpaceOverlay;

        root.AddComponent<CanvasScaler>();
        root.AddComponent<GraphicRaycaster>();

        GameObject bg = new GameObject("BG");
        bg.transform.SetParent(root.transform);

        Image bgImg = bg.AddComponent<Image>();
        bgImg.color = Color.black;

        RectTransform bgRt = bg.GetComponent<RectTransform>();
        bgRt.sizeDelta = new Vector2(120, 15);

        GameObject f = new GameObject("Fill");
        f.transform.SetParent(bg.transform);

        fill = f.AddComponent<Image>();
        fill.color = Color.red;

        RectTransform frt = f.GetComponent<RectTransform>();
        frt.anchorMin = Vector2.zero;
        frt.anchorMax = Vector2.one;
        frt.offsetMin = Vector2.zero;
        frt.offsetMax = Vector2.zero;
    }

    void LateUpdate()
    {
        if (boss == null) return;

        Vector3 pos = cam.WorldToScreenPoint(boss.position + offset);
        root.transform.position = pos;
    }

    public void SetMaxHP(int hp)
    {
        maxHP = hp;
        currentHP = hp;
    }

    public void UpdateHP(int hp)
    {
        currentHP = hp;

        float percent = (float)currentHP / maxHP;

        fill.transform.localScale = new Vector3(percent, 1, 1);
    }
}