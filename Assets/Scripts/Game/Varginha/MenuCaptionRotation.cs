using System;
using UnityEngine;

namespace Game.Varginha
{
    public static class MenuCaptionRotation
    {
        private const string PreferenceKey = "Oizlede.MenuCaption";
        private static readonly string[] Captions = {
            "EDELZIO AO CONTRÁRIO!",
            "BORA PROGRAMAR!",
            "VOU MANDAR O DIALETO!",
            "INIMIGO DO DIA, SEM MEDO DA ESCURIDÃO!"
        };

        public static int Count => Captions.Length;
        public static string Text(int index) => Captions[index];

        // Independent from Unity's gameplay random state; never repeat the last menu entry.
        public static int NextIndex(int previous, System.Random random)
        {
            if(previous < 0 || previous >= Count) return random.Next(Count);
            int next = random.Next(Count - 1);
            return next >= previous ? next + 1 : next;
        }

        public static string EnterMenu()
        {
            int index = NextIndex(PlayerPrefs.GetInt(PreferenceKey, -1), new System.Random(Guid.NewGuid().GetHashCode()));
            PlayerPrefs.SetInt(PreferenceKey, index);
            PlayerPrefs.Save();
            return Text(index);
        }
    }
}
