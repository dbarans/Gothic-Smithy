namespace Smithy.Core
{
    public interface IInteractable
    {
        string GetPromptText();
        void Interact(InteractContext context);
    }

    public struct InteractContext
    {
        public UnityEngine.GameObject Player;
    }
}
