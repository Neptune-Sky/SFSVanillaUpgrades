using SFS.UI;
using VanillaUpgrades.Utility;
using Button = SFS.UI.ModGUI.Button;
using Object = UnityEngine.Object;
using Type = SFS.UI.ModGUI.Type;

namespace VanillaUpgrades;

public static class ConfigUI
{
    const int ToggleHeight = 32;

    public static CategoriesWindow window;

    public static void Setup()
    {
        window = new CategoriesWindow();
        window.Initialize(750, 1200, -800, 600, "VanillaUpgrades Config", Builder.SceneToAttach.BaseScene,
            "MainConfigWindow");
        window.AddMenu("GUI", transform1 => GetGUISettings(transform1, window.RecommendedContentSize));
        window.AddMenu("Units", transform1 => GetUnitsSettings(transform1, window.RecommendedContentSize));
        window.AddMenu("Misc", transform1 => GetMiscSettings(transform1, window.RecommendedContentSize));
        window.AddMenu("Cheats", transform1 => GetCheatsSettings(transform1, window.RecommendedContentSize));
        window.AddMenu("Windows", transform1 => GetWindowSettings(transform1, window.RecommendedContentSize));
    }

    static GameObject GetGUISettings(Transform parent, Vector2Int size)
    {
        Box box = Builder.CreateBox(parent, size.x, size.y);
        box.CreateLayoutGroup(Type.Vertical, TextAnchor.UpperCenter, 35, new RectOffset(15, 15, 15, 15));

        int elementWidth = size.x - 60;

        Builder.CreateLabel(box, elementWidth, 50, 0, 0, "GUI");
        if (!Main.buildSettingsPresent)
        {
            Builder.CreateToggleWithLabel(box, elementWidth, ToggleHeight, () => Config.settingsData.showBuildGui,
                () => Config.settingsData.showBuildGui.Value ^= true, 0, 0, "Show Build Settings");
            Builder.CreateSeparator(box, elementWidth - 20);
        }

        Builder.CreateToggleWithLabel(box, elementWidth, ToggleHeight, () => Config.settingsData.showAdvanced,
            () => Config.settingsData.showAdvanced.Value ^= true, 0, 0, "Show Advanced Info");
        Builder.CreateToggleWithLabel(box, elementWidth, ToggleHeight, () => Config.settingsData.horizontalMode,
            () => Config.settingsData.horizontalMode.Value ^= true, 0, 0, "Horizontal Mode");
        Builder.CreateSeparator(box, elementWidth - 20);
        /*
        Builder.CreateToggleWithLabel(box, elementWidth, ToggleHeight, () => Config.settingsData.showCalc, () => Config.settingsData.showCalc ^= true, 0, 0, "Show dV Calc by Default");
        Builder.CreateToggleWithLabel(box, elementWidth, ToggleHeight, () => Config.settingsData.showAverager, () => Config.settingsData.showAverager ^= true, 0, 0, "Averager Default");
        Builder.CreateSeparator(box, elementWidth - 20);
        */
        Builder.CreateToggleWithLabel(box, elementWidth, ToggleHeight, () => Config.settingsData.showTime,
            () => Config.settingsData.showTime.Value ^= true, 0, 0, "Show Clock While Timewarping");
        Builder.CreateToggleWithLabel(box, elementWidth, ToggleHeight, () => Config.settingsData.showWorldTime,
            () => Config.settingsData.showWorldTime.Value ^= true, 0, 0, "Show World Time in Clock");
        Builder.CreateToggleWithLabel(box, elementWidth, ToggleHeight, () => Config.settingsData.alwaysShowTime,
            () => Config.settingsData.alwaysShowTime.Value ^= true, 0, 0, "Always Show World Clock");

        return box.gameObject;
    }

