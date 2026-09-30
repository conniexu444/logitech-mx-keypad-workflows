#!/usr/bin/env python3

from pathlib import Path
from PIL import Image, ImageDraw, ImageFont


SIZE = 192
SCALE = 4
CANVAS = SIZE * SCALE
OUTPUT = Path(__file__).parent / "images"


def canvas() -> tuple[Image.Image, ImageDraw.ImageDraw]:
    image = Image.new("RGB", (CANVAS, CANVAS), "#17191f")
    return image, ImageDraw.Draw(image)


def save(name: str, image: Image.Image) -> None:
    OUTPUT.mkdir(parents=True, exist_ok=True)
    image.resize((SIZE, SIZE), Image.Resampling.LANCZOS).save(OUTPUT / f"{name}.png")


def shape_icon(name: str, draw_shape) -> None:
    image, draw = canvas()
    draw.rounded_rectangle((20 * SCALE, 20 * SCALE, 172 * SCALE, 172 * SCALE), radius=28 * SCALE, fill="#242730")
    draw_shape(draw)
    save(name, image)


def main() -> None:
    shape_icon("square", lambda draw: draw.rounded_rectangle((55 * SCALE, 55 * SCALE, 137 * SCALE, 137 * SCALE), radius=8 * SCALE, fill="#f24e1e"))
    shape_icon("ellipse", lambda draw: draw.ellipse((51 * SCALE, 55 * SCALE, 141 * SCALE, 137 * SCALE), fill="#a259ff"))
    shape_icon("diamond", lambda draw: draw.polygon([(96 * SCALE, 45 * SCALE), (147 * SCALE, 96 * SCALE), (96 * SCALE, 147 * SCALE), (45 * SCALE, 96 * SCALE)], fill="#f7b84b"))
    shape_icon("triangle", lambda draw: draw.polygon([(96 * SCALE, 43 * SCALE), (149 * SCALE, 143 * SCALE), (43 * SCALE, 143 * SCALE)], fill="#1abcfe"))
    shape_icon("star", lambda draw: draw.regular_polygon((96 * SCALE, 96 * SCALE, 57 * SCALE), n_sides=5, rotation=-90, fill="#ffdc58"))

    def draw_connector(draw: ImageDraw.ImageDraw) -> None:
        width = 10 * SCALE
        draw.line((46 * SCALE, 132 * SCALE, 84 * SCALE, 88 * SCALE, 134 * SCALE, 58 * SCALE), fill="#0acf83", width=width, joint="curve")
        draw.polygon([(134 * SCALE, 58 * SCALE), (112 * SCALE, 58 * SCALE), (134 * SCALE, 80 * SCALE)], fill="#0acf83")

    shape_icon("connector", draw_connector)

    def draw_sticky(draw: ImageDraw.ImageDraw) -> None:
        draw.rounded_rectangle((49 * SCALE, 48 * SCALE, 143 * SCALE, 144 * SCALE), radius=8 * SCALE, fill="#ffdc58")
        draw.polygon([(119 * SCALE, 144 * SCALE), (143 * SCALE, 120 * SCALE), (143 * SCALE, 144 * SCALE)], fill="#e6b928")

    shape_icon("sticky", draw_sticky)

    def draw_text(draw: ImageDraw.ImageDraw) -> None:
        font_path = "/System/Library/Fonts/Supplemental/Arial Bold.ttf"
        font = ImageFont.truetype(font_path, 94 * SCALE)
        draw.text((96 * SCALE, 99 * SCALE), "T", font=font, fill="#ffffff", anchor="mm")

    shape_icon("text", draw_text)


if __name__ == "__main__":
    main()
