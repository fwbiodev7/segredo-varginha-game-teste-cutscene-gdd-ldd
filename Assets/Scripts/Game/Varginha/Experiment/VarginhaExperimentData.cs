using System;
using System.IO;
using UnityEngine;

namespace Game.Varginha.Experiment
{
    [Serializable] public class ExperimentShot
    {
        public string id, asset, mode, title, subtitle, voice;
        public float duration;
        public int panel;
    }
    [Serializable] public class ExperimentPuzzle
    {
        public string id, title, description, hint, reward, code;
        public string[] items;
        public int[] solution, initial;
    }
    [Serializable] public class ExperimentCharacter
    {
        public string id, name, role, description;
        public int cell;
    }
    [Serializable] public class ExperimentDefinition
    {
        public string title;
        public ExperimentShot[] shots;
        public ExperimentPuzzle[] puzzles;
        public ExperimentCharacter[] characters;
        public static ExperimentDefinition Load()
        {
            var source = Resources.Load<TextAsset>("Varginha/Experiment/ExperimentDefinition");
            if (source == null) throw new InvalidOperationException("Dados do laboratório ausentes.");
            var data = JsonUtility.FromJson<ExperimentDefinition>(source.text);
            if (data?.shots == null || data.puzzles == null || data.characters == null)
                throw new InvalidOperationException("Dados do laboratório incompletos.");
            return data;
        }
    }
    [Serializable] public class ExperimentProgress
    {
        public int version = 1;
        public bool openingSeen, mapSolved, documentsSolved, codeSolved;
        public int[] mapOrder, documentOrder;
        public string codeInput = "";
        public bool CanOpen(int index) => index == 0 || (index == 1 && mapSolved) || (index == 2 && documentsSolved);
        public bool IsSolved(int index) => index == 0 ? mapSolved : index == 1 ? documentsSolved : codeSolved;
        public void FinishOpening() => openingSeen = true;
        public bool Submit(ExperimentDefinition data, int index)
        {
            if (index < 0 || index >= data.puzzles.Length || !CanOpen(index)) return false;
            var puzzle = data.puzzles[index];
            bool valid = index == 2 ? codeInput == puzzle.code
                : Matches(index == 0 ? mapOrder : documentOrder, puzzle.solution);
            if (!valid) return false;
            if (index == 0) mapSolved = true;
            else if (index == 1) documentsSolved = true;
            else codeSolved = true;
            return true;
        }
        public void Repair(ExperimentDefinition data)
        {
            if (!IsPermutation(mapOrder, data.puzzles[0].items.Length))
                mapOrder = (int[])data.puzzles[0].initial.Clone();
            if (!IsPermutation(documentOrder, data.puzzles[1].items.Length))
                documentOrder = (int[])data.puzzles[1].initial.Clone();
            // A save cannot bypass the investigation chain.
            if (!mapSolved) { documentsSolved = false; codeSolved = false; }
            if (!documentsSolved) codeSolved = false;
            if (codeInput == null || codeInput.Length > 4 || !IsDigits(codeInput)) codeInput = "";
        }
        public static bool Matches(int[] actual, int[] expected)
        {
            if (actual == null || expected == null || actual.Length != expected.Length) return false;
            for (int i = 0; i < actual.Length; i++) if (actual[i] != expected[i]) return false;
            return true;
        }
        public static bool IsPermutation(int[] order, int count)
        {
            if (order == null || order.Length != count) return false;
            var seen = new bool[count];
            foreach (var value in order)
            {
                if (value < 0 || value >= count || seen[value]) return false;
                seen[value] = true;
            }
            return true;
        }
        private static bool IsDigits(string value)
        {
            foreach (char digit in value) if (digit < '0' || digit > '9') return false;
            return true;
        }
    }
    public static class ExperimentSave
    {
        public static string Path => System.IO.Path.Combine(Application.persistentDataPath, "ExperimentGDDLDDSave.json");
        public static ExperimentProgress Load(ExperimentDefinition data)
        {
            ExperimentProgress progress = null;
            try
            {
                if (File.Exists(Path)) progress = JsonUtility.FromJson<ExperimentProgress>(File.ReadAllText(Path));
            }
            catch (Exception error) when (error is IOException || error is ArgumentException || error is UnauthorizedAccessException)
            { Debug.LogWarning("Save experimental recuperado: " + error.GetType().Name); }
            if (progress == null || progress.version != 1) progress = new ExperimentProgress();
            progress.Repair(data);
            return progress;
        }
        public static void Write(ExperimentProgress progress)
        {
            try
            {
                Directory.CreateDirectory(Application.persistentDataPath);
                string temporary = Path + ".tmp";
                File.WriteAllText(temporary, JsonUtility.ToJson(progress, true));
                if (File.Exists(Path)) File.Replace(temporary, Path, null);
                else File.Move(temporary, Path);
            }
            catch (Exception error) when (error is IOException || error is UnauthorizedAccessException || error is PlatformNotSupportedException)
            { Debug.LogWarning("Não foi possível salvar o laboratório: " + error.GetType().Name); }
        }
    }
    public sealed class ExperimentTimeline
    {
        public readonly ExperimentShot[] Shots;
        public float Time { get; private set; }
        public bool Paused { get; set; }
        public float Duration { get; }
        public bool Finished => Time >= Duration;
        public int ShotIndex
        {
            get
            {
                float end = 0;
                for (int i = 0; i < Shots.Length; i++) { end += Shots[i].duration; if (Time < end) return i; }
                return Shots.Length - 1;
            }
        }
        public float ShotTime
        {
            get
            {
                float start = 0;
                for (int i = 0; i < ShotIndex; i++) start += Shots[i].duration;
                return Time - start;
            }
        }
        public ExperimentTimeline(ExperimentShot[] shots)
        {
            if (shots == null || shots.Length == 0) throw new ArgumentException("Timeline sem planos.");
            Shots = shots;
            foreach (var shot in shots)
            {
                if (shot.duration <= 0) throw new ArgumentException("Plano com duração inválida.");
                Duration += shot.duration;
            }
        }
        public void Tick(float delta) { if (!Paused) Time = Mathf.Clamp(Time + Mathf.Max(0, delta), 0, Duration); }
        public void Seek(float time) => Time = Mathf.Clamp(time, 0, Duration);
        public void Finish(ExperimentProgress progress) { Seek(Duration); progress.FinishOpening(); }
    }
}
