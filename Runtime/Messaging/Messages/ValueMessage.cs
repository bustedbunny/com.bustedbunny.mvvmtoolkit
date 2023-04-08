namespace MVVMToolkit.Messaging
{
    public abstract class ValueMessage<TType, TValue> : BaseValueMessage<TValue>
        where TType : BaseValueMessage<TValue>, new()
    {
        private static readonly TType Instance = new();

        public static TType Message(TValue value)
        {
            Instance.value = value;
            return Instance;
        }
    }
}