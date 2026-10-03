using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Varginha.Experiment
{
    [Serializable]
    public sealed class CampaignStory
    {
        public int version = 1, phase = 1, routine, inspection;
        public bool boxFound, pagesSolved, arrival, renanMet, lesson, archive, buildingSolved;
        public bool symbolsFound, legendFound, codeSolved, renanConfirmed;
        public int studentsTalked;
        public bool ouzanaNote;
        public int[] pages = { 2, 0, 1 }, symbols = { 1, 0, 1, 0 };
        public int circle = 1, triangle = 1;
        public float driveDistance, x, y;
        public int positionPhase;
        public CampaignExpansionState expansion = new();
        public int MapFragments => (pagesSolved ? 1 : 0) + (buildingSolved ? 1 : 0) + ((expansion.visited & 1) != 0 ? 1 : 0);
        public bool CanLeaveHouse => routine == 15 && pagesSolved;
        public string RemainingHouseTasks
        {
            get
            {
                var pending=new System.Collections.Generic.List<string>();
                if((routine&1)==0)pending.Add("lavar o rosto na pia da cozinha");
                if((routine&2)==0)pending.Add("tomar café");
                if((routine&4)==0)pending.Add("preparar o notebook");
                if((routine&8)==0)pending.Add("pegar a mochila");
                return string.Join(", ",pending);
            }
        }
        public bool CanSolveBuilding => renanMet && lesson && archive;
        public bool CanDecode => buildingSolved && symbolsFound && legendFound;
        public bool SubmitPages()
        {
            if (!boxFound || routine != 15 || !Sequence(pages, new[] { 0, 1, 2 })) return false;
            pagesSolved = true; return true;
        }
        public bool SubmitBuilding(int choice)
        {
            if (!CanSolveBuilding || choice != 1) return false;
            buildingSolved = true; return true;
        }
        public bool SubmitCode()
        {
            if (!CanDecode || circle != 2 || triangle != 3 || !Sequence(symbols, new[] { 0, 1, 0, 1 })) return false;
            codeSolved = true; return true;
        }
        public static bool Sequence(int[] a, int[] b)
        {
            if (a == null || a.Length != b.Length) return false;
            for (int i = 0; i < a.Length; i++) if (a[i] != b[i]) return false;
            return true;
        }
        public void Repair()
        {
            phase = Mathf.Clamp(phase, 1, 10); routine &= 15; inspection &= 7;
            expansion ??= new CampaignExpansionState(); expansion.Repair();
            if (pages == null || pages.Length != 3 || !IsPermutation(pages)) pages = new[] { 2, 0, 1 };
            if (symbols == null || symbols.Length != 4) symbols = new[] { 1, 0, 1, 0 };
            for (int i = 0; i < symbols.Length; i++) symbols[i] = Mathf.Clamp(symbols[i], 0, 1);
            circle = Mathf.Clamp(circle, 1, 4); triangle = Mathf.Clamp(triangle, 1, 4);
            if (float.IsNaN(driveDistance) || float.IsInfinity(driveDistance)) driveDistance = 0;
            driveDistance = Mathf.Clamp(driveDistance, 0, 120);
            if (float.IsNaN(x) || float.IsInfinity(x) || float.IsNaN(y) || float.IsInfinity(y)) positionPhase = 0;
            if (!boxFound || routine != 15) pagesSolved = false;
            if (!CanSolveBuilding) buildingSolved = false;
            if (!CanDecode) codeSolved = false;
            if (!codeSolved) renanConfirmed = false;
        }
        private static bool IsPermutation(int[] a)
            => a[0] >= 0 && a[0] < 3 && a[1] >= 0 && a[1] < 3 && a[2] >= 0 && a[2] < 3 && a[0] != a[1] && a[0] != a[2] && a[1] != a[2];
    }
    public static class CampaignStorySave
    {
        public static string Path => System.IO.Path.Combine(Application.persistentDataPath, "CampaignStory2026.json");
        public static bool Exists => File.Exists(Path) || CampaignMemorySave.Exists;
        public static CampaignStory Load()
        {
            try
            {
                var data = File.Exists(Path) ? JsonUtility.FromJson<CampaignStory>(File.ReadAllText(Path)) : null;
                if (data != null && data.version == 1) { data.Repair(); return data; }
            }
            catch (Exception e) when (e is IOException || e is ArgumentException || e is UnauthorizedAccessException)
            { Debug.LogWarning("Progresso da campanha recuperado: " + e.GetType().Name); }
            return new CampaignStory();
        }
        public static void Write(CampaignStory data)
        {
            string temp=Path+"."+Guid.NewGuid().ToString("N")+".tmp";
            try
            {
                Directory.CreateDirectory(Application.persistentDataPath);
                File.WriteAllText(temp, JsonUtility.ToJson(data, true));
                for(int attempt=0;;attempt++)
                {
                    try{if(File.Exists(Path))File.Replace(temp,Path,null);else File.Move(temp,Path);break;}
                    catch(IOException) when(attempt<4){System.Threading.Thread.Sleep(10);}
                }
            }
            catch (Exception e) when (e is IOException || e is UnauthorizedAccessException || e is PlatformNotSupportedException)
            { Debug.LogWarning("Não foi possível salvar a campanha: " + e.GetType().Name); }
            finally { try{if(File.Exists(temp))File.Delete(temp);}catch(IOException){}catch(UnauthorizedAccessException){} }
        }
        public static string Scene(int phase) => phase == 1 ? VarginhaCampaignPhase1.SceneName
            : phase == 3 ? VarginhaCampaignDrive.SceneName : phase >= 6 ? CampaignExpansionController.SceneName(phase)
            : "Ato2_Fase" + phase + "_" + (phase == 2 ? "A_Chave_e_a_Caixa" : phase == 4 ? "Entre_Aulas_e_Pistas" : "O_Codigo_das_2323");
        public static void GoTo(int phase)
        {
            string scene = Scene(phase);
            if (!Application.CanStreamedLevelBeLoaded(scene)) { Debug.LogError("Cena da campanha ausente: " + scene); return; }
            var progress = Load(); progress.phase = phase; progress.positionPhase = 0; Write(progress);
            Time.timeScale = 1; SceneManager.LoadScene(scene);
        }
    }
}
