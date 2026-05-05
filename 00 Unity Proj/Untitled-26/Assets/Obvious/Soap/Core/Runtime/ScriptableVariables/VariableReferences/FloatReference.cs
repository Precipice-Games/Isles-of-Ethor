namespace Obvious.Soap
{
    [System.Serializable]
    public class FloatReference : VariableReference<FloatVariable, float>
    {
        public FloatReference()
        {
        }
        
        public FloatReference(float initialValue, bool useLocal = false)
        {
            LocalValue = initialValue;
            UseLocal = useLocal;
        }
    }
}