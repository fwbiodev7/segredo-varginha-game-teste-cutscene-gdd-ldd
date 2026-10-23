using System;
using UnityEngine;

namespace Game.Varginha.Experiment
{
    [Serializable]
    public sealed class CampaignHouseInvestigation
    {
        public int step, clues, tutorial;
        public bool deduced, bagReminder;
        public bool KeyOnCounter => step == 1;
        public bool Missing => step >= 2;
        public bool CanOpenBox => step >= 3 && deduced;
        public bool Complete => step >= 5;
        public bool RememberBag()
        {
            if(step!=1||bagReminder)return false;
            bagReminder=true;return true;
        }
        public bool PlaceKey(int routine)
        {
            if (step != 0 || (routine & 7) != 7) return false;
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
            if (step < 2 || clues != 7 || conclusion != 1) return false;
            deduced = true; step = Math.Max(step, 3); return true;
        }
        public void Repair(bool found, bool solved)
        {
            step = Mathf.Clamp(step, 0, 5); clues &= 7; tutorial &= 15;
            if (solved) { step = 5; deduced = true; clues = 7; }
            else if (found) { step = Math.Max(step, 4); deduced = true; clues = 7; }
            else if (step >= 4) step = 2;
            if (deduced && clues == 7) step = Math.Max(step, 3);
            else { deduced = false; if (step == 3) step = 2; }
            // Older saves already past the pickup must never repeat the new door reminder.
            if(step>=2)bagReminder=true;
        }
        public string Objective(int routine)
        {
            if (Complete) return "Chave e caderno guardados. Vá ao Fusca para seguir à Industrial.";
            if (step == 0) return (routine & 1) == 0 ? "Prepare-se para a aula: lave o rosto na pia."
                : (routine & 2) == 0 ? "Tome o café na cozinha antes de sair."
                : (routine & 4) == 0 ? "Confira o material da aula no notebook do escritório."
                : "Deixe a chave do Fusca na mesa do café antes de buscar a mochila.";
            if (step == 1) return bagReminder ? "Pegue a mochila no quarto antes de sair." : "Tudo preparado. Vá até a porta de saída.";
            if (!deduced) return clues == 7 ? "Compare as três pistas no registro da investigação. O que elas permitem concluir?"
                : "A chave sumiu. Confira a mesa, a porta e o rastro de papel perto do quarto.";
            return step < 4 ? "O ruído vem debaixo da cama. Reexamine a caixa de brinquedos."
                : "Reconstrua o desenho do caderno comparando as bordas rasgadas e os traços.";
        }
        public string Target(int routine) => Complete ? "car" : step == 0
            ? (routine & 1) == 0 ? "wash" : (routine & 2) == 0 ? "food" : (routine & 4) == 0 ? "work" : "counter"
            : step == 1 ? bagReminder ? "bag" : "exitclue" : deduced ? "box" : null;
        public string Hint(int routine, int level)
        {
            level = Mathf.Clamp(level, 0, 2);
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
                return level == 0 ? "Organize o que vai levar antes de buscar a mochila."
                    : level == 1 ? "A mesa do café deixa a chave à vista."
                    : "Aproxime-se da mesa do café e interaja para deixar a chave do Fusca.";
            }
            if (step == 1 && !bagReminder) return level == 0 ? "A rotina está pronta. É hora de sair."
                : level == 1 ? "A saída fica do lado direito da sala." : "Vá à porta da sala e tente sair.";
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
