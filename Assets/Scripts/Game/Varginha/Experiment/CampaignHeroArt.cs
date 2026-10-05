using System.Collections.Generic;
using UnityEngine;
namespace Game.Varginha.Experiment
{
    public static class CampaignHeroArt
    {
        private static readonly Dictionary<string,Sprite> Effects=new();
        public static Color ColorFor(string student)=>VarginhaStudentSprites.AssetName(student) switch
        {
            "Yasmin"=>new Color(.75f,.3f,1),"Pedro"=>new Color(1,.22f,.28f),
            "AnaTavares" or "AnnaSabia"=>new Color(1,.25f,.36f),"Marcos"=>new Color(.48f,.45f,1),
            "LuisMiguelMessias"=>new Color(.18f,.62f,1),"Fabio" or "Matias"=>new Color(.75f,.82f,1),_=>Color.white
        };
        public static string AttackName(string student,int variant)
        {
            var names=VarginhaStudentSprites.AssetName(student) switch
            {
                "Yasmin"=>new[]{"ONDA SONORA","PIANO DO CÉU"},"Pedro"=>new[]{"RIFF SONORO","ACORDE EM ÁREA"},
                "Matias"=>new[]{"COMBO DE QUEDAS","PROJEÇÃO"},"Fabio"=>new[]{"CORTE DE KATANA","ARREMESSO DE ANILHAS"},
                "Marcos"=>new[]{"SAQUE POTENTE","BOLA EM ÁREA"},"AnnaSabia"=>new[]{"RAQUETADA RÁPIDA","SPIN EM ÁREA"},
                "AnaTavares"=>new[]{"TOQUE PRECISO","CHUVA DE BOLAS"},"LuisMiguelMessias"=>new[]{"RIMA PESADA","SOLTA A VOZ"},
                _=>new[]{"JATO DE TINTA","EXPLOSÃO DE CORES"}
            };return names[Mathf.Clamp(variant,0,1)];
        }
        public static bool Available(string student)=>Resources.Load<TextAsset>("Varginha/StoryCharacters/Hero"+VarginhaStudentSprites.AssetName(student))!=null;
        public static Sprite Pose(string student,int variant,bool left,int stage)=>CampaignStorySprites.Frame("Hero"+VarginhaStudentSprites.AssetName(student),variant,(left?3:0)+stage);
        public static Sprite Effect(string student,int variant,int stage)
        {
            string name=VarginhaStudentSprites.AssetName(student),key=name+variant+stage;
            if(Effects.TryGetValue(key,out var sprite)&&sprite!=null)return sprite;
            var texture=Resources.Load<Texture2D>("Varginha/StoryCharacters/Hero"+name+"Effects");if(texture==null)return null;
            return Effects[key]=Sprite.Create(texture,new Rect(stage*128,(1-variant)*128,128,128),Vector2.one*.5f,48,0,SpriteMeshType.FullRect);
        }
    }
}
