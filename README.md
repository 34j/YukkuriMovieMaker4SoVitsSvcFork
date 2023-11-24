# YMM4用 so-vits-svc-fork 音声エフェクト

## 説明

- 音声をボイスチェンジャーに通す音声エフェクトのプラグインです。
- 一応動作しますが、非常に動作が重く、細切れで変換されるためクオリティがやや低下しています。

## インストール

- [`so-vits-svc-fork`](https://github.com/voicepaw/so-vits-svc-fork)をインストールしてください。

```shell
winget install --id=Python.Python.3.10 -e
py -3 -m pip install --user git+https://github.com/pypa/pipx.git
py -3 -m pipx ensurepath
pipx install so-vits-svc-fork --python=3.10
# pipx inject so-vits-svc-fork torch torchaudio --pip-args="--upgrade" --index-url=https://download.pytorch.org/whl/cu118 # https://download.pytorch.org/whl/nightly/cu121
```

- Dllを`user/plugin`に配置してください。

## 使い方

- `so-vits-svc 4.0`に対応している好きなモデルを用意し、音声エフェクトの画面からモデルとコンフィグファイルが含まれているフォルダを指定してください。
