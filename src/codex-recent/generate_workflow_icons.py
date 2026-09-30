#!/usr/bin/env python3

from pathlib import Path

from PIL import Image, ImageDraw, ImageFont


OUTPUT = Path(__file__).with_name("images")
FONT_PATHS = (
    "/System/Library/Fonts/SFNSRounded.ttf",
    "/System/Library/Fonts/Supplemental/Arial Unicode.ttf",
)
ICONS = {
    "recent1": "1",
    "recent2": "2",
    "recent3": "3",
    "voice": "MIC",
    "previous": "‹",
    "new": "+",
    "next": "›",
    "sidebar": "◫",
    "terminal": ">_",
    "review_panel": "◎",
    "explore": "◇",
    "explain": "?",
    "review_code": "✓",
    "optimize": "⚡",
    "refactor": "↻",
    "tests": "T",
    "document": "≡",
    "fix_bug": "✦",
    "security": "⛨",
    "git_status": "●",
    "git_diff": "±",
    "git_log": "≣",
    "git_commit": "✓",
    "create_pr": "PR",
    "git_push": "↑",
}


def load_font(size: int) -> ImageFont.FreeTypeFont | ImageFont.ImageFont:
    for path in FONT_PATHS:
        try:
            return ImageFont.truetype(path, size)
        except OSError:
            continue
    return ImageFont.load_default()


def make_icon(name: str, glyph: str) -> None:
    size = 180
    image = Image.new("RGBA", (size, size), (9, 15, 27, 255))
    draw = ImageDraw.Draw(image)
    draw.rounded_rectangle((7, 7, 173, 173), radius=34, fill=(14, 25, 43, 255), outline=(51, 215, 190, 255), width=6)
    draw.ellipse((24, 24, 156, 156), fill=(18, 42, 62, 255), outline=(116, 238, 216, 110), width=3)
    font = load_font(78 if len(glyph) == 1 else 58)
    bounds = draw.textbbox((0, 0), glyph, font=font)
    width = bounds[2] - bounds[0]
    height = bounds[3] - bounds[1]
    draw.text(((size - width) / 2, (size - height) / 2 - bounds[1] - 2), glyph, font=font, fill=(225, 255, 249, 255))
    image.resize((90, 90), Image.Resampling.LANCZOS).save(OUTPUT / f"{name}.png")


def main() -> None:
    OUTPUT.mkdir(parents=True, exist_ok=True)
    for name, glyph in ICONS.items():
        make_icon(name, glyph)


if __name__ == "__main__":
    main()
