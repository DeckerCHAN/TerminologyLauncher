namespace TerminologyLauncher.GUI.Toolkits
{
    public class FieldReference<T>
    {
        public T Value { get; set; }

        public FieldReference(T value)
        {
            this.Value = value;
        }


    }
}
