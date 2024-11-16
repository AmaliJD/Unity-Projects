public class Note
{
    public int raw;
    public int index;
    public int octave;
    public string name;

    public override string ToString()
    {
        if (name == null)
            return "Nan";

        return name + octave;
    }
}
