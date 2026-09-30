#!/usr/bin/env python3

from __future__ import annotations

import math
from pathlib import Path

from PIL import Image, ImageDraw


OUTPUT = Path(__file__).parent / "images" / "frames"
PREVIEW_OUTPUT = Path(__file__).resolve().parents[2] / "assets" / "animated-stars-preview.gif"
PANELS = ("left", "center", "right")
FRAME_COUNT = 24


def draw_frame(panel_index: int, frame_index: int) -> Image.Image:
    image = Image.new("RGBA", (90, 90), (7, 12, 30, 255))
    draw = ImageDraw.Draw(image)
    stars = ((12, 19), (27, 62), (46, 31), (68, 70), (81, 18), (17, 80))
    for star_index, (x, y) in enumerate(stars):
        global_x = x + panel_index * 90
        phase = frame_index / FRAME_COUNT * math.tau + global_x * 0.05 + star_index
        radius = 1.5 + 3.5 * ((math.sin(phase) + 1) / 2)
        color = (190, 220, 255, 225) if star_index % 2 else (225, 207, 255, 235)
        draw.ellipse((x - radius, y - radius, x + radius, y + radius), fill=color)
        draw.line((x - radius * 2, y, x + radius * 2, y), fill=color, width=1)
        draw.line((x, y - radius * 2, x, y + radius * 2), fill=color, width=1)
    return image


def main() -> None:
    OUTPUT.mkdir(parents=True, exist_ok=True)
    preview_frames = []
    for panel_index, panel_name in enumerate(PANELS):
        for frame_index in range(FRAME_COUNT):
            draw_frame(panel_index, frame_index).save(
                OUTPUT / f"{panel_name}-{frame_index:02d}.png"
            )

    for frame_index in range(FRAME_COUNT):
        combined = Image.new("RGBA", (270, 90))
        for panel_index in range(len(PANELS)):
            combined.alpha_composite(draw_frame(panel_index, frame_index), (panel_index * 90, 0))
        preview_frames.append(combined.convert("RGB"))

    PREVIEW_OUTPUT.parent.mkdir(parents=True, exist_ok=True)
    preview_frames[0].save(
        PREVIEW_OUTPUT,
        save_all=True,
        append_images=preview_frames[1:],
        duration=100,
        loop=0,
        optimize=True,
    )


if __name__ == "__main__":
    main()
