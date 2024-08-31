namespace Zafiro;

public class Option
{
    public Option(string name, OptionValue optionValue = OptionValue.None)
    {
        Name = name;
        OptionValue = optionValue;
    }

    public string Name { get; }
    public OptionValue OptionValue { get; }
}