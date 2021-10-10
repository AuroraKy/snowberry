﻿using Celeste;
using Microsoft.Xna.Framework;
using Monocle;

namespace LevelEditorMod.Editor.Entities {
    [EntityPlugin("jumpThru")]
    public class Plugin_JumpThru : EntityPlugin {
        [EntityOption("texture")] public string Texture = "default";
        [EntityOption("surfaceIndex")] public int SurfaceIndex = -1;

        internal override void Render() {
            base.Render();

            string name = Texture == "default" ? "wood" : Texture;
            MTexture tex = GFX.Game[$"objects/jumpthru/{name}"];

            int w = Width / 8;
            for (int i = 0; i < w; i++) {
                int tx = 1, ty = 0;

                if (i == 0) {
                    tx = 0;
                    ty = Room.GetFgTile(Position - Vector2.UnitX) == '0' ? 1 : 0;
                } else if (i == w - 1) {
                    tx = 2;
                    ty = Room.GetFgTile(Position + Vector2.UnitX * Width) == '0' ? 1 : 0;
                }

                tex.GetSubtexture(tx * 8, ty * 8, 8, 8).Draw(Position + Vector2.UnitX * i * 8);
            }
        }
    }
}
