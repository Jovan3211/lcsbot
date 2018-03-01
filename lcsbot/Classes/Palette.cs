using Discord;

namespace lcsbot
{
    static class Palette
    {
        static private Color pink = new Color(252, 81, 133);
        static private Color white = new Color(245, 245, 245);
        static private Color purple = new Color(106, 44, 112);

        static public Color Pink { get => pink; set => pink = value; }
        static public Color White { get => white; set => white = value; }
        static public Color Purple { get => purple; set => purple = value; }
    }
}
