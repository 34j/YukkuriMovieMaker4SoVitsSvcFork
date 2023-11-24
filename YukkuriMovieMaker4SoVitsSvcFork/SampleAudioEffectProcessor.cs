using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.DirectoryServices;
using System.IO;
using NAudio.Dmo;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using Windows.Media;
using YukkuriMovieMaker.Commons;
using YukkuriMovieMaker.Controls;
using YukkuriMovieMaker.Exo;
using YukkuriMovieMaker.Player.Audio.Effects;
using YukkuriMovieMaker.Plugin.Effects;

namespace SoVitsSvcForkEffect
{
    internal class SampleAudioEffectProcessor : AudioEffectProcessorBase
    {
        readonly SoVitsSvcForkAudioEffect item;
        readonly TimeSpan duration;

        //出力サンプリングレート。リサンプリング処理をしない場合はInputのHzをそのまま返す。
        public override int Hz => Input?.Hz ?? 0;

        //出力するサンプル数
        public override long Duration => (long)(duration.TotalSeconds * this.Hz);

        public SampleAudioEffectProcessor(SoVitsSvcForkAudioEffect item, TimeSpan duration)
        {
            this.item = item;
            this.duration = duration;
        }

        //シーク処理
        protected override void seek(long position)
        {
            Input?.Seek(position);
        }

        //エフェクトを適用する
        protected override int read(float[] destBuffer, int offset, int count)
        {
            // read Input
            Input?.Read(destBuffer, offset, count);

            // write destBuffer
            string tempInFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".wav");
            WaveFormat waveFormat = new(Input?.Hz ?? 0, 2);
            using (var writer = new WaveFileWriter(tempInFile, waveFormat))
            {
                writer.WriteSamples(destBuffer, 0, destBuffer.Length);
            }

            // convert to mono
            //using (var reader = new AudioFileReader(tempInFile))
            //{
            //    var mono = new StereoToMonoSampleProvider(reader);
            //    WaveFileWriter.CreateWaveFile16(tempInFile, mono);
            //}

            // call so-vits-svc-fork from command line
            string tempOutFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".wav");
            if (false && this.item.Bypass)
            {
                File.Copy(tempInFile, tempOutFile);
            }
            else
            {
                using (var process = new Process())
                {

                    process.StartInfo.FileName = "svc.exe";
                    process.StartInfo.Arguments = $"infer -o {tempOutFile} {tempInFile} -m {this.item.ModelPath} -c {this.item.ConfigPath}";
                    process.StartInfo.CreateNoWindow = false;
                    process.StartInfo.UseShellExecute = false;
                    process.Start();
                    process.WaitForExit();
                }
            }

            // read destBuffer as float[]
            using (var reader = new AudioFileReader(tempOutFile))
            {
                var stereo = new MonoToStereoSampleProvider(reader);
                int bytesRead = stereo.Read(destBuffer, 0, destBuffer.Length);
                return count;
            }
        }
    }
}
