# Animated buttons: the three-key Starlight workflow

![Three pulsing star keys](../assets/animated-stars-preview.gif)

Logi Options+ normally assigns one static image to an action. The Starlight workflow creates animation by exposing a custom plugin action that returns a different PNG frame every 100 milliseconds and tells Logitech that its image changed.

The animation spans three adjacent keys by treating each key as one synchronized panel:

```text
Starlight Left | Starlight Center | Starlight Right
```

All three commands calculate their frame number from the same system clock, so they stay visually synchronized without communicating with one another.

## Build and install it

The repository's normal build and installer include Starlight:

```bash
./scripts/build.sh
./scripts/install-plugins.sh
```

Restart Logi Options+. Search the action browser for **Starlight**, then place **Starlight Left**, **Starlight Center**, and **Starlight Right** beside one another in that order.

## How it works

### 1. Generate frame sequences

Run:

```bash
python3 src/starlight/generate_starlight_frames.py
```

This produces 24 PNGs per panel under `src/starlight/images/frames/`, named so alphabetical ordering is also animation ordering:

```text
left-00.png   center-00.png   right-00.png
left-01.png   center-01.png   right-01.png
...
```

The generator also refreshes the animated GIF preview shown above. Replace `draw_frame()` with your own Pillow drawing code to make a different animation.

### 2. Embed the PNGs in the plugin

`MatrixRainPlugin.csproj` contains:

```xml
<EmbeddedResource Include="images/frames/*.png" />
```

Embedding keeps the installed plugin self-contained and lets each command load its panel's frames directly from the assembly.

### 3. Render edge-to-edge

The command calls `SetWidget(true)`, and `GetCommandDisplayName()` returns `null`. Both details matter: a normal command or even a blank text label causes Logi Options+ to reserve label space and shrink the artwork.

### 4. Advance the animation

Each command starts a `System.Threading.Timer`. Every 100 ms it calls `ActionImageChanged()`. Logitech then calls `GetCommandImage()`, which selects the current frame using `Environment.TickCount64`.

At 100 ms per frame, the animation runs at 10 frames per second. This is smooth enough for a key display without requesting excessive redraws.

## Make your own animation

Keep these constraints:

- Export one 90×90 PNG sequence per physical key.
- Give every panel the same number of frames.
- Use zero-padded frame numbers (`00`, `01`, …) so resource sorting is stable.
- Derive the frame index from a shared clock to keep multiple keys synchronized.
- Return `null` for the display name when you want full-key artwork.
- Keep the timer alive for the lifetime of the command.
- Catch redraw errors because Logitech can detach actions briefly while changing profiles.

For a single-key animation, keep only one panel command and one frame sequence. For a larger strip, add more panel subclasses and extend the generator's `PANELS` tuple.

## Performance tips

- Start around 8–12 FPS; faster refreshes are rarely useful on a keypad display.
- Load image bytes once in the constructor instead of reading files every frame.
- Keep frames small and optimized.
- Avoid one timer per frame or background processes that never terminate.
