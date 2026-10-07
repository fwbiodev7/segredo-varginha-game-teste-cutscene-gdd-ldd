using System.Collections;
using System.Collections.Generic;
using Game.Player;
using Game.UI;
using UnityEngine;
using UnityEngine.InputSystem;
namespace Game.Varginha.Experiment
{
 public sealed class CampaignFinalAllies:MonoBehaviour
 {
  public static CampaignFinalAllies Active {get;private set;}
  public static readonly string[] Names={"Renan","Ouzana","Padre Fábio"};
  public static readonly string[] Descriptions={"PULSO DE INTERFERÊNCIA • Arcos laranja do notebook interrompem e atordoam os inimigos.","FRASCO REATIVO • Arremessa um frasco verde que cria uma nuvem de dano gradual e desaceleração.","AURA DE PROTEÇÃO • Cúpula dourada com cruzes reduz o dano em 55% e afasta os filhotes por seis segundos."};
  readonly float[] ready=new float[3]; readonly SpriteRenderer[] actors=new SpriteRenderer[3];
  readonly bool[] casting=new bool[3];readonly List<Vector2> followPath=new();float nextPath;
  readonly float[] captionUntil=new float[3];readonly Vector3[] captionPositions=new Vector3[3];
  static readonly Sprite[,] effects=new Sprite[3,6]; EdelzioTopDownController player; Vector2 shieldCentre; float shieldUntil,commandBusyUntil;
  static readonly Dictionary<Sprite,Sprite> arenaPriestSprites=new();
  public VarginhaStudentAllySquad Squad {get;private set;}
  public int SelectedSupport {get;private set;}=-1;
  public bool CombatActive {get;set;}
  public float DamageMultiplier=>Time.time<shieldUntil&&Vector2.Distance(shieldCentre,player.transform.position)<2.7f?.45f:1;
  public float Cooldown(int i)=>Mathf.Max(0,ready[i]-Time.time);
  public static Sprite Portrait(int i)=>i==0?CampaignStorySprites.Frame("RenanTeaching",0,0):i==1?CampaignStorySprites.Frame("OuzanaBiologist",0,0):VarginhaReferenceSprites.PadreFabio();
  public static Sprite PortraitForFinalScene(int i)=>ArenaPose(i,Portrait(i));
  static Sprite ArenaPose(int i,Sprite source)
  {
   if(i!=2||source==null)return source;
   if(arenaPriestSprites.TryGetValue(source,out var sprite)&&sprite!=null)return sprite;
   float ppu=source.pixelsPerUnit/1.7f;
   // Scale around the feet, keeping both the arena navigation anchor and church assets intact.
   var pivot=new Vector2(source.pivot.x/source.rect.width,(source.pivot.y-.58f*source.pixelsPerUnit+.58f*ppu)/source.rect.height);
   sprite=Sprite.Create(source.texture,source.rect,pivot,ppu,0,SpriteMeshType.FullRect);sprite.name=source.name+"_Arena";
   return arenaPriestSprites[source]=sprite;
  }
  public void Configure(EdelzioTopDownController p)
  {
   Active=this;player=p;Squad=VarginhaStudentAllySquad.BuildForFuturePhase(transform,p.transform,false);Squad.ActivateManualAllies(5,true);Squad.ConfigureSelectionSlots(3);
   for(int i=0;i<3;i++){var go=new GameObject(Names[i]+"_Apoio");go.transform.SetParent(transform);go.transform.position=p.transform.position;actors[i]=go.AddComponent<SpriteRenderer>();actors[i].sprite=ArenaPose(i,Portrait(i));actors[i].enabled=false;VarginhaWorldDepth.Ensure(actors[i]);}
  }
  public bool SelectSupport(int i)
  {
   if(i<0||i>=3)return false;SelectedSupport=SelectedSupport==i?-1:i;followPath.Clear();nextPath=0;
   for(int j=0;j<3;j++)actors[j].enabled=j==SelectedSupport;
   if(SelectedSupport>=0&&!casting[SelectedSupport]){var p=player.transform.position-Vector3.up*1.8f;actors[SelectedSupport].transform.position=CampaignContinuationController.Active.Plan.IsClear((Vector2)p-Vector2.up*.58f,.33f)?p:player.transform.position;}
   return true;
  }
  void LateUpdate()
  {
   if(SelectedSupport<0||casting[SelectedSupport]||player==null||Time.timeScale<=0||CampaignContinuationController.Active?.Modal==true)return;
   var actor=actors[SelectedSupport];Vector2 destination=(Vector2)player.transform.position+Vector2.down*1.8f;
   if(Time.time>=nextPath){VarginhaSchoolNavigation.FindPath(player.transform,actor.transform.position,destination,followPath);nextPath=Time.time+.3f;}
   while(followPath.Count>1&&VarginhaSchoolNavigation.CanWalkSegment(actor.transform.position,followPath[1]))followPath.RemoveAt(0);
   if(followPath.Count>0)actor.transform.position=Vector2.MoveTowards(actor.transform.position,followPath[0],3.5f*Time.deltaTime);
  }
  bool Running=>CombatActive&&player!=null&&!player.IsInputLocked&&CampaignContinuationController.Active?.Modal!=true&&Time.timeScale>0;
  void Update()
  {
   if(!Running)return;
   var keyboard=Keyboard.current;var pad=Gamepad.current;
   if(Time.time>=commandBusyUntil)
   {
    if(keyboard?.digit1Key.wasPressedThisFrame==true)Squad.TryInvokeSelectedAttack(0);
    else if(keyboard?.digit2Key.wasPressedThisFrame==true)Squad.TryInvokeSelectedAttack(1);
    else if(keyboard?.digit3Key.wasPressedThisFrame==true)Squad.TryInvokeSelectedAttack(2);
    else if(VarginhaInputBindings.WasPressedThisFrame(VarginhaInputAction.AllyCommand))Squad.TryInvokeAttack(null);
   }
   if(SelectedSupport>=0&&VarginhaInputBindings.WasPressedThisFrame(VarginhaInputAction.SupportCommand))InvokeSupport(SelectedSupport);
   if(Time.time<shieldUntil)foreach(var e in Object.FindObjectsByType<CampaignManifestationCombat>())if(e.IsMinor&&Vector2.Distance(e.transform.position,shieldCentre)<2.6f)e.Repel(shieldCentre,Time.deltaTime*3);
  }
  public bool InvokeSupport(int i){if(!Running||i<0||i>2||Cooldown(i)>0||Squad.CommandInProgress||Time.time<commandBusyUntil)return false;commandBusyUntil=Time.time+1;ready[i]=Time.time+(i==2?12:8);captionUntil[i]=Time.time+1.6f;captionPositions[i]=player.transform.position+Vector3.up*2.3f;StartCoroutine(Present(i));return true;}
  void OnGUI(){if(VarginhaGameHUD.Instance?.IsInventoryOpen==true)return;for(int i=0;i<3;i++)if(Time.time<captionUntil[i])CampaignAttackCaption.Draw(captionPositions[i],Names[i],new[]{"PULSO DE INTERFERÊNCIA","FRASCO REATIVO","AURA DE PROTEÇÃO"}[i],i==0?new Color(1,.57f,.22f):i==1?new Color(.2f,1,.58f):new Color(1,.83f,.3f),3);}
  public void DrawCommands(float x,float y)
  {
   for(int slot=0;slot<4;slot++)
   {
    var rect=new Rect(x+slot*135,y,slot==3?145:130,46);ExperimentGUI.Panel(rect);
    string label,status;
    if(slot<3)
    {
     var ally=slot<Squad.SelectedStudentIndices.Count?Squad.Allies[Squad.SelectedStudentIndices[slot]]:null;
     label=(slot+1)+" • "+(ally==null?"VAZIO":ally.StudentName.Replace("Luis Miguel Messias","L. Messias").Replace("Luis Martins","L. Martins"));
     status=ally==null?"EQUIPE NA MOCHILA":ally.IsAttacking?"ATACANDO":ally.ManualCooldownRemaining>0?"RECARGA "+Mathf.CeilToInt(ally.ManualCooldownRemaining)+"s":"PRONTO";
    }
    else
    {
     label=VarginhaInputBindings.DisplayName(VarginhaInputAction.SupportCommand)+" • "+(SelectedSupport<0?"APOIO":Names[SelectedSupport]);
     status=SelectedSupport<0?"EQUIPE NA MOCHILA":Cooldown(SelectedSupport)>0?"RECARGA "+Mathf.CeilToInt(Cooldown(SelectedSupport))+"s":"PRONTO";
    }
    PixelMenuTheme.Label(new Rect(rect.x+8,rect.y+5,rect.width-16,19),label.ToUpperInvariant(),7,ExperimentGUI.Paper);
    PixelMenuTheme.Label(new Rect(rect.x+8,rect.y+25,rect.width-16,16),status,7,status=="PRONTO"?ExperimentGUI.Accent:ExperimentGUI.Muted);
   }
  }
  Vector2 Target()
  {
   Vector2 p=(Vector2)player.transform.position+player.FacingDirection*3;float best=5.5f;
   foreach(var e in Object.FindObjectsByType<CampaignManifestationCombat>()){float d=Vector2.Distance(e.transform.position,player.transform.position);if(!e.Defeated&&d<best&&Clear(player.transform.position,e.transform.position)){best=d;p=e.transform.position;}}
   return p;
  }
  static bool Clear(Vector2 a,Vector2 b)
  {
   var map=CampaignContinuationController.Active?.transform.Find("Mapa_Campanha");if(map==null)return false;
   foreach(var h in Physics2D.LinecastAll(a,b))if(!h.collider.isTrigger&&h.collider.transform.IsChildOf(map))return false;return true;
  }
  void Hit(int i,Vector2 centre,float radius,float damage)
  {
   foreach(var e in Object.FindObjectsByType<CampaignManifestationCombat>())
   {
    if(e.Defeated||Vector2.Distance(e.transform.position,centre)>radius||!Clear(centre,e.transform.position))continue;
    if(i==0)e.Stagger(2.2f);else{e.Slow(3);e.Stagger(.55f);}
    e.GetComponent<VarginhaCombatTarget>().ReceiveHit(damage,((Vector2)e.transform.position-centre).normalized,.05f);
   }
  }
  static Sprite Effect(int row,int col)
  {
   if(effects[row,col]!=null)return effects[row,col];var t=Resources.Load<Texture2D>("Varginha/StoryCharacters/SupportEffects");
   return effects[row,col]=Sprite.Create(t,new Rect(col*128,(2-row)*128,128,128),Vector2.one*.5f,48,0,SpriteMeshType.FullRect);
  }
  IEnumerator Present(int i)
  {
   casting[i]=true;var actor=actors[i];Vector2 start=player.transform.position,end=Target();int side=end.x<start.x?0:3;
   actor.transform.position=start+Vector2.left*.7f;actor.sprite=ArenaPose(i,CampaignStorySprites.Frame("SupportAttacks",i,side));
   float t=0;while(t<.32f&&CombatActive){if(Running)t+=Time.deltaTime;yield return null;}
   if(!CombatActive){casting[i]=false;actor.sprite=ArenaPose(i,Portrait(i));yield break;}
   actor.sprite=ArenaPose(i,CampaignStorySprites.Frame("SupportAttacks",i,side+1));
   var go=new GameObject("Habilidade_"+Names[i]);go.transform.SetParent(transform);var fx=go.AddComponent<SpriteRenderer>();fx.sortingOrder=30000;fx.flipX=end.x<start.x;go.transform.position=start;
   t=0;float duration=i==2?6:i==1?.6f:.55f;
   if(i==2){shieldCentre=start;shieldUntil=Time.time+duration;player.GetComponent<HealthSystem>().Heal(20);}
   while(t<duration)
   {
    if(!CombatActive)break;if(!Running){yield return null;continue;}t+=Time.deltaTime;float u=Mathf.Clamp01(t/duration);
    fx.sprite=Effect(i==0?1:i==1?0:2,i==2?Mathf.Min(4,1+(int)(t*6)):i==1?Mathf.Min(1,(int)(u*2)):Mathf.Min(2,(int)(u*3)));
    if(i==1)go.transform.position=Vector2.Lerp(start,end,u)+Vector2.up*Mathf.Sin(u*Mathf.PI)*1.2f;
    else if(i==0){go.transform.position=Vector2.Lerp(start,end,u*.55f);go.transform.localScale=Vector3.one*(.5f+u*1.6f);}
    else{go.transform.position=start-Vector2.up*.15f;go.transform.localScale=Vector3.one*2.3f;fx.color=new Color(1,1,1,.85f);}
    yield return null;
   }
   if(i<2){Hit(i,i==0?start:end,i==0?5.5f:2,i==0?42:24);go.transform.position=end;fx.flipX=false;}
   actor.sprite=ArenaPose(i,CampaignStorySprites.Frame("SupportAttacks",i,side+2));t=0;float tail=i==1?2.6f:.45f,tick=.65f;
   while(t<tail)
   {
    if(!CombatActive)break;if(!Running){yield return null;continue;}t+=Time.deltaTime;
    fx.sprite=Effect(i==0?1:i==1?0:2,i==1?Mathf.Min(5,2+(int)(t*1.3f)):i==0?3:5);fx.color=new Color(1,1,1,1-t/tail);
    if(i==1&&t>=tick){tick+=.65f;Hit(1,end,2,6);}yield return null;
   }
   casting[i]=false;actor.sprite=ArenaPose(i,Portrait(i));followPath.Clear();nextPath=0;Destroy(go);
  }
  void OnDestroy(){if(Active==this)Active=null;}
 }
}
