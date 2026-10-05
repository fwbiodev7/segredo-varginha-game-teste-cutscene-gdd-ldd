using UnityEngine;

namespace Game.Varginha.Experiment
{
    public static class CampaignPresentation
    {
        // Keep a fixed scale and anchor each pose to the body/feet, rather than the atlas cell.
        // Only Sprite rectangles and pivots change; the source pixels remain untouched.
        public static Sprite AlignedFrame(Texture2D sheet, RectInt cell, float ppu, bool footPivot)
        {
            var pixels = sheet.GetPixels32();
            int left = cell.xMax, right = cell.xMin - 1, bottom = cell.yMax, top = cell.yMin - 1;
            for (int y = cell.yMin; y < cell.yMax; y++) for (int x = cell.xMin; x < cell.xMax; x++)
                if (pixels[y * sheet.width + x].a > 128)
                { left = Mathf.Min(left, x); right = Mathf.Max(right, x); bottom = Mathf.Min(bottom, y); top = Mathf.Max(top, y); }
            if (right < left) return Sprite.Create(sheet, new Rect(cell.x, cell.y, cell.width, cell.height), new Vector2(.5f, .5f), ppu);
            // Anchor using head/torso center; swinging hands and feet must not shift the body sideways.
            int upperLeft = right, upperRight = left, threshold = bottom + (top - bottom) * 2 / 3;
            for (int y = threshold; y <= top; y++) for (int x = left; x <= right; x++)
                if (pixels[y * sheet.width + x].a > 128) { upperLeft = Mathf.Min(upperLeft, x); upperRight = Mathf.Max(upperRight, x); }
            float anchorX = (upperLeft + upperRight + 1) * .5f;
            float anchorY = footPivot ? bottom + 2 : bottom + cell.height * .40f;
            // Retain the full authored cell: the original backpack compositor paints in this
            // 64-pixel coordinate system. Trimming it would shrink/misplace straps and pockets.
            var sprite = Sprite.Create(sheet, new Rect(cell.x, cell.y, cell.width, cell.height),
                new Vector2((anchorX - cell.x) / cell.width, (anchorY - cell.y) / cell.height), ppu, 0, SpriteMeshType.FullRect);
            return sprite;
        }
        public static void FootCollision(EdelzioTopDownController actor, bool child)
        {
            var circle = actor.GetComponent<CircleCollider2D>();
            if (circle != null)
            {
                circle.radius = child ? .22f : .23f;
                // Adult atlas origin is at the waist; child's origin is at the feet.
                circle.offset = child ? new Vector2(0, .06f) : new Vector2(0, -.58f);
            }
            var body = actor.GetComponent<Rigidbody2D>();
            if (body != null) { body.interpolation = RigidbodyInterpolation2D.Interpolate; body.collisionDetectionMode = CollisionDetectionMode2D.Continuous; body.constraints = RigidbodyConstraints2D.FreezeRotation; }
            CampaignWallBody.Ensure(actor,child);
        }
        public static void QuietWorld()
        {
            foreach (var prop in Object.FindObjectsByType<InteractableProp>(FindObjectsInactive.Include)) prop.enabled = false;
            foreach (var enemy in Object.FindObjectsByType<EntityManifestationAI>(FindObjectsInactive.Include)) enemy.gameObject.SetActive(false);
            foreach (var enemy in Object.FindObjectsByType<VarginhaCombatEnemy>(FindObjectsInactive.Include)) enemy.gameObject.SetActive(false);
            foreach (var exit in Object.FindObjectsByType<FuscaLevelExit>(FindObjectsInactive.Include)) exit.enabled = false;
            if (VarginhaGameHUD.Instance != null)
            {
                VarginhaGameHUD.Instance.CloseDialogue();
                if (VarginhaCampaignStage.Active != null || CampaignExpansionController.Active != null) VarginhaGameHUD.Instance.CampaignInventoryOnly = true;
                else VarginhaGameHUD.Instance.enabled = false;
            }
            if (VarginhaNotebookQuiz.Instance != null) VarginhaNotebookQuiz.Instance.enabled = false;
            foreach (var attack in Object.FindObjectsByType<VarginhaPlayerAttack>()) attack.enabled = false;
        }
    }
}
