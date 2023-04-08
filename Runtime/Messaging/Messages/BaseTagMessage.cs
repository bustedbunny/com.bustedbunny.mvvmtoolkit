namespace MVVMToolkit.Messaging
{
    public abstract class BaseTagMessage<TType> where TType : new()
    {
        private static readonly TType Instance = new();

        public static TType Message() => Instance;
    }
}