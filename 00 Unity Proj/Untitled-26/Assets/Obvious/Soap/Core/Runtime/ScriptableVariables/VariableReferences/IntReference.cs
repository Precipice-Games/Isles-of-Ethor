namespace Obvious.Soap
{
    [System.Serializable]
    public class IntReference : VariableReference<IntVariable, int>
    {
        public IntReference()
        {
        }

        public IntReference(int initialValue, bool useLocal = false)
        {
            LocalValue = initialValue;
            UseLocal = useLocal;
        }
    }
}