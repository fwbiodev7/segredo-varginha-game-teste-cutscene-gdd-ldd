"""Read-only intake check for the 15 narrator/reporter takes; never imports audio."""
import argparse
import hashlib
import json
import math
from pathlib import Path
import shutil
import struct
import subprocess
from datetime import datetime, timezone

ROOT = Path(__file__).resolve().parents[2]
OUTPUT = ROOT / "output/recebimento-narrador-2026-10-29"
EXPECTED = ["REP_01", "REP_02"] + [f"NAR_{i:02}" for i in range(1, 14)]
EXTENSIONS = {".wav", ".mp3", ".ogg"}
IGNORED = {"readme.md", ".ds_store", "thumbs.db", "desktop.ini"}


def sha256(path):
    digest = hashlib.sha256()
    with path.open("rb") as source:
        for chunk in iter(lambda: source.read(1024 * 1024), b""):
            digest.update(chunk)
    return digest.hexdigest()


def header_check(path):
    """Reject renamed text even without ffprobe; compressed headers are not decoding."""
    with path.open("rb") as source:
        head = source.read(12)
        extension = path.suffix.lower()
        if extension == ".mp3":
            if not (head.startswith(b"ID3") or len(head) >= 2 and head[0] == 255 and head[1] & 224 == 224):
                raise ValueError("Cabeçalho MP3 ausente; texto renomeado não é áudio.")
            return {"container_header": "mp3", "validation_level": "header_only"}
        if extension == ".ogg":
            if not head.startswith(b"OggS"):
                raise ValueError("Cabeçalho Ogg ausente; texto renomeado não é áudio.")
            return {"container_header": "ogg", "validation_level": "header_only"}
        if head[:4] != b"RIFF" or head[8:12] != b"WAVE":
            # RF64 can be inspected by ffprobe; no false promise of WAV parsing.
            if head[:4] == b"RF64" and head[8:12] == b"WAVE":
                return {"container_header": "rf64", "validation_level": "header_only"}
            raise ValueError("Cabeçalho RIFF/WAVE ausente; texto renomeado não é áudio.")
        size, metadata, data_bytes = path.stat().st_size, None, None
        while source.tell() + 8 <= size:
            chunk, length = struct.unpack("<4sI", source.read(8))
            start = source.tell()
            if start + length > size:
                raise ValueError("WAV truncado: tamanho do chunk excede o arquivo.")
            if chunk == b"fmt " and length >= 16:
                tag, channels, rate, _, align, bits = struct.unpack("<HHIIHH", source.read(16))
                metadata = {"codec_tag": tag, "channels": channels, "sample_rate": rate,
                            "block_align": align, "bits_per_sample": bits}
            if chunk == b"data":
                data_bytes = length
            source.seek(start + length + length % 2)
        if not metadata or not data_bytes or min(metadata["channels"], metadata["sample_rate"], metadata["block_align"]) <= 0:
            raise ValueError("WAV sem formato válido ou sem amostras de áudio.")
        metadata.update(container_header="wav", validation_level="wav_structure",
                        duration_seconds=data_bytes / metadata["block_align"] / metadata["sample_rate"])
        return metadata


def probe_audio(executable, path):
    command = [executable, "-v", "error", "-show_entries",
               "format=duration,format_name:stream=codec_type,codec_name,sample_rate,channels,duration",
               "-of", "json", str(path)]
    result = subprocess.run(command, capture_output=True, text=True, encoding="utf-8", errors="replace", timeout=30)
    if result.returncode:
        raise ValueError("ffprobe rejeitou o arquivo: " + result.stderr.strip()[:600])
    data = json.loads(result.stdout)
    audio = [stream for stream in data.get("streams", []) if stream.get("codec_type") == "audio"]
    if not audio:
        raise ValueError("ffprobe não encontrou stream de áudio.")
    duration = data.get("format", {}).get("duration") or audio[0].get("duration")
    seconds = float(duration)
    if not math.isfinite(seconds) or seconds <= 0:
        raise ValueError("Duração de áudio inválida ou nula.")
    return {"validation_level": "ffprobe_metadata", "duration_seconds": seconds,
            "sample_rate": int(audio[0].get("sample_rate", 0)), "channels": audio[0].get("channels"),
            "codec": audio[0].get("codec_name"), "probe": data}


