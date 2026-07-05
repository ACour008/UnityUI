using UnityEngine;
public class UIManager : Singleton<UIManager>
{
    public Material uiMaterial { get; private set; }
    public UICanvas canvas { get; set; }

    public UIManager()
    {
        uiMaterial = Resources.Load<Material>("UIMaterial");
    }
}