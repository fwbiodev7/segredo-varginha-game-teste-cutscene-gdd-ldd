using System.Collections;
using UnityEngine;

namespace Game.Varginha
{
    /// <summary>ETs fire at captive students' energy cages until Edelzio engages them.</summary>
    [RequireComponent(typeof(VarginhaCombatEnemy))]
    public sealed class VarginhaClassroomPressure : MonoBehaviour
    {
        private VarginhaCombatEnemy _enemy;
        private VarginhaCombatTarget _target;
        private EdelzioTopDownController _player;
        private float _nextShot;
        private GameObject _projectile;

        private void Awake()
        {
            _enemy=GetComponent<VarginhaCombatEnemy>();
            _target=GetComponent<VarginhaCombatTarget>();
            _nextShot=Time.time+1.2f+Mathf.Abs(transform.position.x)*.12f;
        }

        private void Update()
        {
            if(_player==null) _player=FindAnyObjectByType<EdelzioTopDownController>();
            if(_player==null || Time.timeScale<=0 || VarginhaWorldFeedback.IsHidden || _projectile!=null
                || _target.IsDead || _enemy.State!=VarginhaCombatEnemy.CombatState.Pursuing
                || _player.IsInputLocked || Time.time<_nextShot) return;
            // The normal combat AI takes over as soon as Edelzio approaches.
            if(Vector2.Distance(_player.transform.position,transform.position)<6.1f) return;
            VarginhaStudentHostage nearest=null;
            float best=4f;
            foreach(var student in FindObjectsByType<VarginhaStudentHostage>(FindObjectsInactive.Exclude))
            {
                float distance=Vector2.Distance(transform.position,student.transform.position);
                if(!student.IsCaged || distance>=best
                    || !VarginhaSchoolNavigation.CanWalkSegment(transform.position,student.transform.position)) continue;
                best=distance; nearest=student;
            }
            _nextShot=Time.time+2.4f;
            if(nearest!=null) StartCoroutine(FireAtCage(nearest));
        }

        private IEnumerator FireAtCage(VarginhaStudentHostage student)
        {
            _projectile=new GameObject("Disparo_ET_Jaula");
            _projectile.transform.SetParent(transform.parent,false);
            var renderer=_projectile.AddComponent<SpriteRenderer>();
            renderer.sprite=VarginhaSceneryArt.Create("Dust",new Vector2(.16f,.16f));
            renderer.color=new Color(.5f,1f,.75f);
            renderer.sortingOrder=24;
            Vector3 start=transform.position;
            float elapsed=0;
            while(elapsed<.4f && student!=null && student.IsCaged && !_target.IsDead)
            {
                if(VarginhaWorldFeedback.IsHidden || _enemy.State!=VarginhaCombatEnemy.CombatState.Pursuing) break;
                elapsed+=Time.deltaTime;
                _projectile.transform.position=Vector3.Lerp(start,student.transform.position,elapsed/.4f);
                yield return null;
            }
            if(elapsed>=.4f && student!=null && student.IsCaged) student.ReceiveCageImpact();
            if(_projectile!=null) Destroy(_projectile);
            _projectile=null;
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            if(_projectile!=null) Destroy(_projectile);
            _projectile=null;
        }
    }
}