def inspect(input_dir, executable):
    report = {"generated_utc": datetime.now(timezone.utc).isoformat(), "input_dir": str(input_dir),
              "expected_ids": EXPECTED, "ffprobe": executable, "files": [], "errors": [], "warnings": [],
              "missing_ids": [], "duplicate_ids": {}, "ignored_files": [], "audio_received_confirmed": False,
              "scope": "Inventário, integridade e metadados. Escuta, texto, mix e sincronização exigem revisão humana; nenhuma importação foi executada."}
    inventory = {identifier: [] for identifier in EXPECTED}
    if not input_dir.is_dir():
        report["errors"].append("Pasta de entrada inexistente ou não é diretório.")
    else:
        for path in sorted(input_dir.rglob("*")):
            if not path.is_file():
                continue
            if path.name.lower() in IGNORED:
                report["ignored_files"].append(str(path.relative_to(input_dir)))
                continue
            entry = {"path": str(path.resolve()), "relative_path": str(path.relative_to(input_dir)),
                     "id": path.stem.upper(), "extension": path.suffix.lower(), "errors": [], "warnings": []}
            report["files"].append(entry)
            if entry["id"] not in inventory:
                entry["errors"].append("ID inesperado: use somente ID.ext, sem sufixo de take.")
            else:
                inventory[entry["id"]].append(entry["relative_path"])
            if entry["extension"] not in EXTENSIONS:
                entry["errors"].append("Extensão não aceita; use WAV, MP3 ou OGG.")
            try:
                entry["bytes"] = path.stat().st_size
                entry["sha256"] = sha256(path)
                if not entry["bytes"]:
                    entry["errors"].append("Arquivo vazio.")
                if not entry["errors"]:
                    entry["audio"] = header_check(path)
                    if executable:
                        entry["audio"].update(probe_audio(executable, path))
                    else:
                        entry["warnings"].append("ffprobe indisponível: estrutura/cabeçalho inspecionado, decodificação não confirmada.")
                    if entry["audio"].get("channels") not in (None, 1):
                        entry["warnings"].append("Mono é preferível; ouvir o arquivo antes de qualquer conversão.")
                    if entry["audio"].get("sample_rate") not in (None, 48000):
                        entry["warnings"].append("48 kHz é preferível; outra taxa não exige nova gravação se a voz estiver boa.")
            except (OSError, ValueError, TypeError, struct.error, subprocess.SubprocessError) as error:
                entry["errors"].append(str(error))
            report["errors"].extend(f"{entry['relative_path']}: {error}" for error in entry["errors"])
    report["missing_ids"] = [identifier for identifier, paths in inventory.items() if not paths]
    report["duplicate_ids"] = {identifier: paths for identifier, paths in inventory.items() if len(paths) > 1}
    if report["missing_ids"]:
        report["errors"].append("IDs faltantes: " + ", ".join(report["missing_ids"]))
    if report["duplicate_ids"]:
        report["errors"].append("IDs duplicados: " + ", ".join(report["duplicate_ids"]))
    if not executable:
        report["warnings"].append("ffprobe ausente; isso não reprova sozinho o lote. A inspeção de áudio permanece limitada.")
    report["success"] = not report["errors"]
    report["status"] = "inventario_aprovado_escuta_pendente" if report["success"] else "pendente_corrigir_lote"
    report["summary"] = {"expected": len(EXPECTED), "files": len(report["files"]), "missing": len(report["missing_ids"]),
                         "duplicates": len(report["duplicate_ids"]), "errors": len(report["errors"]),
                         "ffprobe_metadata_verified": sum(entry.get("audio", {}).get("validation_level") == "ffprobe_metadata" for entry in report["files"])}
    return report


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--inputdir", type=Path, required=True, help="Pasta com os 15 takes escolhidos; busca recursiva.")
    parser.add_argument("--ffprobe", help="Executável ffprobe opcional; se omitido, procura no PATH.")
    parser.add_argument("--report", type=Path, help="JSON de saída fora da pasta de entrada; padrão em output/recebimento-narrador-2026-10-29.")
    args = parser.parse_args()
    input_dir = args.inputdir.resolve()
    executable = shutil.which(args.ffprobe or "ffprobe")
    report = inspect(input_dir, executable)
    if args.ffprobe and not executable:
        report["warnings"].append("ffprobe solicitado não foi encontrado; usada inspeção de cabeçalho/estrutura.")
    stamp = datetime.now(timezone.utc).strftime("%Y%m%dT%H%M%S%fZ")
    destination = (args.report or OUTPUT / ("validation-" + stamp + ".json")).resolve()
    if destination == input_dir or input_dir in destination.parents:
        parser.error("O relatório deve ficar fora da pasta de entrada para preservar os arquivos recebidos.")
    destination.parent.mkdir(parents=True, exist_ok=True)
    # Exclusive creation protects an existing report or any other file at this path.
    with destination.open("x", encoding="utf-8") as target:
        json.dump(report, target, ensure_ascii=False, indent=2)
        target.write("\n")
    print(json.dumps({"success": report["success"], "summary": report["summary"], "report": str(destination)}, ensure_ascii=False))
    return 0 if report["success"] else 1


if __name__ == "__main__":
    raise SystemExit(main())