    static GameObject GetUnitsSettings(Transform parent, Vector2Int size)
    {
        Box box = Builder.CreateBox(parent, size.x, size.y);
        box.CreateLayoutGroup(Type.Vertical, TextAnchor.UpperCenter, 35, new RectOffset(15, 15, 15, 15));

        int elementWidth = size.x - 60;

        Builder.CreateLabel(box, elementWidth, 50, 0, 0, "Units");

        Builder.CreateToggleWithLabel(box, elementWidth, ToggleHeight, () => Config.settingsData.mmUnits,
            () => Config.settingsData.mmUnits ^= true, 0, 0, "Megameters (Mm)");
        Builder.CreateSeparator(box, elementWidth - 20);
        Builder.CreateToggleWithLabel(box, elementWidth, ToggleHeight, () => Config.settingsData.kmsUnits,
            () => Config.settingsData.kmsUnits ^= true, 0, 0, "Kilometers/Second (km/s)");
        Builder.CreateSeparator(box, elementWidth - 20);
        Builder.CreateToggleWithLabel(box, elementWidth, ToggleHeight, () => Config.settingsData.cUnits,
            () => Config.settingsData.cUnits ^= true, 0, 0, "% Speed of Light (c)");
        Builder.CreateSeparator(box, elementWidth - 20);
        Builder.CreateToggleWithLabel(box, elementWidth, ToggleHeight, () => Config.settingsData.ktUnits,
            () => Config.settingsData.ktUnits ^= true, 0, 0, "Kilotonnes (kt)");

        return box.gameObject;
    }

    static GameObject GetMiscSettings(Transform parent, Vector2Int size)
    {
        Box box = Builder.CreateBox(parent, size.x, size.y);
        box.CreateLayoutGroup(Type.Vertical, TextAnchor.UpperCenter, 35, new RectOffset(15, 15, 15, 15));

        int elementWidth = size.x - 60;

        Builder.CreateLabel(box, elementWidth, 50, 0, 0, "Miscellaneous");
        Builder.CreateToggleWithLabel(box, elementWidth, ToggleHeight, () => Config.settingsData.explosions,
            () => Config.settingsData.explosions ^= true, 0, 0, "Explosion Effects");
        Builder.CreateToggleWithLabel(box, elementWidth, ToggleHeight, () => Config.settingsData.explosionShake,
            () => Config.settingsData.explosionShake ^= true, 0, 0, "Explosion Shake");
        Builder.CreateSeparator(box, elementWidth - 20);
        Builder.CreateToggleWithLabel(box, elementWidth, ToggleHeight,
            () => Config.settingsData.stopTimewarpOnEncounter,
            () => Config.settingsData.stopTimewarpOnEncounter ^= true, 0, 0, "Stop Timewarp On Encounter");
        Builder.CreateSeparator(box, elementWidth - 20);
        Builder.CreateToggleWithLabel(box, elementWidth, ToggleHeight, () => Config.settingsData.moreCameraZoom,
            () => Config.settingsData.moreCameraZoom ^= true, 0, 0, "More Camera Zoom");
        Builder.CreateSeparator(box, elementWidth - 20);
        Builder.CreateToggleWithLabel(box, elementWidth, ToggleHeight, () => Config.settingsData.moreCameraMove,
            () => Config.settingsData.moreCameraMove ^= true, 0, 0, "More Camera Movement");

        return box.gameObject;
    }

    static GameObject GetCheatsSettings(Transform parent, Vector2Int size)
    {
        Box box = Builder.CreateBox(parent, size.x, size.y);
        box.CreateLayoutGroup(Type.Vertical, TextAnchor.UpperCenter, 35, new RectOffset(15, 15, 15, 15));

        int elementWidth = size.x - 60;

        Builder.CreateLabel(box, elementWidth, 50, 0, 0, "Cheats");
        Builder.CreateToggleWithLabel(box, elementWidth, ToggleHeight, () => Config.settingsData.allowTimeSlowdown,
            () => Config.settingsData.allowTimeSlowdown.Value ^= true, 0, 0, "Allow Time Slowdown");
        Builder.CreateSeparator(box, elementWidth - 20);
        Builder.CreateToggleWithLabel(box, elementWidth, ToggleHeight, () => Config.settingsData.higherPhysicsWarp,
            () => Config.settingsData.higherPhysicsWarp ^= true, 0, 0, "Higher Physics Timewarps");

        return box.gameObject;
    }

    static GameObject GetWindowSettings(Transform parent, Vector2Int size)
    {
        Box box = Builder.CreateBox(parent, size.x, size.y);
        box.CreateLayoutGroup(Type.Vertical, TextAnchor.UpperCenter, 35, new RectOffset(15, 15, 15, 15));

        int elementWidth = size.x - 60;

        Builder.CreateLabel(box, elementWidth, 50, 0, 0, "Windows");

        Container container = Builder.CreateContainer(box);
        container.CreateLayoutGroup(Type.Horizontal, TextAnchor.MiddleLeft, 0);

        CustomUI.AlignedLabel(container, elementWidth - 225, ToggleHeight, "Window Scale");

        Container SliderLabel = Builder.CreateContainer(container);
        SliderLabel.CreateLayoutGroup(Type.Horizontal, TextAnchor.MiddleRight);

        Builder.CreateSlider(SliderLabel, 225, Config.settingsData.persistentVars.windowScale.Value, (0.5f, 1.5f),
            false,
            val => { Config.settingsData.persistentVars.windowScale.Value = val; },
            val => { return Math.Round(val * 100) + "%"; });

        return box.gameObject;
    }
}

