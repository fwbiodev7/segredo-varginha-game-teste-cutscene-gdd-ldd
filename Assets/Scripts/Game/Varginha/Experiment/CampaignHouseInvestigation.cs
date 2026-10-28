using System;
using UnityEngine;

namespace Game.Varginha.Experiment
{
    // Values 0..5 remain compatible with existing saves. New departure states are additive.
    public enum CampaignHouseState { Preparing=0, KeyOnTable=1, KeyMissing=2, CluesDeduced=3, BoxOpened=4, Ready=5, KeyInPocket=6, ReturningForBag=7 }
    [Serializable]
    public sealed class CampaignHouseInvestigation
    {
        public int step, clues, tutorial;
        public bool deduced, bagReminder;
        public CampaignHouseState State => (CampaignHouseState)step;
        public bool KeyOnCounter => State == CampaignHouseState.KeyOnTable;
        public bool HasInitialKey => State == CampaignHouseState.KeyInPocket || State == CampaignHouseState.ReturningForBag;
        public bool Missing => step >= 2 && step <= 5;
        public bool CanOpenBox => step >= 3 && step <= 5 && deduced;
        public bool Complete => State == CampaignHouseState.Ready;
        public bool KeyVisibleOnTable(int routine) => KeyOnCounter || State == CampaignHouseState.Preparing && (routine & 7) == 7;
        public bool TakeInitialKey(int routine)
        {
            if(State!=CampaignHouseState.Preparing || (routine&7)!=7)return false;
            step=(int)CampaignHouseState.KeyInPocket;return true;
        }
        public bool RememberBag()
        {
            if(State!=CampaignHouseState.KeyInPocket)return false;
            step=(int)CampaignHouseState.ReturningForBag;bagReminder=true;return true;
        }
        public bool PlaceKey(int routine)
        {
            if (State != CampaignHouseState.ReturningForBag || (routine & 7) != 7) return false;
            step = 1; return true;
        }
        public bool TakeBag()
        {
            if (step != 1 || !bagReminder) return false;
            step = 2; return true;
        }
        public bool Observe(int clue)
        {
            if (!Missing || clue < 0 || clue > 2) return false;
            clues |= 1 << clue; return true;
        }
        public bool Deduce(int conclusion)
        {
            if (!Missing || clues != 7 || conclusion != 1) return false;
            deduced = true; step = Math.Max(step, 3); return true;
        }
        public void Repair(bool found, bool solved)
        {
            if(step<0||step>7)step=0;clues &= 7; tutorial &= 15;
            if (solved) { step = 5; deduced = true; clues = 7; }
            else if (found) { step = Math.Max(step, 4); deduced = true; clues = 7; }
            else if (step == 4 || step == 5) step = 2;
            if(HasInitialKey){deduced=false;clues=0;bagReminder=State==CampaignHouseState.ReturningForBag;return;}
            if (deduced && clues == 7) step = Math.Max(step, 3);
            else { deduced = false; if (step == 3) step = 2; }
            // Older saves already past the pickup must never repeat the new door reminder.
            if(step>=1)bagReminder=true;
        }
        public string Objective(int routine)
        {
            if (Complete) return "Chave e caderno guardados. Vá ao Fusca para seguir à Industrial.";
            if(State==CampaignHouseState.KeyInPocket)return "Chave no bolso. Vá até o Fusca para sair.";
            if(State==CampaignHouseState.ReturningForBag)return "Volte à mesa do café e deixe a chave antes de buscar a mochila.";
            if (step == 0) return (routine & 1) == 0 ? "Prepare-se para a aula: lave o rosto na pia."
                : (routine & 2) == 0 ? "Tome o café na cozinha antes de sair."
                : (routine & 4) == 0 ? "Confira o material da aula no notebook do escritório."
                : "Pegue a chave do Fusca na mesa do café para sair.";
            if (step == 1) return "Pegue a mochila no quarto antes de sair.";
            if (!deduced) return clues == 7 ? "Compare as três pistas no registro da investigação. O que elas permitem concluir?"
                : "A chave sumiu. Confira a mesa, a porta e o rastro de papel perto do quarto.";
            return step < 4 ? "O ruído vem debaixo da cama. Reexamine a caixa de brinquedos."
                : "Reconstrua o desenho do caderno comparando as bordas rasgadas e os traços.";
        }
        public string Target(int routine) => Complete || State==CampaignHouseState.KeyInPocket ? "car" : State==CampaignHouseState.ReturningForBag ? "counter" : step == 0
            ? (routine & 1) == 0 ? "wash" : (routine & 2) == 0 ? "food" : (routine & 4) == 0 ? "work" : "counter"
            : step == 1 ? "bag" : deduced ? "box" : null;
        public string Hint(int routine, int level)
        {
            level = Mathf.Clamp(level, 0, 2);
            if(State==CampaignHouseState.KeyInPocket)return level==0?"Está tudo pronto para a primeira tentativa de sair.":level==1?"O Fusca está estacionado na rua, além da sala.":"Saia pela porta da sala, aproxime-se do motorista e interaja com o Fusca.";
            if(State==CampaignHouseState.ReturningForBag)return level==0?"A mochila ficou para trás.":level==1?"Volte para a cozinha antes de ir ao quarto.":"Interaja com a mesa do café para deixar a chave; depois pegue a mochila.";
            if (Complete) return level == 0 ? "O material da aula e os vestígios estão guardados."
                : level == 1 ? "A próxima etapa começa no quintal, junto ao carro."
                : "Vá ao Fusca e interaja para seguir à Industrial.";
            if (step == 0)
            {
                if ((routine & 1) == 0) return level == 0 ? "Antes da aula, termine a rotina da manhã."
                    : level == 1 ? "Procure a pia na cozinha, ao lado do fogão."
                    : "Aproxime-se da pia e use a ação de interação indicada na tela para lavar o rosto.";
                if ((routine & 2) == 0) return level == 0 ? "Ainda falta tomar o café antes de sair."
                    : level == 1 ? "O café está na cozinha, junto à mesa."
                    : "Aproxime-se do café e interaja; aguarde Edelzio terminar de beber.";
                if ((routine & 4) == 0) return level == 0 ? "Confira se o material da aula está preparado."
                    : level == 1 ? "O notebook ficou no escritório."
                    : "Interaja com o notebook no escritório para conferir o material da aula.";
                return level == 0 ? "Pegue o que precisa para sair."
                    : level == 1 ? "A chave do Fusca está na mesa do café."
                    : "Interaja com a mesa do café para pegar a chave e vá ao carro.";
            }
            if (step == 1) return level == 0 ? "Falta levar o material da aula."
                : level == 1 ? "A mochila ficou no quarto."
                : "Vá ao quarto e pegue a mochila; depois volte à mesa do café.";
            if (!deduced) return level == 0 ? "Compare o que mudou e o que permaneceu igual."
                : level == 1 ? "Confira a mesa, o trinco e a direção dos pedaços de papel."
                : "A porta não abriu e os papéis seguem até a cama. Vale procurar na caixa.";
            if (step < 4) return level == 0 ? "As pistas apontam para algo que ficou dentro da casa."
                : level == 1 ? "Escute o ruído perto da cama e observe a origem do papel."
                : "Reexamine a caixa de brinquedos debaixo da cama.";
            return level == 0 ? "A fotografia e o desenho retratam a mesma noite; a tinta atravessa os rasgos."
                : level == 1 ? "Uma borda termina no muro. Outra continua o galho. Faça os traços se encontrarem."
                : "Organize CASA → ÁRVORE → FIGURA. A escrita recente se revela sobre o desenho antigo.";
        }
    }
}
