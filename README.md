# SC2-Multi

A lightweight tool that allows you to run multiple instances of StarCraft II simultaneously.

## How It Works

StarCraft II prevents multiple instances by holding named kernel handles (Events and Sections). This tool closes those handles in all running `SC2_x64.exe` processes, allowing you to launch additional instances.

### Handles Closed
| Type | Name |
|------|------|
| Event | `StarCraft II Game Application (Global)` |
| Event | `StarCraft II Game Application` |
| Section | `StarCraft II IPC Mem` |

## Usage

1. Launch StarCraft II
2. Run **SC2-Multi.exe** (requires Administrator)
3. Click **Close SC2 Handles**
4. Launch another StarCraft II instance

## Download

**[Download SC2-Multi.exe](https://github.com/NoxRTS/SC2-Multi/releases/latest)**

Self-contained executable — no .NET installation required.

## Created by [Nox](https://github.com/NoxRTS)