public class CategoriesWindow
{
    readonly List<Button> categoriesButtons = new();

    readonly List<(string name, Func<Transform, GameObject> createWindowFunc)> elements = new();
    readonly int windowID = Builder.GetRandomID();
    Box categoriesButtonsBox;
    GameObject currentDataScreen;

    GameObject holder;

    bool initialized;
    Window mainWindow;
    string uniqueName;

    public Vector2Int RecommendedContentSize { get; private set; }

    public void Initialize(int width, int height, int posX, int posY, string title,
        Builder.SceneToAttach attachMode, string uniqueName)
    {
        if (initialized)
            return;
        initialized = true;

        holder = Builder.CreateHolder(attachMode, "CategoriesWindow Holder");
        holder.SetActive(false);

        this.uniqueName = uniqueName;
        Vector2Int position =
            Config.settingsData.windowsSavedPosition.GetValueOrDefault(uniqueName, new Vector2Int(posX, posY));
        BuildBase(width, height, position.x, position.y, title, attachMode);
    }

    void BuildBase(int width, int height, int posX, int posY, string title, Builder.SceneToAttach attachMode)
    {
        // Window
        mainWindow = Builder.CreateWindow(holder.transform, windowID, width, height, posX, posY, false, true, 1,
            title);
        mainWindow.CreateLayoutGroup(Type.Horizontal, TextAnchor.UpperLeft,
            padding: new RectOffset(20, 20, 20, 20));
        mainWindow.gameObject.GetComponent<DraggableWindowModule>().OnDropAction +=
            () => WindowManager.Save(uniqueName, mainWindow);

        // Categories Buttons
        categoriesButtonsBox = Builder.CreateBox(mainWindow, 150, 10, opacity: 0.15f);
        categoriesButtonsBox.CreateLayoutGroup(Type.Vertical, TextAnchor.UpperCenter, 10,
            new RectOffset(10, 10, 10, 10));
        categoriesButtonsBox.gameObject.GetOrAddComponent<ContentSizeFitter>().verticalFit =
            ContentSizeFitter.FitMode.PreferredSize; // Vertical Auto-resizing

        RecommendedContentSize = new Vector2Int(width - 210, height - 100);

        // Placeholder
        Box dataBox = Builder.CreateBox(mainWindow, RecommendedContentSize.x, RecommendedContentSize.y);
        currentDataScreen = dataBox.gameObject;
    }

    public void Disable()
    {
        holder.SetActive(false);
    }

    public void Enable()
    {
        if (elements.Count == 0)
            SetScreen(string.Empty);
        else
            SetScreen(elements[0].name);
        holder.SetActive(true);
    }

    void SetScreen(string name)
    {
        if (name == string.Empty)
            SetPlaceholder();
        else
            SetByName(name);
    }

    void SetPlaceholder()
    {
        Object.Destroy(currentDataScreen);
        Box dataBox = Builder.CreateBox(mainWindow, RecommendedContentSize.x, RecommendedContentSize.y);
        currentDataScreen = dataBox.gameObject;
    }

    void SetByName(string name)
    {
        Object.Destroy(currentDataScreen);
        currentDataScreen = elements.FirstOrDefault(x => x.name == name).createWindowFunc?.Invoke(mainWindow);
        foreach (Button button in categoriesButtons)
            if (button.Text == name)
                button.gameObject.GetComponent<ButtonPC>().SetSelected(true);
            else
                button.gameObject.GetComponent<ButtonPC>().SetSelected(false);

        LayoutRebuilder.ForceRebuildLayoutImmediate(currentDataScreen.transform as RectTransform);
    }

    public void AddMenu(string name, Func<Transform, GameObject> createWindowFunc)
    {
        elements.Add((name, createWindowFunc));
        Button selectButton =
            Builder.CreateButton(categoriesButtonsBox, 130, 60, 0, 0, () => SetScreen(name), name);
        categoriesButtons.Add(selectButton);
    }
}