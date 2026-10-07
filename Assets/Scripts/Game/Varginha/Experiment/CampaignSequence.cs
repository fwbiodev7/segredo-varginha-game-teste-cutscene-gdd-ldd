using UnityEngine;

namespace Game.Varginha.Experiment
{
    // Serialized phase IDs and scene names stay stable. Chapters are player-facing.
    public static class CampaignSequence
    {
        public const int SaveVersion = 3;
        public const int Count = 14;
        public static readonly int[] Entries = { 1, 2, 4, 6, 7, 8, 9, 11, 12, 14, 15, 18, 20, 21 };
        private static readonly int[] Chapters = { 1, 2, 3, 3, 3, 4, 5, 6, 7, 7, 8, 9, 9, 10, 11, 12, 12, 12, 12, 13, 14 };
        private static readonly string[] Titles = {
            "O Caso de Varginha", "A Caixa Esquecida", "Entre Aulas e Pistas", "Fragmentos", "A Mata", "A Âncora",
            "Marcas no Fusca", "Arquivos Alterados", "O Casarão de 1898", "O Que Fábio Escondeu", "Antes da Explosão",
            "A Criatura e o Acordo", "A Batalha da Manifestação", "O Retorno"
        };
        private static readonly string[] Acts = { "A lembrança volta", "O caminho da capela", "Evidências que mudam", "A noite esquecida", "O verdadeiro segredo" };
        public static int Chapter(int id) => Chapters[Mathf.Clamp(id, 1, 21) - 1];
        public static string Title(int id) => Titles[Chapter(id) - 1];
        public static int Act(int id) => Chapter(id) >= 12 ? 5 : (Chapter(id) - 1) / 3 + 1;
        public static string Heading(int id) => "ATO " + Act(id) + " • " + Acts[Act(id) - 1] + " • FASE " + Chapter(id);
        public static int Resume(int id) => id == 3 || id == 5 ? 4 : id == 16 || id == 17 || id == 19 ? 18 : id;
        public static int Next(int id) => id == 2 || id == 3 ? 4 : id == 4 || id == 5 ? 6 : id == 15 || id == 16 || id == 17 ? 18 : id == 18 || id == 19 ? 20 : Mathf.Min(21, id + 1);
        public static string ContinueLabel(int id) => Chapter(id) == Chapter(Next(id))
            ? (id == 9 ? "IR À OFICINA" : "ENTRAR NO PORÃO") : "CONTINUAR • FASE " + Chapter(Next(id));

        public static void Migrate(CampaignStory story)
        {
            if (story.version >= SaveVersion) return;
            bool legacy = story.version < 2;
            int oldPhase = story.phase;
            story.phase = Resume(Mathf.Clamp(oldPhase, 1, 21));
            if (story.phase != oldPhase) story.positionPhase = 0;
            // Removed gates never invalidate previously acquired evidence.
            if (story.pagesSolved) story.boxFound = true;
            if (story.codeSolved) { story.renanMet = story.photoOpened = story.buildingSolved = story.timeOpened = true; story.schoolTimeChoice = 1; }
            var e=story.expansion ??= new CampaignExpansionState();
            if(e.testDriven || e.stabilized || e.sprayed==7){e.sprayed=7;e.stabilized=true;}
            if(e.sprayed!=0 || e.reagentUnlocked)e.anchorFound=e.evidencePresented=e.reagentUnlocked=true;
            if(e.anchorFound){e.anchorClues=7;e.anchorYear=1996;e.anchorSymbol=1;e.anchorRecord=23;}
            if(e.fabioMet){e.visited=7;e.mapSolved=true;e.forestSigns=3;}
            if(e.mapSolved)e.visited=7;
            var state = story.continuation;
            if(state.finished)state.finalStep=4;
            if (legacy && (state.finalStep > 0 || state.finished))
            { state.manifestationDispelled = true; state.chambersPrepared = 7; state.finalRegulators=new[]{1,0,1}; state.finalSealActive=state.finalStep<4; state.finalCalibrated = true; }
            else if (legacy && state.manifestationDispelled && (state.finalCalibrated || state.solved[9]))
            {state.finalCalibrated=true;state.finalRegulators=new[]{1,0,1};state.finalSealActive=true;state.chambersPrepared=7;}
            // The removed descent is completed, but its documents remain in their old slots.
            state.solved[5] = true;
            // Slot 6 remains serialized only to retain compatibility with old saves.
            state.solved[6] = true;
            story.version = SaveVersion;
        }
    }
}
