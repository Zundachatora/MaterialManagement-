using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;

public class CursorCompositeScript : InputBindingComposite<Vector2>
{
    [InputControl(layout = "Button")] public int press;
    [InputControl(layout = "Vector2")] public int position;

#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoadMethod]
#else
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
#endif
    private static void Initialize()
    {
        InputSystem.RegisterBindingComposite<CursorCompositeScript>("PressPosition");
    }

    public override Vector2 ReadValue(ref InputBindingCompositeContext context)
    {
        return context.ReadValue<Vector2, Vector2MagnitudeComparer>(position);
    }

    public override float EvaluateMagnitude(ref InputBindingCompositeContext context)
    {
        return context.ReadValue<float>(press);
    }
}