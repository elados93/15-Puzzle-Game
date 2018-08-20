namespace WpfApp.Misc {
    public enum Level {
        Easy, Medium, Hard
    }

    public enum Direction {
        UP = 1, RIGHT = 2, DOWN = 3, LEFT = 4, NO_DIRECTION = 5
    }

    public class LevelClass {
        public static Level getLevelFromString(string s) {
            if (s.Equals("Easy"))
                return Level.Easy;
            if (s.Equals("Medium"))
                return Level.Medium;
            return Level.Hard;
        }
    }
}
