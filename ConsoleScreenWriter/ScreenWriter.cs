using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Accord.Video.FFMPEG;
using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Vision.Motion;
using ClientProcessors;
using ConsoleClient.Logger;

namespace ConsoleClient
{
    public class ScreenWriter : IScreenWriter
    {
        private readonly object _lock = new object();
        private readonly string _path;
        private readonly IProcessor _processor;
        private readonly ILogger _logger;
        private readonly MotionDetector _detector;

        private bool _recording;
        private bool _isCameraStart;
        private IVideoSource _videoSource;
        private FilterInfo _currentDevice;
        private DateTime? _firstFrameTime;

        private Bitmap _bitmap;
        public VideoFileWriter _writer;


        #region Constructor
        public ScreenWriter(IProcessor processor, ILogger logger, MotionDetector motionDetector)
        {
            _processor = processor;
            _logger = logger;
            _path = @"C:\OutOfRulesDir";
            _recording = false;
            _detector = motionDetector;
        }
        #endregion

        #region Methods
        public void Init()
        {
            CheckDirictory();
            InitDevice();
        }

        public void Start()
        {
            while (true)
            {
                StartProcess();
                Thread.Sleep(30000);
            }
        }

        public void InitDevice()
        {
            var devices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            _currentDevice = devices[0];
        }

        public void CheckDirictory()
        {
            if (!Directory.Exists(_path))
            {
                DirectoryInfo dir = Directory.CreateDirectory(_path);
                dir.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }
        }

        private void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            lock (_lock)
            {
                Thread.Sleep(40);

                try
                {
                    if (_isCameraStart)
                    {
                        using (var bitmap = (Bitmap)eventArgs.Frame.Clone())
                        {
                            _bitmap = bitmap;

                            if (_recording)
                            {
                                _detector.ProcessFrame(bitmap);

                                if (_firstFrameTime != null)
                                {
                                    _writer.WriteVideoFrame(bitmap, DateTime.Now - _firstFrameTime.Value);
                                }
                                else
                                {
                                    _writer.WriteVideoFrame(bitmap);
                                    _firstFrameTime = DateTime.Now;
                                }
                            }
                            else
                            {
                                if (_detector.ProcessFrame(bitmap) > 0.02) // нарушитель замечен
                                {
                                    _logger.Info($"Start recording process | {DateTime.Now}");
                                    _recording = true;
                                    var filename = Guid.NewGuid().ToString();

                                    CreateSnapShot(filename);
                                    CreateVideoShot(filename);
                                }
                            }

                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.Error("Error on _videoSource_NewFrame:\n" + e);
                    StopCamera();
                }
            }
        }

        public void StartProcess()
        {
            if (!_isCameraStart)
            {
                StartCamera();
            }
        }

        private void StartCamera()
        {
            if (_currentDevice != null)
            {
                lock (_lock)
                {
                    if (!_isCameraStart)
                    {
                        _videoSource = new VideoCaptureDevice(_currentDevice.MonikerString);
                        _videoSource.NewFrame += video_NewFrame;
                        _videoSource.Start();
                        _isCameraStart = true;
                    }
                }
            }
        }

        private void StopCamera()
        {
            if (_videoSource != null && _videoSource.IsRunning)
            {
                _videoSource.SignalToStop();
                _videoSource.NewFrame -= video_NewFrame;
            }
            _isCameraStart = false;
            _logger.Info($"Stop camera | {DateTime.Now}");
            GC.Collect();
        }

        public async void StartVideoTimer()
        {
            await Task.Factory.StartNew(() =>
            {
                Thread.Sleep(5200); // время записи видео 
            });

            lock (_lock)
            {
                StopRecording();
                StopCamera();
            }
        }

        private void CreateSnapShot(string filename)
        {
            var image = _bitmap.ToBitmapImage();
            image.Freeze();
            
            var path = _path + $"\\{filename}.png";
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));
            using (var filestream = new FileStream(path, FileMode.Create))
            {
                encoder.Save(filestream);
            }
            _logger.Info($"Create snapshot: \"{path}\"");

        }

        private void CreateVideoShot(string filename)
        {
            var path = _path + $"\\{filename}.avi";
            _firstFrameTime = null;
            _writer = new VideoFileWriter();
            _writer.Open(path, (int)Math.Round((decimal)_bitmap.Width, 1),
                (int)Math.Round((decimal)_bitmap.Height, 1));
            _logger.Info($"Start video recording: \"{path}\" | {DateTime.Now}");
            StartVideoTimer();
        }

        private void StopRecording()
        {
            _writer.Close();
            _writer.Dispose();
            _recording = false;
            _logger.Info($"Stop video recording | {DateTime.Now}");
            GC.Collect();
        }

        public void Dispose()
        {
            if (_videoSource != null && _videoSource.IsRunning)
            {
                _videoSource.SignalToStop();
            }
            _writer?.Dispose();
        }
        #endregion
    }
}
