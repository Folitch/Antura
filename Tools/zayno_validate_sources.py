#!/usr/bin/env python3
import argparse
import json
from pathlib import Path

def count_records(path: Path) -> int:
    if not path.exists():
        return -1
    return sum(1 for line in path.read_text(encoding="utf-8", errors="ignore").splitlines() if line.lstrip().startswith("- _Id:"))

def audio_count(root: Path) -> int:
    extensions = {".mp3", ".ogg", ".wav"}
    return sum(1 for path in root.rglob("*") if path.is_file() and path.suffix.lower() in extensions)

def inspect_current(root: Path):
    base = root / "Assets/_config/content_Arabic"
    return {
        "learning_blocks": count_records(base / "DB Arabic_LearningBlock.asset"),
        "play_sessions": count_records(base / "DB Arabic_PlaySession.asset"),
        "letter_entries": count_records(base / "DB Arabic_Letter.asset"),
        "word_entries": count_records(base / "DB Arabic_Word.asset"),
        "phrase_entries": count_records(base / "DB Arabic_Phrase.asset"),
        "audio_files": audio_count(root / "Assets"),
    }

def inspect_historical(root: Path):
    base = root / "Assets/Resources/Database"
    return {
        "learning_blocks": count_records(base / "Database_LearningBlock.asset"),
        "play_sessions": count_records(base / "Database_PlaySession.asset"),
        "letter_entries": count_records(base / "Database_Letter.asset"),
        "word_entries": count_records(base / "Database_Word.asset"),
        "phrase_entries": count_records(base / "Database_Phrase.asset"),
        "audio_files": audio_count(root / "Assets/_app/Audio/Resources/AudioArabic"),
    }

def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--current", type=Path, required=True)
    parser.add_argument("--historical", type=Path, required=True)
    parser.add_argument("--output", type=Path, default=Path("zayno-source-audit.json"))
    args = parser.parse_args()
    report = {"current_multiedition": inspect_current(args.current), "historical_arabic": inspect_historical(args.historical)}
    report["differences"] = {key: report["current_multiedition"][key] - report["historical_arabic"][key] for key in report["current_multiedition"]}
    args.output.write_text(json.dumps(report, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    print(json.dumps(report, ensure_ascii=False, indent=2))
    required = [("current learning blocks", report["current_multiedition"]["learning_blocks"]), ("current play sessions", report["current_multiedition"]["play_sessions"]), ("current letters", report["current_multiedition"]["letter_entries"]), ("current words", report["current_multiedition"]["word_entries"]), ("historical phrases", report["historical_arabic"]["phrase_entries"]), ("historical Arabic audio", report["historical_arabic"]["audio_files"])]
    missing = [name for name, value in required if value <= 0]
    if missing:
        raise SystemExit("Missing required Antura content: " + ", ".join(missing))

if __name__ == "__main__":
    main()