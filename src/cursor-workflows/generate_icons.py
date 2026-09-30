#!/usr/bin/env python3

from pathlib import Path

from PIL import Image, ImageDraw, ImageFont


OUTPUT = Path(__file__).with_name("images")
FONT_PATHS = (
    "/System/Library/Fonts/SFNSRounded.ttf",
    "/System/Library/Fonts/Supplemental/Arial Bold.ttf",
)
ICON_DEFINITIONS = {
    "new_agent": ("AGT", ("NEW", "AGENT"), "main"),
    "open_folder": ("FLD", ("OPEN", "FOLDER"), "main"),
    "open_ide": ("IDE", ("OPEN", "IDE"), "main"),
    "changes": ("CHG", ("VIEW", "CHANGES"), "main"),
    "browser": ("WEB", ("OPEN", "BROWSER"), "main"),
    "files": ("FILE", ("SHOW", "FILES"), "main"),
    "new_terminal": (">_", ("NEW", "TERMINAL"), "main"),
    "terminal": (">_", ("SHOW", "TERMINAL"), "main"),
    "command_palette": ("CMD", ("COMMAND", "PALETTE"), "main"),
    "explore": ("EXP", ("EXPLORE", "CODE"), "prompt"),
    "explain": ("WHY", ("EXPLAIN", "CODE"), "prompt"),
    "review_code": ("REV", ("REVIEW", "CODE"), "prompt"),
    "optimize": ("FAST", ("OPTIMIZE", "CODE"), "prompt"),
    "refactor": ("REF", ("REFACTOR", "CODE"), "prompt"),
    "write_tests": ("T", ("WRITE", "TESTS"), "prompt"),
    "document": ("DOC", ("WRITE", "DOCS"), "prompt"),
    "fix_bug": ("BUG", ("FIX", "BUG"), "prompt"),
    "security": ("SEC", ("SECURITY", "AUDIT"), "prompt"),
    "git_status": ("STAT", ("GIT", "STATUS"), "git"),
    "git_diff": ("DIFF", ("GIT", "DIFF"), "git"),
    "git_log": ("LOG", ("GIT", "LOG"), "git"),
    "git_commit": ("COM", ("CREATE", "COMMIT"), "git"),
    "create_pr": ("PR", ("CREATE", "PR"), "git"),
    "git_push": ("PUSH", ("PUSH", "BRANCH"), "git"),
    "new": ("AGT", ("NEW", "AGENT"), "main"),
    "voice": ("MIC", ("START", "VOICE"), "main"),
    "voice_draft": ("MIC", ("VOICE", "DRAFT"), "main"),
    "previous": ("<", ("PREVIOUS", "TAB"), "main"),
    "next": (">", ("NEXT", "TAB"), "main"),
    "model": ("AI", ("MODEL", "PICKER"), "main"),
    "compact": ("CMP", ("COMPACT", "CONTEXT"), "main"),
    "up": ("^", ("MOVE", "UP"), "main"),
    "enter": ("OK", ("PRESS", "ENTER"), "main"),
    "down": ("v", ("MOVE", "DOWN"), "main"),
    "scroll_down": ("PG", ("SCROLL", "DOWN"), "main"),
}
COLORS = {
    "main": ((10, 30, 42, 255), (45, 226, 196, 255)),
    "prompt": ((28, 20, 52, 255), (178, 136, 255, 255)),
    "git": ((48, 24, 17, 255), (255, 151, 83, 255)),
}


def load_font(size: int) -> ImageFont.FreeTypeFont | ImageFont.ImageFont:
    for path in FONT_PATHS:
        try:
            return ImageFont.truetype(path, size)
        except OSError:
            continue
    return ImageFont.load_default()


def centered_text(draw: ImageDraw.ImageDraw, y: float, text: str, font: ImageFont.ImageFont, fill: tuple[int, ...]) -> None:
    bounds = draw.textbbox((0, 0), text, font=font)
    width = bounds[2] - bounds[0]
    draw.text(((180 - width) / 2, y - bounds[1]), text, font=font, fill=fill)


def make_icon(name: str, glyph: str, lines: tuple[str, str], category: str) -> None:
    background, accent = COLORS[category]
    image = Image.new("RGBA", (180, 180), (7, 11, 19, 255))
    draw = ImageDraw.Draw(image)
    draw.rounded_rectangle((5, 5, 175, 175), radius=32, fill=background, outline=accent, width=7)
    panel_fill = tuple(min(channel + 22, 255) for channel in background[:3]) + (255,)
    draw.rounded_rectangle((16, 15, 164, 96), radius=23, fill=panel_fill, outline=accent, width=3)
    glyph_font = load_font(50 if len(glyph) <= 2 else 38)
    centered_text(draw, 30, glyph, glyph_font, (238, 255, 252, 255))
    label_font = load_font(25 if max(map(len, lines)) <= 7 else 21)
    centered_text(draw, 108, lines[0], label_font, (255, 255, 255, 255))
    centered_text(draw, 137, lines[1], label_font, accent)
    image.resize((90, 90), Image.Resampling.LANCZOS).save(OUTPUT / f"{name}.png")


def main() -> None:
    OUTPUT.mkdir(parents=True, exist_ok=True)
    for name, (glyph, lines, category) in ICON_DEFINITIONS.items():
        make_icon(name, glyph, lines, category)


if __name__ == "__main__":
    main()
