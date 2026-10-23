# Generator Fuel Rate

Shows how many fuel items per minute a fuel-burning power plant burns, right in the plant's info window.

Click any fuel-burning plant, such as the Thermal or Fuel Cell Power Plant, the Mini Fusion or Fission Power Station, the Artificial Star or the GenesisBook Artificial Photonix. A new line appears under the fuel efficiency text:

```text
12.5 / min  (max 18)
```

- **First number:** fuel burned per minute at the plant's current output. It drops when the plant runs below full load.
- **max:** fuel per minute at full load.

## How the numbers are calculated

The values use the game's own fuel drain formula (`PowerGeneratorComponent.GenEnergyByFuel`) and the plant's live output. Proliferator bonuses, the antimatter-rod doubling and GenesisBook's reactor and planet-focus bonuses are all included.

## Settings

In `BepInEx/config/bug.dsp.generatorfuelrate.cfg`, or in your mod manager's config editor:

- `ShowCurrent`: show the current use (default on)
- `ShowMax`: show the full-load use (default on)
- `Language`: `English` (default), `Chinese` or `Turkish`

## Compatibility

- **Works with or without GenesisBook.**
- **Display only.** The mod adds one text line to the power generator window and changes nothing else.

## Building from source

You need a current .NET SDK and the .NET Framework 4.7.2 targeting pack. The game, Unity and BepInEx assemblies are referenced from your own installation and are not part of this repository.

```text
dotnet build -c Release "-p:DSPGamePath=D:\Games\Dyson Sphere Program" "-p:BepInExCorePath=C:\path\to\BepInEx\core"
```

The output is `bin/Release/net472/GeneratorFuelRate.dll`. Copy it to `BepInEx/plugins/GeneratorFuelRate/`.

## License

MIT, see [LICENSE](LICENSE).
