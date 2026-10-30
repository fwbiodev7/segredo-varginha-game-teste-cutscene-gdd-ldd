namespace Game.Varginha
{
    /// <summary>Stable save indices: the nine original students, followed by the final-battle recruits.</summary>
    public static class VarginhaStudentRoster
    {
        public const int Count = 12;
        public static string Name(int index) => index < 9 && index >= 0
            ? VarginhaPhase2Controller.StudentNames[index]
            : index == 9 ? "Giovana" : index == 10 ? "Bianca" : index == 11 ? "Moscardini" : string.Empty;

        // Keep Anna's historical ID for scenes, dialogue, powers and saved progress.
        public static string DisplayName(string name) => name == "Anna Sabia" ? "Anna Sabiá" : name;
        public static string DisplayName(int index) => DisplayName(Name(index));
    }
}
