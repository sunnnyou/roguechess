import fontforge
import os
from PIL import Image, ImageDraw, ImageFont

# Settings
font_path = "PlayfairDisplay-Bold.ttf"
output_font_path = "pixelated_font.ttf"
pixel_size = 8  # Size to downscale to for pixelation
image_size = 64  # Canvas size for each glyph
characters = [chr(c) for c in range(32, 127)]  # Basic ASCII

# Create temp folder
os.makedirs("glyphs", exist_ok=True)

# Load font via Pillow
font = ImageFont.truetype(font_path, image_size)

# Generate pixelated glyph images
for char in characters:
    img = Image.new("L", (image_size, image_size), color=255)
    draw = ImageDraw.Draw(img)
    w, h = draw.textsize(char, font=font)
    draw.text(((image_size - w) / 2, (image_size - h) / 2), char, font=font, fill=0)

    # Pixelate
    img = img.resize((pixel_size, pixel_size), Image.NEAREST)
    img = img.resize((image_size, image_size), Image.NEAREST)

    img.save(f"glyphs/{ord(char)}.png")

# Create new font using fontforge
new_font = fontforge.font()
new_font.encoding = "UnicodeFull"
new_font.fontname = "PixelatedFont"
new_font.familyname = "Pixelated Font"
new_font.fullname = "Pixelated Font"

for char in characters:
    img_path = f"glyphs/{ord(char)}.png"
    if not os.path.exists(img_path):
        continue

    glyph = new_font.createChar(ord(char))
    glyph.importOutlines(img_path)
    glyph.autoTrace()
    glyph.simplify()
    glyph.correctDirection()

# Generate the final TTF file
new_font.generate(output_font_path)
new_font.close()

print(f"Pixelated font saved to {output_font_path}")
