using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.DirectoryServices;
using System.IO;
using NAudio.Dmo;
using NAudio.Wave;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Exo;
using YukkuriMovieMaker.Player.Audio.Effects;
using YukkuriMovieMaker.Plugin.Effects;

namespace SoVitsSvcForkEffect
{
    /// <summary>
    /// 音声エフェクト
    /// 音声エフェクトには必ず[AudioEffect]属性を設定してください。
    /// </summary>
    [AudioEffect("ボイスチェンジャー", new[] { "ボイスチェンジャー" }, new string[] { })]
    public class SoVitsSvcForkAudioEffect : AudioEffectBase
    {
        /// <summary>
        /// エフェクトの名前
        /// </summary>
        public override string Label => "ボイスチェンジャー";



        /// <summary>
        /// フォルダ選択コントロール
        /// </summary>
        [Display(GroupName = "パス", Name = "フォルダ", Description = "モデルとコンフィグが含まれているフォルダ")]
        [DirectorySelector]
        public string Directory { get => directory; set => Set(ref directory, value); }
        string directory = "";

        /// <summary>
        /// bool値をトグルするコントロール
        /// </summary>
        [Display(GroupName = "デバッグ", Name = "バイパス", Description = "変換しない")]
        [ToggleSlider]
        public bool Bypass { get => bypass; set => Set(ref bypass, value); }
        bool bypass = false;

        /// <summary>
        /// Return the latest *.pth file in the directory
        /// </summary>
        public string ModelPath => Directory == "" ? "" : System.IO.Directory.GetFiles(Directory, "*.pth").FirstOrDefault("");

        public string ConfigPath => Directory == "" ? "" : Path.Combine(Directory, "config.json");

        /// <summary>
        /// 音声エフェクトを作成する
        /// </summary>
        /// <param name="duration">音声エフェクトの長さ</param>
        /// <returns>音声エフェクト</returns>
        public override IAudioEffectProcessor CreateAudioEffect(TimeSpan duration)
        {
            return new SampleAudioEffectProcessor(this, duration);
        }

        /// <summary>
        /// ExoFilterを作成する
        /// </summary>
        /// <param name="keyFrameIndex">キーフレーム番号</param>
        /// <param name="exoOutputDescription">exo出力に必要な各種項目</param>
        /// <returns>exoフィルタ</returns>
        public override IEnumerable<string> CreateExoAudioFilters(int keyFrameIndex, ExoOutputDescription exoOutputDescription)
        {
            return new[] { $"so-vits-svc-fork({this.Directory})" };
        }

        /// <summary>
        /// IAnimatableを実装するプロパティを返す
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<IAnimatable> GetAnimatables() => Array.Empty<IAnimatable>();
    }
}
