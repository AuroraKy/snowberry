﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Monocle;
using System;

namespace Snowberry.Editor.UI;

public class UIScrollPane : UIElement {
    public Color BG = Calc.HexToColor("202929") * (185 / 255f);
    public int BottomPadding = 0, TopPadding = 0;
    public bool ShowScrollBar = true;
    public bool Vertical = true;
    public float Scroll = 0;

    public UIScrollPane() {
        Background = BG;
        GrabsScroll = true;
        GrabsClick = true;
    }

    public override void Render(Vector2 position = default) {
        Rectangle rect = new Rectangle((int)position.X, (int)position.Y, Width, Height);
        // render the BG ourselves
        if (Background.HasValue)
            Draw.Rect(rect, Background.Value);

        DrawUtil.WithinScissorRectangle(rect, () => {
            base.Render(position + ScrollOffset());

            // this is extremely stupid
            // todo: make this not extremely stupid
            if (ShowScrollBar) {
                UIElement low = null, high = null;
                foreach (var item in Children) {
                    if (low == null || item.Position.Y > low.Position.Y) low = item;
                    if (high == null || item.Position.Y < high.Position.Y) high = item;
                }

                if (high != null && low != null) {
                    var scrollPoints = ScrollPoints(13);
                    var scrollSize = Math.Abs(scrollPoints.X - scrollPoints.Y);
                    var offset = position.Y - scrollPoints.X;
                    Draw.Rect(position + new Vector2(Width - 4, (offset / scrollSize) * (Height + 40)), 2, 40, Color.DarkCyan);
                }
            }
        });
    }

    private Vector2 ScrollOffset() {
        return (Vertical ? Vector2.UnitY : Vector2.UnitX) * Scroll;
    }

    public override void Update(Vector2 position = default) {
        bool hovered = Bounds.Contains((int)Editor.Mouse.Screen.X, (int)Editor.Mouse.Screen.Y);

        // pretend that the mouse has already been clicked if the mouse is outside of the scroll pane's bounds
        bool mouseClicked = Editor.MouseClicked;
        Editor.MouseClicked = !hovered || mouseClicked;
        base.Update(position + ScrollOffset());
        Editor.MouseClicked = mouseClicked;

        if (hovered)
            ScrollBy(MInput.Mouse.WheelDelta, 1);

        // TODO: make optional? or into UIElement behaviour?
        // fit to parent's height if not set, like for the tile brush panel
        Height = Height == 0 ? Parent?.Height ?? 0 : Height;
    }

    public override Vector2 BoundsOffset() => ScrollOffset();

    protected override bool RenderBg() => false;

    public void ScrollBy(int dir, float amount) {
        var points = ScrollPoints(13);
        if (dir > 0 && points.X < 0)
            Scroll += amount * 13;
        else if (dir < 0 && points.Y > Height)
            Scroll -= amount * 13;
    }

    // X,Y = Top, Bottom
    public Vector2 ScrollPoints(int scrollSpeed) {
        UIElement low = null, high = null;
        foreach (var item in Children) {
            if (low == null || item.Position.Y > low.Position.Y) low = item;
            if (high == null || item.Position.Y < high.Position.Y) high = item;
        }

        return new Vector2((high != null ? (high.Position.Y + scrollSpeed - TopPadding) : 0) + ScrollOffset().Y, (low != null ? (low.Position.Y + low.Height + scrollSpeed + BottomPadding) : 0) + ScrollOffset().Y);
    }
}